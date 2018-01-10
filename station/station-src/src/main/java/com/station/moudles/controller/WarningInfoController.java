package com.station.moudles.controller;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.StationWarningInfo;
import com.station.moudles.entity.WarnArea;
import com.station.moudles.service.impl.WarningInfoServiceImpl;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.report.WarnReport;

import io.swagger.annotations.ApiOperation;

/**
 * This class was generated by Bill Generator. This class corresponds to the
 * database table warning_info 基站报警表
 *
 * @zdmgenerated 2017-47-24 07:47
 */
@Controller
@RequestMapping(value = "/warningInfo")
public class WarningInfoController extends BaseController {
	/*
	 * @Autowired WarningInfoService warningInfoSer;
	 */
	@Autowired
	WarningInfoServiceImpl warningInfoSer;

	@RequestMapping(value = "/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取基站报警表列表", notes = "返回基站报警表列表")
	public AjaxResponse<List<StationWarningInfo>> getWarningInfoList(@RequestBody CommonSearchVo commonSearchVo) {
		AjaxResponse<List<StationWarningInfo>> ajaxResponse = validateBean(commonSearchVo);
		if (ajaxResponse == null) {
			List<StationWarningInfo> warningInfoList = warningInfoSer.selectWarningList(commonSearchVo);
			//暂时过滤掉温度的告警
			warningInfoList = warningInfoList.stream()
					.filter(k -> !k.getGprsId().startsWith("Y0A"))
					.filter(k -> k.getGenVolHigh() > 0 || k.getGenVolLow() > 0 || k.getLossElectricity() > 0 || k.getSocLow() > 0)
					.map(k -> {k.setCellTemHigh((byte)0);k.setCellTemLow((byte)0);k.setEnvTemHigh((byte)0);k.setEnvTemLow((byte)0);return k;})
//				.filter(k->!StringUtils.isNull(k.getLossElectricityStr()) || !StringUtils.isNull(k.getGenVolStr()) 
//						|| !StringUtils.isNull(k.getSocStr()))
//				.map(k -> {k.setCellTemStr(null);k.setEnvTemStr(null);return k;})
				.collect(Collectors.toList());
			ajaxResponse = new AjaxResponse<List<StationWarningInfo>>(warningInfoList);
		}
		return ajaxResponse;
	}

	@ResponseBody
	@RequestMapping(value = "/exportRealTime", method = RequestMethod.GET)
	public AjaxResponse download(CommonSearchVo commonSearchVo) throws IOException {
		AjaxResponse<List<StationWarningInfo>> ajaxResponse = validateBean(commonSearchVo);
		if (ajaxResponse == null) {
			List<StationWarningInfo> warningInfoList = warningInfoSer.selectWarningList(commonSearchVo);
			WarnReport warnReport = warningInfoSer.getWarnReport(commonSearchVo);
			String filePath = warningInfoSer.exportToExcel(warningInfoList , warnReport);
			File file = new File(filePath);
			String fileName = new String((warnReport.getCompany() + "_实时告警信息报表.xlsx").getBytes("UTF-8"), "iso-8859-1");
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
			}
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/warnArea/list", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据条件获取基站区域报警表列表", notes = "返回基站区域报警列表")
	public AjaxResponse<List<WarnArea>> getWarnAreaList(@RequestBody CommonSearchVo commonSearchVo) {
		AjaxResponse<List<WarnArea>> ajaxResponse = validateBean(commonSearchVo);
		if (ajaxResponse == null) {
			List<WarnArea> warningInfoList = warningInfoSer.selectWarnAreaList(commonSearchVo);
			ajaxResponse = new AjaxResponse<List<WarnArea>>(warningInfoList);
		}
		return ajaxResponse;
	}

	@ResponseBody
	@RequestMapping(value = "/exportWarnArea", method = RequestMethod.GET)
	public AjaxResponse downloadWarnArea(CommonSearchVo commonSearchVo) throws IOException {
		AjaxResponse<List<StationWarningInfo>> ajaxResponse = validateBean(commonSearchVo);
		if (ajaxResponse == null) {
			List<WarnArea> warningInfoList = warningInfoSer.selectWarnAreaList(commonSearchVo);
			WarnReport warnReport = warningInfoSer.getWarnReport(commonSearchVo);
			String filePath = warningInfoSer.exportWarnAreaToExcel(warningInfoList, warnReport);
			File file = new File(filePath);
			String fileName = new String((warnReport.getCompany() + "_告警信息统计报表.xlsx").getBytes("UTF-8"), "iso-8859-1");
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
			}
		}
		return ajaxResponse;
	}

	@RequestMapping(value = "/station/{gprsId}", method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据GPRSid获取基站报警信息", notes = "返回基站报警表")
	public AjaxResponse<List<StationWarningInfo>> getWarningInfo(@PathVariable String gprsId) {
		if (gprsId == null) {
			return new AjaxResponse<List<StationWarningInfo>>(Constant.RS_CODE_ERROR, "gprsId为空！");
		}

		StationInfo info = new StationInfo();
		info.setGprsId(gprsId);
		List<StationInfo> stationInfos = new ArrayList<>();
		stationInfos.add(info);

		AjaxResponse<List<StationWarningInfo>> ajaxResponse = null;
		if (ajaxResponse == null) {
			List<StationWarningInfo> warningInfoList = warningInfoSer.selectWarningListByStation(stationInfos);
			//暂时过滤掉温度的告警
			warningInfoList = warningInfoList.stream()
					.filter(k -> !k.getGprsId().startsWith("Y0A"))
					.filter(k -> k.getGenVolHigh() > 0 || k.getGenVolLow() > 0 || k.getLossElectricity() > 0 || k.getSocLow() > 0)
					.map(k -> {k.setCellTemHigh((byte)0);k.setCellTemLow((byte)0);k.setEnvTemHigh((byte)0);k.setEnvTemLow((byte)0);return k;})
//				.filter(k->!StringUtils.isNull(k.getLossElectricityStr()) || !StringUtils.isNull(k.getGenVolStr()) 
//						|| !StringUtils.isNull(k.getSocStr()))
//				.map(k -> {k.setCellTemStr(null);k.setEnvTemStr(null);return k;})
				.collect(Collectors.toList());
			ajaxResponse = new AjaxResponse<List<StationWarningInfo>>(warningInfoList);
		}
		return ajaxResponse;
	}

}