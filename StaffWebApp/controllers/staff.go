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
		//TODO: error handling
		return
	}
	searchTerm := params.Get("searchTerm")

	page, err := services.LoadStaffPage(r.Context(), int64(pageNumber), searchTerm)
	if err != nil {
		logger.Default.Error("failed to load staff page: %v", err)
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
		//TODO: error handling
		return
	}
	if password == "" {
		//TODO: error handling
		return
	}

	err := services.CreateNewStaffProfile(r.Context(), username, password)
	if err != nil {
		logger.Default.Error(err)
		//TODO: error handling
		return
	}

	http.Redirect(w, r, "/Staff", http.StatusSeeOther)
}
