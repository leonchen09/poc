/**
 * Created by Meteor on 2016/4/26.
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

/**
 * 表格td元素属性节点
 * @param keyValue
 * @param classValue
 * @constructor
 */
function TableTdAttributeNode(keyValue, classValue) {
    this.keyValue = keyValue;
    this.classValue = classValue;
}

function OpClass(opName, opFunction, parameterNames) {
    this.opName = opName;
    this.opFunction = opFunction;
    this.parameterNames = parameterNames;
}

/**
 * 初始化表格
 * @param tableId
 * @param data
 * @param ops
 */
function initTable(tableId, data, ops) {
    tableId = "#" + tableId;

    //td元素属性节点List
    var tableTdAttributeNodeList = [];

    //注：此处获取的是js对象，需要转换成 jquery 对象才能使用jquery方法
    var tdsJsObject = $(tableId + " tbody tr:first td");
    var tds = [];

    //转换成 jquery 对象
    for (var y = 0; y < tdsJsObject.length; y++) {
        tds.push($(tdsJsObject[y]));
    }

    for (var i = 0; i < tds.length; i++) {
        tableTdAttributeNodeList.push(new TableTdAttributeNode(tds[i].attr("name"), tds[i].attr("class")));
    }

    var html = "";

    //构造一行demo
    html += "<tr class='elementHidden'>";
    for (var z = 0; z < tableTdAttributeNodeList.length; z++) {
        html += "<td class=\"" + tableTdAttributeNodeList[z].classValue + "\" name=\"" + tableTdAttributeNodeList[z].keyValue + "\"></td>";
    }
    html += "</tr>";

    var childrenTreeType = 1000;
    $(tableId + " tbody").html(html + createTbodyStatement(data, tableTdAttributeNodeList, ops));
}

/**
 * 构造tbody html语句块
 * @param html
 * @param data
 * @param tableTdAttributeNodeList
 * @param ops
 * @returns {*}
 */
function createTbodyStatement(data, tableTdAttributeNodeList, ops) {
    //构造表格tbody
    var html = "";
    for (var k = 0; k < data.length; k++) {
        html += "<tr>";
        for (var x = 0; x < tableTdAttributeNodeList.length; x++) {

            var node = tableTdAttributeNodeList[x];
            var tdText = data[k][node.keyValue];
            if (tdText != null && tdText != "null") {


                html += "<td class=\"" + node.classValue + "\" name=\"" + node.keyValue + "\">"
                    + tdText + "</td>";
            } else {

                html += "<td class=\"" + node.classValue + "\" name=\"" + node.keyValue + "\"></td>";
            }

        }

        //构造操作列，比如删除，修改等
        if (ops != null && ops.length > 0) {

            html += "<td>";
            for (var opsCounter = 0; opsCounter < ops.length; opsCounter++) {
                var op = ops[opsCounter];

                var opStatement = op.opFunction + "(";
                var parameterNames = op.parameterNames;
                if (parameterNames != null && parameterNames.length > 0) {

                    var parameterNamesLength = parameterNames.length;

                    for (var a = 0; a < parameterNamesLength; a++) {
                        if (a == parameterNamesLength - 1) {
                            opStatement += data[k][parameterNames[a]];
                        } else {
                            opStatement += data[k][parameterNames[a]] + ",";
                        }
                    }
                }

                opStatement += ")";

                html += "<button onclick='" + opStatement + "'>" + op.opName + "</button>";
            }
            html += "</td>";
        }
        html += "</tr>";
    }
    return html;
}
































