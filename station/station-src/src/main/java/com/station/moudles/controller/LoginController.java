package com.station.moudles.controller;

import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import com.station.common.Constant;
import com.station.common.utils.MD5;
import com.station.common.utils.jwt.JwtHelper;
import com.station.moudles.entity.Permissions;
import com.station.moudles.entity.User;
import com.station.moudles.service.CompanyService;
import com.station.moudles.service.PermissionsService;
import com.station.moudles.service.UserService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.LoginUserVo;

@RestController
@RequestMapping(value = "/login")
public class LoginController extends BaseController {
	
	@Autowired
	CompanyService companySer;
	@Autowired
	private UserService userSer;
	@Autowired
	PermissionsService permissionsSer;

	@RequestMapping(value = "/doLogin", method = RequestMethod.POST)
	public AjaxResponse<User> doLogin(@RequestBody LoginUserVo loginUser) {
		AjaxResponse<User> ajaxResponse = validateBean(loginUser);
		if (ajaxResponse != null) {
			return ajaxResponse;
		}
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		User queryUser = new User();
		BeanUtils.copyProperties(loginUser, queryUser);
		queryUser.setDisableFlag(0);
		// 按用户名/密码查询出用户，不判断usertype。
		queryUser.setUserType(null);
		List<User> users = userSer.selectListSelective(queryUser);
		ajaxResponse = new AjaxResponse<>(Constant.RS_CODE_ERROR, "用户名或密码错误！");
		if (users.size() == 0) {
			return ajaxResponse;
		}
		User user = users.get(0);
		if ((user.getUserType().intValue() & loginUser.getUserType().intValue()) != loginUser.getUserType()) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "当前用户无权访问当前系统！");
		}
		String tokenStr = JwtHelper.createToken(users.get(0).getUserId().toString());
		user.setToken(tokenStr);
		List<Permissions> permissionList = permissionsSer.selectListByUserId(user.getUserId());
		permissionList = permissionList.stream().filter(p -> p.getPermissionSystem() == user.getUserType())
				.collect(Collectors.toList());
		Set<String> permissionCodeList = permissionList.stream().flatMap(p -> Stream.of(p.getPermissionCode()))
				.collect(Collectors.toSet());
		user.setPermissionCodeList(permissionCodeList);
		//将通过用户返回公司放到user里面
		String loginId = loginUser.getLoginId();
		List<Map<String, String>> companyList = companySer.selectCompany1ByUser(loginId);
		if(companyList.size() != 0) {
		Map<String, String> map = companyList.get(0);
		user.setCompany1(map.get("company1"));
		user.setCompany2(map.get("company2"));
		user.setCompany3(map.get("company3"));
		}
		//-------end
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("用户登录成功");
		ajaxResponse.setData(user);
		return ajaxResponse;
	}
	/**
	 * 通过用户id返回公司
	 */
	@RequestMapping(value="/user",method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<List<Map<String, String>>> getCompany3ByUserId(@RequestBody User user){
		String loginId = user.getLoginId();
		List<Map<String, String>> selectCompany1ByUser = companySer.selectCompany1ByUser(loginId);
		AjaxResponse<List<Map<String,String>>> ajaxResponse =  new AjaxResponse<List<Map<String, String>>>(Constant.RS_CODE_SUCCESS,"获取成功");
		ajaxResponse.setData(selectCompany1ByUser);
		return ajaxResponse;
		
	}
	
	
	
	
	public static void main(String[] args) {
		System.out.println(MD5.toMD5("admin"));
	}
}
