package com.station.moudles.service.impl;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.math.BigDecimal;
import java.math.MathContext;
import java.math.RoundingMode;
import java.net.URLDecoder;
import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.TreeMap;
import java.util.stream.Collectors;

import org.apache.commons.collections.CollectionUtils;
import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.google.common.collect.Maps;
import com.station.common.Constant;
import com.station.common.utils.BeanValueUtils;
import com.station.common.utils.CommonConvertUtils;
import com.station.common.utils.JxlsUtil;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.ReflectUtil;
import com.station.common.utils.RowParseHelper;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.CellInfoDetail;
import com.station.moudles.entity.GprsBalanceSend;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.GprsDeviceType;
import com.station.moudles.entity.PackDataExpandLatest;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.PulseCalculationSend;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationDetail;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.mapper.CellInfoMapper;
import com.station.moudles.mapper.GprsDeviceTypeMapper;
import com.station.moudles.mapper.PackDataInfoMapper;
import com.station.moudles.mapper.PulseCalculationSendMapper;
import com.station.moudles.mapper.StationInfoMapper;
import com.station.moudles.service.CachService;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.CommonService;
import com.station.moudles.service.CompanyService;
import com.station.moudles.service.GprsBalanceSendService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.GprsDeviceTypeService;
import com.station.moudles.service.PackDataExpandLatestService;
import com.station.moudles.service.PackDataInfoLatestService;
import com.station.moudles.service.PackDataInfoService;
import com.station.moudles.service.PulseDischargeSendService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;
import com.station.moudles.vo.search.SearchWarningInfoPagingVo;
import net.sf.jxls.exception.ParsePropertyException;
import net.sf.jxls.transformer.XLSTransformer;

@Service
public class StationInfoServiceImpl extends BaseServiceImpl<StationInfo, Integer> implements StationInfoService {
	// 导入电池组标志判断
	private static boolean stationFile = false;

	@Autowired
	StationInfoMapper stationInfoMapper;
	@Autowired
	CachService cachSer;
	@Autowired
	PackDataInfoLatestService packDataInfoLatestSer;
	@Autowired
	PackDataExpandLatestService packDataExpandLatestSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	CommonService commonSer;
	@Autowired
	SubDeviceService subDeviceSer;
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;
	@Autowired
	PackDataInfoService packDataInfoSer;
	@Autowired
	CompanyService companySer;
	@Autowired
	GprsBalanceSendService gprsBalanceSendService;
	@Autowired
	PulseDischargeSendService pulseDischargeSendService;
	@Autowired
	PulseCalculationSendMapper pulseCalculationSendMapper;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	PackDataInfoMapper packDataInfoMapper;
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	GprsDeviceTypeService gprsDeviceTypeSer;
	@Autowired
	CellInfoMapper cellInfoMapper;
	@Autowired
	GprsDeviceTypeMapper gprsDeviceTypeMapper;

	private static final Logger logger = LoggerFactory.getLogger(StationInfoServiceImpl.class);

	@Override
	public StationDetail getStationDetailByStationId(Integer stationId) {
		StationInfo stationInfo = selectByPrimaryKey(stationId);
		StationDetail stationDetail = new StationDetail();
		BeanUtils.copyProperties(stationInfo, stationDetail);
		String gprsId = stationDetail.getGprsId();

		PackDataExpandLatest pde = null;

		if (!gprsId.equals("-1")) {
			GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
			queryGprsConfigInfo.setGprsId(stationInfo.getGprsId());
			List<GprsConfigInfo> configList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
			GprsConfigInfo c = null;
			if (configList.size() > 0) {
				c = configList.get(0);
				stationDetail.setLinkStatus(c.getLinkStatus());
				stationDetail.setDeviceType(c.getDeviceType());
				stationDetail.setDevicePhone(c.getDevicePhone());

			}
			// boolean flag = false;
			// if (c != null && c.getConsoleCellCapError() != null &&
			// c.getConsoleCellCapNormal() != null) {
			// flag = true;
			// }
			PackDataInfoLatest queryPdi = new PackDataInfoLatest();
			queryPdi.setGprsId(gprsId);
			List<PackDataInfoLatest> pdiList = packDataInfoLatestSer.selectListSelective(queryPdi);
			PackDataInfoLatest pdi = null;
			if (pdiList.size() > 0) {
				pdi = pdiList.get(0);
				stationDetail.setUpdateTime(pdi.getRcvTime());
				stationDetail.setState(pdi.getState());
				stationDetail.setGenCur(pdi.getGenCur());
				stationDetail.setGenVol(pdi.getGenVol());
				stationDetail.setEnvironTem(pdi.getEnvironTem());
				stationDetail.setSoc(pdi.getSoc());
			}
			PackDataExpandLatest queryPde = new PackDataExpandLatest();
			queryPde.setGprsId(gprsId);
			List<PackDataExpandLatest> pdeList = packDataExpandLatestSer.selectListSelective(queryPde);

			if (pdeList.size() > 0) {
				pde = pdeList.get(0);
				stationDetail.setPackCapPred(pde.getPackCapPred());
				stationDetail.setPackDischargeTimePred(pde.getPackDischargeTimePred());
			}
			CellInfo queryCell = new CellInfo();
			queryCell.setStationId(stationDetail.getId());
			List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);
			SubDevice querySubDevice = new SubDevice();
			querySubDevice.setGprsId(gprsId);
			List<SubDevice> subDeviceList = subDeviceSer.selectListSelective(querySubDevice);
			Map<Integer, SubDevice> subDeviceMap = new HashMap<Integer, SubDevice>();
			for (SubDevice subDevice : subDeviceList) {
				subDeviceMap.put(subDevice.getCellSort(), subDevice);
			}
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
				SubDevice subDevice = subDeviceMap.get(cell.getCellIndex());
				if (subDevice != null) {
					cellDetail.setSubDeviceId(subDevice.getSubDeviceId());
					cellDetail.setSubDeviceIdOut(subDevice.getSubDeviceIdOut());
				}
				cellDetailList.add(cellDetail);
			}
			// cellcapindex没有使用，屏蔽这段代码。
			// sortByCap(cellDetailList);
			// if (null != pde) {
			// for (CellInfoDetail cellInfoDetail : cellDetailList) {
			// cellInfoDetail.setCellCapIndex(
			// (Integer) BeanValueUtils.getValue("cellCapSort" +
			// cellInfoDetail.getCellIndex(), pde));
			// }
			// }
			sortByIndex(cellDetailList);
			stationDetail.setCellInfoDetailList(cellDetailList);
		}

		Map<String, Object> paramMap = Maps.newHashMap();
		paramMap.put("gprsId", gprsId);
		paramMap.put("state", "浮充");
		List<PackDataInfo> pdList = packDataInfoMapper.selectByOrderSelective(paramMap);
		PackDataInfo startPdi = null;
		// PackDataInfo endPdi = null;
		if (!pdList.isEmpty()) {
			startPdi = pdList.get(0);
		}

		stationDetail.setOperatorTypeStr(
				CommonConvertUtils.convertToStationOperatorTypeStr(stationDetail.getOperatorType()));

		// 每次更新实时时长。
		if (stationDetail.getPackDischargeTimePred() != null && stationDetail.getSoc() != null) {
			stationDetail.setRealDuration(stationDetail.getPackDischargeTimePred()
					.multiply(BigDecimal.valueOf(stationDetail.getSoc() / 100.0)));
		}

		BigDecimal stationCap = null;
		String stationCapStr = stationDetail.getPackType();
		if (stationCapStr != null) {
			stationCapStr = stationCapStr.toLowerCase();
			stationCapStr = stationCapStr.substring(stationCapStr.indexOf("v") + 1, stationCapStr.length() - 2);
			stationCap = new BigDecimal(stationCapStr);

		}
		for (CellInfoDetail cd : stationDetail.getCellInfoDetailList()) {
			if (new Integer(0).equals(cd.getCellStatus())) {
				cd.setCellStatusStr("正常");
			} else if (new Integer(1).equals(cd.getCellStatus())) {
				cd.setCellStatusStr("较差");
			} else if (new Integer(2).equals(cd.getCellStatus())) {
				cd.setCellStatusStr("故障");
			}

			if (stationCap != null) {
				BigDecimal percent = null;
				if (cd.getCellCap() != null) { // 加了个判断
					percent = cd.getCellCap().divide(stationCap, 4, BigDecimal.ROUND_HALF_UP)
							.multiply(new BigDecimal(100), new MathContext(3, RoundingMode.HALF_UP));
				}
				cd.setCapPercent(percent);
			}
			if (startPdi != null) {
				cd.setStartVol((BigDecimal) ReflectUtil.getValueByKey(startPdi, "cellVol" + cd.getCellIndex()));
				// cd.setEndVol((BigDecimal) ReflectUtil.getValueByKey(endPdi, "cellVol" +
				// cd.getCellIndex()));
			}
			if (pde != null) {
				// 暂时用该字段表示内阻
				cd.setEndVol((BigDecimal) ReflectUtil.getValueByKey(pde, "cellResist" + cd.getCellIndex()));
			}
		}

		return stationDetail;
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
	 * 根据电池容量从小到大排序
	 * 
	 * @param cellList
	 */
	public void sortByCap(List<CellInfoDetail> cellList) {
		Collections.sort(cellList, new Comparator<CellInfoDetail>() {
			@Override
			public int compare(CellInfoDetail o1, CellInfoDetail o2) {
				BigDecimal cellCap1 = o1.getCellCap();
				BigDecimal cellCap2 = o2.getCellCap();
				if (cellCap1 == null) {
					cellCap1 = new BigDecimal("0");
				}
				if (cellCap2 == null) {
					cellCap2 = new BigDecimal("0");
				}
				return cellCap1.compareTo(cellCap2);
			}
		});
	}

	@Override
	public void deleteByPKs(Integer[] ids) {
		if (ids.length == 1 && ids[0].equals(99999)) {
			String id = stationInfoMapper.selectDb();
			stationInfoMapper.delByPKs(id);
		} else {
			super.deleteByPKs(ids);
		}

	}

	@Override
	public boolean parseStationExcelFile(File file, AjaxResponse ajaxResponse, Integer companyId)
			throws EncryptedDocumentException, InvalidFormatException, IOException {
		// 默认为 false
		if (stationFile) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("系统繁忙！");
			return true;
		}
		// 进入导入设置为true
		stationFile = true;
		// 先查询所有的单体平台电压对应的设备类型；
		GprsDeviceType gprsDeviceType = new GprsDeviceType();
		List<GprsDeviceType> deviceTypeList = gprsDeviceTypeSer.selectListSelective(gprsDeviceType);
		//k-type.V-电压
		Map<Integer, Integer> typeAndVol = new HashMap<Integer, Integer>();
		//k-type,V-sub_device_count
		//Map<Integer, Integer> typeAndSubCount = new HashMap<Integer, Integer>();
		if (deviceTypeList.size() != 0) {
			for (GprsDeviceType deviceType : deviceTypeList) {
				typeAndVol.put(deviceType.getTypeCode(), deviceType.getVolLevel());
				//typeAndSubCount.put(deviceType.getTypeCode(), deviceType.getSubDeviceCount());
			}
		}
		// 导入电池组，问题行详细记录信息
		TreeMap<Integer, String> errorRow = new TreeMap<Integer, String>();
		try {
			if (companyId == null) {
				ajaxResponse.setMsg("必须选择公司！");
				return false;
			}
			StationInfo companyStationInfo = new StationInfo();
			companyStationInfo.setCompanyId3(companyId);
			if (!commonSer.completeCompany(companyStationInfo)) {
				ajaxResponse.setMsg("公司不存在!");
				return false;
			}
			logger.info("start parse Station excel file!");
			int rowNum = 1;
			int successCount = 0;
			InputStream inp = new FileInputStream(file);
			Workbook wb = WorkbookFactory.create(inp);
			Sheet sheet = wb.getSheetAt(0);
			for (Row row : sheet) {
				boolean autoBind = false;
				rowNum = row.getRowNum();
				if (rowNum >= 2) {
					if (!RowParseHelper.hasData(row, 16)) {
						// 解析当前行有没有数据
						break;
					}
					StationInfo stationInfo = new StationInfo();
					BeanUtils.copyProperties(companyStationInfo, stationInfo);
					// 省份
					if (StringUtils.getString(row.getCell(0)) != null) {
						if (20 >= StringUtils.getString(row.getCell(0)).length()) {
							stationInfo.setProvince(StringUtils.getString(row.getCell(0)));
						} else {
							errorRow.put(rowNum + 1, "省份不存在！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "省份没有填写！");
						continue;
					}
					// 城市
					if (StringUtils.getString(row.getCell(1)) != null) {
						if (20 >= StringUtils.getString(row.getCell(1)).length()) {
							stationInfo.setCity(StringUtils.getString(row.getCell(1)));
						} else {
							errorRow.put(rowNum + 1, "地市不存在！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "城市没有填写！");
						continue;
					}
					// 区县
					if (StringUtils.getString(row.getCell(2)) != null) {
						if (20 >= StringUtils.getString(row.getCell(2)).length()) {
							stationInfo.setDistrict(StringUtils.getString(row.getCell(2)));
						} else {
							errorRow.put(rowNum + 1, "区县不存在！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "区县没有填写！");
						continue;
					}
					// 运行商
					if (StringUtils.getString(row.getCell(3)) != null) {
						stationInfo.setOperatorType(
								CommonConvertUtils.convertStrToStationOperatorType(row.getCell(3).toString()));
					} else {
						errorRow.put(rowNum + 1, "运营商没有填写！");
						continue;
					}
					// 物理站址名称
					if (StringUtils.getString(row.getCell(4)) != null) {
						String addressName = RowParseHelper.getCell(row, 4);
						if (30 >= addressName.length()) {
							stationInfo.setName(addressName);
						} else {
							errorRow.put(rowNum + 1, "物理地址信息过长，不超过30个字符！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "物理地址没有填写！");
						continue;
					}
					// 站址编号
					if (StringUtils.getString(row.getCell(5)) != null) {
						String code = RowParseHelper.getCell(row, 5);
						if (30 >= code.length()) {
							stationInfo.setAddressCoding(code);
						} else {
							errorRow.put(rowNum + 1, "站址编号长度不超过30个字符！");
							continue;
						}
					}

					// 解决当运维ID是纯数字时，被自动转化成科学计数法的格式
					if (StringUtils.getString(row.getCell(6)) != null) {
						String maintainanceId = null;
						try {
							maintainanceId = new DecimalFormat("0").format(row.getCell(6).getNumericCellValue());
						} catch (Exception e) {
							maintainanceId = StringUtils.getString(row.getCell(6));
						}
						if (30 >= maintainanceId.length()) {
							stationInfo.setMaintainanceId(maintainanceId);
						} else {
							errorRow.put(rowNum + 1, "运维ID不超过30个字符！");
							continue;
						}
					}
					if (StringUtils.getString(row.getCell(7)) != null) {
						String address = RowParseHelper.getCell(row, 7);
						if (45 >= address.length()) {
							stationInfo.setAddress(address);
						} else {
							errorRow.put(rowNum + 1, "详细地址信息过长，不超过45个字符！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "详细地址没有填写！");
						continue;
					}

					if (StringUtils.getString(row.getCell(8)) != null) {

						BigDecimal lng = null;
						try {
							lng = new BigDecimal(row.getCell(8).toString());
						} catch (Exception e) {
							errorRow.put(rowNum + 1, "经度值必须是数字类型！");
							continue;
						}
						if (16 >= String.valueOf(lng.doubleValue()).length()) {
							if (lng.doubleValue() < 0.0) {
								errorRow.put(rowNum + 1, "在中国境内经度值不能为负数！");
								continue;
							}
							if (lng.abs().compareTo(new BigDecimal(180d)) != 1) {
								stationInfo.setLng(lng);
							} else {
								errorRow.put(rowNum + 1, "经度值错误！");
								continue;
							}
						} else {
							errorRow.put(rowNum + 1, "经度值过长！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "经度没有填写！");
						continue;
					}

					if (StringUtils.getString(row.getCell(9)) != null) {
						BigDecimal lat = null;
						try {
							lat = new BigDecimal(row.getCell(9).toString());
						} catch (Exception e) {
							errorRow.put(rowNum + 1, "纬度值必须是数字类型！");
							continue;
						}
						if (17 >= String.valueOf(lat.doubleValue()).length()) {
							if (lat.doubleValue() < 0.0) {
								errorRow.put(rowNum + 1, "在中国境内纬度值不能为负数！");
								continue;
							}
							if (lat.abs().compareTo(new BigDecimal(90d)) != 1) {
								stationInfo.setLat(new BigDecimal(row.getCell(9).toString()));
							} else {
								errorRow.put(rowNum + 1, "纬度值错误！");
								continue;
							}
						} else {
							errorRow.put(rowNum + 1, "纬度值过长！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "纬度没有填写！");
						continue;
					}

					// 电压平台
					if (StringUtils.getString(row.getCell(14)) != null) {
						String temp = row.getCell(14).toString().trim().toUpperCase();
						Pattern regex = Pattern.compile("^[1-9]{1,3}[V]{1}$");
						Matcher matcher = regex.matcher(temp);
						boolean matches = matcher.matches();
						if (!matches) {
							errorRow.put(rowNum + 1, "单体电压平台值不符合规范！ 例：3V");
							continue;
						}

						if (11 >= temp.length()) {
							if (!StringUtils.isNull(temp)) {
								if (temp.contains("V")) {
									stationInfo.setVolLevel(Integer.parseInt(temp.replace("V", "")));
								} else {
									stationInfo.setVolLevel(Integer.parseInt(temp));
								}
							}
						} else {
							errorRow.put(rowNum + 1, "单体电压平台值过长！");
							continue;
						}
						// 修改单体电压平台为必填
					} else {
						errorRow.put(rowNum + 1, "单体电压平台没有填写！");
						continue;
					}
					//电池组类型
					if (StringUtils.getString(row.getCell(12)) != null) {
						// 正则匹配 v前要有数字，ah前面要有数字且ah结尾
						String packType = StringUtils.getString(row.getCell(12)).toUpperCase();
						if (45 >= packType.length()) {
							if (packType.matches("^[1-9]\\d*V[1-9]\\d*AH$")) {
								stationInfo.setPackType(packType);
							} else {
								errorRow.put(rowNum + 1, "电池组类型格式错误！");
								continue;
							}
						} else {
							errorRow.put(rowNum + 1, "电池组类型过长！");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "电池组类型没有填写！");
						continue;
					}
					// 增加一列设备编号
					if (StringUtils.getString(row.getCell(10)) != null) {
						String id = RowParseHelper.getCell(row, 10);
						String gprsId = id.trim();
						if (!StringUtils.isNull(gprsId)) {
							// 查询设备是否被占用
							StationInfo stationBean = new StationInfo();
							stationBean.setGprsId(gprsId);
							List<StationInfo> stationList = stationInfoMapper.selectListSelective(stationBean);
							if (stationList.size() > 0) {
								// 提示该列设备已被占用
								errorRow.put(rowNum + 1, gprsId + "设备已被使用");
								continue;
							} else {
								// 查询是否有该设备信息
								GprsConfigInfo gprsBean = new GprsConfigInfo();
								gprsBean.setGprsId(gprsId);
								List<GprsConfigInfo> gprsList = gprsConfigInfoSer.selectListSelective(gprsBean);
								// 判断gprs_config_info 存在设备编号 和判断不是备用的；1 备用
								if (gprsList.size() > 0 && gprsList.get(0).getGprsFlag() != 1) {
									// 系统有该设备信息，自动绑定
									stationInfo.setGprsId(gprsId);
									stationInfo.setGprsIdOut(gprsId);
									stationInfo.setInspectStatus(30);// 已安装
									GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
									updateGprsConfigInfo.setGprsId(gprsId);
									updateGprsConfigInfo.setCompanyId(stationInfo.getCompanyId3());
									// 检查gprs_id_out是否为null，如果是，则设置为gprs_id.
									GprsConfigInfo queryInfo = new GprsConfigInfo();
									queryInfo.setGprsId(updateGprsConfigInfo.getGprsId());
									List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoSer
											.selectListSelective(queryInfo);
									if (gprsConfigInfos != null && gprsConfigInfos.size() > 0) {
										queryInfo = gprsConfigInfos.get(0);
										if (queryInfo.getGprsIdOut() == null || queryInfo.getGprsIdOut().isEmpty()) {
											updateGprsConfigInfo.setGprsIdOut(updateGprsConfigInfo.getGprsId());
										}
										//判断电压平台对应的设备类型和设备的设备类型是否一致
										if(typeAndVol.get(queryInfo.getDeviceType()) != stationInfo.getVolLevel()) {
											errorRow.put(rowNum + 1, "设备编号是" + gprsId + "的设备类型和电压平台对应的设备类型不一致！");
											continue;
										}
										//判断单体数量和从机数量
										Integer cellCount = calculateCellCount(stationInfo);
										if(queryInfo.getSubDeviceCount() != cellCount) {
											errorRow.put(rowNum + 1, "设备编号是" + gprsId + "的设备类型和电压平台对应的单体数量和从机数量不一致！");
											continue;
										}
									}
									gprsConfigInfoSer.updateByGprsSelective(updateGprsConfigInfo);
									autoBind = true;
								} else {
									// 系统无此设备编号，反馈:系统XX设备,请在设备信息中导入
									errorRow.put(rowNum + 1, "系统无" + gprsId + "设备信息或者设备是备用主机，请在设备信息中导入");
									continue;
								}
							}
						}
					}
					if (StringUtils.getString(row.getCell(11)) != null) {
						String type = RowParseHelper.getCell(row, 11);
						if (30 >= type.length()) {
							stationInfo.setRoomType(type);
						} else {
							errorRow.put(rowNum + 1, "机房类型过长！不超过30个字符");
							continue;
						}
					}

					// 增加了一列负载功率等级
					if (StringUtils.getString(row.getCell(13)) != null) {
						String temp = row.getCell(13).toString().trim().toUpperCase();
						Pattern regex = Pattern.compile("^\\d{1,9}$");
						Matcher matcher = regex.matcher(temp);
						boolean matches = matcher.matches();

						Pattern regex2 = Pattern.compile("^[0-9]+(.[0]{1})?");
						Matcher matcher2 = regex2.matcher(temp);
						boolean matches2 = matcher2.matches();

						Integer temp2 = null;
						if (!matches) {
							if (!matches2) {
								errorRow.put(rowNum + 1, "负载功率等级值不符合规范！ 例：200；是整数");
								continue;
							} else {
								double dou = Double.parseDouble(temp);
								temp2 = (int) dou;
							}
						}
						if (12 > temp.length()) {
							if (!StringUtils.isNull(temp)) {
								if (temp.contains("W")) {
									stationInfo
											.setLoadPower(BigDecimal.valueOf(Integer.valueOf(temp.replace("W", ""))));
								} else {
									stationInfo.setLoadPower(BigDecimal.valueOf(Integer.valueOf(temp2)));
								}
							}
						} else {
							errorRow.put(rowNum + 1, "负载功率等级值过长！");
							continue;
						}
					}

					if (StringUtils.getString(row.getCell(15)) != null) {
						String plant = RowParseHelper.getCell(row, 15);
						if (30 >= plant.length()) {
							stationInfo.setCellPlant(plant);
						} else {
							errorRow.put(rowNum + 1, "品牌字段过长！不超过30个字符");
							continue;
						}
					}
					createStationCells(stationInfo);
					// 自动绑定
					if (autoBind) {
						GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
						updateGprsConfigInfo.setGprsId(stationInfo.getGprsId());
						updateGprsConfigInfo.setCompanyId(stationInfo.getCompanyId3());
						gprsConfigInfoSer.updateByGprsSelective(updateGprsConfigInfo);
						CellInfo updateCellInfo = new CellInfo();
						updateCellInfo.setGprsId(stationInfo.getGprsId());
						updateCellInfo.setStationId(stationInfo.getId());
						cellInfoSer.updateGprsIdByStationId(updateCellInfo);
					}
					successCount++;
				}

			}
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("电池组导入处理完成，共导入" + successCount + "行数据\r\n" + sb.toString());
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("电池组表导入出错，请选择正确模板！");
			logger.error("电池组表导入出错-->", e);
		}
		errorRow.clear();
		// 最后设置为false
		stationFile = false;
		return true;
	}

	@Override
	public void createStationCells(StationInfo stationInfo) {
		// 根据设备类型和设置电压平台的参数俩决定电池的数量 如电池组类型48V500AH 电压平台2V 电池数量 48/2 =24
		String packType = stationInfo.getPackType();
		Integer type = Integer.parseInt(packType.substring(0, packType.lastIndexOf("V")));
		Integer volLevel = stationInfo.getVolLevel();
		if (volLevel == 0) {
			throw new IllegalArgumentException("电压平台不能为0V！");
		}
		Integer number = type % volLevel;
		if (number == 0) {
			Integer cellCount = type / volLevel;
			stationInfo.setCellCount(cellCount);
			insertSelective(stationInfo);
			for (int i = 1; i < cellCount + 1; i++) {
				CellInfo cellInfo = new CellInfo();
				// ---10/24 gprsid 添加到cellInfo中
				cellInfo.setGprsId(stationInfo.getGprsId());
				cellInfo.setCellIndex(i);
				cellInfo.setCellPlant(stationInfo.getCellPlant());
				cellInfo.setStationId(stationInfo.getId());
				cellInfo.setUpdateTime(new Date());
				cellInfoSer.insertSelective(cellInfo);
			}
		} else {
			throw new IllegalArgumentException("电池组类型和电压平台不匹配！");
		}
	}

	@Override
	public void pulseDischargeAsync(CommonSearchVo commonSearchVo) {
		new Thread(new Runnable() {
			@Override
			public void run() {
				pulseDischargeSync(commonSearchVo);
			}
		}).start();
	}

	void pulseDischargeSync(CommonSearchVo commonSearchVo) {
		if (commonSearchVo.getGprsList() == null) {
			StationInfo queryStationInfo = new StationInfo();
			BeanUtils.copyProperties(commonSearchVo, queryStationInfo);
			queryStationInfo.setCompanyId3(commonSearchVo.getCompanyId());
			List<StationInfo> stationList = selectListSelective(queryStationInfo);
			List<String> gprsList = new ArrayList<String>();
			for (StationInfo stationInfo : stationList) {
				if (!stationInfo.getGprsId().equals("-1")) {
					gprsList.add(stationInfo.getGprsId());
				}
			}
			commonSearchVo.setGprsList(gprsList);
		}
		if (commonSearchVo.getGprsList().size() > 0) {
			cellInfoSer.updateSendDoneByGprs(commonSearchVo.getGprsList());
		}
		logger.debug("pulse discharge over!!!!!!!!!!!!");
	}

	@Override
	public void updateByGprsSelective(StationInfo record) {
		if (record.getGprsId().equals("-1"))
			throw new RuntimeException("按GPRSID更新时，gprsd=-1，会更新其他数据，请检查业务逻辑。");
		int row = stationInfoMapper.updateByGprsSelective(record);
		logger.debug("updateByGprsSelective row=" + row);
	}

	@Override
	public void updateByCompanyIdSelective(StationInfo record) {
		stationInfoMapper.updateByCompanyIdSelective(record);
	}

	@Override
	public String exportStationCheckToExcel(StationDetail stationDetail) {
		String tplPath = Constant.TEMPLETE_PATH + "电池组性能及容量检测报告.xlsx";
		String destPath = new Date().getTime() + "_" + stationDetail.getGprsId() + ".xlsx";
		Map beanParams = new HashMap();
		try {
			beanParams.put("sd", stationDetail);
			beanParams.put("exportDateTime", MyDateUtils.getDateString(new Date(), "yyyy/MM/dd HH:mm"));
			XLSTransformer former = new XLSTransformer();
			tplPath = URLDecoder.decode(tplPath, "UTF-8");
			former.transformXLS(tplPath, beanParams, destPath);
		} catch (ParsePropertyException e) {
			e.printStackTrace();
		} catch (InvalidFormatException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return destPath;
	}

	@SuppressWarnings({ "rawtypes", "unchecked" })
	@Override
	public List getStationStatusList(CommonSearchVo commonSearchVo) {
		List<Map> stationStatusList = stationInfoMapper.selectStationStatus(commonSearchVo);
		Map<String, Map> sumMap = new HashMap<String, Map>();
		Map companyMap = new HashMap();
		for (Map m : stationStatusList) {
			companyMap.put(m.get("company_id3"), m.get("company_id3"));
			if (sumMap.get(m.get("company_id3") + "|" + m.get("city")) == null) {
				Map tmpMap = new HashMap();
				tmpMap.put("company_name3", m.get("company_name3"));
				tmpMap.put("district", m.get("city"));
				tmpMap.put("city", m.get("city"));
				tmpMap.put("sum_num", m.get("sum_num"));
				tmpMap.put("company_id3", m.get("company_id3"));
				tmpMap.put("num0", m.get("num0"));
				tmpMap.put("num1", m.get("num1"));
				tmpMap.put("num2", m.get("num2"));

				sumMap.put(m.get("company_id3") + "|" + m.get("city"), tmpMap);
			} else {
				Map tmpMap = (Map) sumMap.get(m.get("company_id3") + "|" + m.get("city"));
				tmpMap.put("sum_num", (Long) m.get("sum_num") + (Long) tmpMap.get("sum_num"));

				tmpMap.put("num0", (Long) m.get("num0") + (Long) tmpMap.get("num0"));
				tmpMap.put("num1", (Long) m.get("num1") + (Long) tmpMap.get("num1"));
				tmpMap.put("num2", (Long) m.get("num2") + (Long) tmpMap.get("num2"));

			}
		}

		for (Map m : sumMap.values()) {
			m.put("percent0", new BigDecimal((long) m.get("num0")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
			m.put("percent1", new BigDecimal((long) m.get("num1")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
			m.put("percent2", new BigDecimal((long) m.get("num2")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
		}
		List resutlList = new ArrayList();
		stationStatusList.addAll(sumMap.values());

		Map<Integer, List> stationStatusMap = new HashMap<Integer, List>();
		for (Map m : stationStatusList) {
			Integer companyId = Integer.parseInt(m.get("company_id3").toString());
			if (stationStatusMap.get(companyId) == null) {
				List tmpList = new ArrayList<>();
				tmpList.add(m);
				stationStatusMap.put(companyId, tmpList);
			} else {
				((ArrayList) stationStatusMap.get(companyId)).add(m);
			}
		}

		for (Object o : companyMap.values()) {
			Integer companyId = Integer.parseInt(o.toString());
			GprsConfigInfo queryGprsConfig = new GprsConfigInfo();
			queryGprsConfig.setCompanyId(companyId);
			List<GprsConfigInfo> companyList = gprsConfigInfoSer.selectListSelective(queryGprsConfig);
			for (GprsConfigInfo config : companyList) {
				if (config.getConsoleCellCapNormal() != null) {
					Map m = new HashMap();
					m.put("consoleCellCapNormal", NumberFormat.getInstance().format(config.getConsoleCellCapNormal()));
					m.put("consoleCellCapError", NumberFormat.getInstance().format(config.getConsoleCellCapError()));
					m.put("companyName", companySer.selectByPrimaryKey(config.getCompanyId()).getCompanyName());
					List sList = stationStatusMap.get(companyId);
					sList.sort(new Comparator<Map>() {

						@Override
						public int compare(Map o1, Map o2) {
							if (o1.get("city").equals(o2.get("city"))) {
								if (o1.get("district").equals(o1.get("city"))) {
									return 1;
								} else if (o2.get("district").equals(o2.get("city"))) {
									return -1;
								}

							}
							return o1.get("city").toString().compareTo(o2.get("city").toString());
						}

					});
					m.put("sList", sList);
					resutlList.add(m);
					break;
				}
			}
		}
		return resutlList;
	}

	@SuppressWarnings({ "rawtypes", "unchecked" })
	@Override
	public List getStationDurationStatusList(CommonSearchVo commonSearchVo) {
		List<Map> stationStatusList = stationInfoMapper.selectStationDurationStatus(commonSearchVo);
		Map<String, Map> sumMap = new HashMap<String, Map>();
		Map companyMap = new HashMap();
		for (Map m : stationStatusList) {
			companyMap.put(m.get("company_id3"), m.get("company_id3"));
			if (sumMap.get(m.get("company_id3") + "|" + m.get("city")) == null) {
				Map tmpMap = new HashMap();
				tmpMap.put("company_name3", m.get("company_name3"));
				tmpMap.put("district", m.get("city"));
				tmpMap.put("city", m.get("city"));
				tmpMap.put("sum_num", m.get("sum_num"));
				tmpMap.put("company_id3", m.get("company_id3"));
				tmpMap.put("num4", m.get("num4"));
				tmpMap.put("num1", m.get("num1"));
				tmpMap.put("num2", m.get("num2"));
				tmpMap.put("num3", m.get("num3"));

				sumMap.put(m.get("company_id3") + "|" + m.get("city"), tmpMap);
			} else {
				Map tmpMap = (Map) sumMap.get(m.get("company_id3") + "|" + m.get("city"));
				tmpMap.put("sum_num", (Long) m.get("sum_num") + (Long) tmpMap.get("sum_num"));

				tmpMap.put("num4", (Long) m.get("num4") + (Long) tmpMap.get("num4"));
				tmpMap.put("num1", (Long) m.get("num1") + (Long) tmpMap.get("num1"));
				tmpMap.put("num2", (Long) m.get("num2") + (Long) tmpMap.get("num2"));
				tmpMap.put("num3", (Long) m.get("num3") + (Long) tmpMap.get("num3"));

			}
		}

		for (Map m : sumMap.values()) {
			m.put("percent4", new BigDecimal((long) m.get("num4")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
			m.put("percent1", new BigDecimal((long) m.get("num1")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
			m.put("percent2", new BigDecimal((long) m.get("num2")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
			m.put("percent3", new BigDecimal((long) m.get("num3")).multiply(new BigDecimal(100))
					.divide(new BigDecimal((long) m.get("sum_num")), 2, BigDecimal.ROUND_HALF_UP));
		}
		List resutlList = new ArrayList();
		stationStatusList.addAll(sumMap.values());

		Map<Integer, List> stationStatusMap = new HashMap<Integer, List>();
		for (Map m : stationStatusList) {
			Integer companyId = Integer.parseInt(m.get("company_id3").toString());
			if (stationStatusMap.get(companyId) == null) {
				List tmpList = new ArrayList<>();
				tmpList.add(m);
				stationStatusMap.put(companyId, tmpList);
			} else {
				((ArrayList) stationStatusMap.get(companyId)).add(m);
			}
		}

		for (Object o : companyMap.values()) {
			Integer companyId = Integer.parseInt(o.toString());
			GprsConfigInfo queryGprsConfig = new GprsConfigInfo();
			queryGprsConfig.setCompanyId(companyId);
			List<GprsConfigInfo> companyList = gprsConfigInfoSer.selectListSelective(queryGprsConfig);
			for (GprsConfigInfo config : companyList) {
				if (config.getConsoleCellCapNormal() != null) {
					Map m = new HashMap();
					m.put("durationMinExcellent", JxlsUtil.minute2Hour(config.getDurationMinExcellent()));
					m.put("durationMaxExcellent", JxlsUtil.minute2Hour(config.getDurationMaxExcellent()));
					m.put("durationMinGood", JxlsUtil.minute2Hour(config.getDurationMinGood()));
					m.put("durationMaxGood", JxlsUtil.minute2Hour(config.getDurationMaxGood()));

					m.put("durationMinMedium", JxlsUtil.minute2Hour(config.getDurationMinMedium()));
					m.put("durationMaxMedium", JxlsUtil.minute2Hour(config.getDurationMaxMedium()));
					m.put("durationMinBad", JxlsUtil.minute2Hour(config.getDurationMinBad()));
					m.put("durationMaxBad", JxlsUtil.minute2Hour(config.getDurationMaxBad()));
					m.put("companyName", companySer.selectByPrimaryKey(config.getCompanyId()).getCompanyName());
					List sList = stationStatusMap.get(companyId);
					sList.sort(new Comparator<Map>() {

						@Override
						public int compare(Map o1, Map o2) {
							if (o1.get("city").equals(o2.get("city"))) {
								if (o1.get("district").equals(o1.get("city"))) {
									return 1;
								} else if (o2.get("district").equals(o2.get("city"))) {
									return -1;
								}

							}
							return o1.get("city").toString().compareTo(o2.get("city").toString());
						}

					});
					m.put("sList", sList);
					resutlList.add(m);
					break;
				}
			}
		}
		return resutlList;
	}

	@Override
	public String exportStationStatusToExcel(Integer companyId, Integer companyLevel) {
		CommonSearchVo commonSearchVo = new CommonSearchVo();
		commonSearchVo.setCompanyId(companyId);
		commonSearchVo.setCompanyLevel(companyLevel);
		List stationStatusList = getStationStatusList(commonSearchVo);
		List stationDurationStatusList = getStationDurationStatusList(commonSearchVo);
		String tplPath = Constant.TEMPLETE_PATH + "公司电池组状态统计报表.xlsx";
		String destPath = new Date().getTime() + ".xlsx";
		Map beanParams = new HashMap();

		beanParams.put("stationList", stationStatusList);
		beanParams.put("stationDurationList", stationDurationStatusList);
		beanParams.put("exportDateTime", MyDateUtils.getDateString(new Date(), "yyyy-MM-dd HH:mm"));
		beanParams.put("companyName", companySer.selectByPrimaryKey(companyId).getCompanyName());
		XLSTransformer former = new XLSTransformer();
		try {
			tplPath = URLDecoder.decode(tplPath, "UTF-8");
			former.transformXLS(tplPath, beanParams, destPath);
		} catch (ParsePropertyException e) {
			e.printStackTrace();
		} catch (InvalidFormatException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return destPath;
	}

	@Override
	public StationDetail getStationDetailBasicByStationId(Integer stationId) {
		StationDetail stationDetail = new StationDetail();
		StationInfo stationInfo = selectByPrimaryKey(stationId);
		if (stationInfo == null) {
			return stationDetail;
		}
		detailInfoHandler(stationDetail, stationInfo);

		return stationDetail;
	}

	// --------------- add ------------------
	@Override
	public StationDetail getStationDetailBasicByGprsId(String gprsId) {
		StationDetail stationDetail = new StationDetail();
		// eg: T0B000002 base_station_info表中没有数据
		StationInfo stationInfo = new StationInfo();
		stationInfo.setGprsId(gprsId);
		detailInfoHandler(stationDetail, stationInfo);
		return stationDetail;
	}

	private void detailInfoHandler(StationDetail stationDetail, StationInfo stationInfo) {
		BeanUtils.copyProperties(stationInfo, stationDetail);
		final String gprsId = stationDetail.getGprsId();
		if (!gprsId.equals("-1")) {
			GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
			queryGprsConfigInfo.setGprsId(stationInfo.getGprsId());
			List<GprsConfigInfo> configList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
			GprsConfigInfo c = null;
			if (configList.size() > 0) {
				c = configList.get(0);
				stationDetail.setLinkStatus(c.getLinkStatus());
				stationDetail.setDeviceType(c.getDeviceType());
				stationDetail.setDeviceTypeStr(c.getDeviceTypeStr());
				stationDetail.setGprsFlag(c.getGprsFlag());
				// ----10/16 add 添加电话卡号 \端口\规格
				stationDetail.setDevicePhone(c.getDevicePhone());
				stationDetail.setGprsPort(c.getGprsPort());
				stationDetail.setGprsSpec(c.getGprsSpec());

			}
			PackDataInfoLatest queryPdi = new PackDataInfoLatest();
			queryPdi.setGprsId(gprsId);
			List<PackDataInfoLatest> pdiList = packDataInfoLatestSer.selectListSelective(queryPdi);
			PackDataInfoLatest pdi = null;
			if (pdiList.size() > 0) {
				pdi = pdiList.get(0);
				stationDetail.setUpdateTime(pdi.getRcvTime());
				stationDetail.setState(pdi.getState());
				stationDetail.setGenCur(pdi.getGenCur());
				stationDetail.setGenVol(pdi.getGenVol());
				stationDetail.setEnvironTem(pdi.getEnvironTem());
				stationDetail.setSoc(pdi.getSoc());
				// 加的
				stationDetail.setPackDataInfoLatest(pdi);
			}
			PackDataExpandLatest queryPde = new PackDataExpandLatest();
			queryPde.setGprsId(gprsId);
			List<PackDataExpandLatest> pdeList = packDataExpandLatestSer.selectListSelective(queryPde);
			PackDataExpandLatest pde = null;
			if (pdeList.size() > 0) {
				pde = pdeList.get(0);
				stationDetail.setPackCapPred(pde.getPackCapPred());
				stationDetail.setPackDischargeTimePred(pde.getPackDischargeTimePred());
			}
			PulseCalculationSend latestRecord = pulseCalculationSendMapper.getLatestRecord(gprsId);
			if (latestRecord != null) {
				stationDetail.setCurrentModelCalculationStatus(latestRecord.getSendDone());
				stationDetail.setCurrentModelCalculationStatusStr(latestRecord.getSendDoneMessage());
				stationDetail.setCurrentModelCalculationTime(latestRecord.getSendTime());

				// 0:内阻容量都成功, 1:内阻成功容量失败, 2:内阻失败容量成功, 3:内阻容量都失败
				Map<String, Object> paramMap = Maps.newHashMap();
				paramMap.put("gprsId", gprsId);
				paramMap.put("exclusionId", latestRecord.getId());
				if (latestRecord.getSendDone() == 1) {
					PulseCalculationSend item = pulseCalculationSendMapper.getCapacityRecord(paramMap);
					if (item != null) {
						stationDetail.setLastCapcityCalculationSuccessTime(item.getSendTime());
					}
				} else if (latestRecord.getSendDone() == 2) {
					PulseCalculationSend item = pulseCalculationSendMapper.getResistanceRecord(paramMap);
					if (item != null) {
						stationDetail.setLastResistanceCalculationSuccessTime(item.getSendTime());
					}
				} else if (latestRecord.getSendDone() == 3) {
					PulseCalculationSend item = pulseCalculationSendMapper.getCapacityRecord(paramMap);
					if (item != null) {
						stationDetail.setLastCapcityCalculationSuccessTime(item.getSendTime());
					}
					item = pulseCalculationSendMapper.getResistanceRecord(paramMap);
					if (item != null) {
						stationDetail.setLastResistanceCalculationSuccessTime(item.getSendTime());
					}
				}
			}
			CellInfo queryCell = new CellInfo();
			queryCell.setStationId(stationDetail.getId());
			queryCell.setGprsId(stationDetail.getGprsId()); // 增加了GPRS_ID 的参数，解决页面显示48个单体的bug
			List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);
			SubDevice querySubDevice = new SubDevice();
			querySubDevice.setGprsId(gprsId);
			List<SubDevice> subDeviceList = subDeviceSer.selectListSelective(querySubDevice);
			Map<Integer, SubDevice> subDeviceMap = new HashMap<Integer, SubDevice>();
			for (SubDevice subDevice : subDeviceList) {
				subDeviceMap.put(subDevice.getCellSort(), subDevice);
			}
			GprsBalanceSend gprsBalanceSend = gprsBalanceSendService.selectByGprs(gprsId);
			PulseDischargeSend pulseDischargeSend = new PulseDischargeSend();
			pulseDischargeSend.setGprsId(gprsId);
			List<PulseDischargeSend> dischargeSends = pulseDischargeSendService.selectListSelective(pulseDischargeSend);
			Map<Integer, List<PulseDischargeSend>> dischargeMap = dischargeSends.stream()
					.collect(Collectors.groupingBy(PulseDischargeSend::getPulseCell));
			List<CellInfoDetail> cellDetailList = new ArrayList<CellInfoDetail>();
			for (CellInfo cell : cellList) {
				CellInfoDetail cellDetail = new CellInfoDetail();
				BeanUtils.copyProperties(cell, cellDetail);
				List<PulseDischargeSend> items = dischargeMap.get(cellDetail.getCellIndex());
				if (CollectionUtils.isNotEmpty(items)) {
					items.sort(Comparator.comparing(PulseDischargeSend::getInsertTime).reversed());
					PulseDischargeSend item = items.get(0);
					cellDetail.setTestStatus(item.getSendDone().intValue());
					switch (cellDetail.getTestStatus()) {
					case 0:
						cellDetail.setTestStatusStr("未发送");
						break;
					case 1:
						cellDetail.setTestStatusStr("发送成功");
						break;
					case 2:
						cellDetail.setTestStatusStr("特征执行成功");
						break;
					case 3:
						cellDetail.setTestStatusStr("特征执行失败");
						break;
					case 6:
						cellDetail.setTestStatusStr("特征执行超时");
						break;
					}
					cellDetail.setTestTime(item.getInsertTime());
				}
				if (pde != null) {
					cellDetail.setCellCapIndex(
							(Integer) BeanValueUtils.getValue("cellCapSort" + cellDetail.getCellIndex(), pde));
					cellDetail.setCellResist(
							(BigDecimal) BeanValueUtils.getValue("cellResist" + cellDetail.getCellIndex(), pde));
					cellDetail.setCellStatus(
							(Integer) BeanValueUtils.getValue("cellEvalu" + cellDetail.getCellIndex(), pde));
					cellDetail.setCellCap(
							(BigDecimal) BeanValueUtils.getValue("cellCap" + cellDetail.getCellIndex(), pde));
				}
				if (pdi != null) {
					cellDetail.setCellVol(
							(BigDecimal) BeanValueUtils.getValue("cellVol" + cellDetail.getCellIndex(), pdi));
					cellDetail.setCellEqu((Byte) BeanValueUtils.getValue("cellEqu" + cellDetail.getCellIndex(), pdi));
					cellDetail
							.setCellTem((Integer) BeanValueUtils.getValue("cellTem" + cellDetail.getCellIndex(), pdi));
				} else {
					// 添加逻辑，当pdi表中没有数据时，默认cellEqu赋值为0，方便前端显示
					cellDetail.setCellEqu((byte) 0);
				}
				if (gprsBalanceSend != null) {
					cellDetail.setBalanceStatus(
							((Byte) BeanValueUtils.getValue("cell" + cellDetail.getCellIndex(), gprsBalanceSend))
									.intValue());
					switch (cellDetail.getBalanceStatus()) {
					case 3:
						cellDetail.setBalanceStatusStr("均衡中（降压）");
						cellDetail.setBalanceCommand("降压均衡" + gprsBalanceSend.getDuration() + "分钟");
						break;
					case 2:
						cellDetail.setBalanceStatusStr("均衡中（升压）");
						cellDetail.setBalanceCommand("升压均衡" + gprsBalanceSend.getDuration() + "分钟");
						break;
					case 1:
						cellDetail.setBalanceStatusStr("托管");
						cellDetail.setBalanceCommand("托管" + gprsBalanceSend.getDuration() + "分钟");
						break;
					case 0:
						cellDetail.setBalanceStatusStr("关闭");
						cellDetail.setBalanceCommand("关闭" + gprsBalanceSend.getDuration() + "分钟");
						break;
					}
					cellDetail.setBalanceTime(gprsBalanceSend.getSendTime());
				}
				SubDevice subDevice = subDeviceMap.get(cell.getCellIndex());
				if (subDevice != null) {
					cellDetail.setSubDeviceId(subDevice.getSubDeviceId());
					cellDetail.setSubDeviceIdOut(subDevice.getSubDeviceIdOut());
				}
				cellDetailList.add(cellDetail);
			}
			sortByCap(cellDetailList);
			sortByIndex(cellDetailList);
			stationDetail.setCellInfoDetailList(cellDetailList);
		}

		stationDetail.setOperatorTypeStr(
				CommonConvertUtils.convertToStationOperatorTypeStr(stationDetail.getOperatorType()));
		stationDetail.setDurationHour(
				(stationDetail.getDuration() == null ? 0 : stationDetail.getDuration().floatValue()) / 60);
		// 每次更新实时时长。
		if (stationDetail.getPackDischargeTimePred() != null && stationDetail.getSoc() != null) {
			stationDetail.setRealDuration(stationDetail.getPackDischargeTimePred()
					.multiply(BigDecimal.valueOf(stationDetail.getSoc() / 100.0)));
		}

		BigDecimal stationCap = null;
		String stationCapStr = stationDetail.getPackType();
		if (!StringUtils.isNull(stationCap)) {
			stationCapStr = stationCapStr.toLowerCase();
			stationCapStr = stationCapStr.substring(stationCapStr.indexOf("v") + 1, stationCapStr.length() - 2);
			stationCap = new BigDecimal(stationCapStr);

		}
		for (CellInfoDetail cd : stationDetail.getCellInfoDetailList()) {
			if (new Integer("0").equals(cd.getCellStatus())) {
				cd.setCellStatusStr("正常");
			} else if (new Integer("1").equals(cd.getCellStatus())) {
				cd.setCellStatusStr("较差");
			} else if (new Integer("2").equals(cd.getCellStatus())) {
				cd.setCellStatusStr("故障");
			}

			if (stationCap != null && cd.getCellCap() != null) {
				BigDecimal percent = cd.getCellCap().divide(stationCap, 4, BigDecimal.ROUND_HALF_UP)
						.multiply(new BigDecimal(100), new MathContext(3, RoundingMode.HALF_UP));
				cd.setCapPercent(percent);
			}
		}
	}

	@Override
	public void updateGprsAndCellAndStation(StationInfo stationInfo) {
		String gprsId = stationInfo.getGprsId();
		// 绑定中的修改
		if (!StringUtils.isNull(gprsId) && !gprsId.equals("-1")) {
			GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
			updateGprsConfigInfo.setGprsId(gprsId);
			updateGprsConfigInfo.setCompanyId(stationInfo.getCompanyId3());
			// 检查gprs_id_out是否为null，如果是，则设置为gprs_id.
			GprsConfigInfo queryInfo = new GprsConfigInfo();
			queryInfo.setGprsId(updateGprsConfigInfo.getGprsId());
			List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoSer.selectListSelective(queryInfo);
			String gprsIdOut = gprsId;
			if (gprsConfigInfos != null && gprsConfigInfos.size() > 0) {
				queryInfo = gprsConfigInfos.get(0);
				if (queryInfo.getGprsIdOut() == null || queryInfo.getGprsIdOut().isEmpty()) {
					updateGprsConfigInfo.setGprsIdOut(updateGprsConfigInfo.getGprsId());
					gprsIdOut = updateGprsConfigInfo.getGprsId();
				} else {
					gprsIdOut = queryInfo.getGprsIdOut();
				}
				// 判断电压平台是否一致
				GprsDeviceType gprsDeviceType = new GprsDeviceType();
				gprsDeviceType.setTypeCode(queryInfo.getDeviceType());
				List<GprsDeviceType> deviceType = gprsDeviceTypeSer.selectListSelective(gprsDeviceType);
				if (deviceType.size() != 0) {
					if (stationInfo.getVolLevel() != deviceType.get(0).getVolLevel()) {
						throw new RuntimeException("绑定的设备类型与当前电池组的类型、单体电压平台不匹配，保存失败！");
					}
					// 根据单体电压平台和电池组类型决定单体的个数
					CellInfo cellInfo = new CellInfo();
					cellInfo.setStationId(stationInfo.getId());
					List<CellInfo> cellList = cellInfoSer.selectListSelective(cellInfo);
					Integer cellCount = null;
					if (cellList.size() != 0) {
						cellCount = calculateCellCount(stationInfo);
					}
					if (cellCount != null) {
						// 绑定设备的从机和电池组单体是否对应
						if (cellCount != deviceType.get(0).getSubDeviceCount()) {
							throw new RuntimeException("绑定设备对应的从机个数和该电池组对应的单体个数不相符合，保存失败！");
						}
						Integer cellCountOld = cellList.size();
						isInsertCellInfo(stationInfo, cellCountOld, cellCount);
						stationInfo.setCellCount(cellCount);
					}
				}
			}
			gprsConfigInfoSer.updateByGprsSelective(updateGprsConfigInfo);

			stationInfo.setInspectStatus(30);// 已安装
			stationInfo.setGprsIdOut(gprsIdOut);
			stationInfoSer.updateByPrimaryKeySelective(stationInfo);

			CellInfo updateCellInfo = new CellInfo();
			updateCellInfo.setGprsId(stationInfo.getGprsId());
			updateCellInfo.setStationId(stationInfo.getId());
			cellInfoSer.updateGprsIdByStationId(updateCellInfo);
		}

		// 未绑定的电池组修改或已经绑定的电池组解绑
		if (gprsId != null && gprsId.equals("-1")) {
			// 根据单体电压平台和电池组类型决定单体的个数
			CellInfo cellInfo = new CellInfo();
			cellInfo.setStationId(stationInfo.getId());
			List<CellInfo> cellList = cellInfoSer.selectListSelective(cellInfo);
			Integer cellCount = null;
			if (cellList.size() != 0) {
				cellCount = calculateCellCount(stationInfo);
			}
			if (cellCount != null) {
				Integer cellCountOld = cellList.size();
				isInsertCellInfo(stationInfo, cellCountOld, cellCount);
				stationInfo.setCellCount(cellCount);
			}
			stationInfo.setGprsIdOut("-1");
			stationInfo.setInspectStatus(99);// 未安装
			stationInfoSer.updateByPrimaryKeySelective(stationInfo);

			CellInfo updateCell = new CellInfo();
			updateCell.setGprsId("-1");
			updateCell.setStationId(stationInfo.getId());
			cellInfoSer.updateGprsIdByStationId(updateCell);
		}

	}

	// 根据单体的平台的电压和设备类型计算单体的个数
	public Integer calculateCellCount(StationInfo stationInfo) {
		String packType = stationInfo.getPackType();
		Integer type = Integer.parseInt(packType.substring(0, packType.lastIndexOf("V")));
		Integer volLevel = stationInfo.getVolLevel();
		if (volLevel == 0) {
			throw new IllegalArgumentException("电压平台不能为0V！");
		}
		Integer number = type % volLevel;
		if (number == 0) {
			Integer cellCount = type / volLevel;
			return cellCount;
		} else {
			throw new IllegalArgumentException("电池组类型和电压平台不匹配！");
		}
	}

	// 根据以前单体的个数和现在计算后的单体个数比较；判断是否需要新增单体信息 。
	public void isInsertCellInfo(StationInfo stationInfo, Integer cellCountOld, Integer cellCount) {
		if (cellCountOld < cellCount) {
			for (int i = cellCountOld + 1; i < cellCount + 1; i++) {
				CellInfo cellInfo = new CellInfo();
				cellInfo.setGprsId(stationInfo.getGprsId());
				cellInfo.setCellIndex(i);
				cellInfo.setCellPlant(stationInfo.getCellPlant());
				cellInfo.setStationId(stationInfo.getId());
				cellInfo.setUpdateTime(new Date());
				cellInfoSer.insertSelective(cellInfo);
			}
		}
		if (cellCountOld > cellCount) {
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("cellCountOld", cellCountOld + 1);
			map.put("cellCount", cellCount + 1);
			map.put("stationId", stationInfo.getId());
			cellInfoMapper.deleteMoreCell(map);
		}
	}

	// app 首页分页
	@Override
	public List<StationInfo> appSelectListSelectivePaging(SearchStationInfoPagingVo searchStationInfoPagingVo) {
		return stationInfoMapper.appSelectListSelectivePaging(searchStationInfoPagingVo);
	}

	// app绑定电池组添加巡检记录
	@Override
	public void bindingAddRoutingInspections(RoutingInspectionStationDetail routingInspectionsDetail) {
		RoutingInspections routingInspections = new RoutingInspections();
		routingInspections.setRoutingInspectionStatus(0);// 未提交之前的状态
		routingInspections.setOperateTime(new Date());
		routingInspections.setStationId(routingInspectionsDetail.getStationId());
		routingInspections.setOperateType(routingInspectionsDetail.getOperateType());
		routingInspections.setOperateId(routingInspectionsDetail.getOperateId());
		routingInspections.setOperateName(routingInspectionsDetail.getOperateName());
		routingInspections.setDeviceType(routingInspectionsDetail.getDeviceType());
		routingInspections.setGprsId(routingInspectionsDetail.getGprsId());
		routingInspections.setOperatePhone(routingInspectionsDetail.getOperatePhone());
		routingInspectionsSer.insertSelective(routingInspections);

		RoutingInspectionDetail routingDeail = new RoutingInspectionDetail();
		routingDeail.setRoutingInspectionsId(routingInspections.getRoutingInspectionId());
		routingDeail.setDetailOperateId(routingInspectionsDetail.getOperateId());
		routingDeail.setDetailOperateName(routingInspectionsDetail.getOperateName());
		routingDeail.setDetailOperateValueNew(routingInspectionsDetail.getGprsId());
		routingDeail.setDetailOperateType(1);// 安装主机
		routingDeail.setRequestSeq(1);// app 第一次请求
		routingDeail.setRequestType(0);// 请求
		routingInspectionDetailSer.insertSelective(routingDeail);

	}

	@Override
	public List<StationInfo> appWarnAreaSelectListSelective(SearchWarningInfoPagingVo searchWarningInfoPagingVo) {

		return stationInfoMapper.appWarnAreaSelectListSelective(searchWarningInfoPagingVo);
	}

	/**
	 * 展示不经常改变的数据
	 */
	@Override
	public StationDetail getStationDetailBasicInfoByStationId(Integer stationId) {
		StationDetail stationDetail = new StationDetail();
		StationInfo stationInfo = selectByPrimaryKey(stationId);
		if (stationInfo == null) {
			return stationDetail;
		}
		detailInfo(stationDetail, stationInfo);

		return stationDetail;
	}

	public void detailInfo(StationDetail stationDetail, StationInfo stationInfo) {
		BeanUtils.copyProperties(stationInfo, stationDetail);
		final String gprsId = stationDetail.getGprsId();
		if (!gprsId.equals("-1")) {
			GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
			queryGprsConfigInfo.setGprsId(stationInfo.getGprsId());
			List<GprsConfigInfo> configList = gprsConfigInfoSer.selectListSelective(queryGprsConfigInfo);
			GprsConfigInfo c = null;
			if (configList.size() > 0) {
				c = configList.get(0);
				// 这里要改合并代码后要查询gprs_device_typ 表来返回设备类型
				stationDetail.setDeviceType(c.getDeviceType());

			}
			PackDataExpandLatest queryPde = new PackDataExpandLatest();
			queryPde.setGprsId(gprsId);
			List<PackDataExpandLatest> pdeList = packDataExpandLatestSer.selectListSelective(queryPde);
			PackDataExpandLatest pde = null;
			if (pdeList.size() > 0) {
				pde = pdeList.get(0);
				// 电池组容量预测
				stationDetail.setPackCapPred(pde.getPackCapPred());
				// 电池组放点时长预测
				stationDetail.setPackDischargeTimePred(pde.getPackDischargeTimePred());
			}

			CellInfo queryCell = new CellInfo();
			queryCell.setStationId(stationDetail.getId());
			queryCell.setGprsId(stationDetail.getGprsId()); // 增加了GPRS_ID 的参数，解决页面显示48个单体的bug
			List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);
			List<CellInfoDetail> cellDetailList = new ArrayList<CellInfoDetail>();
			for (CellInfo cell : cellList) {
				CellInfoDetail cellDetail = new CellInfoDetail();
				BeanUtils.copyProperties(cell, cellDetail);
				if (pde != null) {
					cellDetail.setCellCapIndex(
							(Integer) BeanValueUtils.getValue("cellCapSort" + cellDetail.getCellIndex(), pde));
					cellDetail.setCellResist(
							(BigDecimal) BeanValueUtils.getValue("cellResist" + cellDetail.getCellIndex(), pde));
					cellDetail.setCellStatus(
							(Integer) BeanValueUtils.getValue("cellEvalu" + cellDetail.getCellIndex(), pde));
					cellDetail.setCellCap(
							(BigDecimal) BeanValueUtils.getValue("cellCap" + cellDetail.getCellIndex(), pde));
				}
				cellDetailList.add(cellDetail);
			}
			sortByCap(cellDetailList);
			sortByIndex(cellDetailList);
			stationDetail.setCellInfoDetailList(cellDetailList);
		}

	}

	@Override
	public StationDetail getStationDetailpackInfoByStationId(String gprsId) {
		StationDetail stationDetail = new StationDetail();
		// eg: T0B000002 base_station_info表中没有数据
		StationInfo stationInfo = new StationInfo();
		stationInfo.setGprsId(gprsId);
		packInfo(stationDetail, stationInfo);
		return stationDetail;
	}

	public void packInfo(StationDetail stationDetail, StationInfo stationInfo) {
		BeanUtils.copyProperties(stationInfo, stationDetail);
		final String gprsId = stationDetail.getGprsId();
		if (!gprsId.equals("-1")) {
			PackDataExpandLatest queryPde = new PackDataExpandLatest();
			queryPde.setGprsId(gprsId);
			List<PackDataExpandLatest> pdeList = packDataExpandLatestSer.selectListSelective(queryPde);
			PackDataExpandLatest pde = null;
			if (pdeList.size() > 0) {
				pde = pdeList.get(0);
				stationDetail.setPackCapPred(pde.getPackCapPred());
				stationDetail.setPackDischargeTimePred(pde.getPackDischargeTimePred());
			}

			PackDataInfoLatest queryPdi = new PackDataInfoLatest();
			queryPdi.setGprsId(gprsId);
			List<PackDataInfoLatest> pdiList = packDataInfoLatestSer.selectListSelective(queryPdi);
			PackDataInfoLatest pdi = null;
			if (pdiList.size() > 0) {
				pdi = pdiList.get(0);
				stationDetail.setUpdateTime(pdi.getRcvTime());
				stationDetail.setState(pdi.getState());
				stationDetail.setGenCur(pdi.getGenCur());
				stationDetail.setGenVol(pdi.getGenVol());
				stationDetail.setEnvironTem(pdi.getEnvironTem());
				stationDetail.setSoc(pdi.getSoc());
				// 加的
				// stationDetail.setPackDataInfoLatest(pdi);
			}
			CellInfo queryCell = new CellInfo();
			queryCell.setStationId(stationDetail.getId());
			queryCell.setGprsId(stationDetail.getGprsId()); // 增加了GPRS_ID 的参数，解决页面显示48个单体的bug
			List<CellInfo> cellList = cellInfoSer.selectListSelective(queryCell);
			List<CellInfoDetail> cellDetailList = new ArrayList<CellInfoDetail>();
			for (CellInfo cell : cellList) {
				CellInfoDetail cellDetail = new CellInfoDetail();
				BeanUtils.copyProperties(cell, cellDetail);
				if (pdi != null) {
					cellDetail.setCellVol(
							(BigDecimal) BeanValueUtils.getValue("cellVol" + cellDetail.getCellIndex(), pdi));
					cellDetail.setCellEqu((Byte) BeanValueUtils.getValue("cellEqu" + cellDetail.getCellIndex(), pdi));
					cellDetail
							.setCellTem((Integer) BeanValueUtils.getValue("cellTem" + cellDetail.getCellIndex(), pdi));
				} else {
					// 添加逻辑，当pdi表中没有数据时，默认cellEqu赋值为0，方便前端显示
					cellDetail.setCellEqu((byte) 0);
				}
				cellDetailList.add(cellDetail);

			}
			sortByCap(cellDetailList);
			sortByIndex(cellDetailList);
			stationDetail.setCellInfoDetailList(cellDetailList);
			// 每次更新实时时长。
			if (stationDetail.getPackDischargeTimePred() != null && stationDetail.getSoc() != null) {
				stationDetail.setRealDuration(stationDetail.getPackDischargeTimePred()
						.multiply(BigDecimal.valueOf(stationDetail.getSoc() / 100.0)));
			}

		}
	}

	@Override
	public List<StationInfo> selectStationInfoList(StationInfo stationIfno) {
		// TODO Auto-generated method stub
		return stationInfoMapper.selectStationInfoList(stationIfno);
	}

	@Override
	public void judgeCellVolLevel(StationInfo stationInfo) {
		// 判断电压平台是否一致
//		GprsConfigInfo queryInfo = new GprsConfigInfo();
//		queryInfo.setGprsId(stationInfo.getGprsId());
//		List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoSer.selectListSelective(queryInfo);
//
//		GprsDeviceType gprsDeviceType = new GprsDeviceType();
//		gprsDeviceType.setTypeCode(gprsConfigInfos.get(0).getDeviceType());
//		List<GprsDeviceType> deviceType = gprsDeviceTypeSer.selectListSelective(gprsDeviceType);
//		if (deviceType.size() != 0) {
//			if (stationInfo.getVolLevel() != deviceType.get(0).getVolLevel().intValue()) {
//				throw new RuntimeException("绑定的设备类型与当前电池组的类型、单体电压平台不匹配，保存失败！");
//			}
//		}
//		Integer cellCount = calculateCellCount(stationInfo);
//		if(cellCount != gprsConfigInfos.get(0).getSubDeviceCount()) {
//			throw new RuntimeException("绑定设备对应的从机个数和该电池组对应的单体个数不相符合，保存失败！");
//		}
		
		GprsDeviceType dviceType = gprsDeviceTypeMapper.selectVolLevelAanCellCount(stationInfo.getGprsId());
		if(dviceType == null) {
			throw new RuntimeException("设备编号不存在！");
		}
		if (stationInfo.getVolLevel() != dviceType.getVolLevel().intValue()) {
			throw new RuntimeException("绑定的设备类型与当前电池组的类型、单体电压平台不匹配，保存失败！");
		}
		Integer cellCount = calculateCellCount(stationInfo);
		if(cellCount != dviceType.getSubDeviceCount()) {
			throw new RuntimeException("绑定设备对应的从机个数和该电池组对应的单体个数不相符合，保存失败！");
		}
	}

}