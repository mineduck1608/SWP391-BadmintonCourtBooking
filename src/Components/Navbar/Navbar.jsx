import React, { useEffect, useState } from 'react'
import './navbar.css'
import { MdSportsTennis } from "react-icons/md";
import { AiFillCloseCircle } from "react-icons/ai";
import { TbGridDots } from "react-icons/tb";
import { Link } from "react-router-dom";


const Navbar = () => {

    //Code to toggle/show navBar
    const [active, setActive] = useState('navBar')
    const showNav = () => {
        setActive('navBar activeNavbar')
    }

    // Code to remove Navbar
    const removeNav = () => {
        setActive('navBar ')
    }

    //Code to add background color to the header...
    const [transparent, setTransparent] = useState('header1')
    const addBg = ()=>{
        if (window.scrollY >= 10) {
            setTransparent('header1 activeHeader')
        }
        else {
            setTransparent('header1')
        }
    }
    window.addEventListener('scroll', addBg);

    return (
        <section className='navBarSection'>
            <div className={transparent}>
                <div className="logoDiv">
                    <Link to={'/'} className="logo">
                        <h1 className='flex'><MdSportsTennis className="icon" />
                            BMTC
                        </h1>
                    </Link>
                </div>

                <div className={active}>
                    <ul className="navLists flex">
                        <div className="headerBtns flex">
                            <button className='btn loginBtn'>
                                <Link to={'/signin'}>Login</Link>
                            </button>
                            <button className='btn '>
                                <Link to={'/signup'}>Sign Up</Link>
                            </button>
                        </div>

                    </ul>

                    <div onClick={removeNav} className="closeNavbar">
                        <AiFillCloseCircle className="icon" />
                    </div>
                </div>

                <div onClick={showNav} className="toggleNavbar">
                    <TbGridDots className="icon" />

                </div>
            </div>
        </section>
    )
}

export default Navbar
