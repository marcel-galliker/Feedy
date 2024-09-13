
var websocket = new WebSocket("ws://127.0.0.1:12345");

websocket.onmessage = (e) => {
    var lines = e.data.split('\n');
    for (var i = 0; i < lines.length; i++) {
        var args = lines[i].split('=');
        var ctrl = document.getElementById(args[0]);
        if (ctrl!=null) ctrl.textContent = args[1];
    }
};
