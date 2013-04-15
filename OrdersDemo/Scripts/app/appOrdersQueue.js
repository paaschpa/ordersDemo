var OrdersQueueCtrl = function ($scope, $http) {

    $scope.refreshGrid = function () {
        var results = $http.get('/api/waitingfulfillment');
        results.success(function (data) {
            $scope.orders = data;
        });
    };

    $scope.refreshGrid();
}