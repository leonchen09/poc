package com.station.moudles.service.impl;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.Map.Entry;

import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.station.common.Constant;
import com.station.common.utils.RowParseHelper;
import com.station.common.utils.StringUtils;
import com.station.moudles.entity.GprsDeviceType;
import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.ModifyGprsidSend;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionStationDetail;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.mapper.SubDeviceMapper;
import com.station.moudles.service.GprsDeviceTypeService;
import com.station.moudles.service.InspectSignCellIndexService;
import com.station.moudles.service.ModifyGprsidSendService;
import com.station.moudles.service.RoutingInspectionDetailService;
import com.station.moudles.service.RoutingInspectionsService;
import com.station.moudles.service.SubDeviceService;
import com.station.moudles.vo.AjaxResponse;

@Service
public class SubDeviceServiceImpl extends BaseServiceImpl<SubDevice, Integer> implements SubDeviceService {
	private static boolean subFile = false;//备用从机导入
	@Autowired
	SubDeviceMapper subDeviceMapper;
	@Autowired
	RoutingInspectionsService routingInspectionsSer;
	@Autowired
	RoutingInspectionDetailService routingInspectionDetailSer;
	@Autowired
	ModifyGprsidSendService modifyGprsidSendSer;
	@Autowired
	GprsDeviceTypeService gprsDeviceTypeSer;

	private static final Logger logger = LoggerFactory.getLogger(SubDeviceServiceImpl.class);

	@Deprecated
	@Override
	public void changeSubDevice(SubDevice subDevice) {
		subDevice.setUpdateTime(new Date());
		updateByPrimaryKeySelective(subDevice);
		ModifyGprsidSend modifyGprsidSend = new ModifyGprsidSend();
		modifyGprsidSend.setGprsId(subDevice.getGprsId());
		modifyGprsidSend.setType(2);
		modifyGprsidSend.setInnerId(subDevice.getSubDeviceId());
		modifyGprsidSend.setOuterId(subDevice.getSubDeviceIdOut());
		modifyGprsidSend.setSendDone((byte) 0);
		modifyGprsidSend.setTime(new Date());
		modifyGprsidSendSer.insertSelective(modifyGprsidSend);
	}

	@Override
	public boolean parseSubDeviceExcelFile(File file, AjaxResponse ajaxResponse)
			throws EncryptedDocumentException, InvalidFormatException, IOException {
		
		if(subFile) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			ajaxResponse.setMsg("系统繁忙！");
			return true;
		}
		subFile = true;
		// 记录异常信息
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
		logger.info("start parse subDevice excel file!");
		try {
			int rowNum = 1;
			int successCount = 0;
			InputStream inp = new FileInputStream(file);
			Workbook wb = WorkbookFactory.create(inp);
			Sheet sheet = wb.getSheetAt(0);
			int a = sheet.getLastRowNum();
			for (Row row : sheet) {
				rowNum = row.getRowNum();
				if (rowNum >= 2) {
					ajaxResponse.setMsg("文件导入失败，第" + (rowNum + 1) + "行数据有问题。");
					SubDevice subDevice = new SubDevice();
					if (StringUtils.getString(row.getCell(1)) != null) {
						String subDeviceId = RowParseHelper.getCell(row, 1);
						Pattern regex = Pattern.compile("^([A-Z]\\d{8})$");
						Matcher matcher = regex.matcher(subDeviceId);
						boolean matches = matcher.matches();
						if (matches) {
							subDevice.setSubDeviceIdOut(subDeviceId);
							subDevice.setSubDeviceId(subDeviceId);
						} else {
							if (StringUtils.getString(row.getCell(0)) != null
									|| StringUtils.getString(row.getCell(2)) != null) {

								errorRow.put(rowNum + 1,
										"第" + (rowNum + 1) + "行从机Id没有填写或填写不正确或者模板错误！例：B00000002 长度不超过9个字符");
							}
							break;
						}
					} else {
						if (StringUtils.getString(row.getCell(0)) != null
								|| StringUtils.getString(row.getCell(2)) != null) {

							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行从机Id没有填写或填写不正确！例：B00000002");
						}
						break;
					}
					// ----add 导入的从机设备标志为备用
					subDevice.setSubFlag(1);
					List subDeviceList = selectListSelective(subDevice);
					if (subDeviceList.size() == 0) {
						// -----10/10 add 添加导入备用从机设备类型和规格
						if (StringUtils.getString(row.getCell(0)) != null && row.getCell(0).toString().length() <= 20) {
							String subType = row.getCell(0).toString();
//							if (subType != null && subType.indexOf("诊断") != -1) {
//								subDevice.setSubType(2);
//							} else if (subType != null && subType.indexOf("复用") != -1) {
//								subDevice.setSubType(1);
//							} else {
//								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行从机类型没有填写不正确，注意模板是否正确！");
//								continue;
//							}
							switch (subType) {
							case "蓄电池串联复用设备":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							case "蓄电池串联复用诊断组件":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							
							case "蓄电池2V监测设备":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							case "蓄电池12V监测设备":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							default:
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行备用从机的设备类型类型没有填写或者填写错误");
								continue;
							}
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行从机类型没有填写或者备用从机的设备类型字符长度超过20！");
							continue;
						}

						if (StringUtils.getString(row.getCell(2)) != null) {
							String spec = RowParseHelper.getCell(row, 2);
							if (spec.length() <= 15) {
								subDevice.setSubSpec(spec);
							} else {
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "备用从机的设备规格字符长度超过15！");
								continue;
							}

						}
						// -------end
						insertSelective(subDevice);
						successCount++;
					}

					// ----10/16 add 如果原来有就更新
					if (subDeviceList.size() > 0) {
						if (StringUtils.getString(row.getCell(0)) != null && row.getCell(0).toString().length() <= 20) {
							String subType = row.getCell(0).toString();
//							if (subType != null && subType.indexOf("诊断") != -1) {
//								subDevice.setSubType(2);
//							} else if (subType != null && subType.indexOf("复用") != -1) {
//								subDevice.setSubType(1);
//							} else {
//								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行从机类型没有填写或者备用从机的设备类型超过20个字符！");
//								continue;
//							}
							switch (subType) {
							case "蓄电池串联复用设备":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							case "蓄电池串联复用诊断组件":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							
							case "蓄电池2V监测设备":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							case "蓄电池12V监测设备":
								subDevice.setSubType(nameAndcode.get(subType));
								break;
							default:
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行备用从机的设备类型没有填写或者填写错误");
								continue;
							}
						} else {
							errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行从机类型没有填写或者备用从机的设备类型超过20个字符！");
							continue;
						}
						if (StringUtils.getString(row.getCell(2)) != null) {
							String spec = RowParseHelper.getCell(row, 2);
							if (spec.length() <= 15) {
								subDevice.setSubSpec(spec);
							} else {
								errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "备用从机的设备规格字符长度超过15！");
								continue;
							}

						}
						subDeviceMapper.updateByPrimaryKeySelectiveBySubDdviceId(subDevice);
						successCount++;
					}
				}
			}
			if (successCount == 0 && errorRow.size() <= 0) {
				errorRow.put(rowNum + 1, "第" + (rowNum + 1) + "行的从机的ID没有填写或者填写不正确！例：B00000012");
			}
			ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
			// ajaxResponse.setMsg("文件导入成功");
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("备用从机导入处理；共导入" + successCount + "行数据\n" + sb.toString());

		} catch (Exception e) {
			ajaxResponse.setCode(Constant.RS_CODE_ERROR);
			StringBuffer sb = new StringBuffer();
			if (errorRow.size() > 0) {
				for (Entry<Integer, String> et : errorRow.entrySet()) {
					String error = "第" + et.getKey() + "行，" + et.getValue() + "\r\n";
					sb.append(error);
				}
			}
			ajaxResponse.setMsg("备用从机导入失败\n" + sb.toString() + e.getMessage());
			// ajaxResponse.setMsg("备用从机表导入出错：" + e.getMessage());
			logger.error("备用从机表导入出错-->", e);
		}
		subFile = false;
		return true;
	}

	@Override
	public int selectListCountSelective(SubDevice subDevice) {

		return subDeviceMapper.selectListCountSelective(subDevice);
	}

	@Override
	public void updateByPrimaryKeySelectiveBySubDdviceId(SubDevice subDevice) {
		// TODO Auto-generated method stub

	}

	// 修改从机id
	@Override
	public void updateSubDevice(RoutingInspectionStationDetail stationDetail, int row) throws Exception {
		List<RoutingInspectionDetail> updateSubDeviceList = stationDetail.getRoutingInspectionDetailList();
		// 循环修改从机id
		RoutingInspectionDetail routingInpectionDetail = updateSubDeviceList.get(0);
		SubDevice querySubdevice = new SubDevice();
		querySubdevice.setGprsId(stationDetail.getGprsId());
		querySubdevice.setSubDeviceIdOut(routingInpectionDetail.getDetailOperateValueOld());
		List<SubDevice> oldSubList = subDeviceMapper.selectListSelective(querySubdevice);
		if (oldSubList.size() != 0) {
			for (SubDevice master : oldSubList) {
				if (routingInpectionDetail.getCellIndex() == master.getCellSort()) {
					// 判断新的从机是否存在
					querySubdevice.setGprsId(null);
					querySubdevice.setSubDeviceIdOut(routingInpectionDetail.getDetailOperateValueNew());
					List<SubDevice> newSubList = subDeviceMapper.selectListSelective(querySubdevice);
					if (newSubList.size() > 0) {
						if (newSubList.get(0).getSubFlag() == 1) {
							// 发送指令
							ModifyGprsidSend send = new ModifyGprsidSend();
							send.setGprsId(stationDetail.getGprsId());
							send.setInnerId(master.getSubDeviceId());
							send.setOuterId(routingInpectionDetail.getDetailOperateValueNew());
							send.setType(2);// 跟换从机
							modifyGprsidSendSer.changeDeviceId(send, row);

							// 更换从机保存
							RoutingInspections routingInspections = new RoutingInspections();
							routingInspections.setStationId(stationDetail.getStationId());
							routingInspections.setGprsId(master.getGprsId());
							//routingInspections.setRoutingInspectionStatus(1);// 安装维护状态
							// 通过电池组的状态在维护中 ，电池组id,设备id来判断是否有这条数据 如果没有就新增
							List<RoutingInspections> routingList = routingInspectionsSer.selectListSelectiveFirst(routingInspections);
							if (routingList.size() == 0) {
								if (stationDetail.getOperateTime() != null) {
									routingInspections.setOperateTime(stationDetail.getOperateTime());
								} else {
									Date date = new Date();
									routingInspections.setOperateTime(date);
								}
								routingInspections.setRoutingInspectionStatus(0);//提交之前的状态
								routingInspections.setDeviceType(master.getSubType());
								routingInspections.setOperateType(stationDetail.getOperateType());
								routingInspections.setOperateId(stationDetail.getOperateId());
								routingInspections.setOperateName(stationDetail.getOperateName());
								routingInspections.setOperatePhone(stationDetail.getOperatePhone());
								routingInspectionsSer.insertSelective(routingInspections);
								logger.info("新增巡检记录stationid({})--->({})完成",
										new Object[] { stationDetail.getStationId(), master.getSubDeviceId() });
							}
							
							Integer routingId = null;
							if(routingList.size() == 0) {
								routingId = routingInspections.getRoutingInspectionId();
							}else {
								routingId = routingList.get(0).getRoutingInspectionId();
							}
							RoutingInspectionDetail Detail = new RoutingInspectionDetail();
							Detail.setRoutingInspectionsId(routingId);
							Detail.setRequestType(1);//web回应
							// 查询出提交的最大次数
							List<RoutingInspectionDetail> routingDetailType = routingInspectionDetailSer.selectListSelective(Detail);
							Detail.setRequestType(0);//通过requestType == 0 查询出最大RequestSeq的数据
							List<RoutingInspectionDetail> routingDetailSeq = routingInspectionDetailSer.selectListSelective(Detail);
							if (routingDetailType.size() != 0) {
								Detail.setRoutingInspectionsId(routingId);
								Detail.setRequestType(0);
								RoutingInspectionDetail InspectionDetail = routingDetailSeq.stream().max(Comparator.comparing(RoutingInspectionDetail::getRequestSeq)).get();
								if (InspectionDetail.getRequestSeq() == null) {
									Detail.setRequestSeq(1);
								} else {
									Detail.setRequestSeq(InspectionDetail.getRequestSeq() + 1);
								}
								if (stationDetail.getOperateTime() != null) {
									Detail.setCreateTime(stationDetail.getOperateTime());
								}
								Detail.setDetailOperateId(stationDetail.getOperateId());
								Detail.setDetailOperateName(stationDetail.getOperateName());
								Detail.setDetailOperateType(4);// 跟换从机
								Detail.setDetailOperateValueOld(master.getSubDeviceId());
								Detail.setDetailOperateValueNew(routingInpectionDetail.getDetailOperateValueNew());
								Detail.setCellIndex(routingInpectionDetail.getCellIndex());
								routingInspectionDetailSer.insertSelective(Detail);

							} else {
								Detail.setRoutingInspectionsId(routingId);
								Detail.setRequestType(0);
								if (stationDetail.getOperateTime() != null) {
									Detail.setCreateTime(stationDetail.getOperateTime());
								}
								Detail.setRequestSeq(1);
								Detail.setDetailOperateId(stationDetail.getOperateId());
								Detail.setDetailOperateName(stationDetail.getOperateName());
								Detail.setDetailOperateType(4);// 跟换从机
								Detail.setDetailOperateValueOld(master.getSubDeviceId());
								Detail.setDetailOperateValueNew(routingInpectionDetail.getDetailOperateValueNew());
								Detail.setCellIndex(routingInpectionDetail.getCellIndex());
								routingInspectionDetailSer.insertSelective(Detail);
							}

						} else {
							throw new IllegalArgumentException("第" + routingInpectionDetail.getCellIndex() + "号新从机不是备用从机，请重新输入！");
						}

					} else {
						throw new IllegalArgumentException("第" + routingInpectionDetail.getCellIndex() + "号新从机ID不存在，请重新输入！");

					}
				} else {
					throw new IllegalArgumentException("第" + routingInpectionDetail.getCellIndex() + "号故障从机编号有误，请重新输入！");
				}
			}
		}else {
			throw new IllegalArgumentException("第" + routingInpectionDetail.getCellIndex() + "号故障从机ID不存在，请重新输入！");
		}
	}

	//删除多余的从机
	@Override
	public void deleteMoreSubDevice(Map map) {
		subDeviceMapper.deleteMoreSubDevice(map);
		
	}
}
