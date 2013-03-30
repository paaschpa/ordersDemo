var FulfillmentCtrl = function ($scope, $http) {

    $scope.refreshGrid = function () {
        var results = $http.get('api/fulfillment');
        results.success(function (data) {
            $scope.fulfillments = data.fulfillments;
        });
    };

    $scope.refreshGrid();

    $scope.getButtonText = function (status) {
        if (status == 'New') {
            return 'Start';
        }
        if (status == 'Start') {
            return 'Complete';
        }
    };

    $scope.changeState = function (order) {
        if (order.status = 'New') {
            order.status = 'Start';
            $http.put('api/fulfillment', order);
        }
    };
}