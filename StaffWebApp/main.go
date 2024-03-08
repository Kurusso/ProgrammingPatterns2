package main

import (
	"fmt"
	"net/http"
	"staff-web-app/components"
	"staff-web-app/controllers"

	"github.com/a-h/templ"
	"github.com/julienschmidt/httprouter"
)

func main() {

	router := httprouter.New()
	router.Handler(http.MethodGet, "/", templ.Handler(components.MainPage(components.Clients)))
	router.Handler(http.MethodGet, "/Staff", templ.Handler(components.MainPage(components.Staff)))
	router.Handler(http.MethodGet, "/Credits", templ.Handler(components.MainPage(components.Credits)))

	router.GET("/api/clients/:id/accounts", controllers.ListUserAccounts)
	router.GET("/api/accounts/:id/operations", controllers.ListAccountOperations)
	router.GET("/api/clients/:id/credits", controllers.ListUserCredits)

	err := http.ListenAndServe(":8080", router)
	if err != nil {
		fmt.Printf("failed to start server on 0.0.0.0:8080: %v", err)
		return
	}
}
