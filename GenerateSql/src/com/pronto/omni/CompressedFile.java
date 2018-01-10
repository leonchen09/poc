package com.pronto.omni;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Enumeration;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;
import java.util.zip.ZipOutputStream;

public class CompressedFile {

	/**
	 * @param args
	 * @throws IOException 
	 */
	public static void main(String[] args) throws IOException {
		CompressedFile cf = new CompressedFile();
		cf.unpack("e:\\ProntoDir\\pdm\\doc1.docx", "e:\\ProntoDir\\pdm\\doc1");

	}
	public void zipFile(String fileName, String zipFileName) throws IOException {
		File inFile = new File(fileName);
		ZipOutputStream zos = new ZipOutputStream(new FileOutputStream(zipFileName));
		//zos.setComment("多文件处理");
		zipFile(inFile, zos, "");
		zos.close();
	}
	//压缩文件
	private void zipFile(File inFile, ZipOutputStream zos, String dir) throws IOException {
		if (inFile.isDirectory()) {
			File[] files = inFile.listFiles();
			for (File file:files)

				zipFile(file, zos, dir + "\\" + inFile.getName());
		} else {
			String entryName = null;
			if (!"".equals(dir))
				entryName = dir + "\\" + inFile.getName();
			else
				entryName = inFile.getName();
			ZipEntry entry = new ZipEntry(entryName);
			zos.putNextEntry(entry);
			InputStream is = new FileInputStream(inFile);
			int len = 0;
			while ((len = is.read()) != -1)
				zos.write(len);
			is.close();
		}

	}
	
	public void unpack(String zipFileName, String outputPath) throws IOException { 
		File file = new File(zipFileName);//压缩文件  
		ZipFile zipFile = new ZipFile(file);

		Enumeration<ZipEntry> entries = (Enumeration<ZipEntry>) zipFile.entries();  
		while (entries.hasMoreElements()) {  
			ZipEntry zipEntry = entries.nextElement();  
			String fileName = zipEntry.getName();  
			File temp = new File(outputPath + "\\" + fileName);  
			if (! temp.getParentFile().exists())  
				temp.getParentFile().mkdirs();  
				OutputStream os = new FileOutputStream(temp);  
				//通过ZipFile的getInputStream方法拿到具体的ZipEntry的输入流  
				InputStream is = zipFile.getInputStream(zipEntry);  
				int len = 0;  
				while ((len = is.read()) != -1)  
					os.write(len);  
					os.close();  
					is.close();  
				}  
			}  

}
