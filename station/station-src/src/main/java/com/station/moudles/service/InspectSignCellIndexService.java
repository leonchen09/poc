package com.station.moudles.service;

import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.RoutingInspectionStationDetail;

public interface InspectSignCellIndexService  extends BaseService<InspectSignCellIndex, Integer>{


	void addinspectSignCellIndex(RoutingInspectionStationDetail stationDetail);
}
