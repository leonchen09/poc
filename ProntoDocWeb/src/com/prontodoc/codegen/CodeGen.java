package com.prontodoc.codegen;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.poi.xssf.usermodel.XSSFCell;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;

public class CodeGen {
	
	private static final String rootFolder = "D:\\Source\\Pronto\\Java\\src\\";
	
	
	private static List<String> packages = new ArrayList<String>();
	private static List<String> classes = new ArrayList<String>();

	/**
	 * @param args
	 * @throws IOException 
	 */
	public static void main(String[] args) throws Exception {
		String excelFile = "E:\\01Pronto\\SX\\folderstructure_java.xlsx";
		readExcel(excelFile);
		for(int i = 0; i < packages.size(); i ++){
			System.out.println(packages.get(i) + "\t" + classes.get(i));
			createPackage(packages.get(i));
			createClass(packages.get(i), classes.get(i));
		}
//		String packageName = "com.stepfunction.prontodoc.sx.sample";
//		String className = "TestClass";
//		createPackage(packageName);
//		createClass(packageName, className);

	}
	
	private static void createPackage(String packageName){
		String path = packageName.replace('.', '\\');
		File file = new File(rootFolder + path);
		if(!file.isDirectory()){
			file.mkdirs();
		}
	}
	
	private static void createClass(String packageName, String className) throws IOException{
		String path = packageName.replace('.', '\\');
		File file = new File(rootFolder + path+"\\"+className+".java");
		if(!file.exists()){
			file.createNewFile();
		}
		FileOutputStream out = new FileOutputStream(file);
		out.write(("/*\r\n").getBytes());
		out.write((" * @" + className + ".java\t0.1\t2011/06/14\r\n").getBytes());
		out.write((" * \r\n").getBytes());
		out.write((" * Copyright (c) 2011, StepFunction. All rights reserved.\r\n").getBytes());
		out.write((" */\r\n").getBytes());
		out.write(("package "+packageName+";\r\n\r\n").getBytes());
		out.write(("public class "+ className + " {\r\n\r\n").getBytes());
		out.write(("  /**\r\n").getBytes());
		out.write(("   * test method\r\n").getBytes());
		out.write(("   */\r\n").getBytes());
		out.write(("  public static void main(String[] args) {\r\n").getBytes());
		out.write(("    // TODO Auto-generated method stub\r\n").getBytes());
		out.write(("  }\r\n\r\n").getBytes());
		out.write(("}").getBytes());
		out.close();
	}
	
	private static void readExcel(String excelFileName) throws Exception{
		XSSFWorkbook workbook = new XSSFWorkbook(new FileInputStream(excelFileName));
		XSSFSheet sheet = workbook.getSheet("package_update");
		int rowIdx = 1;
		String packageName = null;
		String className = null;
		while(rowIdx <= sheet.getLastRowNum()){
			XSSFRow row = sheet.getRow(rowIdx);
			XSSFCell cell = row.getCell(1);
			if(cell != null && cell.getStringCellValue() != null && !cell.getStringCellValue().equals("")){
				packageName = cell.getStringCellValue();
			}else if(row.getCell(2) != null && row.getCell(2).getStringCellValue().length() > 0){
				className = row.getCell(2).getStringCellValue();
				packages.add(packageName);
				classes.add(className);
			}
			rowIdx ++;
		}

	}
}
