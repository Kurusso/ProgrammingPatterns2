package services

import (
	"context"
	"math/rand"
	"net/http"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/repository"

	"github.com/jackc/pgx/v5/pgtype"
)

const CookieName = "StaffWebAppSession"

func CheckSessionCookie(r *http.Request) bool {
	cookie, err := r.Cookie(CookieName)
	if err != nil {
		return false
	}

	dbctx := repository.NewDbContext(r.Context())
	accessToken, err := dbctx.GetTokenBySessionId(r.Context(), cookie.Value)
	if err != nil {
		return false
	}

	_, err = GetUserId(r.Context(), accessToken.String)
	return err == nil
}

func RandomString(length int) string {
	const charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"

	result := make([]byte, length)
	for i := range result {
		result[i] = charset[rand.Intn(len(charset))]
	}
	return string(result)
}

func GetSeessionId(r *http.Request) string {
	cookie, err := r.Cookie(CookieName)
	if err != nil {
		return ""
	}
	return cookie.Value
}

func GetUserId(ctx context.Context, accessToken string) (string, error) {
	reqUrl, _ := url.JoinPath(config.Default.AuthApiUrl, "/auth/validate")
	var headers = map[string]string{
		"Authorization": "Bearer " + accessToken,
	}
	userId, err := makeRequestWithHeaders(
		ctx,
		"GET",
		reqUrl,
		headers,
	)
	return userId, err
}

func CreateCookie(ctx context.Context, accessToken string) (string, error) {
	session := RandomString(30)
	dbctx := repository.NewDbContext(ctx)
	err := dbctx.InsertSession(ctx, repository.InsertSessionParams{
		Sessionid:   session,
		Accesstoken: pgtype.Text{String: accessToken, Valid: true},
	})
	if err != nil {
		return "", err
	}

	return session, nil
}

func makeAccessTokenHeader(ctx context.Context, sessionId string) map[string]string {
	dbctx := repository.NewDbContext(ctx)
	token, err := dbctx.GetTokenBySessionId(ctx, sessionId)
	if err != nil {
		return nil
	}
	return map[string]string{
		"Authorization": "Bearer " + token.String,
	}
}
