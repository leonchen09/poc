package com.station.moudles.helper;

import com.google.common.collect.Lists;
import com.station.common.utils.MyDateUtils;
import com.station.moudles.entity.GprsConfigInfo;
import com.station.moudles.entity.PackDataInfo;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.vo.report.ChargeDischargeEvent;
import org.apache.commons.lang3.ArrayUtils;
import org.bouncycastle.crypto.util.Pack;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.math.BigDecimal;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

public abstract class AbstractEvent {
    protected Logger logger = LoggerFactory.getLogger(getClass());
    protected final static int THRESHOLD = 10;

    protected final String type;
    
    private boolean isOneReport;
    
    private GprsConfigInfo gprsConfigInfo;
    
    private EventParams params;

    public AbstractEvent(String type, boolean isOneReport) {
        this.type = type;
        this.isOneReport = isOneReport;
    }

    public List<ChargeDischargeEvent> generateEvents(String gprsId, List<PackDataInfo> packDataInfos) {
        if (params == null) {
			params = new EventParams();
		}else {
			if (params.currentCount < 0 || params.forwardCount < 0 || params.backwardCount < 0) {
				logger.info("传入了非法参数EventParams,编号:{}", type, gprsId);
				return null;
			}
		}
        packDataInfos = packDataInfos.stream().sorted(Comparator.comparing(PackDataInfo::getRcvTime).reversed())
        					  				  .collect(Collectors.toList());
    	List<ChargeDischargeEvent> events = Lists.newArrayList();
        int fromIndex = 0;
        while (true) {
//            int[] lossChargeIndex = getEventRange(gprsId, fromIndex, packDataInfos);
            int[] lossChargeIndex = getEventRangeByParams(gprsId, fromIndex, packDataInfos);
            if (ArrayUtils.isEmpty(lossChargeIndex)) {
                break;
            }
            List<PackDataInfo> items = packDataInfos.subList(lossChargeIndex[0], lossChargeIndex[1]);
            PackDataInfo item = items.get(items.size() - 1 - params.forwardCount);//items.get(0);

            ChargeDischargeEvent event = new ChargeDischargeEvent();
            event.setEvent(type + "事件");
            event.setStartTime(MyDateUtils.getDateString(item.getRcvTime(), "yyyy/MM/dd HH:mm:ss"));
//            event.setStartCurrent(item.getGenCur().toString());
            event.setStartVoltage(item.getGenVol().toString());
            item = items.get(0 + params.backwardCount);//items.get(items.size() - 11);
            event.setEndTime(MyDateUtils.getDateString(item.getRcvTime(), "yyyy/MM/dd HH:mm:ss"));
//            event.setEndCurrent(item.getGenCur().toString());
            event.setEndVoltage(item.getGenVol().toString());
            event.setDetails(items);
            events.add(event);
            fromIndex = lossChargeIndex[1];
            if (isOneReport) {
				break;
			}
        }
        return events;
    }

    private int[] getEventRange(String gprsId, int fromIndex, List<PackDataInfo> packDataInfos) {
        if (fromIndex + 20 > packDataInfos.size()) {
            logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
            return null;
        }
        int counter = 0;
        int startIndex = 0;
        for (int i = fromIndex; i < packDataInfos.size(); i++) {
            PackDataInfo packDataInfo = packDataInfos.get(i);
            if (eventCondition(packDataInfo)) {
            	// 放电时，电流小于放电阈值，充电时，电流大于充电阈值
            	switch (type) {
				case "放电":
					if (gprsConfigInfo != null) {
						BigDecimal dischargeCur = gprsConfigInfo.getValidDischargeCur().abs().multiply(BigDecimal.valueOf(-1));
						if (packDataInfo.getGenCur().compareTo(dischargeCur) < 0) {
							counter++;
						}
					}
					break;
				case "充电":
					if (gprsConfigInfo != null) {
						BigDecimal chargeCur = gprsConfigInfo.getValidChargeCur().abs();
						if (packDataInfo.getGenCur().compareTo(chargeCur) > 0) {
							counter++;
						}
					}
					break;
				default:
					counter++;
					break;
				}
            } else {
                counter = 0;
            }
            if (counter >= THRESHOLD) {
                startIndex = i - 9;
                break;
            }
        }
        // 找连续10条满足条件的数据，且之后至少还有10条数据
        if (counter < THRESHOLD || startIndex + 19 > packDataInfos.size()) {
            logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
            return null;
        }
        counter = 0;
        int endIndex = 0;
        for (int i = startIndex + 9; i < packDataInfos.size(); i++) {
            PackDataInfo packDataInfo = packDataInfos.get(i);
            if (!eventCondition(packDataInfo)) {
                counter++;
            } else {
                counter = 0;
            }
            if (counter >= THRESHOLD) {
                endIndex = i;
                break;
            }
        }
        if (counter < THRESHOLD) {
            logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
            return null;
        }
        return new int[]{startIndex, endIndex + 1};
    }

    protected abstract boolean eventCondition(PackDataInfo packDataInfo);

	public GprsConfigInfo getGprsConfigInfo() {
		return gprsConfigInfo;
	}

	public void setGprsConfigInfo(GprsConfigInfo gprsConfigInfo) {
		this.gprsConfigInfo = gprsConfigInfo;
	}
	
	public EventParams getParams() {
		return params;
	}

	public void setParams(EventParams params) {
		this.params = params;
	}

	/**
	 * 通过动态设置参数，查询充电事件
	 * @param gprsId
	 * @param fromIndex
	 * @param packDataInfos
	 * @return
	 */
	private int[] getEventRangeByParams(String gprsId, int fromIndex, List<PackDataInfo> packDataInfos) {
        int minCount = params.backwardCount + params.forwardCount + params.currentCount;
		if (fromIndex + minCount > packDataInfos.size()) {
            logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
            return null;
        }
        int counter = 0;
        int startIndex = 0;
        int endIndex = 0;
        for (int i = fromIndex; i < packDataInfos.size(); i++) {
            PackDataInfo packDataInfo = packDataInfos.get(i);
            if (eventCondition(packDataInfo)) {
            	// 放电时，电流小于放电阈值，充电时，电流大于充电阈值
            	switch (type) {
				case "放电":
					if (gprsConfigInfo != null) {
						BigDecimal dischargeCur = gprsConfigInfo.getValidDischargeCur().abs().multiply(BigDecimal.valueOf(-1));
						if (packDataInfo.getGenCur().compareTo(dischargeCur) < 0) {
							counter++;
						}
					}
					break;
				case "充电":
					if (gprsConfigInfo != null) {
						BigDecimal chargeCur = gprsConfigInfo.getValidChargeCur().abs();
						if (packDataInfo.getGenCur().compareTo(chargeCur) > 0) {
							counter++;
						}
					}
					break;
				default:
					counter++;
					break;
				}
            } else {
                counter = 0;
            }
            if (counter >= params.currentCount) {
                startIndex = i - (params.currentCount - 1);
                endIndex = i + 1;
                break;
            }
        }
        if (counter < params.currentCount) {
            logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
            return null;
        }
        
        // 向前找，packDataInfo是按时间倒叙的
        if (params.forwardCount > 0) {
        	counter = 0;
            for (int i = startIndex + params.currentCount; i < packDataInfos.size(); i++) {
                PackDataInfo packDataInfo = packDataInfos.get(i);
                if (!eventCondition(packDataInfo)) {
                    counter++;
                } else {
                    counter = 0;
                }
                if (counter >= params.forwardCount) {
                    endIndex = i + 1;
                    break;
                }
            }
            if (counter < params.forwardCount) {
                logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
                return null;
            }
		}
        
        // 向后找，packDataInfo是按时间倒叙的
        if (params.backwardCount < 0 || startIndex < params.backwardCount) {
        	logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
			return null;
		}
        if (params.backwardCount > 0) {
        	counter = 0;
            for (int i = startIndex - 1; i >=0 ; i--) {
            	PackDataInfo packDataInfo = packDataInfos.get(i);
            	if (!eventCondition(packDataInfo)) {
                    counter++;
                } else {
                    counter = 0;
                }
                if (counter >= params.backwardCount) {
                    startIndex = i;
                    break;
                }
    		}
		}
        if (counter < params.backwardCount) {
        	logger.info("不能找到满足{}的数据,编号:{}", type, gprsId);
			return null;
		}
        
        return new int[]{startIndex, endIndex};
    }
	
}
