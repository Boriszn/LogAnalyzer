'use strict';

var app = angular.module('LogMonitorApp', ['ngRoute', 'ngAnimate', 'ui.bootstrap', 'cgBusy', '720kb.tooltips', 'timepickerPop',
     'dnd', 'ngStorage', 'highcharts-ng', 'ngSanitize', 'angularStats']);

app.config(function ($routeProvider, $httpProvider, $locationProvider) {

    // If true, will rely on history.pushState to change urls where supported. 
    // Will fall back to hash-prefixed paths in browsers that do not support pushState
    // $locationProvider.html5Mode(true);

    $routeProvider

        .when('/', {
            templateUrl: '/app/views/default.html',
            controller: 'DefaultController'
        })

        .when('/:collectionName', {
            templateUrl: '/app/views/collection.html',
            controller: 'CollectionController'
        })

       .otherwise('/');

    /* Exception && Errors handling setup */
    $httpProvider.responseInterceptors.push(['$rootScope', '$q', function (scope, $q) {

        function success(response) {
            return response;
        }

        function error(response) {
            scope.$broadcast('event:AjaxError');
            return $q.reject(response);
        }

        return function (promise) {
            return promise.then(success, error);
        }

    }]);

});

/* Exception && Errors handler */
app.run(function ($rootScope, $modal) {
    var instance;

    $rootScope.$on('event:AjaxError', function () {
        console.log('ajax error');

        if (!instance) {
            instance = $modal.open({
                templateUrl: 'app/views/ajaxErrorModal.html',
                size: 'sm'
            });
        }

        $rootScope.cancel = function () {
            instance.dismiss('cancel');
            instance = null;
        };
    });
});
