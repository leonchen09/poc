package com.station.moudles.controller;

import java.util.List;
import java.util.regex.Pattern;

import org.apache.poi.util.StringUtil;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.ResponseBody;

import com.station.common.Constant;
import com.station.moudles.entity.CellVolLevel;
import com.station.moudles.service.CellVolLevelService;
import com.station.moudles.vo.AjaxResponse;
import com.station.moudles.vo.ShowPage;
import com.station.moudles.vo.search.SearchCellVolLevelPagingVo;
import io.swagger.annotations.ApiOperation;

/**
 * 电压平台controller
 * @author ywg
 */
@Controller
@RequestMapping(value="/cellVolLevel")
public class CellVolLevelController extends BaseController{
	
	@Autowired
	CellVolLevelService cellVolLevelSer;
	
	@RequestMapping(value="/listPage",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="根据参数显示电压平台列表",notes="返回电压平台列表")
	public AjaxResponse<ShowPage<CellVolLevel>> getCellVolLevePaging(@RequestBody SearchCellVolLevelPagingVo searchCellVolLevelPagingVo){
		List<CellVolLevel> cellVolLevelList = cellVolLevelSer.selectListSelectivePaging(searchCellVolLevelPagingVo);
		ShowPage<CellVolLevel> page = new ShowPage<CellVolLevel>(searchCellVolLevelPagingVo, cellVolLevelList);
		AjaxResponse<ShowPage<CellVolLevel>> ajaxResponse = new AjaxResponse<ShowPage<CellVolLevel>>(page);
		return ajaxResponse;
	}
	
	@RequestMapping(value="/list",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="根据参数显示电压平台列表",notes="返回电压平台列表")
	public AjaxResponse<List<CellVolLevel>> getCellVolLeveList(@RequestBody CellVolLevel cellVolLevel){
		if(cellVolLevel == null) {
			return new  AjaxResponse<List<CellVolLevel>>(Constant.RS_CODE_ERROR,"查询失败");
		}
		AjaxResponse<List<CellVolLevel>> ajaxResponse = new AjaxResponse<List<CellVolLevel>>(Constant.RS_CODE_SUCCESS,"查询成功");
		List<CellVolLevel> cellVolLevelList = cellVolLevelSer.selectListSelective(cellVolLevel);	
		ajaxResponse.setData(cellVolLevelList);		
		return ajaxResponse;
	}
	
	@RequestMapping(value="/save",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="根据参数添加电压平台列表",notes="返回是否添加成功")
	public AjaxResponse<Object> save(@RequestBody CellVolLevel cellVolLevel){
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_SUCCESS,"新增成功");
		if(cellVolLevel == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请填写毕业的参数！");
		}
		
		if(cellVolLevel.getCreateId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "创建人员没填写！");
		}

		if(cellVolLevel.getVolLevelName() != null) {
			Pattern regex = Pattern.compile("^([1-9]{1}\\d{0,2})[V]$");
			boolean vol = regex.matcher(cellVolLevel.getVolLevelName().toString()).matches();
			if (vol) {
				//判断电压平台名称是否重复
				CellVolLevel queryName = new CellVolLevel();
				queryName.setVolLevelName(cellVolLevel.getVolLevelName());
				List<CellVolLevel> volName = cellVolLevelSer.selectListSelective(queryName);
				if(volName.size() != 0) {
					ajaxResponse.setCode(Constant.RS_CODE_ERROR);
					ajaxResponse.setMsg("单体平台电压不能重复！");
					return ajaxResponse;
				}
				// 将编码是名称去掉V
				String levelName = cellVolLevel.getVolLevelName().toUpperCase();
				String code = levelName.substring(0, levelName.indexOf("V"));
				cellVolLevel.setVolLevelCode(Integer.parseInt(code));
				cellVolLevelSer.insertSelective(cellVolLevel);
			} else {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("填写错误，填写电压必须是1-999的整数，单位V！");
				return ajaxResponse;
			}
		}else {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "单体电压名称没有填写！");
		}
		return ajaxResponse;
	}
	
	@RequestMapping(value = "/update",method = RequestMethod.POST)
	@ResponseBody
	@ApiOperation(value="根据参数修改电压平台列表",notes="返回是否修改成功")
	public AjaxResponse<Object> update(@RequestBody CellVolLevel cellVolLevel){
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_SUCCESS,"修改成功");
		if(cellVolLevel == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请传入必要的参数！");
		}
		
		if(cellVolLevel.getId() == null) {
			return new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "请设置PK！");
		}
		
		if(cellVolLevel.getVolLevelName() != null) {
			Pattern regex = Pattern.compile("^([1-9]{1}\\d{0,2})[V]$");
			boolean vol = regex.matcher(cellVolLevel.getVolLevelName().toString()).matches();
			if(!vol) {
				ajaxResponse.setCode(Constant.RS_CODE_ERROR);
				ajaxResponse.setMsg("填写错误，填写电压必须是1-999的整数，单位V！");
				return ajaxResponse;
			}
			// 判断电压平台名称是否重复
			CellVolLevel queryName = new CellVolLevel();
			queryName.setVolLevelName(cellVolLevel.getVolLevelName());
			CellVolLevel volLevel = cellVolLevelSer.selectByPrimaryKey(cellVolLevel.getId());
			if (!volLevel.getVolLevelName().equals(cellVolLevel.getVolLevelName())) {
				List<CellVolLevel> cellVolLevels = cellVolLevelSer.selectListSelective(queryName);
				if (cellVolLevels.size() != 0) {
					ajaxResponse.setCode(Constant.RS_CODE_ERROR);
					ajaxResponse.setMsg("名称不能重复！");
					return ajaxResponse;
				}
			}
			cellVolLevelSer.updateByPrimaryKeySelective(cellVolLevel);
		}
		
		return ajaxResponse;
		
	}
	
}
