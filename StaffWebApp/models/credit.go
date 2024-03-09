package models

type CreditRate struct {
	Id      string  `json:"id"`
	Name    string  `json:"name"`
	Percent float64 `json:"monthPercent"`
}

type CreditShort struct {
	Id            string     `json:"id"`
	Rate          CreditRate `json:"creditRate"`
	RemainingDebt Money      `json:"RemainingDebt"`
	UnpaidDebt    Money      `json:"UnpaidDebt"`
	// MoneyTaken    Money      `json:"fullMoneyAmount"`
}

type CreditDetailed struct {
}
