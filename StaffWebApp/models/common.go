package models

import (
	"fmt"
	"time"
)

type NameId struct {
	Name string
	Id   string
}

type Money struct {
	Amount   float64      `json:"amount"`
	Currency CurrencyType `json:"currency"`
}

func (m *Money) String() string {
	return fmt.Sprintf("%g%s", m.Amount, m.Currency.ToIcon())
}

type PageInfo struct {
	CurrentPage int `json:"currentPage"`
	PagesTotal  int `json:"pagesTotal"`
}

type Page[T any] struct {
	PageInfo PageInfo `json:"pageInfo"`
	Items    []T      `json:"items"`
}

func PrettifyDate(date string) string {
	t, err := time.Parse(time.RFC3339, date)
	if err != nil {
		return date
	}

	return t.Format("2006-01-02 03:04")
}
