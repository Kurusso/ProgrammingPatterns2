package services

import (
	"context"
	"net/http"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/models"
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
