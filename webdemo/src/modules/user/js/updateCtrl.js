app.controller("updateCtrl",["$scope",'$stateParams','$state','list','ngDialog','validation', function ($scope,$stateParams,$state,list,ngDialog,validation) {

    var getOption=function(){
        list.getSex().success(function(d){
           if(d.resultCode=='0000'){
               $scope.sexList= d.result;
           }
        });
        list.getColorList().success(function(d){
            if(d.resultCode=='0000'){
                $scope.colorList= d.result;
                $.each($scope.colorList,function(i,n){
                    $.each($scope.upDateForm.color,function(v,k){
                        if(n.val==k){
                            n.checked=true;
                        }
                    })
                })
            }
        });
    };
    getOption();

    $scope.validate = {
        name:{isError:false,text:''},
        phone: {isError: false, text: ''},
        age: {isError: false, text: ''}
    };

    var validate={
        name:{required:true},
        phone: {required:true,phone:true},
        age:{digits:true}
    };

    var validateDate=function(data, valid, target){
        var isValid = true;
        var fun = {
            required: function (is, key) {
                if(!is) return true;
                if($.trim(data[key]) == '') {
                    target[key].text = '请填写相关内容！';
                    target[key].isError = true;
                    return false;
                }else{
                    target[key].text = '';
                    target[key].isError = false;
                    return true;
                }
            },
            digits: function (is, key) {
                if(!is) return true;
                if(validation.isDigit(data[key])||data[key].length>2) {
                    target[key].text = '请填两位以内的正整数！';
                    target[key].isError = true;
                    return false;
                }else{
                    target[key].text = '';
                    target[key].isError = false;
                    return true;
                }
            },
            phone: function(is, key) {
                if(!is || !data[key]) return true;
                if(!validation.isPhone(data[key])) {
                    target[key].text = '请填写正确的电话号码！';
                    target[key].isError = true;
                    return false;
                }else{
                    target[key].text = '';
                    target[key].isError = false;
                    return true;
                }
            }
        };

        $.each(valid, function(i, v) {
            $.each(v, function(i2, v2) {
                console.log(i,v,'----');
                console.log(i2,v2,'++++');
                if(!fun[i2](v2, i)){
                    isValid = false;
                }
            });
        });
        return isValid;
    };

    $scope.upDateSubmit=function(){
        if(!validateDate($scope.upDateForm, validate, $scope.validate)){
            return false;
        }
        var colorArr=[];
        $.each($scope.colorList,function(i,n){
            if(n.val!='0'){
                colorArr.push(n.val);
            }
        });
        $scope.upDateForm.color=colorArr;
        list.validate($scope.upDateForm).success(function(d){
           if(d.resultCode=='0000'){
               list.updateSub(
                   $scope.upDateForm
               ).success(function(d){
                       if(d.resultCode=='0000'){
                           alert(d.result.message);
                           ngDialog.close();
                       }
                   });
           }else if(d.resultCode=='0021'){
               $.each(d.result,function(i,n){
                   $scope.validate[n.fieldName].isError=true;
                   $scope.validate[n.fieldName].text= n.message;
               })
           }
        });
    }
}]);