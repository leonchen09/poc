package com.station.moudles.quartz;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.Timer;
import java.util.TimerTask;
import java.util.stream.Collectors;

import org.apache.commons.collections.CollectionUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;

import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.ReflectUtil;
import com.station.moudles.entity.GprsBalanceSend;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.RoutingInspectionDetail;
import com.station.moudles.entity.RoutingInspectionPackData;
import com.station.moudles.mapper.PackDataInfoLatestMapper;
import com.station.moudles.mapper.PackDataInfoMapper;
import com.station.moudles.mapper.RoutingInspectionDetailMapper;
import com.station.moudles.service.GprsBalanceSendService;
import com.station.moudles.service.GprsConfigInfoService;

public class AutoBalanceTask {

	private static final Logger logger = LoggerFactory.getLogger(AutoBalanceTask.class);

	// private static final int CHECK_INTERVAL = 10;// 默认检查的时间间隔(分钟)。
	private static final int MIN_DISCHARGE_COUNT = 10;// 检查时间内，最小放电条数。
	private final static String DISCHARGE_NAME = "放电";
	private final static String CELL_PREFIX = "cell";
	private final static String CELL_VOL_PREFIX = "cellVol";
	private Map<String, AutoBalanceRecord> dischargeRecordMap = new HashMap<>();
	private Map<String, GprsBalanceSend> balanceSendMap = new HashMap<>();

	@Autowired
	GprsConfigInfoService gprsConfigInfoService;
	@Autowired
	PackDataInfoMapper packDataInfoMapper;
	@Autowired
	PackDataInfoLatestMapper packDataInfoLatestMapper;
	@Autowired
	RoutingInspectionDetailMapper routingInspectionDetailMapper;
	@Autowired
	GprsBalanceSendService gprsBalanceSendSer;

	public void execute() {
		try {
			Map<String, Object> param = new HashMap<>();
			param.put("state", DISCHARGE_NAME);
			param.put("operateType", 3);
			// 得到正在"放电"且为"更换单体"的巡检记录
			List<RoutingInspectionPackData> routingCellList = packDataInfoLatestMapper.selectListHasChangeCell(param);
			if (!CollectionUtils.isEmpty(routingCellList)) {
				Map<String, List<RoutingInspectionPackData>> routingCellMap = routingCellList.stream()
						.collect(Collectors.groupingBy(RoutingInspectionPackData::getGprsId));
				for (String gprsId : routingCellMap.keySet()) {
					if (dischargeRecordMap.containsKey(gprsId)) {
						// 该电池组还在放电
						// 再次均衡
						RoutingInspectionPackData inspectionPackData = routingCellMap.get(gprsId).get(0);
						PackDataInfo packDataInfo = new PackDataInfo();
						BeanUtils.copyProperties(inspectionPackData, packDataInfo);
						againBalance(gprsId, packDataInfo);
					} else {
						// 执行第一次均衡
						firstBalance(gprsId, routingCellMap);
					}
				}

				List<String> removeGprsIdList = new ArrayList<>(); // 记录要移除缓存的 gprsId
				for (String gprsId : dischargeRecordMap.keySet()) {
					if (!routingCellMap.containsKey(gprsId)) {
						// 该电池组，当前不是放电状态；查询是否有10条非放电数据
						List<PackDataInfo> selectListByTime = get10PackDataInfosByTime(gprsId, new Date());
						// 校验是否都为非放电数据
						if (stateVerify(selectListByTime, DISCHARGE_NAME, true)) {
							// 关闭均衡，清除缓存
							if (balanceSendMap.containsKey(gprsId)) {
								GprsBalanceSend send = new GprsBalanceSend();
								for (int i = 1; i < 25; i++) {
									ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 0);
								}
								send.setDuration(60);// 60分钟
								send.setGprsId(gprsId);
								send.setMode(0);
								gprsBalanceSendSer.send(send);
								balanceSendMap.remove(gprsId);// 移除均衡指令
								removeGprsIdList.add(gprsId);
							} else {
								// 表示从来没发过指令，但是有记录 。eg：k=24 没有满足发送条件，直接放电结束
								removeGprsIdList.add(gprsId);
							}
						} else {
							// 数据乱跳，默认放电，执行再次均衡
							PackDataInfo packDataInfo = selectListByTime.stream()
									.filter(p -> p.getState().equals(DISCHARGE_NAME))
									.max(Comparator.comparing(PackDataInfo::getId)).get();
							againBalance(gprsId, packDataInfo);
						}
					}
				}
				// 清除缓存
				for (String gprsId : removeGprsIdList) {
					dischargeRecordMap.remove(gprsId);
				}
			}
		} catch (Exception e) {
			logger.error(this.getClass().getName() + "-->" + e.toString());
		}
	}

	/**
	 * RoutingInspectionPackData 有pack_data_info_latest中的数据 和 巡检记录
	 * 
	 * @param gprsId
	 * @param routingCellMap
	 */
	private void firstBalance(String gprsId, Map<String, List<RoutingInspectionPackData>> routingCellMap) {
		RoutingInspectionPackData riPd = routingCellMap.get(gprsId).get(0);
		// 查询 pack_data_info
		List<PackDataInfo> selectListByTime = get10PackDataInfosByTime(riPd.getGprsId(), riPd.getRcvTime());
		if (CollectionUtils.isEmpty(selectListByTime) || selectListByTime.size() < MIN_DISCHARGE_COUNT) {
			// 查询的数据为空或数量少于10条
			return;
		}
		// 校验 10条数据是否都是 '放电'
		if (stateVerify(selectListByTime, DISCHARGE_NAME, false)) {
			BigDecimal genVol = riPd.getGenVol();
			if (genVol.compareTo(new BigDecimal(53)) < 0) { // 判断电压
				AutoBalanceRecord balanceRecord = new AutoBalanceRecord();
				balanceRecord.setGprsId(gprsId);
				// 设置放电开始时间
				Integer maxId = selectListByTime.stream().max(Comparator.comparing(PackDataInfo::getId)).get().getId();
				List<PackDataInfo> dischargeDatas = forwardLookup(maxId, gprsId, 50);
				Date dischargeStartTime = dischargeDatas.stream().min(Comparator.comparing(PackDataInfo::getRcvTime))
						.get().getRcvTime();
				balanceRecord.setDischargeStartTime(dischargeStartTime);
				// 判断电压执行 '容量均衡'或'电压均衡'
				if (genVol.compareTo(new BigDecimal(48)) > 0) {
					// 容量均衡 --> 初始指令发送
					Set<Integer> cellIndexs = null; // 新电池的编号
					// 查询 '巡检详情表' 得到新电池的数量和编号
					RoutingInspectionDetail record = new RoutingInspectionDetail();
					record.setRoutingInspectionsId(riPd.getRoutingInspectionId());
					record.setDetailOperateType(5);
					record.setDetailOperateValueNew("新电池");
					List<RoutingInspectionDetail> inspectionDetails = routingInspectionDetailMapper
							.selectListSelective(record);
					if (CollectionUtils.isEmpty(inspectionDetails)) {
						balanceRecord.setNewCellCount(0);
					} else {
						balanceRecord.setNewCellCount(inspectionDetails.size());
						cellIndexs = inspectionDetails.stream().map(r -> r.getCellIndex()).collect(Collectors.toSet());
					}
					// 对每个单体设置均衡指令
					if (balanceRecord.getNewCellCount() < 24) {
						if (balanceRecord.getNewCellCount() >= 12) {
							// 新电池个数 >= 12 && < 24
							balanceSendGt12(gprsId, balanceRecord, cellIndexs);
						} else {
							// 新电池个数 < 12
							balanceSendLt12(gprsId, balanceRecord, cellIndexs);
						}
					} else {
						// 24 个都是新电池, 只记录时间，不发指令
						Date startTime = new Date();
						balanceRecord.setBalanceStartTime(startTime);
						balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, 60));
						dischargeRecordMap.put(gprsId, balanceRecord);
					}
				} else {
					// 电压均衡--> 均衡控制
					balanceRecord.setNewCellCount(-1);
					PackDataInfo packDataInfo = new PackDataInfo();
					BeanUtils.copyProperties(riPd, packDataInfo);
					volBalanceCommand(balanceRecord, packDataInfo);
				}
			}
		}
	}

	private void againBalance(String gprsId, PackDataInfo packDataInfo) {
		AutoBalanceRecord balanceRecord = dischargeRecordMap.get(gprsId);
		// 判断时间
		if (balanceRecord.getBalanceEndTime().compareTo(new Date()) < 0) {
			BigDecimal genVol = packDataInfo.getGenVol();
			if (genVol.compareTo(new BigDecimal(53)) < 0) { // 判断电压
				if (genVol.compareTo(new BigDecimal(48)) > 0) {
					// 容量均衡，均衡控制
					capBalanceCommand(gprsId, balanceRecord, genVol);
				} else {
					if (balanceRecord.getNewCellCount() >= 0 && balanceRecord.getNewCellCount() < 24) {
						// 电压均衡，由容量均衡进入
						volBalanceFromCap(balanceRecord, packDataInfo);
					} else {
						// 电压均衡，均衡控制
						volBalanceCommand(balanceRecord, packDataInfo);
					}
				}
			}
		}
	}

	private void capBalanceCommand(String gprsId, AutoBalanceRecord balanceRecord, BigDecimal genVol) {
		if (balanceRecord.getNewCellCount() >= 0 && balanceRecord.getNewCellCount() < 24) {
			GprsBalanceSend gprsBalanceSend = balanceSendMap.get(gprsId);
			gprsBalanceSend.setId(null);
			gprsBalanceSend.setDuration(null);
			gprsBalanceSend.setSendDone(null);
			gprsBalanceSend.setSendTime(null);
			if (genVol.compareTo(new BigDecimal(50)) >= 0) {
				gprsBalanceSend.setDuration(60);
			} else if (genVol.compareTo(new BigDecimal(48.5)) >= 0) {
				gprsBalanceSend.setDuration(30);
			} else {
				gprsBalanceSend.setDuration(10);
			}
			gprsBalanceSendSer.send(gprsBalanceSend);

			// 更新缓存
			Date startTime = gprsBalanceSend.getSendTime();
			balanceRecord.setBalanceStartTime(startTime);
			balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, gprsBalanceSend.getDuration()));

		} else if (balanceRecord.getNewCellCount() == 24) {
			int duration = 0;
			if (genVol.compareTo(new BigDecimal(50)) >= 0) {
				duration = 60;
			} else if (genVol.compareTo(new BigDecimal(48.5)) >= 0) {
				duration = 30;
			} else {
				duration = 10;
			}
			// 更新缓存
			Date startTime = new Date();
			balanceRecord.setBalanceStartTime(startTime);
			balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, duration));
		}
	}

	private void balanceSendGt12(String gprsId, AutoBalanceRecord balanceRecord, Set<Integer> cellIndexs) {
		// 1、2#单体设为降压，自3#单体往后按顺序将新电池设为升压，升压个数达到12后，其余设为降压
		// 0: 关闭 2:升压 3:降压
		GprsBalanceSend send = new GprsBalanceSend();
		int volUpCount = 0;
		for (int i = 1; i < 25; i++) {
			if (i <= 2) {
				ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 3);
			} else {
				if (cellIndexs.contains(i)) {
					if (volUpCount >= 12) {
						ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 3);
					} else {
						ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 2);
					}
					volUpCount++;
				} else {
					ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 3);
				}
			}
		}
		send.setDuration(60); // 60分钟
		send.setGprsId(gprsId);
		send.setMode(0);
		gprsBalanceSendSer.send(send);
		// 缓存 均衡指令
		balanceSendMap.put(gprsId, send);
		// 记录 均衡开始时间、结束时间
		Date startTime = new Date();
		balanceRecord.setBalanceStartTime(startTime);
		balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, 60));
		dischargeRecordMap.put(gprsId, balanceRecord); // 放入缓存
	}

	private void balanceSendLt12(String gprsId, AutoBalanceRecord balanceRecord, Set<Integer> cellIndexs) {
		// 1、2#单体设为降压，自3#单体往后将新电池设为升压，其余设为降压，
		// 如1、2#单体中有新电池,则5分钟后将其设为升压，其余保持原配置，再发一次指令。
		// 0: 关闭 2:升压 3:降压
		GprsBalanceSend send = new GprsBalanceSend();
		for (int i = 1; i < 25; i++) {
			if (i <= 2) {
				ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 3);
			} else {
				if (cellIndexs.contains(i)) {
					ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 2);
				} else {
					ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 3);
				}
			}
		}
		send.setDuration(30); // 30分钟
		send.setGprsId(gprsId);
		send.setMode(0);
		gprsBalanceSendSer.send(send);
		// 缓存 均衡指令
		balanceSendMap.put(gprsId, send);
		// 记录 均衡开始时间、结束时间
		Date startTime = send.getSendTime();
		balanceRecord.setBalanceStartTime(startTime);
		balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, 30));
		dischargeRecordMap.put(gprsId, balanceRecord); // 放入缓存

		if (cellIndexs.contains(1) || cellIndexs.contains(2)) {
			// 如1、2#单体中有新电池 ,5 分钟后，将其设为升压，其余不变，再发一次指令
			Timer timerTask = new Timer();
			timerTask.schedule(new TimerTask() {
				@Override
				public void run() {
					GprsBalanceSend twiceSend = new GprsBalanceSend();
					for (int i = 1; i < 25; i++) {
						if (cellIndexs.contains(i)) {
							ReflectUtil.setValueByKet(twiceSend, CELL_PREFIX + i, (byte) 2);
						} else {
							ReflectUtil.setValueByKet(twiceSend, CELL_PREFIX + i, (byte) 3);
						}
					}
					twiceSend.setDuration(30); // 30分钟
					twiceSend.setGprsId(gprsId);
					twiceSend.setMode(0);
					// 再次发送
					gprsBalanceSendSer.send(twiceSend);
					// 均衡指令 缓存修改
					balanceSendMap.put(gprsId, twiceSend);
					// 记录 均衡开始时间、结束时间、放电开始时间
					Date twiceStartTime = twiceSend.getSendTime();
					balanceRecord.setBalanceStartTime(twiceStartTime);
					balanceRecord.setBalanceEndTime(MyDateUtils.add(twiceStartTime, Calendar.MINUTE, 30));
					dischargeRecordMap.put(gprsId, balanceRecord); // 缓存修改
				}
			}, 300000);
		}
	}

	private void volBalanceFromCap(AutoBalanceRecord balanceRecord, PackDataInfo packDataInfo) {
		int volUpCount = 0;
		int volDownCount = 0;
		List<CellInfoHelper> volUpList = new ArrayList<>();
		BigDecimal averageVol = getAverageVol(packDataInfo);
		GprsBalanceSend gprsBalanceSend = balanceSendMap.get(balanceRecord.getGprsId());
		gprsBalanceSend.setId(null);
		gprsBalanceSend.setDuration(null);
		gprsBalanceSend.setSendDone(null);
		gprsBalanceSend.setSendTime(null);
		// 0: 关闭 2:升压 3:降压
		for (int i = 1; i < 25; i++) {
			Object obj = ReflectUtil.getValueByKey(gprsBalanceSend, CELL_PREFIX + i);
			int cell = Integer.valueOf(obj.toString());
			Object objVol = ReflectUtil.getValueByKey(packDataInfo, CELL_VOL_PREFIX + i);
			BigDecimal vol = new BigDecimal(objVol == null ? "0" : objVol.toString());
			if (cell == 2) {
				// 升压变关闭
				if (vol.compareTo(averageVol) <= 0) {
					ReflectUtil.setValueByKet(gprsBalanceSend, CELL_PREFIX + i, (byte) 0);
				} else {
					CellInfoHelper helper = new CellInfoHelper();
					helper.setCellIndex(i);
					helper.setCellVol(vol);
					volUpList.add(helper);
					volUpCount++;
				}
			} else if (cell == 3) {
				volDownCount++;
				// 降压改为升压
				if (vol.compareTo(averageVol.add(new BigDecimal(0.2))) > 0) {
					ReflectUtil.setValueByKet(gprsBalanceSend, CELL_PREFIX + i, (byte) 2);
					volDownCount--;
					volUpCount++;
					CellInfoHelper helper = new CellInfoHelper();
					helper.setCellIndex(i);
					helper.setCellVol(vol);
					volUpList.add(helper);
				}
			}
		}

		// 如升压个数大于降压个数，则将离平均电压最小升压的依次变为关闭,直到升压个数等于降压个数
		if (volUpCount > volDownCount) {
			int count = volUpCount - volDownCount;
			volUpList.sort(Comparator.comparing(CellInfoHelper::getCellVol));
			for (int i = 0; i < count; i++) {
				Integer index = volUpList.get(i).getCellIndex();
				ReflectUtil.setValueByKet(gprsBalanceSend, CELL_PREFIX + index, (byte) 0);
			}
		}
		gprsBalanceSend.setDuration(30);
		gprsBalanceSendSer.send(gprsBalanceSend);

		// 更新缓存
		Date startTime = gprsBalanceSend.getSendTime();
		balanceRecord.setBalanceStartTime(startTime);
		balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, 30));
	}

	private void volBalanceCommand(AutoBalanceRecord balanceRecord, PackDataInfo packDataInfo) {
		int volUpCount = 0; // 记录升压
		int volDownCount = 0;// 记录降压
		List<CellInfoHelper> volUpList = new ArrayList<>();// 记录升压
		List<CellInfoHelper> volCloseList = new ArrayList<>();// 记录关闭
		BigDecimal averageVol = getAverageVol(packDataInfo);
		GprsBalanceSend send = new GprsBalanceSend();
		for (int i = 1; i < 25; i++) {
			// 0: 关闭 2:升压 3:降压
			/*
			 * avg + 200< vol 升； avg - 200 <= vol <= avg + 200 关； vol < avg - 200 降
			 */
			Object obj = ReflectUtil.getValueByKey(packDataInfo, CELL_VOL_PREFIX + i);
			BigDecimal vol = new BigDecimal(obj == null ? "0" : obj.toString());
			if (vol.compareTo(averageVol.add(new BigDecimal(0.2))) > 0) {
				ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 2);
				volUpCount++;
				CellInfoHelper helper = new CellInfoHelper();
				helper.setCellIndex(i);
				helper.setCellVol(vol);
				volUpList.add(helper);
			} else if (vol.compareTo(averageVol.add(new BigDecimal(-0.2))) >= 0) {
				ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 0);
				CellInfoHelper helper = new CellInfoHelper();
				helper.setCellIndex(i);
				helper.setCellVol(vol);
				volCloseList.add(helper);
			} else {
				ReflectUtil.setValueByKet(send, CELL_PREFIX + i, (byte) 3);
				volDownCount++;
			}
		}
		// 如升压个数大于降压个数，则将小于平均电压最大单体的依次变为降压,直到升压个数等于降压个数
		if (volUpCount > volDownCount) {
			int count = volUpCount - volDownCount;
			volCloseList.sort(Comparator.comparing(CellInfoHelper::getCellVol));
			if (count > volCloseList.size()) {
				for (int i = 0; i < volCloseList.size(); i++) {
					Integer index = volCloseList.get(i).getCellIndex();
					ReflectUtil.setValueByKet(send, CELL_PREFIX + index, (byte) 3);
				}
				// 多出的个数,把升压变为关闭
				count = count - volCloseList.size();
				volUpList.sort(Comparator.comparing(CellInfoHelper::getCellVol));
				for (int i = 0; i < count; i++) {
					Integer index = volUpList.get(i).getCellIndex();
					ReflectUtil.setValueByKet(send, CELL_PREFIX + index, (byte) 0);
				}
			} else {
				for (int i = 0; i < count; i++) {
					Integer index = volCloseList.get(i).getCellIndex();
					ReflectUtil.setValueByKet(send, CELL_PREFIX + index, (byte) 3);
				}
			}
		}
		send.setGprsId(packDataInfo.getGprsId());
		send.setDuration(10); // 10分钟
		send.setMode(0);
		gprsBalanceSendSer.send(send);
		balanceSendMap.put(packDataInfo.getGprsId(), send);
		Date startTime = send.getSendTime();
		balanceRecord.setBalanceStartTime(startTime);
		balanceRecord.setBalanceEndTime(MyDateUtils.add(startTime, Calendar.MINUTE, 10));
		dischargeRecordMap.put(packDataInfo.getGprsId(), balanceRecord);
	}

	private BigDecimal getAverageVol(PackDataInfo packDataInfo) {
		BigDecimal avg = BigDecimal.ZERO;
		for (int i = 1; i < 25; i++) {
			Object obj = ReflectUtil.getValueByKey(packDataInfo, CELL_VOL_PREFIX + i);
			BigDecimal vol = new BigDecimal(obj == null ? "0" : obj.toString());
			avg = avg.add(vol);
		}
		avg = avg.divide(BigDecimal.valueOf(24), 2, BigDecimal.ROUND_HALF_UP);
		return avg;
	}

	/**
	 * 判断集合是否都为指定状态
	 * 
	 * @param list
	 * @param state
	 *            判断状态
	 * @param isContrary
	 *            是否判断相反的状态
	 * @return
	 */
	private boolean stateVerify(List<PackDataInfo> list, String state, boolean isContrary) {
		for (PackDataInfo packDataInfo : list) {
			if (!isContrary && !state.equals(packDataInfo.getState())) {
				return false;
			}
			if (isContrary && state.equals(packDataInfo.getState())) {
				return false;
			}
		}
		return true;
	}

	/**
	 * 得到指定时间之前的10条记录
	 * 
	 * @param bean
	 * @return
	 */
	private List<PackDataInfo> get10PackDataInfosByTime(String gprsId, Date endTime) {
		Map<String, Object> param = new HashMap<>();
		param.put("gprsId", gprsId);
		param.put("endTime", endTime);
		param.put("pageNum", 0);
		param.put("pageSize", 10);
		return packDataInfoMapper.getPackDataInfosByTimes(param);
	}

	private List<PackDataInfo> forwardLookup(Integer startId, String gprsId, int pageSize) {
		List<PackDataInfo> results = Lists.newArrayList();
		// 向前找非放电态的数据
		int pageNum = 0;
		int count = 0;
		while (true) {
			Map<String, Object> paramMap = Maps.newHashMap();
			paramMap.put("gprsId", gprsId);
			paramMap.put("id", startId);
			paramMap.put("pageNum", pageNum);
			paramMap.put("pageSize", pageSize);
			List<PackDataInfo> items = packDataInfoMapper.getPackDataInfosWhichIdLessThanGivenValue(paramMap);
			if (CollectionUtils.isEmpty(items)) {
				break;
			}
			for (PackDataInfo item : items) {
				results.add(item);
				if (!DISCHARGE_NAME.equals(item.getState())) {
					count++;
				} else {
					count = 0;
				}
				if (count >= 10) {
					break;
				}
			}
			if (count >= 10) {
				break;
			}
			if (items.size() < pageSize) {
				break;
			}
			startId = items.get(items.size() - 1).getId();
			pageNum += pageSize;
		}
		results = results.stream().filter(k -> DISCHARGE_NAME.equals(k.getState())).collect(Collectors.toList());
		return results;
	}

	private List<PackDataInfo> backwardLookup(Integer startId, String gprsId, int pageSize) {
		List<PackDataInfo> results = Lists.newArrayList();
		// 向后找连续非放电态的数据
		int pageNum = 0;
		int count = 0;
		while (true) {
			Map<String, Object> paramMap = Maps.newHashMap();
			paramMap.put("gprsId", gprsId);
			paramMap.put("id", startId);
			paramMap.put("pageNum", pageNum);
			paramMap.put("pageSize", pageSize);
			List<PackDataInfo> items = packDataInfoMapper.getPackDataInfosWhichGreaterThanGivenValue(paramMap);
			if (CollectionUtils.isEmpty(items)) {
				break;
			}
			for (PackDataInfo item : items) {
				results.add(item);
				if (!DISCHARGE_NAME.equals(item.getState())) {
					count++;
				} else {
					count = 0;
				}
				if (count >= 2) {
					break;
				}
			}
			if (count >= 2) {
				break;
			}
			if (items.size() < pageSize) {
				break;
			}
			startId = items.get(items.size() - 1).getId();
			pageNum += pageSize;
		}
		// results = results.stream().filter(k ->
		// DISCHARGE_NAME.equals(k.getState())).collect(Collectors.toList());
		return results;
	}

	class CellInfoHelper {
		private Integer cellIndex;
		private BigDecimal cellVol;

		public Integer getCellIndex() {
			return cellIndex;
		}

		public void setCellIndex(Integer cellIndex) {
			this.cellIndex = cellIndex;
		}

		public BigDecimal getCellVol() {
			return cellVol;
		}

		public void setCellVol(BigDecimal cellVol) {
			this.cellVol = cellVol;
		}
	}

}
