const express = require('express');
const bodyParser = require('body-parser');
const nodemailer = require('nodemailer');
const fs = require('fs');
const app = express();
const cors = require('cors');
const port = 3005;

app.use(cors());
app.use(bodyParser.json());

const transporter = nodemailer.createTransport({
  service: 'Gmail',
  auth: {
    user: 'linhnhpse183865@fpt.edu.vn',
    pass: 'hqyt dkwa kapf quzu'
  }
});

let db = JSON.parse(fs.readFileSync('db.json', 'utf-8'));

app.post('/send-otp', (req, res) => {
  const { email } = req.body;
  const user = db.useraccount.find(user => user.email === email);

  if (!user) {
    return res.status(404).json({ message: 'User not found' });
  }

  const otp = Math.floor(100000 + Math.random() * 900000).toString();
  user.otp = otp;
  fs.writeFileSync('db.json', JSON.stringify(db, null, 2));

  const mailOptions = {
    from: 'your-email@gmail.com',
    to: email,
    subject: 'Your OTP Code',
    text: `Your OTP code is ${otp}`
  };

  transporter.sendMail(mailOptions, (error, info) => {
    if (error) {
      console.error(error);
      return res.status(500).json({ message: 'Error sending OTP' });
    }
    res.json({ message: 'OTP sent successfully' });
  });
});

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
