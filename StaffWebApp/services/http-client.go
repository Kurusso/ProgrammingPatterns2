package services

import (
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"staff-web-app/logger"
)

var HttpClient *http.Client

func makeRequestParseBody(ctx context.Context, method string, url string, body io.Reader, value any) error {
	request, err := http.NewRequestWithContext(
		ctx,
		method,
		url,
		body,
	)

	if err != nil {
		return fmt.Errorf("failed to create request: %v", err)
	}

	if body != nil {
		request.Header.Add("Content-Type", "application/json")
	}

	resp, err := http.DefaultClient.Do(request)
	if err != nil {
		return fmt.Errorf("failed to make request: %v", err)
	}

	if resp.StatusCode >= 300 {
		respBody, _ := io.ReadAll(resp.Body)
		return fmt.Errorf("got invalid status code (%d) from service: %v", resp.StatusCode, string(respBody))
	}

	if value != nil {
		err = json.NewDecoder(resp.Body).Decode(value)
		if err != nil {
			return fmt.Errorf("failed to parse response body: %v", err)
		}
	}

	return nil
}

func makeRequestParseBodyWithHeaders(ctx context.Context, method string, url string, body io.Reader, value any, headers map[string]string) error {
	request, err := http.NewRequestWithContext(
		ctx,
		method,
		url,
		body,
	)

	if err != nil {
		return fmt.Errorf("failed to create request: %v", err)
	}

	for key, value := range headers {
		request.Header.Set(key, value)
	}
	logger.Default.Info(request)
	resp, err := http.DefaultClient.Do(request)
	if err != nil {
		return fmt.Errorf("failed to make request: %v", err)
	}

	if resp.StatusCode >= 300 {
		respBody, _ := io.ReadAll(resp.Body)
		return fmt.Errorf("got invalid status code (%d) from service: %v", resp.StatusCode, string(respBody))
	}

	if value != nil {
		err = json.NewDecoder(resp.Body).Decode(value)
		if err != nil {
			return fmt.Errorf("failed to parse response body: %v", err)
		}
	}

	return nil
}

func makeRequestWithHeaders(ctx context.Context, method string, url string, headers map[string]string) (string, error) {
	request, err := http.NewRequestWithContext(
		ctx,
		method,
		url,
		nil,
	)

	if err != nil {
		return "", fmt.Errorf("failed to create request: %v", err)
	}

	for key, value := range headers {
		request.Header.Set(key, value)
	}
	logger.Default.Info(request)
	resp, err := http.DefaultClient.Do(request)
	if err != nil {
		return "", fmt.Errorf("failed to make request: %v", err)
	}

	if resp.StatusCode >= 300 {
		respBody, _ := io.ReadAll(resp.Body)
		return "", fmt.Errorf("got invalid status code (%d) from service: %v", resp.StatusCode, string(respBody))
	}

	output, _ := io.ReadAll(resp.Body)
	return string(output), nil
}
