
/*服务端接口地址*/
app.constant('ngForm_API', {

});

app.config(['$stateProvider','CC_API', function($stateProvider,CC_API) {
    $stateProvider
        //angularJs表单验证
        .state('ngForm',{
            url:"/ngForm",
            views: {
                '': {
                    templateUrl: CC_API.baseTpl+"ngForm/view/ngForm.html",
                    controller:"ngFormCtrl"
                }
            }
        });
}]);