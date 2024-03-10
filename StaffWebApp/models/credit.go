package models

type CreditRate struct {
	Id      string  `json:"id"`
	Name    string  `json:"name"`
	Percent float64 `json:"monthPercent"`
}

type CreditShort struct {
	Id            string     `json:"id"`
	UserId        string     `json:"userId"`
	Rate          CreditRate `json:"creditRate"`
	RemainingDebt Money      `json:"remainingDebt"`
	UnpaidDebt    Money      `json:"unpaidDebt"`
}

type CreditDetailed struct {
	CreditShort
	LinkedAccount  string `json:"payingAccountId"`
	MoneyTaken     Money  `json:"fullMoneyAmount"`
	MonthlyPayment Money  `json:"monthPayAmount"`
}
