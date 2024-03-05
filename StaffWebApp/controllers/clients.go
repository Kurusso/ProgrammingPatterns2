package controllers

import (
	"fmt"
	"net/http"
	"staff-web-app/components/clients"
	"staff-web-app/models"

	"github.com/julienschmidt/httprouter"
)

func ListUserAccounts(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	id := ps.ByName("id")
	if id == "" {
		fmt.Println("can't find Id")
		//TODO: error handling
	}

	clients.AccountList([]models.Account{
		{"12345", 1534, models.Dollar},
		{"84756", 7584, models.Dollar},
		{"19385", 1283, models.Euro},
		{"73453", 9574, models.Ruble},
	}).Render(r.Context(), w)

}

func ListAccountOperations(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	id := ps.ByName("id")
	if id == "" {
		fmt.Println("can't find Id")
		//TODO: error handling
	}

	clients.OperationList(
		[]models.Operation{
			{models.Deposit, 700, "17-10-2023"},
			{models.Withdraw, 100, "18-10-2023"},
			{models.Withdraw, 350, "19-10-2023"},
			{models.Deposit, 80, "20-10-2023"},
		}, models.Dollar,
	).Render(r.Context(), w)
}
