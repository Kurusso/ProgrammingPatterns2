package models

type NameId struct {
	Name string
	Id   string
}

type Money struct {
	Amount   float64
	Currency CurrencyType
}
