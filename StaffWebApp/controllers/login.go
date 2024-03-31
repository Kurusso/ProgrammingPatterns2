package controllers

import (
	"fmt"
	"net/http"
	"staff-web-app/logger"
	"staff-web-app/services"
	"time"
)

const LoginUrlPattern = "GET /login/callback"

func LoginCallback(w http.ResponseWriter, r *http.Request) {
	logger.Default.Info(r.URL.Query().Get("code"))
	time.Sleep(2 * time.Second)
	code := r.URL.Query().Get("code")
	if code == "" {
		fmt.Fprintf(w, "test1")
	} else {
		token, err := services.RetrieveOauth2Token(r.Context(), code)
		logger.Default.Info(token, err)
		fmt.Fprintf(w, "test2")
	}

	// http.Redirect(w, r, "/", http.StatusSeeOther)
}
