var FulfillmentCtrl = function ($scope, $http) {

    var results = $http.get('api/fulfillment');
    results.success(function (data) {
        $scope.fulfillments = data.fulfillments;
    });

    $scope.refreshGrid = function() {
        var results1 = $http.get('api/fulfillment');
        results1.success(function(data) {
            $scope.fulfillments = data.fulfillments;
        });
    };
}