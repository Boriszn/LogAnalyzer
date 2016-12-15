'use strict';

app.factory('formsUtilService', function() {
    return {
        ajax: function($http, $scope, params, successMethod, isLoadingDiable, errorMethod) {

            //Ajax loading indicator cmp options
            $scope.delay = 0;
            $scope.minDuration = 0;
            $scope.message = 'Loading...';
            $scope.backdrop = true;
            $scope.promise = null;

            //Button with loading indicator onption (Ladda cmp button)
            $scope.loading = true;

            $scope.promise = $http({
                    method: 'GET',
                    url: params.url,
                    params: params.queryParams,
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                })
                .success(function(data) {
                    //ajax button stop loading indications
                    $scope.loading = false;
                    if (successMethod) {
                        successMethod(data);
                    }
                })
                .error(function(data) {
                    $scope.loading = false;
                    if (errorMethod) {
                        errorMethod(data);
                    }
                });

            //Hide loading panel
            if (isLoadingDiable) {
               $scope.promise = null;
            }

        },

        ajaxForm: function($http, $scope, url, initParamsFunc, successFunc, initUrlFunc) {
            var self = this;

            $scope.processForm = function() {

                var param = initParamsFunc();

                if (!url) {
                    url = initUrlFunc().url;
                }

                self.ajax($http, $scope,
                    {
                        url: url,
                        queryParams: param
                    },
                    function(data) {
                        console.log(" Data from: " + url);
                        if (successFunc)
                            successFunc(data);
                    });
            };
        },
    }
});

