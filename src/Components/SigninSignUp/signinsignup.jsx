import React from "react";
import Login from "../Login/login";
import './signinsignup.css'
import { MdSportsTennis } from "react-icons/md";

const SignInSignUp = () => {
    return (
        <div>
            <div className="body">
                <div className="body-element">
                    <div className="text-element">
                    <a href="./" className="logo">
                        <h1><MdSportsTennis className="icon" />
                            BMTC
                        </h1>
                    </a>
                    <h1>
                        Hệ Thống Chuỗi Sân Cầu Lông BMTC
                    </h1>
                    <h3>Mô tả chung</h3>
                    <p>Chào mừng bạn đến với hệ thống chuỗi sân cầu lông BMTC, nơi mang đến cho bạn những trải nghiệm thể thao đỉnh cao và không gian luyện tập thoải mái. Với ba chi nhánh tọa lạc tại những vị trí thuận lợi, chúng tôi tự hào là lựa chọn hàng đầu của cộng đồng yêu thích cầu lông trong khu vực.</p>
                    <h4>Chi nhánh 1: </h4>
                    <h5>Địa chỉ: </h5>
                    <h4>Chi nhánh 2: </h4>
                    <h5>Địa chỉ: </h5>
                    <h4>Chi nhánh 3: </h4>
                    <h5>Địa chỉ: </h5>
                    <h3>Liên hệ</h3>
                    <p>Hotline:</p>
                    <p>Email:</p>
                    <p>Website:</p>
                    </div>
                    <div className="login">
                        <Login/>
                    </div>
                </div>
            </div>
        </div>

    );
}
export default SignInSignUp