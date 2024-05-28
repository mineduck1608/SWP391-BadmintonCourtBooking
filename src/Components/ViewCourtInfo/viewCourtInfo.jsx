import React from "react";
import Header from "../Header/header";
import './viewCourtInfo.css'
import Footer from "../Footer/Footer";
import image2 from '../../Assets/image2.jpg';

const ViewCourtInfo = () => {
    return (
        <div className="viewcourtinfo">
            <div>
                <Header />
            </div>
            <div className="viewcourtinfo-body">
                <div className="viewcourtinfo-body-pic">
                    <img className="viewcourtinfo-img" src={image2} alt="" />
                    <div className="viewcourtinfo-body-des">
                        <h1>Description:</h1>
                        <p>Hi chung minh la</p>
                    </div>
                </div>
                <div className="viewcourtinfo-info">
                    <h2>Name: </h2>
                    <p>Address: Name</p>
                    <p>Time: AAAAA</p>
                    <p>Branch: BBBB</p>
                    <p>Status: FREE</p>
                </div>
            </div>
            <div className="viewcourtinfo-feedback">
                   
            </div>
            <div className="viewcourtinfo-footer">
                <Footer />
            </div>
        </div>
    );
}

export default ViewCourtInfo