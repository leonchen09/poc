package com.station.app.controller;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.common.utils.ReflectUtil;
import com.station.moudles.controller.BaseController;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.Parameter;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.ModifyGprsidSendService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.ParameterService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.vo.AjaxResponse;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping(value = "/app/subdevice")
public class AppSubdeviceController extends BaseController {
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	@Autowired
	SubDeviceService subDeviceSer;
	@Autowired
	ModifyGprsidSendService modifyGprsidSendSer;
	@Autowired
	ParameterService paramterSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;


	@RequestMapping(value = "/subDeviceAndCellInfo/{gprsId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据gprsid得到从机信息(目前只能得到电压)", notes = "返回数据")
	public AjaxResponse<Map> getSubdeviceInfo(@PathVariable String gprsId) {
		AjaxResponse<Map> ajaxResponse = new AjaxResponse<Map>(Constant.RS_CODE_ERROR, "获取失败！");
		PackDataInfoLatest queryPdi = new PackDataInfoLatest();
		queryPdi.setGprsId(gprsId);
		//查询出从机电压的范围
		Parameter  par = new Parameter();
		par.setParameterCode("subMaxVol");
		List<Parameter> subMaxVol = paramterSer.selectListSelective(par);
		String maxVol = subMaxVol.get(0).getParameterValue();
		BigDecimal maxV = new BigDecimal(maxVol);
		par.setParameterCode("subMinVol");
		List<Parameter> subMinVol = paramterSer.selectListSelective(par);
		String minVol = subMinVol.get(0).getParameterValue();
		BigDecimal minV = new BigDecimal(minVol);
		//查询单体的电压范围
		par.setParameterCode("chargeMaxVol");
		List<Parameter> cellMaxVol = paramterSer.selectListSelective(par);
		String cellMaxV = cellMaxVol.get(0).getParameterValue();
		par.setParameterCode("chargeMinVol");
		List<Parameter> cellMixVol = paramterSer.selectListSelective(par);
		String cellMixV = cellMixVol.get(0).getParameterValue();
		//查询出主机是否在线
		GprsConfigInfo gprsConfigInfo = new GprsConfigInfo();
		gprsConfigInfo.setGprsId(gprsId);
		List<GprsConfigInfo> gprsList = gprsConfigInfoSer.selectListSelective(gprsConfigInfo);

		//查询出从机的电压
		List<PackDataInfoLatest> pdiList = packDataInfoLatestSer.selectListSelective(queryPdi);
		PackDataInfoLatest pdi = null;
		Map<String,Object> map = new HashMap<String,Object>();
		if(gprsList.size() != 0) {
			map.put("linkStatus", gprsList.get(0).getLinkStatus());
		}
		map.put("minVol", cellMixV);
		map.put("maxVol", cellMaxV);
		List<Map> volList = new ArrayList<>();
		if (pdiList.size() > 0) {
			pdi = pdiList.get(0);
			for (int i = 1; i < 25; i++) {
				Map<String,Object> entity = new HashMap<>();
				entity.put("isNormal", true);
				Object obj = ReflectUtil.getValueByKey(pdi, "cellVol" + i);
				String cellVol = obj == null ? "0.000" : obj.toString();
				entity.put("vol", cellVol);
				//判断是否再正常电压范围内
				BigDecimal vol = new BigDecimal(cellVol);
				if(vol.compareTo(maxV) == 1 || minV.compareTo(vol) == 1) {
					entity.put("isNormal", false);
				}
				volList.add(entity);
			}
		}else {
			for (int i = 1; i < 25; i++) {
				Map<String,Object> entity = new HashMap<>();
				entity.put("vol", null);
				entity.put("isNormal", false);
				volList.add(entity);
			}
		}
		map.put("vols", volList);
		ajaxResponse.setData(map);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取成功！");
		return ajaxResponse;
	}
	/**
	 * 修改从机id  参数 station_id、 gprs_id、 operate_type、 old、 new 、OperateId、operateName
	 * @param appSubdevice
	 * @return
	 */
	@RequestMapping(value = "/updateSubDeviceId", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk更新从机设备", notes = "根据pk更新从机设备")
	public AjaxResponse<Object> updateSubDeviceId(@RequestBody RoutingInspectionStationDetail routingInspectionStationDetail) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "修改从机设备出错！");
		try {
			subDeviceSer.updateSubDevice(routingInspectionStationDetail,0);
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("修改从机指令发送成功！");
			
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg(e.getMessage());
		}
		return ajaxResponse;
	}

}
