import React from 'react'
import './popular.css'
import { BsArrowLeftShort } from 'react-icons/bs'
import { BsArrowRightShort } from 'react-icons/bs'
import { BsDot } from 'react-icons/bs'

//Import the images====================>
import img2 from '../../Assets/image1.jpg'
import img5 from '../../Assets/image2.jpg'
import img7 from '../../Assets/image3.jpg'
import img9 from '../../Assets/image4.jpg'


//Use high order array method to display all the destination

const Data = [
    {
        id: 1,
        imgSrc: img2,
        destTitle: 'San1',
        location: '1',
        grade: 'CULTURAL RELAX '
    },

    {
        id: 2,
        imgSrc: img2,
        destTitle: 'San2',
        location: '2',
        grade: 'CULTURAL RELAX '
    },

    {
        id: 3,
        imgSrc: img7,
        destTitle: 'San2',
        location: '2',
        grade: 'CULTURAL RELAX '
    },

    {
        id: 4,
        imgSrc: img9,
        destTitle: 'San2',
        location: '2',
        grade: 'CULTURAL RELAX '
    },
]

const Popular = () => {
    return (
        <section className='popular section container'>
            <div className="secContainer">

                <div className="secHeader flex">
                    <div className="textDiv">
                        <h2 className="secTitle">
                            Popular Badminton Branch.
                        </h2>
                        <p>
                            From historical cities to natural specteculars, come see
                            the best of the world!
                        </p>
                    </div>

                    <div className="iconsDiv flex">
                        <BsArrowLeftShort className="icon leftIcon" />
                        <BsArrowRightShort className="icon" />
                    </div>
                </div>

                <div className="mainContent grid">
                    {
                        Data.map(({id, imgSrc, destTitle, location, grade}) => {
                            return (
                                <div className="singleDestination">
                                    <div className="destImage">

                                        <img src={imgSrc} alt="Image title" />

                                        <div className="overlayInfo">
                                            <h3>{destTitle}</h3>
                                            <p>
                                                {location}
                                            </p>

                                            <BsArrowRightShort className='icon' />
                                        </div>

                                    </div>

                                    <div className="destFooter">
                                        <div className="number">
                                            0{id}
                                        </div>

                                        <div className="destText flex">
                                            <h6>
                                                {location}
                                            </h6>
                                            <span className='flex'>
                                                <span className="dot">
                                                    <BsDot className="icon" />
                                                </span>
                                                Dot
                                            </span>
                                        </div>
                                    </div>
                                </div>
                            )
                        })
                    }
                </div>

            </div>
        </section>
    )
}

export default Popular