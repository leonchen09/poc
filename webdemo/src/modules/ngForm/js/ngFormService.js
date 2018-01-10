app.factory('ngForm',[ 'AppHttp', 'CC_API','ngForm_API', function( AppHttp, CC_API,ngForm_API) {
    var ngForm = {};

    //效验用户名
    ngForm.test = function (data) {
        return AppHttp({
            method: 'post',
            url: CC_API.base + ngForm_API.test,
            data: data
        });
    };


    return ngForm;
}] );
