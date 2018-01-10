package com.station.app.controller;

import java.util.List;

import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.controller.BaseController;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.ModifyGprsidSend;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.StationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.ModifyGprsidSendService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.vo.AjaxResponse;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping(value = "/app/gprsConfigInfo")
public class AppGprsConfigInfoController extends BaseController {
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	ModifyGprsidSendService modifyGprsidSendSer;
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	//暂时不调用
	@RequestMapping(value = "/entity/{gprsOutId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "app扫描设备得到设备信息", notes = "得到设备信息")
	public AjaxResponse<StationDetail> getEntity(@PathVariable String gprsOutId) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
		queryGprsConfigInfo.setGprsIdOut(gprsOutId);
		List<GprsConfigInfo> gprsConfigInfoList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
		if (gprsConfigInfoList.size() > 0) {
			// 查询base_station_info 表得到 巡检状态
			StationInfo queryStationInfo = new StationInfo();
			queryStationInfo.setGprsIdOut(gprsOutId);
			List<StationInfo> stationInfoList = stationInfoSer.selectListSelective(queryStationInfo);
			StationDetail stationDetail = null;
			if (stationInfoList.size() > 0) {
				// 主机绑定过电池组
				stationDetail = stationInfoSer.getStationDetailBasicByStationId(stationInfoList.get(0).getId());
			} else {
				// 首次安装
				stationDetail = stationInfoSer.getStationDetailBasicByGprsId(gprsOutId);
			}
			ajaxResponse.setData(stationDetail);
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("获取成功！");
		} else {
			// 数据库没有主机信息
			ajaxResponse.setData(null);
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("没有该主机信息！");
		}
		return ajaxResponse;
	}
	
	
	/**
	 * 扫描设备
	 * @param gprsOutId
	 * @return
	 */
	@RequestMapping(value = "/scan/{gprsOutId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "app扫描设备得到设备信息", notes = "得到设备信息")
	public AjaxResponse<StationDetail> getGprsConfigInfo(@PathVariable String gprsOutId) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
		queryGprsConfigInfo.setGprsIdOut(gprsOutId);
		List<GprsConfigInfo> gprsConfigInfoList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
		if (gprsConfigInfoList.size() > 0) {
			StationDetail stationDetail = new StationDetail();
			GprsConfigInfo gprsConfigInfo = gprsConfigInfoList.get(0);
			BeanUtils.copyProperties(gprsConfigInfo, stationDetail);
			stationDetail.setId(null);
			//stationDetail.setDeviceType(gprsConfigInfo.getDeviceType());
			//stationDetail.setGprsFlag(gprsConfigInfo.getGprsFlag());
			// 查询base_station_info 表得到 巡检状态
			StationInfo queryStationInfo = new StationInfo();
			queryStationInfo.setGprsIdOut(gprsOutId);
			List<StationInfo> stationInfoList = stationInfoSer.selectListSelective(queryStationInfo);
			if (stationInfoList.size() > 0) {
				StationInfo stationInfo = stationInfoList.get(0);
				BeanUtils.copyProperties(stationInfo, stationDetail);
				PackDataInfoLatest packDateInfo = packDataInfoLatestSer.selectByPrimaryKey(gprsConfigInfo.getGprsId());
				if(packDateInfo != null) {
					stationDetail.setState(packDateInfo.getState());
				}
			}else {
				stationDetail.setGprsId(gprsConfigInfo.getGprsId());
				stationDetail.setGprsFlag(gprsConfigInfo.getGprsFlag());
				stationDetail.setInspectStatus(99);//未安装
			}
			
			ajaxResponse.setData(stationDetail);
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("获取成功！");
		} else {
			// 数据库没有主机信息
			ajaxResponse.setData(null);
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("没有该主机信息！");
		}
		return ajaxResponse;
	}
	
	
	
	/**
	 * APP端修改主机Id
	 * 传递的参数有stationid gprsId（内部id） newgprsIdOut ,operateId,operatename
	 * 
	 */
	@RequestMapping(value = "/updateGprsId", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据电池组id更新主机id", notes = "根据电池组id更新主机id")
	public AjaxResponse<RoutingInspectionStationDetail> updteGprsConfigId(@RequestBody RoutingInspectionStationDetail routingInspectionStationDetail) {
		if (routingInspectionStationDetail == null) {
			return new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_ERROR, "无效的电池组id！");
		}
		AjaxResponse<RoutingInspectionStationDetail> ajaxResponse = new AjaxResponse<RoutingInspectionStationDetail>(Constant.RS_CODE_SUCCESS, "修改成功！");
		try {
			//判断是否备用
			GprsConfigInfo query = new GprsConfigInfo();
			query.setGprsId(routingInspectionStationDetail.getNewGprsIdOut());
			query.setGprsFlag(1);
			List<GprsConfigInfo> gprsList = gprsConfigInfoSer.selectListSelective(query);
			if(gprsList.size() == 0) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("新设备不是备用设备！");
				return ajaxResponse;
			}
			//发送
			ModifyGprsidSend send = new ModifyGprsidSend();
			send.setGprsId(routingInspectionStationDetail.getNewGprsIdOut());
			send.setInnerId(routingInspectionStationDetail.getGprsId());
			send.setOuterId(routingInspectionStationDetail.getNewGprsIdOut());
			send.setType(1);//修改主机
			 modifyGprsidSendSer.changeDeviceId(send,0);
			// 修改主机id
			gprsConfigInfoSer.updateGprsIdApp(routingInspectionStationDetail);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg(e.getMessage());
		}
		return ajaxResponse;

	}
}
