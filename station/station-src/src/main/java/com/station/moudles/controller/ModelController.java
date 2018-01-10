package com.station.moudles.controller;

import com.station.common.Constant;
import com.station.moudles.entity.PulseCalculationSend;
import com.station.moudles.entity.StationInfo;
import com.station.moudles.mapper.PulseCalculationSendMapper;
import com.station.moudles.mapper.StationInfoMapper;
import com.station.moudles.service.ModelCalculationService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.ResponseStatus;
import io.swagger.annotations.ApiOperation;
import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang3.ArrayUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.StopWatch;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import java.util.Date;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

@Controller
@RequestMapping(value = "/model")
public class ModelController extends BaseController {

    private final ModelCalculationService modelCalculationService;
    private final PulseCalculationSendMapper pulseCalculationSendMapper;
    private final StationInfoMapper stationInfoMapper;
    private final ExecutorService fixedThreadPool = Executors.newSingleThreadExecutor();

    @Autowired
    public ModelController(ModelCalculationService modelCalculationService,
                           PulseCalculationSendMapper pulseCalculationSendMapper,
                           StationInfoMapper stationInfoMapper) {
        this.modelCalculationService = modelCalculationService;
        this.pulseCalculationSendMapper = pulseCalculationSendMapper;
        this.stationInfoMapper = stationInfoMapper;
    }

    /**
     * 对指定三级公司所有基站进行模型计算
     *
     * @return
     */
    @PostMapping(value = "/calculation/all/{company_id}")
    @ResponseBody
    public AjaxResponse allModelCalculate(@PathVariable("company_id") Integer companyId) {
        if (companyId == null) {
            return new AjaxResponse<>(Constant.RS_CODE_ERROR, "三级公司编号必须!");
        }
        StationInfo info = new StationInfo();
        info.setCompanyId3(companyId);
        List<StationInfo> stationInfos = stationInfoMapper.selectListSelective(info);
        if (CollectionUtils.isEmpty(stationInfos)) {
            return new AjaxResponse<>(Constant.RS_CODE_ERROR, "没有任何基站!");
        }
        logger.info("{}个基站需要进行模型计算", stationInfos.size());
        fixedThreadPool.execute(() -> {
            for (StationInfo stationInfo : stationInfos) {
            	if(stationInfo.getGprsId() == null || stationInfo.getGprsId().equals("-1"))//未绑定设备，不做计算。
            		continue;
                calculateHandler(stationInfo);
            }
        });
        return new AjaxResponse<>(Constant.RS_CODE_SUCCESS, "提交全部基站模型计算成功！");
    }

    /**
     * 批量模型计算,多个基站编号用英文逗号分割
     * e.g. /model/multiple_calculation/1,2,3,4
     *
     * @param stationIds
     * @return
     */
    @PostMapping(value = "/multiple_calculation/{stationIds}")
    @ResponseBody
    public AjaxResponse multipleModelCalculate(@PathVariable("stationIds") Integer[] stationIds) {
        if (ArrayUtils.isEmpty(stationIds)) {
            return new AjaxResponse<>(Constant.RS_CODE_ERROR, "请输入需要进行批量模型计算的基站编号");
        }
        fixedThreadPool.execute(() -> {
            for (Integer stationId : stationIds) {
                if (stationId == null) {
                    continue;
                }
                StationInfo stationInfo = stationInfoMapper.selectByPrimaryKey(stationId);
                if (stationInfo == null) {
                    logger.warn("无效的基站编号:{}", stationId);
                    continue;
                }
                calculateHandler(stationInfo);
            }
        });

        return new AjaxResponse<>(Constant.RS_CODE_SUCCESS, "提交批量模型计算成功！");
    }

    @RequestMapping(value = "/calculation/{stationId}", method = RequestMethod.POST)
    @ResponseBody
    @ApiOperation(value = "模型计算", notes = "返回计算状态")
    public AjaxResponse<List<ResponseStatus>> modelCalculate(@PathVariable Integer stationId) {
        if (stationId == null) {
            return new AjaxResponse<>(Constant.RS_CODE_ERROR, "请输入基站编号");
        }
        StationInfo stationInfo = stationInfoMapper.selectByPrimaryKey(stationId);
        if (null == stationInfo) {
            return new AjaxResponse<>(Constant.RS_CODE_ERROR, "请输入有效的基站编号");
        }
        AjaxResponse<List<ResponseStatus>> ajaxResponse = new AjaxResponse<>(Constant.RS_CODE_SUCCESS, "模型计算成功！");
        List<ResponseStatus> statuses = calculateHandler(stationInfo);
        ajaxResponse.setData(statuses);
        if (CollectionUtils.isEmpty(statuses)) {
            ajaxResponse.setCode(Constant.RS_CODE_ERROR);
            ajaxResponse.setMsg("内阻失败, 容量失败");
        } else {
            StringBuilder sb = new StringBuilder();
            for (ResponseStatus status : statuses) {
                sb.append(status.getMessage()).append(", ");
            }
            sb.setLength(sb.length() - 2);
            ajaxResponse.setMsg(sb.toString());
        }
        return ajaxResponse;
    }

    /**
     * 模型计算处理器
     *
     * @param stationInfo
     * @return
     */
    private List<ResponseStatus> calculateHandler(StationInfo stationInfo) {
        StopWatch stopWatch = new StopWatch();
        stopWatch.start();
        List<ResponseStatus> statuses = null;
        try {
            PulseCalculationSend pulseCalculationSend = new PulseCalculationSend();
            pulseCalculationSend.setSendTime(new Date());
            pulseCalculationSend.setGprsId(stationInfo.getGprsId());
            statuses = modelCalculationService.calculate(stationInfo.getId());
            StringBuilder sb = new StringBuilder();
            boolean resistanceStatus = false;
            boolean capacityStatus = false;
            for (ResponseStatus status : statuses) {
                sb.append(status.getMessage()).append(", ");
                if (StringUtils.contains(status.getMessage(), "内阻")) {
                    resistanceStatus = Integer.valueOf(1).equals(status.getStatus());
                    pulseCalculationSend.setResistanceStatus(status.getStatus());
                    pulseCalculationSend.setResistanceStatusMessage(StringUtils.isBlank(status.getExtMessage())
                            ? status.getMessage() : status.getExtMessage());
                } else if (StringUtils.contains(status.getMessage(), "容量")) {
                    capacityStatus = Integer.valueOf(1).equals(status.getStatus());
                    pulseCalculationSend.setCapacityStatus(status.getStatus());
                    pulseCalculationSend.setCapacityStatusMessage(StringUtils.isBlank(status.getExtMessage()) ? status
                            .getMessage() : status.getExtMessage());
                }
            }
            // 0:内阻容量都成功, 1:内阻成功容量失败, 2:内阻失败容量成功, 3:内阻容量都失败
            Integer sendDone = 3;
            if (resistanceStatus && capacityStatus) {
                sendDone = 0;
            } else if (resistanceStatus && !capacityStatus) {
                sendDone = 1;
            } else if (!resistanceStatus && capacityStatus) {
                sendDone = 2;
            }
            pulseCalculationSend.setSendDone(sendDone);
            sb.setLength(sb.length() - 2);
            pulseCalculationSend.setSendDoneMessage(sb.toString());
            pulseCalculationSend.setEndTime(new Date());
            pulseCalculationSendMapper.insert(pulseCalculationSend);
        } catch (Exception e) {
            logger.error("模型计算失败--->基站编号:{}", stationInfo.getId(), e);
        } finally {
            stopWatch.stop();
            logger.info("模型计算耗时:{},基站编号:{}", stopWatch, stationInfo.getId());
        }
        return statuses;
    }
}
