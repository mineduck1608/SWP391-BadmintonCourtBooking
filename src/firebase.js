// Import the functions you need from the SDKs you need
import { initializeApp, getApps } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import {getAuth} from "firebase/auth";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyCmkK6jK6ZaSLjOKzNlXiXAc_RtTWK6cPs",
  authDomain: "badmintoncourt-2fc4c.firebaseapp.com",
  projectId: "badmintoncourt-2fc4c",
  storageBucket: "badmintoncourt-2fc4c.appspot.com",
  messagingSenderId: "898330108778",
  appId: "1:898330108778:web:1dcce5e295d03d644dbb13",
  measurementId: "G-YF1WDTZD8C"
};

// Initialize Firebase
let app;
if(!getApps().length) {
  app = initializeApp(firebaseConfig);
} else{
  app = getApps()[0];
}

const analytics = getAnalytics(app);
const auth = getAuth(app);

export { auth, analytics};