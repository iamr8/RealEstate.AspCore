(function () {
    "use strict";

    const app = angular.module("RSApp", [
//        'ngRoute',
    ]);

})();

(function (fltr) {
    "use strict";

    fltr.filter("spaceLess", function () {
        return function(input) {
            if (input) {
                return input.replace(/[()._-\s+!#$%^&*,;/|<>"'?=:\\]/g, '_');
            }
        };
    });

    fltr.filter("index", function () {
        return function (array, index) {
            if (!index)
                index = 'index';
            for (var i = 0; i < array.length; ++i) {
                array[i][index] = i;
            }
            return array;
        };
    });

    fltr.directive("confirmClick", function ($window) {
        var i = 0;
        return {
            restrict: "A",
            priority: 1,
            compile: function (tElem, tAttrs) {
                var fn = `$$confirmClick${i++}`,
                    _ngClick = tAttrs.ngClick;
                tAttrs.ngClick = fn + "($event)";

                return function (scope, elem, attrs) {
                    var confirmMsg = attrs.confirmClick || "Are you sure?";

                    scope[fn] = function (event) {
                        if ($window.confirm(confirmMsg)) {
                            scope.$eval(_ngClick, { $event: event });
                        }
                    };
                };
            }
        };
    });

    //fltr.directive('a', function () {
    //    return {
    //        restrict: 'E',
    //        link: function (scope, elem, attrs) {
    //            if (attrs.ngClick || attrs.href === '' || attrs.href === '#') {
    //                elem.on('click', function (e) {
    //                    e.preventDefault();
    //                    e.stopPropagation();
    //                });
    //            }
    //        }
    //    };
    //});

})(angular.module("RSApp"));
