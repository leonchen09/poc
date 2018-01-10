package com.stepfunction.qrcode;

import java.awt.image.BufferedImage;
import java.awt.image.RenderedImage;
import java.io.BufferedInputStream;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.Writer;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.Hashtable;

import javax.imageio.ImageIO;
import javax.imageio.stream.ImageOutputStream;

import oracle.sql.CLOB;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.EncodeHintType;
import com.google.zxing.MultiFormatWriter;
import com.google.zxing.WriterException;
import com.google.zxing.common.BitMatrix;

public class QrCodeGenerator {

	private static final int BLACK = 0xff000000;
	private static final int WHITE = 0xFFFFFFFF;
	private final static String CODEC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
	
	/**
	 * @param args
	 * @throws WriterException 
	 * @throws IOException 
	 */
	public static void main(String[] args) throws WriterException, IOException {
		QrCodeGenerator qt = new QrCodeGenerator();
		RenderedImage image = qt.qrCode("abc@email.com",  190);
		ImageIO.write(image, "png", new File("e:\\1.jpg"));
		
		String result = qrCode2Base64("this is sample;\r\n中文,http://",190);
		FileOutputStream out = new FileOutputStream("e:\\11.txt");
		out.write(result.getBytes());
		out.close();
	}
	
	public static CLOB qrCodeStr(String contents)  throws SQLException{
		CLOB result = CLOB.createTemporary(getConnection(), true, CLOB.DURATION_CALL);
		try {
			String strResult = qrCode2Base64(contents, 190);
			Writer writer = result.getCharacterOutputStream();
			writer.write(strResult);
			writer.close();
		} catch (WriterException e) {
			throw new SQLException(e.getMessage());
		} catch (IOException e) {
			throw new SQLException(e.getMessage());
		}
		return result;
	}
	
	public static String qrCode2Base64(String contents, int size) throws WriterException, IOException{
		String result = "";
		RenderedImage image = qrCode(contents, size);
		ByteArrayOutputStream bs =new ByteArrayOutputStream();
		ImageOutputStream imOut =ImageIO.createImageOutputStream(bs);
		ImageIO.write(image, "png", imOut); 
		InputStream is =new ByteArrayInputStream(bs.toByteArray()); 
		BufferedInputStream in = new BufferedInputStream( is );
		
		result = base64Encode(in);
		return result;
	}
	
	
	/**
	 * 
	 * @param contents
	 * @param format
	 * @param size
	 * @return
	 * @throws WriterException
	 */
	public static RenderedImage qrCode(String contents, int size) throws WriterException {
		Hashtable<EncodeHintType, String> hints = new Hashtable<EncodeHintType, String>();
        hints.put(EncodeHintType.CHARACTER_SET, "UTF-8");
		BitMatrix bitMatrix = new MultiFormatWriter().encode(contents, BarcodeFormat.QR_CODE, size, size, hints);
		RenderedImage image = toBufferedImage(bitMatrix);
		return image;
	}
	/**
	 * generate image from matrix.
	 * @param matrix which generate by zxing.
	 * @return
	 */
	public static BufferedImage toBufferedImage(BitMatrix matrix) {
		int width = matrix.getWidth();
		int height = matrix.getHeight();
		BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_ARGB);
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				image.setRGB(x, y, matrix.get(x, y) == true ? BLACK : WHITE);
			}
		}
		return image;
	}
	
	/**
	 * encode innputstream to base64code.
	 * @param input
	 * @return
	 * @throws IOException
	 */
	private static String base64Encode(InputStream input) throws IOException {
		StringBuilder s = new StringBuilder();
		int i = 0;
		byte pos;
		//every time it get 3 chars, and encode them base on 3*8=4*6.
		byte[] readByte = new byte[3];
		int byteRead = 0;
		while((byteRead = input.read(readByte)) != -1){
			if(byteRead == 3){
				i ++;
				pos = (byte) ((readByte[0] >> 2) & 63);
				s.append(CODEC.charAt(pos));

				pos = (byte) (((readByte[0] & 3) << 4) + ((readByte[1] >> 4) & 15));
				s.append(CODEC.charAt(pos));

				pos = (byte) (((readByte[1] & 15) << 2) + ((readByte[2] >> 6) & 3));
				s.append(CODEC.charAt(pos));

				pos = (byte) (((readByte[2]) & 63));
				s.append(CODEC.charAt(pos));
				// create new line for each 79 chars.
				if((i % 19) == 0){
					s.append("\n");
				}
			}else if(byteRead == 2){// there is only 2 chars existed.
				pos = (byte) ((readByte[0] >> 2) & 63);
				s.append(CODEC.charAt(pos));

				pos = (byte) (((readByte[0] & 3) << 4) + ((readByte[1] >> 4) & 15));
				s.append(CODEC.charAt(pos));

				pos = (byte) ((readByte[1] & 15) << 2);
				s.append(CODEC.charAt(pos));

				s.append("=");
			}if(byteRead == 1){//there is only 1 char existed.
				
				pos = (byte) ((readByte[0] >> 2) & 63);
				s.append(CODEC.charAt(pos));
				pos = (byte) ((readByte[0] & 3) << 4);
				s.append(CODEC.charAt(pos));

				s.append("==");
			}
		}
		return s.toString();
	}
	
	private static Connection getConnection() throws SQLException{
		return (new oracle.jdbc.driver.OracleDriver()).defaultConnection();
	}
}
