var OrderCtrl = function ($scope, $http) {

    $scope.itemSets = items; //global variable...*shrug*
    $scope.message = ''; //start out empty
    
    var results = $http.get('api/orders');
    results.success(function (data) {
        $scope.orders = data;
    });

    $scope.getImgPath = function (imgId) {
        return "Content/Images/" + imgId + ".jpg";
    };

    $scope.openModal = function (item) {
        $scope.newOrderItemId = item.value.id;
        $scope.newOrderItemName = item.value.name;
        $('#modalOrders').modal('show');
        return;
    };

    $scope.addOrder = function () {
        var newOrder = {
            'customerFirstName': $scope.newOrderCustomerFirstName,
            'customerLastName': $scope.newOrderCustomerLastName,
            'itemId': $scope.newOrderItemId,
            'itemName': $scope.newOrderItemName,
            'quantity': $scope.newOrderQuantity
        };

        $http.post('/api/orders', newOrder)
            .success(function (data) {
                console.log(data);
                $scope.message = 'Order for ' + data.itemName + ' successful.';
                $('#message').show();
            })
            .error(function (data) {
                console.log(data);
                $scope.message = data.responseStatus.message;
                $('#message').show();
            });

        $('#modalOrders').modal('hide');
    };
}