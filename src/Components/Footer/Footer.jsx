import React from 'react'
import './footer.css'
import { MdSportsTennis } from "react-icons/md";
import { ImFacebook } from 'react-icons/im';
import { BsTwitter } from 'react-icons/bs';
import { AiFillInstagram } from 'react-icons/ai';

const Footer = () => {
    return (
        <div className='footer'>
            <div className="secContainer container grid">
                <div className="logoDiv">

                    <div className="footerLogo">
                        <a href="#" className="logo flex">
                            <h1 className='flex'><MdSportsTennis className="icon" />
                                Dot</h1>
                        </a>
                    </div>

                    <div className="socials flex">
                        <ImFacebook className="icon" />
                        <BsTwitter className="icon" />
                        <AiFillInstagram className="icon" />
                    </div>

                </div>


                <div className="footerLinks">
                    <span className="linkTitle">
                        Information
                    </span>
                    <li>
                        <a href="#">Home</a>
                    </li>
                    <li>
                        <a href="#">Terms & Conditions</a>
                    </li>
                    <li>
                        <a href="#">Privacy</a>
                    </li>
                    <li>
                        <a href="#">Courts</a>
                    </li>
                </div>

                <div className="footerLinks">
                    <span className="linkTitle">
                        Contact Us
                    </span>
                <span className='phone'>+899 504 918</span>
                <span className='email'>linhnhpse183865@fpt.edu.vn</span>
                </div>
            </div>
        </div>
    )
}
export default Footer