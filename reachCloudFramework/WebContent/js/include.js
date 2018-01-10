/**
 * Created by Meteor on 2016/4/28.
 */
/**
 * 引入其他js文件
 * @param path
 */
function includeJs(path) {
    var a = document.createElement("script");
    a.type = "text/javascript";
    a.src = path;
    var head = document.getElementsByTagName("head")[0];
    head.appendChild(a);
}
/**
 * 引入css文件
 * @param path
 */
function includeCss(path) {
    var a = document.createElement("link");
    a.rel = "stylesheet";
    a.href = path;
    var head = document.getElementsByTagName("head")[0];
    head.appendChild(a);
}