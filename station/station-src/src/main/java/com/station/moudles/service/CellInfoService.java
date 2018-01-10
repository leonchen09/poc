package com.station.moudles.service;

import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;

import java.util.List;

public interface CellInfoService extends BaseService<CellInfo, Integer> {

    void updateSendDoneByGprs(List<String> gprsList);

    List<CellInfo> selectWaitForPuls();

    void updateGprsIdByStationId(CellInfo cellInfo);

    void updateSendDoneByGprsCellIndex(String gprsId, Integer cellIndex, Integer pulseSendDone);

    void exportUpdateCellInfo(RoutingInspectionStationDetail routingInspectionStationDetail,RoutingInspectionDetail detailList,String cellPlant);
   
    void appUpdateCellInfo(RoutingInspectionStationDetail routingInspectionStationDetail,RoutingInspectionDetail detailList,String cellPlant,boolean isInsert);

}