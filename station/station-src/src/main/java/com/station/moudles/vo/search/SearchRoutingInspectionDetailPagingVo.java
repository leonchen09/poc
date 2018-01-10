package com.station.moudles.vo.search;

import com.fasterxml.jackson.annotation.JsonFormat;
import io.swagger.annotations.ApiModel;
import io.swagger.annotations.ApiModelProperty;
import java.util.Date;

/**
 * This class was generated by Bill Generator.
 * This class corresponds to the database table routing_inspection_detail  
 *
 * @zdmgenerated 2017-13-19 07:13
 */
@ApiModel(value="RoutingInspectionDetail查询",description="RoutingInspectionDetail查询描述")
public class SearchRoutingInspectionDetailPagingVo extends PageEntity {
    /**
     * This field corresponds to the database column routing_inspection_detail.routing_inspection_detail_id  
     */
    @ApiModelProperty(value="pk",required=false)
    private Integer routingInspectionDetailId;

    /**
     * This field corresponds to the database column routing_inspection_detail.routing_inspections_id  
     */
    @ApiModelProperty(value="routingInspectionsId",required=false)
    private Integer routingInspectionsId;

    /**
     * This field corresponds to the database column routing_inspection_detail.create_time  创建时间
     */
    @JsonFormat(pattern="yyyy-MM-dd HH:mm:ss",timezone = "GMT+8")
    @ApiModelProperty(value="创建时间")
    private Date createTime;

    /**
     * This field corresponds to the database column routing_inspection_detail.detail_operate_id  
     */
    @ApiModelProperty(value="detailOperateId",required=false)
    private Integer detailOperateId;

    /**
     * This field corresponds to the database column routing_inspection_detail.detail_operate_name  
     */
    @ApiModelProperty(value="detailOperateName",example="detailOperateName",required=false)
    private String detailOperateName;

    /**
     * This field corresponds to the database column routing_inspection_detail.detail_operator_type  1巡检人员，2确认人
     */
    @ApiModelProperty(value="1巡检人员，2确认人",required=false)
    private Integer detailOperatorType;

    /**
     * This field corresponds to the database column routing_inspection_detail.comment  
     */
    @ApiModelProperty(value="comment",example="comment",required=false)
    private String comment;

    /**
     * This field corresponds to the database column routing_inspection_detail.detail_operate_type  1安装主机，2更换主机，3安装霍尔感应器，4更换从机，5更换单体_电池类型,6更换单体_电池品牌
     */
    @ApiModelProperty(value="1安装主机，2更换主机，3安装霍尔感应器，4更换从机，5更换单体_电池类型,6更换单体_电池品牌",example="1安装主机，2更换主机，3安装霍尔感应器，4更换从机，5更换单体_电池类型,6更换单体_电池品牌",required=false)
    private String detailOperateType;

    /**
     * This field corresponds to the database column routing_inspection_detail.detail_operate_value_old  如有多个以,分隔
     */
    @ApiModelProperty(value="如有多个以,分隔",example="如有多个以,分隔",required=false)
    private String detailOperateValueOld;

    /**
     * This field corresponds to the database column routing_inspection_detail.detail_operate_value_new  如有多个以,分隔
     */
    @ApiModelProperty(value="如有多个以,分隔",example="如有多个以,分隔",required=false)
    private String detailOperateValueNew;

    /**
     * This field corresponds to the database column routing_inspection_detail.remark  备注
     */
    @ApiModelProperty(value="备注",example="备注",required=false)
    private String remark;

    /**
     * This field corresponds to the database column routing_inspection_detail.cell_index  编号
     */
    @ApiModelProperty(value="编号",required=false)
    private Integer cellIndex;

    /**
     * This method returns the value of the database column routing_inspection_detail.routing_inspection_detail_id  
     * @return the value of routing_inspection_detail.routing_inspection_detail_id
     */
    public Integer getRoutingInspectionDetailId() {
        return routingInspectionDetailId;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.routing_inspection_detail_id  
     * @param routingInspectionDetailId the value for routing_inspection_detail.routing_inspection_detail_id
     */
    public void setRoutingInspectionDetailId(Integer routingInspectionDetailId) {
        this.routingInspectionDetailId = routingInspectionDetailId;
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.routing_inspections_id  
     * @return the value of routing_inspection_detail.routing_inspections_id
     */
    public Integer getRoutingInspectionsId() {
        return routingInspectionsId;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.routing_inspections_id  
     * @param routingInspectionsId the value for routing_inspection_detail.routing_inspections_id
     */
    public void setRoutingInspectionsId(Integer routingInspectionsId) {
        this.routingInspectionsId = routingInspectionsId;
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.create_time  创建时间
     * @return the value of routing_inspection_detail.create_time
     */
    public Date getCreateTime() {
        return createTime;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.create_time  创建时间
     * @param createTime the value for routing_inspection_detail.create_time
     */
    public void setCreateTime(Date createTime) {
        this.createTime = createTime;
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.detail_operate_id  
     * @return the value of routing_inspection_detail.detail_operate_id
     */
    public Integer getDetailOperateId() {
        return detailOperateId;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.detail_operate_id  
     * @param detailOperateId the value for routing_inspection_detail.detail_operate_id
     */
    public void setDetailOperateId(Integer detailOperateId) {
        this.detailOperateId = detailOperateId;
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.detail_operate_name  
     * @return the value of routing_inspection_detail.detail_operate_name
     */
    public String getDetailOperateName() {
        return detailOperateName;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.detail_operate_name  
     * @param detailOperateName the value for routing_inspection_detail.detail_operate_name
     */
    public void setDetailOperateName(String detailOperateName) {
        this.detailOperateName = detailOperateName == null ? null : detailOperateName.trim();
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.detail_operator_type  1巡检人员，2确认人
     * @return the value of routing_inspection_detail.detail_operator_type
     */
    public Integer getDetailOperatorType() {
        return detailOperatorType;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.detail_operator_type  1巡检人员，2确认人
     * @param detailOperatorType the value for routing_inspection_detail.detail_operator_type
     */
    public void setDetailOperatorType(Integer detailOperatorType) {
        this.detailOperatorType = detailOperatorType;
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.comment  
     * @return the value of routing_inspection_detail.comment
     */
    public String getComment() {
        return comment;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.comment  
     * @param comment the value for routing_inspection_detail.comment
     */
    public void setComment(String comment) {
        this.comment = comment == null ? null : comment.trim();
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.detail_operate_type  1安装主机，2更换主机，3安装霍尔感应器，4更换从机，5更换单体_电池类型,6更换单体_电池品牌
     * @return the value of routing_inspection_detail.detail_operate_type
     */
    public String getDetailOperateType() {
        return detailOperateType;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.detail_operate_type  1安装主机，2更换主机，3安装霍尔感应器，4更换从机，5更换单体_电池类型,6更换单体_电池品牌
     * @param detailOperateType the value for routing_inspection_detail.detail_operate_type
     */
    public void setDetailOperateType(String detailOperateType) {
        this.detailOperateType = detailOperateType == null ? null : detailOperateType.trim();
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.detail_operate_value_old  如有多个以,分隔
     * @return the value of routing_inspection_detail.detail_operate_value_old
     */
    public String getDetailOperateValueOld() {
        return detailOperateValueOld;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.detail_operate_value_old  如有多个以,分隔
     * @param detailOperateValueOld the value for routing_inspection_detail.detail_operate_value_old
     */
    public void setDetailOperateValueOld(String detailOperateValueOld) {
        this.detailOperateValueOld = detailOperateValueOld == null ? null : detailOperateValueOld.trim();
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.detail_operate_value_new  如有多个以,分隔
     * @return the value of routing_inspection_detail.detail_operate_value_new
     */
    public String getDetailOperateValueNew() {
        return detailOperateValueNew;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.detail_operate_value_new  如有多个以,分隔
     * @param detailOperateValueNew the value for routing_inspection_detail.detail_operate_value_new
     */
    public void setDetailOperateValueNew(String detailOperateValueNew) {
        this.detailOperateValueNew = detailOperateValueNew == null ? null : detailOperateValueNew.trim();
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.remark  备注
     * @return the value of routing_inspection_detail.remark
     */
    public String getRemark() {
        return remark;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.remark  备注
     * @param remark the value for routing_inspection_detail.remark
     */
    public void setRemark(String remark) {
        this.remark = remark == null ? null : remark.trim();
    }

    /**
     * This method returns the value of the database column routing_inspection_detail.cell_index  编号
     * @return the value of routing_inspection_detail.cell_index
     */
    public Integer getCellIndex() {
        return cellIndex;
    }

    /**
     * This method sets the value of the database column routing_inspection_detail.cell_index  编号
     * @param cellIndex the value for routing_inspection_detail.cell_index
     */
    public void setCellIndex(Integer cellIndex) {
        this.cellIndex = cellIndex;
    }
}