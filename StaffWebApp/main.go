package main

import (
	"crypto/tls"
	"fmt"
	"net/http"
	"staff-web-app/config"
	"staff-web-app/controllers"
	"staff-web-app/logger"

	"github.com/julienschmidt/httprouter"
)

func main() {
	log := logger.InitConsoleLogger(true)
	defer log.Sync()

	err := config.ReadConfig("application-config.json")
	if err != nil {
		fmt.Printf("failed to read config: %v\n", err)
		return
	}

	http.DefaultTransport.(*http.Transport).TLSClientConfig = &tls.Config{InsecureSkipVerify: true}

	router := httprouter.New()
	router.GET("/", controllers.RenderClientsPage)
	router.GET("/Staff", controllers.RenderStaffPage)
	router.GET("/Credits", controllers.RenderCreditsPage)

	router.GET(controllers.ListUserAccountUrlPattern, controllers.ListUserAccounts)
	router.GET(controllers.ListAccountOperationsUrlPattern, controllers.ListAccountOperations)
	router.GET(controllers.ListUserCreditsUrlPattern, controllers.ListUserCredits)
	router.GET(controllers.DetailedUserInfoUrlPattern, controllers.DetailedCreditInfo)
	router.GET(controllers.ListStaffPageUrlPattern, controllers.ListStaffPage)
	router.POST(controllers.CreateCreditRateUrlPattern, controllers.CreateCreditRate)
	router.POST(controllers.CreateStaffProfileUrlPattern, controllers.CreateStaffProfile)
	router.DELETE(controllers.BlockStaffProfileUrlPattern, controllers.BlockStaffProfile)

	err = http.ListenAndServe(":8080", router)
	if err != nil {
		fmt.Printf("failed to start server on 0.0.0.0:8080: %v", err)
		return
	}
}
