jQuery(function ($) {
    //filters
    Vue.filter('toTime', function (value) {
        if (value) {
            return moment(value).format('h:mm:ss');
        }
    });
});