jQuery(function ($) {
    var adminModule = {};
    window.CSM_Admin = {
        addModule: function (name, value) {
            adminModule[name] = value;
        },
        getModule: function (name) {
            return adminModule[name];
        }
    }
})

Vue.filter('toTime', function (value) {
    if (value) {
        return moment(value).format('h:mm:ss');
    }
});