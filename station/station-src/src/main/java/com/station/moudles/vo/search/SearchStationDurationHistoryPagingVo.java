package com.station.moudles.vo.search;

import com.fasterxml.jackson.annotation.JsonFormat;
import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import java.util.Date;

/**
 * This class was generated by Bill Generator.
 * This class corresponds to the database table station_duration_history  放电时长趋势图数据
 *
 * @zdmgenerated 2017-13-19 07:13
 */
@ApiModel(value="放电时长趋势图数据查询",description="放电时长趋势图数据查询描述")
public class SearchStationDurationHistoryPagingVo extends PageEntity {
    /**
     * This field corresponds to the database column station_duration_history.id  
     */
    @ApiModelProperty(value="pk",required=false)
    private Integer id;

    /**
     * This field corresponds to the database column station_duration_history.gprs_id  
     */
    @ApiModelProperty(value="gprsId",example="gprsId",required=false)
    private String gprsId;

    /**
     * This field corresponds to the database column station_duration_history.duration  预测时长，单位分钟
     */
    @ApiModelProperty(value="预测时长，单位分钟",required=false)
    private Integer duration;

    /**
     * This field corresponds to the database column station_duration_history.evaluate_time  评估时间，为每个月1号
     */
    @JsonFormat(pattern="yyyy-MM-dd HH:mm:ss",timezone = "GMT+8")
    @ApiModelProperty(value="评估时间，为每个月1号")
    private Date evaluateTime;

    /**
     * This method returns the value of the database column station_duration_history.id  
     * @return the value of station_duration_history.id
     */
    public Integer getId() {
        return id;
    }

    /**
     * This method sets the value of the database column station_duration_history.id  
     * @param id the value for station_duration_history.id
     */
    public void setId(Integer id) {
        this.id = id;
    }

    /**
     * This method returns the value of the database column station_duration_history.gprs_id  
     * @return the value of station_duration_history.gprs_id
     */
    public String getGprsId() {
        return gprsId;
    }

    /**
     * This method sets the value of the database column station_duration_history.gprs_id  
     * @param gprsId the value for station_duration_history.gprs_id
     */
    public void setGprsId(String gprsId) {
        this.gprsId = gprsId == null ? null : gprsId.trim();
    }

    /**
     * This method returns the value of the database column station_duration_history.duration  预测时长，单位分钟
     * @return the value of station_duration_history.duration
     */
    public Integer getDuration() {
        return duration;
    }

    /**
     * This method sets the value of the database column station_duration_history.duration  预测时长，单位分钟
     * @param duration the value for station_duration_history.duration
     */
    public void setDuration(Integer duration) {
        this.duration = duration;
    }

    /**
     * This method returns the value of the database column station_duration_history.evaluate_time  评估时间，为每个月1号
     * @return the value of station_duration_history.evaluate_time
     */
    public Date getEvaluateTime() {
        return evaluateTime;
    }

    /**
     * This method sets the value of the database column station_duration_history.evaluate_time  评估时间，为每个月1号
     * @param evaluateTime the value for station_duration_history.evaluate_time
     */
    public void setEvaluateTime(Date evaluateTime) {
        this.evaluateTime = evaluateTime;
    }
}