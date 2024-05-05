package services

import (
	"bytes"
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/repository"

	"github.com/jackc/pgx/v5/pgtype"
)

func UpdateTheme(ctx context.Context, userId string, theme repository.Theme) error {
	dbx := repository.NewDbContext(ctx)
	uuid := pgtype.UUID{}
	uuid.Scan(userId)
	return dbx.UpdateUserPreferences(ctx, repository.UpdateUserPreferencesParams{
		Userid: uuid,
		Theme:  repository.NullTheme{Theme: theme, Valid: true},
	})
}

func InitPreferences(ctx context.Context, userId string) error {
	dbx := repository.NewDbContext(ctx)
	uuid := pgtype.UUID{}
	uuid.Scan(userId)
	return dbx.InitUserPreferences(ctx, uuid)
}

func NotificationsSubscribe(ctx context.Context, userId string, token string) error {
	url, err := url.JoinPath(config.Default.CoreApiUrl, "Notifications/", userId)
	if err != nil {
		return fmt.Errorf("failed to make url: %v", err)
	}
	var body = map[string]string{
		"token": token,
		"appId": config.Default.FirebaseAppId,
	}
	bodyOutput, err := json.Marshal(body)
	if err != nil {
		return fmt.Errorf("failed to marshal body: %v", err)
	}

	err = makeRequestParseBody(
		ctx,
		http.MethodPost,
		url,
		bytes.NewReader(bodyOutput),
		nil,
	)

	if err != nil {
		return fmt.Errorf("request to core failed: %v", err)
	}

	return nil
}
