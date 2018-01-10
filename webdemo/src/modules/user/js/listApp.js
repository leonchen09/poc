
/*服务端接口地址*/
app.constant('LIST_API', {
    queryList:'/service/queryList',  //查询列表
    queryItem:'/service/queryItem',  //查询单条数据
    getSex:'/service/getSex',  //获取性别下拉数据
    updateSub:'/service/updateSub',  //修改提交
    deleteSub:'/service/deleteSub',  //删除提交
    getColorList:'/service/getColorList',  //获取颜色字典
    validate:'/service/validate'  //验证表单
});


//路由
app.config(['$stateProvider','CC_API', function($stateProvider,CC_API) {
    $stateProvider
        //列表
        .state('list',{
            url:"/user/:page",
            views: {
                '': {
                    templateUrl: CC_API.baseTpl+"user/view/list.html",
                    controller:"listCtrl"
                }
            }
        })
}]);