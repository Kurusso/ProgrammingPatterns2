package services

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/models"
	"strings"
)

func LoadCreditRates(ctx context.Context) ([]models.CreditRate, error) {
	var rates []models.CreditRate
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.CreditsApiUrl+"CreditRates/GetAll",
		nil,
		&rates,
	)
	return rates, err
}

func LoadUserCredits(ctx context.Context, userId string) ([]models.CreditShort, error) {
	queryParams := url.Values{}
	queryParams.Set("userId", userId)
	var credits []models.CreditShort
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.CreditsApiUrl+"Credit/GetUserCredits?"+queryParams.Encode(),
		nil,
		&credits,
	)
	return credits, err
}

func LoadUserCreditRating(ctx context.Context, userId string) (float64, error) {
	queryParams := url.Values{}
	queryParams.Set("userId", userId)
	var output map[string]any
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.CreditsApiUrl+"Credit/GetUserCreditScore?"+queryParams.Encode(),
		nil,
		&output,
	)
	if err != nil {
		return 0, err
	}
	scoreRaw, ok := output["score"]
	if !ok {
		return 0, fmt.Errorf("response body has no field 'score'")
	}
	score, ok := scoreRaw.(float64)
	if !ok {
		return 0, fmt.Errorf("'score' field is not of type float64")
	}

	return score, nil
}

func LoadCreditDetailedInfo(ctx context.Context, userId string, creditId string) (*models.CreditDetailed, error) {
	queryParams := url.Values{}
	queryParams.Set("userId", userId)
	queryParams.Set("id", creditId)

	var creditInfo models.CreditDetailed
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.CreditsApiUrl+"Credit/GetInfo?"+queryParams.Encode(),
		nil,
		&creditInfo,
	)

	return &creditInfo, err

}

func CreateCreditRate(ctx context.Context, name string, percent int) error {
	body := make(map[string]any, 2)
	body["name"] = name
	body["monthPercent"] = float64(percent) / 100.0

	output, err := json.Marshal(body)
	if err != nil {
		return fmt.Errorf("failed to encode body: %v", err)
	}

	err = makeRequestParseBody(
		ctx,
		http.MethodPost,
		config.Default.CreditsApiUrl+"CreditRates/Create",
		strings.NewReader(string(output)),
		nil,
	)

	if err != nil {
		return fmt.Errorf("create credit rate request failed: %v", err)
	}

	return nil
}
