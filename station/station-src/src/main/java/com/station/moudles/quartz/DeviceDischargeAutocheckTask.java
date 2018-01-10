package com.station.moudles.quartz;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;

import com.station.common.Constant;
import com.station.common.cache.InitCache;
import com.station.moudles.entity.DeviceDischargeAutocheck;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.helper.DischargeEvent;
import com.station.moudles.helper.EventParams;
import com.station.moudles.service.DeviceDischargeAutocheckService;
import com.station.moudles.service.ReportService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.impl.GprsConfigInfoServiceImpl;
import com.station.moudles.vo.report.ChargeDischargeEvent;

/**
 * 定时计算放电检测
 * 
 * @author
 *
 */
public class DeviceDischargeAutocheckTask {
	private static final Logger logger = LoggerFactory.getLogger(DeviceDischargeAutocheckTask.class);

	@Autowired
	DeviceDischargeAutocheckService deviceDischargeAutocheckSer;
	@Autowired
	ReportService reportService;
	@Autowired
	GprsConfigInfoServiceImpl gprsConfigInfoSer;

	public void dischargeAutoCheck() {
		Date sd = new Date();
/*		logger.debug("开始放电检测！！！！！！！" + Thread.currentThread().getName());
		if (InitCache.deviceDischargeAutocheckFlage) {
			logger.debug("开始放电检测处理中！！！！！！！！！！！！！！结束");
			return;
		} else {
			InitCache.deviceDischargeAutocheckFlage = true;
		}
		logger.debug("开始放电检测处理开始!!!!!!!!!!!");*/

		// 查询所有的gprsid且 返回数据的时间不为null
		GprsConfigInfo gprsConfigInfo = new GprsConfigInfo();
		List<GprsConfigInfo> gprsConfigInfoList = gprsConfigInfoSer.selectRcvTimeNotNull(gprsConfigInfo);
		if (gprsConfigInfoList.size() == 0) {
			logger.debug("没有设备数据");
			return;
		}
		for (int k = 0; k < gprsConfigInfoList.size(); k++) {
			DeviceDischargeAutocheck deviceDischarge = new DeviceDischargeAutocheck();
			// 根据gprs_id 和is_correct 为null
			String gprsId = gprsConfigInfoList.get(k).getGprsId();
			deviceDischarge.setGprsId(gprsId);
			List<DeviceDischargeAutocheck> isDevice = deviceDischargeAutocheckSer
					.getIsDevice(deviceDischarge);

			// 验证时间
			// 初始化当前时间
			Date endTime = new Date();
			Date startTime = null;
			int validDay = gprsConfigInfoList.get(0).getValidDay();
			long start = endTime.getTime(); // 当前开始时间毫秒数
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(endTime);
			calendar.add(Calendar.DATE, -validDay);// 当前时间减去期限天数
			Date time = calendar.getTime();
			//排除已经在device_discharge_autocheck 中的设备并且 is_correct 不为null
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
				EventParams params = new EventParams();
				params.currentCount = 10;
				params.forwardCount = 10;
				params.backwardCount = 0;
				List<ChargeDischargeEvent> report = reportService.getChargeDischargeEvent(gprsId, startTime, endTime,
						new DischargeEvent(true), params);
				if (report != null && report.size() != 0 && report.get(0).getDetails().size() != 0) {
					// 放电详情
					List<PackDataInfo> packDateInfoList = report.get(0).getDetails();
					// 排除后面的10 条非放电数据
					List<PackDataInfo> newPackDateInfo = new ArrayList<PackDataInfo>();
					newPackDateInfo.addAll(packDateInfoList.subList(0, packDateInfoList.size() - 10));
					// 第一次放电时间
					deviceDischarge.setFirstDischargeTime(newPackDateInfo.get(newPackDateInfo.size() - 1).getRcvTime());
					if (newPackDateInfo.size() >= 50) {//必须找到超过50条放电数据才可以做判断。
						//接口返回的数据，最近的时间在list的最前面。因此，list的开始是结束电压，list的结束时开始电压。
						BigDecimal endSumVol = BigDecimal.ZERO;
						for (int i = 0; i < 20; i++) {
							PackDataInfo dataInfo = newPackDateInfo.get(i);
							endSumVol = endSumVol.add(dataInfo.getGenVol());
						}

						BigDecimal startSumVol = BigDecimal.ZERO;
						for (int i = newPackDateInfo.size() - 20; i < newPackDateInfo.size(); i++) {
							PackDataInfo dataInfo = newPackDateInfo.get(i);
							startSumVol = startSumVol.add(dataInfo.getGenVol());
						}

						BigDecimal big = new BigDecimal(20.00);
						// 开始电压的平均值- 结束电压的平均值
						BigDecimal subtract = startSumVol.divide(big).subtract(endSumVol.divide(big));
						if(subtract.abs().doubleValue() < 2.0) {// 差值大于2，才能做判断，否则认为判断条件不足，继续往下找，或者超过时间，留待下次检测。
							PackDataInfo packDataInfo = newPackDateInfo.get(newPackDateInfo.size() - 1);
							endTime = packDataInfo.getRcvTime();
							// 如果得到的时间超过了极限时间要跳出循环			
							if (endTime.getTime() < startTime.getTime()) {
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
						}else {//开始，结束电压差值大于2，才进行判断。
							int r = subtract.compareTo(BigDecimal.ZERO); 
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
							logger.debug("记录了检测出的放电数据:" + gprsId, startTime, endTime);
							logger.debug("完成！！！！！！!耗时：" + (new Date().getTime() - sd.getTime()) / 1000 + "s");
							break;
						}
					} else {
						// 不符合50条要求 得到该批次数据的最早时间 作为结束时间
						PackDataInfo packDataInfo = newPackDateInfo.get(newPackDateInfo.size() - 1);
						endTime = packDataInfo.getRcvTime();

						// 如果得到的时间超过了极限时间要跳出循环			
						if (endTime.getTime() < startTime.getTime()) {

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
							logger.debug("没有满足50条的放电数据:" + gprsId, startTime, endTime);
							logger.debug("完成！！！！！！!耗时：" + (new Date().getTime() - sd.getTime()) / 1000 + "s");
							break;
						}
					}
				} else {
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
					logger.debug("完成！！！！！！!耗时：" + (new Date().getTime() - sd.getTime()) / 1000 + "s");
					break;
				}

			}
			//如果任何调度超过4个小时 就停止4*60*60*1000
			if((new Date().getTime()-sd.getTime()) > 4*60*60*1000) {
				logger.info("放电检测结束时间"+new Date());
				return;
			}
		}
		
	}

}
