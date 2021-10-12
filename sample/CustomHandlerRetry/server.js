// const express = require('express');
// const bodyParser = require('body-parser');
// const fs = require('fs')

// const app = express();
// const port = process.env.FUNCTIONS_HTTPWORKER_PORT || 5000;

// fs.writeFile('C:\\Users\\magordon\\Repos\\azure-functions-host\\sample\\CustomHandlerRetry\\serveroutput.txt', port, err => {})


// app.use(bodyParser.json()) // for parsing application/json

// // app.post('/api/httptrigger', (req, res) => {
// //     let retryCount = req?.body?.Metadata?.RetryContext?.RetryCount || 0;
// //     let maxRetry = req?.body?.Metadata?.RetryContext?.MaxRetryCount;
// //     let exception = req?.body?.Metadata?.RetryContext?.Exception?.message || '';
// //     let errorString = 'An error occurred';
// //     let json = JSON.stringify({ functionName: req.url.replace("/", ""), retryCount, maxRetry });
// //     res.send(json);
// // })

// app.get('/api/httptrigger', (req, res) => {
//     res.status(200).send();
// })

// try {
//     app.listen(port, () => {
//         console.log(`Example app listening at http://localhost:${port}`);
//        // fs.writeFile('C:\\Users\\magordon\\Repos\\azure-functions-host\\sample\\CustomHandlerRetry\\serveroutput.txt', ${port}, err => {})

//     })
// } catch (e) {
//     fs.writeFile('C:\\Users\\magordon\\Repos\\azure-functions-host\\sample\\CustomHandlerRetry\\serveroutput.txt', e, err => {})
// }

var http = require('http');
const url = require('url');
const port = process.env.FUNCTIONS_HTTPWORKER_PORT;
console.log("port" + port);
//create a server object:
http.createServer(function (req, res) {
  const reqUrl = url.parse(req.url, true);
  console.log("Request handler random was called.");
  res.writeHead(200, {"Content-Type": "application/json"});
  var json = JSON.stringify({ functionName : req.url.replace("/","")});
  res.end(json);
}).listen(port); 