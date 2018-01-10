/**
 * Created by Meteor on 2016/4/27.
 */
/**
 * 判断tableTdDisplayKeys中是否包含某个值
 * @param tableTdDisplayKeys
 * @param value
 * @returns {boolean}
 * @constructor
 */
function tableTdDisplayKeysContains(tableTdDisplayKeys, value) {
    var result = true;
    for (var i = 0; i < tableTdDisplayKeys.length; i++) {
        if (tableTdDisplayKeys[i].keyValue == value) {
            return result;
        }
    }
    return false;
}

/**
 * 获取json数据的key
 * @param data
 * @returns {Array}
 */
function getJsonKeys(data) {
    var keyArray = [];
    for (var key in data) {
        keyArray.push(key + "");
    }

    return keyArray;
}

/**
 * 删除所在行
 * @param row
 */
function deleteRow(row) {
    row = $(row);
    var ele = row.parent().parent();
    ele.remove();
}

/**
 * 初始化表格支持树形结构的表格
 * @param tableId
 * @param data
 * @param ops
 * @param childrenName
 */
function initTable(tableId, data,ops,childrenName) {
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
    html+="<tr class='elementHidden'>";
    for (var z = 0; z < tableTdAttributeNodeList.length; z++) {
        html += "<td class=\"" + tableTdAttributeNodeList[z].classValue + "\" name=\"" + tableTdAttributeNodeList[z].keyValue + "\"></td>";
    }
    html += "</tr>";


    var childrenTreeType = 1000;
    $(tableId + " tbody").html(html+createTbodyStatement(data,tableTdAttributeNodeList,ops,childrenName,false,childrenTreeType));
}


/**
 * 构造tbody html语句块 支持树形结构的表格构造
 * @param html
 * @param data
 * @param tableTdAttributeNodeList
 * @param ops
 * @param childrenName
 * @param isHidden
 * @param childrenTreeType
 * @returns {*}
 */
function createTbodyStatement(data,tableTdAttributeNodeList,ops,childrenName,isHidden,childrenTreeType){
    //构造表格tbody

    var currentChildrenTreeType = childrenTreeType;

    var html="";
    for (var k = 0; k < data.length; k++) {

        if(isHidden){
            html += "<tr class=\"elementHidden childrenTree"+currentChildrenTreeType+"\">";
        }else{
            html += "<tr>";
        }

        var children = data[k][childrenName];
        for (var x = 0; x < tableTdAttributeNodeList.length; x++) {

            var node = tableTdAttributeNodeList[x];
            var tdText = data[k][node.keyValue];
            if(tdText!=null&&tdText!="null"){


                if(node.classValue.indexOf("childrenIcon")>=0&&children!=null&&children.length>0){

                    html += "<td class=\"" + node.classValue + "\" name=\"" + node.keyValue + "\">" +
                        "<span class='glyphicon glyphicon-search' aria-hidden='true' open-tree = 'false'" +
                        " onclick=\"openOrCloseTree('childrenTree"+(currentChildrenTreeType+1)+"',this)\"></span>"
                        + tdText + "</td>";

                }else{

                    html += "<td class=\"" + node.classValue + "\" name=\"" + node.keyValue + "\">"
                        + tdText + "</td>";

                }
            }else{

                html += "<td class=\"" + node.classValue + "\" name=\"" + node.keyValue + "\"></td>";
            }

        }

        //构造操作列，比如删除，修改等
        if(ops!=null&&ops.length>0){

            html+="<td>";
            for(var opsCounter = 0;opsCounter<ops.length;opsCounter++){
                var op = ops[opsCounter];

                var opStatement = op.opFunction+"(";
                var parameterNames = op.parameterNames;
                if(parameterNames!=null&&parameterNames.length>0){

                    var parameterNamesLength = parameterNames.length;

                    for(var a=0;a<parameterNamesLength;a++){
                        if(a==parameterNamesLength-1){
                            opStatement+=data[k][parameterNames[a]];
                        }else{
                            opStatement+=data[k][parameterNames[a]]+",";
                        }
                    }
                }

                opStatement+=")";

                html+="<button onclick='"+opStatement+"'>"+op.opName+"</button>";
            }
            html+="</td>";
        }
        html += "</tr>";
        //如果有子数，则递归
        if(children!=null&&children.length>0){
            html+=createTbodyStatement(children,tableTdAttributeNodeList,ops,childrenName,true,++childrenTreeType);
        }
    }
    return html;
}

function openOrCloseTree(classValue,obj){
    obj=$(obj);
    if(obj.attr("open-tree")=="true"){
        obj.attr("open-tree",false);
        $("."+classValue).hide();
    }else if(obj.attr("open-tree")=="false"){
        obj.attr("open-tree",true);
        $("."+classValue).show();

    }
}

/**
 * 序列化表单参数
 * @param formId 表单id
 * @returns {{}} JSON对象
 */
function getFormJson(formId) {
    formId = "#" + formId;
    var o = {};
    var a = $(formId).serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
}