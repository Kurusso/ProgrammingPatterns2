package repository

import (
	"context"
	"staff-web-app/config"
	"staff-web-app/logger"

	"github.com/jackc/pgx/v5/pgxpool"
)

var dbpool *pgxpool.Pool

func ConnectToDatabase() func() {
	var err error
	dbpool, err = pgxpool.New(context.Background(), config.Default.DbConnectionString)
	if err != nil {
		logger.Default.Panic("failed to init database pool: %v", err)
	}

	return dbpool.Close
}

func NewDbContext(ctx context.Context) *Queries {
	conn, err := dbpool.Acquire(ctx)
	if err != nil {
		logger.Default.Panic("failed to acquire db connection from the pool")
	}
	context.AfterFunc(ctx, conn.Release)
	return New(conn)
}
