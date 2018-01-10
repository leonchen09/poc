/**
 * Created by Meteor on 2016/4/27.
 */

/**
 * 页面初始化方法
 */
function initPeoplePage(){
    //构建分页组件
    createPageComponent("pageInfoDiv","getTableData");
    //按钮事件绑定
    $("#search-btn").click(function(){
        getTableData();
    });

    $("#selectAll-btn").click(function(){
        checkboxSelectAll("peopleTable-selector");
    });

    $("#reverse-btn").click(function(){
        checkboxReverse("peopleTable-selector");
    });

    $("#batchDelete-btn").click(function(){
        deletePeoples();
    });

    $("#people-modal-submit").click(function(){
        savePeople();
    });

    /*$('#datetimepicker').datetimepicker({
        language:  'fr',
        weekStart: 1,
        todayBtn:  1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0
    });*/

    //initDatePicker();
    $(".date-picker").initDatePicker();

    getTableData();
}

/**
 * 测试调用初始化表格
 */
function getTableData() {
    $.ajax({
        type: "post",
        url: "/TraceFramework/people/getList.do",
        data: {"data": JSON.stringify($("#searchForm").serializeJson())},
        dataType: "json",
        success: function (data) {
            successFunction(data);
        }
    });

}

/**
 * 格式化数字
 * @param s 原始数字
 * @param n 保留小数位位数
 * @returns {string}
 */
function formatMoney(s, n) {
    n = n > 0 && n <= 20 ? n : 2;
    s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
    var l = s.split(".")[0].split("").reverse(),
        r = s.split(".")[1];
    t = "";
    for (i = 0; i < l.length; i++) {
        t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
    }
    return t.split("").reverse().join("") + "." + r;
}

function formatMoneyTwoDigits(str) {
    return formatMoney(str, 2);
}

function successFunction(data) {
    if (data.resultCode == "SUCCESS") {
        data = data.result;
        //重置分页信息及相关按钮状态
        setPageInfo(data.pageNo, data.totalPage, data.totalRecord);

        //加入操作
        var ops = [];
        ops.push(new OpClass("Delete", "deletePeople", ["id"]));
        ops.push(new OpClass("Update", "updatePeople", ["id"]));

        //选择选择器类型和value字段名称
        var selector = new TableSelector("checkBox", "peopleTable-selector", "id");
        initTable("peopleTable", data.result, ops, selector);
    } else {
        alert(data.result);
    }
}

/**
 * 保存（新增或修改）
 */
function savePeople() {

    $.ajax({
        type: "post",
        url: "/TraceFramework/people/save.do",
        data: $("#peopleForm").serializeJson(),
        dataType: "json",
        success: function (data) {
            console.log(data);
            getTableData();
        }
    });

}

/**
 * 删除
 * @param id
 */
function deletePeople(id) {

    $.ajax({
        type: "post",
        url: "/TraceFramework/people/delete.do",
        data: {"id": id},
        dataType: "json",
        success: function (data) {
            console.log(data);
            getTableData();
        }
    });

}

/**
 * 批量删除
 */
function deletePeoples() {

    $.ajax({
        type: "post",
        url: "/TraceFramework/people/batchDelete.do",
        data: {"ids": JSON.stringify(getCheckboxValues("peopleTable-selector"))},
        dataType: "json",
        success: function (data) {
            console.log(data);
            getTableData();
        }
    });
}

/**
 * 根据ID查询并自动插入表单
 * @param id
 */
function insertPeopleForm(id) {

    $.ajax({
        type: "post",
        url: "/TraceFramework/people/getById.do",
        data: {"id": id},

        dataType: "json",
        success: function (data) {
            insertFormData("peopleForm", data.result);
        }
    });
}

/**
 * 打开修改模态窗口
 * @param id
 */
function updatePeople(id) {
    insertPeopleForm(id);
    $('#peopleFormModal').modal();
}

/**
 * 日期控件初始化
 */
/*function initDatePicker(){
    $('#datetimepicker').datetimepicker({
        language:  'fr',
        weekStart: 1,
        todayBtn:  1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0
    });
}*/

$(function(){
    initPeoplePage();
});