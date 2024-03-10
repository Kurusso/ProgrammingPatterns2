package controllers

import (
	"net/http"
	"staff-web-app/components/clients"
	"staff-web-app/logger"
	"staff-web-app/services"
	"strconv"

	"github.com/julienschmidt/httprouter"
)

const ListUserAccountUrlPattern = "/api/clients/:userId/accounts"

func ListUserAccounts(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	id := ps.ByName("userId")
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

const ListAccountOperationsUrlPattern = "/api/clients/:userId/accounts/:accountId/operations"

func ListAccountOperations(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	accountId := ps.ByName("accountId")
	if accountId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	account, err := services.LoadAccountOperationHistory(r.Context(), accountId, userId)
	if err != nil {
		logger.Default.Error("failed to load account operation history: ", err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	clients.OperationList(account).Render(r.Context(), w)
}

const ListUserCreditsUrlPattern = "/api/clients/:userId/credits"

func ListUserCredits(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
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

const DetailedUserInfoUrlPattern = "/api/clients/:userId/credits/:creditId"

func DetailedCreditInfo(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	creditId := ps.ByName("creditId")
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

const ListClientsPageUrlPattern = "/api/clients"

func ListClientsPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
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

const CreateClientProfileUrlPattern = "/api/clients"

func CreateClientProfile(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
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

const BlockClientProfileUrlPattern = "/api/clients/:userId"

func BlockClientProfile(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
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

	ListClientsPage(w, r, ps)
}

func RenderClientsPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	clients.ClientsPage().Render(r.Context(), w)
}
