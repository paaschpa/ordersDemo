var OrdersQueueCtrl = function ($scope, $http, dateFilter) {

    $scope.refreshGrid = function () {
        var results = $http.get('/api/waitingfulfillment');
        results.success(function (data) {
            $scope.orders = data;
        });
    };

    $scope.refreshGrid();

    $scope.dateFormat = function (d) {
        //should probably handle date on the backend...but meh, this hack works
        var dt = new Date(parseInt(d.substr(6))); 
        return dateFilter(dt, 'medium');
    };

}