import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";

export class NotificationsService{

    public async getNotifications(): Promise<void> {

    }
    public async EnableNotifications(accountId: string):Promise<void> {
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
    }
    public async DisableNotifications(accountId: string):Promise<void> {
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
    }
}