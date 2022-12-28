"use strict";

var notificationConnection  = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

var sound = new Audio('/notification/notificationSoundMessage.mp3');

notificationConnection.start().then(function () {
    let playNotificationSound = false;

    notificationConnection.invoke("GetUserNotificationsCount", playNotificationSound).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

notificationConnection.on("ReceiveNotification", function (count, playNotificationSound) {
    document.getElementById("notificationCount").innerText = count; // span _NotificationBadgePartial

    let title = document.querySelector("head title"); // head
    let bracketIndex = title.innerText.indexOf(")");
    let newTitle = "";

    if (count > 0) {
        if (playNotificationSound) {
            sound.play();
        }

        document.getElementById("notificationCount").classList.add("notificationCircle", "notificationPulse");
        newTitle = `(${count}) ${title.innerText.substring(bracketIndex + 1, title.innerText.length)}`;
    } else {
        document.getElementById("notificationCount").classList.remove("notificationCircle", "notificationPulse");
        newTitle = `${title.innerText.substring(bracketIndex + 1, title.innerText.length)}`;
    }

    title.innerText = newTitle;
});

notificationConnection.on("VisualizeNotification", function (notification) {
    let section = document.getElementById("allUserNotifications"); // section Notifications/Index.cshtml

    if (section) {
        let newNotification = createNotification(notification);

        if (section.children.length == 0) {
            section.appendChild(newNotification);
        } else {
            section.insertBefore(newNotification, section.childNodes[0]);
        }
    }
});

function createNotification(notification) {
    let newNotification = document.createElement("article");
    let dateTime = new Date();
    let formattedDate =
        `${dateTime.getDate()}-${(dateTime.getMonth() + 1)}-${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}:${dateTime.getSeconds()}`;

    newNotification.id = notification.id;

    newNotification.innerHTML =
        `<section class="notification-container-content">
                    <img src="https://res.cloudinary.com/dxfq3iotg/image/upload/v1574583246/AAA/2.jpg" alt="avatar" />
                    <header>
                        <h4 class="text-primary">
                            <span class="heading-span">
                                <a class="delete-notification-button" onclick="deleteNotification('${notification.id}')">
                                    <i class="fas fa-trash-alt"></i>
                                </a>
                                <span>${notification.heading}</span>
                            </span>
                        </h4>
                    </header>
                    <main>
                        <p class="notification-text">${notification.text}</p>
                    </main>
                    <footer>
                        <span class="time-span">
                            <i class="far fa-clock"></i>
                            <span>${formattedDate}</span>
                        </span>
                    </footer>
                </section>`;

    return newNotification;
}