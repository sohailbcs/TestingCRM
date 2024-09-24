"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

// Variable to keep track of the notification count
var notificationCount = 0;

// Log connection state changes
connection.onclose(function (error) {
    console.log("Connection closed. Trying to restart...");
    if (error) {
        console.error(error);
    }
    // Attempt to restart the connection after a delay
    setTimeout(function () {
        connection.start().then(function () {
            console.log("Connection restarted.");
            // Invoke LoadNotifications once the connection is re-established
            connection.invoke("LoadNotifications");
        }).catch(function (err) {
            console.error(err.toString());
        });
    }, 5000); // 5 seconds delay, adjust as needed
});

// Log received notifications
connection.on("ReceiveNotifications", function (notifications) {
    console.log("Received notifications:", notifications);
    // Filter out already read notifications
    var unreadNotifications = notifications.filter(function (notification) {
        return !notification.isRead; // Assuming there is an isRead property in your Notification model
    });

    if (unreadNotifications.length > 0) {
        notificationCount += unreadNotifications.length;
        // Update the notification number in the UI
        updateNotificationNumber();
    }

    // Clear existing notifications in the view
    var msgList = document.getElementById("msgList");
    if (!msgList) {
        console.error("msgList element not found in the document.");
        return;
    }
    msgList.innerHTML = "";

    // Display notifications in the view
    notifications.forEach(function (notification) {
        var li = document.createElement("li");
        li.style.display = "flex";
        li.style.alignItems = "center";
        li.style.padding = "10px 15px";
        li.style.borderBottom = "1px solid #e5e5e5";

        var icon = document.createElement("i");
        icon.className = "bi bi-exclamation-circle text-warning";
        icon.style.fontSize = "24px";

        var contentDiv = document.createElement("div");
        contentDiv.style.marginLeft = "15px";
        contentDiv.style.flex = "1";

        var title = document.createElement("h4");
        title.style.margin = "0";
        title.style.fontSize = "16px";
        title.style.fontWeight = "bold";
       

        var message = document.createElement("p");
        message.style.margin = "0";
        message.style.fontSize = "14px";
        message.style.color = "#6c757d";
        message.textContent = notification.message;

        var timestamp = document.createElement("p");
        timestamp.style.margin = "0";
        timestamp.style.fontSize = "12px";
        timestamp.style.color = "#999";
        timestamp.textContent = formatDate(notification.notificationTime); // Format the notification time

        contentDiv.appendChild(title);
        contentDiv.appendChild(message);
        contentDiv.appendChild(timestamp);

        li.appendChild(icon);
        li.appendChild(contentDiv);

        // Append horizontal rule after each notification
        var hr = document.createElement("hr");
        hr.style.margin = "0";
        hr.style.border = "none";
        hr.style.borderBottom = "1px solid #e5e5e5";

        msgList.appendChild(li);
        msgList.appendChild(hr);
    });

    // Update the notification number in the UI
    updateNotificationNumber();
});

// Function to format the notification time
function formatDate(date) {
    if (!date) return "";
    var options = { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' };
    return new Date(date).toLocaleDateString(undefined, options);
}

// Function to update the notification number in the UI
function updateNotificationNumber() {
    var badgeNumber = document.querySelector("#notificationIcon .badge-number");
    if (!badgeNumber) {
        console.error("badgeNumber element not found in the document.");
        return;
    }
    badgeNumber.textContent = notificationCount;
    badgeNumber.style.display = notificationCount > 0 ? "inline" : "none";
}

// Event handler for clicking the bell icon
document.querySelector("#notificationIcon").addEventListener("click", function () {
    notificationCount = 0;
    updateNotificationNumber();
});

// Start the connection
connection.start().then(function () {
    console.log("Connection started.");
    // Invoke LoadNotifications once the connection is established
    connection.invoke("LoadNotifications").then(function () {
        console.log("LoadNotifications invoked successfully hi.");
    }).catch(function (err) {
        console.error("Error invoking LoadNotifications:", err.toString());
    });
}).catch(function (err) {
    console.error("Error starting connection:", err.toString());
});
