package com.station.common.utils;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.net.URLDecoder;
import java.util.Calendar;
import java.util.Date;

import org.apache.poi.ss.usermodel.Cell;
import org.apache.poi.ss.usermodel.CellStyle;
import org.apache.poi.ss.usermodel.RichTextString;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.xssf.usermodel.XSSFCell;
import org.apache.poi.xssf.usermodel.XSSFCellStyle;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;

/**
 * 共分为六部完成根据模板导出excel操作：<br/>
 * 第一步、设置excel模板路径（setSrcPath）<br/>
 * 第二步、设置要生成excel文件路径（setDesPath）<br/>
 * 第三步、设置模板中哪个Sheet列（setSheetName）<br/>
 * 第四步、获取所读取excel模板的对象（getSheet）<br/>
 * 第五步、设置数据（分为种类型数据：setCellStrValue、setCellDateValue、setCellDoubleValue、setCellBoolValue、setCellCalendarValue、
 * setCellRichTextStrValue）<br/>
 * 第六步、完成导出 （exportToNewFile）<br/>
 * 
 * @author Administrator
 * 
 */
public class ExcelUtil {
	private String srcXlsPath = ""; // // excel模板路径
	private String desXlsPath = "";
	private String sheetName = "";
	// POIFSFileSystem fs = null;
	public XSSFWorkbook wb = null;
	public XSSFSheet sheet = null;

	/**
	 * 第一步、设置excel模板路径
	 * 
	 * @param srcXlsPath
	 */
	public void setSrcPath(String srcXlsPath) {
		this.srcXlsPath = srcXlsPath;
	}

	/**
	 * 第二步、设置要生成excel文件路径
	 * 
	 * @param desXlsPath
	 */
	public void setDesPath(String desXlsPath) {
		this.desXlsPath = desXlsPath;
	}

	/**
	 * 第三步、设置模板中哪个Sheet列
	 * 
	 * @param sheetName
	 */
	public void setSheetName(String sheetName) {
		this.sheetName = sheetName;
	}

	/**
	 * 第四步、获取所读取excel模板的对象
	 */
	public void getSheet() {
		try {
			srcXlsPath = URLDecoder.decode(srcXlsPath, "UTF-8");
			File fi = new File(srcXlsPath);
			if (!fi.exists()) {
				System.out.println("模板文件:" + srcXlsPath + "不存在!");
				return;
			}
			// fs = new POIFSFileSystem(new FileInputStream(fi));
			wb = new XSSFWorkbook(new FileInputStream(fi));
			sheet = wb.getSheet(sheetName);
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	/**
	 * 第五步、设置字符串类型的数据
	 * 
	 * @param rowIndex
	 *            --行值
	 * @param cellnum
	 *            --列值
	 * @param value
	 *            --字符串类型的数据
	 */
	public void setCellStrValue(int rowIndex, int cellnum, String value) {
		XSSFCell cell = sheet.getRow(rowIndex).getCell(cellnum);
		// cell.setCellStyle(cellStyle);
		cell.setCellValue(value);
	}

	/**
	 * 第五步、设置日期/时间类型的数据
	 * 
	 * @param rowIndex
	 *            --行值
	 * @param cellnum
	 *            --列值
	 * @param value
	 *            --日期/时间类型的数据
	 */
	public void setCellDateValue(int rowIndex, int cellnum, Date value) {
		XSSFCell cell = sheet.getRow(rowIndex).getCell(cellnum);
		cell.setCellValue(value);
	}

	/**
	 * 第五步、设置浮点类型的数据
	 * 
	 * @param rowIndex
	 *            --行值
	 * @param cellnum
	 *            --列值
	 * @param value
	 *            --浮点类型的数据
	 */
	public void setCellDoubleValue(int rowIndex, int cellnum, double value) {
		XSSFCell cell = sheet.getRow(rowIndex).getCell(cellnum);
		cell.setCellValue(value);
	}

	/**
	 * 第五步、设置Bool类型的数据
	 * 
	 * @param rowIndex
	 *            --行值
	 * @param cellnum
	 *            --列值
	 * @param value
	 *            --Bool类型的数据
	 */
	public void setCellBoolValue(int rowIndex, int cellnum, boolean value) {
		XSSFCell cell = sheet.getRow(rowIndex).getCell(cellnum);
		cell.setCellValue(value);
	}

	/**
	 * 第五步、设置日历类型的数据
	 * 
	 * @param rowIndex
	 *            --行值
	 * @param cellnum
	 *            --列值
	 * @param value
	 *            --日历类型的数据
	 */
	public void setCellCalendarValue(int rowIndex, int cellnum, Calendar value) {
		XSSFCell cell = sheet.getRow(rowIndex).getCell(cellnum);
		cell.setCellValue(value);
	}

	/**
	 * 第五步、设置富文本字符串类型的数据。可以为同一个单元格内的字符串的不同部分设置不同的字体、颜色、下划线
	 * 
	 * @param rowIndex
	 *            --行值
	 * @param cellnum
	 *            --列值
	 * @param value
	 *            --富文本字符串类型的数据
	 */
	public void setCellRichTextStrValue(int rowIndex, int cellnum, RichTextString value) {
		XSSFCell cell = sheet.getRow(rowIndex).getCell(cellnum);
		cell.setCellValue(value);
	}

	public XSSFCellStyle getCellStyle(int rowIndex, int cellnum) {
		return sheet.getRow(rowIndex).getCell(cellnum).getCellStyle();
	}

	/**
	 * 第六步、完成导出
	 */
	public void exportToNewFile() {
		FileOutputStream out;
		try {
			out = new FileOutputStream(desXlsPath);
			wb.write(out);
			out.close();
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}
	
	public Row createRow(int rowIndex){
		return sheet.createRow(rowIndex);
	}
	
	public void createSetCell(Row row,int cellnum,String value,CellStyle cellStyle){
		Cell cell=row.createCell(cellnum);
		cell.setCellStyle(cellStyle);
		cell.setCellValue(value);
	}
}
