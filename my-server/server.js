const express = require('express');
const multer = require('multer');
const path = require('path');
const cors = require('cors'); 

const app = express();
const PORT = 8080;
app.use(cors());
const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    cb(null, 'uploads');
  },
  filename: (req, file, cb) => {
    cb(null, Date.now() + path.extname(file.originalname));
  }
});

const upload = multer({ storage });

app.use(express.static(__dirname));

app.post('/uploads', upload.single('receipt'), (req, res) => {
  if (req.file) {
    res.json({ filePath: `http://localhost:${PORT}/uploads/${req.file.filename}` });
  } else {
    res.status(400).send('No file uploaded.');
  }
});

app.listen(PORT, () => {
  console.log(`Server is running on http://localhost:${PORT}`);
});