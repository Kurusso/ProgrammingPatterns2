package services

import (
	"context"
	"fmt"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/logger"
	"strings"
)

const redirectCallback = "http://localhost:8080/login/callback"

func MakeOauth2AuthUrl() string {
	requestUrl, _ := url.JoinPath(config.Default.AuthApiUrl, "/auth")
	queryParams := url.Values{}
	queryParams.Set("client_id", "StaffApplication")
	queryParams.Set("response_type", "code")
	queryParams.Set("redirect_uri", redirectCallback)
	return requestUrl + "?" + queryParams.Encode()
}

func RetrieveOauth2Token(ctx context.Context, code string) (string, error) {
	requestUrl, _ := url.JoinPath(config.Default.AuthApiUrl, "/auth/token")

	var value map[string]any
	var headers = map[string]string{
		"Content-Type":  "application/x-www-form-urlencoded",
		"Accept":        "application/x-www-form-urlencoded, application/json",
		"Authorization": "Basic U3RhZmZBcHBsaWNhdGlvbjo5MDE1NjRBNS1FN0ZFLTQyQ0ItQjEwRC02MUVGNkE4RjM2NTY=",
	}

	queryParams := url.Values{}
	queryParams.Add("grant_type", "authorization_code")
	queryParams.Add("code", code)
	queryParams.Add("redirect_uri", redirectCallback)

	buffer := strings.NewReader(queryParams.Encode())
	logger.Default.Debug(requestUrl)
	logger.Default.Debug(buffer)
	err := makeRequestParseBodyWithHeaders(
		ctx,
		"POST",
		requestUrl,
		buffer,
		&value,
		headers,
	)
	logger.Default.Info(value)
	if err != nil {
		return "", err
	}
	token, ok := value["access_token"]
	if !ok {
		return "", fmt.Errorf("failed to get access_token")
	}
	return token.(string), err

}
