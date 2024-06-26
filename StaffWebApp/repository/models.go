// Code generated by sqlc. DO NOT EDIT.
// versions:
//   sqlc v1.25.0

package repository

import (
	"database/sql/driver"
	"fmt"

	"github.com/jackc/pgx/v5/pgtype"
)

type Theme string

const (
	ThemeLight Theme = "light"
	ThemeDark  Theme = "dark"
)

func (e *Theme) Scan(src interface{}) error {
	switch s := src.(type) {
	case []byte:
		*e = Theme(s)
	case string:
		*e = Theme(s)
	default:
		return fmt.Errorf("unsupported scan type for Theme: %T", src)
	}
	return nil
}

type NullTheme struct {
	Theme Theme
	Valid bool // Valid is true if Theme is not NULL
}

// Scan implements the Scanner interface.
func (ns *NullTheme) Scan(value interface{}) error {
	if value == nil {
		ns.Theme, ns.Valid = "", false
		return nil
	}
	ns.Valid = true
	return ns.Theme.Scan(value)
}

// Value implements the driver Valuer interface.
func (ns NullTheme) Value() (driver.Value, error) {
	if !ns.Valid {
		return nil, nil
	}
	return string(ns.Theme), nil
}

type Preference struct {
	Userid pgtype.UUID
	Theme  NullTheme
}

type Session struct {
	Sessionid   string
	Accesstoken pgtype.Text
}
