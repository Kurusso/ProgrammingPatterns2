package services

import (
	"context"
	"net/http"
	"net/url"
	"staff-web-app/config"
	"staff-web-app/models"
	"strconv"
)

func LoadStaffPage(ctx context.Context, page int64, searchTerm string) (*models.Page[models.StaffShort], error) {
	params := url.Values{}
	if page == 0 {
		page = 1
	}

	params.Set("page", strconv.FormatInt(page, 10))
	params.Set("searchPattern", searchTerm)

	var staffPage models.Page[models.StaffShort]
	err := makeRequestParseBody(
		ctx,
		http.MethodGet,
		config.Default.UserApiUrl+"staff?"+params.Encode(),
		nil,
		&staffPage,
	)

	return &staffPage, err
}
