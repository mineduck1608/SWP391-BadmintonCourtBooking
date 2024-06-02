import { sendPasswordResetEmail } from "firebase/auth";
import React from "react";
import { auth } from "../googleSignin/config";
import { toast } from "react-toastify";

const ResetPassword = () => {

    const handleSubmit = async(e) => {
        e.preventDefault();
        const emailVal = e.target.email.value;
        sendPasswordResetEmail(auth, emailVal).then(data => {
            toast.success("Check your gmail to reset password.");
            console.log(emailVal);
        }).catch(err => {
            toast.warning("Unsuccess.")
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