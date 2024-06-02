// Import the functions you need from the SDKs you need
import { initializeApp, getApps } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import { getAuth } from "firebase/auth";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
    apiKey: "AIzaSyAnrdte2F_VtOXJjqaq4m7cUgzIvYe3MaU",
    authDomain: "badmintoncourtbooking-183b2.firebaseapp.com",
    projectId: "badmintoncourtbooking-183b2",
    storageBucket: "badmintoncourtbooking-183b2.appspot.com",
    messagingSenderId: "859078707099",
    appId: "1:859078707099:web:185eae36409c2810be833e",
    measurementId: "G-51XHF6B2B5"
};

// Initialize Firebase
let app;
if (!getApps().length) {
    app = initializeApp(firebaseConfig);
} else {
    app = getApps()[0];
}

const analytics = getAnalytics(app);
const auth = getAuth(app);

export { auth, analytics };