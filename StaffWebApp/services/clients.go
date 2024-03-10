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
