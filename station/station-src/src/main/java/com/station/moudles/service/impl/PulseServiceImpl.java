package com.station.moudles.service.impl;

import com.google.common.base.Preconditions;
import com.google.common.collect.Lists;
import com.station.common.utils.MyDateUtils;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.PulseDischargeSendMapper;
import com.station.moudles.mapper.StationInfoMapper;
import com.station.moudles.service.CellInfoService;
import com.station.moudles.service.PulseService;
import com.station.moudles.vo.PulseVo;
import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.apache.commons.lang3.time.StopWatch;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.concurrent.ThreadPoolTaskExecutor;
import org.springframework.stereotype.Service;

import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.concurrent.Executors;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

@Service
public class PulseServiceImpl implements PulseService {

    private static final Logger LOGGER = LoggerFactory.getLogger(PulseServiceImpl.class);
    private static final int THRESHOLD = 10;

    private final StationInfoMapper stationInfoMapper;
    private final GprsConfigInfoMapper gprsConfigInfoMapper;
    private final PulseDischargeSendMapper pulseDischargeSendMapper;
    private final CellInfoService cellInfoService;
    private final ThreadPoolTaskExecutor taskExecutor;

    @Autowired
    public PulseServiceImpl(StationInfoMapper stationInfoMapper,
                            GprsConfigInfoMapper gprsConfigInfoMapper,
                            PulseDischargeSendMapper pulseDischargeSendMapper,
                            CellInfoService cellInfoService,
                            ThreadPoolTaskExecutor taskExecutor) {
        this.stationInfoMapper = stationInfoMapper;
        this.gprsConfigInfoMapper = gprsConfigInfoMapper;
        this.pulseDischargeSendMapper = pulseDischargeSendMapper;
        this.cellInfoService = cellInfoService;
        this.taskExecutor = taskExecutor;
        // 将send_done=1并且超过了10分钟的数据更新为超时,
//        	该逻辑不需要，由适配器完成，leonchen注释
//        Executors.newScheduledThreadPool(1).scheduleWithFixedDelay(() -> updateAsTimeout(),
//                0, THRESHOLD, TimeUnit.MINUTES);
    }

    private void updateAsTimeout() {
        Date specifiedTime = DateUtils.addMinutes(new Date(), -THRESHOLD);
        List<PulseDischargeSend> unProcessedItems = pulseDischargeSendMapper.getRecordsWhichSentButUnPorcessed(specifiedTime);
        LOGGER.debug("获取{}条状态为1未响应的数据,时间:{}", unProcessedItems.size(), MyDateUtils.getDateString(specifiedTime));
        if (CollectionUtils.isEmpty(unProcessedItems)) {
            return;
        }
        StopWatch stopWatch = new StopWatch();
        stopWatch.start();
        for (PulseDischargeSend item : unProcessedItems) {
            pulseDischargeSendMapper.updateAsTimeout(item.getId());
        }
        stopWatch.stop();
        LOGGER.debug("更新{}条状态为1未响应的数据完成，耗时:{}", unProcessedItems.size(), stopWatch);
    }

    @Override
    public void pulseTest(PulseVo pulseVo) {
        checkPayload(pulseVo);

        List<Integer> cells = Lists.newArrayList();
        if (pulseVo.getCellIndex() != null && pulseVo.getCellIndex() > 0 && pulseVo.getCellIndex() < 25) {
            cells.add(pulseVo.getCellIndex());
        }
        if (CollectionUtils.isEmpty(cells)) {
            cells.addAll(Lists.newArrayList(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11,
                    12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24));
        }

        List<String> gprsIds = Lists.newArrayList();
        if (pulseVo.getCompanyId() != null) {
            StationInfo stationInfo = new StationInfo();
            stationInfo.setCompanyId3(pulseVo.getCompanyId());
            List<StationInfo> stationInfos = stationInfoMapper.selectListSelective(stationInfo);
            if (CollectionUtils.isNotEmpty(stationInfos)) {
                gprsIds.addAll(stationInfos.stream().map(StationInfo::getGprsId).collect(Collectors.toSet()));
            }
        } else if (CollectionUtils.isNotEmpty(pulseVo.getGprsIdList())) {
            gprsIds.addAll(pulseVo.getGprsIdList().stream().collect(Collectors.toSet()));
        }
        if (CollectionUtils.isEmpty(gprsIds)) {
            return;
        }
        taskExecutor.execute(() -> {
            for (final String gprsId : gprsIds) {
                sendCommand(gprsId, cells, pulseVo);
            }
        });
    }

    private void sendCommand(String gprsid, List<Integer> cells, PulseVo pulseVo) {
        if (StringUtils.isBlank(gprsid) || StringUtils.equalsIgnoreCase(gprsid, "-1")) {
            return;
        }
        StopWatch stopWatch = new StopWatch();
        stopWatch.start();
        Map<Integer, List<PulseDischargeSend>> commandMap = pulseDischargeSendMapper.getLatestUnProcessedCommands(gprsid)
                .stream()
                .collect(Collectors.groupingBy(PulseDischargeSend::getPulseCell));
        List<Integer> sendDones = Lists.newArrayList();
        List<Integer> unSendDones = Lists.newArrayList();
        for (Integer cell : cells) {
            LOGGER.info("开始处理基站({})第({})号单体", gprsid, cell);
            List<PulseDischargeSend> sends = commandMap.get(cell);
            if (CollectionUtils.isEmpty(sends)) {
                normalSend(gprsid, cell, pulseVo);
                sendDones.add(cell);
            } else {
                Map<Integer, List<PulseDischargeSend>> sendDoneMap = sends.stream()
                        .collect(Collectors.groupingBy(item -> item.getSendDone().intValue()));
                if (CollectionUtils.isNotEmpty(sendDoneMap.get(0))) {
                    unSendDones.add(cell);
                    continue;
                }
                boolean waiting = false;
                List<PulseDischargeSend> unProcessedSends = sendDoneMap.getOrDefault(1, Lists.newArrayList());
                for (PulseDischargeSend item : unProcessedSends) {
                    long diff = new Date().getTime() - item.getInsertTime().getTime();
                    if (diff > TimeUnit.MINUTES.toMillis(THRESHOLD)) {
                        // 超时，由适配器处理超时，
//                        pulseDischargeSendMapper.updateAsTimeout(item.getId());
//                        updateCellInfo(gprsid, cell, 6);
                    } else {
                        waiting = true;
                        break;
                    }
                }
                if (waiting) {
                    unSendDones.add(cell);
                } else {
                    normalSend(gprsid, cell, pulseVo);
                    sendDones.add(cell);
                }
            }
        }
        LOGGER.info("{}个单体({})发送成功,{}个单体({})发送失败,基站编号:{},耗时:{}",
                new Object[]{sendDones.size(), StringUtils.join(sendDones, ","),
                        unSendDones.size(), StringUtils.join(unSendDones, ","),
                        gprsid, stopWatch.toString()});
    }

    private void normalSend(String gprsid, Integer cell, PulseVo pulseVo) {
        saveCommand(gprsid, cell, pulseVo);
        updateCellInfo(gprsid, cell, 0);
        updateGprsConfigInfo(gprsid, pulseVo);
    }

    private void updateGprsConfigInfo(String gprsid, PulseVo pulseVo) {
        GprsConfigInfo condition = new GprsConfigInfo();
        condition.setGprsId(gprsid);
        List<GprsConfigInfo> configs = gprsConfigInfoMapper.selectListSelective(condition);
       
        
      
        
        
        
        if (CollectionUtils.isEmpty(configs)) {
            return;
        }
        GprsConfigInfo gprsConfigInfo = configs.get(0);
        gprsConfigInfo.setFastSampleInterval(pulseVo.getFastSampleInterval());
        gprsConfigInfo.setDischargeTime(pulseVo.getDischargeTime());
        gprsConfigInfo.setSlowSampleInterval(pulseVo.getSlowSampleInterval());
        gprsConfigInfo.setSlowSampleTime(pulseVo.getSlowSampleTime());
        gprsConfigInfoMapper.updateByPrimaryKeySelective(gprsConfigInfo);
    }

    private void updateCellInfo(String gprsid, Integer cell, Integer pulseSendDone) {
        cellInfoService.updateSendDoneByGprsCellIndex(gprsid, cell, pulseSendDone);
    }

    private void saveCommand(String gprsid, Integer cell, PulseVo pulseVo) {
        PulseDischargeSend send = new PulseDischargeSend();
        send.setGprsId(gprsid);
        send.setPulseCell(cell);
        send.setSendDone(Integer.valueOf(0).byteValue());
        send.setDischargeTime(pulseVo.getDischargeTime());
        send.setFastSampleInterval(pulseVo.getFastSampleInterval());
        send.setSlowSampleInterval(pulseVo.getSlowSampleInterval());
        send.setSlowSampleTime(pulseVo.getSlowSampleTime());
        pulseDischargeSendMapper.insertSelective(send);
    }

    private void checkPayload(PulseVo pulseVo) {
        Preconditions.checkNotNull(pulseVo, "特征测试指令参数错误");
        Preconditions.checkNotNull(pulseVo.getFastSampleInterval(), "特征测试指令参数错误");
        Preconditions.checkNotNull(pulseVo.getSlowSampleInterval(), "特征测试指令参数错误");
        Preconditions.checkNotNull(pulseVo.getDischargeTime(), "特征测试指令参数错误");
        Preconditions.checkNotNull(pulseVo.getSlowSampleTime(), "特征测试指令参数错误");
        if (CollectionUtils.isEmpty(pulseVo.getGprsIdList()) && pulseVo.getCompanyId() == null) {
            throw new IllegalArgumentException("特征测试指令参数错误");
        }
    }
}
