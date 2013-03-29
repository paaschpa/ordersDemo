var FulfillmentCtrl = function ($scope, $http) {

    $scope.refreshGrid = function () {
        var results1 = $http.get('api/fulfillment');
        results1.success(function (data) {
            for (var item in data.fulfillments) {
                data.fulfillments[item].buttonText = 'Start'
            }
            $scope.fulfillments = data.fulfillments;
        });
    };

    $scope.refreshGrid();

    $scope.getButtonState = function (order) {
        if (order.status = 'New') {
            order.buttonText = 'Complete';
        }
    }
}