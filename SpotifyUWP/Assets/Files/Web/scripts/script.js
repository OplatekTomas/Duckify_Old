function init() {
    document.querySelector("#searchInput").oninput = serachApi;
    //Hide search result table - it will be visible again when I need it
    document.getElementById("searchResultTable").style.display = "none";
    getQueue();
    setInterval(getQueue, 3000);
}

function serachApi(e) {
    $.getJSON('/api/search?query=' + e.target.value, function (data) {
        document.getElementById("searchTableBod").innerHTML = "";
        generateSearchResults(data);
    });
}

function getQueue() {
    console.log("send req");
    $.getJSON('/api/queue', function (data) {
        document.getElementById("tableBod").innerHTML = "";
        generateTable(data);
    });
}

function generateSearchResults(data) {
    var table = document.getElementById("searchResultTable");
    if (data.length > 0) {
        table.style.display = "";
    } else {
        table.style.display = "none";
    }
    data.forEach(element => {
        var table = document.getElementById("searchTableBod");
        var row = table.insertRow(-1);
        //create cell at index 0
        var coverCell = row.insertCell(0);
        coverCell.setAttribute("class", "align-middle");
        coverCell.innerHTML = "<img height='64px' width='64px' src='" + element.coverUrl + "' />";
        //create cell at index 1
        var infoCell = row.insertCell(1);
        infoCell.setAttribute("class", "align-middle demo");
        infoCell.innerHTML = "<span class='font-weight-bold'>" + element.songName + "</span><div><span>" + element.artistName + "</span></div >";
        //craete cell at index 2
        var coverCell = row.insertCell(2);
        coverCell.setAttribute("class", "align-middle");
        var btn = document.createElement("button");
        btn.setAttribute("class", "btn btn-outline-success");
        btn.innerHTML = "Add";
        btn.onclick = function () {
            addSong(element.songId)
        }
        coverCell.appendChild(btn);


    });
}


//Yes. It's ugly.
function generateTable(data) {

    data.forEach(element => {
        var table = document.getElementById("tableBod");
        var row = table.insertRow(-1);
        //create cell at index 0
        var coverCell = row.insertCell(0);
        coverCell.setAttribute("class", "align-middle");
        coverCell.innerHTML = "<img height='64px' width='64px' src='" + element.coverUrl + "' />";
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

function addSong(data) {
    sendLike(data);
    document.getElementById("searchTableBod").innerHTML = "";
    document.getElementById("searchResultTable").style.display = "none";
    document.getElementById("searchInput").value = "";

}

function sendLike(data) {
    $.getJSON('/api/like?songId=' + data, function (resopnse) {
        setTimeout(function () {
            getQueue();
        }, 200);
    });

}