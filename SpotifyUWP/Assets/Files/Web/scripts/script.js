function init() {
    sendRequest();
    setInterval(sendRequest, 3000);
}

function sendRequest() {
    console.log("send req");
    $.getJSON('http://192.168.1.83:5850/api/queue', function (data) {
        $("#tableBod tr").remove();
        generateTable(data);
    });
}

//Yes. It's ugly.
function generateTable(data) {
    data.forEach(element => {
        var table = document.getElementById("contentTable");
        var row = table.insertRow(-1);
        //create cell at index 0
        var coverCell = row.insertCell(0);
        coverCell.setAttribute("class", "align-middle");
        coverCell.innerHTML = "<img height='64px' width='64px' src='" + element.coverUrl + "'></img>";
        //create cell at index 1
        var infoCell = row.insertCell(1);
        infoCell.setAttribute("class", "align-middle demo");
        infoCell.innerHTML = "<span class='font-weight-bold'>" + element.songName + "</span><div><span>" + element.artistName + "</span></div >";
        //create cell at index 2
        var likesCell = row.insertCell(2);
        likesCell.setAttribute("class", "align-middle");
        likesCell.innerHTML = element.numberOfLikes;
        //craete cell at index 3
        var coverCell = row.insertCell(3);
        coverCell.setAttribute("class", "align-middle");
        var btn = document.createElement("button");
        btn.setAttribute("class", "btn btn-outline-danger");
        btn.innerHTML = "Like";
        btn.onclick = function () {
            sendLike(element.songId)
        }
        coverCell.appendChild(btn);
    });

}

function sendLike(data) {
    alert(data);

}