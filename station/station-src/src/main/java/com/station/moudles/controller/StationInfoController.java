package com.station.moudles.controller;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.io.FileUtils;
import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.common.utils.BeanValueUtils;
import com.station.common.utils.CommonConvertUtils;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.ReflectUtil;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.Company;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataExpandLatest;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationDetail;
import com.station.moudles.entity.StationDurationHistory;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.CommonService;
import com.station.moudles.service.CompanyService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.GprsDeviceTypeService;
import com.station.moudles.service.PackDataExpandLatestService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.PackDataInfoService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationDurationHistoryService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

import io.swagger.annotations.ApiImplicitParam;
import io.swagger.annotations.ApiImplicitParams;
import io.swagger.annotations.ApiOperation;

/**
 * This class was generated by Bill Generator. This class corresponds to the
 * database table base_station_info
 *
 * @zdmgenerated 2017-28-19 02:28
 */
@Controller
@RequestMapping(value = "/stationInfo")
public class StationInfoController extends BaseController {
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	CommonService commonSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	PackDataInfoService packDataInfoSer;
	@Autowired
	StationDurationHistoryService stationDurationHistorySer;
	@Autowired
	PackDataExpandLatestService packDataExpandLatestSer;
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	@Autowired
	CompanyService companySer;
	
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	GprsDeviceTypeService gprsDeviceTypeSer;

	private final ExecutorService fixedThreadPool = Executors.newSingleThreadExecutor();

	private static final Logger logger = LoggerFactory.getLogger(StationInfoController.class);

	@RequestMapping(value = "/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取列表", notes = "返回列表")
	public AjaxResponse<List<StationInfo>> getStationInfoList(@RequestBody StationInfo queryStationInfo) {
		List<StationInfo> stationInfoList = stationInfoSer.selectListSelective(queryStationInfo);
		AjaxResponse<List<StationInfo>> ajaxResponse = new AjaxResponse<List<StationInfo>>(stationInfoList);
		return ajaxResponse;
	}
	// 前台页面的电池组列表
	@RequestMapping(value = "/stationList", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取列表", notes = "返回列表")
	public AjaxResponse<List<StationInfo>> getStationList(@RequestBody StationInfo queryStationInfo) {
		List<StationInfo> stationInfoList = stationInfoSer.selectStationInfoList(queryStationInfo);
		AjaxResponse<List<StationInfo>> ajaxResponse = new AjaxResponse<List<StationInfo>>(stationInfoList);
		return ajaxResponse;
	}

	@RequestMapping(value = "/listPage", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取列表分页", notes = "返回列表分页")
	public AjaxResponse<ShowPage<StationInfo>> getStationInfoListPage(@RequestBody CommonSearchVo commonSearchVo) {
		SearchStationInfoPagingVo searchStationInfoPagingVo = new SearchStationInfoPagingVo();
		CommonConvertUtils.convertCommonToStationPage(commonSearchVo, searchStationInfoPagingVo);
		List<StationInfo> stationInfoList = stationInfoSer.selectListSelectivePaging(searchStationInfoPagingVo);
		ShowPage<StationInfo> page = new ShowPage<StationInfo>(searchStationInfoPagingVo, stationInfoList);
		AjaxResponse<ShowPage<StationInfo>> ajaxResponse = new AjaxResponse<ShowPage<StationInfo>>(page);
		return ajaxResponse;
	}

	@RequestMapping(value = "/save", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "新增", notes = "新增")
	@ApiImplicitParams(value = { @ApiImplicitParam(required = false, name = "id", paramType = "query") })
	public AjaxResponse<StationInfo> save(@RequestBody StationInfo stationInfo) {
		stationInfo.setId(null);
		stationInfo.setDelFlag(0);
		stationInfo.setGprsIdOut(stationInfo.getGprsId());
		AjaxResponse<StationInfo> ajaxResponse = new AjaxResponse<StationInfo>(Constant.RS_CODE_ERROR, "添加出错！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		String gprsId = stationInfo.getGprsId();
		if (commonSer.completeCompany(stationInfo)) {
			// 校验经纬度
			BigDecimal lat = stationInfo.getLat();
			BigDecimal lng = stationInfo.getLng();
			if (lat.abs().compareTo(new BigDecimal(90d)) == 1 || lng.abs().compareTo(new BigDecimal(180d)) == 1) {
				ajaxResponse.setMsg("经纬度非法！请正确填写！");
				return ajaxResponse;
			}
			// 校验电池组类型
			String packType = stationInfo.getPackType();
			if (packType != null) {
				packType = packType.toUpperCase();
				stationInfo.setPackType(packType);
				if (!packType.matches("^[1-9]\\d*[V][1-9]\\d*AH$")) {
					ajaxResponse.setMsg("电池组类型非法！请正确填写！");
					return ajaxResponse;
				}
			}
			// 负载功率等级
			BigDecimal loadPower = stationInfo.getLoadPower();
			if (loadPower != null) {
				String strLoad = loadPower.toString();
				boolean matches = strLoad.matches("[0-9]*\\.?[0-9]*");
				if (!matches) {
					ajaxResponse.setMsg("负载功率等级必须是数字组成！");
					return ajaxResponse;
				}
			}
			
			if (!StringUtils.isNull(gprsId) && !gprsId.equals("-1")) {
				// 判断电压平台一致性问题
				try {
					stationInfoSer.judgeCellVolLevel(stationInfo);
				} catch (Exception e) {
					ajaxResponse.setMsg(e.getMessage());
					return ajaxResponse;
				}
				GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
				updateGprsConfigInfo.setGprsId(stationInfo.getGprsId());
				updateGprsConfigInfo.setCompanyId(stationInfo.getCompanyId3());
				gprsConfigInfoSer.updateByGprsSelective(updateGprsConfigInfo);
				stationInfo.setInspectStatus(30);//已经绑定设比为已安装
				stationInfo.setGprsIdOut(stationInfo.getGprsId());
			}
			try {
				stationInfoSer.createStationCells(stationInfo);
			} catch (Exception e) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg(e.getMessage());
				return ajaxResponse;
			}
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("添加成功！");
		} else {
			ajaxResponse.setMsg("公司不存在！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/delAll", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk集合删除", notes = "根据pk集合删除，以','为分隔符")
	@ApiImplicitParams(value = {
	@ApiImplicitParam(required = true, name = "ids", value = "pk集合", paramType = "query", allowMultiple = true, dataType = "Integer")})
	public AjaxResponse<Object> delAll(Integer[] ids) {
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请选择删除项！");
		if (ids == null || ids.length < 0) {
			return ajaxResponse;
		}
		ajaxResponse.setMsg("删除失败!");
		request.setAttribute("ajaxResponse", ajaxResponse);
		stationInfoSer.deleteByPKs(ids);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("删除成功!");
		return ajaxResponse;
	}

	@RequestMapping(value = "/update", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk更新", notes = "根据pk更新，属性为null的不更新")
	public AjaxResponse<Object> update(@RequestBody StationInfo stationInfo) {
		if (stationInfo.getId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请设置pk！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "修改出错！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		if (commonSer.completeCompany(stationInfo)) {

			// 校验经纬度
			BigDecimal lat = stationInfo.getLat();
			BigDecimal lng = stationInfo.getLng();
			if (lat != null && lng != null) {
				if (lat.abs().compareTo(new BigDecimal(90d)) == 1 || lng.abs().compareTo(new BigDecimal(180d)) == 1) {
					ajaxResponse.setMsg("经纬度非法！请正确填写！");
					return ajaxResponse;
				}
			}
			// 校验电池组类型
			String packType = stationInfo.getPackType();
			if (packType != null) {
				packType = packType.toUpperCase();
				stationInfo.setPackType(packType);
				if (!packType.matches("^[1-9]\\d*V[1-9]\\d*AH$")) {
					ajaxResponse.setMsg("电池组类型非法！请正确填写！");
					return ajaxResponse;
				}
			}else {
				ajaxResponse.setMsg("电池组类型不能为空！");
				return ajaxResponse;
			}
			// -----add 10/31 调整在统一个service 中调用
			try {
				stationInfoSer.updateGprsAndCellAndStation(stationInfo);
			} catch (Exception e) {
				ajaxResponse.setMsg(e.getMessage());
				return ajaxResponse;
			}

			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg("修改成功！");
		} else {
			ajaxResponse.setMsg("公司不存在！");
		}
		return ajaxResponse;
	}

	// 删除
	@RequestMapping(value = "/delete", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<Object> delete(@RequestBody StationInfo stationInfo) {
		if (stationInfo.getId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请设置pk！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_SUCCESS, "删除成功！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		// 判断是否解除绑定
		StationInfo station = stationInfoSer.selectByPrimaryKey(stationInfo.getId());
		if (station != null && station.getGprsId().equals("-1") && stationInfo.getDelFlag() == 1
				&& station.getGprsIdOut().equals("-1")) {
			stationInfoSer.updateByPrimaryKeySelective(stationInfo);
		} else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("请先解除绑定！");
			return ajaxResponse;
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/entity/{id}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取", notes = "根据pk获取")
	public AjaxResponse<StationInfo> getEntity(@PathVariable Integer id) {
		if (id == null) {
			return new AjaxResponse<StationInfo>(Constant.RS_CODE_ERROR, "请选择pk！");
		}
		StationInfo stationInfo = stationInfoSer.selectByPrimaryKey(id);
		AjaxResponse<StationInfo> ajaxResponse = new AjaxResponse<StationInfo>(Constant.RS_CODE_SUCCESS, "获取成功！");
		if (stationInfo != null) {
			ajaxResponse.setData(stationInfo);
		} else {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("获取失败！");
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/entity/detail/{id}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取详情", notes = "根据pk获取详情")
	public AjaxResponse<StationDetail> getStationDetail(@PathVariable Integer id) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		StationDetail stationDetail = stationInfoSer.getStationDetailByStationId(id);
		ajaxResponse.setData(stationDetail);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取成功！");
		return ajaxResponse;
	}

	/**
	 * 以前通过一个借口展示所以电池组的信息
	 * @param id
	 * @return
	 */
	@RequestMapping(value = "/entity/detail/basic/{id}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取详情", notes = "根据pk获取详情")
	public AjaxResponse<StationDetail> getStationDetailBasic(@PathVariable Integer id) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		StationDetail stationDetail = stationInfoSer.getStationDetailBasicByStationId(id);
		ajaxResponse.setData(stationDetail);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取成功！");
		return ajaxResponse;
	}
	/**
	 * 展示不经常改变的信息
	 * @param id
	 * @return
	 */
	@RequestMapping(value = "/entity/detail/basicInfo/{id}", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<StationDetail> getStationDetailBasicInfo(@PathVariable Integer id) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		StationDetail stationDetail = stationInfoSer.getStationDetailBasicInfoByStationId(id);
		ajaxResponse.setData(stationDetail);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取成功！");
		return ajaxResponse;
	}
	/**
	 * 显示需要刷新的数据
	 * @param id
	 * @return
	 */
	@RequestMapping(value = "/entity/detail/packInfo/{gprsId}", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<StationDetail> getStationDetailpackInfo(@PathVariable String gprsId) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		StationDetail stationDetail = stationInfoSer.getStationDetailpackInfoByStationId(gprsId);
		ajaxResponse.setData(stationDetail);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取成功！");
		return ajaxResponse;
	}
	
	
	// ---------------- add -----------------
	@RequestMapping(value = "/entity/detail/info/{gprsId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据pk获取详情", notes = "根据pk获取详情")
	public AjaxResponse<StationDetail> getStationDetailInfo(@PathVariable String gprsId) {
		AjaxResponse<StationDetail> ajaxResponse = new AjaxResponse<StationDetail>(Constant.RS_CODE_ERROR, "获取失败！");
		request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
		StationDetail stationDetail = stationInfoSer.getStationDetailBasicByGprsId(gprsId);
		ajaxResponse.setData(stationDetail);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取成功！");
		return ajaxResponse;
	}

	@Override
	public boolean parseFile(File file, AjaxResponse ajaxResponse, Integer companyId)
			throws EncryptedDocumentException, FileNotFoundException, InvalidFormatException, IOException {
	
		return stationInfoSer.parseStationExcelFile(file, ajaxResponse, companyId);

	}
	
	@RequestMapping(value = "/toUpload", method = RequestMethod.GET)
	public String toFileUpload() {
		return "testFileImport";
	}

	@RequestMapping(value = "/pulseDischarge", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件发送特征测试", notes = "根据条件发送特征测试")
	public AjaxResponse pulseDischarge(@RequestBody CommonSearchVo commonSearchVo) {
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_SUCCESS, "特征测试命令接收成功");
		if (commonSearchVo.getGprsList() == null && commonSearchVo.getCompanyId() == null) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("参数设置错误！");
		} else {
			stationInfoSer.pulseDischargeAsync(commonSearchVo);
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/sumVolCur/{gprsId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "获取电池组总电压电流", notes = "返回列表")
	public AjaxResponse<List<Map<String, Object>>> getSumVolCurList(@PathVariable String gprsId) {
		List<Map<String, Object>> sumVolCurList = packDataInfoSer.getSumVolCur(gprsId);
		AjaxResponse<List<Map<String, Object>>> ajaxResponse = new AjaxResponse<List<Map<String, Object>>>(
				sumVolCurList);
		logger.debug("sumVolCurList size=" + sumVolCurList.size());
		return ajaxResponse;
	}

	@ResponseBody
	@ApiOperation(value = "获取GPRS站点历史放电记录")
	@RequestMapping(value = "/dischargeHistory/{gprsId}", method = RequestMethod.POST)
	public AjaxResponse getDischargeHistory(@PathVariable String gprsId) {
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_ERROR, "获取GPRS站点历史放电记录失败！");
		List resultList = packDataInfoSer.getDischargeHistory(gprsId);
		ajaxResponse.setData(resultList);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取GPRS站点历史放电记录成功！");
		return ajaxResponse;
	}

	@ResponseBody
	@ApiOperation(value = "获取GPRS站点预测时长记录")
	@RequestMapping(value = "/durationHistory/{gprsId}", method = RequestMethod.POST)
	public AjaxResponse getDurationHistory(@PathVariable String gprsId) {
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_ERROR, "获取GPRS站点预测时长记录失败！");
		StationDurationHistory sdh = new StationDurationHistory();
		sdh.setGprsId(gprsId);
		// 以6个月前的1号0点0分0秒开始查询
		sdh.setEvaluateTime(MyDateUtils.getFirstDayDiffMonth(new Date(), -6));
		List<StationDurationHistory> resultList = stationDurationHistorySer.selectListSelective(sdh);
		//add 添加单体整治信息
		RoutingInspections  routing  = new RoutingInspections();
		routing.setGprsId(gprsId);
		routing.setOperateType(3);//更好单体
		Date  date = new Date();//当前时间
		routing.setOperateTime(date);
		
		// 以6个月前的1号0点0分0秒开始查询
		routing.setEndTime(MyDateUtils.getFirstDayDiffMonth(date, -6));
		//整治信息
		List<RoutingInspections> routingList = routingInspectionsSer.selectCellInspace(routing);
		//装载整治信息的集合
		List<StationDurationHistory> resultList2 = new ArrayList<StationDurationHistory>();
		if(routingList.size() != 0) {
			for(int i=0;i<routingList.size();i++) {
				StationDurationHistory SDH = new StationDurationHistory();
				Date opTime =routingList.get(i).getOperateTime();
				SDH.setEvaluateTime(opTime);
				SDH.setType(1);
				resultList2.add(SDH);
			}	
		}
		
		if(resultList2.size() != 0) {
			resultList.addAll(resultList2);
		}
		ajaxResponse.setData(resultList);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取GPRS站点预测时长记录成功！");
		return ajaxResponse;
	}

	@ResponseBody
	@ApiOperation(value = "获取GPRS单体详情")
	@RequestMapping(value = "/gprs/cell", method = RequestMethod.POST)
	public AjaxResponse getDischargeCellDetail(@RequestBody CellInfo cellInfo) {
		String gprsId = cellInfo.getGprsId();
		Integer cellIndex = cellInfo.getCellIndex();
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_ERROR, "获取GPRS单体详情失败！");
		Map<String, Object> m = new HashMap<String, Object>();
		StationInfo queryStationInfo = new StationInfo();
		queryStationInfo.setGprsId(gprsId);
		List<StationInfo> stationInfoList = stationInfoSer.selectListSelective(queryStationInfo);
		if (stationInfoList.size() > 0) {
			StationInfo stationInfo = stationInfoList.get(0);
			m.put("name", stationInfo.getName());
		}
		CellInfo queryCell = new CellInfo();
		queryCell.setGprsId(gprsId);
		queryCell.setCellIndex(cellIndex);
		List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);
		CellInfo cell = cellList.get(0);
		m.put("useFrom", cell.getUseFrom());
		m.put("remainTime", cell.getRemainTime());
		m.put("diffTimeStr", MyDateUtils.diffDays(new Date(), cellList.get(0).getUseFrom()));
		PackDataInfoLatest packData = packDataInfoLatestSer.selectByPrimaryKey(gprsId);
		// mv.addObject("packData", packData);
		PackDataExpandLatest packDataExpand = packDataExpandLatestSer.selectByPrimaryKey(gprsId);
		if (packData != null) {
			m.put("cellVol", ReflectUtil.getValueByKey(packData, "cellVol" + cellIndex));
			m.put("cellTem", ReflectUtil.getValueByKey(packData, "cellTem" + cellIndex));
		} else {
			m.put("cellVol", null);
			m.put("cellTem", null);
		}
		if (packDataExpand != null) {
			m.put("cellResist", ReflectUtil.getValueByKey(packDataExpand, "cellResist" + cellIndex));
			m.put("cellCap", ReflectUtil.getValueByKey(packDataExpand, "cellCap" + cellIndex));
		} else {
			m.put("cellResist", null);
			m.put("cellCap", null);
		}
		GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
		queryGprsConfigInfo.setGprsId(gprsId);
		List<GprsConfigInfo> configList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
		GprsConfigInfo c = null;
		if (configList.size() > 0) {
			c = configList.get(0);
		}
		if (c != null && c.getConsoleCellCapError() != null && c.getConsoleCellCapNormal() != null) {
			BigDecimal capError = c.getConsoleCellCapError();
			BigDecimal capNormal = c.getConsoleCellCapNormal();
			BigDecimal cellCap = (BigDecimal) ReflectUtil.getValueByKey(packDataExpand,
					"cellCap" + cellInfo.getCellIndex());
			Integer cellStatus = null;
			if (cellCap != null) {
				if (cellCap.compareTo(capNormal) == 1) {
					cellStatus = 0;
				} else if (cellCap.compareTo(capError) == -1) {
					cellStatus = 1;
				} else {
					cellStatus = 2;
				}
			}
			m.put("cellStatus", cellStatus);
		} else {
			m.put("cellStatus", null);
		}
		ajaxResponse.setData(m);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取GPRS单体详情成功！");
		return ajaxResponse;
	}

	@RequestMapping(value = "/cell/vol", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "获取电池组总电压电流", notes = "返回列表")
	public AjaxResponse<List<Map<String, Object>>> getCellVolList(@RequestBody CellInfo cellInfo) {
		List<Map<String, Object>> cellVolList = packDataInfoSer.getCellVolList(cellInfo.getGprsId(),
				cellInfo.getCellIndex());
		AjaxResponse<List<Map<String, Object>>> ajaxResponse = new AjaxResponse<List<Map<String, Object>>>(cellVolList);
		logger.debug("cellVolList size=" + cellVolList.size());
		return ajaxResponse;
	}

	@ResponseBody
	@ApiOperation(value = "获取GPRS电池单体历史放电记录")
	@RequestMapping(value = "/cell/dischargeHistory", method = RequestMethod.POST)
	public AjaxResponse getCellDischargeHistory(@RequestBody CellInfo cellInfo) {
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_ERROR, "获取GPRS站点历史放电记录失败！");
		List resultList = packDataInfoSer.getCellDischargeHistory(cellInfo.getGprsId(), cellInfo.getCellIndex());
		ajaxResponse.setData(resultList);
		ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
		ajaxResponse.setMsg("获取GPRS电池单体历史放电记录成功！");
		return ajaxResponse;
	}

	@RequestMapping(value = "/pulseDischargeCell/{id}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件发送特征测试", notes = "根据条件发送特征测试")
	public AjaxResponse pulseDischargeCell(@PathVariable Integer id) {
		AjaxResponse ajaxResponse = new AjaxResponse(Constant.RS_CODE_SUCCESS, "单体特征测试命令接收成功");
		CellInfo cell = new CellInfo();
		cell.setId(id);
		cell.setPulseSendDone(0);
		cellInfoSer.updateByPrimaryKeySelective(cell);
		return ajaxResponse;
	}

	@ResponseBody
	@RequestMapping(value = "/exportStationCheck/{stationId}", method = RequestMethod.GET)
	public AjaxResponse downloadStationCheck(@PathVariable Integer stationId) throws IOException {
		AjaxResponse ajaxResponse = new AjaxResponse();

		StationDetail sd = stationInfoSer.getStationDetailByStationId(stationId);
		String filePath = stationInfoSer.exportStationCheckToExcel(sd);
		File file = new File(filePath);
		String fileName = new String((sd.getGprsId() + "电池组性能及容量检测报告.xlsx").getBytes("UTF-8"), "iso-8859-1");
		// 设置response
		response.setContentType("application/x-msdownload; charset=UTF-8");
		response.addHeader("content-type", "application/x-msdownload;");
		response.addHeader("content-disposition", "attachment; filename=" + fileName);
		response.setContentLength((int) file.length());
		// 输出
		InputStream in = new FileInputStream(file);
		OutputStream os = response.getOutputStream();
		try {
			int length = 0;
			byte[] buffer = new byte[1024];
			while ((length = in.read(buffer)) != -1) {
				os.write(buffer, 0, length);
			}
			os.flush();
		} finally {
			if (in != null) {
				in.close();
			}
			FileUtils.deleteQuietly(file);
		}

		return ajaxResponse;
	}

	private String stationCheck(Integer stationId) {
		StationDetail sd = stationInfoSer.getStationDetailByStationId(stationId);
		String filePath = stationInfoSer.exportStationCheckToExcel(sd);
		return filePath;
	}

	@PostMapping(value = "/exportStationCheck/all/{company_id}")
	@ResponseBody
	public AjaxResponse exportStationCheckAll(@PathVariable("company_id") Integer companyId) {
		if (companyId == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "三级公司编号必须!");
		}
		StationInfo info = new StationInfo();
		info.setCompanyId3(companyId);
		List<StationInfo> stationInfos = stationInfoSer.selectListSelective(info);
		if (CollectionUtils.isEmpty(stationInfos)) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "没有任何基站!");
		}
		logger.info("{}个电池组进行性能检查报告导出", stationInfos.size());
		fixedThreadPool.execute(() -> {
			for (StationInfo stationInfo : stationInfos) {
				stationCheck(stationInfo.getId());
			}
		});
		return new AjaxResponse<>(Constant.RS_CODE_SUCCESS, "提交性能检查报告成功！");
	}

	@RequestMapping(value = "/stationStatusList", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取列表", notes = "返回列表")
	public AjaxResponse getStationStatusList(@RequestBody CommonSearchVo commonSearchVo) {
		List<StationInfo> stationInfoList = stationInfoSer.getStationStatusList(commonSearchVo);
		AjaxResponse ajaxResponse = new AjaxResponse(stationInfoList);
		return ajaxResponse;
	}

	@ResponseBody
	@RequestMapping(value = "/exportStationStatus", method = RequestMethod.GET)
	public AjaxResponse downloadStationStatus(Integer companyId, Integer companyLevel) throws IOException {
		AjaxResponse ajaxResponse = new AjaxResponse();
		CommonSearchVo commonSearchVo = new CommonSearchVo();
		commonSearchVo.setCompanyId(companyId);
		commonSearchVo.setCompanyLevel(companyLevel);

		String filePath = stationInfoSer.exportStationStatusToExcel(companyId, companyLevel);
		File file = new File(filePath);
		Company c = companySer.selectByPrimaryKey(companyId);
		String fileName = new String((c.getCompanyName() + "蓄电池性能统计表.xlsx").getBytes("UTF-8"), "iso-8859-1");
		// 设置response
		response.setContentType("application/x-msdownload; charset=UTF-8");
		response.addHeader("content-type", "application/x-msdownload;");
		response.addHeader("content-disposition", "attachment; filename=" + fileName);
		response.setContentLength((int) file.length());
		// 输出
		InputStream in = new FileInputStream(file);
		OutputStream os = response.getOutputStream();
		try {
			int length = 0;
			byte[] buffer = new byte[1024];
			while ((length = in.read(buffer)) != -1) {
				os.write(buffer, 0, length);
			}
			os.flush();
		} finally {
			if (in != null) {
				in.close();
			}
			FileUtils.deleteQuietly(file);
		}

		return ajaxResponse;
	}

	@RequestMapping(value = "/stationDurationStatusList", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取列表", notes = "返回列表")
	public AjaxResponse getStationDurationStatusList(@RequestBody CommonSearchVo commonSearchVo) {
		List<StationInfo> stationInfoList = stationInfoSer.getStationDurationStatusList(commonSearchVo);
		AjaxResponse ajaxResponse = new AjaxResponse(stationInfoList);
		return ajaxResponse;
	}

	@RequestMapping(value = "/gendata")
	@ResponseBody
	public AjaxResponse genVol(String gprsid) {
		Calendar cal = Calendar.getInstance();
		cal.setTime(new Date());
		cal.add(Calendar.DATE, -3);
		Date startDate = new Date(cal.getTimeInMillis());
		List<PackDataInfo> infos = packDataInfoSer.selectListByGprsIdTime(gprsid, startDate);
		for (PackDataInfo info : infos) {
			double sumVol = 0;
			for (int i = 1; i < 25; i++) {
				double num = (double) (Math.random()) / 10;
				num += 2.214;
				sumVol += num;
				BigDecimal d = new BigDecimal(num);
				d = d.setScale(3, BigDecimal.ROUND_HALF_UP);
				String name = "cellVol" + i;
				BeanValueUtils.bindProperty(name, d, info);
			}
			BigDecimal d = new BigDecimal(sumVol).setScale(3, BigDecimal.ROUND_HALF_UP);
			BeanValueUtils.bindProperty("genVol", d, info);
			packDataInfoSer.updateByPrimaryKey(info);
		}
		PackDataInfoLatest query = new PackDataInfoLatest();
		query.setGprsId(gprsid);
		PackDataInfoLatest datal = packDataInfoLatestSer.selectListSelective(query).get(0);
		double sumVol = 0;
		for (int i = 1; i < 25; i++) {
			double num = (double) (Math.random()) / 10;
			num += 2.214;
			sumVol += num;
			BigDecimal d = new BigDecimal(num);
			d = d.setScale(3, BigDecimal.ROUND_HALF_UP);
			String name = "cellVol" + i;
			BeanValueUtils.bindProperty(name, d, datal);
		}
		BigDecimal d = new BigDecimal(sumVol).setScale(3, BigDecimal.ROUND_HALF_UP);
		BeanValueUtils.bindProperty("genVol", d, datal);
		packDataInfoLatestSer.updateByPrimaryKey(datal);
		return new AjaxResponse(Constant.RS_CODE_SUCCESS, "成功");
	}

}