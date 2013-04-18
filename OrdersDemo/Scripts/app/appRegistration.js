var RegistrationCtrl = function ($scope, $http) {

    $('#RegistrationSuccess').hide();
    $('#error').hide();

    $scope.register = function () {
        var newRegistration = {
            'username': $scope.userName,
            'password': $scope.password
        };

        var registration = $http.post('/api/register', newRegistration);
        registration.success(function (data) {
            $('#RegistrationForm').hide();
            $('#RegistrationSuccess').show();
        });
        registration.error(function (data) {
            $('#error').show();
            $('#RegistrationSuccess').hide();
            $scope.errorMsg = data.responseStatus.message;
        });


    };
}