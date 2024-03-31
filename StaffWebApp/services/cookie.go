package services

import "net/http"

const CookieName = "StaffWebAppSession"
const tokenSecret = "fp{Zc(QA!<3kUY7X-yxCG2"

func CheckSessionCookie(r *http.Request) bool {
	_, err := r.Cookie(CookieName)
	return err == nil

}

// func CreateCookie(userId uuid, sessionId uuid, theme string) (string, error) {
// 	token := jwt.NewWithClaims(jwt.SigningMethodHS256, jwt.MapClaims{
// 		"exp":       time.Now().Add(240 * time.Hour).Unix(),
// 		"sub":       userId,
// 		"theme":     theme,
// 		"sessionId": sessionId,
// 	})

// 	return token.SignedString(tokenSecret)
// }
