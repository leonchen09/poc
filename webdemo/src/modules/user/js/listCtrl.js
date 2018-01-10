app.controller("listCtrl",["$scope",'$stateParams','$state','ngDialog','list','CC_API', function ($scope,$stateParams,$state,ngDialog,list,CC_API) {
    var pageSize=18;

    var getData=function(){
        list.queryList({
            page:$stateParams.page,
            pageSize:pageSize
        }).success(function(d){
            if(d.resultCode=='0000'){
                $scope.listData= d.result.rows;
                $scope.total= d.result.total;
                $scope.movePage = {
                    total: parseInt($scope.total),
                    page: $stateParams.page,
                    pageSize: pageSize
                };
            }
        });
    };
    getData();

    //切换页码方法
    $scope.search = function(page) {
        $state.go('list',{
           page:page
        });
    };

    //修改
    $scope.updateFnc=function(id){
        //console.log(id);
        $scope.thisId=id;
        list.queryItem({id:$scope.thisId}).then(function(d){
            if(d.resultCode=='0000'){
                $scope.upDateForm={
                    name: d.result.name,
                    sex: d.result.sex,
                    smoke: d.result.smoke,
                    color: d.result.color,
                    date: d.result.date,
                    age: d.result.age,
                    phone: d.result.phone
                };
                ngDialog.open({
                    template: CC_API.baseTpl+'user/view/update_dialog.html',
                    className: 'ngdialog-theme-default',
                    controller:"updateCtrl",
                    contentWidth:"450px",
                    scope:$scope
                });
            }
        });
    };

    //删除
    $scope.deleteFnc=function(id){
        //$scope.thisId=id;
        var r=confirm("确定要删除吗？");
        if (r==true)
        {
            list.deleteSub({id:id}).success(function(d){
               if(d.resultCode=='0000'){
                   alert(d.result.message);
                   getData();
               }
            });
        }
        else
        {
            console.log('取消');
        }
    }
}]);