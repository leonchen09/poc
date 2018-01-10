/**
 * Created by Meteor on 2016/5/5.
 */
/**
 * 点击导航条切换资源
 * @param obj 需要引入的 html 页面
 */
function toggleSource(obj) {
    var value = $(obj).attr("value");
    $.get(value, function (data) {
        var mainRight = $("#main-right");
        mainRight.html(data);
    });

    loopNav({
        "callback":function(eleObj){
            $(eleObj).removeAttr("class");
        }
    });

    $(obj).attr("class","active");

}

/**
 * 循环导航条并执行回调方法
 * @param callbackFunction
 */
function loopNav(callbackFunction){
    var temp = $("li[role=index-nav-bar]");
    temp.each(function () {
        callbackFunction["callback"](this);

    });
}

/**
 * 页面加载完成给导航条绑定方法
 */
$(function () {
    loopNav({
        "callback":function(obj){
            $(obj).click(function () {
                toggleSource(obj);
            });
        }
    });
});
