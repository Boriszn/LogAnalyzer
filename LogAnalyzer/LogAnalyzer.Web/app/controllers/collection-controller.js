'use strict';

app.controller('CollectionController',
    function ($scope, $routeParams, $modal, $http, formsUtilService, logItemsService, newLogsNumberService,
        dateTimeService, errorsChartService) {

        var collection = $routeParams.collectionName;
        $scope.LogCollectionName = collection;

        $scope.chartIsOpen = true;
        $scope.logs = [];
        $scope.QueryText = '';
        $scope.Query = null;

        $scope.LoadFrom = null;
        $scope.LoadTo = null;

        $scope.exceptionStatus = {};
        $scope.exceptionStatus.open = true;

        //Loading collection
        formsUtilService.ajax($http, $scope,
            {
                url: 'api/' + collection,
                queryParams: null
            },
            function(data) {
                $scope.logs = data;
                newLogsNumberService.getNumberOfNewItems($scope);
            });
        
        //Quick filters
        $scope.quickFilterClick = function(levelText) {
            $scope.Query = levelText != null ? 'Level == \"' + levelText + '\"' : '';
            $scope.LoadFrom = null;
            $scope.LoadTo = null;
            $scope.search();
        }

        //Collection query search btn click
        $scope.search = function() {
            var params = [];

            if ($scope.Query.length > 0) {
                params['query'] = $scope.Query;
                $scope.QueryText = $scope.Query;
            }

            if ($scope.LoadFrom) {
                params['loadFrom'] = dateTimeService.combineDateAndTime($scope.LoadFrom, $scope.TimeLoadFrom);
            }

            if ($scope.LoadTo) {
                params['loadTo'] = dateTimeService.combineDateAndTime($scope.LoadTo, $scope.TimeLoadTo);
            }

            formsUtilService.ajax($http, $scope,
                {
                    url: 'api/' + collection,
                    queryParams: params
                },
                function(data) {
                    $scope.logs = data;
                    newLogsNumberService.getNumberOfNewItems($scope);
                });
        }

        //Load more search btn click
        $scope.loadMore = function() {
            logItemsService.loadMoreLogs($scope);
        }

        //Show or hide log item btn click 
        $scope.showHideDetail = function (item, $event) {
            logItemsService.getDetilLogItem($scope, item);
        }

        $scope.showHideChart = function () {
            $scope.chartIsOpen = !$scope.chartIsOpen ? true : false;
        }

        //Initilise and Adjustments of drag and drop for New items Number 
        newLogsNumberService.dragAndDropSets($scope);

        //Errror chart
        errorsChartService.buildChart($scope, collection);

        (function dateTimePickersSetup() {
            
            //Date picker settings
            $scope.today = new Date();
            $scope.openFrom = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.fromOpened = true;
            };
            $scope.openTo = function ($event) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope.toOpened = true;
            };

            //Time picker settings
            $scope.TimeLoadFrom = new Date();
            $scope.TimeLoadTo = new Date();
            $scope.showMeridian = false;
            $scope.disabled = false;

        })();

})