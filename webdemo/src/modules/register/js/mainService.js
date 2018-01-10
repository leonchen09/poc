app.factory('main',[ 'AppHttp', 'CC_API','MAIN_API', function( AppHttp, CC_API,MAIN_API) {
    var main = {};

    //效验用户名
    main.verify = function (data) {
        return AppHttp({
            method: 'post',
            url: CC_API.base + MAIN_API.verify,
            data: data
        });
    };

    //注册
    main.register = function (data) {
        return AppHttp({
            method: 'post',
            url: CC_API.base + MAIN_API.register,
            data: data
        });
    };

    return main;
}] );
