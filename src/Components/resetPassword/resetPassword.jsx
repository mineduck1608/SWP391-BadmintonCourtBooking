import { sendPasswordResetEmail } from "firebase/auth";
import React from "react";
import { auth } from "../googleSignin/config";

const ResetPassword = () => {

    const handleSubmit = async(e) => {
        e.preventDefault();
        const emailVal = e.target.email.value;
        sendPasswordResetEmail(auth, emailVal).then(data => {
            alert("Check your gmail");
        }).catch(err => {
            alert(err.code);
            console.log(err)
        })
    }
    return(
        <div>
            <h1>Reset Password</h1>
            <form onSubmit={(e) => handleSubmit(e)}>
                <input name="email"/>
                <button>Reset</button>
            </form>
        </div>
    )
}

export default ResetPassword;