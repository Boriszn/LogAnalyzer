
app.directive('numberhighlighter', [
    '$timeout', function($timeout) {
        return {
            restrict: 'A',
            scope: {
                model: '=numberhighlighter'
            },
            link: function(scope, element) {
                scope.$watch('model', function(nv, ov) {
                    console.log("number highlighter");

                    if (nv !== ov) {
                        // apply class
                        element.addClass('number-highlight');

                        // auto remove after some delay
                        $timeout(function() {
                            element.removeClass('number-highlight');
                        }, 1000);
                    }
                });
            }
        };
    }
]);