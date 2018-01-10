package com.station.common.utils;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URLEncoder;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Map;

import javax.servlet.http.HttpServletResponse;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;

import net.sf.jxls.exception.ParsePropertyException;
import net.sf.jxls.transformer.XLSTransformer;

public class JxlsUtil {

	/**
	 * 下载
	 * 
	 * @param path
	 * @param name
	 * @param response
	 * @throws IOException
	 */
	public static void doDownLoad(String path, String name, HttpServletResponse response) throws IOException {

		response.reset();
		response.setHeader("Content-disposition",
				"attachment;success=true;filename =" + URLEncoder.encode(name, "utf-8"));
		response.setContentType("application/vnd.ms-excel");
		BufferedInputStream bis = null;
		BufferedOutputStream bos = null;
		OutputStream fos = null;
		InputStream fis = null;
		File uploadFile = new File(path);
		fis = new FileInputStream(uploadFile);
		bis = new BufferedInputStream(fis);
		fos = response.getOutputStream();
		bos = new BufferedOutputStream(fos);
		// 弹出下载对话框
		int bytesRead = 0;
		byte[] buffer = new byte[8192];
		while ((bytesRead = bis.read(buffer, 0, 8192)) != -1) {
			bos.write(buffer, 0, bytesRead);
		}
		bos.flush();
		fis.close();
		bis.close();
		fos.close();
		bos.close();
	}

	/**
	 * 生成excel 传入模板文件 要生成的内容 生成文件 返回生成文件的完整路径
	 * 
	 * @param srcFilePath
	 *            模板路径
	 * @param beans
	 *            生成的内容
	 * @param toFilePath
	 *            生成后文件的完整路径
	 * @return
	 */
	public static String doExcel(String srcFilePath, Map beans, String toFilePath) {
		XLSTransformer transformer = new XLSTransformer();
		try {
			transformer.transformXLS(srcFilePath, beans, toFilePath);
		} catch (ParsePropertyException e) {
			e.printStackTrace();
		} catch (InvalidFormatException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return toFilePath;
	}

	// 日期格式化
	public static String dateFmt(Date date) {
		String fmt = "yyyy-MM-dd HH:mm:ss";
		if (date == null) {
			return "";
		}
		try {
			SimpleDateFormat dateFmt = new SimpleDateFormat(fmt);
			return dateFmt.format(date);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return "";
	}

	// 分钟转化为小时
	public static Double minute2Hour(Integer min) {
		if (min != null) {
			return min*1d / 60;
		}
		return null;
	}

}