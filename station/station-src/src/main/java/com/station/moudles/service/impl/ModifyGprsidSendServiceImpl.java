package com.station.moudles.service.impl;

import com.google.common.collect.Lists;
import com.station.common.utils.BeanValueUtils;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.ModifyGprsidSend;
import com.station.moudles.entity.PackDataInfoLatest;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SubDevice;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.ModifyGprsidSendMapper;
import com.station.moudles.mapper.PackDataInfoLatestMapper;
import com.station.moudles.mapper.StationInfoMapper;
import com.station.moudles.mapper.SubDeviceMapper;
import com.station.moudles.service.ModifyGprsidSendService;
import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.StopWatch;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Map;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

@Service
public class ModifyGprsidSendServiceImpl extends BaseServiceImpl<ModifyGprsidSend, Integer>
		implements ModifyGprsidSendService {

	private final ModifyGprsidSendMapper modifyGprsidSendMapper;
	private final GprsConfigInfoMapper gprsConfigInfoMapper;
	private final StationInfoMapper stationInfoMapper;
	private final SubDeviceMapper subDeviceMapper;
	private final PackDataInfoLatestMapper packDataInfoLatestMapper;

	@Autowired
	public ModifyGprsidSendServiceImpl(ModifyGprsidSendMapper modifyGprsidSendMapper,
			GprsConfigInfoMapper gprsConfigInfoMapper, StationInfoMapper stationInfoMapper,
			SubDeviceMapper subDeviceMapper, PackDataInfoLatestMapper packDataInfoLatestMapper) {
		this.modifyGprsidSendMapper = modifyGprsidSendMapper;
		this.gprsConfigInfoMapper = gprsConfigInfoMapper;
		this.stationInfoMapper = stationInfoMapper;
		this.subDeviceMapper = subDeviceMapper;
		this.packDataInfoLatestMapper = packDataInfoLatestMapper;
		// 轮询modify_gprsid_send为send_done=2并且state=0
		Executors.newScheduledThreadPool(1).scheduleWithFixedDelay(() -> loopSendCommand(), 0, 5, TimeUnit.MINUTES);
	}

	private void loopSendCommand() {
		try {
			ModifyGprsidSend send = new ModifyGprsidSend();
			send.setSendDone(Integer.valueOf(2).byteValue());
			send.setState(0);
			List<ModifyGprsidSend> items = modifyGprsidSendMapper.selectListSelective(send);
			if (CollectionUtils.isEmpty(items)) {
				return;
			}
			StopWatch stopWatch = new StopWatch();
			stopWatch.start();
			logger.info("获取{}条修改主从指令成功但未被处理过的记录", items.size());
			for (ModifyGprsidSend item : items) {
				if (1 == item.getType()) {
					// 修改主机
					updateGprsConfigInfo(item);
					updateStationInfo(item);
				} else if (2 == item.getType()) {
					updateSubDevice(item);
				}
				// 标记为已处理
				ModifyGprsidSend updateItem = new ModifyGprsidSend();
				updateItem.setId(item.getId());
				updateItem.setState(1);
				modifyGprsidSendMapper.updateByPrimaryKeySelective(updateItem);
			}
			stopWatch.stop();
			logger.info("处理{}条修改主从指令成功记录，耗时:{}", items.size(), stopWatch);
		} catch (Exception e) {
			logger.error("ModifyGprsidSendServiceImpl 轮询器-->",e);
		}
	}

	private void updateGprsConfigInfo(ModifyGprsidSend item) {
		List<GprsConfigInfo> items = gprsConfigInfoMapper
				.selectByGprsIds(Lists.newArrayList(item.getInnerId(), item.getOuterId()));
		if (CollectionUtils.isEmpty(items)) {
			return;
		}
		Map<String, GprsConfigInfo> map = items.stream().collect(
				Collectors.toMap(key -> StringUtils.trimToEmpty(key.getGprsId()).toUpperCase(), value -> value));

		// 由于gprs_id_out有Unique限制，导致先修改会失败。因此先删除再修改
		GprsConfigInfo slaver = map.get(StringUtils.trimToEmpty(item.getOuterId()).toUpperCase());
		if (slaver == null) {
			return;
		}
		gprsConfigInfoMapper.deleteByPrimaryKey(slaver.getId());
		logger.info("删除设备配置:{}成功", slaver.getId());

		GprsConfigInfo master = map.get(StringUtils.trimToEmpty(item.getInnerId()).toUpperCase());
		if (master == null) {
			return;
		}
		GprsConfigInfo gprsConfigInfo = new GprsConfigInfo();
		gprsConfigInfo.setId(master.getId());
		gprsConfigInfo.setGprsIdOut(item.getOuterId());
		// 备用主机的标志修改
		gprsConfigInfo.setGprsFlag(0);
		gprsConfigInfo.setDeviceType(slaver.getDeviceType());
		gprsConfigInfo.setDevicePhone(slaver.getDevicePhone());
		gprsConfigInfo.setGprsPort(slaver.getGprsPort());
		gprsConfigInfo.setGprsSpec(slaver.getGprsSpec());
		gprsConfigInfoMapper.updateByPrimaryKeySelective(gprsConfigInfo);

		logger.info("更新设备配置外部基站ID({})--->({})完成，设备编号:{}/{}",
				new Object[] { master.getGprsIdOut(), item.getOuterId(), master.getGprsId(), master.getId() });
	}

	private void updateStationInfo(ModifyGprsidSend item) {
		StationInfo input = new StationInfo();
		input.setGprsId(item.getInnerId());
		List<StationInfo> items = stationInfoMapper.selectListSelective(input);
		if (CollectionUtils.isEmpty(items)) {
			return;
		}
		StationInfo info = items.get(0);
		StationInfo stationInfo = new StationInfo();
		stationInfo.setId(info.getId());
		stationInfo.setGprsIdOut(item.getOuterId());
		
		stationInfoMapper.updateByPrimaryKeySelective(stationInfo);
		logger.info("更新设备外部基站ID({})--->({})完成，设备编号:{}/{}",
				new Object[] { info.getGprsIdOut(), item.getOuterId(), info.getGprsId(), info.getId() });
	}

	private void updateSubDevice(ModifyGprsidSend item) {
		SubDevice querySubDevice = new SubDevice();
		querySubDevice.setGprsId(item.getGprsId());
		querySubDevice.setSubDeviceId(item.getInnerId());
		List<SubDevice> items = subDeviceMapper.selectListSelective(querySubDevice);
		if (CollectionUtils.isEmpty(items)) {
			return;
		}
		SubDevice querySubOut = new SubDevice();
		querySubOut.setSubDeviceId(item.getOuterId());
		List<SubDevice> slaverItem = subDeviceMapper.selectListSelective(querySubOut);
		if (CollectionUtils.isEmpty(slaverItem)) {
			return;
		}
		SubDevice slaver = slaverItem.get(0);
		SubDevice master = items.get(0);
		SubDevice subDevice = new SubDevice();
		subDevice.setId(master.getId());
		subDevice.setSubDeviceIdOut(item.getOuterId());
		// 备用从机的标志修改
		subDevice.setSubFlag(0);
		subDevice.setSubSpec(slaver.getSubSpec());
		subDevice.setSubType(slaver.getSubType());
		subDeviceMapper.updateByPrimaryKeySelective(subDevice);
		logger.info("更新备用从机外部ID({})--->({})完成，编号:{}",
				new Object[] { master.getSubDeviceIdOut(), item.getOuterId(), master.getId() });

//		SubDevice deleteSubDevice = new SubDevice();
//		deleteSubDevice.setGprsId(item.getGprsId());
//		deleteSubDevice.setSubFlag(1);
//		deleteSubDevice.setSubDeviceId(item.getOuterId());
//		int res = subDeviceMapper.deleteSelective(slaver);
		int res = subDeviceMapper.deleteByPrimaryKey(slaver.getId());
		logger.info("删除{}条备用从机完成，设备编号:{},从机编号:{}", new Object[] { res, item.getGprsId(), item.getOuterId() });
	}

	/**
	 * 主备机修改
	 *
	 * @param modifyGprsidSend
	 */
	private void updateMaster(ModifyGprsidSend modifyGprsidSend,int row) {
		final String master = modifyGprsidSend.getInnerId();
		final String slaver = modifyGprsidSend.getOuterId();

		List<GprsConfigInfo> items = gprsConfigInfoMapper.selectByGprsIds(Lists.newArrayList(master, slaver));
		if (CollectionUtils.isEmpty(items)) {
			throw new IllegalArgumentException("第"+row+"行请输入新的主机ID");
		}

		Map<String, GprsConfigInfo> map = items.stream()
				.collect(Collectors.toMap(key -> StringUtils.trimToEmpty(key.getGprsId()).toUpperCase(), item -> item));

		GprsConfigInfo masterInfo = map.get(StringUtils.trimToEmpty(master).toUpperCase());
		if (masterInfo == null) {
			throw new IllegalArgumentException("第"+row+"行请输入有效的主机信息");
		}

		GprsConfigInfo slaverInfo = map.get(StringUtils.trimToEmpty(slaver).toUpperCase());
		if (slaverInfo == null) {
			throw new IllegalArgumentException("第"+row+"行请输入有效的新主机信息");
		}
		//判断设备类型一直性
		if(masterInfo.getDeviceType().equals(slaverInfo.getDeviceType())) {
		}else {
			throw new IllegalArgumentException("修改的主机和备用主机的类型不一致！");
		}
	// 原来判断主机必须离线，备机必须在线
//		boolean statusSlaver =slaverInfo.getLinkStatus() != null && slaverInfo.getLinkStatus().intValue() == 1; 
//		boolean statusMaster =(masterInfo.getLinkStatus() != null && masterInfo.getLinkStatus().intValue() == 0) 
//				||null == masterInfo.getLinkStatus();
//		if (!statusSlaver || !statusMaster) {
//			throw new IllegalArgumentException("主机必须离线，备机必须在线");
//		}
	//---- 改  主机是否在线不判断，备机必须在线	 12/15
		boolean statusSlaver =slaverInfo.getLinkStatus() != null && slaverInfo.getLinkStatus().intValue() == 1; 
		if (!statusSlaver ) {
			throw new IllegalArgumentException("备机必须在线");
		}
	}

	/**
	 * 从机备用从机修改
	 *
	 * @param modifyGprsidSend
	 */
	private void updateSlaver(ModifyGprsidSend modifyGprsidSend ,int row) {
		GprsConfigInfo input = new GprsConfigInfo();
		input.setGprsId(modifyGprsidSend.getGprsId());
		List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoMapper.selectListSelective(input);
		if (CollectionUtils.isEmpty(gprsConfigInfos)) {
			throw new IllegalArgumentException("第"+row+"行请输入有效的基站信息");
		}
		SubDevice querySubDevice = new SubDevice();
		querySubDevice.setGprsId(modifyGprsidSend.getGprsId());
		querySubDevice.setSubDeviceId(modifyGprsidSend.getInnerId());
		List<SubDevice> items = subDeviceMapper.selectListSelective(querySubDevice);
		if (CollectionUtils.isEmpty(items)) {
			throw new IllegalArgumentException("第"+row+"行找不到从机设备从机编号配置数据");
		}
		SubDevice oldSubDevice = new SubDevice();
		oldSubDevice.setSubDeviceId(modifyGprsidSend.getOuterId());
		List<SubDevice> oldSub = subDeviceMapper.selectListSelective(oldSubDevice);
		//判断设备类型一直性
		if(oldSub.get(0).getSubType().equals(items.get(0).getSubType())) {
		}else {
			throw new IllegalArgumentException("从机和备用从机的类型不一致！");
		}
		// 从机的设备必须在线，从机需离线
		boolean status = gprsConfigInfos.get(0).getLinkStatus() != null
				&& gprsConfigInfos.get(0).getLinkStatus().intValue() == 1 && isSubDeviceOffline(items.get(0));
		if (!status) {
			throw new IllegalArgumentException("主机必须在线，从机需离线");
		}
	}

	/**
	 * 检查从机是否离线
	 *
	 * @param subDevice
	 * @return true-->离线, false-->在线
	 */
	private boolean isSubDeviceOffline(SubDevice subDevice) {
		PackDataInfoLatest packDataInfoLatest = new PackDataInfoLatest();
		packDataInfoLatest.setGprsId(subDevice.getGprsId());
		List<PackDataInfoLatest> items = packDataInfoLatestMapper.selectListSelective(packDataInfoLatest);
		if (CollectionUtils.isEmpty(items)) {
			return true;
		}
		PackDataInfoLatest item = items.get(0);
		Integer comSuc = (Integer) BeanValueUtils.getValue("comSuc" + subDevice.getCellSort(), item);
		return comSuc == null || comSuc == 0;
	}

	@Override
	public void changeDeviceId(ModifyGprsidSend modifyGprsidSend, int row) {
		if (StringUtils.isBlank(modifyGprsidSend.getGprsId()) || StringUtils.isBlank(modifyGprsidSend.getInnerId())
				|| StringUtils.isBlank(modifyGprsidSend.getOuterId())) {
			throw new IllegalArgumentException("请输入设备编号");
		}
		if (1 == modifyGprsidSend.getType()) {
			updateMaster(modifyGprsidSend,row);
		} else if (2 == modifyGprsidSend.getType()) {
			updateSlaver(modifyGprsidSend,row);
		} else {
			throw new IllegalArgumentException("不支持的操作");
		}
		modifyGprsidSend.setSendDone(Integer.valueOf(0).byteValue());
		modifyGprsidSendMapper.insertSelective(modifyGprsidSend);
	}

}