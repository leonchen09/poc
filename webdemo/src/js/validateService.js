/*
常用正则验证服务
 */
app.factory('validation',[ 'AppHttp','$q', 'CC_API', function( AppHttp,$q, CC_API) {
    var validation={};
    //邮箱
    validation.isEmail=function(str){
        var reg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+/;
        return reg.test(str);
    };
    //移动电话及固话
    validation.isPhone=function(str){
        var regu =/^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$/,
            regu1 =/^(0\d{2,3}-\d{7,8})$/;
        return  regu.test(str)||regu1.test(str);
    };
    //手机
    validation.isMobile=function(str){
        var regu =/^(0|86|17951)?(13[0-9]|15[012356789]|17[678]|18[0-9]|14[57])[0-9]{8}$/;
        return  regu.test(str);
    };
    //正整数
    validation.isDigit=function(str){
        return $.trim(str) != '' && (isNaN(str) || parseInt(str) != str);
    };
    //Url
    validation.isUrl=function(str){
        var strRegex = '^((https|http|ftp|rtsp|mms)?://)'
            + '?(([0-9a-z_!~*\'().&=+$%-]+: )?[0-9a-z_!~*\'().&=+$%-]+@)?' //ftp的user@
            + '(([0-9]{1,3}.){3}[0-9]{1,3}' // IP形式的URL- 199.194.52.184
            + '|' // 允许IP和DOMAIN（域名）
            + '([0-9a-z_!~*\'()-]+.)*' // 域名- www.
            + '([0-9a-z][0-9a-z-]{0,61})?[0-9a-z].' // 二级域名
            + '[a-z]{2,6})' // first level domain- .com or .museum
            + '(:[0-9]{1,4})?' // 端口- :80
            + '((/?)|' // a slash isn't required if there is no file name
            + '(/[0-9a-z_!~*\'().;?:@&=+$,%#-]+)+/?)$';
        var re=new RegExp(strRegex);
        return re.test(str);
    };
    //身份证
    validation.isIdentityCard=function(str){
        var s=/(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
        return s.test(str);
    };
    //密码    以字母开头，长度在6~18之间，只能包含字符、数字和下划线。
    validation.isPassword=function(str){
        var p =/^[a-zA-Z]\w{5,17}$/;
        return p.test(str);
    };

    return validation;
}]);