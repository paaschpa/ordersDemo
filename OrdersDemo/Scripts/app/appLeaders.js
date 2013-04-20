var LeaderCtrl = function ($scope, $http) {

    $scope.refreshGrid = function () {
        var results = $http.get('/api/leaders');
        results.success(function (data) {
            $scope.leaders = data;
        });
    };

    $scope.refreshGrid();
}