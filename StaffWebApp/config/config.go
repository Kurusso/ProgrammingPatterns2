package config

import (
	"encoding/json"
	"fmt"
	"os"
)

var Default ApplicationConfig

type ApplicationConfig struct {
	CreditsApiUrl string
	CoreApiUrl    string
	UserApiUrl    string
}

func ReadConfig(path string) error {
	fconfig, err := os.Open(path)
	if err != nil {
		return fmt.Errorf("failed to open config file: %v", err)
	}

	err = json.NewDecoder(fconfig).Decode(&Default)
	if err != nil {
		return fmt.Errorf("failed to parse config: %v", err)
	}

	return nil
}
