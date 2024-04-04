package controllers

import (
	"fmt"
	"net/http"
	"staff-web-app/logger"
	"staff-web-app/services"
)

const LoginUrlPattern = "GET /login/callback"

func LoginCallback(w http.ResponseWriter, r *http.Request) {
	logger.Default.Info(r.URL.Query().Get("code"))
	code := r.URL.Query().Get("code")
	if code == "" {
		fmt.Fprintf(w, "test1")
	} else {
		token, err := services.RetrieveOauth2Token(r.Context(), code)
		if err != nil {
			http.Redirect(w, r, "/Error", http.StatusSeeOther)
		}

		session, err := services.CreateCookie(r.Context(), token)
		if err != nil {
			http.Redirect(w, r, "/Error", http.StatusSeeOther)
		}

		http.SetCookie(w, &http.Cookie{
			Name:   services.CookieName,
			Value:  session,
			Domain: r.URL.Host,
			Path:   "/",
		})
		userId, err := services.GetUserId(r.Context(), token)
		if err == nil {
			err = services.InitPreferences(r.Context(), userId)
			if err != nil {
				logger.Default.Error("failed to init user preferences")
			}
		}

		http.Redirect(w, r, "/", http.StatusSeeOther)
	}
}
