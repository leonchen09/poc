package com.prontodoc.codegen;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.poi.xssf.usermodel.XSSFCell;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;

public class ParamGen {
	private static final String rootFolder = "D:\\Source\\Pronto\\Java\\src\\com\\stepfunction\\prontodoc\\sx\\param";

	/**
	 * @param args
	 * @throws IOException
	 */
	public static void main(String[] args) throws Exception {
		String excelFile = "E:\\01Pronto\\SX\\API summary_update.xlsx";
		readExcel(excelFile);

	}

	private static void createPackage(String packageName) {
		String path = packageName.replace('.', '\\');
		File file = new File(rootFolder + path);
		if (!file.isDirectory()) {
			file.mkdirs();
		}
	}

	private static FileOutputStream createClass(String className)
			throws IOException {
		File file = new File(rootFolder + "\\" + className + ".java");
		if (!file.exists()) {
			file.createNewFile();
		}
		FileOutputStream out = new FileOutputStream(file);
		out.write(("/*\r\n").getBytes());
		out.write((" * @" + className + ".java\t0.1\t2011/06/22\r\n")
				.getBytes());
		out.write((" * \r\n").getBytes());
		out.write((" * Copyright (c) 2011, StepFunction. All rights reserved.\r\n")
				.getBytes());
		out.write((" */\r\n").getBytes());
		out.write(("package com.stepfunction.prontodoc.sx.param;\r\n\r\n")
				.getBytes());
		out.write(("public class " + className + " {\r\n\r\n").getBytes());
		out.write(("  /**\r\n").getBytes());
		out.write(("   * Only this Constructor allow.\r\n").getBytes());
		out.write(("   */\r\n").getBytes());
		// out.write(("  public " + className + "(").getBytes());
		return out;
	}

	private static FileOutputStream createAPI(String api, String param)
			throws IOException {
		File file = new File("e:\\ProntoDir\\api\\" + api.split(" ")[2]
				+ ".txt");
		if (!file.exists()) {
			file.createNewFile();
		}
		FileOutputStream out = new FileOutputStream(file);
		out.write(("public class "+param.split(" ")[0] + " {\r\n").getBytes());
		out.write(("//API declare.\r\n").getBytes());
		out.write((api + "(" + param + ");\r\n").getBytes());
		return out;
	}

	private static void writeClassEnd(FileOutputStream out, FileOutputStream api) throws IOException {
		out.write(("\r\n}").getBytes());
		out.close();
		api.write(("\r\n}").getBytes());
		api.close();
	}

	private static void readExcel(String excelFileName) throws Exception {
		XSSFWorkbook workbook = new XSSFWorkbook(new FileInputStream(
				excelFileName));
		XSSFSheet sheet = workbook.getSheet("API summary_Java");
		for (int col = 1; col < 18; col++) {
			int rowIdx = 1;
			XSSFRow row = sheet.getRow(rowIdx);
			XSSFCell cell = row.getCell(col);
			FileOutputStream api = createAPI(cell.getStringCellValue(), sheet
					.getRow(2).getCell(col).getStringCellValue());

			rowIdx = 4;
			row = sheet.getRow(rowIdx);
			cell = row.getCell(col);
			FileOutputStream write = null;
			if (cell != null && cell.getStringCellValue().length() > 0) {
				String val = cell.getStringCellValue();
				write = createClass(val);
				rowIdx++;
				row = sheet.getRow(rowIdx);
				cell = row.getCell(col);
				String params = "  public " + val + "(";
				while (cell != null && cell.getStringCellValue() != null
						&& cell.getStringCellValue().length() > 0) {
					params += cell.getStringCellValue() + ", ";
					rowIdx++;
					row = sheet.getRow(rowIdx);
					cell = row.getCell(col);
				}
				if (params.length() > 1)
					params = params.substring(0, params.length() - 2);
				api.write(("//Parameter object constructor.\r\n").getBytes());
				api.write(params.getBytes());
				api.write((");\r\n//Parameter object class member.\r\n").getBytes());
				write.write(params.getBytes());
				write.write((") {\r\n  }\r\n").getBytes());
				write.write("/**\r\n".getBytes());
				write.write(" * class member. \r\n".getBytes());
				write.write(" */\r\n\r\n".getBytes());

				List<String> members = new ArrayList<String>();
				rowIdx = 12;
				row = sheet.getRow(rowIdx);
				cell = row.getCell(col);
				val = cell.getStringCellValue();
				while (val != null && val.length() > 0) {
					members.add(val);
					write.write(("  private " + val + "; \r\n").getBytes());
					api.write(("  " + val + "; \r\n").getBytes());
					rowIdx++;
					row = sheet.getRow(rowIdx);
					if (row != null && row.getCell(col) != null) {
						val = row.getCell(col).getStringCellValue();
					} else {
						val = null;
					}
				}
				write.write(("\r\n\r\n").getBytes());
				api.write(("//Parameter object setting method.\r\n").getBytes());
				writeSetGet(members, write, api);
				writeClassEnd(write, api);
			}
		}
	}

	private static void writeSetGet(List<String> members, FileOutputStream out,
			FileOutputStream api) throws IOException {
		for (int i = 0; i < members.size(); i++) {
			String[] temp = members.get(i).split(" ");
			String type = temp[0];
			String name = temp[1].replace(";", "");
			api.write(("  public void set" + name + "(" + type + " " + name + ");\r\n")
					.getBytes());
			out.write(("  public void set" + name + "(" + type + " " + name + "){\r\n")
					.getBytes());
			out.write(("    this." + name + " = " + name + "; \r\n").getBytes());
			out.write(("  }\r\n\r\n").getBytes());
			out.write(("  public " + type + " get" + name + "(){\r\n")
					.getBytes());
			out.write(("    return " + name + "; \r\n").getBytes());
			out.write(("  }\r\n\r\n").getBytes());
		}
	}
}
