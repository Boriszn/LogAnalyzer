
app.factory('newLogsNumberService', function (formsUtilService, $http, $interval, $rootScope, dateTimeService, $route, $localStorage) {
    {
        //Dec. of signalR hub and connection
        var newLogsNumberHub = $.connection.newLogsNumberHub,

        stopPreviousHubConnection = function () {
            if ($.connection && $.connection.hub.state !== 4 /* state > disconected */) {
                $.connection.hub.stop();
                console.log("NewLogsNumberService > Hub Connection Stoped");
            }
        },

        registerSignalClientEventsListeners = function ($scope) {
            //[SignalR  Client events listener] Gets data from server side Hub and NewLogNumberBroadcaster
            newLogsNumberHub.client.updateNumberOfNewItems = function (data) {
                console.log(data);
                console.log(" Сollection: " + $scope.LogCollectionName);
                //Update model in scope
                $scope.NewLogsNumber = data;
                //Refresh model in scope
                $scope.$digest();
            };
        },

        //Implementation of signalr 
        startGettingNumbers = function ($scope, logItem) {

            stopPreviousHubConnection($scope);
            registerSignalClientEventsListeners($scope);
            
            //Start connection with Signal Hub server and register server calls
            if (logItem) {
                newLogsNumberHub.connection.logging = false;
                $.connection.hub.start().done(function() {
                    console.log("NewLogsNumberService > Hub Connection Started for: " + $scope.LogCollectionName);
                    //Executes server (NewLogsNumberHub.cs) function 
                    newLogsNumberHub.server.updateNumberOfNewItems($scope.LogCollectionName, logItem.Id, ($scope.QueryText || null),
                        dateTimeService.combineDateAndTime($scope.LoadFrom, $scope.TimeLoadFrom),
                        dateTimeService.combineDateAndTime($scope.LoadTo, $scope.TimeLoadTo));
                });
            }
        },

        getNewItemsApiCall = function ($scope, logItem) {
             if (logItem) {

                 //$scope.reloadPage = $route.reload();

                formsUtilService.ajax($http, $scope,
                    {
                        url: 'api/getNewItems/' + $scope.LogCollectionName,
                        queryParams: {
                            loadFromId: logItem.Id
                        }
                    },
                    function (data) {
                        //Insert new log data in the begining of new logs
                        var logs = $scope.logs;
                        $scope.logs = (data.concat(logs));
                        $scope.NewLogsNumber = 0;
                        
                        startGettingNumbers($scope, $scope.logs[0]);
                    });
            }
        }
    }
    return {
        
        getNumberOfNewItems: function ($scope) {
            var self = this;
            $scope.NewLogsNumber = 0;

            //Implementation of signalr
            startGettingNumbers($scope, self.getLoadFromIdLog($scope));

            //Get new logs by click
            $scope.NewLogsNumberClick = function () {
                if ($scope.NewLogsNumber != 0) {
                    getNewItemsApiCall($scope, self.getLoadFromIdLog($scope));
                }
            };
        },

        getLoadFromIdLog: function ($scope) {
            return ($scope.logs[0]);
        },

        dragAndDropSets: function ($scope) {
            $scope.dragmodel = '';
            $scope.dragableRect = {};
            $scope.dragend = function () {
                var numberObject = $(".mfb-component__wrap");
                //Save position of Newnumbers control from local storage
                $localStorage.dndPosition = numberObject.position();
                console.log("Position: " + numberObject.position().left
                    + " : " + numberObject.position().top);

                //Remove drag start css style
                numberObject.css("border", "");
            }

            $scope.dragstart = function(data) {
                //Add drag start css style
                $(".mfb-component__wrap").css("border", "2px dashed #f00");
            }

            //Get saved position of Newnumbers control from local storage
            if ($localStorage.dndPosition) {
                $scope.dragableRect = {
                    left: ($localStorage.dndPosition.left + 'px'),
                    top: ($localStorage.dndPosition.top + 'px'),
                };
            }
        },

        stopPreviousHubConnection: function () {
            return stopPreviousHubConnection();
        }
    }
})