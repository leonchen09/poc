app.directive('layDate', ['$rootScope',"$parse",function ($rootScope,$parse) {
    return {
        link: function ($scope, iElement, iAttrs, controller) {
            iElement.on("focus",function(){
                laydate({
                    istime: true,
                    format: 'YYYY-MM-DD hh:mm:ss',
                    choose:function(data){
                        $parse(iAttrs['ngModel']).assign($scope, data);
                        $scope.$apply();
                    }
                });
            });
            iElement.on("blur",function(){
                $parse(iAttrs['ngModel']).assign($scope,"");
                $scope.$apply();
            });
        }
    };
}]);