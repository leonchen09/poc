package com.station.common.utils;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.util.Iterator;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;

/**
 * http请求工具类，暂只支持POST请求,数据json结构，编码UTF-8
 * 
 * @author lif
 *
 */
public class HttpClientUtil {
	private static String ENCODE = "UTF-8";

	/**
	 * 发送POST请求
	 * 
	 * @param url
	 *            url地址
	 * @param data
	 * @param headData
	 *            自定义头文件数据
	 * @param useSSL
	 *            是否加密,true表示加密
	 */
	public static String doClient(String url, String data, Map<String, String> headData, boolean useSSL, String type) {
		try {
			// 建立连接
			URL httpUrl = useSSL ? new URL("https://" + url) : new URL("http://" + url);
			HttpURLConnection httpConn = (HttpURLConnection) httpUrl.openConnection();
			// //设置连接属性
			httpConn.setDoOutput(true);// 使用 URL 连接进行输出
			httpConn.setDoInput(true);// 使用 URL 连接进行输入
			httpConn.setUseCaches(false);// 忽略缓存
			httpConn.setRequestMethod(type);// 设置URL请求方法
			// 设置请求属性
			// 获得数据字节数据，请求数据流的编码，必须和下面服务器端处理请求流的编码一致
			byte[] requestStringBytes = null;
			if (data != null) {
				requestStringBytes = data.getBytes(ENCODE);
				httpConn.setRequestProperty("Content-length", "" + requestStringBytes.length);
			} else {
				httpConn.setRequestProperty("Content-length", "0");
			}
			// httpConn.setRequestProperty("Content-Type",
			// "application/octet-stream");
			httpConn.setRequestProperty("Content-Type", "application/json");
			httpConn.setRequestProperty("Connection", "Keep-Alive");// 维持长连接
			httpConn.setRequestProperty("Charset", "UTF-8");
			// 设置自定义头信息
			if (null != headData) {
				Iterator<String> iteKey = headData.keySet().iterator();
				while (iteKey.hasNext()) {
					String headKey = iteKey.next();
					httpConn.setRequestProperty(URLEncoder.encode(headKey, "utf-8"),
							URLEncoder.encode(headData.get(headKey), "utf-8"));
				}
			}
			// 建立输出流，并写入数据
			OutputStream outputStream = httpConn.getOutputStream();
			outputStream.write(requestStringBytes);
			outputStream.close();
			// 获得响应状态
			int responseCode = httpConn.getResponseCode();
			if (HttpURLConnection.HTTP_OK == responseCode) {// 连接成功
				// 当正确响应时处理数据
				StringBuffer sb = new StringBuffer();
				String readLine;
				BufferedReader responseReader;
				// 处理响应流，必须与服务器响应流输出的编码一致
				responseReader = new BufferedReader(new InputStreamReader(httpConn.getInputStream(), ENCODE));
				while ((readLine = responseReader.readLine()) != null) {
					sb.append(readLine).append("\n");
				}
				responseReader.close();
				// System.out.println(sb.toString());
				return sb.toString();
			} else {
				System.out.println("http status:" + responseCode);
			}
		} catch (Exception ex) {
			ex.printStackTrace();
		}
		return null;
	}

	/**
	 * 获取用户真实ip
	 * 
	 * @param request
	 * @return
	 */
	public static String getIpAddr(HttpServletRequest request) {
		String ip = request.getHeader("x-forwarded-for");
		if (ip == null || ip.length() == 0 || "unknown".equalsIgnoreCase(ip)) {
			ip = request.getHeader("Proxy-Client-IP");
			// System.out.println("Proxy-Client-IP:" + ip);
		}
		if (ip == null || ip.length() == 0 || "unknown".equalsIgnoreCase(ip)) {
			ip = request.getHeader("WL-Proxy-Client-IP");
			// System.out.println("WL-Proxy-Client-IP:" + ip);
		}
		if (ip == null || ip.length() == 0 || "unknown".equalsIgnoreCase(ip)) {
			ip = request.getRemoteAddr();
			// System.out.println("RemoteAddr:" + ip);
		}
		return ip;
	}

	public static void main(String[] args) {
		HttpClientUtil hcu = new HttpClientUtil();
		String data = "{\"loginId\":\"15812345678\",\"password\":\"123456\",\"channelId\":\"123\",\"deviceType\":0}";
		String json = hcu.doClient("192.168.0.200:8080/labpoo/token/add", data, null, false, "POST");
		System.out.println(json);
	}
}
