package com.station.moudles.controller;

import java.util.HashMap;
import java.util.Map;

import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import com.station.common.Constant;
import com.station.common.utils.jwt.JwtHelper;
import com.station.moudles.vo.AjaxResponse;

import io.swagger.annotations.ApiOperation;

@RestController
@RequestMapping(value = "/token")
public class TokenController extends BaseController {
	@ApiOperation(value = "更新token")
	@RequestMapping(value = "/renew", method = RequestMethod.POST)
	public AjaxResponse<Map<String, String>> updateToken() {
		AjaxResponse<Map<String, String>> ajaxResponse = new AjaxResponse<Map<String, String>>(Constant.RS_CODE_ERROR, "更新token失败");
		String tokenStr = JwtHelper.createToken(userId);
		Map<String, String> resultMap = new HashMap<String, String>();
		resultMap.put("token", tokenStr);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("更新token成功");
		ajaxResponse.setData(resultMap);
		return ajaxResponse;
	}

	@ApiOperation(value = "获取token")
	@RequestMapping(value = "/get/{userId}", method = RequestMethod.POST)
	public AjaxResponse<String> getToken(@PathVariable String userId) {
		AjaxResponse<String> ajaxResponse = new AjaxResponse<String>(Constant.RS_CODE_ERROR, "获取token失败");
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取token成功");
		String tokenStr = JwtHelper.createToken(userId);
		ajaxResponse.setData(tokenStr);
		return ajaxResponse;
	}

}
