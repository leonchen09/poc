app.controller("mainCtrl",["$scope",'main', function ($scope,main) {
    $scope.register={
        username:{
            val:'',
            flog:false,
            text:''
        },
        password:{
            val:'',
            flog:false,
            text:''
        }
    };

    var registerVerify={
        username:{
            required:{
                flog:true,
                parameter:'',
                msg:'不能为空'
            },
            min:{
                flog:true,
                parameter:8,
                msg:'最小长度为8'
            },
            max:{
                flog:false,
                parameter:13,
                msg:'最大长度为13'
            },
            reg:{
                flog:true,
                parameter:'',
                msg:'格式错误'
            },
            only:{
                flog:true,
                parameter:'',
                msg:'用户名已存在'
            }

        },
        password:{
            required:{
                flog:true,
                parameter:'',
                msg:'不能为空'
            },
            min:{
                flog:true,
                parameter:8,
                msg:'最小长度为8'
            },
            max:{
                flog:false,
                parameter:13,
                msg:'最大长度为13'
            },
            reg:{
                flog:true,
                parameter:'',
                msg:'格式错误'
            }
        }
    };
    var verifyFuc={
        required:function(obj,n){
            if(obj.val== n.parameter&&!obj.flog){
                return {flog:true,text: n.msg}
            }
        },
        min:function(obj,n){
            if(obj.val.length<n.parameter&&!obj.flog){
                return {flog:true,text:n.msg}
            }
        },
        max:function(obj,n){
            if(obj.val.length>n.parameter&&!obj.flog){
                return {flog:true,text:n.msg}
            }
        },
        reg:function(obj,n){
            if(!(/\d/.test(obj.val) && /[a-zA-Z]/.test(obj.val) && /^[a-zA-Z0-9]+$/.test(obj.val))&&!obj.flog){
                return {flog:true,text:n.msg}
            }
        },
        only:function(obj,n){
            if(!obj.flog){
                main.verify({username:obj.val}).success(function(data){
                    if('0020'==data.resultCode){
                        obj.flog=true;
                        obj.text= n.msg;
                    }
                });
            }
        }
    };

    //失去焦点验证
    $scope.verify=function(username){
        $scope.register[username].flog=false;
        $scope.register[username].text='';
        $.each(registerVerify[username],function(i,n){
            if(n.flog){
                var errObj=verifyFuc[i]($scope.register[username],n);
                if(errObj&&i!='only'){
                    $scope.register[username].flog=errObj.flog;
                    $scope.register[username].text=errObj.text;
                }
            }
        });
    };

    //注册提交
    $scope.registerClick=function(){
        var f=true;
        $.each($scope.register,function(i,n){
            if(n.flog|| n.val==''){
                f=false;
            }
        });
        if(f){
            //调用注册方法
            main.register({
                username:$scope.register.username.val,
                password:$scope.register.password.val
            }).success(function(data){
               if(data.resultCode=='0000'){
                   alert(data.result.message);
               }
            });
        }else{
            $.each($scope.register,function(i,n){
                $scope.verify(i);
            })
        }
    };
}]);