import React from 'react';

const GoogleMap = () => {
    return (
        <>
            <iframe 
                src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3918.60994153052!2d106.80730807583869!3d10.841132857995182!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752731176b07b1%3A0xb752b24b379bae5e!2sFPT%20University%20HCMC!5e0!3m2!1sen!2s!4v1719730961649!5m2!1sen!2s" 
                width="600" 
                height="450" 
                style={{ border: 0 }} 
                allowFullScreen 
                loading="lazy" 
                referrerPolicy="no-referrer-when-downgrade"
            ></iframe>
        </>
    );
}

export default GoogleMap;
