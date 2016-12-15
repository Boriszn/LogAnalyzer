
app.factory('logItemsService', function (formsUtilService, $http, dateTimeService, jsonToHtmlService) {
    return {
        loadMoreLogs: function($scope) {
            $scope.IsNoDataToLoadMore = false;

            var lastLogItem = $scope.logs[$scope.logs.length - 1];
            console.log("[Load More] lastlLog item: " + lastLogItem.Id);

            if ($scope.LastLogItem && $scope.LastLogItem.Id == lastLogItem.Id) {
                console.log("[End to Load More]");
                $scope.IsNoDataToLoadMore = true;
                return;
            }

            $scope.LastLogItem = lastLogItem;

            if (lastLogItem && $scope.LogCollectionName) {
                formsUtilService.ajax($http, $scope,
                    {
                        url: 'api/' + $scope.LogCollectionName,
                        queryParams: {
                            loadToId: lastLogItem.Id,
                            query: $scope.QueryText,
                            loadFrom: dateTimeService.combineDateAndTime($scope.LoadFrom, $scope.TimeLoadFrom),
                            loadTo: dateTimeService.combineDateAndTime($scope.LoadTo, $scope.TimeLoadTo)
                        }
                    },
                    function(data) {
                        $scope.logs = ($scope.logs.concat(data));
                    });
            }
        },

        getDetilLogItem: function($scope, item) {
            var self = this;
            if (!item.DetailItemModel) {
                formsUtilService.ajax($http, $scope,
                    {
                        url: "api/" + $scope.LogCollectionName + "/" + item.Id,
                        queryParams: null
                    },
                    function(data) {
                        var data = JSON.parse(JSON.parse(data));
                        console.log(data);
                        
                        self.setItemVisible(item, data);
                        item.DetailItemModel = {};
                        
                        self.initGeneralObjects(item, data, $scope);
                        self.initCustomObjects(item, $scope, data);
                        //Sets exception prop order, mapping, etc
                        self.initExeptionObject(data.Exceptions, item, $scope);
                        //Generate Html from Custom object
                        item.DetailItemModel.CustomObjectsData = self.initRestObjects(data, $scope);

                    });

            } else {
                item.IsVisible = !item.IsVisible ? true : false;
            }
        },

        initGeneralObjects: function (item, data, $scope) {
            item.DetailItemModel.Id = item.Id;
            item.DetailItemModel.Message = data.Message;

            item.DetailItemModel.Info = jsonToHtmlService.buildHtmlByJson(data, $scope, true);
            console.log(item.DetailItemModel.Info);
        },

        initCustomObjects: function (item, $scope, data){
            if (data.Request) {
                item.DetailItemModel.Request = jsonToHtmlService.buildHtmlByJson(data.Request, $scope);
                console.log(item.DetailItemModel.Request);
            }

            if (data.AdditionalInfo) {
                item.DetailItemModel.AdditionalInfo = jsonToHtmlService.buildHtmlByJson(data.AdditionalInfo, $scope);
                console.log(item.DetailItemModel.AdditionalInfo);
            }

            if (data.VisitedPageName) {
                item.DetailItemModel.VisitedPageName = jsonToHtmlService.buildHtmlByJson(data.VisitedPageName, $scope);
            }
        },

        initExeptionObject: function (exceptions, item, $scope) {
            var self = this;
            if (!exceptions) {
                return;
            }
           
            var ordredExceptionsArray = [];
            
            //Reorder properties in Exception object
            for (a in exceptions) {

                console.log(exceptions[a]["StackTrace"]);

                //Order general exception item/objects
                ordredExceptionsArray.push({
                    "Type": exceptions[a]["Type"],
                    "Message": exceptions[a]["Message"],
                    "StackTrace": self.getFormatedStackTrace(exceptions[a]["StackTrace"]),
                    "DbEntityValidation": exceptions[a]["DbEntityValidation"]
                });

                //Find ingeneral exception items/objects
                var exceptionsKeys = Object.keys(exceptions[a]);
                for (var exceptionKeyItem in exceptionsKeys){

                    if(exceptionsKeys[exceptionKeyItem] != 'Message'
                        && exceptionsKeys[exceptionKeyItem] != 'StackTrace'
                        && exceptionsKeys[exceptionKeyItem] != 'Type') {

                        var exceptionKey = exceptionsKeys[exceptionKeyItem];
                        console.log(exceptionKey);

                        var exceptionItemObj = {};
                        exceptionItemObj[exceptionKey] = exceptions[a][exceptionKey];
                        
                        ordredExceptionsArray.push(exceptionItemObj);
                    }
                }
            }

            //Builds html by json frim Exception object
            item.DetailItemModel.Exceptions = jsonToHtmlService.buildHtmlByJson(ordredExceptionsArray, $scope);
            console.log(item.DetailItemModel.Exceptions);
        },

        getFormatedStackTrace: function (stackTraceArray) {
            var parsedStackTrace = "";
            if (stackTraceArray) {
                for (var stackTraceItem in stackTraceArray) {
                    var stack = stackTraceArray[stackTraceItem];
                    
                    if (stack == "\n") {
                        parsedStackTrace += (stack + "</br>");
                    }
                    
                    parsedStackTrace += stack;
                }
            }


            return parsedStackTrace;
        },

        //Builds html by json from folowing custom objects
        initRestObjects: function (data, $scope) {
            //Generate Html from Custom object

            var customObjectsData = "";

            angular.forEach(data, function (value, key) { 

                //Exception objects list
                if (key == '_id' ||
                    key == 'EventDate' ||
                    key == 'Exceptions' ||
                    key == 'EventDateUtc' || 
                    key ==  'VisitedPageName' ||
                    key == 'Request'||
                    key == 'VisitDate' ||
                    key == 'UserDomains' || 
                    key == 'AdditionalInfo') {
                    return;
                }

                if (typeof value === 'object') {

                    customObjectsData += '<div class="b-info panel panel-default" >'
                        + '<div class="panel-heading" >'
                        + ' <h4 class="panel-title">'
                        + ' <a  class="accordion-toggle">'
                        + ' <span class="ng-scope">'
                        + key + '</span><i class="pull-right fa fa-angle-down"></i>'
                        + '</a>'
                        + ' </h4>'
                        + ' </div>'
                        + '<div class="panel-collapse collapse in" style="height: auto;">'
                        + '<div class="panel-body collapse in">'
                        + '<div>'
                        + jsonToHtmlService.buildHtmlByJson(value, $scope)
                        + '</div>'
                        + ' </div>'
                        + '</div>'
                        + '</div>';

                } else {

                    //debugger;
                }

            });

            

            return customObjectsData;
        },

        setItemVisible: function(item) {
            item.IsVisible = !item.IsVisible ? true : false;
        }
    }
})