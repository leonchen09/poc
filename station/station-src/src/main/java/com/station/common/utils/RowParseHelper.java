package com.station.common.utils;

import java.text.DecimalFormat;

import org.apache.poi.ss.usermodel.Row;

public class RowParseHelper {
	/**
	 * 导入表格，解析当前行是否有数据
	 * 
	 * @param row
	 * @param columnTotal
	 * @return
	 */
	public static boolean hasData(Row row, int columnTotal) {
		if (row == null) {
			return false;
		}
		for (int i = 0; i < columnTotal; i++) {
			if (!StringUtils.isNull(row.getCell(i))) {
				return true;
			}
		}
		return false;
	}

	public static String getCell(Row row, int column) {
		if (row == null) {
			return null;
		}
		if (StringUtils.getString(row.getCell(column)) != null) {
			try {
				return new DecimalFormat("0").format(row.getCell(column).getNumericCellValue());
			} catch (Exception e) {
				return StringUtils.getString(row.getCell(column));
			}
		}
		return null;
	}
}
