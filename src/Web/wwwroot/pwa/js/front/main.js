jQuery(function ($) {
    window.CSM = {
        askForPermissioToReceiveNotifications: function () {
            return new Promise(function (resolve, reject) {
                const messaging = firebase.messaging();
                messaging.requestPermission().then(function () {
                    messaging.getToken().then(function (token) {
                        console.log('token do usuário:', token);
                        resolve(token);
                    }).catch(reject);;
                }).catch(function (error) {
                    console.error(error);
                    reject();
                });
            })
        }
    }
});

