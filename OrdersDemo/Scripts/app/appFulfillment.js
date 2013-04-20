var FulfillmentCtrl = function ($scope, $http) {

    $scope.fulfiller = '';
    $scope.setFulfiller = function (userName) {
        $scope.fulfiller = userName;
    };

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

    $scope.applyFormat = function (fulfillment) {
        if (fulfillment.fulfiller == $scope.fulfiller) {
            return 'success';
        }
    };

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

    $scope.changeState = function (fulfillment) {
        var el = $('#' + fulfillment.id);
        if (fulfillment.status == 'New') {
            $http.put('/api/fulfillment', { status: 'Start', id: fulfillment.id, fulfiller: $scope.fulfiller })
                .success(function (data) {
                    fulfillment.status = 'Start';
                    fulfillment.fulfiller = $scope.fulfiller;
                    fulfillment.waitTime = 'Wait ' + fulfillment.quantity + ' to complete!';

                    el.attr('disabled', true);
                    var t = window.setTimeout(function () {
                        $scope.$apply(function () {
                            el.attr('disabled', false); //don't need jquery here just needed to use $scope.$apply
                            fulfillment.waitTime = '';
                        });
                    }, 1000 * fulfillment.quantity);
                })
                .error(function (data) {
                    console.log(data);
                    $scope.message = data.responseStatus.message;
                    $('#modalMessage').modal('show');
                });

            return;
        }
        if (fulfillment.status == 'Start') {
            $http.put('/api/fulfillment', { status: 'Completed', id: fulfillment.id, fulfiller: fulfillment.fulfiller })
                .success(function (data) {
                    fulfillment.status = 'Completed';
                })
                .error(function (data) {
                    $scope.message = data.responseStatus.message;
                    $('#modalMessage').modal('show');
                });

            return;
        }
    };

    $scope.addToGrid = function (fulfillment) {
        $scope.$apply(function () {
            var newFulfillment = {
                id: fulfillment.Id,
                orderId: fulfillment.OrderId,
                itemName: fulfillment.ItemName,
                quantity: fulfillment.Quantity,
                status: fulfillment.Status
            };
            $scope.fulfillments.push(newFulfillment);
        });
    };

    $scope.updateGrid = function (fulfillment) {
        for (var i = 0; i < $scope.fulfillments.length; i++) {
            if (fulfillment.Id == $scope.fulfillments[i].id) {
                $scope.$apply(function () {
                    $scope.fulfillments[i].status = fulfillment.Status;
                    $scope.fulfillments[i].fulfiller = fulfillment.Fulfiller;
                    if ($scope.fulfillments[i].status == 'Completed') {
                        $scope.fulfillments.splice(i, 1);
                    }
                });
                break;
            }
        }
        return;
    };

}