package com.sofn.framework.base.entity;

import java.io.Serializable;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class Searcher<T> implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 7024687147516775752L;
	
	private Map<String, Object> parameters; //查询参数
	//private Map<String, List<Object>> arrayParameters; //数组类型的参数
	private List<T> result;		//查询结果集
	
	public Searcher(){
		init();
	}
	
	public Searcher(Map<String, Object> parameters){
		this.parameters = parameters;
	}
	
	public Searcher(List<T> result){
		this.result = result;
	}
	
	private void init(){
		this.parameters = new HashMap<String, Object>();
//		this.arrayParameters = new HashMap<String, List<Object>>();
	}
	
	//获得参数值
	public Object getParameterValue(String parameterName){
		return this.parameters.get(parameterName);
	}
	
	/**
	 * 修改参数值。用在controller中进行模糊查询
	 * @param parameterName
	 * @param newValue
	 * @return
	 */
	public Object updateParameterValue(String parameterName, Object newValue){
		return this.parameters.replace(parameterName, newValue);
	}
	
	/**
	 * 增加参数
	 * @param parameterName
	 * @param value
	 */
	public void addParameter(String parameterName, Object value){
		this.parameters.put(parameterName, value);
	}
	/**
	 * 增加多个参数
	 * @param params
	 */
	public void addParameters(Map<String, Object> params){
		this.parameters.putAll(params);
	}
	
//	public void addArrayParameter(String parameterName, List<Object> values){
//		this.arrayParameters.put(parameterName, values);
//	}

	/**
	 * 清除以前的参数，设置新的参数map
	 * @param parameters
	 */
	public void setParameters(Map<String, Object> parameters) {
		this.parameters = parameters;
	}
	
	/**
	 * ��������������Ὣԭ�е����������ա�
	 * @param arrayParameters
	 */
//	public void setArrayParameters(Map<String, List<Object>> arrayParameters) {
//		this.arrayParameters = arrayParameters;
//	}

	public Map<String, Object> getParameters() {
		return parameters;
	}
	
//	public Map<String, List<Object>> getArrayParameters() {
//		return arrayParameters;
//	}
	
	public List<T> getResult() {
		return result;
	}

	public void setResult(List<T> result) {
		this.result = result;
	}
}
