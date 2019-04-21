jQuery(function ($) {
    const routes = [
        { name: "default", path: '/', component: window.CSM.readingsPage },
        { name: "offline", path: '/offline', component: window.CSM.offlinePage }
    ]

    const router = new VueRouter({
        mode: 'history',
        routes
    })

    const app = new Vue({
        router
    }).$mount("#app");

    window.addEventListener('offline', function () {
        if (router.history.current.path != "/offline") {
            router.push("/offline")
        }
    });
});