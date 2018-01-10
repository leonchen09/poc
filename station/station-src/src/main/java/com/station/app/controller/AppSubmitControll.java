package com.station.app.controller;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.Date;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.controller.BaseController;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.ModifyGprsidSend;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.ModifyGprsidSendService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.vo.AjaxResponse;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping(value = "/app/submit")
public class AppSubmitControll extends BaseController {

	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	SubDeviceService SubDeviceSer;
	@Autowired
	ModifyGprsidSendService modifyGprsidSendSer;

	/**
	 * app提交信息保存在routing_inspections表和routiing_inspection_deteail表
	 */
	@RequestMapping(value = "/saveInfo", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "保存app提交的信息", notes = "保存app提交的信息")
	public AjaxResponse<RoutingInspectionStationDetail> saveRoutingInspection(@RequestBody RoutingInspectionStationDetail routingInspectionStationDetail) {
		if (routingInspectionStationDetail == null) {
			return new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_ERROR, "请选择提交项！");
		}
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse =  new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_SUCCESS, "提交成功！");
		//改变电池组的状态 安装、维护流程，电池组状态。
		//99:未安装，30:已安装，31，在线；32，离线；10:安装中，11:安装中等待确认状态，12:安装中后台确认未完成状态，20:维护中，21:维护中等待确认状态，22:维护中后台确认未完成状态
		StationInfo station =new StationInfo();
		StationInfo stationinfo = stationInfoSer.selectByPrimaryKey(routingInspectionStationDetail.getStationId());
		if(stationinfo != null) {
			station.setInspectStatus(routingInspectionStationDetail.getInspectStatus());
			station.setId(routingInspectionStationDetail.getStationId());
			stationInfoSer.updateByPrimaryKeySelective(station);
		}
		RoutingInspections routingInspections = new RoutingInspections();		
		routingInspections.setStationId(routingInspectionStationDetail.getStationId());
		routingInspections.setGprsId(routingInspectionStationDetail.getGprsId());
		routingInspections.setRoutingInspectionStatus(1);//设备维护中
		// 通过电池组的状态在维护中 ，电池组id,设备id来判断是否有这条数据 如果没有就新增
		List<RoutingInspections> routingList = routingInspectionsSer.selectListSelective(routingInspections);
//		boolean flag=true;
//		for (RoutingInspections ListStatus : routingList) {
//			// 如果电池组是安装维护中或者是失败状态就不在主表中插入数据
//			if(ListStatus.getRoutingInspectionStatus() != null) {
//				if(ListStatus.getRoutingInspectionStatus() ==1 || ListStatus.getRoutingInspectionStatus() == 3) {
//					routingInspections.setRoutingInspectionStatus(ListStatus.getRoutingInspectionStatus());
//					flag=false;
//				}
//			}
//		}
		if (routingList.size() == 0) {
			Date date = new Date();
			routingInspections.setOperateTime(date);
			routingInspections.setRoutingInspectionStatus(1);
			routingInspections.setOperateType(routingInspectionStationDetail.getOperateType());
			routingInspections.setOperateId(routingInspectionStationDetail.getOperateId());
			routingInspections.setOperateName(routingInspectionStationDetail.getOperateName());
			routingInspections.setOperatePhone(routingInspectionStationDetail.getOperatePhone());
			routingInspections.setDeviceType(routingInspectionStationDetail.getDeviceType());
			routingInspectionsSer.insertSelective(routingInspections);
		} 
			// 查询刚插入的这条数据得到id后--新增详情记录
		Integer routingId = null;
		if(routingList.size() == 0) {
			routingId = routingInspections.getRoutingInspectionId();
		} else {
			routingId = routingList.get(0).getRoutingInspectionId();
		}
			if (routingId != null) {
				RoutingInspectionDetail routingInspectionDetail = new RoutingInspectionDetail();
				routingInspectionDetail.setRoutingInspectionsId(routingId);
				routingInspectionDetail.setRequestType(0);
				List<RoutingInspectionDetail> routingInspectionDetailList = routingInspectionStationDetail.getRoutingInspectionDetailList();
				if(routingInspectionDetailList.size() == 0) {
					ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
					ajaxResponse.setMsg("没有单体异常情况，等待后台确认！");
					return ajaxResponse;
				}				
				// 查询出提交的最大次数
				List<RoutingInspectionDetail> routingDetail = routingInspectionDetailSer.selectListSelective(routingInspectionDetail);
				if (routingDetail.size() != 0) {
					//RoutingInspectionDetail InspectionDetail = routingDetail.get(routingDetail.size() - 1);
					RoutingInspectionDetail InspectionDetail= routingDetail.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
					// 循环新增详情
					for (RoutingInspectionDetail detailList : routingInspectionDetailList) {
						if (InspectionDetail.getRequestSeq() == null) {
							detailList.setRequestSeq(1);
						}else {
							detailList.setRequestSeq(InspectionDetail.getRequestSeq() + 1);
						}
						//判断operateType == 4 更换从机
						if(detailList.getDetailOperateType() == 4) {
							 AjaxResponse<RoutingInspectionStationDetail> updateSubDevice = updateSubDevice(routingInspectionStationDetail,detailList);
							 if(updateSubDevice.getCode().equals("0001")) {
								 return updateSubDevice;
							 }
						}
						detailList.setRoutingInspectionsId(routingId);
						detailList.setDetailOperateId(routingInspectionStationDetail.getOperateId());
						detailList.setDetailOperateName(routingInspectionStationDetail.getOperateName());
						routingInspectionDetailSer.insertSelective(detailList);					
						//更新单体信息 标记单体在确定完成后
						//cellInfoSer.appUpdateCellInfo(routingInspectionStationDetail,detailList,null);
					}
				} else { 
					for (RoutingInspectionDetail detailList : routingInspectionDetailList) {
						//判断operateType == 4 更换从机
						if(detailList.getDetailOperateType() == 4) {
							AjaxResponse<RoutingInspectionStationDetail> updateSubDevice = updateSubDevice(routingInspectionStationDetail,detailList);
						 	if(updateSubDevice.getCode().equals("0001")) {
						 		return updateSubDevice;
						 	}
						}
						detailList.setRequestSeq(1);
						detailList.setRoutingInspectionsId(routingId);
						detailList.setDetailOperateId(routingInspectionStationDetail.getOperateId());
						detailList.setDetailOperateName(routingInspectionStationDetail.getOperateName());
						
						routingInspectionDetailSer.insertSelective(detailList);
						//更新单体信息 单体标记另外在确定完成后标记
						//cellInfoSer.appUpdateCellInfo(routingInspectionStationDetail,detailList,null);
					}
				}
			} else {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("提交失败！");
			}
		
		return ajaxResponse;
	}
	
	/*
	 * 当DetailOperateType() == 4 的时候调用修改从机
	 */
	public AjaxResponse<RoutingInspectionStationDetail> updateSubDevice(RoutingInspectionStationDetail routingInspectionStationDetail,RoutingInspectionDetail detailList ){
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse = new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_SUCCESS,"修改成功");
		//根据gprsid 和cellIndex 查找出old sub_device_id_out
		SubDevice subDevice = new SubDevice();
		subDevice.setGprsId(routingInspectionStationDetail.getGprsId());
		subDevice.setCellSort(detailList.getCellIndex());
		subDevice.setSubFlag(0);//配套
		List<SubDevice> subDeviceList = SubDeviceSer.selectListSelective(subDevice);
		if(subDeviceList.size() != 0) {
		detailList.setDetailOperateValueOld(subDeviceList.get(0).getSubDeviceIdOut());
		//判断是否是备用从机
		SubDevice queryStandby = new SubDevice();
		queryStandby.setSubDeviceId(detailList.getDetailOperateValueNew());
		queryStandby.setSubFlag(1);							
		List<SubDevice> isStandby = SubDeviceSer.selectListSelective(queryStandby);
		if(isStandby.size() != 0) {
			//发送指令
			ModifyGprsidSend send = new ModifyGprsidSend();
			send.setGprsId(routingInspectionStationDetail.getGprsId());
			send.setInnerId(subDeviceList.get(0).getSubDeviceIdOut());
			send.setOuterId(detailList.getDetailOperateValueNew());
			send.setType(2);// 跟换从机
			modifyGprsidSendSer.changeDeviceId(send, 0);
		}else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("跟换的从机不是备用从机！");
			return ajaxResponse;
		}
	
		}else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("没有对应的从机！");
			return ajaxResponse;
		}
	return ajaxResponse;
		
	}
}