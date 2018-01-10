package com.station.common.utils;

import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class ReflectUtil {
	private static final Logger log = LoggerFactory.getLogger(ReflectUtil.class);

	/**
	 * 不判断继承的超类
	 * 
	 * @param o
	 * @return
	 */
	@SuppressWarnings("rawtypes")
	public static boolean checkBeanNullWithoutSup(Object o, Class cls) {
		Field[] fs = cls.getDeclaredFields();
		for (Field f : fs) {
			f.setAccessible(true);
			try {
				Object value = f.get(o);
				if (value != null) {
					return false;
				}
			} catch (IllegalArgumentException e) {
				e.printStackTrace();
			} catch (IllegalAccessException e) {
				e.printStackTrace();
			}
		}
		return true;
	}

	@SuppressWarnings("rawtypes")
	public static boolean checkBeanNullWithSup(Object o, Class cls) {
		if (cls.getSuperclass() != Object.class) {
			if (!checkBeanNullWithoutSup(o, cls)) {
				return false;
			}
			return checkBeanNullWithSup(o, cls.getSuperclass());
		} else {
			return checkBeanNullWithoutSup(o, cls);
		}
	}

	public static List<Object> getValueByStartsWith(Object obj, String prefix) {
		if(obj == null)
			return null;
		Field[] fs = obj.getClass().getDeclaredFields();
		List<Object> resultList = new ArrayList<Object>();
		for (Field f : fs) {
			f.setAccessible(true);
			if (f.getName().startsWith(prefix)) {
				try {
					resultList.add(f.get(obj));
				} catch (IllegalArgumentException | IllegalAccessException e) {
					e.printStackTrace();
					resultList.add(null);
				}
			}
		}
		return resultList;
	}
	
	public static void setValueByKet(Object obj, String key,Object value){
		if(obj == null)
			return;
		Field[] fs = obj.getClass().getDeclaredFields();
		for (Field f : fs) {
			f.setAccessible(true);
			if (f.getName().equals(key)) {
				try {
					f.set(obj, value);
				} catch (IllegalArgumentException | IllegalAccessException e) {
					e.printStackTrace();					
				}
			}
		}
	}

	public static Object getValueByKey(Object obj, String key) {
		if(obj == null)
			return null;
		Field[] fs = obj.getClass().getDeclaredFields();
		for (Field f : fs) {
			f.setAccessible(true);
			if (f.getName().equals(key)) {
				try {
					return f.get(obj);
				} catch (IllegalArgumentException | IllegalAccessException e) {
					e.printStackTrace();
					return null;
				}
			}
		}
		return null;
	}

	/**
	 * 将newObj的null属性替换为oldObj的相同属性名属性
	 * 
	 * @param oldObj
	 * @param newObj
	 */
	public static void replaceNullValue(Object oldObj, Object newObj) {
		if (!oldObj.getClass().equals(newObj.getClass())) {
			log.error("类不相同，不能比较！");
			return;
		}
		Field[] fs = newObj.getClass().getDeclaredFields();
		for (Field f : fs) {
			f.setAccessible(true);
			try {
				if (f.get(newObj) == null) {
					f.set(newObj, f.get(oldObj));
				}
			} catch (IllegalArgumentException e) {
				e.printStackTrace();
			} catch (IllegalAccessException e) {
				e.printStackTrace();
			}
		}
	}

	/**
	 * 获取class的所有属性，包含其父类的属性
	 * @param cls
	 * @return
	 */
	public static <T> List<Field> getAllFields(Class cls) {
		List<Field> fieldList = new ArrayList<Field>();
		if (cls.getSuperclass() != Object.class) {
			fieldList.addAll(getAllFields(cls.getSuperclass()));
		}
		Field[] fs = cls.getDeclaredFields();
		fieldList.addAll(Arrays.asList(fs));
		return fieldList;
	}

	public static void main(String[] args) {
	}
}
