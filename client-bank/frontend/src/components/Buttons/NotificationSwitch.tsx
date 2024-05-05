import React, {useEffect} from "react";
import {getFirebaseToken} from "../../other/firebase-notifications";
import {isAuthenticated} from "../../api/auth";
import {NotificationsService} from "../../api/notifications";
import {getRawAsset} from "node:sea";
import {Theme, userSettings} from "../../api/userSettings";

export const NotificationSwitch = () => {
 const [isNotificationOn, setNotification] = React.useState(false);

    useEffect(() => {

        const fetchNotifications = async () => {
            try {
                if (!isAuthenticated())
                    throw new Error("not authenticated")
                let notifications
                    = await NotificationsService.getNotifications();
                let token= getFirebaseToken();
                let notification = notifications.find(notification=>notification.token===token)
                setNotification(!!notification)
            }
            catch (e ){
                console.log(e)
            }

        }

        fetchNotifications();

        //GetNotification
        //Filter
    }, []);

    const handleSwitchChange= async () => {
        if (!isAuthenticated())
            throw new Error("not authenticated")
        if (isNotificationOn) {
            //DisableNotifications
            await NotificationsService.DisableNotifications()
        } else {
            //EnableNotifications
            await NotificationsService.EnableNotifications()
        }

        setNotification(!isNotificationOn);
    }

    return (
        <div>
            <h3>Notification Settings</h3>
            <label>
                <input
                    type="checkbox"
                    checked={isNotificationOn}
                    onChange={handleSwitchChange}
                />
                {isNotificationOn ? 'Notifications are ON' : 'Notifications are OFF'}
            </label>
        </div>
    );
};