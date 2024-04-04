package services

import (
	"context"
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
