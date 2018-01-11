package com.station.moudles.controller;

import java.util.List;
import java.util.regex.Pattern;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.entity.GprsDeviceType;
import com.station.moudles.service.GprsDeviceTypeService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchGprsDeviceTypePagingVo;

import io.swagger.annotations.ApiOperation;

@Controller
@RequestMapping(value = "/gprsDeviceType")
public class GprsDeviceTypeController extends BaseController {
	
	@Autowired
	GprsDeviceTypeService gprsDeviceTypeSer;
	
	@RequestMapping(value="/listPage",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="根据参数显示设备类型",notes="返回设备类型列表")
	public AjaxResponse<ShowPage<GprsDeviceType>> getGprsDeviceTypePaging(@RequestBody SearchGprsDeviceTypePagingVo searchGprsDeviceTypePagingVo){
		List<GprsDeviceType> cellVolLevelList = gprsDeviceTypeSer.selectListSelectivePaging(searchGprsDeviceTypePagingVo);
		ShowPage<GprsDeviceType> page = new ShowPage<GprsDeviceType>(searchGprsDeviceTypePagingVo, cellVolLevelList);
		AjaxResponse<ShowPage<GprsDeviceType>> ajaxResponse = new AjaxResponse<ShowPage<GprsDeviceType>>(page);
		return ajaxResponse;
	}
	
	@RequestMapping(value="/list",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="根据参数显示设备类型",notes="返回设备类型列表")
	public AjaxResponse<List<GprsDeviceType>> getGprsDeviceTypeList(@RequestBody GprsDeviceType gprsDeviceType){
		if(gprsDeviceType == null) {
			return new  AjaxResponse<List<GprsDeviceType>>(Constant.RS_CODE_ERROR,"查询失败");
		}
		AjaxResponse<List<GprsDeviceType>> ajaxResponse = new AjaxResponse<List<GprsDeviceType>>(Constant.RS_CODE_SUCCESS,"查询成功");
		List<GprsDeviceType> cellVolLevelList = gprsDeviceTypeSer.selectListSelective(gprsDeviceType);
		ajaxResponse.setData(cellVolLevelList);
		return ajaxResponse;
	}
	@RequestMapping(value = "/save",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value = "根据参数新增设备类型",notes = "返回是否新增结果")
	public AjaxResponse<Object> save(@RequestBody GprsDeviceType gprsDeviceType){
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_SUCCESS,"新增成功！");
		if(gprsDeviceType == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR,"设置毕业的参数！");
		}
		if(gprsDeviceType.getCreateId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "创建人员没填写！");
		}
		
//		if(gprsDeviceType.getTypeCode() != null) {
//			GprsDeviceType queryCode = new GprsDeviceType();
//			queryCode.setTypeCode(gprsDeviceType.getTypeCode());
//			List<GprsDeviceType> typeCode = gprsDeviceTypeSer.selectListSelective(queryCode);
//			if(typeCode.size() != 0) {
//				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
//				ajaxResponse.setMsg("设备类型编码重复！");
//				return ajaxResponse;
//			}
//			Pattern regex = Pattern.compile("^[0-9]{1,10}$");
//			boolean code = regex.matcher(gprsDeviceType.getTypeName().toString()).matches();
//			if(!code) {
//				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
//				ajaxResponse.setMsg("设备类型编码必须是1-10 位有效数字！");
//				return ajaxResponse;
//			}
//		}else {
//			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR,"请设置设备类的编码！");
//		}
		
		if(gprsDeviceType.getTypeName() != null) {
			//判断设备类型名称是否重复
			GprsDeviceType queryName = new GprsDeviceType();
			queryName.setTypeName(gprsDeviceType.getTypeName());
			List<GprsDeviceType> typeName = gprsDeviceTypeSer.selectListSelective(queryName);
			if(typeName.size() != 0) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("设备类型名称重复！");
				return ajaxResponse;
			}
			//设置typeCode 加1
			GprsDeviceType queryCode = new GprsDeviceType();
			List<GprsDeviceType> typeCode = gprsDeviceTypeSer.selectListSelective(queryCode);
			if(typeCode.size() != 0) {
				gprsDeviceType.setTypeCode(typeCode.get(typeCode.size()-1).getTypeCode()+1);
			}else {
				gprsDeviceType.setTypeCode(1);
			}
			Pattern regex = Pattern.compile("^[\\u4e00-\\u9fa5_a-zA-Z0-9]{1,16}$");
			boolean name = regex.matcher(gprsDeviceType.getTypeName().toString()).matches();
			if(!name) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("设备类型填写不符合规范，是以汉字、英文字母、数字或者下划线组成！");
				return ajaxResponse;
			}
		}else {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR,"请设置设备类型！");
		}
			gprsDeviceTypeSer.insertSelective(gprsDeviceType);
		return ajaxResponse;
		
	}
	
	
	@RequestMapping(value = "/update",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="需要修改的参数",notes = "返回是否修改成功")
	public AjaxResponse<Object> update(@RequestBody GprsDeviceType gprsDeviceType){
		if(gprsDeviceType == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR,"请填写需要修改的信息！");
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_SUCCESS,"修改成功");
		
		if(gprsDeviceType.getId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR,"请设置PK！");
		}
		
		if(gprsDeviceType.getTypeName() != null) {	
			Pattern regex = Pattern.compile("^[\\u4e00-\\u9fa5_a-zA-Z0-9]{1,16}$");
			boolean vol = regex.matcher(gprsDeviceType.getTypeName().toString()).matches();
			if(!vol) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("设备类型填写不符合规范，是以汉字、英文字母、数字或者下划线组成！");
				return ajaxResponse;
			}
			// 判断设备类型名称是否重复
			GprsDeviceType queryName = new GprsDeviceType();
			queryName.setTypeName(gprsDeviceType.getTypeName());
			GprsDeviceType deviceType = gprsDeviceTypeSer.selectByPrimaryKey(gprsDeviceType.getId());

			if (!deviceType.getTypeName().equals(gprsDeviceType.getTypeName())) {
				List<GprsDeviceType> typeName = gprsDeviceTypeSer.selectListSelective(queryName);
				if (typeName.size() != 0) {
					ajaxResponse.setCode(Constant.RS_CODE_ERROR);
					ajaxResponse.setMsg("设备类型名称重复！");
					return ajaxResponse;
				}
			}
			gprsDeviceTypeSer.updateByPrimaryKeySelective(gprsDeviceType);
		}

		return ajaxResponse;
		
	}
	
}
