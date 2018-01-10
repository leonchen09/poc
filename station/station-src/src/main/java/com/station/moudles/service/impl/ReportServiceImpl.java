package com.station.moudles.service.impl;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.stream.Collectors;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.collections.MapUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.apache.commons.lang3.time.StopWatch;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.google.common.base.Preconditions;
import com.google.common.collect.Lists;
import com.google.common.collect.Maps;
import com.google.common.collect.Sets;
import com.station.common.utils.BeanValueUtils;
import com.station.common.utils.ConvertUtils;
import com.station.common.utils.JxlsUtil;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.ReflectUtil;
import com.station.moudles.entity.CellInfo;
import com.station.moudles.entity.Company;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.InspectSignCellIndex;
import com.station.moudles.entity.PackDataExpandLatest;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.Parameter;
import com.station.moudles.entity.PulseDischargeSend;
import com.station.moudles.entity.RoutingInspections;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.entity.SugReplaceCellIndex;
import com.station.moudles.helper.AbstractEvent;
import com.station.moudles.helper.ChargeEvent;
import com.station.moudles.helper.DischargeEvent;
import com.station.moudles.helper.EventParams;
import com.station.moudles.helper.LossChargeEvent;
import com.station.moudles.mapper.CellInfoMapper;
import com.station.moudles.mapper.CompanyMapper;
import com.station.moudles.mapper.GprsConfigInfoMapper;
import com.station.moudles.mapper.InspectSignCellIndexMapper;
import com.station.moudles.mapper.PackDataExpandLatestMapper;
import com.station.moudles.mapper.PackDataInfoMapper;
import com.station.moudles.mapper.ParameterMapper;
import com.station.moudles.mapper.PulseDischargeSendMapper;
import com.station.moudles.mapper.RoutingInspectionsMapper;
import com.station.moudles.mapper.StationInfoMapper;
import com.station.moudles.mapper.SugReplaceCellIndexMapper;
import com.station.moudles.service.ModelCalculationService;
import com.station.moudles.service.ReportService;
import com.station.moudles.vo.report.ChargeDischargeEvent;
import com.station.moudles.vo.report.ChargeDischargeReport;
import com.station.moudles.vo.report.ChargeDischargeReportItem;
import com.station.moudles.vo.report.PulseReport;
import com.station.moudles.vo.report.PulseReportItem;
import com.station.moudles.vo.report.StationReport;
import com.station.moudles.vo.report.StationReportFilter;
import com.station.moudles.vo.report.StationReportItem;
import com.station.moudles.vo.report.SuggestionReport;
import com.station.moudles.vo.report.SuggestionReportItem;
import com.station.moudles.vo.search.SearchStationInfoPagingVo;

/**
 * Created by Jack on 9/17/2017.
 */
@Service
public class ReportServiceImpl implements ReportService {

    private final static Logger LOGGER = LoggerFactory.getLogger(ReportServiceImpl.class);

    private final static Map<Integer, String> carrierMap = Maps.newHashMap();
    private final static Map<Integer, String> deviceTypeMap = Maps.newHashMap();

    static {
        carrierMap.put(1, "移动");
        carrierMap.put(2, "联通");
        carrierMap.put(3, "电信");

        deviceTypeMap.put(1, "蓄电池串联复用设备");
        deviceTypeMap.put(2, "蓄电池串联复用诊断组件");
    }

    private final StationInfoMapper stationInfoMapper;
    private final CompanyMapper companyMapper;
    private final PulseDischargeSendMapper pulseDischargeSendMapper;
    private final InspectSignCellIndexMapper inspectSignCellIndexMapper;
    private final SugReplaceCellIndexMapper sugReplaceCellIndexMapper;
    private final GprsConfigInfoMapper gprsConfigInfoMapper;
    private final PackDataExpandLatestMapper packDataExpandLatestMapper;
    private final ModelCalculationService modelCalculationService;
    private final CellInfoMapper cellInfoMapper;
    private final PackDataInfoMapper packDataInfoMapper;
    private final ParameterMapper parameterMapper;
    @Autowired
    RoutingInspectionsMapper routingInspectionsMapper;

    @Autowired
    public ReportServiceImpl(StationInfoMapper stationInfoMapper,
                             CompanyMapper companyMapper,
                             PulseDischargeSendMapper pulseDischargeSendMapper,
                             InspectSignCellIndexMapper inspectSignCellIndexMapper,
                             SugReplaceCellIndexMapper sugReplaceCellIndexMapper,
                             GprsConfigInfoMapper gprsConfigInfoMapper,
                             PackDataExpandLatestMapper packDataExpandLatestMapper,
                             ModelCalculationService modelCalculationService,
                             CellInfoMapper cellInfoMapper,
                             PackDataInfoMapper packDataInfoMapper,
                             ParameterMapper parameterMapper) {
        this.stationInfoMapper = stationInfoMapper;
        this.companyMapper = companyMapper;
        this.pulseDischargeSendMapper = pulseDischargeSendMapper;
        this.inspectSignCellIndexMapper = inspectSignCellIndexMapper;
        this.sugReplaceCellIndexMapper = sugReplaceCellIndexMapper;
        this.gprsConfigInfoMapper = gprsConfigInfoMapper;
        this.packDataExpandLatestMapper = packDataExpandLatestMapper;
        this.modelCalculationService = modelCalculationService;
        this.cellInfoMapper = cellInfoMapper;
        this.packDataInfoMapper = packDataInfoMapper;
        this.parameterMapper = parameterMapper;
    }

    @Override
    public PulseReport generatePulseReport(Integer companyId, Date startTime, Date endTime) {
        Preconditions.checkNotNull(companyId, "公司编号不能为空");
        Preconditions.checkNotNull(startTime, "开始时间不能为空");
        Preconditions.checkNotNull(endTime, "结束时间不能为空");
        if (startTime.compareTo(endTime) > 0) {
            throw new IllegalArgumentException("开始时间大于结束时间");
        }
        Company company = companyMapper.selectByPrimaryKey(companyId);
        if (company == null) {
            return null;
        }
        PulseReport pulseReport = new PulseReport();
        pulseReport.setCompanyName(company.getCompanyName());
        pulseReport.setStartTime(MyDateUtils.getDateString(startTime, "yyyy/MM/dd HH:mm:ss"));
        pulseReport.setEndTime(MyDateUtils.getDateString(endTime, "yyyy/MM/dd HH:mm:ss"));
        List<PulseReportItem> pulseReportItems = Lists.newArrayList();
        List<StationInfo> items = stationInfoMapper.getStationByCompanyId(companyId)
                .stream()
                .filter(item -> !StringUtils.equalsIgnoreCase("-1", item.getGprsId()))
                .collect(Collectors.toList());
        if (CollectionUtils.isNotEmpty(items)) {
            List<String> gprsIds = items.stream().map(StationInfo::getGprsId).collect(Collectors.toList());
            Map paramMap = Maps.newHashMap();
            paramMap.put("gprsIds", gprsIds);
            paramMap.put("startTime", startTime);
            paramMap.put("endTime", endTime);
            List<PulseDischargeSend> sends = pulseDischargeSendMapper.getPulseDischargeSendsByTimes(paramMap);
            LOGGER.debug("获取{}条特征测试数据，公司编号:{},基站编号:{},时间:{}~{}", new Object[]{sends.size(),
                    companyId, StringUtils.join(gprsIds, ","),
                    pulseReport.getStartTime(), pulseReport.getEndTime()});

            Map<String, List<PulseDischargeSend>> sendMap = sends.stream().collect(Collectors.groupingBy(PulseDischargeSend::getGprsId));
            for (StationInfo item : items) {
                List<PulseDischargeSend> list = sendMap.get(item.getGprsId());
                PulseReportItem pulseReportItem = new PulseReportItem();
                pulseReportItem.setGprsId(item.getGprsId());
                pulseReportItem.setStationName(item.getName());
                if (CollectionUtils.isEmpty(list)) {
                    pulseReportItem.setCellStatusMap(Maps.newHashMap());
                } else {
                    pulseReportItem.setCellStatusMap(list.stream()
                            .collect(Collectors.toMap(PulseDischargeSend::getPulseCell, PulseDischargeSend::getSendDone)));
                }
                pulseReportItems.add(pulseReportItem);
            }

        }
        pulseReportItems.sort(Comparator.comparing(PulseReportItem::getGprsId));
        pulseReport.setItems(pulseReportItems);
        return pulseReport;
    }

    @Override
    public SuggestionReport getSuggestionReportItems(Integer companyId, String province, String city, String district) {
        return suggestionReportCommon(companyId, province, city, district, OperatorType.Search);
    }

    private SuggestionReport suggestionReportCommon(Integer companyId, String province, String city, String district, OperatorType operatorType) {
        Preconditions.checkNotNull(companyId, "公司编号不能为空");
        Company company = companyMapper.selectByPrimaryKey(companyId);
        if (company == null) {
            return null;
        }
        SuggestionReport report = new SuggestionReport();
        report.setCompanyName(company.getCompanyName());
        report.setExportTime(MyDateUtils.getDateString(new Date(), "yyyy-MM-dd HH:mm:ss"));
        Map paramMap = Maps.newHashMap();
        paramMap.put("companyId3", companyId);
        paramMap.put("province", province);
        paramMap.put("city", city);
        paramMap.put("district", district);
        paramMap.put("delFlag", 0);
        paramMap.put("isGprsIdNotNull", true);
        List<SuggestionReportItem> items = Lists.newArrayList();
        List<StationInfo> stationInfos = stationInfoMapper.getStations(paramMap).stream()
                .filter(item -> !StringUtils.equalsIgnoreCase(item.getGprsId(), "-1"))
                .collect(Collectors.toList());
        if (CollectionUtils.isNotEmpty(stationInfos)) {
            if (OperatorType.Create.equals(operatorType)) {
                createSuggestionReportHandler(report, items, stationInfos, company);
            } else {
                getSuggestionReportHandler(report, items, stationInfos, company);
            }
        }
        report.setItems(items);
        return report;
    }

    private void getSuggestionReportHandler(SuggestionReport report,
                                            List<SuggestionReportItem> items,
                                            List<StationInfo> stationInfos,
                                            Company company) {
        Map<String, GprsConfigInfo> configMap = Maps.newHashMap();
        Map<String, Threshold> thresholdMap = Maps.newHashMap();
        List<String> gprsIds = stationInfos.stream().map(StationInfo::getGprsId).collect(Collectors.toList());
        prepareSuggestionReportData(report, gprsIds, configMap, thresholdMap);
        Map<String, SugReplaceCellIndex> map = sugReplaceCellIndexMapper.getLatestByGprsIds(gprsIds)
                .stream().collect(Collectors.toMap(SugReplaceCellIndex::getGprsId, item -> item));
        for (StationInfo stationInfo : stationInfos) {
            items.add(generateSuggestionReportItem(stationInfo, company, configMap.get(stationInfo.getGprsId()), map.get(stationInfo.getGprsId())));
        }
    }

    private void prepareSuggestionReportData(SuggestionReport report,
                                             List<String> gprsIds,
                                             Map<String, GprsConfigInfo> configMap,
                                             Map<String, Threshold> thresholdMap) {
        List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoMapper.selectByGprsIds(gprsIds);
        configMap.putAll(gprsConfigInfos.stream().collect(Collectors.toMap
                (GprsConfigInfo::getGprsId, item -> item)));
        thresholdMap.putAll(gprsConfigInfos.stream().map(gprsConfigInfo -> {
            BigDecimal suggestCellCapPercent = gprsConfigInfo != null && StringUtils.isNotBlank(gprsConfigInfo.getSuggestCellCapPercent()) ?
                    BigDecimal.valueOf(ConvertUtils.toDouble(gprsConfigInfo.getSuggestCellCapPercent(), 20d)).divide(BigDecimal.valueOf(100)) :
                    BigDecimal.valueOf(0.2);
            BigDecimal suggestTime = gprsConfigInfo != null && StringUtils.isNotBlank(gprsConfigInfo.getSuggestTime()) ?
                    BigDecimal.valueOf(ConvertUtils.toDouble(gprsConfigInfo.getSuggestTime(), 3d)) :
                    BigDecimal.valueOf(3);
            Integer suggestAverageNum = gprsConfigInfo != null && gprsConfigInfo.getSuggestAverageNum() != null ?
                    gprsConfigInfo.getSuggestAverageNum() : 6;
            BigDecimal marginTime = gprsConfigInfo != null && gprsConfigInfo.getMarginTime() != null ?
                    gprsConfigInfo.getMarginTime() : BigDecimal.ONE;
            return new Threshold(gprsConfigInfo.getGprsId(), marginTime, suggestCellCapPercent, suggestTime, suggestAverageNum);
        }).collect(Collectors.toMap(Threshold::getGprsId, item -> item)));
        if (MapUtils.isNotEmpty(thresholdMap)) {
            Threshold threshold = thresholdMap.values().iterator().next();
            report.setSuggestCellCapPercent(String.valueOf(threshold.getSuggestCellCapPercent().multiply(BigDecimal.valueOf(100)).intValue()));
            report.setSuggestAverageNum(threshold.getSuggestAverageNum().toString());
            report.setSuggestTime(threshold.getSuggestTime().toString());
            report.setMarginTime(threshold.getMarginTime().toString());
        }
    }

    private void createSuggestionReportHandler(SuggestionReport report,
                                               List<SuggestionReportItem> items,
                                               List<StationInfo> stationInfos,
                                               Company company) {
        Map<String, GprsConfigInfo> configMap = Maps.newHashMap();
        Map<String, Threshold> thresholdMap = Maps.newHashMap();
        List<String> gprsIds = stationInfos.stream().map(StationInfo::getGprsId).collect(Collectors.toList());
        prepareSuggestionReportData(report, gprsIds, configMap, thresholdMap);
        Map<String, PackDataExpandLatest> expandLatestMap = packDataExpandLatestMapper.getByGprsIds(gprsIds)
                .stream()
                .collect(Collectors.toMap(PackDataExpandLatest::getGprsId, item -> item));
        Map<Integer, Set<Integer>> lCategoryMap = inspectSignCellIndexMapper.getLatestByStationIds(
                stationInfos.stream().map(StationInfo::getId).collect(Collectors.toList()))
                .stream()
                .collect(Collectors.groupingBy(InspectSignCellIndex::getStationId,
                        Collectors.mapping(InspectSignCellIndex::getCellIndex, Collectors.toSet())));
        for (StationInfo stationInfo : stationInfos) {
            SugReplaceCellIndex sugReplaceCellIndex = suggestHandler(stationInfo, lCategoryMap, expandLatestMap, thresholdMap);
            sugReplaceCellIndexMapper.insert(sugReplaceCellIndex);
            GprsConfigInfo configInfo = configMap.get(stationInfo.getGprsId());
            items.add(generateSuggestionReportItem(stationInfo, company, configInfo, sugReplaceCellIndex));
        }
    }

    @Override
    public SuggestionReport generateSuggestionReport(Integer companyId, String province, String city, String district) {
        return suggestionReportCommon(companyId, province, city, district, OperatorType.Create);
    }

    @Override
    public ChargeDischargeReport getChargeDischargeReport(Integer stationId, Date startTime, Date endTime,
    													 ChargeEvent chargeEvent,
    													 DischargeEvent dischargeEvent,
    													 LossChargeEvent lossChargeEvent) {
        if (stationId == null) {
            return null;
        }
        StationInfo info = new StationInfo();
        info.setId(stationId);
        List<StationInfo> stationInfos = stationInfoMapper.selectListSelective(info);
        if (CollectionUtils.isEmpty(stationInfos)) {
            return null;
        }
        StationInfo stationInfo = stationInfos.get(0);
        if (StringUtils.equalsIgnoreCase(stationInfo.getGprsId(), "-1")) {
            return null;
        }
        ChargeDischargeReport report = new ChargeDischargeReport();
        report.setStartTime(startTime != null ? MyDateUtils.getDateString(startTime, "yyyy/MM/dd") : null);
        report.setEndTime(endTime != null ? MyDateUtils.getDateString(endTime, "yyyy/MM/dd") : null);
        report.setCompanyName(stationInfo.getCompanyName3());
        report.setAddress(stationInfo.getAddress());
        report.setCarrier(carrierMap.get(stationInfo.getOperatorType()));
        report.setDeviceType(deviceTypeMap.get(stationInfo.getDeviceType()));
        report.setGprsId(stationInfo.getGprsId());
        report.setMaintainanceId(stationInfo.getMaintainanceId());
        report.setStationName(stationInfo.getName());
        report.setPackType(stationInfo.getPackType());
        report.setRoomType(stationInfo.getRoomType());
        report.setLat(stationInfo.getLat() == null ? null : stationInfo.getLat().toString());
        report.setLng(stationInfo.getLng() == null ? null : stationInfo.getLng().toString());
        report.setEvents(Lists.newArrayList());
        report.setDetails(Lists.newArrayList());
        report.setItems(Lists.newArrayList());

        List<PackDataInfo> packDataInfos = getPackDataInfosByRange(stationInfo.getGprsId(), startTime, endTime);
        if (CollectionUtils.isEmpty(packDataInfos)) {
            return report;
        }
        List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoMapper.selectByGprsIds(Lists.newArrayList(stationInfo.getGprsId()));
        GprsConfigInfo gprsConfigInfo = CollectionUtils.isNotEmpty(gprsConfigInfos) ? gprsConfigInfos.get(0) : null;
        if (gprsConfigInfo != null) {
            BigDecimal minCur = gprsConfigInfo.getValidDischargeCur().abs();
            BigDecimal maxCur = gprsConfigInfo.getMaxDischargeCur() == null ? BigDecimal.valueOf(100) : gprsConfigInfo.getMaxDischargeCur();
            maxCur = maxCur.abs();
            report.setVol(gprsConfigInfo.getValidDischargeVol().toString());
            report.setMaxCur(maxCur.toString());
            report.setMinCur(minCur.toString());
        }
        if (gprsConfigInfo == null || gprsConfigInfo.getDownCur() == null || gprsConfigInfo.getDownVol() == null) {
            LOGGER.info("不能找到相关的掉电配置，跳过计算掉电事件, 编号:{}", stationInfo.getGprsId());
        } else {
        	if (lossChargeEvent != null) {
        		lossChargeEvent.setGprsConfigInfo(gprsConfigInfo);
        		generateEvents(stationInfo, report, packDataInfos, lossChargeEvent);
			}
        }
        if (chargeEvent != null) {
        	chargeEvent.setGprsConfigInfo(gprsConfigInfo);
        	generateEvents(stationInfo, report, packDataInfos, chargeEvent);
		}
        if (dischargeEvent != null) {
        	dischargeEvent.setGprsConfigInfo(gprsConfigInfo);
        	generateEvents(stationInfo, report, packDataInfos, dischargeEvent);
		}
        
        StopWatch stopWatch = new StopWatch();
        stopWatch.start();
        List<PackDataInfo> validDischarges = getValidDischarge(gprsConfigInfo, packDataInfos);
        stopWatch.stop();
        LOGGER.debug("获取{}条有效放电,设备编号:{},耗时:{}",
                new Object[]{validDischarges.size(), gprsConfigInfo.getGprsId(), stopWatch});
        if (CollectionUtils.isNotEmpty(validDischarges)) {
            stopWatch.reset();
            stopWatch.start();
            List<ChargeDischargeReportItem> details = getLatestDischarges(validDischarges.get(validDischarges.size() - 1), packDataInfos, stationInfo);
            stopWatch.stop();
            LOGGER.debug("获取{}条最近一次有效放电详情, 设备编号:{},耗时:{}",
                    new Object[]{details.size(), stationInfo.getGprsId(), stopWatch});
            if (CollectionUtils.isNotEmpty(details)) {
                report.getDetails().addAll(details);
            }
            stopWatch.reset();
            stopWatch.start();
            List<ChargeDischargeReportItem> items = getEndVoltage(validDischarges, packDataInfos, stationInfo);
            stopWatch.stop();
            LOGGER.debug("获取{}条截止电压最低放电详情, 设备编号:{},耗时:{}",
                    new Object[]{items.size(), stationInfo.getGprsId(), stopWatch});
            if (CollectionUtils.isNotEmpty(items)) {
                report.getItems().addAll(items);
            }
        }
        return report;
    }
    
    private List<ChargeDischargeReportItem> getEndVoltage(List<PackDataInfo> validDischarges,
                                                          List<PackDataInfo> packDataInfos, StationInfo stationInfo) {
    	packDataInfos = packDataInfos.stream().sorted(Comparator.comparing(PackDataInfo::getRcvTime))
				  .collect(Collectors.toList());
        /*Map<Integer, List<PackDataInfo>> map = Maps.newHashMap();
        for (int i = 0; i < validDischarges.size(); i++) {
            PackDataInfo packDataInfo = validDischarges.get(i);
            List<PackDataInfo> items = getDischarges(packDataInfo, packDataInfos);
            if (CollectionUtils.isEmpty(items)) {
                continue;
            }
            map.put(i, items);
        }

        if (MapUtils.isEmpty(map)) {
            return Collections.emptyList();
        }

        BigDecimal value = BigDecimal.valueOf(Integer.MAX_VALUE);
        Integer matchedKey = null;
        for (Map.Entry<Integer, List<PackDataInfo>> entry : map.entrySet()) {
            List<PackDataInfo> list = entry.getValue();
            BigDecimal vol = list.get(list.size() - 1).getGenVol();
            if (value.compareTo(vol) > 0) {
                value = vol;
                matchedKey = entry.getKey();
            }
        }
        return convertToReportItem(map.get(matchedKey));*/
    	PackDataInfo packDataInfo = validDischarges.stream().min(Comparator.comparing(PackDataInfo::getGenVol)).get();
    	List<PackDataInfo> items = getDischarges(packDataInfo, packDataInfos);
    	return convertToReportItem(items, stationInfo);
    }

    private List<PackDataInfo> getDischarges(PackDataInfo packDataInfo,
                                             List<PackDataInfo> items) {
        int index = 0;
        for (int i = 0; i < items.size(); i++) {
            if (items.get(i).getId().equals(packDataInfo.getId())) {
                index = i;
                break;
            }
        }
        if (index < 10) {
            return Collections.emptyList();
        }
        // 向前找连续10条非放电态的数据
        int startIndex = 0;
        int count = 0;
        for (int i = index - 1; i >= 0; i--) {
            PackDataInfo item = items.get(i);
            if (!ModelCalculationServiceImpl.DISCHARGE_NAME.equals(item.getState())) {
                count++;
            } else {
                count = 0;
            }
            if (count >= 10) {
                startIndex = i;
                break;
            }
        }
        if (count < 10) {
        	List<PackDataInfo> forwardLookup = forwardLookup(items.get(0).getId(), packDataInfo.getGprsId());
			if (CollectionUtils.isEmpty(forwardLookup) || forwardLookup.size() < 10) {
				startIndex = 0;
			} else {
				List<PackDataInfo> collect = forwardLookup.stream()
						.sorted(Comparator.comparing(PackDataInfo::getId).reversed()).collect(Collectors.toList());
				collect = collect.subList(0, 10 - count);
				if (stateVerify(collect, "放电", true)) {
					startIndex = 0;
				} else {
					return Collections.emptyList();
				}
			}
        }
        // 向后找连续10条非放电态的数据
        count = 0;
        int endIndex = 0;
        for (int i = index + 1; i < items.size(); i++) {
            PackDataInfo item = items.get(i);
            if (!ModelCalculationServiceImpl.DISCHARGE_NAME.equals(item.getState())) {
                count++;
            } else {
                count = 0;
            }
            if (count >= 10) {
                endIndex = i + 1;
                break;
            }
        }
        if (count < 10) {
        	List<PackDataInfo> backwardLookup = backwardLookup(items.get(items.size() - 1).getId(),
					packDataInfo.getGprsId());
			if (CollectionUtils.isEmpty(backwardLookup) || backwardLookup.size() < 10) {
				endIndex = items.size();
			} else {
				List<PackDataInfo> collect = backwardLookup.stream()
						.sorted(Comparator.comparing(PackDataInfo::getId)).collect(Collectors.toList());
				collect = collect.subList(0, 10 - count);
				if (stateVerify(collect, "放电", true)) {
					endIndex = items.size();
				} else {
					return Collections.emptyList();
				}
			}
        }
        return items.subList(startIndex, endIndex);
    }

    private List<ChargeDischargeReportItem> getLatestDischarges(PackDataInfo packDataInfo,
                                                                List<PackDataInfo> items, StationInfo stationInfo) {
    	items = items.stream().sorted(Comparator.comparing(PackDataInfo::getRcvTime))
    						  .collect(Collectors.toList());
        List<PackDataInfo> packDataInfos = getDischarges(packDataInfo, items);
        if (CollectionUtils.isEmpty(packDataInfos)) {
            return Collections.emptyList();
        }
        return convertToReportItem(packDataInfos, stationInfo);
    }

    private List<ChargeDischargeReportItem> convertToReportItem(List<PackDataInfo> packDataInfos, StationInfo stationInfo) {
        return packDataInfos.stream()
                .map(item -> {
                    ChargeDischargeReportItem reportItem = new ChargeDischargeReportItem();
                    reportItem.setState(item.getState());
                    reportItem.setTime(MyDateUtils.getDateString(item.getRcvTime(), "yyyy/MM/dd HH:mm:ss"));
                    reportItem.setCur(item.getGenCur().toString());
                    reportItem.setVol(item.getGenVol().toString());
                    Map<String, String> cellMap = Maps.newHashMap();
                    for (int i = 1; i < stationInfo.getCellCount() + 1; i++) {
                        BigDecimal bigDecimal = (BigDecimal) BeanValueUtils.getValue("cellVol" + i, item);
                        cellMap.put(String.valueOf(i), bigDecimal == null ? StringUtils.EMPTY : bigDecimal.toString());
                    }
                    reportItem.setCellMap(cellMap);
                    return reportItem;
                }).collect(Collectors.toList());
    }

    private List<PackDataInfo> getValidDischarge(GprsConfigInfo gprsConfigInfo, List<PackDataInfo>
            packDataInfos) {
        if (gprsConfigInfo == null) {
            return Collections.emptyList();
        }
        int counter = 0;
		List<PackDataInfo> items = Lists.newArrayList();
		for (int i = 0; i < packDataInfos.size(); i++) {
			if (isValidDischarge(packDataInfos.get(i), gprsConfigInfo)) {
				counter++;
			} else {
				counter = 0;
			}
			if (counter >= 2) {
				items.add(packDataInfos.get(i));
				counter = 0;
			}
		}
        
        /*List<PackDataInfo> items = Lists.newArrayList();
        for (PackDataInfo item : packDataInfos) {
            boolean res = ModelCalculationServiceImpl.DISCHARGE_NAME.equals(item.getState());
            if (!res) {
                continue;
            }
            res = item.getGenVol().compareTo(gprsConfigInfo.getValidDischargeVol()) <= 0;
            if (!res) {
                continue;
            }
            BigDecimal minCur = gprsConfigInfo.getValidDischargeCur().abs();
            BigDecimal maxCur = gprsConfigInfo.getMaxDischargeCur() == null ? BigDecimal.valueOf(100) : gprsConfigInfo.getMaxDischargeCur();
            maxCur = maxCur.abs();
            res = (item.getGenCur().compareTo(minCur) >= 0 && item.getGenCur().compareTo(maxCur) <= 0)
                    || (item.getGenCur().compareTo(maxCur.multiply(BigDecimal.valueOf(-1))) >= 0 && item.getGenCur().compareTo(minCur.multiply(BigDecimal.valueOf(-1))) <= 0);
            if (!res) {
                continue;
            }
            items.add(item);
        }*/
        return items.stream()
                .sorted(Comparator.comparing(PackDataInfo::getRcvTime))
                .collect(Collectors.toList());
    }
    
    private boolean isValidDischarge(PackDataInfo item, GprsConfigInfo gprsConfigInfo) {
		boolean res = ModelCalculationServiceImpl.DISCHARGE_NAME.equals(item.getState());
		if (!res) {
			return false;
		}
		res = item.getGenVol().compareTo(gprsConfigInfo.getValidDischargeVol()) <= 0;
		if (!res) {
			return false;
		}
		BigDecimal minCur = gprsConfigInfo.getValidDischargeCur().abs();
		BigDecimal maxCur = gprsConfigInfo.getMaxDischargeCur() == null ? BigDecimal.valueOf(100)
				: gprsConfigInfo.getMaxDischargeCur();
		maxCur = maxCur.abs();
		res = (item.getGenCur().compareTo(minCur) >= 0 && item.getGenCur().compareTo(maxCur) <= 0)
				|| (item.getGenCur().compareTo(maxCur.multiply(BigDecimal.valueOf(-1))) >= 0
						&& item.getGenCur().compareTo(minCur.multiply(BigDecimal.valueOf(-1))) <= 0);
		if (!res) {
			return false;
		}
		return true;
	}

    private void generateEvents(StationInfo stationInfo,
                                ChargeDischargeReport report,
                                List<PackDataInfo> packDataInfos,
                                AbstractEvent event) {
        List<ChargeDischargeEvent> events = event.generateEvents(stationInfo.getGprsId(), packDataInfos, this);
        if (CollectionUtils.isNotEmpty(events)) {
        	//取出来的数据是按时间降序的，需求要按时间升序
        	Collections.reverse(events);
            report.getEvents().addAll(events);
        }
    }

    /**
     * 得到pack_data_info数据，时间降序
     * @param gprsId 
     * @param startTime
     * @param endTime
     * @return
     */
    private List<PackDataInfo> getPackDataInfosByRange(String gprsId, Date startTime, Date endTime) {
        List<PackDataInfo> packDataInfos = Lists.newArrayList();
        int pageNum = 0;
        int pageSize = 5000;
        int queryCount = 0;
        while (true) {
            StopWatch stopWatch = new StopWatch();
            stopWatch.start();
            Map<String, Object> paramMap = Maps.newHashMap();
            paramMap.put("gprsId", gprsId);
            paramMap.put("startTime", startTime);
            paramMap.put("endTime", endTime);
            paramMap.put("pageNum", pageNum);
            paramMap.put("pageSize", pageSize);
            List<PackDataInfo> items = packDataInfoMapper.getPackDataInfosByTimes(paramMap);
            queryCount++;
            LOGGER.debug("第{}次查询---->获取{}条测试数据,耗时:{}", new Object[]{queryCount, items.size(), stopWatch});
            if (CollectionUtils.isEmpty(items)) {
                break;
            }
            packDataInfos.addAll(items);
            if (items.size() < pageSize) {
                break;
            }
            pageNum += pageSize;
        }
        packDataInfos.sort(Comparator.comparing(PackDataInfo::getRcvTime));
        Collections.reverse(packDataInfos);
        LOGGER.debug("获取{}条测试数据, 基站编号:{}, 时间:{}~{}", new Object[]{packDataInfos.size(), gprsId,
                MyDateUtils.getDateString(startTime), MyDateUtils.getDateString(endTime)});
        return packDataInfos;
    }

    @Override
    public SuggestionReport generateSuggestionReport(Integer stationId) {
        if (stationId == null) {
            return null;
        }
        StationInfo stationInfo = stationInfoMapper.selectByPrimaryKey(stationId);
        if (stationInfo == null || StringUtils.equalsIgnoreCase(stationInfo.getGprsId(), "-1")) {
            return null;
        }
        Company company = companyMapper.selectByPrimaryKey(stationInfo.getCompanyId3());
        if (company == null) {
            return null;
        }
        SuggestionReport suggestionReport = new SuggestionReport();
        suggestionReport.setCompanyName(company.getCompanyName());
        suggestionReport.setExportTime(MyDateUtils.getDateString(new Date(), "yyyy-MM-dd HH:mm:ss"));
        List<SuggestionReportItem> items = Lists.newArrayList();
        createSuggestionReportHandler(suggestionReport, items, Lists.newArrayList(stationInfo), company);
        suggestionReport.setItems(items);
        return suggestionReport;
    }

    private SuggestionReportItem generateSuggestionReportItem(StationInfo stationInfo,
                                                              Company company,
                                                              GprsConfigInfo configInfo,
                                                              SugReplaceCellIndex sugReplaceCellIndex) {
        SuggestionReportItem item = new SuggestionReportItem();
        item.setAddress(stationInfo.getAddress());
        item.setCarrier(carrierMap.get(stationInfo.getOperatorType()));
        item.setCompanyName(company.getCompanyName());
        item.setDeviceType(configInfo == null ? null : deviceTypeMap.get(configInfo.getDeviceType()));
        item.setGprsId(stationInfo.getGprsId());
        item.setMaintainanceId(stationInfo.getMaintainanceId());
        item.setStationName(stationInfo.getName());
        // 淘汰
        List<Integer> list1 = Lists.newArrayList();
        // 利旧
        List<Integer> list2 = Lists.newArrayList();
        if (sugReplaceCellIndex != null) {
            for (int i = 1; i < 25; i++) {
                Integer replaceValue = (Integer) BeanValueUtils.getValue("cellReplace" + i, sugReplaceCellIndex);
                Integer typeValue = (Integer) BeanValueUtils.getValue("cellType" + i, sugReplaceCellIndex);
                if (replaceValue != null && typeValue != null && replaceValue == 1) {
                    if (typeValue == 1) {
                        list1.add(i);
                    } else if (typeValue == 0) {
                        list2.add(i);
                    }
                }
            }
        }
        if (CollectionUtils.isNotEmpty(list1)) {
            item.setSuggestion1("更换" + StringUtils.join(list1, "、") + "号单体（淘汰)");
        } else {
            item.setSuggestion1("暂不更换(淘汰)");
        }
        if (CollectionUtils.isNotEmpty(list2)) {
            item.setSuggestion2("更换" + StringUtils.join(list2, "、") + "号单体（利旧)");
        } else {
            item.setSuggestion2("暂不更换(利旧)");
        }
        return item;
    }

    private SugReplaceCellIndex suggestHandler(StationInfo stationInfo,
                                               Map<Integer, Set<Integer>> lCategoryMap,
                                               Map<String, PackDataExpandLatest> expandLatestMap,
                                               Map<String, Threshold> thresholdMap) {
        final String gprsId = stationInfo.getGprsId();
        Set<Integer> processedIndexes = Sets.newHashSet();
        SugReplaceCellIndex sugReplaceCellIndex = new SugReplaceCellIndex();
        sugReplaceCellIndex.setGprsId(gprsId);
        processLCategory(stationInfo, lCategoryMap.get(stationInfo.getId()), processedIndexes, sugReplaceCellIndex);
        PackDataExpandLatest packDataExpandLatest = expandLatestMap.get(gprsId);
        Threshold threshold = thresholdMap.get(gprsId);
        processKCategory(stationInfo, packDataExpandLatest, threshold, processedIndexes, sugReplaceCellIndex);
        processNmCategory(stationInfo, packDataExpandLatest, threshold, processedIndexes, sugReplaceCellIndex);
        return sugReplaceCellIndex;
    }

    /**
     * 有有效放电记录时, 电池组放电预测时长大于等于整治目标时长+Margin_time小时 ---》 不更换
     */
    private void nHandler(StationInfo stationInfo,
                          PackDataExpandLatest packDataExpandLatest,
                          Threshold threshold,
                          Set<Integer> processedIndexes,
                          SugReplaceCellIndex sugReplaceCellIndex) {
        if (packDataExpandLatest == null || processedIndexes.size() >= 24) {
            return;
        }
        if (packDataExpandLatest.getPackDischargeTimePred().compareTo(threshold.getSuggestTime().add(threshold.getMarginTime())) >= 0) {
            LOGGER.info("电池组放电预测时长:{}>=(整治目标时长:{}+整冶余量时长:{})属于N类,基站编号:{}",
                    new Object[]{packDataExpandLatest.getPackDischargeTimePred(), threshold.getSuggestTime(), threshold.getMarginTime(), stationInfo.getGprsId()});
            bindCellValue(stationInfo, 0, null, processedIndexes, sugReplaceCellIndex, "N");
        }
    }

    private void mHanlder(StationInfo stationInfo,
                          BigDecimal power,
                          PackDataExpandLatest packDataExpandLatest,
                          Threshold threshold,
                          Set<Integer> processedIndexes,
                          SugReplaceCellIndex sugReplaceCellIndex) {
        if (packDataExpandLatest == null || processedIndexes.size() >= 24) {
            return;
        }
        LOGGER.info("站点功率:{},电池组放电预测时长:{}", power, packDataExpandLatest.getPackDischargeTimePred());
        List<CellSort> cellSorts = cellSortsWhichNeedRepalceAndReject(stationInfo, processedIndexes, new Date(), packDataExpandLatest);
        // pack_discharge_time_pred小于1小时--->淘汰、更换
        if (packDataExpandLatest.getPackDischargeTimePred().compareTo(BigDecimal.ONE) < 0) {
            bindCellValue(stationInfo, 1, 1, processedIndexes, sugReplaceCellIndex, "M");
        } else if (packDataExpandLatest.getPackDischargeTimePred().compareTo(BigDecimal.ONE) >= 0 &&
                packDataExpandLatest.getPackDischargeTimePred().compareTo(BigDecimal.valueOf(1.5)) < 0) {
            // 1≤pack_discharge_time_pred<1.5
            if (power.compareTo(BigDecimal.valueOf(3000)) < 0) {
                // 当P<3000W时 --> 淘汰、更换
                bindCellValue(stationInfo, 1, 1, processedIndexes, sugReplaceCellIndex, "M");
            } else if (power.compareTo(BigDecimal.valueOf(3000)) >= 0
                    && power.compareTo(BigDecimal.valueOf(4500)) <= 0) {
                // 3000W≤P≤4500 --> 淘汰、更换
                bindCellValue(stationInfo, 1, 1, processedIndexes, sugReplaceCellIndex, "M");
            } else {
                // 当4500<P时 --> 利旧、更换
                bindCellValue(stationInfo, 1, 0, processedIndexes, sugReplaceCellIndex, "M");
            }
        } else if (packDataExpandLatest.getPackDischargeTimePred().compareTo(BigDecimal.valueOf(1.5)) >= 0 &&
                packDataExpandLatest.getPackDischargeTimePred().compareTo(BigDecimal.valueOf(2.5)) < 0) {
            int counter;
            // 1.５≤pack_discharge_time_pred<2.5
            if (power.compareTo(BigDecimal.valueOf(3000)) < 0) {
                // 当P<3000W时 ---> suggest_average_num*2个需要更换标记为淘汰、更换, 不需要更换的标记为利旧、不更换
                counter = threshold.getSuggestAverageNum() * 2;
            } else if (power.compareTo(BigDecimal.valueOf(3000)) >= 0
                    && power.compareTo(BigDecimal.valueOf(4500)) <= 0) {
                // 3000W≤P≤4500 ---> suggest_average_num*2＋２个需要更换标记为淘汰、更换, 不需要更换的标记为利旧、不更换
                counter = threshold.getSuggestAverageNum() * 2 + 2;
            } else {
                // 4500<P ---> suggest_average_num*2＋４个需要更换标记为利旧、更换, 不需要更换的标记为利旧、不更换
                counter = threshold.getSuggestAverageNum() * 2 + 4;
            }
            processCellSort(counter, stationInfo, cellSorts, processedIndexes, sugReplaceCellIndex);
        } else {
            int counter;
            // 有放电记录+ 2.5≤pack_discharge_time_pred<suggest_time+Margin_time小时；无放电记录+2.5≤pack_discharge_time_pred
            if (power.compareTo(BigDecimal.valueOf(3000)) < 0) {
                // 当P<3000W时 ---> suggest_average_num个需要更换标记为淘汰、更换, 不需要更换的标记为利旧、不更换
                counter = threshold.getSuggestAverageNum();
            } else if (power.compareTo(BigDecimal.valueOf(3000)) >= 0
                    && power.compareTo(BigDecimal.valueOf(4500)) <= 0) {
                // 3000W≤P≤4500 ---> suggest_average_num+1个需要更换标记为淘汰、更换, 不需要更换的标记为利旧、不更换
                counter = threshold.getSuggestAverageNum() + 1;
            } else {
                // 4500<P ---> suggest_average_num+2个需要更换标记为利旧、更换, 不需要更换的标记为利旧、不更换
                counter = threshold.getSuggestAverageNum() + 2;
            }
            processCellSort(counter, stationInfo, cellSorts, processedIndexes, sugReplaceCellIndex);
        }
    }

    private void processCellSort(int counter,
                                 StationInfo stationInfo,
                                 List<CellSort> cellSorts,
                                 Set<Integer> processedIndexes,
                                 SugReplaceCellIndex sugReplaceCellIndex) {
        if (counter > cellSorts.size()) {
            // 全部需要更换标记为淘汰、更换
            bindCellValue(stationInfo, 1, 1, processedIndexes, sugReplaceCellIndex, "M");
        } else {
            for (int i = 0; i < cellSorts.size(); i++) {
                Integer cell = cellSorts.get(i).getIndex();
                if (i < counter) {
                    bindValue(cell, 1, 1, sugReplaceCellIndex);
                } else {
                    bindValue(cell, 0, 0, sugReplaceCellIndex);
                }
                processedIndexes.add(cell);
            }
        }
    }

    private List<CellSort> cellSortsWhichNeedRepalceAndReject(StationInfo stationInfo,
                                                              Set<Integer> processedIndexes,
                                                              Date currentDate,
                                                              PackDataExpandLatest packDataExpandLatest) {
        Date defaultCellUseTime = DateUtils.addYears(currentDate, -3);
        // 投入使用时间分区:A\B\C\D, 单体个数排序先按Ａ／Ｂ／Ｃ／Ｄ分区，区内按单体容量排序，容量越低的排序越靠前，更换时先更换排序靠前的
        Map<Integer, Date> cellUseTimeMap = cellInfoMapper.getLatestByGprsId(stationInfo.getGprsId())
                .stream()
                .collect(Collectors.toMap(CellInfo::getCellIndex, item -> item.getUseFrom() == null ? defaultCellUseTime : item.getUseFrom()));

        Map<String, List<CellSort>> groupMap = Maps.newTreeMap();
        List<CellSort> cellSorts = Lists.newArrayList();
        for (int cell = 1; cell < 25; cell++) {
            if (CollectionUtils.isNotEmpty(processedIndexes) && processedIndexes.contains(cell)) {
                continue;
            }
            Date cellUseTime = cellUseTimeMap.get(cell);
            if (cellUseTime == null) {
                cellUseTime = defaultCellUseTime;
            }
            cellSorts.add(new CellSort(cell, cellUseTime, (BigDecimal) BeanValueUtils.getValue("cellCap" + cell, packDataExpandLatest)));
        }
        if (cellSorts.size() > 1) {
            cellSorts.sort(Comparator.comparing(CellSort::getUseTime));
            long start = cellSorts.get(0).getUseTime().getTime();
            long diff = cellSorts.get(cellSorts.size() - 1).getUseTime().getTime() - start;
            long val = diff / 4;
            if (val == 0) {
                cellSorts.sort(Comparator.comparing(CellSort::getCapacity));
                groupMap.put("A", cellSorts);
            } else {
                Map<String, List<Long>> groupRangeMap = Maps.newTreeMap();
                long begin = start;
                long end;
                for (String group : new String[]{"A", "B", "C", "D"}) {
                    List<Long> ranges = groupRangeMap.computeIfAbsent(group, value -> Lists.newArrayList());
                    end = begin + val;
                    ranges.add(begin);
                    ranges.add(end);
                    begin = end;
                }

                for (Map.Entry<String, List<Long>> entry : groupRangeMap.entrySet()) {
                    String group = entry.getKey();
                    List<Long> ranges = entry.getValue();
                    long source = ranges.get(0);
                    long dest = ranges.get(1);
                    for (CellSort cellSort : cellSorts) {
                        long time = cellSort.getUseTime().getTime();
                        if (source <= time && time < dest) {
                            groupMap.computeIfAbsent(group, value -> Lists.newArrayList()).add(cellSort);
                        }
                    }
                }
            }
        } else {
            groupMap.put("A", cellSorts);
        }

        List<CellSort> list = Lists.newArrayList();
        for (Map.Entry<String, List<CellSort>> entry : groupMap.entrySet()) {
            List<CellSort> items = entry.getValue().stream().sorted(Comparator.comparing(CellSort::getCapacity)).collect(Collectors.toList());
            list.addAll(items);
        }
        return list;
    }

    /**
     * 
     * @param stationInfo
     * @param replaceValue 1,更换，0,不更换
     * @param typeValue 1,淘汰, 0, 利旧
     * @param processedIndexes
     * @param sugReplaceCellIndex
     * @param categoryName
     */
    private void bindCellValue(StationInfo stationInfo,
                               Integer replaceValue,
                               Integer typeValue,
                               Set<Integer> processedIndexes,
                               SugReplaceCellIndex sugReplaceCellIndex,
                               String categoryName) {
        List<Integer> cells = Lists.newArrayList();
        for (int cell = 1; cell < 25; cell++) {
            if (CollectionUtils.isNotEmpty(processedIndexes) && processedIndexes.contains(cell)) {
                continue;
            }
            cells.add(cell);
            bindValue(cell, replaceValue, typeValue, sugReplaceCellIndex);
        }
        if (CollectionUtils.isNotEmpty(cells)) {
            processedIndexes.addAll(cells);
        }
        LOGGER.info("{}个单体({})属于{}类,基站编号:{}",
                new Object[]{cells.size(), StringUtils.join(cells, ","), categoryName, stationInfo.getGprsId()});
    }

    /**
     * N & M 类别
     */
    private void processNmCategory(StationInfo stationInfo,
                                   PackDataExpandLatest packDataExpandLatest,
                                   Threshold threshold,
                                   Set<Integer> processedIndexes,
                                   SugReplaceCellIndex sugReplaceCellIndex) {
        if (packDataExpandLatest == null || processedIndexes.size() >= 24) {
            return;
        }
        Date currentDate = new Date();

        PackDataInfo packDataInfo = modelCalculationService.getLatestDischargeRecord(stationInfo.getGprsId(), currentDate);
        List<PackDataInfo> packDataInfos = modelCalculationService.getValidVoltage(stationInfo.getGprsId(), currentDate);
        BigDecimal voltageAvg = null;
        if (packDataInfos.size() == 12) {
        	// 去掉最大值，最小值
        	List<PackDataInfo> filteredPdi = packDataInfos.stream().sorted(Comparator.comparing(PackDataInfo::getGenVol)).collect(Collectors.toList());
        	filteredPdi.remove(0);
        	filteredPdi.remove(filteredPdi.size()-1);
            voltageAvg = BigDecimal.valueOf(filteredPdi.stream().mapToDouble(item -> item.getGenVol().doubleValue()).average().getAsDouble());
        }
        BigDecimal power = BigDecimal.valueOf(2400);
        power = stationInfo.getLoadPower() == null ? power : stationInfo.getLoadPower();
        if (packDataInfo == null) {
            // 无有效放电
            if (stationInfo.getLoadPower() != null && voltageAvg != null) {
                packDataInfos.sort(Comparator.comparing(PackDataInfo::getGenVol));
                packDataInfos.remove(0);
                packDataInfos.remove(packDataInfos.size() - 1);

                power = stationInfo.getLoadPower().divide(BigDecimal.valueOf(48)).multiply(voltageAvg).abs();
            }
            mHanlder(stationInfo, power, packDataExpandLatest, threshold, processedIndexes, sugReplaceCellIndex);
        } else {
            // 有有效放电
            if (voltageAvg != null) {
                List<PackDataInfo> discharges = modelCalculationService.getContinuousDischargeRecords(packDataInfo, stationInfo.getGprsId());
                BigDecimal currentAvg = modelCalculationService.get102DischargeAverageCurrent(discharges);
                power = currentAvg.multiply(voltageAvg).abs();
            }
            nHandler(stationInfo, packDataExpandLatest, threshold, processedIndexes, sugReplaceCellIndex);
            mHanlder(stationInfo, power, packDataExpandLatest, threshold, processedIndexes, sugReplaceCellIndex);
        }
    }

    /**
     * K 类别，电池单体预测容量百分比（%） < suggest_cell_cap_percent
     * 单体容量/电池组容量  < suggest_cell_cap_percent
     */
    private void processKCategory(StationInfo stationInfo,
                                  PackDataExpandLatest packDataExpandLatest,
                                  Threshold threshold,
                                  Set<Integer> processedIndexes,
                                  SugReplaceCellIndex sugReplaceCellIndex) {
        if (packDataExpandLatest == null || processedIndexes.size() >= 24) {
            return;
        }
        List<Integer> cells = Lists.newArrayList();
//        final Integer packCapPred = packDataExpandLatest.getPackCapPred();
        final double packCap = modelCalculationService.getStandardCapacityFromPackType(stationInfo.getPackType());
        for (int i = 1; i < 25; i++) {
            if (CollectionUtils.isNotEmpty(processedIndexes) && processedIndexes.contains(i)) {
                continue;
            }
            BigDecimal value = (BigDecimal) BeanValueUtils.getValue("cellCap" + i, packDataExpandLatest);
            if (value.divide(BigDecimal.valueOf(packCap), 3, BigDecimal.ROUND_HALF_UP).compareTo(threshold.getSuggestCellCapPercent()) < 0) {
                cells.add(i);
                bindValue(i, 1, 1, sugReplaceCellIndex);
            }
        }
        if (CollectionUtils.isNotEmpty(cells)) {
            processedIndexes.addAll(cells);
        }
        LOGGER.info("{}个单体({})属于K类,基站编号:{}",
                new Object[]{cells.size(), StringUtils.join(cells, ","), stationInfo.getGprsId()});
    }

    /**
     * L 类别，巡检标记单体编号
     */
    private void processLCategory(StationInfo stationInfo,
                                  Set<Integer> cells,
                                  Set<Integer> processedCells,
                                  SugReplaceCellIndex sugReplaceCellIndex) {
    	// 电池组最近一次整治时间（就是巡检记录 operate_type = 3）   >  最近一次巡检标记的异常单体时间  则L类 为 0
    	// 否则 L类按巡检人员标记的统计
    	RoutingInspections inspections = new RoutingInspections();
    	inspections.setOperateType(3);
    	inspections.setStationId(stationInfo.getId());
    	RoutingInspections latestInspections = routingInspectionsMapper.selectOneLatestSelective(inspections);
    	if (latestInspections != null) { // 有整治记录信息
    		// 得到最近的整治时间
    		Date latestInspectionTime = latestInspections.getOperateTime();
			// 以最近的整治时间，往后查询；得到有标记异常单体的记录，则 L取标记信息
			Map param = new HashMap<>();
			param.put("stationId", stationInfo.getId());
			param.put("operateType", 2);
			param.put("startTime", latestInspectionTime);
			List<RoutingInspections> inspectSignCells = routingInspectionsMapper.selectListHasInspectSignCell(param);
			if (CollectionUtils.isEmpty(inspectSignCells)) {
				return;
			}
		}
    	
        if (CollectionUtils.isNotEmpty(cells)) {
            for (Integer cell : cells) {
                bindValue(cell, 1, 1, sugReplaceCellIndex);
            }
            processedCells.addAll(cells);
        }
        LOGGER.info("{}个单体({})属于L类,基站编号:{}",
                new Object[]{CollectionUtils.isEmpty(cells) ? 0 : cells.size(), StringUtils.join(cells, ","), stationInfo.getGprsId()});
    }

    private void bindValue(Integer cell, Integer replaceValue,
                           Integer typeValue, SugReplaceCellIndex sugReplaceCellIndex) {
        BeanValueUtils.bindProperty("cellReplace" + cell, replaceValue, sugReplaceCellIndex);
        BeanValueUtils.bindProperty("cellType" + cell, typeValue, sugReplaceCellIndex);
    }

    private static class Threshold {
        private String gprsId;
        private BigDecimal marginTime;
        private BigDecimal suggestCellCapPercent;
        private BigDecimal suggestTime;
        private Integer suggestAverageNum;

        public Threshold(String gprsId, BigDecimal marginTime, BigDecimal suggestCellCapPercent, BigDecimal suggestTime, Integer suggestAverageNum) {
            this.gprsId = gprsId;
            this.marginTime = marginTime;
            this.suggestCellCapPercent = suggestCellCapPercent;
            this.suggestTime = suggestTime;
            this.suggestAverageNum = suggestAverageNum;
        }

        public String getGprsId() {
            return gprsId;
        }

        public void setGprsId(String gprsId) {
            this.gprsId = gprsId;
        }

        public BigDecimal getMarginTime() {
            return marginTime;
        }

        public void setMarginTime(BigDecimal marginTime) {
            this.marginTime = marginTime;
        }

        public BigDecimal getSuggestCellCapPercent() {
            return suggestCellCapPercent;
        }

        public void setSuggestCellCapPercent(BigDecimal suggestCellCapPercent) {
            this.suggestCellCapPercent = suggestCellCapPercent;
        }

        public BigDecimal getSuggestTime() {
            return suggestTime;
        }

        public void setSuggestTime(BigDecimal suggestTime) {
            this.suggestTime = suggestTime;
        }

        public Integer getSuggestAverageNum() {
            return suggestAverageNum;
        }

        public void setSuggestAverageNum(Integer suggestAverageNum) {
            this.suggestAverageNum = suggestAverageNum;
        }
    }

    private static class CellSort {
        private Integer index;
        private Date useTime;
        private BigDecimal capacity;

        public CellSort(Integer index, Date useTime, BigDecimal capacity) {
            this.index = index;
            this.useTime = useTime;
            this.capacity = capacity;
        }

        public Integer getIndex() {
            return index;
        }

        public void setIndex(Integer index) {
            this.index = index;
        }

        public Date getUseTime() {
            return useTime;
        }

        public void setUseTime(Date useTime) {
            this.useTime = useTime;
        }

        public BigDecimal getCapacity() {
            return capacity;
        }

        public void setCapacity(BigDecimal capacity) {
            this.capacity = capacity;
        }
    }

    private enum OperatorType {
        Search,
        Create
    }

    private enum EventType {
        Discharge,
        Charge,
        Loss_Charge
    }
    
    private enum StationFilter {
		maxGenVol, minGenVol, 
		maxGenCur, minGenCur, 
		maxEnvironTem, minEnvironTem, 
		maxCellVol, minCellVol, 
		maxCellTem, minCellTem, 
		maxComSuc, minComSuc
	}
    
    private Logger logger = LoggerFactory.getLogger("TAG"); 
    
    @Override
	public StationReport generateStationVolCurStr(StationReport stationReport) {
		StationReport report = new StationReport();
		BeanUtils.copyProperties(stationReport, report);
		report.setStartRcvTimeStr(JxlsUtil.dateFmt(stationReport.getStartRcvTime()));
		report.setEndRcvTimeStr(JxlsUtil.dateFmt(stationReport.getEndRcvTime()));
		List<StationReportItem> items = new ArrayList<>();
		report.setItems(items);
		Map<String, StationReportFilter> filter = new HashMap<>();
		report.setFilter(filter);
		// 过滤条件赋值
		StationFilter[] values = StationFilter.values();
		for (int i = 0; i < values.length; i++) {
			Parameter record = new Parameter();
			record.setParameterCode(values[i].toString());
			List<Parameter> selectListSelective = parameterMapper.selectListSelective(record);
			Map<String, Parameter> parameterMap = selectListSelective.stream()
					.collect(Collectors.toMap(Parameter::getParameterCategory, p -> p));
			StationReportFilter itemFilter = null;
			for(String key : parameterMap.keySet()){
				if (filter.containsKey(key)) {
					itemFilter = filter.get(key);
				}else {
					itemFilter = new StationReportFilter();
					filter.put(key, itemFilter);
				}
				String parameterValue = parameterMap.get(key).getParameterValue();
				switch (values[i]) {
				case maxGenVol:
				case minGenVol:
				case maxGenCur:
				case minGenCur:
				case maxCellVol:
				case minCellVol:
					BigDecimal convertDec = new BigDecimal(parameterValue);
					ReflectUtil.setValueByKet(itemFilter, values[i].toString(), convertDec);
					break;
				case maxEnvironTem:
				case minEnvironTem:
				case maxCellTem:
				case minCellTem:
				case maxComSuc:
				case minComSuc:
					Integer convertInt = new Integer(parameterValue);
					ReflectUtil.setValueByKet(itemFilter, values[i].toString(), convertInt);
					break;
				}
			}
		}

		SearchStationInfoPagingVo query = new SearchStationInfoPagingVo();
		query.setCompanyId3(report.getCompanyId3());
		query.setLinkStatus(report.getLinkStatus());
		query.setState(report.getState());
		query.setDelFlag(0);//只查询未删除的
		query.setPageSize(null);
		query.setPageNo(null);
		StopWatch stopWatch = new StopWatch();
		stopWatch.reset();
		stopWatch.start();
		List<StationInfo> selectListSelective = stationInfoMapper.selectListSelectivePaging(query);
		stopWatch.stop();
		report.setStationTotal(selectListSelective == null ? 0 : selectListSelective.size());
		logger.info("station查询时间-->"+stopWatch.getTime());
		logger.info("station查询条数-->"+(selectListSelective == null ? 0 : selectListSelective.size()));
		for (StationInfo stationInfo : selectListSelective) {
			 if (stationInfo.getLinkStatus() == null || stationInfo.getLinkStatus() == 0) {
				 StationReportItem item = new StationReportItem();
				 BeanUtils.copyProperties(stationInfo, item);
				 item.setRemark("设备离线或未绑定设备");
				 items.add(item);
				 continue;
			 }
			 exceptionStationCheck(report,items,stationInfo);
		}
		return report;
	}

	private void exceptionStationCheck(StationReport report,List<StationReportItem> items,
										StationInfo stationInfo) {
		Map<String, Object> paramMap = Maps.newHashMap();
		paramMap.put("gprsId", stationInfo.getGprsId());
		paramMap.put("startTime", report.getStartRcvTime());
		paramMap.put("endTime", report.getEndRcvTime());
		StopWatch stopWatch = new StopWatch();
		stopWatch.reset();
		stopWatch.start();
		List<PackDataInfo> pdiByTime = packDataInfoMapper.selectListByTime(paramMap);
		stopWatch.stop();
		logger.info("packData查询时间-->"+stopWatch.getTime());
		logger.info("packData查询条数-->"+(pdiByTime == null ? 0 : pdiByTime.size()));
		if (pdiByTime == null || pdiByTime.size() < 10) {
			StationReportItem item = new StationReportItem();
			BeanUtils.copyProperties(stationInfo, item);
			item.setRemark("数据量过少");
			items.add(item);
			return;
		}
		StationReportFilter filter = report.getFilter().get(stationInfo.getDeviceType() + "");
		int exceptionCount = 0; // 一个电池组，只取5条异常数据
		for (PackDataInfo packDataInfo : pdiByTime) {
			StationReportItem item = new StationReportItem();
			BeanUtils.copyProperties(stationInfo, item);
			boolean hasException = false;
			boolean hasDeviceExce = false;
			if (packDataInfo.getGenVol() == null) {
				item.setGenVolStr(packDataInfo.getGenVol() + "(无值)");
				hasException = true;
				hasDeviceExce = true;
			} else if (packDataInfo.getGenVol().compareTo(filter.getMaxGenVol()) == 1) {
				item.setGenVolStr(packDataInfo.getGenVol() + "(过高)");
				hasException = true;
				hasDeviceExce = true;
			} else if (packDataInfo.getGenVol().compareTo(filter.getMinGenVol()) == -1) {
				item.setGenVolStr(packDataInfo.getGenVol() + "(过低)");
				hasException = true;
				hasDeviceExce = true;
			}

			if (packDataInfo.getGenCur() == null) {
				item.setGenCurStr(packDataInfo.getGenCur() + "(无值)");
				hasException = true;
				hasDeviceExce = true;
			} else if (packDataInfo.getGenCur().compareTo(filter.getMaxGenCur()) == 1) {
				item.setGenCurStr(packDataInfo.getGenCur() + "(过高)");
				hasException = true;
				hasDeviceExce = true;
			} else if (packDataInfo.getGenCur().compareTo(filter.getMinGenCur()) == -1) {
				item.setGenCurStr(packDataInfo.getGenCur() + "(过低)");
				hasException = true;
				hasDeviceExce = true;
			}

			if (packDataInfo.getEnvironTem() == null) {
				item.setEnvironTemStr(packDataInfo.getEnvironTem() + "(无值)");
				hasException = true;
				hasDeviceExce = true;
			} else if (packDataInfo.getEnvironTem().compareTo(filter.getMaxEnvironTem()) == 1) {
				item.setEnvironTemStr(packDataInfo.getEnvironTem() + "(过高)");
				hasException = true;
				hasDeviceExce = true;
			} else if (packDataInfo.getEnvironTem().compareTo(filter.getMinEnvironTem()) == -1) {
				item.setEnvironTemStr(packDataInfo.getEnvironTem() + "(过低)");
				hasException = true;
				hasDeviceExce = true;
			}
			StringBuffer pdiExce = new StringBuffer();
			if (hasDeviceExce) {
				pdiExce.append("主机异常");
			}
			for (int i = 1; i < stationInfo.getCellCount() + 1; i++) {
				StringBuffer sb = new StringBuffer();
				boolean hasSubDeviceExce = false;
				BigDecimal cellVol = (BigDecimal) ReflectUtil.getValueByKey(packDataInfo, "cellVol" + i);
				Integer cellTem = (Integer) ReflectUtil.getValueByKey(packDataInfo, "cellTem" + i);
				Integer comSuc = (Integer) ReflectUtil.getValueByKey(packDataInfo, "comSuc" + i);
				if (cellVol == null || cellVol.compareTo(filter.getMaxCellVol()) == 1
						|| cellVol.compareTo(filter.getMinCellVol()) == -1) {
					sb.append("电压:" + cellVol);
					hasException = true;
					hasSubDeviceExce = true;
				}

				if (comSuc == null || comSuc.compareTo(filter.getMaxComSuc()) == 1
						|| comSuc.compareTo(filter.getMinComSuc()) == -1) {
					sb.append(sb.length() > 0 ? "," : "").append("通讯成功率:" + comSuc);
					hasException = true;
					hasSubDeviceExce = true;
				}

				if (cellTem == null || cellTem.compareTo(filter.getMaxCellTem()) == 1
						|| cellTem.compareTo(filter.getMinCellTem()) == -1) {
					sb.append(sb.length() > 0 ? "," : "").append("温度:" + cellTem);
					hasException = true;
					hasSubDeviceExce = true;
				}
				
				if (hasSubDeviceExce) {
					pdiExce.append(pdiExce.length() > 0 ? "," : "").append(i + "号从机异常");
				}
				ReflectUtil.setValueByKet(item, "cellStr" + i, sb.toString());
			}
			if (hasException) {
				if (exceptionCount >= 5) {
					return;
				}
				exceptionCount++;
				item.setRemark(pdiExce.toString());
				items.add(item);
			}
		}
		
	}
	
	@Override
	public List<ChargeDischargeEvent> getChargeDischargeEvent(String gprsId, Date startTime, Date endTime,
																AbstractEvent event,
																EventParams params) {
		if (event == null) {
			return null;
		}
		event.setParams(params);
		// 得到 PackDataInfo ，时间降序
		List<PackDataInfo> packDataInfos = getPackDataInfosByRange(gprsId, startTime, endTime);
		if (CollectionUtils.isEmpty(packDataInfos)) {
			return null;
		}
		List<GprsConfigInfo> gprsConfigInfos = gprsConfigInfoMapper.selectByGprsIds(Lists.newArrayList(gprsId));
        GprsConfigInfo gprsConfigInfo = CollectionUtils.isNotEmpty(gprsConfigInfos) ? gprsConfigInfos.get(0) : null;
        List<ChargeDischargeEvent> events = null;
        if (gprsConfigInfo == null) {
        	LOGGER.info("不能得到设备相关信息,编号:{}", gprsId);
		}else {
			try {
				event.setGprsConfigInfo(gprsConfigInfo);
		        events = event.generateEvents(gprsId, packDataInfos, this);
			} catch (Exception e) {
				LOGGER.info("不能找到相关的掉电配置,编号:{}", gprsId);
			}
		}
		return events;
	}
	
	public List<PackDataInfo> forwardLookup(Integer startId, String gprsId) {
		Map<String, Object> paramMap = Maps.newHashMap();
		paramMap.put("gprsId", gprsId);
		paramMap.put("id", startId);
		paramMap.put("pageNum", 0);
		paramMap.put("pageSize", 10);
		return packDataInfoMapper.getPackDataInfosWhichIdLessThanGivenValue(paramMap);
	}

	public List<PackDataInfo> backwardLookup(Integer startId, String gprsId) {
		Map<String, Object> paramMap = Maps.newHashMap();
		paramMap.put("gprsId", gprsId);
		paramMap.put("id", startId);
		paramMap.put("pageNum", 0);
		paramMap.put("pageSize", 10);
		return packDataInfoMapper.getPackDataInfosWhichGreaterThanGivenValue(paramMap);
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
}
