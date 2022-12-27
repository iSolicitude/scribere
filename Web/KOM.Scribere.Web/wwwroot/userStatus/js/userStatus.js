"use strict";

var userStatusConnection = new signalR.HubConnectionBuilder().withUrl("/userStatusHub").build();

userStatusConnection.start().then(function () {
    let element = document.getElementById("currentUsername");

    if (element) {
        let currentUsername = element.textContent.substr(1, element.textContent.length);

        userStatusConnection.invoke("IsUserOnline", currentUsername).catch(function (err) {
            return console.error(err.toString());
        });
    }

    let allUserStatusDots = document.querySelectorAll(".all-users-status-dots");

    if (allUserStatusDots.length > 0) {
        for (var user of allUserStatusDots) {
            let usernameElement = user.children[2].textContent; //<h5>
            let username = usernameElement.substr(1, usernameElement.length);

            userStatusConnection.invoke("IsUserOnline", username).catch(function (err) {
                return console.error(err.toString());
            });
        }
    }
}).catch(function (err) {
    return console.error(err.toString());
});

userStatusConnection.on("UserIsOnline", function (username) {
    let profileStatus = document.getElementById(`${username}userStatus`);

    if (profileStatus) {
        profileStatus.innerText = "online"; // span
        document.getElementById(`${username}userStatusDot`).style.backgroundColor = "green";
        document.getElementById(`${username}userStatusDot`).classList.add("pulse");
    }
});

userStatusConnection.on("UserIsOffline", function (username) {
    let profileStatus = document.getElementById(`${username}userStatus`);

    if (profileStatus) {
        profileStatus.innerText = "offline"; // span
        document.getElementById(`${username}userStatusDot`).style.backgroundColor = "red";
    }
});