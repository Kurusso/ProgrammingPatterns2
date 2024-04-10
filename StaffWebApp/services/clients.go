package services

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"staff-web-app/components/clients"
	"staff-web-app/config"
	"staff-web-app/logger"
	"staff-web-app/models"
	"strconv"
	"time"

	"github.com/gorilla/websocket"
)

func LoadClientsPage(
	ctx context.Context,
	searchTerm string,
	page int,
	sessionId string,
) (*models.Page[models.ClientShort], error) {

	params := url.Values{}
	if page == 0 {
		page = 1
	}

	params.Set("page", strconv.Itoa(page))
	params.Set("searchPattern", searchTerm)

	var clientsPage models.Page[models.ClientShort]
	err := makeRequestParseBodyWithHeaders(
		ctx,
		http.MethodGet,
		config.Default.UserApiUrl+"clients?"+params.Encode(),
		nil,
		&clientsPage,
		makeAccessTokenHeader(ctx, sessionId),
	)
	return &clientsPage, err
}

func CreateClientProfile(ctx context.Context, username, password, sessionId string) error {
	body := map[string]string{
		"username": username,
		"password": password,
	}
	output, err := json.Marshal(body)
	if err != nil {
		return fmt.Errorf("body encoding failed: %v", err)
	}
	err = makeRequestParseBodyWithHeaders(
		ctx,
		http.MethodPost,
		config.Default.UserApiUrl+"clients/register",
		bytes.NewReader(output),
		nil,
		makeAccessTokenHeader(ctx, sessionId),
	)
	if err != nil {
		return fmt.Errorf("register new client profile request failed: %v", err)
	}

	return nil
}

func LoadUserAccounts(ctx context.Context, userId string) ([]models.AccountShort, error) {
	requestUrl, err := url.JoinPath(config.Default.CoreApiUrl, "/Account/User/", userId)
	if err != nil {
		return nil, fmt.Errorf("failed to make request url: %v", err)
	}

	var accounts []models.AccountShort
	err = makeRequestParseBody(
		ctx,
		http.MethodGet,
		requestUrl,
		nil,
		&accounts,
	)
	return accounts, err
}

func LoadAccountOperationHistory(ctx context.Context, accountId string, userId string) (*models.AccountDetailed, error) {
	requestUrl, err := url.JoinPath(config.Default.CoreApiUrl, "/Account/GetInfo/", accountId)
	if err != nil {
		return nil, fmt.Errorf("failed to make request url: %v", err)
	}

	queries := url.Values{}
	queries.Set("userId", userId)

	var account models.AccountDetailed
	err = makeRequestParseBody(
		ctx,
		http.MethodGet,
		requestUrl+"?"+queries.Encode(),
		nil,
		&account,
	)
	return &account, err
}

func BlockClientProfile(ctx context.Context, userId, sessionId string) error {
	requestUrl, err := url.JoinPath(config.Default.UserApiUrl, "clients/", userId)
	if err != nil {
		return fmt.Errorf("invalid url: %v", err)
	}

	err = makeRequestParseBodyWithHeaders(
		ctx,
		http.MethodDelete,
		requestUrl,
		nil,
		nil,
		makeAccessTokenHeader(ctx, sessionId),
	)
	if err != nil {
		return fmt.Errorf("block client profile requeset failed: %v", err)
	}

	return nil
}

func WsUpdateAccountOperations(
	w http.ResponseWriter,
	r *http.Request,
	updates chan *models.AccountDetailed,
	clientQuit chan bool,
) {
	defer close(clientQuit)

	var wsUpgrader = websocket.Upgrader{}
	wsConn, err := wsUpgrader.Upgrade(w, r, nil)
	if err != nil {
		logger.Default.Error("failed to upgrade http connection to ws")
		return
	}
	defer wsConn.Close()

	answCh := make(chan wsAnswer)
	go readWsMessage(wsConn, answCh)

	for {
		select {
		case account, ok := <-updates:
			if !ok {
				return
			}

			wsWritter, err := wsConn.NextWriter(websocket.TextMessage)
			if err != nil {
				logger.Default.Error("failed to create wsWriter")
				return
			}
			logger.Default.Info("sending new account info")
			account.SortOperationsByDate()
			err = clients.OperationList(account).Render(r.Context(), wsWritter)
			if err != nil {
				logger.Default.Error("failed to write html template to ws: ", err)
				return
			}

			err = wsWritter.Close()
			if err != nil {
				logger.Default.Error("error during wsWritter close: ", err)
			}

		case _, ok := <-answCh:
			if !ok {
				return
			}
		}

	}
}

type wsAnswer struct {
	message     []byte
	messageType int
}

func wsSendHeartBeat(wsConn *websocket.Conn) {
	for i := 0; ; i++ {
		err := wsConn.WriteMessage(websocket.TextMessage, []byte(strconv.Itoa(i)))
		if err != nil {
			return
		}
		time.Sleep(5 * time.Second)
	}
}

func WsLoadAccountOperations(
	r *http.Request,
	userId string,
	accountId string,
	updates chan *models.AccountDetailed,
	clientQuit chan bool,
) {
	defer close(updates)
	logger.Default.Info("new ws connection for ", accountId)
	defer logger.Default.Info("ws connection closed")

	queryParams := url.Values{}
	queryParams.Add("userId", userId)
	requestUrl := config.Default.CoreWsUrl + "?" + queryParams.Encode()

	wsConn, _, err := websocket.DefaultDialer.DialContext(r.Context(), requestUrl, nil)
	if err != nil {
		logger.Default.Error("failed to connect to websocket: ", err)
		return
	}
	defer func() {
		wsConn.WriteMessage(
			websocket.CloseMessage,
			websocket.FormatCloseMessage(websocket.CloseNormalClosure, ""),
		)
		time.Sleep(3 * time.Second)
		wsConn.Close()
	}()

	wsConn.SetPongHandler(func(appData string) error {
		wsConn.WriteControl(websocket.PongMessage, []byte{}, time.Now().Add(35*time.Second))
		logger.Default.Debug("PONG")
		return nil
	})

	msgCh := make(chan wsAnswer)
	go readWsMessage(wsConn, msgCh)
	go wsSendHeartBeat(wsConn)

	account, err := LoadAccountOperationHistory(r.Context(), accountId, userId)
	if err != nil {
		logger.Default.Error("failed to load accounts details: ", err)
		return
	}
	updates <- account

	for {
		select {
		case msg, ok := <-msgCh:
			if !ok {
				logger.Default.Info("msg channel closed")
				return
			}
			var accountUpdate models.AccountDetailed
			err = json.Unmarshal(msg.message, &accountUpdate)
			if err != nil {
				logger.Default.Error("failed to unmarshal ws message: ", err)
			} else if accountUpdate.Id == accountId {
				updates <- &accountUpdate
			}
		case <-clientQuit:
			logger.Default.Info("client quit, closing socket")
			return
		}
	}
}

func readWsMessage(wsConn *websocket.Conn, msgCh chan<- wsAnswer) {
	for {
		t, msg, err := wsConn.ReadMessage()
		if t == websocket.CloseMessage || err != nil {
			logger.Default.Info("closing websocket: reason - other party")
			close(msgCh)
			return
		}

		msgCh <- wsAnswer{message: msg, messageType: t}
	}

}
