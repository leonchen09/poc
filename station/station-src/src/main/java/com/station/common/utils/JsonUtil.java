package com.station.common.utils;

import java.io.IOException;
import java.lang.reflect.Field;
import java.text.SimpleDateFormat;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.fasterxml.jackson.annotation.JsonInclude.Include;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.JavaType;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.station.moudles.vo.CompareMapVo;
import com.station.moudles.vo.CompareVo;

public class JsonUtil {
	private static final Logger log = LoggerFactory.getLogger(JsonUtil.class);
	private static ObjectMapper jsonMapper = null;
	static {
		if (jsonMapper == null) {
			jsonMapper = new ObjectMapper();
			SimpleDateFormat myDateFormat = new SimpleDateFormat("yyyy-MM-dd hh:mm:ss");
			jsonMapper.setDateFormat(myDateFormat);
			jsonMapper.setSerializationInclusion(Include.NON_NULL);
		}
	}

	private JsonUtil() {
	}

	public static String writeValueAsString(Object o) {
		String result = "";
		try {
			result = jsonMapper.writeValueAsString(o);
		} catch (JsonProcessingException e) {
			e.printStackTrace();
			result = e.getMessage();
		}
		return result;
	}

	/**
	 * 获取泛型的Collection Type
	 * 
	 * @param jsonStr
	 *            json字符串
	 * @param collectionClass
	 *            泛型的Collection
	 * @param elementClasses
	 *            元素类型
	 */
	public static <T> T readJson(String jsonStr, Class<?> collectionClass, Class<?>... elementClasses) {
		JavaType javaType = jsonMapper.getTypeFactory().constructParametricType(collectionClass, elementClasses);
		try {
			return jsonMapper.readValue(jsonStr, javaType);
		} catch (IOException e) {
			e.printStackTrace();
			return null;
		}
	}

	public static <T> T readJson(String jsonStr, Class<T> javaType) {
		try {
			return jsonMapper.readValue(jsonStr, javaType);
		} catch (Exception e) {
			// e.printStackTrace();
			return null;
		}
	}

	/**
	 * 通过json比较2个对象，返回一个比较后的对象
	 * 
	 * @param beforeT
	 * @param afterT
	 * @return
	 */
	public static <T> CompareVo compareBean(T beforeT, T afterT) {
		reflectReplaceColumn(beforeT, beforeT.getClass(), ",", "|||");
		reflectReplaceColumn(afterT, afterT.getClass(), ",", "|||");
		String beforeStr = writeValueAsString(beforeT);
		String afterStr = writeValueAsString(afterT);
		reflectReplaceColumn(beforeT, beforeT.getClass(), "\\|\\|\\|", ",");
		reflectReplaceColumn(afterT, afterT.getClass(), "\\|\\|\\|", ",");
		beforeStr = beforeStr.replace("{", "");
		beforeStr = beforeStr.replace("}", "");
		beforeStr = beforeStr.replaceAll("null", "\"\"");
		afterStr = afterStr.replace("{", "");
		afterStr = afterStr.replace("}", "");
		afterStr = afterStr.replaceAll("null", "\"\"");
		String beforeTemp[] = beforeStr.split(",");
		String afterTemp[] = afterStr.split(",");
		String before = "", after = "", same = "";
		for (int i = 0; i < beforeTemp.length; i++) {
			if (compareString(beforeTemp[i], afterTemp[i])) {
				if (beforeTemp[i].indexOf("null") == -1 && beforeTemp[i].indexOf("\"\"") == -1) {
					same = same + beforeTemp[i] + ",";
				}
			} else {
				before = before + beforeTemp[i] + ",";
				after = after + afterTemp[i] + ",";
			}
		}
		before = format(before);
		after = format(after);
		same = format(same);
		return new CompareVo(before, after, same, null, null);
	}

	protected static boolean compareString(String before, String after) {
		if ((before == null || before.equals("")) && (after == null || after.equals(""))) {
			return true;
		} else {
			return before.equals(after);
		}
	}

	protected static String format(String str) {
		if (str.length() == 0) {
			return "";
		}
		// str = str.replaceAll("\"", "");
		str = str.replaceAll("\\|\\|\\|", ",");
		str = str.substring(0, str.length() - 1);
		return str;
	}

	public static void main(String[] args) {
	}

	public static void reflectReplaceColumn(Object o, Class cls, String oldStr, String replaceStr) {
		if (cls.getSuperclass() != Object.class) {
			reflectReplaceColumn(o, cls.getSuperclass(), oldStr, replaceStr);
		} else {
			Field[] fs = o.getClass().getDeclaredFields();
			for (Field f : fs) {
				f.setAccessible(true);
				try {
					if (f.getType().getSimpleName().equals("String")) {
						String old = (String) f.get(o);
						if (old != null) {
							old = old.replaceAll(oldStr, replaceStr);
							f.set(o, old);
						}
					}
				} catch (IllegalArgumentException e) {
					e.printStackTrace();
				} catch (IllegalAccessException e) {
					e.printStackTrace();
				}
			}
		}
	}

	public static <T> CompareMapVo compareBeanMap(T beforeT, T afterT) {
		Map sameMap = new HashMap<>();
		Map beforeMap = new HashMap<>();
		Map afterMap = new HashMap<>();
		try {
			List<Field> fieldList = ReflectUtil.getAllFields(beforeT.getClass());
			for (Field f : fieldList) {
				f.setAccessible(true);
				if (afterT == null) {
					beforeMap.put(f.getName(), f.get(beforeT));
				} else if (beforeT == null) {
					afterMap.put(f.getName(), f.get(afterT));
				} else if (compareObject(f.get(beforeT), f.get(afterT))) {
					sameMap.put(f.getName(), f.get(beforeT));
				} else {
					beforeMap.put(f.getName(), f.get(beforeT));
					afterMap.put(f.getName(), f.get(afterT));
				}
			}
		} catch (Exception e) {
			log.error("", e);
			return new CompareMapVo();
		}
		return new CompareMapVo(beforeMap, afterMap, sameMap);
	}

	public static boolean compareObject(Object beforeObj, Object afterObj) {
		if (beforeObj != null && afterObj != null) {
			return beforeObj.equals(afterObj);
		} else if (beforeObj == null && afterObj == null) {
			return true;
		} else {
			return false;
		}
	}
}
