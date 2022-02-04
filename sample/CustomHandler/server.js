////const express = require('express');
////var http = require('http');
////const url = require('url');
////const port = process.env.FUNCTIONS_HTTPWORKER_PORT;
////console.log("port" + port);
//////create a server object:
////http.createServer(function (req, res) {
////  const reqUrl = url.parse(req.url, true);
////  console.log("Request handler random was called.");
////    res.writeHead(200, { "Content-Type": "application/json" });
////  var json = JSON.stringify({ functionName : req.url.replace("/","")});
////  res.end(json);
////}).listen(port);

const express = require('express')
const app = express()
const port = process.env.functions_httpworker_port


// app.get('/', (req, res) => {
//   res.send('Hello World!')
// })

// app.post('/api/HttpTrigger', (req, res) => {
//   res.send('Hello World!')
// })

app.use(
  express.urlencoded({
    extended: true
  })
)

app.use(express.json())

app.post('/api/HttpTrigger', (req, res) => {
  console.log(req.body)
  res.send(req.body)
})


app.listen(port, () => {
  console.log(`Example app listening on port ${port}`)
})


// const express = require('express')
// const app = express()
// const port = process.env.functions_httpworker_port

// app.use(
//     express.urlencoded({
//         extended: true
//     })
// )

// app.use(express.json())

// app.post('/api/HttpTrigger', (req, res) => {
// })

// app.listen(port, () => {
//   console.log(`example app listening at http://localhost:${port}`);
// })