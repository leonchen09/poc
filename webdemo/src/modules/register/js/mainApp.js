
/*服务端接口地址*/
app.constant('MAIN_API', {
    verify:'/service/verify',  //效验用户名
    register:'/service/register'  //注册
});

app.config(['$stateProvider','CC_API', function($stateProvider,CC_API) {
    $stateProvider
        //首页
        .state('main',{
            url:"/register",
            views: {
                '': {
                    templateUrl: CC_API.baseTpl+"register/view/main.html",
                    controller:"mainCtrl"
                }
            }
        });
}]);