package models

import "fmt"

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
