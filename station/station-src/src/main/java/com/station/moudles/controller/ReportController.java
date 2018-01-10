package com.station.moudles.controller;

import java.awt.Color;
import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URLDecoder;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Map;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.io.FileUtils;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.BorderStyle;
import org.apache.poi.ss.usermodel.FillPatternType;
import org.apache.poi.ss.usermodel.HorizontalAlignment;
import org.apache.poi.ss.usermodel.IndexedColors;
import org.apache.poi.ss.usermodel.VerticalAlignment;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.xssf.usermodel.XSSFCell;
import org.apache.poi.xssf.usermodel.XSSFCellStyle;
import org.apache.poi.xssf.usermodel.XSSFColor;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;

import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.station.common.Constant;
import com.station.common.utils.StringUtils;
import com.station.moudles.helper.ChargeEvent;
import com.station.moudles.helper.DischargeEvent;
import com.station.moudles.helper.LossChargeEvent;
import com.station.moudles.service.ModelCalculationService;
import com.station.moudles.service.ReportService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.report.ChargeDischargeReport;
import com.station.moudles.vo.report.ModelReport;
import com.station.moudles.vo.report.PulseReport;
import com.station.moudles.vo.report.PulseReportItem;
import com.station.moudles.vo.report.StationReport;
import com.station.moudles.vo.report.SuggestionReport;

import net.sf.jxls.transformer.XLSTransformer;

/**
 * Created by Jack on 9/17/2017.
 */
@Controller
@RequestMapping(value = "/report")
public class ReportController extends BaseController {

	private final ModelCalculationService modelCalculationService;
	private final ReportService reportService;

	@Autowired
	public ReportController(ModelCalculationService modelCalculationService, ReportService reportService) {
		this.modelCalculationService = modelCalculationService;
		this.reportService = reportService;
	}

	@ResponseBody
	@PostMapping(value = "station_suggestion/{station_id}")
	public AjaxResponse stationSuggestion(@PathVariable("station_id") Integer stationId) {
		if (stationId == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "基站编号必须");
		}
		SuggestionReport report = reportService.generateSuggestionReport(stationId);
		if (report == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "整治策略建议失败");
		}
		return new AjaxResponse(Constant.RS_CODE_SUCCESS, "整治策略数据生成成功", report);
	}

	@ResponseBody
	@PostMapping(value = "/charge_discharge/{station_id}")
	public AjaxResponse chargetAndDischargeReport(@PathVariable("station_id") Integer stationId,
			@RequestParam("start_time") Date startTime, @RequestParam("end_time") Date endTime) {
		ChargeDischargeReport report = reportService.getChargeDischargeReport(stationId, startTime, endTime, 
				new ChargeEvent(false), new DischargeEvent(false), new LossChargeEvent(false));
		if (report == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "获取电池组充放电记录失败");
		}
		return new AjaxResponse(Constant.RS_CODE_SUCCESS, "获取电池组充放电记录成功", report);
	}

	/**
	 * 电池组充放电记录报表
	 *
	 * @param stationId
	 * @param startTime
	 * @param endTime
	 * @return
	 * @throws IOException
	 */
	@ResponseBody
	@GetMapping(value = "/charge_discharge/{station_id}")
	public AjaxResponse exportChargeAndDischargeReport(@PathVariable("station_id") Integer stationId,
			@RequestParam("start_time") Date startTime, @RequestParam("end_time") Date endTime)
			throws IOException, InvalidFormatException {
		ChargeDischargeReport report = reportService.getChargeDischargeReport(stationId, startTime, endTime, 
				new ChargeEvent(false), new DischargeEvent(false), new LossChargeEvent(false));
		if (report == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "获取电池组充放电记录失败");
		}
		String fileName = report.getCompanyName() + report.getStationName() + "电池组充放电记录表.xlsx";
		File file = new File(generateChargeDischargeTemplateFile(report));
		return downloadAsExcelFile(fileName, file);
	}

	private String generateChargeDischargeTemplateFile(ChargeDischargeReport report)
			throws IOException, InvalidFormatException {
		String srcPath = Constant.TEMPLETE_PATH + "电池组充放电记录表.xlsx";
		srcPath = URLDecoder.decode(srcPath, "UTF-8");
		String destPath = "电池组充放电记录表" + new Date().getTime() + ".xlsx";

		List<String> sheetNames = Lists.newArrayList("电池组数据", "截止电压最低放电详情", "最近一次有效放电详情");
		try (InputStream is = new BufferedInputStream(new FileInputStream(srcPath));
				OutputStream os = new BufferedOutputStream(new FileOutputStream(destPath))) {
			Workbook workbook = new XLSTransformer().transformMultipleSheetsList(
					new BufferedInputStream(new FileInputStream(srcPath)), Lists.newArrayList(report), sheetNames,
					"result", Maps.newHashMap(), 0);
			workbook.write(os);
			os.flush();
		}
		return destPath;
	}

	/**
	 * 生成整治策略数据
	 *
	 * @param companyId
	 * @param province
	 * @param city
	 * @param district
	 * @return
	 * @throws IOException
	 * @throws InvalidFormatException
	 */
	@ResponseBody
	@PostMapping(value = "/rectification_suggestion/{company_id}/generation")
	public AjaxResponse generateRectificationSuggestion(@PathVariable("company_id") Integer companyId,
			@RequestParam(value = "province", required = false) String province,
			@RequestParam(value = "city", required = false) String city,
			@RequestParam(value = "district", required = false) String district) {
		if (companyId == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "公司编号必须");
		}
		SuggestionReport report = reportService.generateSuggestionReport(companyId, province, city, district);
		if (report == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "整治策略建议失败");
		}
		return new AjaxResponse(Constant.RS_CODE_SUCCESS, "整治策略数据生成成功");
	}

	@ResponseBody
	@PostMapping(value = "/rectification_suggestion/{company_id}")
	public AjaxResponse getRectificationSuggestion(@PathVariable("company_id") Integer companyId,
			@RequestParam(value = "province", required = false) String province,
			@RequestParam(value = "city", required = false) String city,
			@RequestParam(value = "district", required = false) String district) {
		if (companyId == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "公司编号必须");
		}
		SuggestionReport report = reportService.getSuggestionReportItems(companyId, province, city, district);
		if (report == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "整治策略建议失败");
		}
		return new AjaxResponse(Constant.RS_CODE_SUCCESS, "整治策略建议", report.getItems());
	}

	/**
	 * 整治建议报表
	 *
	 * @param companyId
	 * @param province
	 * @param city
	 * @param district
	 * @return
	 */
	@ResponseBody
	@GetMapping(value = "/rectification_suggestion/{company_id}")
	public AjaxResponse exportRectificationSuggestion(@PathVariable("company_id") Integer companyId,
			@RequestParam(value = "province", required = false) String province,
			@RequestParam(value = "city", required = false) String city,
			@RequestParam(value = "district", required = false) String district)
			throws IOException, InvalidFormatException {
		if (companyId == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "公司编号必须");
		}
		SuggestionReport report = reportService.getSuggestionReportItems(companyId, province, city, district);
		if (report == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "整治策略建议失败");
		}
		String fileName = report.getCompanyName() + "电池组整冶建议报表.xlsx";
		File file = new File(generateSuggestionTemplateFile(report));
		return downloadAsExcelFile(fileName, file);
	}

	private String generateSuggestionTemplateFile(SuggestionReport report) throws IOException, InvalidFormatException {
		String srcPath = Constant.TEMPLETE_PATH + "市区级电池组整冶建议报表.xlsx";
		srcPath = URLDecoder.decode(srcPath, "UTF-8");
		String destPath = "市区级电池组整冶建议报表" + new Date().getTime() + ".xlsx";

		try (InputStream is = new BufferedInputStream(new FileInputStream(srcPath));
				OutputStream os = new BufferedOutputStream(new FileOutputStream(destPath))) {
			Map beanParams = Maps.newHashMap();
			beanParams.put("result", report);
			XSSFWorkbook workbook = (XSSFWorkbook) new XLSTransformer().transformXLS(is, beanParams);
			XSSFSheet sheet = workbook.getSheetAt(0);
			if (CollectionUtils.isNotEmpty(report.getItems())) {
				XSSFCellStyle style = workbook.createCellStyle();
				style.setFillForegroundColor(new XSSFColor(new Color(220, 230, 241)));
				style.setFillPattern(FillPatternType.SOLID_FOREGROUND);
				style.setAlignment(HorizontalAlignment.CENTER);
				style.setVerticalAlignment(VerticalAlignment.CENTER);
				for (int i = 4; i < sheet.getLastRowNum(); i++) {
					if (i % 2 == 0) {
						continue;
					}
					XSSFRow row = sheet.getRow(i);
					for (int j = 1; j < 10; j++) {
						XSSFCell cell = row.getCell(j);
						if (cell != null) {
							cell.setCellStyle(style);
						}
					}
				}
			}
			workbook.write(os);
			os.flush();
		}
		return destPath;
	}

	@ResponseBody
	@GetMapping(value = "/pulse_test/{company_id}")
	public AjaxResponse exportPulseTest(@PathVariable("company_id") Integer companyId,
			@RequestParam("start_time") Date startTime, @RequestParam("end_time") Date endTime)
			throws IOException, InvalidFormatException {
		if (companyId == null || startTime == null || endTime == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "公司编号和时间范围必须");
		}
		PulseReport pulseReport = reportService.generatePulseReport(companyId, startTime, endTime);
		if (pulseReport == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "该时间段内没有数据");
		}

		String fileName = pulseReport.getCompanyName() + "特征测试统计报表.xlsx";
		File file = new File(generatePulseTestTemplateFile(pulseReport));
		return downloadAsExcelFile(fileName, file);
	}

	@ResponseBody
	@GetMapping(value = "/model_calculation/{company_id}")
	public AjaxResponse exportModelCalculation(@PathVariable("company_id") Integer companyId)
			throws IOException, InvalidFormatException {
		if (companyId == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "请输入公司编号");
		}
		ModelReport modelReport = modelCalculationService.generateModelReport(companyId);
		if (modelReport == null) {
			return new AjaxResponse<>(Constant.RS_CODE_ERROR, "请输入有效的公司编号");
		}
		String name = "模型计算统计报表";
		String fileName = modelReport.getCompanyName() + name + ".xlsx";
		File file = new File(generateTemplateFile(modelReport, name));
		return downloadAsExcelFile(fileName, file);
	}

	private AjaxResponse downloadAsExcelFile(String fileName, File file) throws IOException {
		// 设置response
		fileName = new String(fileName.getBytes("UTF-8"), "iso-8859-1");
		response.setContentType("application/x-msdownload; charset=UTF-8");
		response.addHeader("content-type", "application/x-msdownload;");
		response.addHeader("content-disposition", "attachment; filename=" + fileName);
		response.setContentLength((int) file.length());
		try (OutputStream os = new BufferedOutputStream(response.getOutputStream())) {
			os.write(FileUtils.readFileToByteArray(file));
			os.flush();
		} finally {
			FileUtils.deleteQuietly(file);
		}
		return new AjaxResponse();
	}

	private <T> String generateTemplateFile(T t, String name) throws IOException, InvalidFormatException {
		String srcPath = Constant.TEMPLETE_PATH + name + ".xlsx";
		srcPath = URLDecoder.decode(srcPath, "UTF-8");
		String destPath = name + "_" + new Date().getTime() + ".xlsx";
		Map beanParams = Maps.newHashMap();
		beanParams.put("result", t);
		new XLSTransformer().transformXLS(srcPath, beanParams, destPath);
		return destPath;
	}

	private String generatePulseTestTemplateFile(PulseReport pulseReport) throws IOException, InvalidFormatException {
		String srcPath = Constant.TEMPLETE_PATH + "特征测试统计.xlsx";
		srcPath = URLDecoder.decode(srcPath, "UTF-8");
		String destPath = "特征测试统计" + new Date().getTime() + ".xlsx";

		try (InputStream is = new BufferedInputStream(new FileInputStream(srcPath));
				OutputStream os = new BufferedOutputStream(new FileOutputStream(destPath))) {
			Map beanParams = Maps.newHashMap();
			beanParams.put("result", pulseReport);
			XSSFWorkbook workbook = (XSSFWorkbook) new XLSTransformer().transformXLS(is, beanParams);
			XSSFSheet sheet = workbook.getSheetAt(0);

			if (CollectionUtils.isNotEmpty(pulseReport.getItems())) {

				XSSFCellStyle defaultStyle = workbook.createCellStyle();
				defaultStyle.setFillForegroundColor(new XSSFColor(new Color(255, 215, 0)));
				defaultStyle.setFillPattern(FillPatternType.SOLID_FOREGROUND);
				defaultStyle.setBorderRight(BorderStyle.THIN);
				defaultStyle.setRightBorderColor(IndexedColors.BLACK.getIndex());
				defaultStyle.setBorderLeft(BorderStyle.THIN);
				defaultStyle.setLeftBorderColor(IndexedColors.BLACK.getIndex());
				defaultStyle.setBorderTop(BorderStyle.THIN);
				defaultStyle.setTopBorderColor(IndexedColors.BLACK.getIndex());
				defaultStyle.setBorderBottom(BorderStyle.THIN);
				defaultStyle.setBottomBorderColor(IndexedColors.BLACK.getIndex());

				XSSFCellStyle failedStyle = workbook.createCellStyle();
				failedStyle.setFillForegroundColor(IndexedColors.RED.getIndex());
				failedStyle.setFillPattern(FillPatternType.SOLID_FOREGROUND);
				failedStyle.setBorderRight(BorderStyle.THIN);
				failedStyle.setRightBorderColor(IndexedColors.BLACK.getIndex());
				failedStyle.setBorderLeft(BorderStyle.THIN);
				failedStyle.setLeftBorderColor(IndexedColors.BLACK.getIndex());
				failedStyle.setBorderTop(BorderStyle.THIN);
				failedStyle.setTopBorderColor(IndexedColors.BLACK.getIndex());
				failedStyle.setBorderBottom(BorderStyle.THIN);
				failedStyle.setBottomBorderColor(IndexedColors.BLACK.getIndex());

				XSSFCellStyle successStyle = workbook.createCellStyle();
				successStyle.setFillForegroundColor(new XSSFColor(new Color(245, 245, 245)));
				successStyle.setFillPattern(FillPatternType.SOLID_FOREGROUND);
				successStyle.setBorderRight(BorderStyle.THIN);
				successStyle.setRightBorderColor(IndexedColors.BLACK.getIndex());
				successStyle.setBorderLeft(BorderStyle.THIN);
				successStyle.setLeftBorderColor(IndexedColors.BLACK.getIndex());
				successStyle.setBorderTop(BorderStyle.THIN);
				successStyle.setTopBorderColor(IndexedColors.BLACK.getIndex());
				successStyle.setBorderBottom(BorderStyle.THIN);
				successStyle.setBottomBorderColor(IndexedColors.BLACK.getIndex());

				int rowIndex = 5;
				for (PulseReportItem item : pulseReport.getItems()) {
					XSSFRow row = sheet.getRow(rowIndex);
					int lastCellNum = row.getLastCellNum();
					for (int j = 2; j < lastCellNum; j++) {
						XSSFCell cell = row.getCell(j);
						XSSFCellStyle style = defaultStyle;
						// 0未发送,1发送成功，2特征执行成功，3特征执行失败
						Byte status = item.getCellStatusMap().get(j - 1);
						if (status == null) {
							status = 0;
						}
						int value = status.intValue();
						if (value == 2) {
							style = successStyle;
						} else if (value == 3) {
							style = failedStyle;
						}
						cell.setCellStyle(style);
					}

					rowIndex++;
				}
			}
			workbook.write(os);
			os.flush();
		}
		return destPath;
	}

	@RequestMapping(value = "/maintainSuggestion", method = RequestMethod.POST)
	@ResponseBody
	public AjaxResponse<StationReport> generateStationVolCurStr(@RequestBody StationReport stationReport) {
		AjaxResponse<StationReport> ajaxResponse = new AjaxResponse<StationReport>(Constant.RS_CODE_ERROR, "请设置公司id！");
		if (stationReport.getCompanyId3() == null) {
			return ajaxResponse;
		}
		try {
			// 需求是，传两天日期，要查询两天的数据。结束时间加一天
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(stationReport.getEndRcvTime());
			calendar.add(Calendar.DATE, 1);
			calendar.add(Calendar.SECOND, -1);
			stationReport.setEndRcvTime(calendar.getTime());
			StationReport report = reportService.generateStationVolCurStr(stationReport);
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg(Constant.RS_MSG_SUCCESS);
			ajaxResponse.setData(report);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg(Constant.RS_MSG_ERROR);
		}
		return ajaxResponse;
	}
	

	@RequestMapping(value ="/maintainSuggestion/download",method = RequestMethod.GET)
	@ResponseBody
	public AjaxResponse<StationReport> downloadMaintainSuggestion(@RequestParam("companyId3") Integer companyId3,
																@RequestParam("companyName") String companyName,
																@RequestParam("startRcvTime") Date startRcvTime,
																@RequestParam("endRcvTime") Date endRcvTime,
																@RequestParam("linkStatus") Byte linkStatus,
																@RequestParam("state") String state){
		AjaxResponse<StationReport> ajaxResponse = new AjaxResponse<StationReport>(Constant.RS_CODE_ERROR, "请设置公司id！");
		if (companyId3 == null) {
			return ajaxResponse;
		}
		try {
			// 需求是，传两天日期，要查询两天的数据。结束时间加一天
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(endRcvTime);
			calendar.add(Calendar.DATE, 1);
			calendar.add(Calendar.SECOND, -1);
			StationReport stationReport = new StationReport();
			stationReport.setCompanyId3(companyId3);
			stationReport.setCompanyName(companyName);
			stationReport.setStartRcvTime(startRcvTime);
			stationReport.setEndRcvTime(calendar.getTime());
			stationReport.setLinkStatus(linkStatus);
			stationReport.setState(StringUtils.isNull(state) ? null:state);
			StationReport report = reportService.generateStationVolCurStr(stationReport);
			String name = "设备维护数据报表";
			String fileName = report.getCompanyName() + name + ".xlsx";
			File file = new File(generateTemplateFile(report, name));
			downloadAsExcelFile(fileName, file);
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			ajaxResponse.setMsg(Constant.RS_MSG_SUCCESS);
			ajaxResponse.setData(report);
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg(Constant.RS_MSG_ERROR);
		}
		return ajaxResponse;
	}
	
}
