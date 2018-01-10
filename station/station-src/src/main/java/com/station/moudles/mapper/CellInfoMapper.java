package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import org.apache.ibatis.annotations.Param;

import com.station.moudles.entity.CellInfo;

public interface CellInfoMapper extends BaseMapper<CellInfo, Integer> {
    int updateSendDoneByGprs(List<String> gprsList);

    List<CellInfo> selectWaitForPuls();

    int updateGprsIdByStationId(CellInfo cellInfo);

    int updateSendDoneByGprsCellIndex(Map m);

    List<CellInfo> getLatestByGprsId(String gprsId);
    //删除多余的电池单体
    void deleteMoreCell(@Param("map")Map map);
}