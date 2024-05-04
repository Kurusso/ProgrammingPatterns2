package services

import (
	"context"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"net/url"
	"slices"
	"staff-web-app/logger"
	"sync"
	"time"

	"github.com/google/uuid"
)

type timeValuePair struct {
	timestamp int64
	success   bool
}
type circuitBreaker struct {
	lastRequests []timeValuePair
	historyTime  int64
	lock         sync.Mutex
}

func prettyUrl(url *url.URL) string {
	shortUrl := url.Host + url.Path
	if len(shortUrl) > 50 {
		shortUrl = shortUrl[:50] + "..."
	}

	return shortUrl
}

func (cb *circuitBreaker) getSuccessRate() float64 {
	cb.lock.Lock()
	defer cb.lock.Unlock()

	cb.lastRequests = slices.DeleteFunc(cb.lastRequests, func(el timeValuePair) bool {
		return el.timestamp < time.Now().Unix()-cb.historyTime
	})

	goodRequestsCount := 0.0
	for i := range cb.lastRequests {
		if cb.lastRequests[i].success {
			goodRequestsCount++
		}
	}

	return goodRequestsCount / float64(len(cb.lastRequests))
}

func (cb *circuitBreaker) updateRequest(success bool) {
	cb.lock.Lock()
	defer cb.lock.Unlock()

	cb.lastRequests = append(cb.lastRequests, timeValuePair{time.Now().Unix(), success})
}

func (cb *circuitBreaker) MakeNextRequest() bool {
	return len(cb.lastRequests) < 5 || cb.getSuccessRate() >= 0.3
}

const retryDelta = 5
const maxRetries = 5

var breaker = circuitBreaker{historyTime: 60, lock: sync.Mutex{}}

func idempotentAutoRetryWrapper(request *http.Request) (*http.Response, error) {
	var (
		resp *http.Response
		err  error
	)

	queries := request.URL.Query()
	queries.Add("idempotentId", uuid.NewString())
	request.URL.RawQuery = queries.Encode()

	retries := 0
	sleepyTime := 0
	for (resp == nil || resp.StatusCode >= 500) && retries < maxRetries {
		if retries > 1 {
			logger.Default.Debugf("%d retries on %s. waiting for %ds", retries, prettyUrl(request.URL), sleepyTime)

		}
		select {
		case <-time.After(time.Duration(sleepyTime) * time.Second):
			// try next request
		case <-request.Context().Done():
			return nil, request.Context().Err()
		}

		retries++
		sleepyTime += retryDelta

		if !breaker.MakeNextRequest() {
			logger.Default.Warnf("request to %s was rejected by ciruict breaker", prettyUrl(request.URL))
			continue
		}

		resp, err = http.DefaultClient.Do(request)
		if err != nil {
			return nil, err
		}

		breaker.updateRequest(resp.StatusCode < 500)
	}

	if retries == maxRetries {
		logger.Default.Debugf("giving up on %s after %d retries", prettyUrl(request.URL), maxRetries)
	} else if retries > 1 {
		logger.Default.Debugf("success on %s after %d retries", prettyUrl(request.URL), retries)
	}

	return resp, nil
}

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

	resp, err := idempotentAutoRetryWrapper(request)
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
	resp, err := idempotentAutoRetryWrapper(request)
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
	resp, err := idempotentAutoRetryWrapper(request)
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
