var OrdersQueueCtrl = function ($scope, $http) {

    var results = $http.get('/api/orderInQueue');
    results.success(function (data) {
        $scope.orders = data;
    });
}