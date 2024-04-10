import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";

export type Theme = 'light' | 'dark';

export interface AccountId {
    accountId: string;
    IsHidden:boolean
}

export class userSettings {

    static async ChangeAccountVisibility(accountId: string) {
        let requestUrl = `${magicConsts.changeAccountVisibilityEndpoint}?accountId=${accountId}`
        const response = await fetch(requestUrl, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken(),
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        //  return response.json();
    }

    static async GetTheme() {

        const response = await fetch(`${magicConsts.getThemeEndpoint}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken(),
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const themeNumber = await response.json();
        let theme: Theme;

        switch (themeNumber) {
            case 0:
                theme = 'light';
                break;
            case 1:
                theme = 'dark';
                break;
            default:
                throw new Error('Invalid theme number from server');
        }
        return theme;

    }

    static async ChangeTheme() {
        const response = await fetch(magicConsts.ChangeThemeEndpoint, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken(),
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

    }
}