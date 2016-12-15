
app.factory('errorsChartService', function ($http, formsUtilService) {

    var chartMinutesInterval = 60,
        timeLabelFormats = 'HH',
        datetimeLabelFormat = 'YYYY-MM-DD HH:mm';

    //Uses this component : http://api.highcharts.com/highcharts
    var initialiseChart = function($scope) {

        //Define Hihghtchart object  
        $scope.chartConfig = {
            options: {
                chart: {
                    renderTo: 'container',
                    type: 'column',
                    animation: true,
                    height: 220,
                    backgroundColor: 'transparent',
                    container: {
                        onclick: null
                    }
                },
                colors: ['#FF0000'],
                tooltip: {
                    enabled: false
                },
                title: {
                    text: ''
                },
                legend: {
                    enabled: false
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    },
                    series: {
                        states: { hover: 'none' },
                        dataLabels: {
                            enabled: true,
                            style: {
                                fontWeight: 'bold'
                            },
                            formatter: function() {
                                if (this.y != 0) {
                                    return this.y;
                                } else {
                                    return null;
                                }
                            }
                        }
                    }
                },
            },
            xAxis: {
                type: 'category',
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Errors'
                }
            },
            series: [{
                    animation: true,
                    cursor: 'pointer',
                    events: {
                        click: function(ev) {
                            var opt;
                            if (ev.explicitOriginalTarget) {
                                opt = ev.explicitOriginalTarget.point.options;
                            } else {
                                opt = event.srcElement.point;
                            }

                            if (opt) {
                                chartClick(opt, $scope);
                            }
                        }
                    }
                }
            ]
        };

    },
        chartClick = function(data, $scope) {
            if (data) {
                var dateFrom = data.dateFrom;
                var dateTo = data.dateTo;

                $scope.Query = 'Level == "Error"';
                $scope.LoadFrom = moment(dateFrom).format("YYYY-MM-DD");
                $scope.LoadTo = moment(dateTo).format("YYYY-MM-DD");

                $scope.TimeLoadFrom = moment(dateFrom)._d;
                $scope.TimeLoadTo = moment(dateTo)._d;

                $scope.loadFromId = null;

                console.log(data.tooltip);

                $scope.search();
            }
        },
        getStarEndDatetime = function() {
            var dateTimeFormat = 'YYYY-MM-DD HH:mm';
            var dateTimeFrom = moment().subtract(24, 'hour').minute(0).second(0);
            
            return {
                datetimeNow: moment().format(dateTimeFormat),
                datetimeFrom: dateTimeFrom.format(dateTimeFormat),
                datetimeFromObj: dateTimeFrom
            };
        },
        getErrorDataFromApi = function($scope, collection, callbackFunction) {
            var dates = getStarEndDatetime();

            $scope.errorsStatisticLabel = (" Between " + dates.datetimeFrom + " and " + dates.datetimeNow);

            $scope.chartLabelDateFrom = dates.datetimeFrom;
            $scope.chartLabelDateTo = dates.datetimeNow;

            //Get error's chart data 
            formsUtilService.ajax($http, $scope,
                {
                    url: '/api/getErrorsCount/' + collection,
                    queryParams: {
                        'loadFrom': dates.datetimeFrom,
                        'loadTo': dates.datetimeNow,
                    }
                },
                function(errorObject) {
                    callbackFunction(errorObject);
                });
        },
        buildItemsDateIntervalArray = function(errorObject) {

            var intervalsArr = [];

            var dateFrom = getStarEndDatetime().datetimeFromObj;
            var dateTo = (getStarEndDatetime().datetimeFromObj).add(chartMinutesInterval, 'minutes');

            while (dateTo <= moment()) {

                //Builds interval's array item
                intervalsArr.push({
                    name: (dateFrom.format(timeLabelFormats)),
                    y: 0,
                    dateTo: dateTo.format(datetimeLabelFormat),
                    dateFrom: dateFrom.format(datetimeLabelFormat),
                    groupId: dateTo.unix(),
                });

                //Change date interval
                dateTo = dateTo.add(chartMinutesInterval, 'minutes');
                dateFrom = dateFrom.add(chartMinutesInterval, 'minutes');
            }

            addErrorsToIntervalArray(intervalsArr, errorObject);

            return intervalsArr;
        },
        addErrorsToIntervalArray = function(intervalsArr, errorsArray) {

            if (intervalsArr.length != 0) {

                angular.forEach(errorsArray, function(errorObjectValue) {

                    var errorObjectDate = moment(errorObjectValue.EventDate);

                    //Checking while date will be Date Now
                    for (var intervalsArrValue = 0; intervalsArrValue < intervalsArr.length; intervalsArrValue++) {

                        var intervalDateFrom = moment(intervalsArr[intervalsArrValue].dateFrom);
                        var intervalDateTo = moment(intervalsArr[intervalsArrValue].dateTo);

                        //Check if currect element is in current date range 
                        if (errorObjectDate >= intervalDateFrom && errorObjectDate <= intervalDateTo) {

                            intervalsArr[intervalsArrValue].y++;

                            break;
                        }
                    }

                });
            }
        },
        buildChart = function($scope, collectionName) {
            getErrorDataFromApi($scope, collectionName,
                function(errorObject) {
                    if (errorObject.length != 0) {
                        var intervalsArr = buildItemsDateIntervalArray(errorObject);
                        if (intervalsArr.length != 0) {
                            $scope.chartDataExists = true;
                            $scope.chartConfig.series[0].data = intervalsArr;
                        }
                    }
                });
        };

    return {
        buildChart: function ($scope, collectionName) {
            $scope.chartDataExists = null;
            $scope.errorsStatisticLabel = '';

            initialiseChart($scope);
            buildChart($scope, collectionName);
        },
    }
})