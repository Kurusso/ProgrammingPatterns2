package services

import (
	"context"
	"net/http"
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

	var credits []models.CreditShort
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.CreditsApiUrl+"Credit/GetUserCredits?userId="+userId,
		nil,
		&credits,
	)
	return credits, err
}
