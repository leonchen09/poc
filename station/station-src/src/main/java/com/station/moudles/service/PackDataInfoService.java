package com.station.moudles.service;

import java.util.Date;
import java.util.List;
import java.util.Map;

import com.station.moudles.entity.PackDataInfo;

public interface PackDataInfoService extends BaseService<PackDataInfo, Integer> {

	List<PackDataInfo> selectListByGprsIdTime(String gprsId, Date rcvTime);

	List<Map<String, Object>> getSumVolCur(String gprsId);

	List getDischargeHistory(String gprsId);

	List<Map<String, Object>> getCellVolList(String gprsId, Integer cellIndex);

	List getCellDischargeHistory(String gprsId, Integer cellIndex);
}