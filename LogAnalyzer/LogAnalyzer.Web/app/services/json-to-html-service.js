
app.factory('jsonToHtmlService', function ($sce) {
    return {

        textSplitSize: 130,

        dateTimeParse: function (key, value) {
            if (key == "$date") {
                value = moment('/Date(' + value + ')/').format('YYYY-MM-DD HH:mm');
                console.log(value);
            }
            return value;
        },

        //splitValueString: function (value) {
        //    var cutSize = this.textSplitSize;
        //    if (value.length >= cutSize) {
        //        var tempValue = "&nbsp";
        //        for (var i = 0; i < value.length; i++) {
        //            if (tempValue.length % cutSize == 0) {
        //                tempValue += value[i] + " </br>";
        //            }
        //            tempValue += value[i];
        //        }
        //        value = tempValue;
        //    }
        //    return value;
        //},

        buildHtmlByJson: function (obj, $scope, isUseFirstLevel) {
            var self = this;
            console.log(obj);

            $scope.resultHtml = "";
            $scope.resultHtml += "<div class='table-responsive'><table class='table table-condensed table-striped'>";

            self.buildHtmlBodyByJson(obj, $scope, null, isUseFirstLevel);
            var resultHtml = $sce.trustAsHtml(($scope.resultHtml += "</table></div>"));

            console.log(resultHtml);

            return resultHtml;
        },

        //Generate Html table item Recusively bypassing JSON object's tree
        buildHtmlBodyByJson: function (obj, $scope, rootStringKey, isUseOnlyFirstLevel) {
            if (!obj) {
                return;
            }

            var self = this;
            if (rootStringKey) {
                $scope.resultHtml += "<tr><td colspan='2'><b>" + rootStringKey + "</b></td></tr>";
            }

            angular.forEach(obj, function (value, key) {
                if (value == undefined) {
                    return;
                }

                if (typeof value === 'object') {
                    if (typeof key === "string") {
                        console.log("string: " + key);
                    }

                    if (isUseOnlyFirstLevel == undefined || isUseOnlyFirstLevel == false) {
                        //Recusively exexutes when find inner object or flag isUseOnlyFirstLevel hasn't specified or false
                        self.buildHtmlBodyByJson(value, $scope, (typeof key === "string" ? key : null), isUseOnlyFirstLevel);
                    }
                } else {
                    value = self.dateTimeParse(key, value);

                    //value = self.splitValueString(value);

                    $scope.resultHtml += '<tr class=\"' + key + '\"><td>' + key + '</td>'
                                           + '<td>' + value + '</td></tr>';
                }
            });
        }
    }
})