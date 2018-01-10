package com.station.app.controller;

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

import com.google.common.collect.Lists;
import com.station.common.Constant;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.jwt.JwtHelper;
import com.station.moudles.controller.BaseController;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.User;
import com.station.moudles.entity.WarnArea;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.UserService;
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
 * 
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
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("用户登录成功");
		ajaxResponse.setData(user);
		return ajaxResponse;
	}

	/**
	 * 获取验证码
	 * @param user
	 *            根据userid修改 传递的参数是 新password
	 * @return
	 */
	@RequestMapping(value = "/getCode/{loginId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk更新", notes = "根据pk更新，属性为null的不更新")
	public AjaxResponse<Object> getCode(@PathVariable String loginId) throws Exception{
		if (loginId == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "设置登录用户名！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "获取验证码失败！");
		User user = new User();
		user.setLoginId(loginId);
		List<User> users = userSer.selectListSelective(user);
		if (users.size() != 0) {
			HttpClient client = new HttpClient();
			PostMethod post = new PostMethod("http://sms.webchinese.cn/web_api/");//注册的api
			post.addRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=gbk");// 在头文件中设置转码
			//随机生成 0-9 六位验证码
			List<Integer> nums = Lists.newArrayList();
	        int num;
	        for (int i = 0; i < 6; i++) {
	            do {
	                num = (int) (Math.random() * 10) ;
	            } while (nums.contains(num));
	            nums.add(num);
	        }
	        String code = StringUtils.join(nums,"");
			NameValuePair[] data = { new NameValuePair("Uid", "英雄ywg"),// 注册的用户名
					// 注册成功后,登录网站使用的密钥，这个密钥要登录到国建网然后有一个API接口，点进去就有一个key，可以改，那个才是密钥
					new NameValuePair("Key", "ee5f65bdf70d87237e79"), 
					new NameValuePair("smsMob", loginId),//电话号码
					new NameValuePair("smsText", "短信验证码："+code) };//短信内容
			//将验证码保存在数据库中
			user.setUserCode(code);
			user.setUserId(users.get(0).getUserId());
			userSer.updateByPrimaryKeySelective(user);
			post.setRequestBody(data);
			client.executeMethod(post);
			Header[] headers = post.getResponseHeaders();
			int statusCode = post.getStatusCode();
			System.out.println("statusCode:" + statusCode);
			for (Header h : headers) {
				System.out.println(h.toString());
			}
			String result = new String(post.getResponseBodyAsString().getBytes("gbk"));
			//System.out.println(result);//打印出结果
			post.releaseConnection();
			if("1".equals(result)) {
				ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
				ajaxResponse.setMsg("获取验证码成功");
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
	 *        根据userid修改 传递的参数是 新password
	 * @return
	 */
	@RequestMapping(value = "/updatePassword", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk更新", notes = "根据pk更新，属性为null的不更新")
	public AjaxResponse<Object> updatePassword(@RequestBody User user) {
		if (user.getUserId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请设置userId！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "修改出错！");
		
		//查询验证码
		User userCode = userSer.selectByPrimaryKey(user.getUserId());
		
		if (!user.getUserCode().equals(userCode.getUserCode())) {
			ajaxResponse.setMsg("验证码不正确");
			session.removeAttribute("code");
			return ajaxResponse;
		}
		try {
			userSer.updateByPrimaryKeySelective(user);
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("修改成功！");
		} catch (Exception e) {
			ajaxResponse.setMsg("修改密码失败！");
			e.printStackTrace();
		}
		return ajaxResponse;
	}

	/**
	 * 个人中心 辖区告警信息统计
	 * @param loginId
	 * @return
	 */
	@RequestMapping(value = "/warnArea/statistics", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取基站区域报警表列表", notes = "返回基站区域报警列表")
	public AjaxResponse<Map<String,Object>> getWarnAreaList(@RequestBody CommonSearchVo commonSearchVo) {
		AjaxResponse<Map<String,Object>> ajaxResponse = new AjaxResponse<Map<String,Object>>(Constant.RS_CODE_ERROR,
				"获取区域报警信息失败！");
		if (commonSearchVo.getLoginId() == null) {
			return new AjaxResponse<Map<String,Object>>(Constant.RS_CODE_ERROR, "用户名参数没有！");
		}
		User user = new User();
		user.setLoginId(commonSearchVo.getLoginId());
		List<User> userList = userSer.selectListSelective(user);
		if (userList.size() == 0 ) {
			ajaxResponse.setMsg("用戶名不正确！");
			return ajaxResponse;
		}
		CommonSearchVo com = new CommonSearchVo();
		com.setCompanyLevel(userList.get(0).getCompanyLevel());
		com.setCompanyId(userList.get(0).getCompanyId());
		List<WarnArea> warningInfoList = warningInfoSer.selectWarnAreaList(com);
		Map<String,Object> map = new HashMap<String,Object>();
		//总掉电数量 /占比
		int lossNum = 0;
		double lossPercent =0;
		//单体温度过高/占比
		int cellTemHight = 0;
		double cellTemHightPercent = 0;
		//单体温度过低/占比
		int cellTemLow = 0;
		double cellTemLowPercent = 0;
		//电压过高/占比
		int hightVol = 0;
		double hightVolPercent =0;
		//电压过低/占比
		int lowVol = 0;
		double lowVolePercent =0;
		//环境温度过高/占比
		int envHightTem = 0;
		double envHightTemPercent = 0;
		//环境温度过低
		int envLowTem = 0;
		double envLowTemPercent = 0;
		//电量过低
		int socLow = 0;
		double socLowPercent =0;
		if(warningInfoList.size() != 0) {
			for (WarnArea warnArea : warningInfoList) {
				if(warnArea.getLossElectricityNum() != null)
				lossNum+=warnArea.getLossElectricityNum();
				if(warnArea.getLossElectricityPercent() != null)
				lossPercent+=Double.parseDouble(warnArea.getLossElectricityPercent());
				
				if(warnArea.getCellTemHighNum()!=null)
				cellTemHight+=warnArea.getCellTemHighNum();
				if(warnArea.getCellTemHighPercent() != null)
				cellTemHightPercent+=Double.parseDouble(warnArea.getCellTemHighPercent());
				
				if(warnArea.getCellTemLowNum() != null)
				cellTemLow+=warnArea.getCellTemLowNum();
				if(warnArea.getCellTemLowPercent() != null)
				cellTemLowPercent+=Double.parseDouble(warnArea.getCellTemLowPercent());
				
				if(warnArea.getGenVolHighNum() != null)
				hightVol+=warnArea.getGenVolHighNum();
				if(warnArea.getGenVolHighPercent() != null)
				hightVolPercent+=Double.parseDouble(warnArea.getGenVolHighPercent());
				
				if(warnArea.getGenVolLowNum() != null)
				lowVol+=warnArea.getGenVolLowNum();
				if(warnArea.getGenVolLowPercent() != null)
				lowVolePercent += Double.parseDouble(warnArea.getGenVolLowPercent());

				if(warnArea.getEnvTemHighNum() != null)
				envHightTem+=warnArea.getEnvTemHighNum();
				if(warnArea.getEnvTemHighPercent() != null)
				envHightTemPercent+=Double.parseDouble(warnArea.getEnvTemHighPercent());
				
				if(warnArea.getEnvTemLowNum() != null)
				envLowTem+=warnArea.getEnvTemLowNum();
				if(warnArea.getEnvTemLowPercent() != null)
				envLowTemPercent+=Double.parseDouble(warnArea.getEnvTemLowPercent());
				
				if(warnArea.getSocLowNum() != null)
				socLow+=warnArea.getSocLowNum();
				if(warnArea.getSocLowPercent() != null)
				socLowPercent+=Double.parseDouble(warnArea.getSocLowPercent());
			}
			lossPercent =lossPercent != 0 ? lossPercent / warningInfoList.size() : 0;
			cellTemHightPercent = cellTemHightPercent != 0 ? cellTemHightPercent/warningInfoList.size():0;
			cellTemLowPercent = cellTemLowPercent != 0 ? cellTemLowPercent/warningInfoList.size() : 0;
			hightVolPercent = hightVolPercent != 0 ? hightVolPercent/warningInfoList.size() : 0;
			lowVolePercent = lowVolePercent != 0 ? lowVolePercent/warningInfoList.size() : 0;
			envHightTemPercent = envHightTemPercent != 0?envHightTemPercent/warningInfoList.size():0;
			envLowTemPercent = envLowTemPercent != 0?envLowTemPercent/warningInfoList.size():0;
			socLowPercent=socLowPercent != 0 ? socLowPercent/warningInfoList.size() :0;	
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
		ajaxResponse = new AjaxResponse<Map<String,Object>>(map);
		return ajaxResponse;
	}

	/**
	 * 告警详细信息列表 
	 * @param searchStationInfoPagingVo
	 * @return
	 */
	@RequestMapping(value = "/warnArea/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "告警详细信息", notes = "返回列表")
	public AjaxResponse<ShowPage<StationInfo>> getStationInfoList(
			@RequestBody SearchWarningInfoPagingVo searchWarningInfoPagingVo) {
		Map<String, Object> m = new HashMap<String, Object>();
		searchWarningInfoPagingVo.setRcvTime(MyDateUtils.getDiffTime(-1 * 60 * 1000));
		List<StationInfo> stationInfoList = stationInfoSer.appWarnAreaSelectListSelectivePaging(searchWarningInfoPagingVo);
		ShowPage<StationInfo> page = new ShowPage<StationInfo>(searchWarningInfoPagingVo, stationInfoList);
		AjaxResponse<ShowPage<StationInfo>> ajaxResponse = new AjaxResponse<ShowPage<StationInfo>>(page);
		return ajaxResponse;
	}
}
