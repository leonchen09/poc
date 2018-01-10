package com.sofn.demo.service;

import com.sofn.demo.entity.StudentBean;

/**
 * @ClassName:ITestDubboService
 * @Description TODO(测试本地调用dubbo远程服务接口)
 * @author xinglai.chen
 * @date 2016年6月8日下午5:15:59
 */
public interface ITestDubboService {
	StudentBean getStudent();
}
