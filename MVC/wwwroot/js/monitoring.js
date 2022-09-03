// util
function getRandomInt(maxExclusive) {
    return Math.floor(Math.random() * maxExclusive); // [0; maxExclusive)
}

// general init
const DepartmentStatus = { 0: 'Blocked', 1: 'Active' };
const DepartmentStatusReversed = { 'Blocked': 0, 'Active': 1 };

let connectButton = document.getElementById("btnConnect");
let textarea = document.getElementById("text");

let $departments = document.getElementsByClassName("DepartmentName");
let count = $departments.length;

// $status init & randomize
let $status = document.getElementsByClassName("DepartmentStatus");
for (let i = 0; i < count; i++) {
    $status[i].innerHTML = DepartmentStatus[getRandomInt(2)];
}

// mapping init
let departmentsNamesList = []; // as keys for 2 maps
for (let i = 0; i < count; i++) {
    departmentsNamesList.push($departments[i].getAttribute("id"));
}

// status ui map (key => Name, value => $status); status ui references
let deparmentsStatusUIMap = new Map();
for (let i = 0; i < count; i++) {
    deparmentsStatusUIMap.set(departmentsNamesList[i], $status[i]);
}

// status api map (key => Name, integer value => status); status values
let departmentsStatusAPIMap = new Map();
for (let i = 0; i < count; i++) {
    departmentsStatusAPIMap.set(departmentsNamesList[i],
        DepartmentStatusReversed[$status[i].innerHTML.toString()]);
}

// api
let jsonNameStatusValuesMap = JSON.stringify(Object.fromEntries(departmentsStatusAPIMap));

connectButton.onclick = function () {
    socket = new WebSocket("wss://localhost:7110/ws");
    socket.onopen = function (event) {
        socket.send(jsonNameStatusValuesMap);
        // socket.send(jsonDepartmentsNamesList);
        //for (const [key, value] of Object.entries(jsonDepartmentsNamesList)) {
        //    console.log(value);
        //}
    };
    socket.onclose = function (event) {
        console.log("closed");
    };
    socket.onerror = function (event) {
        console.log(event.data);
    };
    socket.onmessage = function (event) {
        // textarea.value = event.data;
        let object = JSON.parse(event.data);
        for (const [key, value] of Object.entries(object)) {
            // console.log(key, value);
            deparmentsStatusUIMap.get(key).innerHTML = DepartmentStatus[value];
        }
    };

    setInterval(() => {
        socket.send('');
    }, 3000);
};