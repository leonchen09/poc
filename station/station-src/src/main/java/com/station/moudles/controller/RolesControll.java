package com.station.moudles.controller;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import org.apache.commons.collections.CollectionUtils;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.entity.Permissions;
import com.station.moudles.entity.RolePermission;
import com.station.moudles.entity.Roles;
import com.station.moudles.entity.RolesDetail;
import com.station.moudles.entity.UserRole;
import com.station.moudles.service.PermissionsService;
import com.station.moudles.service.RolePermissionService;
import com.station.moudles.service.RolesService;
import com.station.moudles.service.UserRoleService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchRolesPagingVo;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping("/roles")
public class RolesControll extends BaseController{
	@Autowired
	RolesService rolesSer;
	@Autowired
	RolePermissionService rolePermissionsSer;
	@Autowired
	RolePermissionService rolePermissionSer;
	@Autowired
	UserRoleService userRoleSer;
	@Autowired
	PermissionsService permissionsSer;

	@RequestMapping(value = "/save", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "新增角色模板及权限", notes = "新增角色模板及权限")
	public AjaxResponse<Object> save(@RequestBody RolesDetail roles) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		List<String> permissionCodeList = roles.getPermissionCodeList();
		try {
			Roles role = new Roles();
			role.setRoleName(roles.getRoleName());		
			List<Roles> roleList = rolesSer.selectListSelective(role);
			if(!CollectionUtils.isEmpty(roleList)) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("模板名称重复！");
				return ajaxResponse;
			}
			rolesSer.insertSelective(roles);
			List<Roles> latestRolesList = rolesSer.selectListSelective(roles);
			Roles latestRole = latestRolesList.stream().max(Comparator.comparing(Roles::getRoleId)).get();
			for (String code : permissionCodeList) {
				// 根据code得到权限对象，得到 id
				Permissions queryPermissions = new Permissions();
				queryPermissions.setPermissionCode(code);
				Permissions permissions = permissionsSer.selectListSelective(queryPermissions).get(0);
				// role 和 permission 建立关系
				RolePermission rolePermission = new RolePermission();
				rolePermission.setRoleId(latestRole.getRoleId());
				rolePermission.setPermissionId(permissions.getPermissionId());
				rolePermissionSer.insertSelective(rolePermission);
			}
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("模板名称输入有误！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/bindUser", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "用户设置角色模板", notes = "建立绑定关系")
	public AjaxResponse<Object> bindingUser(@RequestBody List<UserRole> userRoleList) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			if (userRoleList != null && userRoleList.size() > 0) {
				for (UserRole userRole : userRoleList) {
					userRoleSer.insertSelective(userRole);
				}
			}
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("设置模板失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/unbindUser", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "用户设置角色模板", notes = "解除绑定关系")
	public AjaxResponse<Object> unbindUser(@RequestBody List<UserRole> userRoleList) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			if (userRoleList != null && userRoleList.size() > 0) {
				for (UserRole userRole : userRoleList) {
					userRoleSer.deleteSelective(userRole);
				}
			}
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("删除模板失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/delete/{roleId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "通过pk删除", notes = "删除")
	public AjaxResponse<Object> deleteByRoleId(@PathVariable Integer roleId) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			rolesSer.deleteByPrimaryKey(roleId);
			RolePermission deleteRolePerm = new RolePermission();
			deleteRolePerm.setRoleId(roleId);
			rolePermissionSer.deleteSelective(deleteRolePerm);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("删除失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/update", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "通过pk更新", notes = "属性为null的不更新")
	public AjaxResponse<Object> update(@RequestBody RolesDetail roles) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<>();
		try {
			rolesSer.updateByPrimaryKeySelective(roles);
			RolePermission deleteRolePerm = new RolePermission();
			deleteRolePerm.setRoleId(roles.getRoleId());
			rolePermissionSer.deleteSelective(deleteRolePerm);
			List<String> permissionCodeList = roles.getPermissionCodeList();
			for (String code : permissionCodeList) {
				// 根据code得到权限对象，得到 id
				Permissions queryPermissions = new Permissions();
				queryPermissions.setPermissionCode(code);
				Permissions permissions = permissionsSer.selectListSelective(queryPermissions).get(0);
				// role 和 permission 建立关系
				RolePermission rolePermission = new RolePermission();
				rolePermission.setRoleId(roles.getRoleId());
				rolePermission.setPermissionId(permissions.getPermissionId());
				rolePermissionSer.insertSelective(rolePermission);
			}
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("修改出错！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取roles列表", notes = "返回列表")
	public AjaxResponse<List<Roles>> getRolesInfoList(@RequestBody Roles roles) {
		AjaxResponse<List<Roles>> ajaxResponse = new AjaxResponse<>();
		try {
			List<Roles> rolesList = rolesSer.selectListSelective(roles);
			ajaxResponse.setData(rolesList);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取数据失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/listPage", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取roles分页列表", notes = "返回分页列表")
	public AjaxResponse<ShowPage<Roles>> getRolesInfoListPage(@RequestBody SearchRolesPagingVo rolesPagingVo) {
		AjaxResponse<ShowPage<Roles>> ajaxResponse = new AjaxResponse<ShowPage<Roles>>();
		try {
			List<Roles> rolesList = rolesSer.selectListSelectivePaging(rolesPagingVo);
			ShowPage<Roles> page = new ShowPage<Roles>(rolesPagingVo, rolesList);
			ajaxResponse.setData(page);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取数据失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/detail/{roleId}")
	@ResponseBody
	@ApiOperation(value = "根据pk获取详情", notes = "返回详情")
	public AjaxResponse<RolesDetail> getRoleDetail(@PathVariable Integer roleId) {
		AjaxResponse<RolesDetail> ajaxResponse = new AjaxResponse<>();
		try {
			RolesDetail rolesDetail = new RolesDetail();
			Roles roles = rolesSer.selectByPrimaryKey(roleId);
			BeanUtils.copyProperties(roles, rolesDetail);
			List<Permissions> permissionList = permissionsSer.selectListByRoleId(roleId);
			List<String> permissionCodeList = permissionList.stream().flatMap(p -> Stream.of(p.getPermissionCode()))
					.collect(Collectors.toList());
			rolesDetail.setPermissionCodeList(permissionCodeList);
			ajaxResponse.setData(rolesDetail);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取数据失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/ofUser/{userId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据userId得到所属角色", notes = "返回所属角色")
	public AjaxResponse<List<Roles>> getRoleOfUser(@PathVariable Integer userId) {
		AjaxResponse<List<Roles>> ajaxResponse = new AjaxResponse<>();
		try {
			List<Roles> roleList = rolesSer.selectListByUserId(userId);
			ajaxResponse.setData(roleList);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取数据失败！");
		}
		return ajaxResponse;
	}
}
