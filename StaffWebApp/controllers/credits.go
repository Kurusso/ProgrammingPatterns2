package controllers

import (
	"net/http"
	"staff-web-app/components/credits"
	"staff-web-app/logger"
	"staff-web-app/services"

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
