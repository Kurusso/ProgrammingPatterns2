package models

type OperationType int

const (
	Deposit OperationType = iota
	Withdraw
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
	Type           OperationType `json:"operationType"`
	Money          Money         `json:"moneyAmmount"`
	ConvertedMoney float64       `json:"moneyAmmountInAccountCurrency"`
	Date           string
}

type AccountShort struct {
	Id    string
	Money Money
}

type AccountDetailed struct {
	AccountShort
	Operations []Operation `json:"operationsHistory"`
}

type ClientShort struct {
	Id       string
	Username string
}
