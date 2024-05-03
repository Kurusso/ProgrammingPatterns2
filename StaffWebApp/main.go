package main

import (
	"crypto/tls"
	"fmt"
	"net/http"
	"staff-web-app/components"
	"staff-web-app/config"
	"staff-web-app/controllers"
	"staff-web-app/logger"
	"staff-web-app/middleware"
	"staff-web-app/repository"

	"github.com/a-h/templ"
)

func main() {
	log := logger.InitConsoleLogger(true)
	defer log.Sync()

	err := config.ReadConfig("application-config.json")
	if err != nil {
		fmt.Printf("failed to read config: %v\n", err)
		return
	}

	closedb := repository.ConnectToDatabase()
	defer closedb()

	http.DefaultTransport.(*http.Transport).TLSClientConfig = &tls.Config{InsecureSkipVerify: true}

	authRouter := http.NewServeMux()
	authRouter.HandleFunc("GET /", controllers.RenderClientsPage)
	authRouter.HandleFunc("GET /Staff", controllers.RenderStaffPage)
	authRouter.HandleFunc("GET /Credits", controllers.RenderCreditsPage)

	authRouter.HandleFunc(controllers.ListUserAccountUrlPattern, controllers.ListUserAccounts)
	authRouter.HandleFunc(controllers.ListAccountOperationsUrlPattern, controllers.ListAccountOperations)
	authRouter.HandleFunc(controllers.ListUserCreditsUrlPattern, controllers.ListUserCredits)
	authRouter.HandleFunc(controllers.DetailedUserInfoUrlPattern, controllers.DetailedCreditInfo)
	authRouter.HandleFunc(controllers.ListStaffPageUrlPattern, controllers.ListStaffPage)
	authRouter.HandleFunc(controllers.ListClientsPageUrlPattern, controllers.ListClientsPage)

	authRouter.HandleFunc(controllers.CreateCreditRateUrlPattern, controllers.CreateCreditRate)
	authRouter.HandleFunc(controllers.CreateStaffProfileUrlPattern, controllers.CreateStaffProfile)
	authRouter.HandleFunc(controllers.CreateClientProfileUrlPattern, controllers.CreateClientProfile)

	authRouter.HandleFunc(controllers.BlockStaffProfileUrlPattern, controllers.BlockStaffProfile)
	authRouter.HandleFunc(controllers.BlockClientProfileUrlPattern, controllers.BlockClientProfile)
	authRouter.HandleFunc(controllers.UpdateThemeUrlPattern, controllers.UpdateTheme)

	router := http.NewServeMux()
	router.Handle("GET /Error", templ.Handler(components.ErrorPage()))
	router.HandleFunc("GET /firebase-messaging-sw.js", controllers.HandleStatic("./static/firebase-messaging-sw.js"))
	router.HandleFunc(controllers.LoginUrlPattern, controllers.LoginCallback)
	router.Handle("/",
		middleware.Auth(
			middleware.Theme(
				authRouter,
			),
		),
	)

	err = http.ListenAndServe(":8080", router)
	if err != nil {
		fmt.Printf("failed to start server on 0.0.0.0:8080: %v", err)
		return
	}
}
