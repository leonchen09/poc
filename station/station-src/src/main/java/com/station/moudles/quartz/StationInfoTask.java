package com.station.moudles.quartz;

import java.math.BigDecimal;
import java.math.MathContext;
import java.math.RoundingMode;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;

import com.station.common.cache.InitCache;
import com.station.common.utils.ReflectUtil;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataExpandLatest;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.PackDataExpandLatestService;
import com.station.moudles.service.PulseDischargeSendService;
import com.station.moudles.service.StationInfoService;

public class StationInfoTask {
	private static final Logger log = LoggerFactory.getLogger(StationInfoTask.class);
	@Autowired
	PackDataExpandLatestService packDataExpandLatestSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	PulseDischargeSendService pulseDischargeSendSer;

	protected void execute() {
//		Date sd = new Date();
//		log.info("progress StationInfo start");
//		if (InitCache.stationProgressFlag) {
//			log.info("progressing StationInfo end");
//			return;
//		} else {
//			InitCache.stationProgressFlag = true;
//		}
//		List<GprsConfigInfo> configList = gprsConfigInfoSer.selectListSelective(null);
//		Map<String, GprsConfigInfo> configMap = new HashMap<String, GprsConfigInfo>();
//		for (GprsConfigInfo c : configList) {
//			configMap.put(c.getGprsId(), c);
//		}
//		List<PackDataExpandLatest> packDataExpandLatestList = packDataExpandLatestSer.selectListSelective(null);
//		List<StationInfo> stationList = stationInfoSer.selectListSelective(null);
//		log.info("progress StationInfo durationStatus start");
//		Map<String, StationInfo> stationMap = new HashMap<String, StationInfo>();
//		for (StationInfo s : stationList) {
//			stationMap.put(s.getGprsId(), s);
//			String gprsId = s.getGprsId();
//			BigDecimal duration = s.getDuration();
//			if (!gprsId.equals("-1") && duration != null) {
//				//转换为分钟数
//				duration = new BigDecimal(60).multiply(duration);
//				GprsConfigInfo c = configMap.get(gprsId);
//				if (c != null) {
//					List<Object> durationList = ReflectUtil.getValueByStartsWith(c, "durationM");
//					int nullNum = 0;
//					for (Object d : durationList) {
//						if (d == null) {
//							nullNum++;
//						}
//					}
//					if (nullNum == 1) {
//						Integer durationStatus = null;
//						if (duration.compareTo(new BigDecimal(c.getDurationMinExcellent())) >= 0 ) {
//							durationStatus = 1;
//						}else if (duration.compareTo(new BigDecimal(c.getDurationMinGood())) >= 0 && duration.compareTo(new BigDecimal(c.getDurationMaxGood())) < 0) {
////						} else if (s.getDuration() >= c.getDurationMinGood() && s.getDuration() < c.getDurationMaxGood()) {
//							durationStatus = 2;
//						}else if(duration.compareTo(new BigDecimal(c.getDurationMinMedium())) >= 0 && duration.compareTo(new BigDecimal(c.getDurationMaxMedium())) < 0) {
////						} else if (s.getDuration() >= c.getDurationMinMedium() && s.getDuration() < c.getDurationMaxMedium()) {
//							durationStatus = 3;
//						} else {
//							durationStatus = 4;
//						}
//						StationInfo updateStation = new StationInfo();
//						updateStation.setId(s.getId());
//						updateStation.setDurationStatus(durationStatus);
//						stationInfoSer.updateByPrimaryKeySelective(updateStation);
//					}
//				}
//			}
//		}
//		log.info("progress StationInfo durationStatus over");
//		log.info("progress StationInfo Status start");
//		for (PackDataExpandLatest pde : packDataExpandLatestList) {
//			StationInfo stationInfo = new StationInfo();
//			stationInfo.setGprsId(pde.getGprsId());
//			if(pde.getPackDischargeTimePred() != null)
//			{
//				stationInfo.setDuration(pde.getPackDischargeTimePred());
//			}
//			StationInfo oldStation = stationMap.get(pde.getGprsId());
//			if (oldStation != null) {
//				stationInfo.setPackType(oldStation.getPackType());
//			}
//			computeNum(stationInfo, pde, configMap);
//			stationInfoSer.updateByGprsSelective(stationInfo);
//		}
//		log.info("progress StationInfo Status over");
//		InitCache.stationProgressFlag = false;
//		log.info("progress StationInfo over!耗时：" + (new Date().getTime() - sd.getTime()) / 1000 + "s");
	}

	private void computeNum(StationInfo stationInfo, PackDataExpandLatest pde, Map<String, GprsConfigInfo> configMap) {
//		String stationCapStr = stationInfo.getPackType();
//		if (stationCapStr != null) {
//			stationCapStr = stationCapStr.toLowerCase();
//			stationCapStr = stationCapStr.substring(stationCapStr.indexOf("v") + 1, stationCapStr.length() - 2);
//		} else {
//			return;
//		}
//		BigDecimal stationCap = new BigDecimal(stationCapStr);
//		int okNum = 0, poorNum = 0, errorNum = 0;
//		List<Object> cellCapList = ReflectUtil.getValueByStartsWith(pde, "cellCap");
//		GprsConfigInfo c = configMap.get(stationInfo.getGprsId());
//		if (c != null && c.getConsoleCellCapError() != null && c.getConsoleCellCapNormal() != null) {
//			BigDecimal cap = new BigDecimal("0");
//			BigDecimal capError = c.getConsoleCellCapError();
//			BigDecimal capNormal = c.getConsoleCellCapNormal();
//			PackDataExpandLatest updatePde = new PackDataExpandLatest();
//			updatePde.setGprsId(pde.getGprsId());
//			for (int i = 0; i < cellCapList.size(); i++) {
//				Object obj = cellCapList.get(i);
//				if (obj != null) {
//					cap = new BigDecimal(obj.toString());
//				}
//				BigDecimal percent = cap.divide(stationCap, 4, BigDecimal.ROUND_HALF_UP).multiply(new BigDecimal(100), new MathContext(3, RoundingMode.HALF_UP));
//				if (percent.compareTo(capNormal) == 1) {
//					okNum++;
//					ReflectUtil.setValueByKet(updatePde, "cellEvalu" + (i + 1), 0);
//				} else if (percent.compareTo(capError) == -1) {
//					errorNum++;
//					ReflectUtil.setValueByKet(updatePde, "cellEvalu" + (i + 1), 2);
//				} else {
//					poorNum++;
//					ReflectUtil.setValueByKet(updatePde, "cellEvalu" + (i + 1), 1);
//				}
//			}
//			packDataExpandLatestSer.updateByPrimaryKeySelective(updatePde);

		int okNum = 0, poorNum = 0, errorNum = 0;
		List<Object> cellEvaluList = ReflectUtil.getValueByStartsWith(pde, "cellEvalu");
		for (int i = 0; i < cellEvaluList.size(); i++) {
			Object obj = cellEvaluList.get(i);
			int cellStatus = obj == null? 0 : Integer.parseInt(obj.toString());
			if(cellStatus == 0) {
				okNum++;
			}
			else if (cellStatus == 1) {
				poorNum ++;
			}
			else if (cellStatus == 2) {
				errorNum ++;
			}
		}
		if (errorNum > 0) {
			stationInfo.setStatus(2);
		} else if (poorNum > 0) {
			stationInfo.setStatus(1);
		} else {
			stationInfo.setStatus(0);
		}
		stationInfo.setOkNum(okNum);
		stationInfo.setPoorNum(poorNum);
		stationInfo.setErrorNum(errorNum);
		stationInfo.setPackType(null);
	}
}
