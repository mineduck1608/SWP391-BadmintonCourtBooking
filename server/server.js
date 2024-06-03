const express = require('express');
const nodemailer = require('nodemailer');
const bodyParser = require('body-parser');
const cors = require('cors');
const fs = require('fs');
const app = express();
const port = 3005;

app.use(cors());
app.use(bodyParser.json());

const usersFilePath = './server/db.json';

let otpStore = {}; // Temporary store for OTPs

const transporter = nodemailer.createTransport({
  service: 'gmail',
  auth: {
    user: 'phuclinhvip1000@gmail.com',
    pass: '123'
  }
});

app.post('/send-otp', (req, res) => {
  const { email } = req.body;
  const otp = Math.floor(100000 + Math.random() * 900000); // Generate 6-digit OTP

  const mailOptions = {
    from: 'your-email@gmail.com',
    to: email,
    subject: 'Your OTP Code',
    text: `Your OTP code is ${otp}`
  };

  transporter.sendMail(mailOptions, (error, info) => {
    if (error) {
      console.log(error);
      res.status(500).json({ message: 'Error sending OTP' });
    } else {
      otpStore[email] = otp; // Store OTP temporarily
      res.status(200).json({ message: 'OTP sent successfully' });
    }
  });
});

app.post('/verify-otp', (req, res) => {
  const { email, otp } = req.body;
  if (otpStore[email] === parseInt(otp, 10)) {
    delete otpStore[email]; // Remove OTP after verification
    res.status(200).json({ message: 'OTP verified successfully' });
  } else {
    res.status(400).json({ message: 'Invalid OTP' });
  }
});

app.post('/reset-password', (req, res) => {
  const { email, newPassword } = req.body;

  fs.readFile(usersFilePath, 'utf-8', (err, data) => {
    if (err) {
      res.status(500).json({ message: 'Error reading user data' });
      return;
    }

    const users = JSON.parse(data);
    const user = users.find(u => u.email === email);

    if (user) {
      user.password = newPassword;
      fs.writeFile(usersFilePath, JSON.stringify(users, null, 2), 'utf-8', (err) => {
        if (err) {
          res.status(500).json({ message: 'Error updating password' });
        } else {
          res.status(200).json({ message: 'Password updated successfully' });
        }
      });
    } else {
      res.status(404).json({ message: 'User not found' });
    }
  });
});

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
