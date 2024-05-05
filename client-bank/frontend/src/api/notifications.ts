import {magicConsts} from "./magicConst";
import {getAccessToken} from "./auth";
import {firebaseConfig, getFirebaseToken} from "../other/firebase-notifications";

interface NotificationResponse {
    appId: string;
    id: string;
    token: string;
}


export class NotificationsService {

    static async getNotifications(): Promise<NotificationResponse[]> {
        console.log("Getting Notifications");
        let requestUrl = `${magicConsts.NotificationsEndpoint}`
        const response = await fetch(requestUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken(),
            }
        });

        return await response.json();
    }

    static async EnableNotifications(): Promise<void> {
        console.log("Enable Notifications");
        let requestUrl = `${magicConsts.NotificationsEndpoint}`
        const response = await fetch(requestUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': getAccessToken(),
            }, body:JSON.stringify( {
                token:getFirebaseToken(),
                appId:firebaseConfig.appId
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
    }

    static async DisableNotifications(): Promise<void> {
        console.log("Disable Notifications");
        let requestUrl = `${magicConsts.NotificationsEndpoint}/${getFirebaseToken()}`
        const response = await fetch(requestUrl, {
            method: 'DELETE',
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