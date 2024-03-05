package models

type OperationType string

const (
	Deposit  OperationType = "deposit"
	Withdraw OperationType = "withdraw"
)

type CurrencyType string

const (
	Ruble  CurrencyType = "ruble"
	Dollar CurrencyType = "dollar"
	Euro   CurrencyType = "euro"
)

func (c CurrencyType) ToIcon() string {
	switch c {
	case Ruble:
		return "₽"
	case Dollar:
		return "$"
	case Euro:
		return "€"
	default:
		return ""
	}
}

type Operation struct {
	Type           OperationType
	CurrencyAmount uint64
	Date           string
}

type Account struct {
	Id             string
	CurrencyAmount uint64 //TODO: может ли быть счет отрицательным
	Currency       CurrencyType
}
