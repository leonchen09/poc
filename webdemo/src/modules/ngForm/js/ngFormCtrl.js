app.controller("ngFormCtrl",["$scope",'main', function ($scope,main) {
    $scope.submitForm = function(isValid) {
        console.log(isValid,'-----');
        if (!isValid) {
            alert('验证失败');
        }
    };
}]);