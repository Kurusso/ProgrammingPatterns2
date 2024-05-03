package controllers

import (
	"fmt"
	"net/http"
	"os"
)

func HandleStatic(filePath string) func(w http.ResponseWriter, r *http.Request) {
	return func(w http.ResponseWriter, r *http.Request) {
		content, err := os.ReadFile(filePath)
		if err != nil {
			w.WriteHeader(http.StatusNotFound)
			fmt.Fprint(w, "Not Found")
			return
		}

		// contentType := http.DetectContentType(content)
		w.Header().Add("Content-Type", "text/javascript")

		w.Write(content)
	}
}
