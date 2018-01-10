package com.station.moudles.controller;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.entity.Permissions;
import com.station.moudles.service.PermissionsService;
import com.station.moudles.vo.AjaxResponse;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping("/permissions")
public class PermissionsController extends BaseController {
	@Autowired
	PermissionsService permissionsSer;

	@RequestMapping(value = "/entity/{id}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取", notes = "根据pk,得到用户权限")
	public void getPermission(@PathVariable int id) {

	}

	@RequestMapping(value = "/save", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "新增", notes = "新增")
	public AjaxResponse<Object> save(@RequestBody Permissions permissions) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			permissionsSer.insertSelective(permissions);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("添加失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/delete/{permissionId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "通过pk删除", notes = "删除")
	public AjaxResponse<Object> deleteByPermissionId(@PathVariable Integer permissionId) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			permissionsSer.deleteByPrimaryKey(permissionId);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("删除失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/update", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "通过pk更新", notes = "属性为null的不更新")
	public AjaxResponse<Object> update(@RequestBody Permissions permissions) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			permissionsSer.updateByPrimaryKeySelective(permissions);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("修改出错！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取permission列表", notes = "放回列表")
	public AjaxResponse<List<Permissions>> getPermissionInfoList(@RequestBody Permissions permissions) {
		AjaxResponse<List<Permissions>> ajaxResponse = new AjaxResponse<>();
		try {
			List<Permissions> peimissionList = permissionsSer.selectListSelective(permissions);
			ajaxResponse.setData(peimissionList);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取数据失败！");
		}
		return ajaxResponse;
	}
	
	

}
