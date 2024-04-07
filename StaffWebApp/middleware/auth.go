package middleware

import (
	"context"
	"net/http"
	"staff-web-app/logger"
	"staff-web-app/services"
)

func Auth(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		logger.Default.Debug("auth check")
		userId := services.CheckSessionCookie(r)
		if userId == "" {
			authUrl := services.MakeOauth2AuthUrl()
			http.Redirect(w, r, authUrl, http.StatusSeeOther)
		}

		ctx := context.WithValue(r.Context(), "userId", userId)
		newR := r.WithContext(ctx)

		next.ServeHTTP(w, newR)
	})
}
