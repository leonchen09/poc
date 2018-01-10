package com.station.moudles.mapper;

import java.util.List;

import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.GprsConfigInfoDetail;
import com.station.moudles.entity.GprsConfigInfoStation;
import com.station.moudles.vo.search.PageEntity;

public interface GprsConfigInfoMapper extends BaseMapper<GprsConfigInfo, Integer> {

    int updateByCompanyId(GprsConfigInfo gprsConfigInfo);

    GprsConfigInfoDetail selectDetailById(Integer id);

    int updateByGprsSelective(GprsConfigInfo gprsConfigInfo);

    List<GprsConfigInfoStation> selectStationListSelectivePaging(PageEntity pageEntity);

    List selectUnbindGprsList(String gprsId);
    
    //----ADD
    List<String> selectBindGprsList(GprsConfigInfo gprsConfigInfo);
    
    List<GprsConfigInfo> selectByGprsIds(Iterable<String> gprsIds);
    
    //查询 rcv_time 不等于null 的数据
  	List<GprsConfigInfo> selectRcvTimeNotNull(GprsConfigInfo gprsConfigInfo);
}