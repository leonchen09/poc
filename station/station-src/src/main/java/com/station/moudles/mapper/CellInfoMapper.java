package com.station.moudles.mapper;

import java.util.List;
import java.util.Map;

import com.station.moudles.entity.CellInfo;

public interface CellInfoMapper extends BaseMapper<CellInfo, Integer> {
    int updateSendDoneByGprs(List<String> gprsList);

    List<CellInfo> selectWaitForPuls();

    int updateGprsIdByStationId(CellInfo cellInfo);

    int updateSendDoneByGprsCellIndex(Map m);

    List<CellInfo> getLatestByGprsId(String gprsId);
}