package com.station.moudles.quartz;

import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import org.apache.commons.collections.CollectionUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;

import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.station.common.cache.InitCache;
import com.station.common.cache.PackDataCache;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.mapper.PackDataInfoMapper;
import com.station.moudles.service.GprsConfigInfoService;

public class AutoBalanceTask {
	
	private static final Logger logger = LoggerFactory.getLogger(AutoBalanceTask.class);
	
	private static final int CHECK_INTERVAL = 10;//默认检查的时间间隔。
	private static final int MIN_DISCHARGE_COUNT = 5;//检查时间内，最小放电条数。
	private final static String DISCHARGE_NAME = "放电";
	
	@Autowired
	GprsConfigInfoService gprsConfigInfoService;
	@Autowired
	PackDataInfoMapper packDataInfoMapper;
	
	@Autowired
	PackDataCache packDataCache;
	
	public void execute() {
		Date startTime = InitCache.lastPackDataCheckTime;
		if(startTime == null) {
			startTime = new Date();
			startTime = new Date(startTime.getTime() - CHECK_INTERVAL * 60 * 1000);
		}
		Date endTime = new Date();
		InitCache.lastPackDataCheckTime = endTime;
		//所有的设备
		GprsConfigInfo query = new GprsConfigInfo();
		query.setLinkStatus((byte)1);
		List<GprsConfigInfo> gprsList = gprsConfigInfoService.selectListSelective(query);
		List<String> gprsIds = gprsList.stream().map(GprsConfigInfo::getGprsId).collect(Collectors.toList());
		//查找放电的设备
        Map<String, Object> m = new HashMap<String, Object>();
        m.put("state", DISCHARGE_NAME);
        m.put("startTime", startTime);
        m.put("endTime", endTime);
		List<PackDataInfo> packDatas = packDataInfoMapper.selectListByTime(m);
		Map<String, List<PackDataInfo>> groupedPackData = packDatas.stream().collect(Collectors.groupingBy(PackDataInfo::getGprsId));
		for(String gprsId : groupedPackData.keySet()) {
			List<PackDataInfo> singleDatas = groupedPackData.get(gprsId);
			if(singleDatas.size() >= MIN_DISCHARGE_COUNT ) {
				gprsIds.remove(gprsId);
				singleDatas.sort(Comparator.comparing(PackDataInfo::getId));
				List<PackDataInfo> cachedDatas = packDataCache.get(gprsId, Lists.newArrayList());
				if(cachedDatas.isEmpty()) {
					List<PackDataInfo> forwardDatas = forwardLookup(singleDatas.get(0).getId(), gprsId, 50);
					cachedDatas.addAll(forwardDatas);
				}
				cachedDatas.addAll(singleDatas);
				packDataCache.put(gprsId, cachedDatas);
				//对该电池组进行均衡的逻辑处理。
			}
		}
		//找到之前在缓存中已经存在放电记录的，但是当前时间段内没找到放电记录的设备列表，表示放电结束。
		Map<String, List<PackDataInfo>> alreadyDischarged = packDataCache.getAll(gprsIds);
		for(String gprsId : alreadyDischarged.keySet()) {
			if(!gprsIds.contains(gprsId)) {
				continue;
			}
			List<PackDataInfo> cachedDatas = packDataCache.get(gprsId, Lists.newArrayList());
			List<PackDataInfo> endDischargeDatas = backwardLookup(cachedDatas.get(cachedDatas.size() - 1).getId(), gprsId, 50);
			if(endDischargeDatas.size() < 2) {
				//没有找到连续的非放电数据，不表示放电结束，留待下轮处理。
				continue;
			}
			endDischargeDatas = endDischargeDatas.stream().filter(k -> DISCHARGE_NAME.equals(k.getState())).collect(Collectors.toList());
			cachedDatas.addAll(endDischargeDatas);
			//放电结束的处理逻辑。
		
			//清除缓存数据
			packDataCache.remove(gprsId);
		}
	}
	
	private List<PackDataInfo> forwardLookup(Integer startId, String gprsId, int pageSize) {
		List<PackDataInfo> results = Lists.newArrayList();
		// 向前找非放电态的数据
		int pageNum = 0;
		int count = 0;
		while (true) {
			Map<String, Object> paramMap = Maps.newHashMap();
			paramMap.put("gprsId", gprsId);
			paramMap.put("id", startId);
			paramMap.put("pageNum", pageNum);
			paramMap.put("pageSize", pageSize);
			List<PackDataInfo> items = packDataInfoMapper.getPackDataInfosWhichIdLessThanGivenValue(paramMap);
			if (CollectionUtils.isEmpty(items)) {
				break;
			}
			for (PackDataInfo item : items) {
				results.add(item);
				if (!DISCHARGE_NAME.equals(item.getState())) {
					count++;
				} else {
					count = 0;
				}
				if (count >= 2) {
					break;
				}
			}
			if (count >= 2) {
				break;
			}
			if (items.size() < pageSize) {
				break;
			}
			startId = items.get(items.size() - 1).getId();
			pageNum += pageSize;
		}
		results = results.stream().filter(k -> DISCHARGE_NAME.equals(k.getState())).collect(Collectors.toList());
		return results;
	}
	
	private List<PackDataInfo> backwardLookup(Integer startId, String gprsId, int pageSize) {
		List<PackDataInfo> results = Lists.newArrayList();
		// 向后找连续非放电态的数据
		int pageNum = 0;
		int count = 0;
		while (true) {
			Map<String, Object> paramMap = Maps.newHashMap();
			paramMap.put("gprsId", gprsId);
			paramMap.put("id", startId);
			paramMap.put("pageNum", pageNum);
			paramMap.put("pageSize", pageSize);
			List<PackDataInfo> items = packDataInfoMapper.getPackDataInfosWhichGreaterThanGivenValue(paramMap);
			if (CollectionUtils.isEmpty(items)) {
				break;
			}
			for (PackDataInfo item : items) {
				results.add(item);
				if (!DISCHARGE_NAME.equals(item.getState())) {
					count++;
				} else {
					count = 0;
				}
				if (count >= 2) {
					break;
				}
			}
			if (count >= 2) {
				break;
			}
			if (items.size() < pageSize) {
				break;
			}
			startId = items.get(items.size() - 1).getId();
			pageNum += pageSize;
		}
//		results = results.stream().filter(k -> DISCHARGE_NAME.equals(k.getState())).collect(Collectors.toList());
		return results;
	}
}
