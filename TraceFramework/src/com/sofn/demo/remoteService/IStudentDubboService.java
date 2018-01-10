package com.sofn.demo.remoteService;

import com.sofn.demo.entity.StudentBean;

/**
 * @ClassName:IStudentDubboService
 * @Description TODO(dubbo远程服务测试接口)
 * @author xinglai.chen
 * @date 2016年6月8日下午5:14:28
 */
public interface IStudentDubboService {
	StudentBean getStudent();
}
