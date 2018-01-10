package com.station.moudles.service;

import java.util.List;

import com.station.moudles.entity.StationWarningInfo;
import com.station.moudles.entity.WarnArea;
import com.station.moudles.entity.WarningInfo;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.report.WarnReport;

public interface WarningInfoService extends BaseService<WarningInfo, Integer> {

	List<StationWarningInfo> selectWarningList(CommonSearchVo commonSearch);

	String exportToExcel(List<StationWarningInfo> stationWarnList , WarnReport warnReport);

	List<WarnArea> selectWarnAreaList(CommonSearchVo commonSearchVo);

	String exportWarnAreaToExcel(List<WarnArea> warnAreaList, WarnReport warnReport);


}