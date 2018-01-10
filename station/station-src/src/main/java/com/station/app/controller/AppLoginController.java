package com.station.app.controller;

import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URLDecoder;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.commons.httpclient.Header;
import org.apache.commons.httpclient.HttpClient;
import org.apache.commons.httpclient.NameValuePair;
import org.apache.commons.httpclient.methods.PostMethod;
import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;
import com.fasterxml.jackson.core.type.TypeReference;
import com.github.qcloudsms.SmsSingleSender;
import com.github.qcloudsms.SmsSingleSenderResult;
import com.google.common.collect.Lists;
import com.station.common.Constant;
import com.station.common.utils.JsonUtil;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.jwt.JwtHelper;
import com.station.moudles.controller.BaseController;
import com.station.moudles.controller.TemplateExportController;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.User;
import com.station.moudles.entity.WarnArea;
import com.station.moudles.service.ParameterService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.UserService;
import com.station.moudles.service.impl.GprsConfigInfoServiceImpl;
import com.station.moudles.service.impl.WarningInfoServiceImpl;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.LoginUserVo;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;
import com.station.moudles.vo.search.SearchWarningInfoPagingVo;

import io.swagger.annotations.ApiOperation;
import net.sf.jxls.parser.CellParser;

/**
 * app 登录
 * @author admin
 *
 */
@RestController
@RequestMapping(value = "/app/login")
public class AppLoginController extends BaseController {
	
	
	@Autowired
	private UserService userSer;
	@Autowired
	WarningInfoServiceImpl warningInfoSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	GprsConfigInfoServiceImpl gprsConfigInfoSer;

	/**
	 * app 用户登录
	 * @param loginUser
	 *        参数是 loginId , password, userType
	 * @return
	 */
	@RequestMapping(value = "/doLogin", method = RequestMethod.POST)
	public AjaxResponse<User> doLogin(@RequestBody LoginUserVo loginUser) {
		AjaxResponse<User> ajaxResponse = validateBean(loginUser);
		if (ajaxResponse != null) {
			return ajaxResponse;
		}
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		User queryUser = new User();
		BeanUtils.copyProperties(loginUser, queryUser);
		// 按用户名/密码查询出用户，不判断usertype。
		queryUser.setUserType(null);
		queryUser.setDisableFlag(0);
		List<User> users = userSer.selectListSelective(queryUser);
		ajaxResponse = new AjaxResponse<>(Constant.RS_CODE_ERROR, "用户名或密码错误！");
		if (users.size() == 0) {
			return ajaxResponse;
		}
		User user = users.get(0);
		int i = user.getUserType().intValue() & loginUser.getUserType().intValue();
		if ((user.getUserType().intValue() & loginUser.getUserType().intValue()) != loginUser.getUserType()) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "当前用户无权访问当前系统！");
		}
		String tokenStr = JwtHelper.createToken(users.get(0).getUserId().toString());
		user.setToken(tokenStr);
		//根据用户的三级公司查询gprs_config_info表得到刷新时间
		GprsConfigInfo gprsConfigInfo = new GprsConfigInfo();
		gprsConfigInfo.setCompanyId(users.get(0).getCompanyId());
		List<GprsConfigInfo> gprsConfigList = gprsConfigInfoSer.selectListSelective(gprsConfigInfo);
		if (gprsConfigList.size() != 0) {
			Integer refreshTime = gprsConfigList.get(0).getRefreshTime();
			if (refreshTime == null) {
				user.setRefreshTime(60);
			} else {
				user.setRefreshTime(refreshTime);
			}
		}else {
			user.setRefreshTime(60);
		}
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("用户登录成功");
		ajaxResponse.setData(user);
		return ajaxResponse;
	}

	/**
	 * 获取验证码
	 * @param user
	 *根据UserPhone获取验证码
	 * @return
	 */
	@RequestMapping(value = "/getCode", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<Object> getCode(@RequestBody User user) throws Exception {
		if (user == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "设置登录用户名！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "获取验证码失败！");
		if (user.getUserPhone() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "设置登录电话号码！");
		}
		List<User> users = userSer.selectListSelective(user);
		if (users.size() != 0) {
			try {
				// 随机生成 0-9 六位验证码
				List<Integer> nums = Lists.newArrayList();
				int num;
				for (int i = 0; i < 6; i++) {
					do {
						num = (int) (Math.random() * 10);
					} while (nums.contains(num) || num == 0);
					nums.add(num);
				}
				String code = StringUtils.join(nums, "");
				int appid = 1400057108;
				SmsSingleSender sender = new SmsSingleSender(appid, "1c1018076746ae9a8f4951ef8d49b535");
				SmsSingleSenderResult result = sender.send(0, "86", users.get(0).getUserPhone(),
						"智慧巡检找回密码验证码：" + code + "，请于1分钟内填写。如非本人操作，请忽略本短信。", "", "");

				String str = JSON.toJSONString(result);
				JSONObject jsonRes = JSON.parseObject(str);
				if (0== (int)jsonRes.get("result")) {
					// 将验证码保存在数据中
					user.setUserCode(code);
					user.setUserId(users.get(0).getUserId());
					userSer.updateByPrimaryKeySelective(user);
					ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
					ajaxResponse.setMsg("验证码发送成功，请在30分钟内输入，请等待！");
					return ajaxResponse;
				} 
				if(1025== (int)jsonRes.get("result")){
					ajaxResponse.setMsg("当日单个手机号发送验证码过多！");
					return ajaxResponse;
				}
				if(1033 == (int)jsonRes.get("result") ) {
					ajaxResponse.setMsg("公司申请的短信条数不足！");
					return ajaxResponse;
				}
			} catch (Exception e) {
				return ajaxResponse;
			}
		} else {
			ajaxResponse.setMsg("用户没有注册");
			return ajaxResponse;
		}
		return ajaxResponse;
	}

	/**
	 * 修改密码
	 * @param user
		根据UserPhone修改 传递的参数是 新password
	 * @return
	 */
	@RequestMapping(value = "/updatePassword", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk更新", notes = "根据pk更新，属性为null的不更新")
	public AjaxResponse<Object> updatePassword(@RequestBody User user) {
		if (user.getUserPhone() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请设置电话号码！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "修改出错！");
		// 查询验证码
		User query = new User();
		query.setUserPhone(user.getUserPhone());
		List<User> userCode = userSer.selectListSelective(query);
		if (userCode.size() != 0) {
			if (!user.getUserCode().equals(userCode.get(0).getUserCode())) {
				ajaxResponse.setMsg("验证码不正确！");
				return ajaxResponse;
			}
			try {
				user.setUserId(userCode.get(0).getUserId());
				userSer.updateByPrimaryKeySelective(user);
				userCode.get(0).setUserCode(null);
				userSer.updateByPrimaryKey(userCode.get(0));//修改成功后验证码为null
				ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
				ajaxResponse.setMsg("修改成功！");
			} catch (Exception e) {
				ajaxResponse.setMsg("修改密码失败！");
			}
		}
		return ajaxResponse;
	}
	/**
	 * 个人中心 辖区告警信息统计
	 * @return
	 */
	@RequestMapping(value = "/warnArea/statistics", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取基站区域报警表列表", notes = "返回基站区域报警列表")
	public AjaxResponse<Map<String, Object>> getWarnAreaList(@RequestBody CommonSearchVo commonSearchVo) {
		AjaxResponse<Map<String, Object>> ajaxResponse = new AjaxResponse<Map<String, Object>>(Constant.RS_CODE_ERROR,
				"获取区域报警信息失败！");
		if (commonSearchVo.getLoginId() == null) {
			return new AjaxResponse<Map<String, Object>>(Constant.RS_CODE_ERROR, "用户名参数没有！");
		}
		User user = new User();
		user.setLoginId(commonSearchVo.getLoginId());
		List<User> userList = userSer.selectListSelective(user);
		if (userList.size() == 0) {
			ajaxResponse.setMsg("用戶名不正确！");
			return ajaxResponse;
		}
		CommonSearchVo com = new CommonSearchVo();
		com.setCompanyLevel(userList.get(0).getCompanyLevel());
		com.setCompanyId(userList.get(0).getCompanyId());
		//获取刷新时间
		GprsConfigInfo gprsConfig = new GprsConfigInfo();
		gprsConfig.setCompanyId(userList.get(0).getCompanyId());
		List<GprsConfigInfo> gprsConfigList = gprsConfigInfoSer.selectListSelective(gprsConfig);
		Map<String, Object> map = new HashMap<String, Object>();
		List<WarnArea> warningInfoList = warningInfoSer.appSelectWarnAreaList(com);
		// 总掉电数量 /占比
		int lossNum = 0;
		double lossPercent = 0;
		// 单体温度过高/占比
		int cellTemHight = 0;
		double cellTemHightPercent = 0;
		// 单体温度过低/占比
		int cellTemLow = 0;
		double cellTemLowPercent = 0;
		// 电压过高/占比
		int hightVol = 0;
		double hightVolPercent = 0;
		// 电压过低/占比
		int lowVol = 0;
		double lowVolePercent = 0;
		// 环境温度过高/占比
		int envHightTem = 0;
		double envHightTemPercent = 0;
		// 环境温度过低
		int envLowTem = 0;
		double envLowTemPercent = 0;
		// 电量过低
		int socLow = 0;
		double socLowPercent = 0;
		if (warningInfoList.size() != 0) {
			for (WarnArea warnArea : warningInfoList) {
				if (warnArea.getLossElectricityNum() != null)
					lossNum += warnArea.getLossElectricityNum();
				if (warnArea.getLossElectricityPercent() != null)
					lossPercent += Double.parseDouble(warnArea.getLossElectricityPercent());

				if (warnArea.getCellTemHighNum() != null)
					cellTemHight += warnArea.getCellTemHighNum();
				if (warnArea.getCellTemHighPercent() != null)
					cellTemHightPercent += Double.parseDouble(warnArea.getCellTemHighPercent());

				if (warnArea.getCellTemLowNum() != null)
					cellTemLow += warnArea.getCellTemLowNum();
				if (warnArea.getCellTemLowPercent() != null)
					cellTemLowPercent += Double.parseDouble(warnArea.getCellTemLowPercent());

				if (warnArea.getGenVolHighNum() != null)
					hightVol += warnArea.getGenVolHighNum();
				if (warnArea.getGenVolHighPercent() != null)
					hightVolPercent += Double.parseDouble(warnArea.getGenVolHighPercent());

				if (warnArea.getGenVolLowNum() != null)
					lowVol += warnArea.getGenVolLowNum();
				if (warnArea.getGenVolLowPercent() != null)
					lowVolePercent += Double.parseDouble(warnArea.getGenVolLowPercent());

				if (warnArea.getEnvTemHighNum() != null)
					envHightTem += warnArea.getEnvTemHighNum();
				if (warnArea.getEnvTemHighPercent() != null)
					envHightTemPercent += Double.parseDouble(warnArea.getEnvTemHighPercent());

				if (warnArea.getEnvTemLowNum() != null)
					envLowTem += warnArea.getEnvTemLowNum();
				if (warnArea.getEnvTemLowPercent() != null)
					envLowTemPercent += Double.parseDouble(warnArea.getEnvTemLowPercent());

				if (warnArea.getSocLowNum() != null)
					socLow += warnArea.getSocLowNum();
				if (warnArea.getSocLowPercent() != null)
					socLowPercent += Double.parseDouble(warnArea.getSocLowPercent());
			}
			lossPercent = lossPercent != 0 ? lossPercent / warningInfoList.size() : 0;
			cellTemHightPercent = cellTemHightPercent != 0 ? cellTemHightPercent / warningInfoList.size() : 0;
			cellTemLowPercent = cellTemLowPercent != 0 ? cellTemLowPercent / warningInfoList.size() : 0;
			hightVolPercent = hightVolPercent != 0 ? hightVolPercent / warningInfoList.size() : 0;
			lowVolePercent = lowVolePercent != 0 ? lowVolePercent / warningInfoList.size() : 0;
			envHightTemPercent = envHightTemPercent != 0 ? envHightTemPercent / warningInfoList.size() : 0;
			envLowTemPercent = envLowTemPercent != 0 ? envLowTemPercent / warningInfoList.size() : 0;
			socLowPercent = socLowPercent != 0 ? socLowPercent / warningInfoList.size() : 0;
		}
		map.put("lossNum", lossNum);
		map.put("lossPercent", lossPercent);

		map.put("cellTemHight", cellTemHight);
		map.put("cellTemHightPercent", cellTemHightPercent);

		map.put("cellTemLow", cellTemLow);
		map.put("cellTemLowPercent", cellTemLowPercent);

		map.put("hightVol", hightVol);
		map.put("hightVolPercent", hightVolPercent);

		map.put("lowVol", lowVol);
		map.put("lowVolePercent", lowVolePercent);

		map.put("envHightTem", envHightTem);
		map.put("envHightTemPercent", envHightTemPercent);

		map.put("envLowTem", envLowTem);
		map.put("envLowTemPercent", envLowTemPercent);

		map.put("socLow", socLow);
		map.put("socLowPercent", socLowPercent);
		ajaxResponse = new AjaxResponse<Map<String, Object>>(map);
		return ajaxResponse;

		// if(warningInfoList.size() != 0) {
		// WarnArea warn =warningInfoList.get(warningInfoList.size()-1);
		// //直接放到map中返回
		// map.put("lossNum", warn.getLossElectricityNum());
		// map.put("lossPercent", warn.getLossElectricityPercent());
		//
		// map.put("cellTemHight", warn.getCellTemHighNum());
		// map.put("cellTemHightPercent", warn.getCellTemHighPercent());
		//
		// map.put("cellTemLow", warn.getCellTemLowNum());
		// map.put("cellTemLowPercent", warn.getCellTemLowPercent());
		//
		// map.put("hightVol", warn.getGenVolHighNum());
		// map.put("hightVolPercent", warn.getGenVolHighPercent());
		//
		// map.put("lowVol", warn.getGenVolLowNum());
		// map.put("lowVolePercent", warn.getGenVolLowPercent());
		//
		// map.put("envHightTem", warn.getEnvTemHighNum());
		// map.put("envHightTemPercent", warn.getEnvTemHighPercent());
		//
		// map.put("envLowTem", warn.getEnvTemLowNum());
		// map.put("envLowTemPercent", warn.getEnvTemLowPercent());
		//
		// map.put("socLow", warn.getSocLowNum());
		// map.put("socLowPercent", warn.getSocLowPercent());
		// ajaxResponse = new AjaxResponse<Map<String,Object>>(map);
		// }
		// return ajaxResponse;
	}

	/**
	 * 告警详细信息列表
	 * 
	 * @param searchStationInfoPagingVo
	 * @return
	 */
	@RequestMapping(value = "/warnArea/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "告警详细信息", notes = "返回列表")
	public AjaxResponse<ShowPage<StationInfo>> getStationInfoList(
			@RequestBody SearchWarningInfoPagingVo searchWarningInfoPagingVo) {
		searchWarningInfoPagingVo.setRcvTime(MyDateUtils.getDiffTime(-1 * 60 * 1000));
		List<StationInfo> stationInfoList = stationInfoSer.appWarnAreaSelectListSelective(searchWarningInfoPagingVo);
		ShowPage<StationInfo> page = new ShowPage<StationInfo>(searchWarningInfoPagingVo, stationInfoList);
		AjaxResponse<ShowPage<StationInfo>> ajaxResponse = new AjaxResponse<ShowPage<StationInfo>>(page);
		return ajaxResponse;
	}

	/*
	 * private String parent =
	 * TemplateExportController.class.getClassLoader().getResource("").getPath() +
	 * "base_template/"; private String[] sourceName = { "巡检app.apk"};
	 *//**
		 * app下载
		 * 
		 * @param type
		 * @return
		 *//*
			 * @RequestMapping(value="/donwnloadApp/{type}",method = RequestMethod.GET)
			 * 
			 * @ResponseBody public AjaxResponse<Object> downloadApp(@PathVariable Integer
			 * type) throws Exception {
			 * 
			 * AjaxResponse<Object> ajaxResponse = new
			 * AjaxResponse(Constant.RS_CODE_SUCCESS,"下载成功！"); if (type < 0 || type >=
			 * sourceName.length) { ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			 * ajaxResponse.setMsg("请不要传入非法参数！"); return ajaxResponse; } String filePath =
			 * parent + sourceName[type]; filePath = URLDecoder.decode(filePath, "UTF-8");
			 * File file = new File(filePath); String name = sourceName[type]; String
			 * filename = new String(name.getBytes("UTF-8"), "iso-8859-1"); // 设置response
			 * response.setContentType("application/x-msdownload; charset=UTF-8");
			 * response.addHeader("content-type", "application/x-msdownload;");
			 * response.addHeader("content-disposition", "attachment; filename=" +
			 * filename); response.setContentLength((int) file.length()); // 输出 InputStream
			 * in = new FileInputStream(file); OutputStream os = response.getOutputStream();
			 * try { int length = 0; byte[] buffer = new byte[1024]; while ((length =
			 * in.read(buffer)) != -1) { os.write(buffer, 0, length); } os.flush(); }
			 * finally { if (in != null) { in.close(); } } return ajaxResponse;
			 * 
			 * }
			 */
}
