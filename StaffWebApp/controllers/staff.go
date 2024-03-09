package controllers

import (
	"net/http"
	"staff-web-app/components/staff"

	"github.com/julienschmidt/httprouter"
)

func RenderStaffPage(w http.ResponseWriter, r *http.Request, ps httprouter.Params) {
	staff.StaffPage().Render(r.Context(), w)
}
