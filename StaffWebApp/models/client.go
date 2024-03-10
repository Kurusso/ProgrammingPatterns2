package models

type OperationType string

const (
	Deposit  OperationType = "deposit"
	Withdraw OperationType = "withdraw"
)

type CurrencyType int

const (
	Ruble CurrencyType = iota
	Dollar
	Euro
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
