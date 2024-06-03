import { sendPasswordResetEmail } from "firebase/auth";
import React, { useState } from "react";
import { auth } from "../googleSignin/config";
import { toast } from "react-toastify";

const ResetPassword = () => {
    const [email, emailChange] = useState("");

    const handleSubmit = async(e) => {
        e.preventDefault();
        sendPasswordResetEmail(auth, email).then(data => {
            toast.success("Check your gmail to reset password.");
            console.log(email);
        }).catch(err => {
            toast.warning("Unsuccess.")
        })
    }
    return(
        <div>
            <h1>Reset Password</h1>
            <form onSubmit={handleSubmit}>
            <input value={email} onChange={e => emailChange(e.target.value)} type="email" placeholder="Email" required />
                <button>Reset</button>
            </form>
        </div>
    )
}

export default ResetPassword;