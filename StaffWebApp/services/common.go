package services

import (
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
)

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

	resp, err := http.DefaultClient.Do(request)
	if err != nil {
		return fmt.Errorf("failed to make request: %v", err)
	}

	if resp.StatusCode >= 300 {
		return fmt.Errorf("got invalid status code (%d) from service", resp.StatusCode)
	}

	err = json.NewDecoder(resp.Body).Decode(value)
	if err != nil {
		return fmt.Errorf("failed to parse response body: %v", err)
	}

	return nil
}
