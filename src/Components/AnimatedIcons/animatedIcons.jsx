import React from 'react';
import './animatedIcons.css';
import FacebookLogo from '../../Assets/facebook.png'; // Update the path according to your project structure
import MessengerLogo from '../../Assets/messenger.png'; // Update the path according to your project structure

const AnimatedIcons = () => {
  return (
    <div className="icon-container">
      <a href="https://www.facebook.com/profile.php?id=61563063866196" target="_blank" rel="noopener noreferrer" className="facebook-icon">
        <img src={FacebookLogo} alt="Facebook" className="facebook-image" />
      </a>
      <a href="https://www.facebook.com/messages/t/382281944965988" target="_blank" rel="noopener noreferrer" className="messenger-icon">
        <img src={MessengerLogo} alt="Messenger" className="messenger-image" />
      </a>
    </div>
  );
};

export default AnimatedIcons;
