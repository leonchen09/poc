package com.station.moudles.entity;

/**
 * Created by Jack on 9/9/2017.
 */
public class InspectSignCellIndex {
    private Integer id;
    private Integer routingInspectionId;
    private Integer cellIndex;

    private Integer stationId;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public Integer getRoutingInspectionId() {
        return routingInspectionId;
    }

    public void setRoutingInspectionId(Integer routingInspectionId) {
        this.routingInspectionId = routingInspectionId;
    }

    public Integer getCellIndex() {
        return cellIndex;
    }

    public void setCellIndex(Integer cellIndex) {
        this.cellIndex = cellIndex;
    }

    public Integer getStationId() {
        return stationId;
    }

    public void setStationId(Integer stationId) {
        this.stationId = stationId;
    }
}
