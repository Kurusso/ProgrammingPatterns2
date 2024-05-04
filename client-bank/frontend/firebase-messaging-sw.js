importScripts('https://www.gstatic.com/firebasejs/8.10.0/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/8.10.0/firebase-messaging.js');

self.firebase.initializeApp({
    apiKey: "AIzaSyCD-yzykycUz9wXu4Wv9V6LJpD74Pgbaik",
    authDomain: "patterns-959c2.firebaseapp.com",
    projectId: "patterns-959c2",
    storageBucket: "patterns-959c2.appspot.com",
    messagingSenderId: "605380956040",
    appId: "1:605380956040:web:2c5430e6e05bb254d9743d",
    measurementId: "G-9S4465239E"
});

const messaging = self.firebase.messaging();