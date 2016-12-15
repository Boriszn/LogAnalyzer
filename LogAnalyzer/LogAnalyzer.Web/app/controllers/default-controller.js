'use strict';

app.controller('DefaultController', function($scope, newLogsNumberService) {
    newLogsNumberService.stopPreviousHubConnection();
});

