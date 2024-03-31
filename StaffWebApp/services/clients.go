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

func CreateClientProfile(ctx context.Context, username, password string) error {
	body := map[string]string{
		"username": username,
		"password": password,
	}
	output, err := json.Marshal(body)
	if err != nil {
		return fmt.Errorf("body encoding failed: %v", err)
	}
	err = makeRequestParseBody(
		ctx,
		http.MethodPost,
		config.Default.UserApiUrl+"clients/register",
		bytes.NewReader(output),
		nil,
	)
	if err != nil {
		return fmt.Errorf("register new staff profile request failed: %v", err)
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

func BlockClientProfile(ctx context.Context, userId string) error {
	requestUrl, err := url.JoinPath(config.Default.UserApiUrl, "clients/", userId)
	if err != nil {
		return fmt.Errorf("invalid url: %v", err)
	}

	err = makeRequestParseBody(
		ctx,
		http.MethodDelete,
		requestUrl,
		nil,
		nil,
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

	pingTimer := time.NewTicker(30 * time.Second)
	defer pingTimer.Stop()

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

			err = clients.OperationList(account).Render(r.Context(), wsWritter)
			if err != nil {
				logger.Default.Error("failed to write html template to ws: ", err)
				return
			}

			err = wsWritter.Close()
			if err != nil {
				logger.Default.Error("error during wsWritter close: ", err)
			}

		case <-pingTimer.C:
			err = wsConn.WriteMessage(websocket.PingMessage, nil)
			if err != nil {
				logger.Default.Error("ws ping failed")
				return
			}
		}

	}
}

func WsLoadAccountOperations(
	r *http.Request,
	updates chan *models.AccountDetailed,
	clientQuit chan bool,
) {
	defer close(updates)

}
