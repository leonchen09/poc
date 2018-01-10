/**
 * 分页组件.
 */
app.directive('movePage', [
    function() {
        'use strict';
        return {
            restrict: 'A',
            link: function (scope, ele, attrs, transclude) {
                ele.addClass('list_Pagination_z');
                var watchClose = scope.$watch(attrs.movePage, function(newValue, oldValue) {
                    if(newValue) {
                        //watchClose();
                        //console.log(newValue)
                        if(scope[attrs.movePage].total>0){
                            makeMovePage(
                                scope[attrs.movePage].total,
                                parseInt(scope[attrs.movePage].page),
                                parseInt(scope[attrs.movePage].pageSize)
                            );
                        }
                    }
                });

                function makeMovePage(total, page, pageSize) {
                    ele.html('');
                    var aFrist = angular.element('<a class="pages fontFamilyST" href="javascript:void(0)">|&lt;</a>'),
                        aLeft = angular.element('<a class="pages fontFamilyST" href="javascript:void(0)">&lt;</a>'),
                        aLast = angular.element('<a class="pages fontFamilyST" href="javascript:void(0)">&gt;|</a>'),
                        aRight = angular.element('<a class="pages fontFamilyST" href="javascript:void(0)">&gt;</a>'),
                        goBtn = angular.element('<button class="Pagination_btn">GO</button>');
                    ele.append(aFrist, aLeft);

                    var n = Math.ceil(total/pageSize);
                    //var begin = (Math.floor(page/10) * 10);
                    var begin = page - 4;
                    if(begin < 1)begin = 0;
                    var i,j=1;
                    if(begin == 0){
                        begin++;
                    }

                    for (i = begin; i <= n && j<=10; i++,j++) {
                        var a = angular.element('<a href="javascript:void(0)">' + i + '</a>');
                        if(i == page)
                            a.addClass('pageNow');
                        else
                            a.addClass('pages');
                        a.on('click', function(event) {
                            if(!angular.element(this).hasClass('active')){
                                scope[attrs.search](angular.element(this).html());
                            }
                        });
                        ele.append(a);
                    }

                    ele.append(aRight, aLast,
                        '<input type="text" class="Pagination_input" style="margin-left: 5px"/>',
                        goBtn,
                        '<div class="page_tip">' + page + '/' + n + '&nbsp;&nbsp;共'+total+'条</div>');

                    goBtn.on('click', function() {
                        var val = angular.element(this).parent().find('input').val();
                        if(!isNaN(val) && val > 0 && val <= n) {
                            scope[attrs.search](val);
                        }
                    });

                    aFrist.on('click', function(event) {
                        scope[attrs.search](1);
                    });

                    aLeft.on('click', function(event) {
                        if(page > 1)
                            scope[attrs.search](page - 1);
                    });

                    aLast.on('click', function(event) {
                        scope[attrs.search](n);
                    });

                    aRight.on('click', function(event) {
                        if(page < n)
                            scope[attrs.search](page + 1);
                    });
                }
            }
        }
    }]);