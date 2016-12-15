'use strict';
app.controller('MainController', function ($scope, $http) {

    $scope.app = {
        name: 'Log Aniliser tool',
        version: '1.1.1',
        settings: {
            themeID: 1,
            navbarHeaderColor: 'bg-black',
            navbarCollapseColor: 'bg-white-only',
            asideColor: 'bg-black',
            headerFixed: true,
            asideFixed:  true,
            asideFolded: false
        },
        evironmentVersion: null
    };

    //Performance test utility
    /*
    var ngStats = showAngularStats({
        position: 'bottom-left'
    });
    */

    $http.get('/api/environmentInfo').
        success(function(data) {
            $scope.app.evironmentVersion = JSON.parse(data);
        });

})