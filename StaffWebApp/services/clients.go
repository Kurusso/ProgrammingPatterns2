package services

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/models"
	"strconv"
)

func LoadClientsPage(
	ctx context.Context,
	searchTerm string,
	page int,
) (*models.Page[models.ClientShort], error) {

	params := url.Values{}
	if page == 0 {
		page = 1
	}

	params.Set("page", strconv.Itoa(page))
	params.Set("searchPattern", searchTerm)

	var clientsPage models.Page[models.ClientShort]
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.UserApiUrl+"clients?"+params.Encode(),
		nil,
		&clientsPage,
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

func LoadAccountOperationHistory(ctx context.Context, accountId string) (*models.AccountDetailed, error) {
	requestUrl, err := url.JoinPath(config.Default.CoreApiUrl, "/Account/GetInfo/", accountId)
	if err != nil {
		return nil, fmt.Errorf("failed to make request url: %v", err)
	}

	var account models.AccountDetailed
	err = makeRequestParseBody(
		ctx,
		http.MethodGet,
		requestUrl,
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
