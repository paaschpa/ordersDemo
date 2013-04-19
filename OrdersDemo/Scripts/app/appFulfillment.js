var FulfillmentCtrl = function ($scope, $http) {

    $scope.refreshGrid = function () {
        var results = $http.get('/api/fulfillment');
        results.success(function (data) {
            for (var i = 0; i < data.length; i++) {
                data[i].disable = false;
            }
            $scope.fulfillments = data;
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
        if (status == 'Complete' || status == 'Completed') {
            return 'Completed';
        }
    };

    $scope.changeState = function (order) {
        var el = $('#' + order.id);
        if (order.status == 'New') {
            order.status = 'Start';
            $http.put('/api/fulfillment', order);
            el.attr('disabled', true);
            var t = window.setTimeout(function () {
                el.attr('disabled', false);
            }, 1000 * order.quantity);
            return;
        }
        if (order.status == 'Start') {
            order.status = 'Completed';
            $http.put('/api/fulfillment', order);
            return;
        }
    };

    $scope.addToGrid = function (fulfillment) {
        $scope.$apply(function () {
            var newFulfillment = { 
                id:fulfillment.Id, 
                orderId:fulfillment.OrderId, 
                itemName:fulfillment.ItemName, 
                quantity:fulfillment.Quantity, 
                status:fulfillment.Status 
            };
            $scope.fulfillments.push(newFulfillment);
        });
    };

    $scope.updateGrid = function (order) {
        for (var i = 0; i < $scope.fulfillments.length; i++) {
            if (order.OrderId == $scope.fulfillments[i].orderId) {
                $scope.$apply(function () {
                    $scope.fulfillments[i].status = order.Status;
                    $scope.fulfillments[i].fulfiller = order.Fulfiller;
                });
                break;
            }
        }
        return;
    };

}