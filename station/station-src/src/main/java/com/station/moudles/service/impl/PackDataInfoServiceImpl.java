package com.station.moudles.service.impl;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.function.Function;
import java.util.function.ObjDoubleConsumer;

import org.apache.commons.collections.CollectionUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Lazy;
import org.springframework.stereotype.Service;

import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.station.common.cache.DischargeCache;
import com.station.common.utils.BeanValueUtils;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.ReflectUtil;
import com.station.common.utils.WaveFilter;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.mapper.PackDataInfoMapper;
import com.station.moudles.service.PackDataInfoService;

@Service
public class PackDataInfoServiceImpl extends BaseServiceImpl<PackDataInfo, Integer> implements PackDataInfoService {

    private final PackDataInfoMapper packDataInfoMapper;
    private final DischargeCache dischargeCache;

    @Autowired
    @Lazy
    public PackDataInfoServiceImpl(PackDataInfoMapper packDataInfoMapper,
                                   DischargeCache dischargeCache) {
        this.packDataInfoMapper = packDataInfoMapper;
        this.dischargeCache = dischargeCache;
    }

    @Override
    public List<PackDataInfo> selectListByGprsIdTime(String gprsId, Date rcvTime) {
        Map<Object, Object> m = new HashMap<>();
        m.put("gprsId", gprsId);
        m.put("rcvTime", rcvTime);
        return packDataInfoMapper.selectListByGprsIdTime(m);
    }

    @Override
    public List<Map<String, Object>> getSumVolCur(String gprsId) {
        Date rcvTime = MyDateUtils.getDiffTime(-6 * 60 * 60 * 1000);
        List<PackDataInfo> pdiList = selectListByGprsIdTime(gprsId, rcvTime);
//        Function<PackDataInfo, Double> getVol = (PackDataInfo pdi) -> {return Double.valueOf(pdi.getGenVol().doubleValue()); };
//        ObjDoubleConsumer<PackDataInfo> setVol = (PackDataInfo pdi, double val) -> {pdi.setGenVol(BigDecimal.valueOf(val).setScale(3, BigDecimal.ROUND_HALF_UP));};
//        WaveFilter.swingFilter(pdiList, getVol, setVol, 53, 47, 0.1);
//		
//        WaveFilter.avgFilter(pdiList, 
//				(PackDataInfo pdi) -> {return Double.valueOf(pdi.getGenCur().doubleValue());},
//				(PackDataInfo pdi, double val) -> {pdi.setGenCur(BigDecimal.valueOf(val).setScale(3, BigDecimal.ROUND_HALF_UP));}, 
//				100, -100, 0.1);
        List<Map<String, Object>> resultList = new ArrayList();
        for (PackDataInfo pdi : pdiList) {
            Map<String, Object> m = new HashMap<String, Object>();
            m.put("genVol", pdi.getGenVol());
            m.put("genCur", pdi.getGenCur());
            m.put("rcvTime", pdi.getRcvTime());
            resultList.add(m);
        }
        return resultList;
    }

    @Override
    public List<Map<String, Object>> getCellVolList(String gprsId, Integer cellIndex) {
        Date rcvTime = MyDateUtils.getDiffTime(-6 * 60 * 60 * 1000);
        List<PackDataInfo> pdiList = selectListByGprsIdTime(gprsId, rcvTime);
        List<Map<String, Object>> resultList = new ArrayList();
        for (PackDataInfo pdi : pdiList) {
            Map<String, Object> m = new HashMap<String, Object>();
            m.put("cellVol", ReflectUtil.getValueByKey(pdi, "cellVol" + cellIndex));
            m.put("rcvTime", pdi.getRcvTime());
            resultList.add(m);
        }
        return resultList;
    }


    @Override
    public List getDischargeHistory(String gprsId) {
        List<PackDataInfo> items = dischargeCache.get(gprsId, Collections.emptyList());
        logger.info("获取{}条放电测试记录,基站编号:{}", items.size(), gprsId);
        if (CollectionUtils.isEmpty(items)) {
            return null;
        }
        List<Map> results = Lists.newArrayList();
        items.stream().forEach(item -> {
            Map map = Maps.newHashMap();
            map.put("id", item.getId());
            map.put("state", item.getState());
            map.put("rcvTime", item.getRcvTime());
            map.put("genVol", item.getGenVol());
            map.put("genCur", item.getGenCur());
            results.add(map);
        });
        results.sort(Comparator.comparing(o -> ((Long) ((Date) o.get("rcvTime")).getTime())));
        return results;
    }

    @Override
    public List getCellDischargeHistory(String gprsId, Integer cellIndex) {
        List<PackDataInfo> items = dischargeCache.get(gprsId, Collections.emptyList());
        logger.info("获取{}条放电测试记录,基站编号:{}", items.size(), gprsId);
        if (CollectionUtils.isEmpty(items)) {
            return null;
        }
        List<Map> results = Lists.newArrayList();
        items.stream().forEach(item -> {
            Map map = Maps.newHashMap();
            map.put("id", item.getId());
            map.put("state", item.getState());
            map.put("rcvTime", item.getRcvTime());
            map.put("cenVol", BeanValueUtils.getValue("cellVol" + cellIndex, item));
            results.add(map);
        });
        results.sort(Comparator.comparing(o -> ((Long) ((Date) o.get("rcvTime")).getTime())));
        return results;
    }

    public PackDataInfo selectByOrderSelective(String gprsId, String state, Date rcvTime) {
        try {
            Map<String, Object> m = new HashMap<String, Object>();
            m.put("gprsId", gprsId);
            m.put("state", state);
            m.put("rcvTime", rcvTime);
            List<PackDataInfo> resultList = packDataInfoMapper.selectByOrderSelective(m);
            if (resultList.size() > 0) {
                return resultList.get(0);
            } else {
                return null;
            }
        } catch (Exception e) {
            e.printStackTrace();
            throw e;
        }
    }

    List<PackDataInfo> selectListByTime(String gprsId, String state, Date startTime, Date endTime) {
        try {
            Map<String, Object> m = new HashMap<String, Object>();
            m.put("gprsId", gprsId);
            m.put("state", state);
            m.put("startTime", startTime);
            m.put("endTime", endTime);
            return packDataInfoMapper.selectListByTime(m);
        } catch (Exception e) {
            e.printStackTrace();
            throw e;
        }
    }
}