package controllers

import (
	"fmt"
	"net/http"
	"staff-web-app/components/clients"
	"staff-web-app/logger"
	"staff-web-app/models"
	"staff-web-app/services"

	"github.com/julienschmidt/httprouter"
)

const ListUserAccountUrlPattern = "/api/clients/:userId/accounts"

func ListUserAccounts(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	id := ps.ByName("userId")
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

const ListAccountOperationsUrlPattern = "/api/accounts/:userId/operations"

func ListAccountOperations(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	id := ps.ByName("userId")
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

const ListUserCreditsUrlPattern = "/api/clients/:userId/credits"

func ListUserCredits(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
	if userId == "" {
		fmt.Println("can't find Id")
		//TODO: error handling
	}

	credits, err := services.LoadUserCredits(r.Context(), userId)
	if err != nil {
		fmt.Println(err)
		return
	}

	clients.CreditsList(credits).Render(r.Context(), w)
}

const DetailedUserInfoUrlPattern = "/api/clients/:userId/credits/:creditId"

func DetailedCreditInfo(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
	if userId == "" {
		fmt.Println("can't find userId")
		//TODO: error handling
	}

	creditId := ps.ByName("creditId")
	if creditId == "" {
		fmt.Println("can't find creditId")
		//TODO: error handling
	}

	creditInfo, err := services.LoadCreditDetailedInfo(r.Context(), userId, creditId)
	if err != nil {
		logger.Default.Error("failed to load credit info: ", err)
		//TODO: error handling
		return
	}

	clients.DetailedCreditInfo(creditInfo).Render(r.Context(), w)
}

func RenderClientsPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	clients.ClientsPage().Render(r.Context(), w)
}