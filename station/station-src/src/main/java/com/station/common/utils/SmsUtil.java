package com.station.common.utils;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLConnection;
import java.net.URLEncoder;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class SmsUtil {

	private final static Logger log = LoggerFactory.getLogger(SmsUtil.class);
	// 用户名
	private static final String name = "dxwimc";
	// 密码
	private static final String pwd = "B1BE51033DABE9A1533264E8CC75";
	// 签名
	private static final String sign = "实验宝";
	// 追加发送时间，可为空，为空为及时发送
	private static final String stime = "";
	// 扩展码，必须为数字 可为空
	private static final StringBuffer extno = new StringBuffer();

	public static void main(String[] args) throws Exception {
		sendSms("15884579559", "test");
		System.out.println();
	}

	public static boolean sendSms(String phone, String content) {
		try {
			log.info("content="+content);
			String result = doPost(name, pwd, phone, content, sign, stime, extno);
			String resultCode = result.split(",")[0];
			if (resultCode.equals("0")) {				
				return true;
			} else {
				 log.error("sendSms error", result);
				 return false;
			}
		} catch (Exception e) {
			e.printStackTrace();
			return false;
		}
	}

	/**
	 * 发送短信
	 * 
	 * @param name
	 *            用户名
	 * @param pwd
	 *            密码
	 * @param mobileString
	 *            电话号码字符串，中间用英文逗号间隔
	 * @param contextString
	 *            内容字符串
	 * @param sign
	 *            签名
	 * @param stime
	 *            追加发送时间，可为空，为空为及时发送
	 * @param extno
	 *            扩展码，必须为数字 可为空
	 * @return
	 * @throws Exception
	 */
	public static String doPost(String name, String pwd, String mobileString, String contextString, String sign,
			String stime, StringBuffer extno) throws Exception {
		StringBuffer param = new StringBuffer();
		param.append("name=" + name);
		param.append("&pwd=" + pwd);
		param.append("&mobile=").append(mobileString);
		param.append("&content=").append(URLEncoder.encode(contextString, "UTF-8"));
		param.append("&stime=" + stime);
		param.append("&sign=").append(URLEncoder.encode(sign, "UTF-8"));
		param.append("&type=pt");
		param.append("&extno=").append(extno);

		URL localURL = new URL("http://web.duanxinwang.cc/asmx/smsservice.aspx?");
		URLConnection connection = localURL.openConnection();
		HttpURLConnection httpURLConnection = (HttpURLConnection) connection;

		httpURLConnection.setDoOutput(true);
		httpURLConnection.setRequestMethod("POST");
		httpURLConnection.setRequestProperty("Accept-Charset", "utf-8");
		httpURLConnection.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
		httpURLConnection.setRequestProperty("Content-Length", String.valueOf(param.length()));

		OutputStream outputStream = null;
		OutputStreamWriter outputStreamWriter = null;
		InputStream inputStream = null;
		InputStreamReader inputStreamReader = null;
		BufferedReader reader = null;
		String resultBuffer = "";

		try {
			outputStream = httpURLConnection.getOutputStream();
			outputStreamWriter = new OutputStreamWriter(outputStream);

			outputStreamWriter.write(param.toString());
			outputStreamWriter.flush();

			if (httpURLConnection.getResponseCode() >= 300) {
				throw new Exception(
						"HTTP Request is not success, Response code is " + httpURLConnection.getResponseCode());
			}

			inputStream = httpURLConnection.getInputStream();
			resultBuffer = convertStreamToString(inputStream);

		} finally {

			if (outputStreamWriter != null) {
				outputStreamWriter.close();
			}

			if (outputStream != null) {
				outputStream.close();
			}

			if (reader != null) {
				reader.close();
			}

			if (inputStreamReader != null) {
				inputStreamReader.close();
			}

			if (inputStream != null) {
				inputStream.close();
			}

		}

		return resultBuffer;
	}

	/**
	 * 转换返回值类型为UTF-8格式.
	 * 
	 * @param is
	 * @return
	 */
	public static String convertStreamToString(InputStream is) {
		StringBuilder sb1 = new StringBuilder();
		byte[] bytes = new byte[4096];
		int size = 0;

		try {
			while ((size = is.read(bytes)) > 0) {
				String str = new String(bytes, 0, size, "UTF-8");
				sb1.append(str);
			}
		} catch (IOException e) {
			e.printStackTrace();
		} finally {
			try {
				is.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return sb1.toString();
	}
}
