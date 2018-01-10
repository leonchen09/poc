/**
 * 依赖加载初始化
 */
var app = angular.module('test', [
    'ui.router',
    'ngDialog'
]);

/**
 * 全局参数配置
 */

/*服务端接口地址*/
app.constant('CC_API', {
    base: '',      //工程路径
    baseTpl: '/modules/'                       //模板路径
});

//http服务配置
app.config(['$httpProvider', function($httpProvider) {
    // 修改传值格式为form
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';

    /**
     * 参数序列化
     */
    var param = function(obj) {
        var query = '', name, value, fullSubName, subName, subValue, innerObj, i;

        for(name in obj) {
            value = obj[name];

            if(value instanceof Array) {
                for(i=0; i<value.length; ++i) {
                    subValue = value[i];
                    fullSubName = name + '[' + i + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if(value instanceof Object) {
                for(subName in value) {
                    subValue = value[subName];
                    fullSubName = name + '[' + subName + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if(value !== undefined && value !== null)
                query += encodeURIComponent(name) + '=' + encodeURIComponent(value) + '&';
        }

        return query.length ? query.substr(0, query.length - 1) : query;
    };

    // 设置自动序列化
    $httpProvider.defaults.transformRequest = [function(data) {
        return angular.isObject(data) && String(data) !== '[object File]' ? param(data) : data;
    }];
    //$httpProvider.interceptors.push('timestampMarker');
}]);

/**
 * 路由表配置
 */
app.config(['$stateProvider', '$urlRouterProvider', function($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise('/register');
    $urlRouterProvider.when("", "/register");
}]);