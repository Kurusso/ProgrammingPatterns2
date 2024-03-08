package models

type CreditRate struct {
	Id      string
	Name    string
	Percent float64
}

type Credit struct {
	Rate          CreditRate
	Id            string
	MoneyTaken    int64
	MonthlyPay    int64
	RemainingDebt int64
	UnpaidDebt    int64
}
