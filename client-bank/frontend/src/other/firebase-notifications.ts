
import {initializeApp} from "firebase/app";
import {getMessaging, getToken,onMessage} from "firebase/messaging"
import {Currency, OperationType} from "../api/account";



export const firebaseConfig = {
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
                            setFirebaseToken(currentToken)
                        } else {
                            console.log("can't get token for firebase")
                        }
                    }
                );

                // Handle incoming messages
                onMessage(messaging, (payload) => {
                    console.log('Message received. ', payload.notification?.body);
                    // Customize notification here
                    const data = JSON.parse(<string>payload.notification?.body);
                    const {UserId, Id, AccountId, OperationType: operationTypeMessage, MoneyAmmount, MoneyAmmountInAccountCurrency, CreationTime} = data;

                    const operationType = OperationType[operationTypeMessage as keyof typeof OperationType];

                    const notificationTitle = `New Operation ${operationType }`;
                    const notificationOptions = {
                        body: `Money Transfer: ${MoneyAmmount.Amount} ${Currency[MoneyAmmount.Currency]}\n`
                    };

                    if (Notification.permission === "granted") {
                        var notification = new Notification(notificationTitle, notificationOptions);
                    }
                });
            } else {
                console.log("Do not have permission")
            }

        })
    } else {
        console.log('This browser does not support notifications.');
    }
}







export function getFirebaseToken(){
    let firebaseToken=localStorage.getItem("firebaseToken");
    if(!firebaseToken){
        console.error("Firebase token not found");
        throw new Error('Failed to get firebase token')
    }
    firebaseToken = firebaseToken.replace(/"/g, '');
    console.log("firebaseToken: "+firebaseToken);
    return firebaseToken;
}
function setFirebaseToken(firebaseToken:string) {
    localStorage.setItem('firebaseToken', JSON.stringify(firebaseToken));
}

requestPermission();


