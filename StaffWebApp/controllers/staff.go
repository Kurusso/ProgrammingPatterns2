package controllers

import (
	"net/http"
	"staff-web-app/components/staff"
	"staff-web-app/logger"
	"staff-web-app/services"
	"strconv"

	"github.com/julienschmidt/httprouter"
)

const ListStaffPageUrlPattern = "/api/staff"

func ListStaffPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	params := r.URL.Query()
	pageNumber, err := strconv.Atoi(params.Get("page"))
	if err != nil {
		logger.Default.Error("incorrect pageNumber: %v", err)
		w.WriteHeader(http.StatusBadRequest)
		return
	}
	searchTerm := params.Get("searchTerm")

	page, err := services.LoadStaffPage(r.Context(), int64(pageNumber), searchTerm)
	if err != nil {
		logger.Default.Error("failed to load staff page: ", err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	staff.StaffList(page).Render(r.Context(), w)

}

func RenderStaffPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	staff.StaffPage().Render(r.Context(), w)
}

const CreateStaffProfileUrlPattern = "/api/staff"

func CreateStaffProfile(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	username := r.FormValue("username")
	password := r.FormValue("password")
	if username == "" {
		http.Redirect(w, r, "/Error", http.StatusTemporaryRedirect)
		return
	}
	if password == "" {
		http.Redirect(w, r, "/Error", http.StatusTemporaryRedirect)
		return
	}

	err := services.CreateStaffProfile(r.Context(), username, password)
	if err != nil {
		logger.Default.Error(err)
		http.Redirect(w, r, "/Error", http.StatusTemporaryRedirect)
		return
	}

	http.Redirect(w, r, "/Staff", http.StatusSeeOther)
}

const BlockStaffProfileUrlPattern = "/api/staff/:userId"

func BlockStaffProfile(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	userId := ps.ByName("userId")
	if userId == "" {
		logger.Default.Error("bad url!: ", r.URL.String())
		w.WriteHeader(http.StatusNotFound)
		return
	}

	err := services.BlockStaffProfile(r.Context(), userId)
	if err != nil {
		logger.Default.Error(err)
		w.WriteHeader(http.StatusBadRequest)
		return
	}

	ListStaffPage(w, r, ps)
}
