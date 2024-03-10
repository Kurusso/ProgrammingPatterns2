package controllers

import (
	"net/http"
	"staff-web-app/components/credits"
	"staff-web-app/logger"
	"staff-web-app/services"
	"strconv"

	"github.com/julienschmidt/httprouter"
)

func RenderCreditsPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	rates, err := services.LoadCreditRates(r.Context())
	if err != nil {
		logger.Default.Error("Failed to render credits page: ", err)
		//TODO: error handling
		return
	}

	credits.CreditsPage(rates).Render(r.Context(), w)
}

const CreateCreditRateUrlPattern = "/api/credits/rates"

func CreateCreditRate(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	creditRateName := r.FormValue("creditRateName")
	percentStr := r.FormValue("creditRatePercent")
	if creditRateName == "" {
		//TODO: error handling
		return
	}
	if percentStr == "" {
		//TODO: error handling
		return
	}

	percent, err := strconv.Atoi(percentStr)
	if err != nil {
		//TODO: error handling
		return
	}

	err = services.CreateCreditRate(r.Context(), creditRateName, percent)
	if err != nil {
		logger.Default.Error(err)
		//TODO: error handling
		return
	}

	http.Redirect(w, r, "/Credits", http.StatusSeeOther)
}
