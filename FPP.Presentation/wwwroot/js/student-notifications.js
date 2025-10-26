// student-notifications.js
(function () {
    'use strict';

    // Khởi tạo SignalR connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationBookingHub")
        .withAutomaticReconnect()
        .build();

    // Lắng nghe sự kiện ReceiveNotification
    connection.on("ReceiveStudentNotification", function (notification) {
        console.log("Student received notification:", notification);
        showStudentNotification(notification);
        //if (notification.type === "BookingStatus") {
            
        //}
    });

    // Bắt đầu kết nối
    connection.start()
        .then(() => console.log("Student SignalR Connected!"))
        .catch(err => {
            console.error("SignalR Error:", err);
            setTimeout(() => connection.start(), 5000);
        });

    // Function hiển thị toast notification
    function showStudentNotification(notification) {
        const toast = document.createElement('div');
        toast.className = 'notification-toast';

        const isApproved = notification.status === "Approved";
        const iconClass = isApproved ? 'fa-check-circle' : 'fa-times-circle';
        const iconColor = isApproved ? '#4caf50' : '#f44336';

        const details = notification.bookingDetails || {};

        toast.innerHTML = `
            <div class="toast-header" style="color: ${iconColor};">
                <i class="fas ${iconClass}"></i>
                <strong>${notification.title || 'Notification'}</strong>
                <span class="toast-time">${new Date().toLocaleTimeString()}</span>
            </div>
            <div class="toast-body">
                <p><strong>${notification.message || ''}</strong></p>
                <small>
                    🏢 ${details.lab || ''} - ${details.zone || ''}<br>
                    📅 ${details.date || ''}<br>
                    ⏰ ${details.startTime || ''} - ${details.endTime || ''}<br>
                    📝 ${details.activity || ''}
                    ${details.reason ? '<br>❌ Reason: ' + details.reason : ''}
                </small>
            </div>
        `;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'slideOut 0.5s ease-out';
            setTimeout(() => toast.remove(), 500);
        }, 7000);

        // Play sound based on status
        playNotificationSound(isApproved);

        // Refresh bookings if on My Bookings page
        if (window.location.pathname.includes('MyBookings')) {
            setTimeout(() => location.reload(), 2000);
        }
    }

    function playNotificationSound(isApproved) {
        try {
            const audioContext = new (window.AudioContext || window.webkitAudioContext)();
            const oscillator = audioContext.createOscillator();
            const gainNode = audioContext.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(audioContext.destination);

            oscillator.frequency.value = isApproved ? 1000 : 400;
            oscillator.type = 'sine';

            gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.3);

            oscillator.start(audioContext.currentTime);
            oscillator.stop(audioContext.currentTime + 0.3);
        } catch (e) {
            console.log('Could not play sound:', e);
        }
    }

    // Reconnect when page becomes visible
    document.addEventListener('visibilitychange', function () {
        if (!document.hidden && connection.state === signalR.HubConnectionState.Disconnected) {
            console.log("Page visible, reconnecting...");
            connection.start();
        }
    });

    // Export cho window nếu cần dùng từ nơi khác
    window.studentNotifications = {
        connection: connection,
        showNotification: showStudentNotification
    };
})();