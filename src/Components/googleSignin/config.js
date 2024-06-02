
import { initializeApp } from "firebase/app";
import { getAuth, GoogleAuthProvider } from "firebase/auth";

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
const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
const provider = new GoogleAuthProvider();

export {auth, provider};
