package controllers

import (
	"net/http"
	"staff-web-app/logger"
	"staff-web-app/repository"
	"staff-web-app/services"
)

const UpdateThemeUrlPattern = "POST /api/preferences/theme"

func UpdateTheme(w http.ResponseWriter, r *http.Request) {
	value := r.FormValue("themeSwitch")
	userId := r.Context().Value("userId").(string)
	theme := repository.ThemeLight
	if value == "on" {
		theme = repository.ThemeDark
	}
	err := services.UpdateTheme(r.Context(), userId, theme)
	if err != nil {
		logger.Default.Error("failed to update theme: ", err)
		w.WriteHeader(http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
}
