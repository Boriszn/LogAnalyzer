
app.filter('maxTextLength', function () {
    return function (input, patternTextLength) {
        if (!input) {
            return "";
        }

        var defaultTextLength = 200;
        
        patternTextLength = (patternTextLength != null ? patternTextLength : defaultTextLength);

        if (input.length > patternTextLength)
            input = input.substring(0, patternTextLength) + "...";

        return input;
    };
})