
app.directive('uiLadda', [
        function() {
            /// <summary>Ajax-Button with loading indicator </summary>
            /// <returns type="Object"></returns>
            return {
                link: function postLink(scope, element, attrs) {
                    //Initialises Lada component
                    var ladda = Ladda.create(element[0]);
                    scope.$watch(attrs.uiLadda, function(newVal, oldVal) {
                        if (angular.isNumber(oldVal)) {
                            if (angular.isNumber(newVal)) {
                                ladda.setProgress(newVal);
                            } else {
                                newVal && ladda.setProgress(0) || ladda.stop();
                            }
                        } else {
                            newVal && ladda.start() || ladda.stop();
                        }
                    });
                }
            };
        }
])