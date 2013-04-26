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

    $scope.addToGrid = function (order) {
        $scope.$apply(function () {
            var orderInQueue = {
                customerName: order.CustomerName,
                orderId: order.OrderId,
                itemName: order.ItemName,
                quantity: order.Quantity,
                status: order.Status
            };
            $scope.orders.push(orderInQueue);
        });
    };

    $scope.updateGrid = function (order) {
        for (var i = 0; i < $scope.orders.length; i++) {
            if (order.OrderId == $scope.orders[i].orderId) {
                $scope.$apply(function () {
                    $scope.orders[i].status = order.Status;
                    $scope.orders[i].fulfiller = order.Fulfiller;
                    if ($scope.orders[i].status == 'Completed') {
                        $scope.orders.splice(i, 1);
                    }
                });
                break;
            }
        }
        return;
    };

}