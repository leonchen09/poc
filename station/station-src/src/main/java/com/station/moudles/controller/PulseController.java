package com.station.moudles.controller;

import com.station.common.Constant;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.service.PulseService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.PulseVo;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.validation.BindingResult;
import org.springframework.validation.ObjectError;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.InitBinder;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;

@Controller
@RequestMapping(value = "/pulse")
public class PulseController extends BaseController {

    private PulseService pulseService;

    @Autowired
    public PulseController(PulseService pulseService) {
        this.pulseService = pulseService;
    }

    @PostMapping(value = "/test")
    @ResponseBody
    public AjaxResponse pulseTest(@Validated @RequestBody PulseVo pulseVo,BindingResult bind) {
        AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_SUCCESS, "发起特征测试指令成功");
    	//对输入的区间五的范围进行验证
        if(bind.hasErrors()) {
    		List<ObjectError> errorList = bind.getAllErrors();
    		for (ObjectError objectError : errorList) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg(objectError.getDefaultMessage());
				return ajaxResponse;
			}
    	}
        try {
            pulseService.pulseTest(pulseVo);
        } catch (Exception e) {
            logger.error("发起特征测试指令失败", e);
            ajaxResponse = new AjaxResponse(Constant.RS_CODE_ERROR, "发起特征测试指令失败");
        }
        return ajaxResponse;
    }
}
