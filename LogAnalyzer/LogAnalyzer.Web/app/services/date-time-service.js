
app.factory('dateTimeService', function () {
    return {
        combineDateAndTime: function (date, time) {
            var timeFormat = "h:mm a";
            var dateFormat = "YYYY-MM-DD";

            var timeLoad = moment(time).isValid() ? moment(time).format(timeFormat) : null;
            var dateLoad = moment(date).isValid() ? moment(date).format(dateFormat) : null;
            
            if (!dateLoad) {
                return "";
            }

            return (dateLoad + (timeLoad != null ? " " + timeLoad : ''));
        },
    }
})