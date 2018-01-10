var express = require('express');
var router = express.Router();



router.post('/service/verify', function(req, res, next) {
    var username=req.body.username;
    var data={};
    if(username=='aaaa1111'){
        data.resultCode='0020';
        data.result={
            message:'用户名已存在'
        };
    }else if(username=='aaaa2222'){
        data.resultCode='0010';
        data.result={
            message:'用户名效验失败'
        };
    }else{
        data.resultCode='0000';
        data.result={
            message:'用户名可用'
        };
    }
    res.send(data);
});

router.post('/service/register', function(req, res, next) {
    var username=req.body.username;
    var data={};
    if(username=='aaaa3333'){
        data.resultCode='0010';
        data.result={
            message:'注册失败'
        };
    }else{
        data.resultCode='0000';
        data.result={
            message:'注册成功'
        };
    }
    res.send(data);
});

router.post('/service/queryList', function(req, res, next) {
    var page=parseInt(req.body.page);
    var pageSize=parseInt(req.body.pageSize);
    var datas=[];
    for(var i=1; i<=pageSize; i++) {
        if((((page-1)*pageSize)+i)=='100'){
            break;
        }
        datas.push({"id":  (((page-1)*pageSize)+i), "name": "张三" + (((page-1)*pageSize)+i),'phone':13544445555,'unit':'曙光集团','date':'2016-06-16','price':'12.9'});
    };
    res.send({
        'resultCode':'0000',
        'result':{
            'rows':datas,
            'message':'成功',
            'total':100
        }
    })
});

router.post('/service/queryItem', function(req, res, next) {
    var id=parseInt(req.body.id);
    console.log(id);
    res.send({
        'resultCode':'0000',
        'result':{
            name:'张三'+id,
            sex:1,
            date:'2012-01-11 11:11:11',
            age:18,
            phone:'13544445555',
            smoke:true,
            //color:{
            //    red:0,
            //    blue:1,
            //    yellow:1
            //}
            color:[1,2]
        }
    })
});

router.post('/service/getSex', function(req, res, next) {
    res.send({
        'resultCode':'0000',
        'result':[{
                value:0,
                text:'女'
            },{
                value:1,
                text:'男'
            }]
    })
});

router.post('/service/updateSub', function(req, res, next) {
    res.send({
        'resultCode':'0000',
        'result':{
            'message':'修改成功'
        }
    })
});

router.post('/service/deleteSub', function(req, res, next) {
    res.send({
        'resultCode':'0000',
        'result':{
            'message':'删除成功'
        }
    })
});

router.post('/service/getColorList', function(req, res, next) {
    res.send({
        'resultCode':'0000',
        'result':[{
                val:0,
                text:'红色',
                name:'red'
            },{
                val:1,
                text:'黄色',
                name:'yellow'
            },{
                val:2,
                text:'绿色',
                name:'green'
            },{
                val:3,
                text:'蓝色',
                name:'blue'
            }]
    })
});

router.post('/service/validate', function(req, res, next) {
    res.send({
        'resultCode':'0021',
        'result':[{
            fieldName:'phone',
            message:'电话号码错误'
        },{
            fieldName:'age',
            message:'年龄错误'
        }]
    })
});

module.exports = router;
