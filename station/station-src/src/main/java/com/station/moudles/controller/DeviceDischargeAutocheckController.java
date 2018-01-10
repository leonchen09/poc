package com.station.moudles.controller;

import java.math.BigDecimal;
import java.math.BigInteger;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.ListIterator;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.apache.commons.collections.CollectionUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.entity.DeviceDischargeAutocheck;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.helper.DischargeEvent;
import com.station.moudles.quartz.DeviceDischargeAutocheckTask;
import com.station.moudles.service.DeviceDischargeAutocheckService;
import com.station.moudles.service.ReportService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.impl.GprsConfigInfoServiceImpl;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.report.ChargeDischargeEvent;
import com.station.moudles.vo.search.SearchDeviceChargeAutocheckVo;

/**
 * 自动检测放电
 * 
 * @author ywg
 *
 */
@Controller
@RequestMapping(value = "/deviceDischargeAutocheck")
public class DeviceDischargeAutocheckController extends BaseController {

	@Autowired
	DeviceDischargeAutocheckService deviceDischargeAutocheckSer;
	@Autowired
	ReportService reportService;
	@Autowired
	GprsConfigInfoServiceImpl gprsConfigInfoSer;
	@Autowired
	StationInfoService stationInfoSer;

	/**
	 * 分页列表
	 * 
	 * @param deviceDischargeAutocheck
	 * @return
	 */
	@RequestMapping(value = "/listPage", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<ShowPage<DeviceDischargeAutocheck>> listPageDeviceDischargeAutocheck(
			@RequestBody SearchDeviceChargeAutocheckVo searchDeviceChargeAutocheckVo) {

		List<DeviceDischargeAutocheck> DeviceDischargeList = deviceDischargeAutocheckSer
				.selectListSelectivePaging(searchDeviceChargeAutocheckVo);
		ShowPage<DeviceDischargeAutocheck> page = new ShowPage<DeviceDischargeAutocheck>(searchDeviceChargeAutocheckVo,
				DeviceDischargeList);
		AjaxResponse<ShowPage<DeviceDischargeAutocheck>> ajaxResponse = new AjaxResponse<ShowPage<DeviceDischargeAutocheck>>(
				page);

		return ajaxResponse;

	}
	
	/**
	 * 新增
	 * 
	 * @param deviceDischargeAutocheck
	 * @return
	 */
	@RequestMapping(value = "/save", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<DeviceDischargeAutocheck> saveDeviceDischargeAutocheck(
			@RequestBody DeviceDischargeAutocheck deviceDischargeAutocheck) {
		AjaxResponse<DeviceDischargeAutocheck> ajaxResponse = new AjaxResponse<DeviceDischargeAutocheck>(
				Constant.RS_CODE_ERROR, "新增自动检测放电数据出错！");
		if (deviceDischargeAutocheck == null) {
			return ajaxResponse;
		}
		deviceDischargeAutocheckSer.insertSelective(deviceDischargeAutocheck);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("新增 自动检测放电成功");
		return ajaxResponse;

	}

	/**
	 * 删除
	 * 
	 * @param deviceDischargeAutocheck
	 * @return
	 */
	@RequestMapping(value = "/delete", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<DeviceDischargeAutocheck> deleteDeviceDischargeAutocheck(
			@RequestBody DeviceDischargeAutocheck deviceDischargeAutocheck) {
		if (deviceDischargeAutocheck.getId() == null) {
			return new AjaxResponse<DeviceDischargeAutocheck>(Constant.RS_CODE_ERROR, "请设置pk！");
		}
		AjaxResponse<DeviceDischargeAutocheck> ajaxResponse = new AjaxResponse<DeviceDischargeAutocheck>(
				Constant.RS_CODE_ERROR, "删除自动检测放电数据出错！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		deviceDischargeAutocheckSer.deleteByPrimaryKey(deviceDischargeAutocheck.getId());
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("删除 自动检测放电成功");
		return ajaxResponse;

	}

	/**
	 * 修改
	 * 
	 * @param deviceDischargeAutocheck
	 * @return
	 */
	@RequestMapping(value = "/update", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<DeviceDischargeAutocheck> updateDeviceDischargeAutocheck(
			@RequestBody DeviceDischargeAutocheck deviceDischargeAutocheck) {
		if (deviceDischargeAutocheck.getId() == null) {
			return new AjaxResponse<DeviceDischargeAutocheck>(Constant.RS_CODE_ERROR, "请设置pk！");
		}
		AjaxResponse<DeviceDischargeAutocheck> ajaxResponse = new AjaxResponse<DeviceDischargeAutocheck>(
				Constant.RS_CODE_ERROR, "修改自动检测放电数据出错！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		deviceDischargeAutocheckSer.updateByPrimaryKeySelective(deviceDischargeAutocheck);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("修改自动检测放电成功");
		return ajaxResponse;

	}

	/**
	 * 查询
	 * 
	 * @param deviceDischargeAutocheck
	 * @return
	 */
	@RequestMapping(value = "/select", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse selectDeviceDischargeAutocheck(@RequestBody DeviceDischargeAutocheck deviceDischargeAutocheck) {
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_ERROR, "查询自动检测放电失败！");
		List<DeviceDischargeAutocheck> deviceDischarge = deviceDischargeAutocheckSer
				.selectListSelective(deviceDischargeAutocheck);
		if (deviceDischarge.isEmpty()) {
			return ajaxResponse;
		}
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("查询自动检测放电成功");
		ajaxResponse.setData(deviceDischarge);
		return ajaxResponse;

	}

	/**
	 * 定时查询更新
	 * 
	 * @param deviceDischargeAutocheck
	 * @return
	 */
	@RequestMapping(value = "/timingSelect/{gprsId}", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse timingSelectDeviceDischargeAutocheck(@PathVariable("gprsId") String gprsId) {
		
	//	DeviceDischargeAutocheckTask  task = new DeviceDischargeAutocheckTask();
	//	task.dischargeAutoCheck();

		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_SUCCESS, "添加放电检测数据成功！");

		//查询所有的gprsid
		GprsConfigInfo gprsConfigInfo = new GprsConfigInfo();
		List<GprsConfigInfo> gprsConfigInfoList = gprsConfigInfoSer.selectRcvTimeNotNull(gprsConfigInfo);
		if (gprsConfigInfoList.size() == 0) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("设备不存在");
			return ajaxResponse;
		}
		for(int k = 0 ; k<gprsConfigInfoList.size() ; k++) {
		DeviceDischargeAutocheck deviceDischarge = new DeviceDischargeAutocheck();
		//根据gprs_id 和is_correct 为null
		deviceDischarge.setGprsId(gprsConfigInfoList.get(k).getGprsId());
		List<DeviceDischargeAutocheck> isDevice = deviceDischargeAutocheckSer.getIsDevice(deviceDischarge);

		// 验证时间
		// 初始化当前时间
		Date endTime = new Date();
		Date startTime = null;
		int validDay = gprsConfigInfoList.get(0).getValidDay();
		long start = endTime.getTime(); // 当前开始时间毫秒数
		Calendar calendar = Calendar.getInstance();    
		calendar.setTime(endTime);    
		calendar.add(Calendar.DATE, -validDay);//当前时间减去期限天数		
		Date time = calendar.getTime();  
		if (isDevice.size() == 0) {
			startTime = time;
		} else if (isDevice.get(0).getIsCorrect() == null){
			startTime = isDevice.get(0).getCheckDate();
			Calendar calendar1 = Calendar.getInstance();
			calendar1.setTime(startTime);
			calendar1.add(Calendar.DATE, -1);// 减去一天
			Date time1 = calendar1.getTime();
			startTime = time1;
			//如果原来有数据 就删除
			deviceDischargeAutocheckSer.deleteByPrimaryKey( isDevice.get(0).getId());
			logger.debug("删了一条数据","id="+isDevice.get(0).getId(),"设备编号是："+isDevice.get(0).getGprsId());
		}else {
			continue;
		}
		
		while (true) {
			// 电池组有效放电数据
			List<ChargeDischargeEvent> report = reportService.getChargeDischargeEvent(gprsId, startTime, endTime,
					new DischargeEvent(false), null);
			if (report != null && report.size() != 0 && report.get(0).getDetails().size() != 0) {
				// 放电详情
				List<PackDataInfo> packDateInfoList = report.get(0).getDetails();
				// 排除后面的10 条非放电数据
					List<PackDataInfo> newPackDateInfo = new ArrayList<PackDataInfo>();
					newPackDateInfo.addAll(packDateInfoList.subList(0, packDateInfoList.size() - 10));
					// 第一次放电时间 
					deviceDischarge.setFirstDischargeTime(newPackDateInfo.get(newPackDateInfo.size()-1).getRcvTime());
					if (newPackDateInfo.size() >= 50) {

						// 超过50条
						BigDecimal startSumVol = BigDecimal.ZERO;
						for (int i = 0; i < 20; i++) {
							PackDataInfo dataInfo = newPackDateInfo.get(i);
							startSumVol=startSumVol.add(dataInfo.getGenVol());
						}

						BigDecimal endSumVol = BigDecimal.ZERO;
						for (int i = newPackDateInfo.size() - 20; i < newPackDateInfo.size(); i++) {
							PackDataInfo dataInfo = newPackDateInfo.get(i);
							endSumVol=endSumVol.add(dataInfo.getGenVol());
						}

						BigDecimal big = new BigDecimal(20.00);
						// 开始电压的平均值- 结束电压的平均值
						BigDecimal subtract = startSumVol.divide(big).subtract(endSumVol.divide(big));
						int r = subtract.compareTo(BigDecimal.ZERO); // 和0，Zero比较
						// 小于 -1 等于 0 大于 1
						if (r == -1) {
							// 数据状态 0 正确 1 错误
							deviceDischarge.setIsCorrect(1);
						} else {
							deviceDischarge.setIsCorrect(0);
						}
						deviceDischarge.setGprsId(gprsId);
						// 开始电压平均值
						deviceDischarge.setStartVol(startSumVol.divide(big));
						// 结束电压平均值
						deviceDischarge.setEndVol(endSumVol.divide(big));
						// 写入改记录时间
						deviceDischarge.setCheckDate(new Date());
						// 未修复0 修改1
						deviceDischarge.setDataUpdated(0);	
						deviceDischargeAutocheckSer.insertSelective(deviceDischarge);
						break;
					} else {
						// 不符合50条要求 得到该批次数据的最早时间 作为结束时间
						PackDataInfo packDataInfo = newPackDateInfo.get(newPackDateInfo.size() - 1);
						endTime = packDataInfo.getRcvTime();
					
						// 如果得到的时间超过了极限时间要跳出循环
						long s =time.getTime();
						long t = endTime.getTime();
						if (endTime.getTime() < time.getTime()) {
							
							deviceDischarge.setGprsId(gprsId);
							// 开始电压
							deviceDischarge.setStartVol(null);
							// 结束电压
							deviceDischarge.setEndVol(null);
							// 写入改记录时间
							deviceDischarge.setCheckDate(new Date());
							// 未修复0 修改1
							deviceDischarge.setDataUpdated(null);
							deviceDischarge.setIsCorrect(null);
							deviceDischargeAutocheckSer.insertSelective(deviceDischarge);
							break;
						}
					}
			}else {
				deviceDischarge.setGprsId(gprsId);
				// 开始电压
				deviceDischarge.setStartVol(null);
				// 结束电压
				deviceDischarge.setEndVol(null);
				// 写入改记录时间
				deviceDischarge.setCheckDate(new Date());
				// 未修复0 修改1
				deviceDischarge.setDataUpdated(null);
				deviceDischarge.setIsCorrect(null);
				deviceDischargeAutocheckSer.insertSelective(deviceDischarge);
				logger.info("没有放电数据:" + gprsId, startTime, endTime);
				//logger.debug("完成！！！！！！!耗时：" + (new Date().getTime() - sd.getTime()) / 1000 + "s");
				break;
			}
			
		}
		}
		return ajaxResponse;

	}
}
