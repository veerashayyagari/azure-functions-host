 const express = require('express');
 const bodyparser = require('body-parser');
 const fs = require('fs')

 const app = express();
 const port = process.env.functions_httpworker_port || 5000;

 fs.writefile('c:\\users\\magordon\\repos\\azure-functions-host\\sample\\customhandlerretry\\serveroutput.txt', port, err => {})


 app.use(bodyparser.json()) // for parsing application/json

 // app.post('/api/httptrigger', (req, res) => {
 //     let retrycount = req?.body?.metadata?.retrycontext?.retrycount || 0;
 //     let maxretry = req?.body?.metadata?.retrycontext?.maxretrycount;
 //     let exception = req?.body?.metadata?.retrycontext?.exception?.message || '';
 //     let errorstring = 'an error occurred';
 //     let json = json.stringify({ functionname: req.url.replace("/", ""), retrycount, maxretry });
 //     res.send(json);
 // })

 app.get('/api/httptrigger', (req, res) => {
     res.status(200).send();
 })

 try {
     app.listen(port, () => {
         console.log(`example app listening at http://localhost:${port}`);
        // fs.writefile('c:\\users\\magordon\\repos\\azure-functions-host\\sample\\customhandlerretry\\serveroutput.txt', ${port}, err => {})

     })
 } catch (e) {
     fs.writefile('c:\\users\\magordon\\repos\\azure-functions-host\\sample\\customhandlerretry\\serveroutput.txt', e, err => {})
 }

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