'use strict';

app.controller('LeftMenuController', function ($scope, $http, formsUtilService) {

    $scope.collections = [];

    formsUtilService.ajax($http, $scope,
        {
            url: 'api/collections',
            queryParams: null
        },
        function(collections) {
            angular.forEach(collections, function(value) {
                value.ErrorsCount = 0;
                value.ToolTip = value.CollectionName;
                angular.forEach(value.LastInfo, function(lastInfoItem) {
                    if (lastInfoItem.Level == "Error" || lastInfoItem.Level == "Fatal") {
                        value.ErrorsCount += lastInfoItem.Count;
                    }
                    value.ToolTip = value.ToolTip + ' \r' + lastInfoItem.Level + ': ' + lastInfoItem.Count;
                });
            });

            $scope.collections = collections;
        });


    $scope.collectionItemClick = function ($event) {
        $('ul.nav .active').removeClass('active');
        $($event.target).parent().addClass('active');
    }

})