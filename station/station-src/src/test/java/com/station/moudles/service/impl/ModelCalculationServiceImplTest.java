package com.station.moudles.service.impl;

import com.google.common.collect.Lists;
import com.station.common.cache.DischargeCache;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.PulseDischargeInfo;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.mapper.CellInfoMapper;
import com.station.moudles.mapper.CompanyMapper;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.ModifyCapacitySendMapper;
import com.station.moudles.mapper.PackDataExpandLatestMapper;
import com.station.moudles.mapper.PackDataExpandMapper;
import com.station.moudles.mapper.PackDataInfoLatestMapper;
import com.station.moudles.mapper.PackDataInfoMapper;
import com.station.moudles.mapper.PulseCalculationSendMapper;
import com.station.moudles.mapper.PulseDischargeInfoMapper;
import com.station.moudles.mapper.PulseDischargeSendMapper;
import com.station.moudles.mapper.RoutingInspectionsMapper;
import com.station.moudles.mapper.StationDurationHistoryMapper;
import com.station.moudles.mapper.StationInfoMapper;
import com.station.moudles.service.ModelCalculationService;
import org.apache.commons.lang3.RandomUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.Before;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.mockito.InjectMocks;
import org.mockito.runners.MockitoJUnitRunner;

import java.math.BigDecimal;
import java.util.Date;
import java.util.List;
import java.util.Map;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;
import static org.mockito.Matchers.any;
import static org.mockito.Matchers.anyVararg;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

@RunWith(MockitoJUnitRunner.class)
public class ModelCalculationServiceImplTest {

    private StationInfoMapper stationInfoMapper = mock(StationInfoMapper.class);
    private PulseDischargeInfoMapper pulseDischargeInfoMapper = mock(PulseDischargeInfoMapper.class);
    private PulseDischargeSendMapper pulseDischargeSendMapper = mock(PulseDischargeSendMapper.class);
    private PackDataExpandLatestMapper packDataExpandLatestMapper = mock(PackDataExpandLatestMapper.class);
    private PackDataExpandMapper packDataExpandMapper = mock(PackDataExpandMapper.class);
    private GprsConfigInfoMapper gprsConfigInfoMapper = mock(GprsConfigInfoMapper.class);
    private PackDataInfoMapper packDataInfoMapper = mock(PackDataInfoMapper.class);
    private CellInfoMapper cellInfoMapper = mock(CellInfoMapper.class);
    private DischargeCache dischargeCache = mock(DischargeCache.class);
    private RoutingInspectionsMapper routingInspectionsMapper = mock(RoutingInspectionsMapper.class);

    @InjectMocks
    private ModelCalculationService modelCalculationService = new ModelCalculationServiceImpl(stationInfoMapper,
            pulseDischargeInfoMapper, pulseDischargeSendMapper, packDataExpandLatestMapper, packDataExpandMapper,
            gprsConfigInfoMapper, packDataInfoMapper,
            mock(PackDataInfoLatestMapper.class), cellInfoMapper, mock(ModifyCapacitySendMapper.class),
            mock(PulseCalculationSendMapper.class), mock(CompanyMapper.class), mock(StationDurationHistoryMapper.class), dischargeCache,
            routingInspectionsMapper);

    @Before
    public void setup() {
        when(pulseDischargeSendMapper.selectListSelective(any(PulseDischargeSend.class))).thenReturn(pulseDischargeSends());
        when(pulseDischargeInfoMapper.findByPulseDischargeSendIds(anyVararg())).thenReturn(pulseDischargeInfos());
        when(cellInfoMapper.selectListSelective(any(CellInfo.class))).thenReturn(cellInfos());
    }

    private List<CellInfo> cellInfos() {
        Date currentDate = new Date();
        List<CellInfo> items = Lists.newArrayList();
        for (int i = 1; i < 25; i++) {
            CellInfo item = new CellInfo();
            item.setCellIndex(i);
            if (i == 1) {
                item.setUseFrom(DateUtils.addYears(currentDate, -3));
            } else if (i == 2) {
                item.setUseFrom(DateUtils.addYears(currentDate, -5));
            } else {
                item.setUseFrom(DateUtils.addMonths(currentDate, -i));
            }
            items.add(item);
        }
        return items;
    }

    private List<PulseDischargeInfo> pulseDischargeInfos() {
        List<PulseDischargeInfo> items = Lists.newArrayList();
        PulseDischargeInfo item = new PulseDischargeInfo();
        item.setPulseDischargeSendId(3);
        item.setCurrent("3.142-2.14-2.14-2.14-2.14-2.14-2.138-2.139-2.138-2.139-2.137-2.137-2.137");
        item.setVoltage("355.34--294.18--294.21--294.25--294.27--294.27--294.28--294.31--294.33--294.34--294.36");
        items.add(item);

        item = new PulseDischargeInfo();
        item.setPulseDischargeSendId(4);
        item.setCurrent("4.142-2.14-2.14-2.14-2.14-2.14-2.138-2.139-2.138-2.139-2.137-2.137-2.137");
        item.setVoltage("455.34--294.18--294.21--294.25--294.27--294.27--294.28--294.31--294.33--294.34--294.36");
        items.add(item);

        item = new PulseDischargeInfo();
        item.setPulseDischargeSendId(5);
        item.setCurrent("5.142-2.14-2.14-2.14-2.14-2.14-2.138-2.139-2.138-2.139-2.137-2.137-2.137");
        item.setVoltage("555.34--294.18--294.21--294.25--294.27--294.27--294.28--294.31--294.33--294.34--294.36");
        items.add(item);

        item = new PulseDischargeInfo();
        item.setPulseDischargeSendId(6);
        item.setCurrent("6.142-2.14-2.14-2.14-2.14-2.14-2.138-2.139-2.138-2.139-2.137-2.137-2.137");
        item.setVoltage("655.34--294.18--294.21--294.25--294.27--294.27--294.28--294.31--294.33--294.34--294.36");
        items.add(item);
        return items;
    }

    private List<PulseDischargeSend> pulseDischargeSends() {
        Date currentDate = new Date();
        List<PulseDischargeSend> sends = Lists.newArrayList();
        PulseDischargeSend send = new PulseDischargeSend();
        send.setPulseCell(1);
        send.setId(1);
        send.setEndTime(DateUtils.addMinutes(currentDate, -10));
        sends.add(send);

        send = new PulseDischargeSend();
        send.setPulseCell(1);
        send.setId(2);
        send.setEndTime(DateUtils.addMinutes(currentDate, -5));
        sends.add(send);

        send = new PulseDischargeSend();
        send.setPulseCell(1);
        send.setId(3);
        send.setEndTime(currentDate);
        sends.add(send);

        send = new PulseDischargeSend();
        send.setPulseCell(2);
        send.setId(4);
        send.setEndTime(currentDate);
        sends.add(send);

        send = new PulseDischargeSend();
        send.setPulseCell(3);
        send.setId(5);
        send.setEndTime(currentDate);
        sends.add(send);

        send = new PulseDischargeSend();
        send.setPulseCell(4);
        send.setId(6);
        send.setEndTime(currentDate);
        sends.add(send);

        send = new PulseDischargeSend();
        send.setPulseCell(5);
        send.setId(7);
        send.setEndTime(currentDate);
        sends.add(send);
        return sends;
    }

    @Test
    public void testGetLatestPulseDischargeInfo() {
        Map<Integer, PulseDischargeInfo> map = modelCalculationService.getLatestPulseDischargeInfo("-1");
        assertEquals(map.size(), 0);

        map = modelCalculationService.getLatestPulseDischargeInfo("TEST-1");
        assertEquals(map.size(), 4);
        PulseDischargeInfo info = map.get(1);
        assertEquals(info.getPulseDischargeSendId().intValue(), 3);
        assertEquals(info.getCurrent(), "3.142-2.14-2.14-2.14-2.14-2.14-2.138-2.139-2.138-2.139-2.137-2.137-2.137");
        assertEquals(info.getVoltage(), "355.34--294.18--294.21--294.25--294.27--294.27--294.28--294.31--294.33--294.34--294.36");

        info = map.get(4);
        assertEquals(info.getPulseDischargeSendId().intValue(), 6);
        assertEquals(info.getCurrent(), "6.142-2.14-2.14-2.14-2.14-2.14-2.138-2.139-2.138-2.139-2.137-2.137-2.137");
        assertEquals(info.getVoltage(), "655.34--294.18--294.21--294.25--294.27--294.27--294.28--294.31--294.33--294.34--294.36");

        info = map.get(5);
        assertNull(info);
    }

    @Test
    public void testCalculateCCN() {
        StationInfo stationInfo = new StationInfo();
        stationInfo.setGprsId("TS0021");
        stationInfo.setPackType("48V500AH");
        Map<Integer, BigDecimal> map = modelCalculationService.calculateCCN(stationInfo, new Date());
        assertEquals(map.size(), 24);
        assertEquals(map.get(1).toString(), "200.0");
        assertEquals(map.get(24).toString(), "300.0");
    }

    @Test
    public void testRandom() {
        List<Integer> nums = Lists.newArrayList();
        int num;
        for (int i = 0; i < 24; i++) {
            do {
                num = (int) (Math.random() * 24) + 1;
            } while (nums.contains(num));
            nums.add(num);
        }
        System.out.println(StringUtils.join(nums, ","));
    }
    @Test
    public void testSingleEntityResistance() {
    	PulseDischargeInfo pulseDischargeInfo = new PulseDischargeInfo();
    	pulseDischargeInfo.setId(1718);
    	pulseDischargeInfo.setFastSampleInterval(0);
    	pulseDischargeInfo.setDischargeTime(0);
    	pulseDischargeInfo.setVoltage("2.182-2.168-2.166-2.17-2.168-2.168-2.17-2.168-2.172-2.17-2.166-2.168-2.174-2.168-2.17-2.17-2.17-2.174-2.168-2.174-2.174-2.17-2.184-2.174-2.17-2.17-2.168-2.154-2.168-2.174-2.178-2.168-2.17-2.17-2.168-2.168-2.17-2.166-2.168-2.166-2.174-2.174-2.166-2.174-2.174-2.168-2.168-2.168-2.168-2.168-2.168-2.166-2.17-2.168-2.168-2.17-2.174-2.174-2.174-2.168-2.174-2.168-2.168-2.168-2.168-2.174-2.174-2.168-2.166-2.17-2.17-2.178-2.168-2.17-2.168-2.168-2.168-2.168-2.164-2.17-2.168-2.168-2.174-2.17-2.168-2.174-2.166-2.168-2.17-2.174-2.168-2.168-2.168-2.166-2.166-2.174-2.166-2.168-2.168-2.168-2.168-2.168-2.168-2.168-2.166-2.166-2.168-2.168-2.166-2.168-2.168-2.172-2.168-2.17-2.168-2.168-2.166-2.168-2.17-2.166-2.168-2.168-2.17-2.168-2.168-2.144-2.168-2.166-2.168-2.168-2.168-2.17-2.168-2.172-2.168-2.166-2.168-2.144-2.168-2.18-2.166-2.168-2.174-2.166-2.166-2.168-2.164-2.166-2.168-2.166-2.164-2.164-2.17-2.174-2.168-2.166-2.174-2.168-2.168-2.164-2.17-2.168-2.164-2.166-2.166-2.166-2.168-2.168-2.166-2.168-2.168-2.17-2.168-2.168-2.168-2.168-2.174-2.164-2.168-2.17-2.164-2.17-2.17-2.164-2.168-2.166-2.168-2.174-2.174-2.17-2.174-2.166-2.168-2.168-2.166-2.168-2.168-2.168-2.168-2.168-2.17-2.168-2.168-2.168-2.168-2.166-2.17-2.17-2.184-2.168-2.168-2.168-2.168-2.168-2.17-2.168-2.168-2.168-2.166-2.168-2.144-2.168-2.184-2.168-2.168-2.166-2.166-2.168-2.168-2.174-2.168-2.168-2.168-2.168-2.168-2.166-2.168-2.17-2.174-2.166-2.168-2.166-2.168-2.168-2.166-2.164-2.17-2.168-2.174-2.17-2.168-2.168-2.168-2.168-2.166-2.172-2.17-2.174-2.168-2.164-2.166-2.168-2.17-2.17-2.17-2.168-2.164-2.17-2.17-2.184-2.168-2.184-2.166-2.164-2.168-2.168-2.168-2.166-2.17-2.168-2.168-2.184-2.168-2.166-2.168-2.168-2.168-2.168-2.17-2.168-2.184-2.166-2.168-2.168-2.166-2.168-2.174-2.168-2.164-2.166-2.168-2.168-2.168-2.166-2.166-2.17-2.17-2.166-2.178-2.168-2.17-2.184-2.182-2.178-2.184-2.182-2.184-2.186-2.198-2.192-2.182-2.182-2.182-2.184-2.182-2.18-2.192-2.182-2.18-2.186-2.186-2.202-2.188-2.182-2.18-2.174-2.18-2.18-2.186-2.184-2.182-2.184-2.182-2.182-2.182-2.186-2.184-2.182-2.18-2.192-2.186-2.184-2.186-2.186-2.182-2.184-2.186-2.188-2.188-2.186-2.188-2.186-2.18-2.186-2.184-2.186-2.188-2.186-2.19-2.19-2.174-2.182-2.182-2.182-2.186-2.182-2.186-2.186-2.186-2.192-2.192-2.182-2.192-2.18-2.188-2.192-2.186-2.182-2.182-2.188-2.182-2.186-2.182-2.184-2.182-2.184-2.184-2.178-2.182-2.18-2.186-2.192-2.188-2.186-2.186-2.186-2.188-2.186-2.186-2.192-2.188-2.18-2.182-2.198-2.182-2.182-2.186-2.188-2.182-2.186-2.184-2.192-2.18-2.182-2.188-2.182-2.19-2.182-2.182-2.186-2.182-2.188-2.188-2.182-2.192-2.182-2.182-2.192-2.182-2.184-2.186-2.182-2.192-2.182-2.182-2.186-2.192-2.188-2.18-2.19-2.186-2.186-2.186-2.186-2.186-2.192-2.182-2.186-2.182-2.182-2.192-2.192-2.192-2.188-2.186-2.188-2.188-2.186-2.182-2.192-2.188-2.182-2.182-2.192-2.186-2.18-2.182-2.188-2.18-2.188-2.182-2.186-2.18-2.182-2.186-2.182-2.186-2.18-2.192-2.202-2.182-2.182-2.186-2.18-2.186-2.192-2.192-2.186-2.18-2.188-2.182-2.182-2.182-2.192-2.186-2.186-2.182-2.186-2.186-2.188-2.18-2.186-2.192-2.186-2.182-2.186-2.18-2.188-2.222-2.188");
    	pulseDischargeInfo.setCurrent("-0.13-13.9-13.13-13.93-14.3-13.23-14-13.33-14.33-13.3-14.03-13.2-13.73-13.43-13.83-14.13-13.36-14.03-13.26-14.1-13.23-14.1-13.23-14.26-14.1-13.1-14.1-13.56-13.9-13.56-14.03-13.03-14-13.6-14.13-13.06-13.8-13.13-13.43-13.03-14.13-13.43-13.86-13.66-13.6-13.13-14.03-13-14.06-13.06-14.1-13.13-14.13-12.93-13.93-14.03-13.9-13.13-14.16-13.26-14.16-13.16-13.43-13.06-13-13.23-13.83-13.06-14.03-14.1-12.96-14.16-13.06-13-13.23-13.96-12.93-13.7-13.56-13.83-13.96-13.93-12.96-13.03-13.13-13.96-12.9-13.86-14.03-14-13-13.33-13.86-13.03-13.86-13.66-13.5-13.23-13.96-13.16-12.86-12.86-13.4-13.8-12.9-13.83-12.9-13-14.06-12.96-13.4-13.8-13-14.06-12.9-14.1-13-13.3-12.93-13.63-13.8-13.03-13.8-13.83-12.93-13.63-13.26-13.06-13.16-14.06-13.63-13.6-12.86-13.4-13.76-12.86-13.6-13.93-13.3-13.46-13.8-12.86-13.86-14-13.03-13.63-13.4-13.9-12.9-13.33-13.8-12.8-13.5-12.9-13.16-13.93-13.66-13.33-13.73-12.76-13.9-13.4-13.6-12.9-13.3-13.83-13.43-12.83-13-12.93-13.86-12.8-13.5-12.83-13.03-13.13-13.9-12.8-13.66-12.86-13.86-13.56-12.9-13.33-13.03-13.13-13.83-12.83-12.9-13.93-12.86-13.2-13.9-12.8-13.33-12.86-13.1-13.2-13.03-13.23-13.33-13.26-12.9-13.83-12.93-13.86-13.03-13.66-12.86-12.76-12.8-13.23-13.83-13.43-13.16-12.96-12.86-12.83-12.8-12.66-13.53-13.26-12.8-12.93-13.2-13.63-13.66-12.93-13.66-12.93-13.3-13.43-13.56-13.7-13.53-13.4-13.83-13.06-13.46-13.26-13.16-12.93-12.93-13.06-12.63-12.76-12.66-12.6-12.66-12.53-12.7-13.1-12.9-12.8-12.76-12.6-13.5-12.6-13.43-13.03-13.3-12.7-13.63-12.56-13.33-13.36-13.73-13.1-13.26-13.6-13.13-12.63-13.33-13.46-13.63-13.7-13.7-13.33-13.53-12.63-12.63-12.53-12.63-12.96-12.83-13.56-12.7-12.56-12.76-12.63-12.73-12.9-13.3-12.73-12.6-12.6-12.76-12.5-12.53-13.46-12.7-12.63-12.73-12.86-12.66-12.7-13.1-12.53-12.63-12.73-12.66-0.43--0.26--0.06--0.16--0.06-0.36-0.3--0.1--0.33--0.1-0.33-0.3--0.2-0.33-0.26--0.2-0.2--0.2-0.03-0.06-0.26--0.16-0.2-0.23--0.33--0.13-0.4-0.36--0.03--0.33-0.03-0.33--0.23--0.23--0.13--0.13-0.06-0.36-0.13-0-0.1-0.06--0.13-0.4-0.1--0.2-0-0.33-0.2--0.2--0.2--0.26--0.1-0.36-0-0-0.33--0.23--0.23--0.13-0.36-0.26--0.23--0.06-0.33--0.13-0.06-0.13--0.2--0.03--0.03--0.3-0.16--0.13-0.3-0.3--0.2--0.16-0.16-0.26--0.16-0.33--0.13-0.16-0.13--0.13-0.1-0.33--0.16--0.23-0.36-0.36-0.13-0.3--0.23--0.06--0.2-0.4--0.06-0.23-0--0.13-0.33--0.23--0.16-0.26--0.16--0.13-0.26--0.23-0.33--0.16-0-0.2--0.23-0.06-0.2--0.16-0.26-0--0.23-0.33-0-0.36--0.03--0.2--0.1--0.1-0.2-0-0.16-0.23-0-0.23--0.16-0.16--0.13--0.16-0.33-0--0.13--0.26-0.13-0.3--0.16-0.23-0--0.13--0.06--0.13-0.23--0.13--0.13-0.13-0.06--0.13--0.1--0.03-0.06--0.1-0.16-0.23-0.06-0.23-0-0--0.16-0--0.13-0.23--0.16-0.13--0.13-0--0.2--0.13-0.13-0.16--0.1--0.16--0.13-0-0-0-0.23-0.1-0.13-0.26--0.2-0.03--0.13--0.16-0--0.03--0.13-0-0.13--0.13--0.03-0.03-0--0.16-0.23--0.13-0-0-0-0.23--0.2");
    	pulseDischargeInfo.setFilterCurrent("13.9-13.13-13.93-14.3-13.23-14.0-13.33-14.33-13.3-14.03-13.2-13.73-13.43-13.83-14.13-13.36-14.03-13.26-14.1-13.23-14.1-13.23-14.26-14.1-13.1-14.1-13.56-13.9-13.56-14.03-13.03-14.0-13.6-14.13-13.06-13.8-13.13-13.43-13.03-14.13-13.43-13.86-13.66-13.6-13.13-14.03-13.0-14.06-13.06-14.1-13.13-14.13-12.93-13.93-14.03-13.9-13.13-14.16-13.26-14.16-13.16-13.43-13.06-13.0-13.23-13.83-13.06-14.03-14.1-12.96-14.16-13.06-13.0-13.23-13.96-12.93-13.7-13.56-13.83-13.96-13.93-12.96-13.03-13.13-13.96-12.9-13.86-14.03-14.0-13.0-13.33-13.86-13.03-13.86-13.66-13.5-13.23-13.96-13.16-12.86-12.86-13.4-13.8-12.9-13.83-12.9-13.0-14.06-12.96-13.4-13.8-13.0-14.06-12.9-14.1-13.0-13.3-12.93-13.63-13.8-13.03-13.8-13.83-12.93-13.63-13.26-13.06-13.16-14.06-13.63-13.6-12.86-13.4-13.76-12.86-13.6-13.93-13.3-13.46-13.8-12.86-13.86-14.0-13.03-13.63-13.4-13.9-12.9-13.33-13.8-12.8-13.5-12.9-13.16-13.93-13.66-13.33-13.73-12.76-13.9-13.4-13.6-12.9-13.3-13.83-13.43-12.83-13.0-12.93-13.86-12.8-13.5-12.83-13.03-13.13-13.9-12.8-13.66-12.86-13.86-13.56-12.9-13.33-13.03-13.13-13.83-12.83-12.9-13.93-12.86-13.2-13.9-12.8-13.33-12.86-13.1-13.2-13.03-13.23-13.33-13.26-12.9-13.83-12.93-13.86-13.03-13.66-12.86-12.76-12.8-13.23-13.83-13.43-13.16-12.96-12.86-12.83-12.8-12.66-13.53-13.26-12.8-12.93-13.2-13.63-13.66-12.93-13.66-12.93-13.3-13.43-13.56-13.7-13.53-13.4-13.83-13.06-13.46-13.26-13.16-12.93-12.93-13.06-12.63-12.76-12.66-12.6-12.66-12.53-12.7-13.1-12.9-12.8-12.76-12.6-13.5-12.6-13.43-13.03-13.3-12.7-13.63-12.56-13.33-13.36-13.73-13.1-13.26-13.6-13.13-12.63-13.33-13.46-13.63-13.7-13.7-13.33-13.53-12.63-12.63-12.53-12.63-12.96-12.83-13.56-12.7-12.56-12.76-12.63-12.73-12.9-13.3-12.73-12.6-12.6-12.76-12.5-12.53-13.46-12.7-12.63-12.73-12.86-12.66-12.7-13.1-12.53-12.63-12.73-12.66-0.43-0.36-0.3-0.33-0.3-0.33-0.26-0.2-0.03-0.06-0.26-0.2-0.23-0.4-0.36-0.03-0.33-0.06-0.36-0.13-0.0-0.1-0.06-0.4-0.1-0.0-0.33-0.2-0.36-0.0-0.0-0.33-0.36-0.26-0.33-0.06-0.13-0.16-0.3-0.3-0.16-0.26-0.33-0.16-0.13-0.1-0.33-0.36-0.36-0.13-0.3-0.4-0.23-0.0-0.33-0.26-0.26-0.33-0.0-0.2-0.06-0.2-0.26-0.0-0.33-0.0-0.36-0.2-0.0-0.16-0.23-0.0-0.23-0.16-0.33-0.0-0.13-0.3-0.23-0.0-0.23-0.13-0.06-0.06-0.16-0.23-0.06-0.23-0.0-0.0-0.0-0.23-0.13-0.0-0.13-0.16-0.0-0.0-0.0-0.23-0.1-0.13-0.26-0.03-0.0-0.0-0.13-0.03-0.0-0.23-0.0-0.0-0.0-0.23-+199-10-153");
    	pulseDischargeInfo.setFilterVoltage("2.182-2.168-2.166-2.17-2.168-2.168-2.17-2.168-2.172-2.17-2.166-2.168-2.174-2.168-2.17-2.17-2.17-2.174-2.168-2.174-2.174-2.17-2.1719999999999997-2.174-2.17-2.17-2.168-2.17-2.168-2.174-2.178-2.168-2.17-2.17-2.168-2.168-2.17-2.166-2.168-2.166-2.174-2.174-2.166-2.174-2.174-2.168-2.168-2.168-2.168-2.168-2.168-2.166-2.17-2.168-2.168-2.17-2.174-2.174-2.174-2.168-2.174-2.168-2.168-2.168-2.168-2.174-2.174-2.168-2.166-2.17-2.17-2.178-2.168-2.17-2.168-2.168-2.168-2.168-2.164-2.17-2.168-2.168-2.174-2.17-2.168-2.174-2.166-2.168-2.17-2.174-2.168-2.168-2.168-2.166-2.166-2.174-2.166-2.168-2.168-2.168-2.168-2.168-2.168-2.168-2.166-2.166-2.168-2.168-2.166-2.168-2.168-2.172-2.168-2.17-2.168-2.168-2.166-2.168-2.17-2.166-2.168-2.168-2.17-2.168-2.168-2.1675-2.168-2.166-2.168-2.168-2.168-2.17-2.168-2.172-2.168-2.166-2.168-2.1705-2.168-2.18-2.166-2.168-2.174-2.166-2.166-2.168-2.164-2.166-2.168-2.166-2.164-2.164-2.17-2.174-2.168-2.166-2.174-2.168-2.168-2.164-2.17-2.168-2.164-2.166-2.166-2.166-2.168-2.168-2.166-2.168-2.168-2.17-2.168-2.168-2.168-2.168-2.174-2.164-2.168-2.17-2.164-2.17-2.17-2.164-2.168-2.166-2.168-2.174-2.174-2.17-2.174-2.166-2.168-2.168-2.166-2.168-2.168-2.168-2.168-2.168-2.17-2.168-2.168-2.168-2.168-2.166-2.17-2.17-2.169-2.168-2.168-2.168-2.168-2.168-2.17-2.168-2.168-2.168-2.166-2.168-2.1715-2.168-2.168875-2.168-2.168-2.166-2.166-2.168-2.168-2.174-2.168-2.168-2.168-2.168-2.168-2.166-2.168-2.17-2.174-2.166-2.168-2.166-2.168-2.168-2.166-2.164-2.17-2.168-2.174-2.17-2.168-2.168-2.168-2.168-2.166-2.172-2.17-2.174-2.168-2.164-2.166-2.168-2.17-2.17-2.17-2.168-2.164-2.17-2.17-2.184-2.168-2.184-2.166-2.164-2.168-2.168-2.168-2.166-2.17-2.168-2.168-2.1675-2.168-2.166-2.168-2.168-2.168-2.168-2.17-2.168-2.168-2.166-2.168-2.168-2.166-2.168-2.174-2.168-2.164-2.166-2.168-2.168-2.168-2.166-2.166-2.17-2.17-2.166-2.178-2.168-2.17-2.184-2.182-2.178-2.184-2.182-2.184-2.186-2.198-2.192-2.182-2.182-2.182-2.184-2.182-2.18-2.181-2.182-2.18-2.186-2.186-2.1855-2.188-2.182-2.18-2.174-2.18-2.18-2.186-2.184-2.182-2.184-2.182-2.182-2.182-2.186-2.184-2.182-2.18-2.192-2.186-2.184-2.186-2.186-2.182-2.184-2.186-2.188-2.188-2.186-2.188-2.186-2.18-2.186-2.184-2.186-2.188-2.186-2.19-2.19-2.174-2.182-2.182-2.182-2.186-2.182-2.186-2.186-2.186-2.192-2.192-2.182-2.192-2.18-2.188-2.192-2.186-2.182-2.182-2.188-2.182-2.186-2.182-2.184-2.182-2.184-2.184-2.178-2.182-2.18-2.186-2.192-2.188-2.186-2.186-2.186-2.188-2.186-2.186-2.192-2.188-2.18-2.182-2.1815-2.182-2.182-2.186-2.188-2.182-2.186-2.184-2.192-2.18-2.182-2.188-2.182-2.19-2.182-2.182-2.186-2.182-2.188-2.188-2.182-2.192-2.182-2.182-2.192-2.182-2.184-2.186-2.182-2.192-2.182-2.182-2.186-2.192-2.188-2.18-2.19-2.186-2.186-2.186-2.186-2.186-2.192-2.182-2.186-2.182-2.182-2.192-2.192-2.192-2.188-2.186-2.188-2.188-2.186-2.182-2.192-2.188-2.182-2.182-2.192-2.186-2.18-2.182-2.188-2.18-2.188-2.182-2.186-2.18-2.182-2.186-2.182-2.186-2.18-2.192-2.202-2.182-2.182-2.186-2.18-2.186-2.192-2.192-2.186-2.18-2.188-2.182-2.182-2.182-2.192-2.186-2.186-2.182-2.186-2.186-2.188-2.18-2.186-2.192-2.186-2.182-2.186-2.18-2.188-2.222-2.188-+200-10-200");
    	BigDecimal result = modelCalculationService.calculateSingleEntityResistance(pulseDischargeInfo, "T0B000012");
//    	assertEquals(result.toString(),"3.6435");
    }
}
