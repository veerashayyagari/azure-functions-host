const express = require('express')
const app = express()
const port = process.env.functions_httpworker_port || 5001


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
  res.send("enable http forwarding is true")
})


app.get('/HttpTrigger', (req, res) => {
  let retryCount = req?.body?.Metadata?.RetryContext?.RetryCount || 0;
  let maxRetry = req?.body?.Metadata?.RetryContext?.MaxRetryCount;
  let exception = req?.body?.Metadata?.RetryContext?.Exception?.message || '';
  let errorString = 'An error occurred';
  let json = JSON.stringify({ functionName: req.url.replace("/", ""), retryCount, maxRetry });
  if (retryCount < maxRetry) {
    res.status(500).send(x)
  }
  else{
    res.status(200).send(x)
  }
})

var x = {
  "returnValue": "hello",
  "outputs": {
    "key1": "value"
  }
}

app.listen(port, () => {
  console.log(`Example app listening on port ${port}`)
})
