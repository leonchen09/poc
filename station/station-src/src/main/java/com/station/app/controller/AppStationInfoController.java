package com.station.app.controller;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.common.utils.CommonConvertUtils;
import com.station.common.utils.ReflectUtil;
import com.station.common.utils.StringUtils;
import com.station.moudles.controller.BaseController;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.CellInfoDetail;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataExpandLatest;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.Parameter;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.RoutingInspectionsStation;
import com.station.moudles.entity.StationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.mapper.PackDataExpandLatestMapper;
import com.station.moudles.mapper.PackDataInfoLatestMapper;
import com.station.moudles.mapper.RoutingInspectionDetailMapper;
import com.station.moudles.mapper.RoutingInspectionsMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.CommonService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.PackDataExpandLatestService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.ParameterService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

import io.swagger.annotations.ApiOperation;
import net.sf.jxls.transformer.CollectionRowTransformer;

@Controller
@RequestMapping(value = "/app/stationInfo")
public class AppStationInfoController extends BaseController {
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	CommonService commonSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	RoutingInspectionsMapper routingInspectionsMapper;
	@Autowired
	RoutingInspectionDetailMapper routingInspectionDetailMapper;
	@Autowired
	PackDataInfoLatestMapper packDataInfoLatestMapper;
	@Autowired
	PackDataExpandLatestMapper packDataExpandLatestMapper;
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	@Autowired
	PackDataExpandLatestService packDataExpandLatestSer;
	@Autowired
	ParameterService parameterSer;
	// 这个接口暂时不用
	@RequestMapping(value = "/detail/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "获取电池组详细信息,支持模糊搜索", notes = "返回列表")
	public AjaxResponse<ShowPage<StationDetail>> getDetailStationInfoList(
			@RequestBody SearchStationInfoPagingVo searchStationInfoPagingVo) {
		List<StationDetail> stationDetailList = new ArrayList<>();
		List<StationInfo> selectListSelectivePaging = stationInfoSer
				.appSelectListSelectivePaging(searchStationInfoPagingVo);
		if (selectListSelectivePaging.size() > 0) {
			for (int i = 0; i < selectListSelectivePaging.size(); i++) {
				StationDetail stationDetail = stationInfoSer
						.getStationDetailBasicByStationId(selectListSelectivePaging.get(i).getId());
				stationDetailList.add(stationDetail);
			}
		}
		ShowPage<StationDetail> page = new ShowPage<StationDetail>(searchStationInfoPagingVo, stationDetailList);
		AjaxResponse<ShowPage<StationDetail>> ajaxResponse = new AjaxResponse<ShowPage<StationDetail>>(page);
		return ajaxResponse;
	}

	/**
	 * 首页
	 * 
	 * @param searchStationInfoPagingVo
	 * @return
	 */
	@RequestMapping(value = "/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "获取电池组信息,支持模糊搜索", notes = "返回列表")
	public AjaxResponse<ShowPage<StationInfo>> getStationInfoList(
			@RequestBody SearchStationInfoPagingVo searchStationInfoPagingVo) {
		List<StationInfo> stationInfoList = stationInfoSer.appSelectListSelectivePaging(searchStationInfoPagingVo);
		ShowPage<StationInfo> page = new ShowPage<StationInfo>(searchStationInfoPagingVo, stationInfoList);
		AjaxResponse<ShowPage<StationInfo>> ajaxResponse = new AjaxResponse<ShowPage<StationInfo>>(page);
		return ajaxResponse;
	}

	/**
	 * app 进入安装维护流程 电池组绑定和首次安装，非首次安装进入维护
	 * 
	 * @param routingInspections
	 * @return
	 */
	@RequestMapping(value = "/routing/entry", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "app 巡检入口", notes = "")
	public AjaxResponse<Object> getBindingStation(@RequestBody RoutingInspectionStationDetail routingInspections) {
		
		if (routingInspections.getStationId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请设置stationId！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "绑定出错！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		StationInfo stationInfo =stationInfoSer.selectByPrimaryKey(routingInspections.getStationId());
		if(stationInfo != null) {
			if (commonSer.completeCompany(stationInfo)) {
				String gprsId = routingInspections.getGprsId();			
				if (gprsId != null && !gprsId.equals("-1")) {
					// InspectStatus == 10 安装中  == 20 维护中 
					if (routingInspections.getInspectStatus() == 10) {
						GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
						updateGprsConfigInfo.setGprsId(gprsId);
						updateGprsConfigInfo.setCompanyId(stationInfo.getCompanyId3());
						// 检查gprs_id_out是否为null，如果是，则设置为gprs_id.
						GprsConfigInfo queryInfo = new GprsConfigInfo();
						queryInfo.setGprsId(updateGprsConfigInfo.getGprsId());
						List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoSer.selectListSelective(queryInfo);
						// 判断是否备用 1 备用
						if (gprsConfigInfos != null && gprsConfigInfos.size() > 0
								&& gprsConfigInfos.get(0).getGprsFlag() != 1) {
							queryInfo = gprsConfigInfos.get(0);
							if (queryInfo.getGprsIdOut() == null || queryInfo.getGprsIdOut().isEmpty()) {
								updateGprsConfigInfo.setGprsIdOut(updateGprsConfigInfo.getGprsId());
							}
							gprsConfigInfoSer.updateByGprsSelective(updateGprsConfigInfo);
							stationInfo.setGprsIdOut(gprsId);
							stationInfo.setGprsId(gprsId); 
							CellInfo updateCellInfo = new CellInfo();
							updateCellInfo.setGprsId(stationInfo.getGprsId());
							updateCellInfo.setStationId(stationInfo.getId());
							cellInfoSer.updateGprsIdByStationId(updateCellInfo);
							stationInfo.setInspectStatus(routingInspections.getInspectStatus());// 改变巡检状态
							stationInfoSer.updateByPrimaryKeySelective(stationInfo);
							// 绑定成功添加记录
							stationInfoSer.bindingAddRoutingInspections(routingInspections);
							ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
							ajaxResponse.setMsg("修改成功！");
						} else {
							ajaxResponse.setMsg("设备是备用设备！");
							return ajaxResponse;
						}
					}else if( routingInspections.getInspectStatus() == 20 ){
						stationInfo.setInspectStatus(routingInspections.getInspectStatus());
						stationInfoSer.updateByPrimaryKeySelective(stationInfo);
						// 维护添加添加记录
						//stationInfoSer.bindingAddRoutingInspections(routingInspections);
						ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
						ajaxResponse.setMsg("修改成功！");
					}
				}else {
					ajaxResponse.setMsg("设备编号未空或者为-1！");
					return ajaxResponse;
				}
			} else {
				ajaxResponse.setMsg("公司不存在！");
			}
		}
		return ajaxResponse;
	}
	
	

	/**
	 * 获取电池单体信息
	 * 
	 * @param gprsId参数
	 * @return
	 */
	@RequestMapping(value = "/cellDetils", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<StationDetail> getCellDetils(@RequestBody StationInfo stationInfo) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR,
				"获取电池单体信息失败");
		if (stationInfo == null) {
			return ajaxResponse;
		}
		StationDetail stationDetail = new StationDetail();
		//判断电池组是否被绑定
		StationInfo station = stationInfoSer.selectByPrimaryKey(stationInfo.getId());
		if(!station.getGprsId().equals(stationInfo.getGprsId())) {
			return ajaxResponse;
		}
		if (StringUtils.getString(stationInfo.getGprsId()) != null && StringUtils.getString(stationInfo.getId()) != null) {
			String gprsId = stationInfo.getGprsId();
			
			PackDataInfoLatest queryPdi = new PackDataInfoLatest();
			queryPdi.setGprsId(gprsId);
			List<PackDataInfoLatest> pdiList = packDataInfoLatestSer.selectListSelective(queryPdi);
			PackDataInfoLatest pdi = null;
			if (pdiList.size() > 0) {
				pdi = pdiList.get(0);
				stationDetail.setState(pdi.getState());
				stationDetail.setGenCur(pdi.getGenCur());
				stationDetail.setGenVol(pdi.getGenVol());
				stationDetail.setEnvironTem(pdi.getEnvironTem());
			} else {
				
				ajaxResponse.setMsg("没有最近一次电池单体信息");
				return ajaxResponse;
			}
			PackDataExpandLatest queryPde = new PackDataExpandLatest();
			queryPde.setGprsId(gprsId);
			List<PackDataExpandLatest> pdeList = packDataExpandLatestSer.selectListSelective(queryPde);
			PackDataExpandLatest pde = null;
			if (pdeList.size() > 0) {
				pde = pdeList.get(0);
			}else {
				ajaxResponse.setMsg("没有最近一次计算后的电池单体信息");
				return ajaxResponse;
			
			}
			
			CellInfo queryCell = new CellInfo();
			queryCell.setStationId(stationInfo.getId());
			List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);
			if(cellList.size() != 0) {
				List<CellInfoDetail> cellDetailList = new ArrayList<CellInfoDetail>();
				for (CellInfo cell : cellList) {
					CellInfoDetail cellDetail = new CellInfoDetail();
					BeanUtils.copyProperties(cell, cellDetail);
					if (pde != null) {
						cellDetail.setCellResist(
								(BigDecimal) ReflectUtil.getValueByKey(pde, "cellResist" + cellDetail.getCellIndex()));
	
						cellDetail.setCellStatus(
								(Integer) ReflectUtil.getValueByKey(pde, "cellEvalu" + cellDetail.getCellIndex()));
						cellDetail.setCellCap(
								(BigDecimal) ReflectUtil.getValueByKey(pde, "cellCap" + cellDetail.getCellIndex()));
					}
					if (pdi != null) {
						cellDetail.setCellVol(
								(BigDecimal) ReflectUtil.getValueByKey(pdi, "cellVol" + cellDetail.getCellIndex()));
						cellDetail.setCellEqu((Byte) ReflectUtil.getValueByKey(pdi, "cellEqu" + cellDetail.getCellIndex()));
						cellDetail.setCellTem(
								(Integer) ReflectUtil.getValueByKey(pdi, "cellTem" + cellDetail.getCellIndex()));
					}
	
					cellDetailList.add(cellDetail);
				}
				sortByIndex(cellDetailList);
				stationDetail.setCellInfoDetailList(cellDetailList);
				
				ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
				ajaxResponse.setData(stationDetail);
				ajaxResponse.setMsg("获取电池单体信息成功");
			}
		}
		return ajaxResponse;
}

	public void sortByIndex(List<CellInfoDetail> cellList) {
		Collections.sort(cellList, new Comparator<CellInfoDetail>() {
			@Override
			public int compare(CellInfoDetail o1, CellInfoDetail o2) {
				return o1.getCellIndex().compareTo(o2.getCellIndex());
			}
		});
	}

	/**
	 * 电池组详情信息
	 * 
	 * @param stationId
	 *            参数
	 * @return
	 */
	@RequestMapping(value = "/stationDetails/{stationId}", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<StationDetail> getStationDetils(@PathVariable Integer stationId) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取电池组信息失败");
		StationDetail stationDetail = new StationDetail();
		if (stationId != null) {
			StationInfo stationIfo = stationInfoSer.selectByPrimaryKey(stationId);
			if (stationIfo != null) {
				BeanUtils.copyProperties(stationIfo, stationDetail);
				String gprsId = stationIfo.getGprsId();
				GprsConfigInfo gprsConfigInfo = new GprsConfigInfo();
				gprsConfigInfo.setGprsId(gprsId);
				List<GprsConfigInfo> gprsInfo = gprsConfigInfoSer.selectListSelective(gprsConfigInfo);
				if(gprsInfo.size() != 0) {
					stationDetail.setDeviceType(gprsInfo.get(0).getDeviceType());
					stationDetail.setLinkStatus(gprsInfo.get(0).getLinkStatus());
				}				
				PackDataExpandLatest packdataExpand = packDataExpandLatestMapper.selectByPrimaryKey(gprsId);
				if (packdataExpand != null) {
					stationDetail.setPackCapPred(packdataExpand.getPackCapPred());// 电池组容量预测
					stationDetail.setPackDischargeTimePred(packdataExpand.getPackDischargeTimePred());// 电池组放点时长预测
				}
				PackDataInfoLatest packData = packDataInfoLatestMapper.selectByPrimaryKey(gprsId);
				if (packData != null) {
						stationDetail.setGenCur(packData.getGenCur());// 总点流
						stationDetail.setGenVol(packData.getGenVol());// 总电压
						stationDetail.setState(packData.getState());// 浮充、放电、充电
						stationDetail.setUpdateTime(packData.getRcvTime());
					
					Parameter parameter = new Parameter();
					//主机异常最高电压
					parameter.setParameterCode("deviceMaxVol");
					List<Parameter> deviceParameterMaxVol = parameterSer.selectListSelective(parameter);
					String maxV = deviceParameterMaxVol.get(0).getParameterValue();
					//主机异常最低电压
					parameter.setParameterCode("deviceMinVol");
					List<Parameter> deviceParameterMinVol = parameterSer.selectListSelective(parameter);
					String minV = deviceParameterMinVol.get(0).getParameterValue();
					BigDecimal bigMaxV = new BigDecimal(maxV);
					BigDecimal bigMinV = new BigDecimal(minV);
					
					//主机异常最高电流
					parameter.setParameterCode("deviceMaxCur");
					List<Parameter> deviceParameterMaxCur = parameterSer.selectListSelective(parameter);
					String maxC = deviceParameterMaxCur.get(0).getParameterValue();
					//主机异常最低电压
					parameter.setParameterCode("deviceMinCur");
					List<Parameter> deviceParameterMinCur = parameterSer.selectListSelective(parameter);
					String minC = deviceParameterMinCur.get(0).getParameterValue();
					BigDecimal bigMaxC = new BigDecimal(maxC);
					BigDecimal bigMinC = new BigDecimal(minC);
					
					 //结果是:-1 小于,0 等于,1 大于
					if( bigMaxV.compareTo(packData.getGenVol()) == 1 && packData.getGenVol().compareTo(bigMinV) == 1 &&
							bigMaxC.compareTo(packData.getGenCur()) == 1 && packData.getGenCur().compareTo(bigMinC) == 1) {
						//电压 电流正常
						stationDetail.setIsNormal(true);
					}else {
						stationDetail.setIsNormal(false);
					}
					
				}
				ajaxResponse.setData(stationDetail);
				ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
				ajaxResponse.setMsg("电池组信息返回成功");
			} else {
				ajaxResponse.setMsg("没有电池组信息");
			}
		} else {
			ajaxResponse.setMsg("没有传递电池组编号");
		}
		return ajaxResponse;

	}

	/**
	 * 提交信息后等待后台确定
	 * 
	 * @param stationId
	 * @return
	 */
	@RequestMapping(value = "/entity/{stationId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "获取station的详情和巡检记录", notes = "返回信息")
	public AjaxResponse<Map<String, Object>> getStationDetailInspectRecord(@PathVariable int stationId) {
		Map<String, Object> info = new HashMap<>();
		AjaxResponse<Map<String, Object>> ajaxResponse = new AjaxResponse<Map<String, Object>>();
		StationDetail stationDetail = stationInfoSer.getStationDetailBasicByStationId(stationId);
		info.put("stationDetail", stationDetail);

		List<RoutingInspectionDetail> recordList = new ArrayList<>();
		RoutingInspections inspections = new RoutingInspections();
		inspections.setStationId(stationId);
		inspections.setGprsId(stationDetail.getGprsId());
		//inspections.setRoutingInspectionStatus(1);
		List<RoutingInspections> routingRecord = routingInspectionsMapper.selectListSelectiveFirst(inspections);
		if (routingRecord.size() > 0) {
			RoutingInspections routingInspections = routingRecord.get(0);
			RoutingInspectionDetail inspectionDetail = new RoutingInspectionDetail();
			inspectionDetail.setRoutingInspectionsId(routingInspections.getRoutingInspectionId());
			List<RoutingInspectionDetail> detailList = routingInspectionDetailMapper
					.selectListSelective(inspectionDetail);
			if(detailList.size() != 0) {
			//得到最新app请求的数据集合
			Integer maxRoutingdetailSeq = detailList.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get().getRequestSeq(); 
			List<RoutingInspectionDetail> newlistSeq = new ArrayList<RoutingInspectionDetail>(); 
			newlistSeq = detailList.stream().filter(k -> k.getRequestSeq() == maxRoutingdetailSeq && k.getRequestType() == 0) .collect(Collectors.toList());			
			//web端返回的最新集合
			Integer maxRoutingDtailType  = detailList.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestType)).get().getRequestType(); 
			List<RoutingInspectionDetail> listType = new ArrayList<RoutingInspectionDetail>(); 
			listType = detailList.stream().filter(k -> k.getRequestType() == maxRoutingDtailType) .collect(Collectors.toList());
			RoutingInspectionDetail routingDeatilListType = null;
			if(listType.size() != 0) {
				 routingDeatilListType = listType.get(listType.size()-1);
			}
			//app请求的所以的数据
			List<RoutingInspectionDetail> appRequestALL = new ArrayList<RoutingInspectionDetail>(); 
			appRequestALL = detailList.stream().filter(k -> k.getRequestType() == 0) .collect(Collectors.toList());
			
			//info.put("inspectionRecord", detailList);
			info.put("newlistSeq", newlistSeq);
			info.put("listType",routingDeatilListType);
			info.put("appRequestALL", appRequestALL);
			}else{
				info.put("newlistSeq", null);
				info.put("listType", null);
				info.put("appRequestALL",null);
			}
		}
		ajaxResponse.setData(info);
		return ajaxResponse;
	}
}
