const http = require("http");
http.createServer(function (request, response) {

    response.end("Connnection Server Status: Online");

}).listen(3000, "127.0.0.1", function () {
    console.log("Server starting on port - 3000");
});