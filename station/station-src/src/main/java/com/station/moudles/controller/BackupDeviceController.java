package com.station.moudles.controller;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;

import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.vo.AjaxResponse;

@Controller
@RequestMapping(value = "/backupDevice")
public class BackupDeviceController extends BaseController {
	@Autowired
	GprsConfigInfoService gprsConfigInfoSer;

	@Override
	public boolean parseFile(File file, AjaxResponse ajaxResponse, Integer companyId) throws EncryptedDocumentException, FileNotFoundException, InvalidFormatException, IOException {
		return gprsConfigInfoSer.parseBackupDeviceExcelFile(file, ajaxResponse);
	}
}
