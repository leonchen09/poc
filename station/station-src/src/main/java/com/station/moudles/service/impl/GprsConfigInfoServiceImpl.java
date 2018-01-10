package com.station.moudles.service.impl;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.text.DecimalFormat;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.TreeMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;
import org.apache.poi.ss.util.CellRangeAddress;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.common.Constant;
import com.station.common.utils.ReflectUtil;
import com.station.common.utils.RowParseHelper;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.GprsConfigInfoDetail;
import com.station.moudles.entity.GprsConfigInfoStation;
import com.station.moudles.entity.GprsConfigSend;
import com.station.moudles.entity.GprsDeviceType;
import com.station.moudles.entity.ModifyBalanceSend;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.SubDeviceMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.GprsConfigInfoService;
import com.station.moudles.service.GprsConfigSendService;
import com.station.moudles.service.GprsDeviceTypeService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.StationInfoService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.CommonSearchVo;
import com.station.moudles.vo.PulseVo;
import com.station.moudles.vo.search.PageEntity;

@Service
public class GprsConfigInfoServiceImpl extends BaseServiceImpl<GprsConfigInfo, Integer>
		implements GprsConfigInfoService {
	
	private static boolean gprsFile = false; //主机导入
	private static boolean backFile = false; //备用主机导入
	
	@Autowired
	GprsConfigInfoMapper gprsConfigInfoMapper;
	@Autowired
	SubDeviceService subDeviceSer;
	@Autowired
	GprsConfigSendService gprsConfigSendSer;
	@Autowired
	CellInfoService cellInfoSer;
	@Autowired
	StationInfoService stationInfoSer;
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	ModifyBalanceInfoServiceImpl modifybalanceInfoSer;
	@Autowired
	GprsDeviceTypeService gprsDeviceTypeSer;
	@Autowired
	SubDeviceMapper subDeviceMapper;
	
	private static final Logger logger = LoggerFactory.getLogger(GprsConfigInfoServiceImpl.class);

	@Override
	public void updateByCompanyId(GprsConfigInfo gprsConfigInfo) {
		// updateByCompanyId(gprsConfigInfo);
		GprsConfigSend gprsConfigSend = new GprsConfigSend();
		BeanUtils.copyProperties(gprsConfigInfo, gprsConfigSend);
		gprsConfigSend.setId(null);
		boolean flag = ReflectUtil.checkBeanNullWithoutSup(gprsConfigSend, GprsConfigSend.class);
		gprsConfigSend.setSendDone((byte) 0);
		gprsConfigSend.setConnectionType(2);
		if (!flag) {
			GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
			queryGprsConfigInfo.setCompanyId(gprsConfigInfo.getCompanyId());
			List<GprsConfigInfo> updateList = selectListSelective(queryGprsConfigInfo);
			for (GprsConfigInfo config : updateList) {
				gprsConfigSend.setGprsId(config.getGprsId());
				gprsConfigSend.setId(null);
				gprsConfigSendSer.insertSelective(gprsConfigSend);
			}
		}

		GprsConfigInfo updateConfig = new GprsConfigInfo();
		BeanUtils.copyProperties(gprsConfigInfo, updateConfig);
		// 没用上，注释了
		// GprsConfigSend nullGprsConfigSend = new GprsConfigSend();
		// BeanUtils.copyProperties(nullGprsConfigSend, updateConfig);
		
		// 12/19 修改 修改主机参数直接修改gprs_congfig_info表里面的参数 
		if (flag) {
			gprsConfigInfoMapper.updateByCompanyId(updateConfig);
		}
		//将gprsDeviceType 设置为null 以前没有传递这个，避免错误
		gprsConfigInfo.setDeviceType(null);
		PulseDischargeSend pulseDischargeSend = new PulseDischargeSend();
		BeanUtils.copyProperties(gprsConfigInfo, pulseDischargeSend);
		pulseDischargeSend.setId(null);
		flag = ReflectUtil.checkBeanNullWithoutSup(pulseDischargeSend, PulseDischargeSend.class);
		if (!flag) {
			GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
			BeanUtils.copyProperties(pulseDischargeSend, updateGprsConfigInfo);
			// updateGprsConfigInfo.setCompanyId(gprsConfigInfo.getCompanyId());
			// gprsConfigInfoMapper.updateByCompanyId(updateGprsConfigInfo);
			CommonSearchVo commonSearchVo = new CommonSearchVo();
			commonSearchVo.setCompanyId(gprsConfigInfo.getCompanyId());
			stationInfoSer.pulseDischargeAsync(commonSearchVo);
		}

	}

	@Override
	public void updateById(GprsConfigInfo gprsConfigInfo) {
		// updateByGprsSelective(gprsConfigInfo);
		GprsConfigSend gprsConfigSend = new GprsConfigSend();
		BeanUtils.copyProperties(gprsConfigInfo, gprsConfigSend);
		gprsConfigSend.setId(null);
		boolean flag = ReflectUtil.checkBeanNullWithoutSup(gprsConfigSend, GprsConfigSend.class);
		if (!flag) {
			gprsConfigSend.setSendDone((byte) 0);
			gprsConfigSend.setConnectionType(2);
			gprsConfigSendSer.insertSelective(gprsConfigSend);
		}
		PulseDischargeSend pulseDischargeSend = new PulseDischargeSend();
		BeanUtils.copyProperties(gprsConfigInfo, pulseDischargeSend);
		pulseDischargeSend.setId(null);
		flag = ReflectUtil.checkBeanNullWithoutSup(pulseDischargeSend, PulseDischargeSend.class);
		if (!flag) {
			GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();

			// BeanUtils.copyProperties(pulseDischargeSend, updateGprsConfigInfo); 原
			// updateGprsConfigInfo.setId(gprsConfigInfo.getId()); 原

			// 添加的，由于 gprs_config_info 表中没有数据
			// 暂时不写入，需要等主机修改成功后，适配器读取主机数据写入该表。
			// 但是5位id的老设备没有功能，我们直接修改表。
			// if (gprsConfigInfo.getGprsId().length() == 6) {
			BeanUtils.copyProperties(gprsConfigInfo, updateGprsConfigInfo);
			updateByPrimaryKeySelective(updateGprsConfigInfo);
			// }

			/*
			 * if (gprsConfigInfo.getGprsId() != null) {
			 * cellInfoSer.updateSendDoneByGprsCellIndex(gprsConfigInfo.getGprsId(),
			 * gprsConfigInfo.getCellIndex()); }
			 */
		}
		// --add 添加均衡信息
		ModifyBalanceSend balanceSend = new ModifyBalanceSend();
		balanceSend.setGprsId(gprsConfigInfo.getGprsId());
		balanceSend.setSendDone(0);// 未发送
		if (gprsConfigInfo.getPara1() != null) {
			balanceSend.setPara1(gprsConfigInfo.getPara1());
		}
		if (gprsConfigInfo.getPara2() != null) {
			balanceSend.setPara2(gprsConfigInfo.getPara2());
		}
		if (gprsConfigInfo.getPara3() != null) {
			balanceSend.setPara3(gprsConfigInfo.getPara3());
		}
		if (gprsConfigInfo.getPara4() != null) {
			balanceSend.setPara4(gprsConfigInfo.getPara4());
		}
		if (gprsConfigInfo.getPara5() != null) {
			balanceSend.setPara5(gprsConfigInfo.getPara5());
		}
		if (gprsConfigInfo.getPara6() != null) {
			balanceSend.setPara6(gprsConfigInfo.getPara6());
		}
		if (gprsConfigInfo.getPara7() != null) {
			balanceSend.setPara7(gprsConfigInfo.getPara7());
			modifybalanceInfoSer.insertSelective(balanceSend);
		}
	}

	@Override
	public boolean parseMasterDeviceExcelFile(File file, AjaxResponse ajaxResponse)
			throws EncryptedDocumentException, InvalidFormatException, IOException {
		if(gprsFile) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("系统繁忙！");
			return true;
		}
		gprsFile = true;
		// 导入主机，问题行详细记录信息
		TreeMap<Integer, String> errorRow = new TreeMap<>();
		// 先查询所有
		GprsDeviceType gprsDeviceType = new GprsDeviceType();
		List<GprsDeviceType> deviceTypeList = gprsDeviceTypeSer.selectListSelective(gprsDeviceType);
		//k- name.V-类型
		Map<String, Integer> nameAndcode = new HashMap<String, Integer>();
		//k- name,V-sub_device_count
		Map<String, Integer> nameAndSubCount = new HashMap<String, Integer>();
		//K-name,v -vol
		Map<String,Integer> nameAndVol = new HashMap<String,Integer>();
		if (deviceTypeList.size() != 0) {
			for (GprsDeviceType deviceType : deviceTypeList) {
				nameAndcode.put(deviceType.getTypeName(), deviceType.getTypeCode());
				nameAndSubCount.put(deviceType.getTypeName(), deviceType.getSubDeviceCount());
				nameAndVol.put(deviceType.getTypeName(), deviceType.getVolLevel());
			}
		}
		logger.info("start parse masterDevice excel file!");
		try {
			int rowNum = 1;
			int successCount = 0;
			Integer subDeviceCount = 0;
			Integer volLevel = 0;
			InputStream inp = new FileInputStream(file);
			Workbook wb = WorkbookFactory.create(inp);
			Sheet sheet = wb.getSheetAt(0);
			for (Row row : sheet) {
				rowNum = row.getRowNum();
				if (rowNum >= 2) {
					ajaxResponse.setCode(Constant.RS_CODE_ERROR);
					ajaxResponse.setMsg("文件导入失败，第" + (rowNum + 1) + "行数据有问题。");
					GprsConfigInfo createGprsConfigInfo = new GprsConfigInfo();
					String gprsId = "";
					if (StringUtils.getString(row.getCell(0)) != null) {
						String Id = RowParseHelper.getCell(row, 0);
						Pattern regex = Pattern.compile("^([A-Z]{1}[0]{1}[A-Z]{1}\\d{6})$");
						Matcher matcher = regex.matcher(Id);
						boolean matches = matcher.matches();
						if (matches) {
							gprsId = Id;
						} else {
							if(StringUtils.getString(row.getCell(1)) != null 
									||StringUtils.getString(row.getCell(2)) != null 
									||StringUtils.getString(row.getCell(3)) != null 
									|| StringUtils.getString(row.getCell(4)) != null) {
								
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的主机的ID没有填写或者填写不正确！例：Y0A000012/T0B000023");
							}
							break;
						}
					} else {
						if(StringUtils.getString(row.getCell(1)) != null 
								||StringUtils.getString(row.getCell(2)) != null 
								||StringUtils.getString(row.getCell(3)) != null 
								|| StringUtils.getString(row.getCell(4)) != null) {
							
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的主机的ID没有填写或者填写不正确！例：Y0A000012");
						}	
						break;
					}
					createGprsConfigInfo.setGprsId(gprsId);
					createGprsConfigInfo.setGprsIdOut(gprsId);
					//电话号码
					if (StringUtils.getString(row.getCell(1)) != null ) {
						String DevicePhone = null;
						try {
							DevicePhone = new DecimalFormat("0").format(row.getCell(1).getNumericCellValue());
						} catch (Exception e) {
							DevicePhone = StringUtils.getString(row.getCell(1));
						}
						boolean flag = false;
						Pattern regex = Pattern.compile("^[\\d\\s]{0,16}$");
						Matcher matcher = regex.matcher(DevicePhone);
						flag = matcher.matches();
						if (flag && DevicePhone.length() <= 16) {
							createGprsConfigInfo.setDevicePhone(DevicePhone);
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "请填写有效的小于16位电话卡号！");
							continue;
						}

					}
					if (StringUtils.getString(row.getCell(2)) != null && row.getCell(2).toString().length() <= 20) {
						String deviceTypeStr = row.getCell(2).toString();
//						if (deviceTypeStr != null && deviceTypeStr.indexOf("诊断") != -1) {
//							createGprsConfigInfo.setDeviceType(2);
//						} else if(deviceTypeStr != null && deviceTypeStr.indexOf("复用") != -1){
//							createGprsConfigInfo.setDeviceType(1);
//						} else {
//							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行主机的操作类型填写错误，注意检查模板是否正确！");
//							continue;
//						}
						switch (deviceTypeStr) {
						case "蓄电池串联复用设备":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							createGprsConfigInfo.setSubDeviceCount(nameAndSubCount.get(deviceTypeStr));
							subDeviceCount=nameAndSubCount.get(deviceTypeStr);
							volLevel = nameAndVol.get(deviceTypeStr);
							break;
						case "蓄电池串联复用诊断组件":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							createGprsConfigInfo.setSubDeviceCount(nameAndSubCount.get(deviceTypeStr));
							subDeviceCount=nameAndSubCount.get(deviceTypeStr);
							volLevel = nameAndVol.get(deviceTypeStr);
							break;
						
						case "蓄电池2V监测设备":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							createGprsConfigInfo.setSubDeviceCount(nameAndSubCount.get(deviceTypeStr));
							subDeviceCount=nameAndSubCount.get(deviceTypeStr);
							volLevel = nameAndVol.get(deviceTypeStr);
							break;
						case "蓄电池12V监测设备":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							createGprsConfigInfo.setSubDeviceCount(nameAndSubCount.get(deviceTypeStr));
							subDeviceCount=nameAndSubCount.get(deviceTypeStr);
							volLevel = nameAndVol.get(deviceTypeStr);
							break;
						default:
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行备用主机的设备类型类型没有填写或者填写错误！");
							continue;
						}
						
						
					} else {
						errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行主机的操作类型没有填写或者字符长度超过20个字符！");
						continue;
					}
					// -----10/10 add 主机的串口和规格
					if (StringUtils.getString(row.getCell(3)) != null) {
						String spec = RowParseHelper.getCell(row, 3);
						if (spec.length() <= 15) {
							createGprsConfigInfo.setGprsSpec(spec);
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "主机规格字符长度超过15个字符！");
							continue;
						}
					}
					if (StringUtils.getString(row.getCell(4)) != null) {
						if (row.getCell(4).toString().length() <= 15) {
							// 解决当端口号被转换成有小数点
							String gprsPort = null;
							try {
								gprsPort = new DecimalFormat("0").format(row.getCell(4).getNumericCellValue());
							} catch (Exception e) {
								gprsPort = StringUtils.getString(row.getCell(4));
							}
							createGprsConfigInfo.setGprsPort(gprsPort);
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "串口号的长度超过15个字符！");
							continue;
						}

					}
					// ---------end
					// ---------10/17 add 主机导入如有就要更新
					GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
					queryGprsConfigInfo.setGprsId(gprsId);// 当导入主机时，gprs_id_out为null的情况，导致500, 改成用gprs_id查找
					List<GprsConfigInfo> configList = selectListSelective(queryGprsConfigInfo);
					if (configList.size() > 0) {

						for (GprsConfigInfo gprsConfigInfo : configList) {
							//保证外部id不改变
							createGprsConfigInfo.setGprsIdOut(gprsConfigInfo.getGprsIdOut());
							// 12/5 add 主机导入必须是配套的 0 
							Integer gprsFlag = gprsConfigInfo.getGprsFlag();
							if(gprsFlag != null && gprsFlag !=0) {
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的设备编号存在，并且是备用的设备，不能导入！");
								continue;
							}else {
							// 查询是否有绑定过电池组
							StationInfo stationInfo = new StationInfo();
							stationInfo.setGprsId(gprsConfigInfo.getGprsId());
							List<StationInfo> stationList = stationInfoSer.selectListSelective(stationInfo);
							if(stationList.size() != 0) {
								if(stationList.get(0).getVolLevel() != volLevel) {
									errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的设备的设备类型对应的电压平台和该设备已经绑定电池组的电压平台不一致，不能导入！");
									continue;
								}
							}
							gprsConfigInfoMapper.updateByGprsSelective(createGprsConfigInfo);
							// 修改从机的设备类型
							SubDevice subDevice = new SubDevice();
							subDevice.setSubType(createGprsConfigInfo.getDeviceType());
							subDevice.setGprsId(gprsConfigInfo.getGprsId());
							subDeviceMapper.updateSubTypeByGprsId(subDevice);
							successCount++;
						
							ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
							ajaxResponse.setMsg("主机导入成功");
							// -------add 新增主机同时新增24个从机 并且设备类型和规格匹配
							Integer deviceTpe = createGprsConfigInfo.getDeviceType();
							String gprsSpec = gprsConfigInfo.getGprsSpec();
							
							// 把逻辑抽成了方法，因为新增时也要用到
							isSubDevice(gprsId, deviceTpe, gprsSpec,subDeviceCount);
							continue;
							}
						}
						continue;
					}
					// --------10/17 end
					insertSelective(createGprsConfigInfo);
					// 原来的createSubDeviceByGprsId(gprsId);
					// -----add新增主机同时新增24个从机 并且设备类型和规格匹配
					createSubDevice(gprsId, createGprsConfigInfo.getDeviceType(), createGprsConfigInfo.getGprsSpec(),0,createGprsConfigInfo.getSubDeviceCount());

					successCount++;
				}
			}
			if (successCount == 0 && errorRow.size() <= 0) {
				errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的主机的ID没有填写或者填写不正确！例：Y0A000012");
			}
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("主机导入处理完成，共导入" + successCount + "行数据\n" + sb.toString());

			logger.info("parse masterDevice excel file over!");
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("主机表导入出错，请选择正确模板！");
			logger.error("主机表导入出错-->", e);
		}
		errorRow.clear();
		gprsFile = false;
		return true;
	}

	public void isSubDevice(String gprsId, Integer deviceType, String gprsSpec,Integer subDeviceCount) {
		SubDevice subDevice = new SubDevice();
		subDevice.setGprsId(gprsId);
		int count = subDeviceSer.selectListCountSelective(subDevice);
		if (count == 0) { // 没有从机，生成从机
			// 原来的createSubDeviceByGprsId(gprsId);
			// ---add
			createSubDevice(gprsId, deviceType, gprsSpec,count,subDeviceCount);
		}
		if(count > subDeviceCount) {//原来是设备是24个从机，现在设备是4个从机，就删除后面的
			Map<String,Object> map = new HashMap<String,Object>();
			map.put("startIndex", subDeviceCount+1);
			map.put("endIndex", count);
			map.put("gprsId", gprsId);
			subDeviceSer.deleteMoreSubDevice(map);
		}
		if(count < subDeviceCount) {//原来是设备是4个从机，现在设备是24个从机，就增加后面的
			createSubDevice(gprsId, deviceType, gprsSpec,count,subDeviceCount);
		}
		
		
	}

	// 备用主机导入
	@Override
	public boolean parseBackupDeviceExcelFile(File file, AjaxResponse ajaxResponse)
			throws EncryptedDocumentException, InvalidFormatException, IOException {
		if(backFile) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("系统繁忙！");
			return true;
		}
		backFile = true;
		// 问题行详细记录信息
		TreeMap<Integer, String> errorRow = new TreeMap<>();
		// 先查询所有
		GprsDeviceType gprsDeviceType = new GprsDeviceType();
		List<GprsDeviceType> deviceTypeList = gprsDeviceTypeSer.selectListSelective(gprsDeviceType);
		//k- name.V-类型
		Map<String, Integer> nameAndcode = new HashMap<String, Integer>();
		if (deviceTypeList.size() != 0) {
			for (GprsDeviceType deviceType : deviceTypeList) {
				nameAndcode.put(deviceType.getTypeName(), deviceType.getTypeCode());
			}
		}
		logger.info("start parse backupDevice excel file!");
		try {
			int rowNum = 1;
			int successCount = 0;
			InputStream inp = new FileInputStream(file);
			Workbook wb = WorkbookFactory.create(inp);
			Sheet sheet = wb.getSheetAt(0);
			for (Row row : sheet) {
				rowNum = row.getRowNum();
				if (rowNum >= 2) {
					String gprsId = "";
					if (StringUtils.getString(row.getCell(0)) != null) {
						String id = RowParseHelper.getCell(row, 0);
						Pattern regex = Pattern.compile("^([A-Z]{1}[1]{1}[A-Z]{1}\\d{6})$");
						Matcher matcher = regex.matcher(id);
						boolean matches = matcher.matches();

						if (matches) {
							gprsId = id;
						} else {
							if(StringUtils.getString(row.getCell(1)) != null 
									||StringUtils.getString(row.getCell(2)) != null 
									||StringUtils.getString(row.getCell(3)) != null 
									|| StringUtils.getString(row.getCell(4)) != null) {
								
								 errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的备用主机的ID没有填写或者填写不正确或者模板错误！例：Y1A000012/T1B000023");
							}
							break;
						}
					} else {
						if(StringUtils.getString(row.getCell(1)) != null 
								||StringUtils.getString(row.getCell(2)) != null 
								||StringUtils.getString(row.getCell(3)) != null 
								|| StringUtils.getString(row.getCell(4)) != null) {
							
							 errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的备用主机的ID没有填写或者填写不正确！例：Y0A000012");
						}
						break;
					}
					//ajaxResponse.setMsg("文件导入失败，第" + (rowNum + 1) + "行数据有问题。");
					GprsConfigInfo createGprsConfigInfo = new GprsConfigInfo();

					// 原来的查询有不更新
					/*
					 * GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
					 * queryGprsConfigInfo.setGprsIdOut(gprsId);
					 * 
					 * List<GprsConfigInfo> configList = selectListSelective(queryGprsConfigInfo);
					 * if (configList.size() > 0) { ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
					 * ajaxResponse.setMsg("备用主机导入成功"); continue; }
					 */

					createGprsConfigInfo.setGprsId(gprsId);
					createGprsConfigInfo.setGprsIdOut(gprsId);
					// ----add 备用主机 导入设置为备用
					createGprsConfigInfo.setGprsFlag(1);
					// ----end
					if (StringUtils.getString(row.getCell(1)) != null && row.getCell(1).toString().length() <= 20) {
						String deviceTypeStr = row.getCell(1).toString();
//						if (deviceTypeStr != null && deviceTypeStr.indexOf("诊断") != -1) {
//							createGprsConfigInfo.setDeviceType(2);
//						} else if(deviceTypeStr.indexOf("复用")  != -1){
//							createGprsConfigInfo.setDeviceType(1);
//						}else {
//							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的备用主机的类型没有填写有无，注意检查模板是否正确！");
//							continue;
//						}
						switch (deviceTypeStr) {
						case "蓄电池串联复用设备":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							break;
						case "蓄电池串联复用诊断组件":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							break;
						
						case "蓄电池2V监测设备":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							break;
						case "蓄电池12V监测设备":
							createGprsConfigInfo.setDeviceType(nameAndcode.get(deviceTypeStr));
							break;
						default:
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行主机的设备类型类型没有填写或者填写错误");
							continue;
						}
					} else {
						errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的备用主机的类型没有填写或者填写的字符超过20个长度！");
						continue;
					}

					// ------10/10 add 添加导入备用主机的规格、电话卡号 和端口
					if (StringUtils.getString(row.getCell(2)) != null) {
						String spec = RowParseHelper.getCell(row, 2);
						if (spec.length() <= 15) {
							createGprsConfigInfo.setGprsSpec(spec);
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "填写的规格字符超过15个长度！");
							continue;
						}
					}

					if (StringUtils.getString(row.getCell(3)) != null) {
						// 解决当电话号码，被自动转化成科学计数法的格式
						String DevicePhone = null;
						try {
							DevicePhone = new DecimalFormat("0").format(row.getCell(3).getNumericCellValue());
						} catch (Exception e) {
							DevicePhone = StringUtils.getString(row.getCell(3));
						}
						boolean flag = false;
						Pattern regex = Pattern.compile("^[\\d\\s]{0,16}$");
						Matcher matcher = regex.matcher(DevicePhone);
						flag = matcher.matches();
						if (flag && DevicePhone.length() <= 16) {
							createGprsConfigInfo.setDevicePhone(DevicePhone);
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "请填写有效的不超过16的电话卡号！");
							continue;
						}

					}
					if (StringUtils.getString(row.getCell(4)) != null) {
						// 解决当端口号被转换成有小数点
						String gprsPort = null;
						try {
							gprsPort = new DecimalFormat("0").format(row.getCell(4).getNumericCellValue());
						} catch (Exception e) {
							gprsPort = StringUtils.getString(row.getCell(4));
						}

						if (row.getCell(4).toString().length() <= 15) {
							
							createGprsConfigInfo.setGprsPort(gprsPort);
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "串口号填写的字符超过15个！");
							continue;
						}

					}
					// -----end
					// -----10/16 add 查询有要就跟新
					GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
					queryGprsConfigInfo.setGprsIdOut(gprsId);
					List<GprsConfigInfo> configList = selectListSelective(queryGprsConfigInfo);
		
					if (configList.size() > 0) {
						for (GprsConfigInfo gprsConfigInfo : configList) {
							// 12/5 add --判断标志是否是备用的 1 备用 
							Integer gprsFlag = gprsConfigInfo.getGprsFlag();
							if(gprsFlag != null && gprsFlag != 1) {
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的设备编号已经存在，并且是配套设备不能导入！");
								continue;
							}
							gprsConfigInfoMapper.updateByGprsSelective(createGprsConfigInfo);
							
							successCount++;
						}
						ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
						ajaxResponse.setMsg("备用主机导入处理完成");
						continue;
					}
					// ----10/16 end
					insertSelective(createGprsConfigInfo);
					ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
					ajaxResponse.setMsg("备用主机导入处理完成");
					successCount++;
				}
			}
			if (successCount == 0 && errorRow.size()<=0) {
				errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的备用主机的ID没有填写或者填写不正确！例：Y0A000012");
			}
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}

			ajaxResponse.setMsg("备用主机导入处理完成，共导入" + successCount + "行数据\n" + sb.toString());

			logger.info("parse backupDevice excel file over!");
		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("备用主机导入导入出错\r\n" + sb.toString());
			ajaxResponse.setMsg("备用主机表导入出错：" + e.getMessage());
			logger.error("备用主机表导入出错-->", e);
		}
		errorRow.clear();
		backFile = false;
		return true;
	}

	// -----原来的
	@Override
	public void createSubDeviceByGprsId(String gprsId) {
		for (int i = 1; i < 25; i++) {
			SubDevice subDevice = new SubDevice();
			subDevice.setCellSort(i);
			subDevice.setGprsId(gprsId);
			subDevice.setSubDeviceId(getSubDeviceIdByGprsId(gprsId, i));
			subDevice.setSubDeviceIdOut(getSubDeviceIdByGprsId(gprsId, i));
			subDeviceSer.insertSelective(subDevice);
		}
	}

	// -------add 新增主机同时新增24个从机 并且设备类型和规格匹配
	//现在根据设备类型来决定新增从机的多少
	@Override
	public void createSubDevice(String gprsId, Integer deviceType, String gprsSpec,Integer count,Integer subDeviceCount) {
		for (int i = count+1; i <= subDeviceCount; i++) {
			SubDevice subDevice = new SubDevice();
			subDevice.setCellSort(i);
			subDevice.setGprsId(gprsId);
			subDevice.setSubDeviceId(getSubDeviceIdByGprsId(gprsId, i));
			subDevice.setSubDeviceIdOut(getSubDeviceIdByGprsId(gprsId, i));
			subDevice.setSubType(deviceType);
			subDevice.setSubSpec(gprsSpec);
			subDeviceSer.insertSelective(subDevice);
		}
	}
	// ---------end

	String getSubDeviceIdByGprsId(String gprsId, int index) {
		String subDeviceId = "";
		if (index > 9) {
			subDeviceId = gprsId.substring(2) + index;
		} else {
			subDeviceId = gprsId.substring(2) + "0" + index;
		}
		return subDeviceId;
	}

	@Override
	public GprsConfigInfoDetail selectDetailById(Integer id) {
		return gprsConfigInfoMapper.selectDetailById(id);
	}

	@Override
	public void updateByGprsSelective(GprsConfigInfo gprsConfigInfo) {
		int rows = gprsConfigInfoMapper.updateByGprsSelective(gprsConfigInfo);
	}

	@Override
	public List<GprsConfigInfoStation> selectStationListSelectivePaging(PageEntity pageEntity) {
		return gprsConfigInfoMapper.selectStationListSelectivePaging(pageEntity);
	}

	@Override
	public List selectUnbindGprsList(String gprsId) {
		return gprsConfigInfoMapper.selectUnbindGprsList(gprsId);
	}

	@Override
	public List<String> selectBindGprsList(GprsConfigInfo gprsConfigInfo) {
		return gprsConfigInfoMapper.selectBindGprsList(gprsConfigInfo);
	}

	public static void main(String[] args) {
		String deviceTypeStr = "蓄电池串联复用诊断组件";
		GprsConfigSend gprsConfigSend = new GprsConfigSend();
		GprsConfigInfo c = new GprsConfigInfo();
		c.setHeartbeatInterval(50);
		BeanUtils.copyProperties(gprsConfigSend, c);
	}

	@Override
	public void updateAndSend(PulseVo pulse) {
		if (pulse.getCompanyId() != null) {
			GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
			queryGprsConfigInfo.setCompanyId(pulse.getCompanyId());
			List<GprsConfigInfo> updateList = selectListSelective(queryGprsConfigInfo);
			for (GprsConfigInfo c : updateList) {
				GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
				updateGprsConfigInfo.setId(c.getId());
				BeanUtils.copyProperties(pulse, updateGprsConfigInfo);
				updateByPrimaryKeySelective(updateGprsConfigInfo);

				cellInfoSer.updateSendDoneByGprsCellIndex(c.getGprsId(), pulse.getCellIndex(), 0);
			}

		} else {

			for (String gprsId : pulse.getGprsIdList()) {
				GprsConfigInfo queryGprsConfigInfo = new GprsConfigInfo();
				queryGprsConfigInfo.setGprsId(gprsId);
				List<GprsConfigInfo> updateList = selectListSelective(queryGprsConfigInfo);
				if (updateList.size() > 0) {
					for (GprsConfigInfo c : updateList) {
						GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
						updateGprsConfigInfo.setId(c.getId());
						BeanUtils.copyProperties(pulse, updateGprsConfigInfo);
						updateByPrimaryKeySelective(updateGprsConfigInfo);
					}
					cellInfoSer.updateSendDoneByGprsCellIndex(gprsId, pulse.getCellIndex(), 0);
				}

			}
		}

	}

	// app端修改gprsid 修改主机id
	@Override
	public void updateGprsIdApp(RoutingInspectionStationDetail stationDetail) {
		// 在巡检表插入数据(主表)
		RoutingInspections routingInspections = new RoutingInspections();
		routingInspections.setStationId(stationDetail.getStationId());
		routingInspections.setGprsId(stationDetail.getGprsId());
		//routingInspections.setRoutingInspectionStatus(1);// 设备维护中
		// 通过电池组的状态在维护中 ，电池组id,设备id来判断是否有这条数据 如果没有就新增
		List<RoutingInspections> routingList = routingInspectionsSer.selectListSelectiveFirst(routingInspections);		
		if (routingList.size() == 0) {
			if (stationDetail.getOperateTime() != null) {
				routingInspections.setOperateTime(stationDetail.getOperateTime());
			} else {
				Date date = new Date();
				routingInspections.setOperateTime(date);
			}
			if(stationDetail.getOperateType() != null) {
				routingInspections.setOperateType(stationDetail.getOperateType());
			}
			routingInspections.setStationId(stationDetail.getStationId());
			routingInspections.setOperateId(stationDetail.getOperateId());
			routingInspections.setOperateName(stationDetail.getOperateName());
			
			routingInspections.setGprsId(stationDetail.getGprsId());
			routingInspections.setOperatePhone(stationDetail.getOperatePhone());
			routingInspections.setDeviceType(stationDetail.getDeviceType());
			routingInspections.setRoutingInspectionStatus(0);// 在提交之前状态
			routingInspectionsSer.insertSelective(routingInspections);

			logger.info("新增巡检记录修改主机:{}成功", routingInspections.getConfirmOperateName());
		}

		//List<RoutingInspections> selectListSelective = routingInspectionsSer.selectListSelective(routingInspections);
		// 详情保存(从表)
		Integer routingInspectionId = null;
		if(routingList.size() != 0) {
			routingInspectionId = routingList.get(0).getRoutingInspectionId();
		}else {
			routingInspectionId = routingInspections.getRoutingInspectionId();
		}
			RoutingInspectionDetail routingInspectionDetail = new RoutingInspectionDetail();
			routingInspectionDetail.setRoutingInspectionsId(routingInspectionId);
			routingInspectionDetail.setRequestType(1);//通过requestType == 1 查询web是否回应
			// 查询出web提交的
			List<RoutingInspectionDetail> routingDetailType = routingInspectionDetailSer.selectListSelective(routingInspectionDetail);
			routingInspectionDetail.setRequestType(0);//通过requestType == 0 查询出最大RequestSeq的数据
			List<RoutingInspectionDetail> routingDetailSeq = routingInspectionDetailSer.selectListSelective(routingInspectionDetail);

			if (routingDetailType.size() != 0) {
				// 得出最新数据		
				RoutingInspectionDetail InspectionDetail = routingDetailSeq.stream()
						.max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
				if (InspectionDetail.getRequestSeq() == null) {
					routingInspectionDetail.setRequestSeq(1);
				} else {
					routingInspectionDetail.setRequestSeq(InspectionDetail.getRequestSeq() + 1);
				}
				if (stationDetail.getOperateTime() != null) {
					routingInspectionDetail.setCreateTime(stationDetail.getOperateTime());
				}
				routingInspectionDetail.setDetailOperateId(stationDetail.getOperateId());
				routingInspectionDetail.setDetailOperateName(stationDetail.getOperateName());
				routingInspectionDetail.setDetailOperateType(2);// 跟换主机
				routingInspectionDetail.setDetailOperateValueOld(stationDetail.getGprsId());
				routingInspectionDetail.setDetailOperateValueNew(stationDetail.getNewGprsIdOut());
				routingInspectionDetailSer.insertSelective(routingInspectionDetail);

				logger.info("新增修改主机详情:{}成功", stationDetail.getNewGprsIdOut());

			} else {
				if (stationDetail.getOperateTime() != null) {
					routingInspectionDetail.setCreateTime(stationDetail.getOperateTime());
				}
				routingInspectionDetail.setRequestSeq(1);
				routingInspectionDetail.setDetailOperateId(stationDetail.getOperateId());
				routingInspectionDetail.setDetailOperateName(stationDetail.getOperateName());
				routingInspectionDetail.setDetailOperateType(2);// 跟换主机
				routingInspectionDetail.setDetailOperateValueOld(stationDetail.getGprsId());
				routingInspectionDetail.setDetailOperateValueNew(stationDetail.getNewGprsIdOut());
				routingInspectionDetailSer.insertSelective(routingInspectionDetail);

				logger.info("新增修改主机详情:{}成功", stationDetail.getNewGprsIdOut());

			}
		}


	// 修改霍尔感应
	@Override
	public void updateHallFlag(RoutingInspectionStationDetail stationDetail) {
		// 在巡检表插入数据(主表)
		RoutingInspections routingInspections = new RoutingInspections();
		routingInspections.setStationId(stationDetail.getStationId());
		routingInspections.setGprsId(stationDetail.getGprsId());
		// 通过电池组的状态在维护中 ，电池组id,设备id来判断是否有这条数据 如果没有就新增
		List<RoutingInspections> routingList = routingInspectionsSer.selectListSelective(routingInspections);
		boolean flag = true;
		for (RoutingInspections ListStatus : routingList) {
			// 如果电池组是安装维护中或者是失败状态就不在主表中插入数据
			if (ListStatus.getRoutingInspectionStatus() != null) {
				if (ListStatus.getRoutingInspectionStatus() == 1 || ListStatus.getRoutingInspectionStatus() == 3) {
					routingInspections.setRoutingInspectionStatus(ListStatus.getRoutingInspectionStatus());
					flag = false;
				}
			}
		}
		if (flag) {
			if (stationDetail.getOperateTime() != null) {
				routingInspections.setOperateTime(stationDetail.getOperateTime());
			} else {
				Date date = new Date();
				routingInspections.setOperateTime(date);
			}
			routingInspections.setStationId(stationDetail.getStationId());
			routingInspections.setOperateId(stationDetail.getOperateId());
			routingInspections.setOperateName(stationDetail.getOperateName());
			routingInspections.setGprsId(stationDetail.getGprsId());
			routingInspections.setOperatePhone(stationDetail.getOperatePhone());
			routingInspections.setOperateType(2);// 安装维护
			routingInspections.setRoutingInspectionStatus(1);// 安装维护中
			routingInspectionsSer.insertSelective(routingInspections);

			logger.info("新增巡检记录修改主机:{}成功", routingInspections.getConfirmOperateName());
		}

		List<RoutingInspections> selectListSelective = routingInspectionsSer.selectListSelective(routingInspections);
		// 详情保存(从表)
		if (selectListSelective.size() != 0) {
			RoutingInspections RI = selectListSelective.get(selectListSelective.size() - 1);
			Integer routingInspectionId = RI.getRoutingInspectionId();

			RoutingInspectionDetail routingInspectionDetail = new RoutingInspectionDetail();
			routingInspectionDetail.setRoutingInspectionsId(routingInspectionId);
			routingInspectionDetail.setRequestType(0);
			// 查询出提交的最大次数
			List<RoutingInspectionDetail> routingDetail = routingInspectionDetailSer
					.selectListSelective(routingInspectionDetail);
			if (routingDetail.size() != 0) {
				// 得出最新数据
				// RoutingInspectionDetail InspectionDetail =
				// routingDetail.get(routingDetail.size() - 1);
				RoutingInspectionDetail InspectionDetail = routingDetail.stream()
						.max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
				if (InspectionDetail.getRequestSeq() == null) {
					routingInspectionDetail.setRequestSeq(1);
				} else {
					routingInspectionDetail.setRequestSeq(InspectionDetail.getRequestSeq() + 1);
				}
				if (stationDetail.getOperateTime() != null) {
					routingInspectionDetail.setCreateTime(stationDetail.getOperateTime());
				}
				routingInspectionDetail.setDetailOperateId(RI.getOperateId());
				routingInspectionDetail.setDetailOperateName(RI.getOperateName());
				routingInspectionDetail.setDetailOperateType(3);// 安装霍尔感应
				routingInspectionDetail.setDetailOperateValueOld(stationDetail.getGprsId());
				routingInspectionDetail.setDetailOperateValueNew(stationDetail.getNewGprsIdOut());
				routingInspectionDetailSer.insertSelective(routingInspectionDetail);

				logger.info("新增修改主机详情:{}成功", stationDetail.getNewGprsIdOut());

			} else {
				if (stationDetail.getOperateTime() != null) {
					routingInspectionDetail.setCreateTime(stationDetail.getOperateTime());
				}
				routingInspectionDetail.setRequestSeq(1);
				routingInspectionDetail.setDetailOperateId(RI.getOperateId());
				routingInspectionDetail.setDetailOperateName(RI.getOperateName());
				routingInspectionDetail.setDetailOperateType(3);// 安装霍尔感应
				routingInspectionDetail.setDetailOperateValueOld(stationDetail.getGprsId());
				routingInspectionDetail.setDetailOperateValueNew(stationDetail.getNewGprsIdOut());
				routingInspectionDetailSer.insertSelective(routingInspectionDetail);

				logger.info("新增修改主机详情:{}成功", stationDetail.getNewGprsIdOut());

			}
		}
	}

	/**
	 * 远程主机参数配置修改
	 * 
	 * @param gprsConfigInfo
	 */
	public void updateParamById(GprsConfigInfo gprsConfigInfo) {
		GprsConfigSend gprsConfigSend = new GprsConfigSend();
		BeanUtils.copyProperties(gprsConfigInfo, gprsConfigSend);
		gprsConfigSend.setId(null);
		boolean flag = ReflectUtil.checkBeanNullWithoutSup(gprsConfigSend, GprsConfigSend.class);
		if (!flag) {
			gprsConfigSend.setSendDone((byte) 0);
			gprsConfigSend.setConnectionType(2);
			gprsConfigSendSer.insertSelective(gprsConfigSend);
			//不直接修改gprs_config_info表
			//updateByPrimaryKeySelective(gprsConfigInfo);
		}
	}

	/**
	 * 远程主机特征测试动作参数修改
	 * 
	 * @param gprsConfigInfo
	 */
	public void updatePulseById(GprsConfigInfo gprsConfigInfo) {
		PulseDischargeSend pulseDischargeSend = new PulseDischargeSend();
		BeanUtils.copyProperties(gprsConfigInfo, pulseDischargeSend);
		pulseDischargeSend.setId(null);
		boolean flag = ReflectUtil.checkBeanNullWithoutSup(pulseDischargeSend, PulseDischargeSend.class);
		if (!flag) {
			GprsConfigInfo updateGprsConfigInfo = new GprsConfigInfo();
			BeanUtils.copyProperties(gprsConfigInfo, updateGprsConfigInfo);
			updateByPrimaryKeySelective(updateGprsConfigInfo);
		}
	}

	/**
	 * 主机均衡策略参数配置
	 * 
	 * @param modifyBalanceSend
	 */
	public void updateBalanceById(ModifyBalanceSend modifyBalanceSend) {
		boolean flag = ReflectUtil.checkBeanNullWithoutSup(modifyBalanceSend, ModifyBalanceSend.class);
		if (!flag) {
			modifyBalanceSend.setId(null);
			modifyBalanceSend.setSendDone(0);// 未发送
			modifybalanceInfoSer.insertSelective(modifyBalanceSend);
		}
	}

	@Override
	public List<GprsConfigInfo> selectRcvTimeNotNull(GprsConfigInfo gprsConfigInfo) {
		
		return gprsConfigInfoMapper.selectRcvTimeNotNull(gprsConfigInfo);
	}

}