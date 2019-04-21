jQuery(function ($) {
    window.CSM.offlinePage = {
        template: "#offlinePageTemplate",
        data: function () {
            return {
                offline: true
            }
        },
        mounted: function () {
            var that = this;
            window.addEventListener('offline', function () {
                that.offline = true;
            });
            window.addEventListener('online', function () {
                that.offline = false;
            });
        },
        methods: {
            reconnect: function () {
                this.$router.push("/");
            }
        }
    };
});