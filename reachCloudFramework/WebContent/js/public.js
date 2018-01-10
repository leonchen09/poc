/**
 * Created by Meteor on 2016/4/26.
 */
/**
 * 表格td元素属性节点
 * @param keyValue
 * @param classValue
 * @param formatFunction
 * @constructor
 */
function TableTdAttributeNode(keyValue, classValue, formatFunction) {
    this.keyValue = keyValue;
    this.classValue = classValue;
    this.formatFunction = formatFunction;
}

/**
 * 操作类
 * @param opName 操作名称
 * @param opFunction 调用方法
 * @param parameterNames 调用参数
 * @constructor
 */
function OpClass(opName, opFunction, parameterNames) {
    this.opName = opName;
    this.opFunction = opFunction;
    this.parameterNames = parameterNames;
}

/**
 * 表格每行最前面的选择器
 * @param selectorType 选择器类型（checkbox,radio）
 * @param selectorName 选择器name属性值
 * @param valueName 选择器值所对应的字段名称
 * @constructor
 */
function TableSelector(selectorType, selectorName, valueName) {
    this.selectorType = selectorType;
    this.selectorName = selectorName;
    this.valueName = valueName;
}

/**
 * 初始化表格
 * @param tableId
 * @param data
 * @param ops
 * @param tableSelector
 */
function initTable(tableId, data, ops, tableSelector) {
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
        tableTdAttributeNodeList.push(
            new TableTdAttributeNode(tds[i].attr("name"), tds[i].attr("class"), tds[i].attr("formatFunction"))
        );
    }

    var html = "";

    //构造一行demo
    html += "<tr class='elementHidden'>";
    for (var z = 0; z < tableTdAttributeNodeList.length; z++) {
        html += "<td class=\"" + tableTdAttributeNodeList[z].classValue + "\" name=\"" + tableTdAttributeNodeList[z].keyValue + "\"";

        if (tableTdAttributeNodeList[z].formatFunction !== undefined) {
            html += "formatFunction=\"" + tableTdAttributeNodeList[z].formatFunction + "\"";
        }

        html += "></td>";
    }
    html += "</tr>";

    if (data !== undefined && data.length > 0) {
        html += createTbodyStatement(data, tableTdAttributeNodeList, ops, tableSelector);
    } else {
        html += "<tr><td colspan='6' style='text-align: center'>No Data</td></tr>";
    }

    $(tableId + " tbody").html(html);
}

/**
 * 构造tbody html语句块
 * @param data
 * @param tableTdAttributeNodeList
 * @param ops
 * @param tableSelector
 * @returns {*}
 */
function createTbodyStatement(data, tableTdAttributeNodeList, ops, tableSelector) {
    //构造表格tbody
    var html = "";
    for (var k = 0; k < data.length; k++) {
        html += "<tr>";
        for (var x = 0; x < tableTdAttributeNodeList.length; x++) {

            var node = tableTdAttributeNodeList[x];
            var tdText = data[k][node.keyValue];

            html += "<td class=\"" + node.classValue + "\" name=\"" + node.keyValue + "\">";

            if (x == 0 && tableSelector != null) {
                html += "<input type=\"" + tableSelector.selectorType + "\" " +
                    "name='" + tableSelector.selectorName + "' class=\"table-selector\" " +
                    "value=\"" + data[k][tableSelector.valueName] + "\"/>";
            }

            if (tdText != null && tdText != "null") {

                if (node.formatFunction !== undefined) {
                    tdText = eval(node.formatFunction + "(" + tdText + ")");
                }

                html += tdText + "</td>";
            } else {
                html += "</td>";
            }

        }

        //构造操作列，比如删除，修改等
        if (ops != null && ops.length > 0) {

            html += "<td class='tdCenter'><div class=\"btn-group btn-group-xs\" role=\"group\">";
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

                html += "<input type='button' class='btn btn-info' onclick='" + opStatement + "' value=" + op.opName + ">&nbsp;&nbsp;";
            }
            html += "</div></td>";
        }
        html += "</tr>";
    }
    return html;
}


/**
 * 构建分页组件
 * @param elementId 放置分页组件的容器的id
 * @param functionName 该分页组件获取数据的方法名称
 */
function createPageComponent(elementId, functionName) {
    var html = "";
    html += "<span class='label pageInfoSpan'>" +
        "<span>PageSize</span>" +
        "</span>" +
        "<select class='pageInfoSpan' id='pageSize' name='pageSize'>" +
        "<option value='5'>5</option>" +
        "<option value='10'>10</option>" +
        "<option value='15'>15</option>" +
        "<option value='20'>20</option>" +
        "</select>" +
        "<span class='label pageInfoSpan'>" +
        "<span></span>" +
        "</span>" +

        "<span class='label pageInfoSpan'>" +
        "<span>No&nbsp;&nbsp;</span>" +
        "<span id='currentPage'>0</span>" +
        "<span>&nbsp;/&nbsp;</span>" +
        "<span id='totalPage'>0</span>" +
        "<span>&nbsp;&nbsp;Page&nbsp;&nbsp;</span>" +
        "<span>Total&nbsp;&nbsp;</span>" +
        "<span id='totalRecord'>0</span>" +
        "<span>&nbsp;&nbsp;</span>" +
        "</span>" +

        "<ul class='pager'>" +
        "<li id='firstPage'><a href=\"#\" onclick='firstPage(\"" + functionName + "\")'>HomePage</a></li>" +
        "<li id='lastPage'><a href=\"#\" onclick='lastPage(\"" + functionName + "\")'>Previous </a></li>" +
        "<li id='nextPage'><a href=\"#\" onclick='nextPage(\"" + functionName + "\")'>Next</a></li>" +
        "<li id='endPage'><a href=\"#\" onclick='endPage(\"" + functionName + "\")'>LastPage</a></li>" +
        "</ul>" +
        "<input type='hidden' id='pageNoHidden' name='pageNo' value='1'>";

    $("#" + elementId).html(html);
}

/**
 * 首页
 * @param functionName 该分页组件获取数据的方法名称
 */
function firstPage(functionName) {
    $("#pageNoHidden").val(1);
    eval(functionName + "()");
}

/**
 * 上一页
 * @param functionName 该分页组件获取数据的方法名称
 */
function lastPage(functionName) {
    var currentPage = parseInt($("#currentPage").text()) - 1;
    //判断上一页是否为0
    if (currentPage <= 0) {
        $("#lastPage").prop("class", "disabled");
        return;
    }
    $("#pageNoHidden").val(currentPage);
    eval(functionName + "()");
}

/**
 * 下一页
 * @param functionName 该分页组件获取数据的方法名称
 */
function nextPage(functionName) {
    var currentPage = parseInt($("#currentPage").text()) + 1;
    var totalPage = parseInt($("#totalPage").text());
    //判断下一页是否超过总页数
    if (currentPage >= totalPage + 1) {
        $("#nextPage").prop("class", "disabled");
        return;
    }
    $("#pageNoHidden").val(currentPage);
    eval(functionName + "()");
}
/**
 * 尾页
 * @param functionName 该分页组件获取数据的方法名称
 */
function endPage(functionName) {
    var totalPage = parseInt($("#totalPage").text());
    $("#pageNoHidden").val(totalPage);
    eval(functionName + "()");
}

/**
 * 页面加载数据后重新设置分页信息
 * @param pageNo 当前页
 * @param totalPage 总页数
 * @param totalRecord 总记录数
 */
function setPageInfo(pageNo, totalPage, totalRecord) {
    $("#currentPage").text(pageNo);
    $("#totalRecord").text(totalRecord);
    $("#totalPage").text(totalPage);
    $("#pageNoHidden").val(pageNo);
    $("#lastPage").removeAttr("class");
    $("#nextPage").removeAttr("class");
}

(function ($) {
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        var str = this.serialize();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };
})(jQuery);

/**
 * 自动填充表单
 * @param formId 表单id
 * @param data JSON数据
 */
function insertFormData(formId, data) {
    var inputs = $("#" + formId + " :input");
    var tagName, type, key, checkBoxArr;
    inputs.each(function () {
        tagName = $(this)[0].tagName;
        type = $(this).attr("type");
        key = $(this).attr("name");

        if (tagName == 'INPUT') {
            if (type == "checkbox") {
                $(this).prop('checked', false);
                checkBoxArr = data[key].split(",");
                for (var value in checkBoxArr) {
                    if (checkBoxArr[value] == $(this).val()) {
                        $(this).prop('checked', true);
                    }
                }

            } else if (type == 'radio') {
                $(this).prop('checked', $(this).val() == data[key]);
            } else {
                $(this).val(data[key]);
            }
        } else if (tagName == 'SELECT' || tagName == 'TEXTAREA') {
            $(this).val(data[key]);
        }
    });
}


/**
 * 分页查询的 ajax 方法封装
 * @param reqType 请求类型
 * @param asyncFlag 是否异步
 * @param url 请求地址
 * @param parameters 查询参数
 * @param dataType 数据类型
 * @param callbackFunction 回调方法
 */
function searchAjaxFunction(reqType, asyncFlag, url, parameters, dataType, callbackFunction) {
    $.ajax({
        type: reqType,
        async: asyncFlag,
        url: url,
        data: {"data": JSON.stringify(parameters)},
        dataType: dataType,
        success: function (data) {
            callbackFunction["success"](data);
        }
    });
}

/**
 * 除分页查询外的其他ajax方法封装
 * @param reqType 请求类型
 * @param asyncFlag 是否异步
 * @param url 请求地址
 * @param parameters 请求参数
 * @param dataType 数据类型
 * @param callbackFunction 回调方法
 */
function submitAjaxFunction(reqType, asyncFlag, url, parameters, dataType, callbackFunction) {
    $.ajax({
        type: reqType,
        async: asyncFlag,
        url: url,
        data: parameters,
        dataType: dataType,
        success: function (data) {
            callbackFunction["success"](data);
        }
    });
}
/**
 * 获取checkbox属性
 * @param checkboxName checkbox name属性值
 */
function getCheckboxValues(checkboxName) {

    var values = [];

    var testCheckboxes = $(":checkbox[name='" + checkboxName + "']:checked");

    testCheckboxes.each(function () {

        values.push($(this).val());

    });
    return values;
}

/**
 * checkbox全选
 * @param checkboxName checkbox name属性值
 */
function checkboxSelectAll(checkboxName) {
    $(":checkbox[name='" + checkboxName + "']").prop("checked", true);
}

/**
 * checkbox全不选
 * @param checkboxName checkbox name属性值
 */
function checkboxUnSelect(checkboxName) {
    $(":checkbox[name='" + checkboxName + "']").prop("checked", false);
}

/**
 * checkbox反选
 * @param checkboxName checkbox name属性值
 */
function checkboxReverse(checkboxName) {

    alert("Reverse");

    $(":checkbox[name='" + checkboxName + "']").each(function () {
        $(this).prop("checked", !$(this).prop("checked"));
    });
}

/**
 * 日期控件初始化
 */
(function ($) {
    $.fn.initDatePicker = function () {

        var obj = $(this).children("input");

        obj.datetimepicker({
            language: 'zh-CN',
            weekStart: 1,
            todayBtn: 1,
            autoclose: 1,
            todayHighlight: 1,
            startView: 2,
            minView: 2,
            forceParse: 0
        });

        var icons = $(this).children("span");

        if (icons[0] != null) {
            $(icons[0]).click(function () {
                obj.val("");
            });
        }
        if(icons[1]!= null){
            $(icons[1]).click(function () {
                obj.datetimepicker('show');
            });
        }
    };
})(jQuery);





