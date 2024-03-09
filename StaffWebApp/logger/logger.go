package logger

import (
	"os"

	"go.uber.org/zap"
	"go.uber.org/zap/zapcore"
)

var (
	Default *zap.SugaredLogger
)

func InitConsoleLogger(debug bool) *zap.Logger {
	level := zapcore.InfoLevel
	if debug {
		level = zapcore.DebugLevel
	}
	logger := zap.New(newConsoleLoggerCore(level))
	Default = logger.Sugar()
	return logger
}

func newConsoleLoggerCore(level zapcore.Level) zapcore.Core {
	cfg := zap.NewDevelopmentEncoderConfig()
	cfg.EncodeLevel = zapcore.CapitalColorLevelEncoder
	core := zapcore.NewCore(
		zapcore.NewConsoleEncoder(cfg),
		zapcore.AddSync(os.Stdout),
		level,
	)

	return core
}
