// Import the functions you need from the SDKs you need
import {initializeApp} from "firebase/app";
import {getAnalytics} from "firebase/analytics";
import {getMessaging, getToken} from "firebase/messaging"
import {useEffect} from "react";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyCD-yzykycUz9wXu4Wv9V6LJpD74Pgbaik",
    authDomain: "patterns-959c2.firebaseapp.com",
    projectId: "patterns-959c2",
    storageBucket: "patterns-959c2.appspot.com",
    messagingSenderId: "605380956040",
    appId: "1:605380956040:web:2c5430e6e05bb254d9743d",
    measurementId: "G-9S4465239E"
};


function requestPermission() {
    if ('Notification' in window) {
        console.log("Requestion permission...")
        Notification.requestPermission().then((permission) => {
            if (permission === 'granted') {
                console.log('Notification permission granted.');
                // Initialize Firebase
                const app = initializeApp(firebaseConfig);
                const messaging = getMessaging(app)
                getToken(messaging, {vapidKey: "BO3xp05U9gbYipL9Y8Z3A0cThd37-N5Diuzzq4uH7oubKVdQXgPmcA53CncYhvklq__ge8AZsobX56BhDURqpyE"}).then((currentToken) => {
                        if (currentToken) {
                            console.log('currentToken', currentToken);
                        } else {
                            console.log("can't get token for firebase")
                        }
                    }
                );
            } else {
                console.log("Do not have permission")
            }

        })
    } else {
        console.log('This browser does not support notifications.');
    }


}

requestPermission();
// export const NotificationCheckbox = () => {
//
//     useEffect(() => {
//         if ('Notification' in window) {
//             requestPermission();
//         } else {
//             console.log('This browser does not support notifications.');
//         }
//     }, []);
//
//     return (
//         <div>
//             Notifications
//         </div>
//     );
// };