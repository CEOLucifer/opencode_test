const mammoth = require('mammoth');
const path = require('path');

const docxPath = path.join(__dirname, '人脸识别系统.docx');

mammoth.extractRawText({path: docxPath})
  .then(result => {
    console.log(result.value);
  })
  .catch(err => {
    console.error('Error:', err);
  });
