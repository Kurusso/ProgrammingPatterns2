package controllers

import (
	"net/http"
	"staff-web-app/components/clients"
	"staff-web-app/logger"
	"staff-web-app/models"
	"staff-web-app/services"
	"strconv"
)

const ListUserAccountUrlPattern = "GET /api/clients/{userId}/accounts"

func ListUserAccounts(w http.ResponseWriter, r *http.Request) {
	id := r.PathValue("userId")
	if id == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	accounts, err := services.LoadUserAccounts(r.Context(), id)
	if err != nil {
		logger.Default.Error("failed to load user accounts: ", err)
		return
	}

	clients.AccountList(accounts).Render(r.Context(), w)
}

const ListAccountOperationsUrlPattern = "GET /api/clients/{userId}/accounts/{accountId}/operations"

func ListAccountOperations(w http.ResponseWriter, r *http.Request) {
	userId := r.PathValue("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	accountId := r.PathValue("accountId")
	if accountId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	updates := make(chan *models.AccountDetailed)
	clientQuit := make(chan bool)
	go services.WsLoadAccountOperations(r, updates, clientQuit)
	services.WsUpdateAccountOperations(w, r, updates, clientQuit)

	// account, err := services.LoadAccountOperationHistory(r.Context(), accountId, userId)
	// if err != nil {
	// logger.Default.Error("failed to load account operation history: ", err)
	// w.WriteHeader(http.StatusInternalServerError)
	// return
	// }

}

const ListUserCreditsUrlPattern = "GET /api/clients/{userId}/credits"

func ListUserCredits(w http.ResponseWriter, r *http.Request) {
	userId := r.PathValue("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	credits, err := services.LoadUserCredits(r.Context(), userId)
	if err != nil {
		logger.Default.Error(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	clients.CreditsList(credits).Render(r.Context(), w)
}

const DetailedUserInfoUrlPattern = "GET /api/clients/{userId}/credits/{creditId}"

func DetailedCreditInfo(w http.ResponseWriter, r *http.Request) {
	userId := r.PathValue("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	creditId := r.PathValue("creditId")
	if creditId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	creditInfo, err := services.LoadCreditDetailedInfo(r.Context(), userId, creditId)
	if err != nil {
		logger.Default.Error(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	clients.DetailedCreditInfo(creditInfo).Render(r.Context(), w)
}

const ListClientsPageUrlPattern = "GET /api/clients"

func ListClientsPage(w http.ResponseWriter, r *http.Request) {
	params := r.URL.Query()
	pageNumber, err := strconv.Atoi(params.Get("page"))
	if err != nil {
		logger.Default.Error("bad query params!: ", r.URL.String())
		w.WriteHeader(http.StatusBadRequest)
		return
	}
	searchTerm := params.Get("searchTerm")

	page, err := services.LoadClientsPage(r.Context(), searchTerm, pageNumber)
	if err != nil {
		logger.Default.Error("failed to load clients page: ", err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	clients.ClientsList(page).Render(r.Context(), w)
}

const CreateClientProfileUrlPattern = "POST /api/clients"

func CreateClientProfile(w http.ResponseWriter, r *http.Request) {
	username := r.FormValue("username")
	password := r.FormValue("password")
	if username == "" {
		logger.Default.Error("couldn't find username in form")
		http.Redirect(w, r, "/Error", http.StatusTemporaryRedirect)
		return
	}
	if password == "" {
		logger.Default.Error("couldn't find password in form")
		http.Redirect(w, r, "/Error", http.StatusTemporaryRedirect)
		return
	}

	err := services.CreateClientProfile(r.Context(), username, password)
	if err != nil {
		logger.Default.Error(err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	http.Redirect(w, r, "/", http.StatusSeeOther)
}

const BlockClientProfileUrlPattern = "DELETE /api/clients/{userId}"

func BlockClientProfile(w http.ResponseWriter, r *http.Request) {
	userId := r.PathValue("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	err := services.BlockClientProfile(r.Context(), userId)
	if err != nil {
		logger.Default.Error(err)
		w.WriteHeader(http.StatusNotFound)
		return
	}

	ListClientsPage(w, r)
}

func RenderClientsPage(w http.ResponseWriter, r *http.Request) {
	clients.ClientsPage().Render(r.Context(), w)
}
