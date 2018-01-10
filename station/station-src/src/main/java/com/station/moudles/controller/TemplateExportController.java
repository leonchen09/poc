package com.station.moudles.controller;

import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URLDecoder;

import org.bouncycastle.util.encoders.UrlBase64;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.moudles.vo.AjaxResponse;

import jline.FileNameCompletor;

@Controller
@RequestMapping(value = "/template")
public class TemplateExportController extends BaseController {
	private String parent = TemplateExportController.class.getClassLoader().getResource("").getPath()
			+ "base_template/";
	private String[] sourceName = { "主机导入模板.xlsx", "电池组导入模板.xlsx",
			"备用主机导入模板.xlsx", "备用从机导入模板.xlsx", "设备维护记录导入模板.xlsx","电池更换记录模板.xlsx","设备维护记录12V从机导入模板.xlsx"};
	

	@ResponseBody
	@RequestMapping(value = "/export/{type}", method = RequestMethod.GET)
	public AjaxResponse export(@PathVariable Integer type) throws Exception {
		AjaxResponse ajaxResponse = new AjaxResponse();
		if (type < 0 || type >= sourceName.length) {
			ajaxResponse.setMsg("请不要传入非法参数！");
			return ajaxResponse;
		}
		String filePath = parent + sourceName[type]; 
		filePath = URLDecoder.decode(filePath, "UTF-8");
		File file = new File(filePath);
		String name = sourceName[type];
		String filename = new String(name.getBytes("UTF-8"), "iso-8859-1");
		// 设置response
		response.setContentType("application/x-msdownload; charset=UTF-8");
		response.addHeader("content-type", "application/x-msdownload;");
		response.addHeader("content-disposition", "attachment; filename=" + filename);
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
		return ajaxResponse;
	}
}
