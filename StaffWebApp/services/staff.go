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

func LoadStaffPage(ctx context.Context, page int64, searchTerm string) (*models.Page[models.StaffShort], error) {
	params := url.Values{}
	if page == 0 {
		page = 1
	}

	params.Set("page", strconv.FormatInt(page, 10))
	params.Set("searchPattern", searchTerm)

	var staffPage models.Page[models.StaffShort]
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.UserApiUrl+"staff?"+params.Encode(),
		nil,
		&staffPage,
	)

	return &staffPage, err
}

func CreateNewStaffProfile(ctx context.Context, username, password string) error {
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
		config.Default.UserApiUrl+"staff/register",
		bytes.NewReader(output),
		nil,
	)
	if err != nil {
		return fmt.Errorf("register new staff profile request failed: %v", err)
	}

	return nil
}

func BlockStaffProfile(ctx context.Context, userId string) error {
	requestUrl, err := url.JoinPath(config.Default.UserApiUrl, "staff/", userId)
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
		return fmt.Errorf("delete staff profile requeset failed: %v", err)
	}

	return nil
}
