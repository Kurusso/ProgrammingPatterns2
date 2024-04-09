package middleware

import (
	"context"
	"net/http"
	"staff-web-app/repository"

	"github.com/jackc/pgx/v5/pgtype"
)

func setLightTheme(r *http.Request) *http.Request {
	ctx := context.WithValue(r.Context(), "theme", "light")
	return r.WithContext(ctx)
}

func setDarkTheme(r *http.Request) *http.Request {
	ctx := context.WithValue(r.Context(), "theme", "dark")
	return r.WithContext(ctx)
}

func Theme(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		userId, ok := r.Context().Value("userId").(string)
		if !ok {
			next.ServeHTTP(w, setLightTheme(r))
			return
		}

		dbx := repository.NewDbContext(r.Context())
		uuid := pgtype.UUID{}
		uuid.Scan(userId)
		theme, err := dbx.GetThemeByUserId(r.Context(), uuid)
		if err != nil {
			next.ServeHTTP(w, setLightTheme(r))
			return
		}

		if theme.Theme == repository.ThemeDark {
			next.ServeHTTP(w, setDarkTheme(r))
		} else {
			next.ServeHTTP(w, setLightTheme(r))
		}
	})
}
