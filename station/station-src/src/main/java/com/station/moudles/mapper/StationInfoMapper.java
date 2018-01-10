package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import com.station.moudles.entity.StationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;
import com.station.moudles.vo.search.SearchWarningInfoPagingVo;

public interface StationInfoMapper extends BaseMapper<StationInfo, Integer> {
    int updateByGprsSelective(StationInfo record);

    int updateByCompanyIdSelective(StationInfo record);

    int delByPKs(String ids);

    String selectDb();

    List<Map> selectStationStatus(CommonSearchVo commonSearchVo);

    List<Map> selectStationDurationStatus(CommonSearchVo commonSearchVo);

    List<StationInfo> getStationByCompanyId(Integer companyId);

    List<StationInfo> getStations(Map map);

    List<StationInfo> findAll();
    
    //APP首页页
  	List<StationInfo>  appSelectListSelectivePaging(SearchStationInfoPagingVo searchStationInfoPagingVo);

  	//查询电池组告警信息列表
  	List<StationInfo> appWarnAreaSelectListSelective(SearchWarningInfoPagingVo searchWarningInfoPagingVo);
  	//前台电池出列表
  	List<StationInfo> selectStationInfoList(StationInfo stationIfno);
	
}