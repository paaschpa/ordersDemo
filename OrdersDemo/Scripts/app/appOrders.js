var OrderCtrl = function ($scope, $http) {

    $scope.itemSets = items; //global variable...*shrug*

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

        $http.post('/api/orders', newOrder);
        //$scope.orders.push(newOrder);
        $('#modalOrders').modal('hide');
    };
}