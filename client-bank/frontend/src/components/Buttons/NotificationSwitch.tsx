import React from "react";

export const NotificationSwitch = () => {
 const [isNotificationOn, setNotification] = React.useState(false);

    const handleSwitchChange= (event: React.ChangeEvent<HTMLInputElement>) => {
        if(isNotificationOn) {
            //DisableNotifications
        }else{
            //EnableNotifications
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