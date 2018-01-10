package com.pronto.jdbc;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.UUID;
/**
 * A stream support large size object to save memory. It's cache object can be as parameter to transform 
 * large size data between different method. It used local file to store data when the data exceed a fixed
 * size, so make sure all the method which call this object are run on same machine.
 * @author ProntoDoc
 *
 */
public class FileBufferedStream extends OutputStream {

	private static long MAX_MEMORY_DATA_LENGTH = 10 * 1024 * 1024; //10m
	private static int BUFFER_SIZE = 1024;
	
	private String tempFolder = System.getProperties().getProperty("java.io.tmpdir");
	// full file name to save the data.
	private String fileName;
	
	private long dataLength = 0;
	private boolean closed = false;
	
	// byte array to save the data
	private byte[] dataBytes;
	
	private OutputStream out;
	private FileOutputStream fout;
	//use to read data from byte array or file.
	private InputStream in;
	
	private boolean useFile = false;

	public FileBufferedStream() throws IOException {
		out = new ByteArrayOutputStream();//defaultly, we use memory to save the data.
	}

	public static void main(String[] argv){
		
	}
	
	/**
	 * init the outputStream , be a FileOutputStream or ByteArrayOutputStrem
	 * @return
	 * @throws FileNotFoundException
	 */
	private void initTempFile() throws FileNotFoundException{
		fileName = tempFolder + File.separator + UUID.randomUUID().toString() + ".pdcache";
		fout = new FileOutputStream(fileName);
	}

	/**
	 * get the inputStream that is FileInputStream or ByteArrayInputStream. Please make sure
	 * to close this stream after used.
	 * @return
	 * @throws IOException 
	 */
	public InputStream getInput() throws IOException {
		//close the outputstream first if it isn't closed.
		if(!closed)
			close();
		
		if (useFile) {
			in = new FileInputStream(this.fileName);
		} else {
			in = new ByteArrayInputStream(this.dataBytes);
		}
		return in;
	}

	/**
	 * get the tempory file name. it will be return null if there is no
	 * temporary file. 
	 * Call this method before 
	 * @return full path file name.
	 * @throws IOException
	 */
	public String getTempFileName() {
		if (useFile)
			return this.fileName;
		else
			return null;
		// throw new
		// IOException("Data stored in byte array, call getData() or getInput() method.");
	}

	/**
	 * get byte array which contain the data, it will be the file path if there is
	 * temporary file.
	 * 
	 * @return
	 * @throws IOException
	 */
	public byte[] getData() throws IOException {
		if (useFile) {			
			return null;
		}
		// throw new
		// IOException("Data stored in file, call getTempFileName() or getInput() method.");
		else
			return this.dataBytes;
	}

	@Override
	public void write(byte[] buffer) throws IOException {
		super.write(buffer);
	}
	/**
	 * Append string to stream, maybe to bytearray or file, depend on total data length.
	 * @param content
	 * @throws IOException
	 */
	public void Append(String content) throws IOException{
		byte[] bytes = content.getBytes();
		write(bytes, 0, bytes.length);
	}
	
	/**
	 * Write byte array to stream, it's feature same with common output stream.
	 */
	public void write(byte[] buffer, int offset, int count) throws IOException {
		//first check use file or not
		if(!useFile){
			if((dataLength + count) > MAX_MEMORY_DATA_LENGTH){
				useFile = true;
				initTempFile();
				this.dataBytes = ((ByteArrayOutputStream) out).toByteArray();
				try {
					out.close();
				} catch (IOException e) {}
				InputStream in = new ByteArrayInputStream(this.dataBytes);
				byte[] data = new byte[BUFFER_SIZE];  
				while(in.read(data,0,BUFFER_SIZE) != -1)  
					fout.write(data, 0, count);  
				in.close();
			}	
		}

		if (useFile) {
 			fout.write(buffer, offset, count);
		} else {
			out.write(buffer, offset, count);
		}
		dataLength += count;
	}
	@Override
	public void write(int b) throws IOException {
		if (useFile) {
 			fout.write(b);
		} else {
			out.write(b);
		}
		dataLength += 4;//int used 4 byte
	}
	/**
	 * cloese the outputSream
	 */
	public void close() throws IOException {
		if (useFile) {
			fout.flush();
			fout.close();
			if(fout !=null){
				fout.close();
			}
		} else {
			this.dataBytes = ((ByteArrayOutputStream) out).toByteArray();
			out.close();
		}
		closed = true;
	}
	/**
	 * close all stream, include input stream, output stream. And delete temporary file.
	 * This method must be call after use this object.
	 */
	public void Dispose(){
		try {
			//close input stream
			if(in != null){
				in.close();
			}
			//make sure close output stream.
			if(!closed)
				close();
		} catch (IOException e) {		}
		//clear temporary file
		File file = new File(fileName);
		if(file.exists())
			file.delete();
	}
	
	@Override
	protected void finalize() throws Throwable{
		super.finalize();
		Dispose();
	}
}
