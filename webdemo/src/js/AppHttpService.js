"use strict";
/**
 * http服务封装
 */
app.factory('AppHttp',['$http', '$timeout',"$rootScope","CC_API",
    function($http, $timeout,$rootScope,CC_API) {
        return function(opt) {
            return $http(opt)
                .success(function (data, status, headers, config) {
                    if(data.resultCode=='0010'){
                        alert(data.result.message);
                    }
                })
                .error(function (data, status, headers, config) {
                    //错误处理
                    //alert('服务器端错误');
                });
        };
}]);