package models

import (
	"sort"
	"time"
)

type OperationType int

const (
	Deposit OperationType = iota
	Withdraw
	TransferGet
	TransferSend
)

func (op OperationType) OperationSign() string {
	if op == Deposit || op == TransferGet {
		return "+"
	}
	return "-"
}

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
	Date           string        `json:"creationTime"`
}

type AccountShort struct {
	Id     string
	UserId string
	Money  Money
}

type AccountDetailed struct {
	AccountShort
	Operations []Operation `json:"operationsHistory"`
}

func (ad *AccountDetailed) SortOperationsByDate() {
	sort.Slice(ad.Operations, func(i, j int) bool {
		a, err := time.Parse(time.RFC3339, ad.Operations[i].Date)
		if err != nil {
			return false
		}
		b, err := time.Parse(time.RFC3339, ad.Operations[j].Date)
		if err != nil {
			return true
		}

		return a.Unix() > b.Unix()
	})
}

type ClientShort struct {
	Id       string
	Username string
}
