app.factory('list',[ 'AppHttp','$q', 'CC_API','LIST_API', function( AppHttp,$q, CC_API,LIST_API) {
    var list = {};

    //获取列表
    list.queryList = function (data) {
        return AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.queryList,
            data: data
        });
    };

    //查询单条数据
    list.queryItem =function (data){
        var d=$q.defer();
        AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.queryItem,
            data: data
        }).success(function(data) {
            d.resolve(data);
        });
        return d.promise;
    };

    //获取性别下拉数据
    list.getSex=function(data){
        return AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.getSex,
            data: data
        });
    };

    //修改提交
    list.updateSub=function(data){
        return AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.updateSub,
            data: data
        });
    };

    //删除
    list.deleteSub=function(data){
        return AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.deleteSub,
            data: data
        });
    };

    //获取颜色字典
    list.getColorList=function(data){
        return AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.getColorList,
            data: data
        });
    };

    //后端验证
    list.validate=function(data){
        return AppHttp({
            method: 'post',
            url: CC_API.base + LIST_API.validate,
            data: data
        });
    };


    return list;
}] );
