package com.station.moudles.service.impl;

import java.io.IOException;
import java.math.BigDecimal;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import org.apache.commons.collections.CollectionUtils;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.util.CellRangeAddress;
import org.apache.poi.xssf.usermodel.XSSFCellStyle;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.common.Constant;
import com.station.common.utils.CommonConvertUtils;
import com.station.common.utils.ExcelUtil;
import com.station.common.utils.MyDateUtils;
import com.station.moudles.entity.Company;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.StationWarningInfo;
import com.station.moudles.entity.WarnArea;
import com.station.moudles.entity.WarningInfo;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.WarningInfoMapper;
import com.station.moudles.service.CompanyService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.WarningInfoService;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.report.WarnReport;

import net.sf.jxls.exception.ParsePropertyException;
import net.sf.jxls.transformer.XLSTransformer;

@Service
public class WarningInfoServiceImpl extends BaseServiceImpl<WarningInfo, Integer> implements WarningInfoService {
	@Autowired
	WarningInfoMapper warningInfoMapper;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	CompanyService companySer;
	@Autowired
	GprsConfigInfoMapper gprsConfigInfoMapper;

	@Override
	public List<StationWarningInfo> selectWarningList(CommonSearchVo commonSearch) {
		List<StationWarningInfo> resultList = new ArrayList<StationWarningInfo>();
		StationInfo stationInfo = new StationInfo();
		CommonConvertUtils.convertCommonToStation(commonSearch, stationInfo);
		List<StationInfo> stationList = stationInfoSer.selectListSelective(stationInfo);
		for(int i=stationList.size()-1;i>0;i--){
			StationInfo s=stationList.get(i);
			if(s.getGprsId().equals("-1")){
				stationList.remove(i);
			}
		}
		if (stationList.size() > 0) {
			resultList = selectWarningListByStation(stationList);
		}
		return resultList;
	}

	public List<StationWarningInfo> selectWarningListByStation(List<StationInfo> stationList) {
		Map m = new HashMap();
		m.put("stationList", stationList);
		m.put("rcvTime", MyDateUtils.getDiffTime(-1 * 60 * 1000));
		List<StationWarningInfo> resultList = warningInfoMapper.selectWarningListByStation(m);
		if (resultList.size() > 0) {
			for (StationWarningInfo sw : resultList) {
				sw.setOperatorTypeStr(CommonConvertUtils.convertToStationOperatorTypeStr(sw.getOperatorType()));
				if (sw.getLossElectricity().equals((byte) 1)) {
					sw.setLossElectricityStr("掉电");
				}
				if (sw.getCellTemHigh().equals((byte) 1)) {
					sw.setCellTemStr("过高");
				} else if (sw.getCellTemLow().equals((byte) 1)) {
					sw.setCellTemStr("过低");
				}
				if (sw.getEnvTemHigh().equals((byte) 1)) {
					sw.setEnvTemStr("过高");
				} else if (sw.getEnvTemLow().equals((byte) 1)) {
					sw.setEnvTemStr("过低");
				}
				if (sw.getGenVolHigh().equals((byte) 1)) {
					sw.setGenVolStr("过高");
				} else if (sw.getGenVolLow().equals((byte) 1)) {
					sw.setGenVolStr("欠压");
				}
				if (sw.getSocLow().equals((byte) 1)) {
					sw.setSocStr("过低");
				}
			}
		}
		return resultList;
	}

	@Override
	public List<WarnArea> selectWarnAreaList(CommonSearchVo commonSearchVo) {
		Map<String, Object> m = new HashMap<String, Object>();
		m.put("rcvTime", MyDateUtils.getDiffTime(-1 * 60 * 1000));
		if (commonSearchVo.getCompanyLevel() != null) {
			switch (commonSearchVo.getCompanyLevel()) {
			case 1:
				m.put("companyId1", commonSearchVo.getCompanyId());
				break;
			case 2:
				m.put("companyId2", commonSearchVo.getCompanyId());
				break;
			case 3:
				m.put("companyId3", commonSearchVo.getCompanyId());
				break;
			default:
				m.put("companyId1", commonSearchVo.getCompanyId());
				break;
			}
		}
		return warningInfoMapper.selectWarnAreaList(m);
	}

	@Override
	public String exportToExcel(List<StationWarningInfo> stationWarnList , WarnReport warnReport) {
		// String tplPath =
		// "E:/workspace/station/src/main/resources/templete/区域级实时告警信息报表.xlsx";
//		WarnReport warnReport = getWarnReport(commonSearchVo);
		String tplPath = Constant.TEMPLETE_PATH + "区域级实时告警信息报表.xlsx";
		String destPath = new Date().getTime() + ".xlsx";
		Map beanParams = new HashMap();
		beanParams.put("stationWarnList", stationWarnList);
		beanParams.put("wr", warnReport);
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

	public WarnReport getWarnReport(CommonSearchVo commonSearchVo) {
		WarnReport warnReport = new WarnReport();
		warnReport.setExportDateTime(MyDateUtils.getDateString(new Date()));
		Company company =  companySer.selectByPrimaryKey(commonSearchVo.getCompanyId());
		String companyAndArea = company != null ? company.getCompanyName() : "";
		warnReport.setCompany(companyAndArea);
		String area = (commonSearchVo.getProvince() == null ? "":commonSearchVo.getProvince())
				+ (commonSearchVo.getCity() == null ? "":commonSearchVo.getCity())
				+ (commonSearchVo.getDistrict() == null ? "":commonSearchVo.getDistrict());
		companyAndArea = companyAndArea + area;
		warnReport.setCompanyAndArea(companyAndArea);
		StationInfo stationInfo = new StationInfo();
		CommonConvertUtils.convertCommonToStation(commonSearchVo, stationInfo);
		List<StationInfo> stationList = stationInfoSer.selectListSelective(stationInfo);
		if (!CollectionUtils.isEmpty(stationList)) {
			stationList = stationList.stream().filter(s -> !s.getGprsId().equals("-1"))
					.collect(Collectors.toList());//.get(0).getGprsId();
			if (!CollectionUtils.isEmpty(stationList)) {
				String gprsId = stationList.get(0).getGprsId();
				GprsConfigInfo query = new GprsConfigInfo();
				query.setGprsId(gprsId);
				List<GprsConfigInfo> selectListSelective = gprsConfigInfoMapper.selectListSelective(query);
				if (!CollectionUtils.isEmpty(selectListSelective)) {
					BeanUtils.copyProperties(selectListSelective.get(0), warnReport);
				}
			}
		}
		return warnReport;
	}

	@Override
	public String exportWarnAreaToExcel(List<WarnArea> warnAreaList, WarnReport warnReport) {
		String destPath = new Date().getTime() + ".xlsx";
		String tplPath = Constant.TEMPLETE_PATH + "区域电池组告警信息统计表.xlsx";
		ExcelUtil excel = new ExcelUtil();
		excel.setSrcPath(tplPath);
		excel.setDesPath(destPath);
		excel.setSheetName("区域电池组告警信息统计表");
		excel.getSheet();
		excel.setCellStrValue(1, 8, "导出时间:" + warnReport.getExportDateTime());
		excel.setCellStrValue(1, 0, "报表公司:" + warnReport.getCompany());
		String standard = "总电压过高告警阈值"+ warnReport.getVolHighWarningThreshold() + "V"
							+ "，总电压过低告警阈值"+ warnReport.getVolLowWarningThreshold() +"V"
							+ "；温度过高告警阈值 "+ warnReport.getTemHighWarningThreshold() +"℃"
							+ "，温度过低告警阈值 "+ warnReport.getTemLowWarningThreshold()+"℃"
							+ "；SOC过低告警阈值 "+warnReport.getSocLowWarningThreshold();
		excel.setCellStrValue(2, 0, "本次告警标准："+standard);
		int rowIndex = 4;
		XSSFCellStyle cellStyle = excel.getCellStyle(rowIndex, 0);
		int firstRow = rowIndex;
		int lastRow = 0;
		for (int i = 0; i < warnAreaList.size(); i++) {
			WarnArea wa = warnAreaList.get(i);
			if (wa.getCity().equals(wa.getDistrict())) {
				lastRow = rowIndex + i;
				excel.sheet.addMergedRegion(new CellRangeAddress(firstRow, lastRow, 0, 0));
				firstRow = lastRow + 1;
			}
			Row row = excel.createRow(rowIndex + i);
			excel.createSetCell(row, 0, wa.getCompanyName3(), cellStyle);
			excel.createSetCell(row, 1, wa.getDistrict(), cellStyle);
			excel.createSetCell(row, 2, numInteger(wa.getNum()) + "", cellStyle);
			excel.createSetCell(row, 3, numInteger(wa.getLossElectricityNum()) + "", cellStyle);
			excel.createSetCell(row, 4, numString(wa.getLossElectricityPercent()) + "%", cellStyle);
			excel.createSetCell(row, 5, (numInteger(wa.getCellTemLowNum()) + numInteger(wa.getCellTemHighNum())) + "", cellStyle);
			excel.createSetCell(row, 6,
					(new BigDecimal(numString(wa.getCellTemHighPercent())).add(new BigDecimal(numString(wa.getCellTemLowPercent())))) + "%",
					cellStyle);
			
			excel.createSetCell(row, 7, (numInteger(wa.getGenVolHighNum()) + numInteger(wa.getGenVolLowNum())) + "", cellStyle);
			excel.createSetCell(row, 8,
					(new BigDecimal(numString(wa.getGenVolHighPercent())).add(new BigDecimal(numString(wa.getGenVolLowPercent())))) + "%",
					cellStyle);
			
			excel.createSetCell(row, 9, (numInteger(wa.getEnvTemHighNum()) + numInteger(wa.getEnvTemLowNum())) + "", cellStyle);
			excel.createSetCell(row, 10,
					(new BigDecimal(numString(wa.getEnvTemHighPercent())).add(new BigDecimal(numString(wa.getEnvTemLowPercent())))) + "%",
					cellStyle);
			
			excel.createSetCell(row, 11, numInteger(wa.getSocLowNum()) + "", cellStyle);
			excel.createSetCell(row, 12, numString(wa.getSocLowPercent()) + "%", cellStyle);
			
		}
		excel.exportToNewFile();
		return destPath;
	}
	
	private Integer numInteger(Integer integer) {
		if (integer == null) {
			return 0;
		}
		return integer;
	}
	
	private String numString(String string) {
		if (string == null || string.trim().isEmpty()) {
			return "0";
		}
		return string;
	}

	public static void main(String[] args) {
		StationWarningInfo sw = new StationWarningInfo();
		sw.setLossElectricity((byte) 1);
	}

	@Override
	public List<WarnArea> appSelectWarnAreaList(CommonSearchVo commonSearchVo) {
		Map<String, Object> m = new HashMap<String, Object>();
		m.put("rcvTime", MyDateUtils.getDiffTime(-1 * 60 * 1000));
		if (commonSearchVo.getCompanyLevel() != null) {
			switch (commonSearchVo.getCompanyLevel()) {
			case 1:
				m.put("companyId1", commonSearchVo.getCompanyId());
				break;
			case 2:
				m.put("companyId2", commonSearchVo.getCompanyId());
				break;
			case 3:
				m.put("companyId3", commonSearchVo.getCompanyId());
				break;
			default:
				m.put("companyId1", commonSearchVo.getCompanyId());
				break;
			}
		}
		return warningInfoMapper.appSelectWarnAreaList(m);
	}
}