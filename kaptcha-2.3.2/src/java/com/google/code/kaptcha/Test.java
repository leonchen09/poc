package com.google.code.kaptcha;

import java.awt.image.BufferedImage;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.Properties;

import javax.imageio.ImageIO;

import com.google.code.kaptcha.util.Config;

public class Test {

	/**
	 * @param args
	 * @throws IOException 
	 */
	public static void main(String[] args) throws IOException {
		Properties properties = new Properties();
		properties.put("kaptcha.textproducer.font.color", "black");
		properties.put("kaptcha.textproducer.char.space", 5 );
		Config config = new Config(properties);
		Producer kaptchaProducer = config.getProducerImpl();
		// TODO Auto-generated method stub
		String capText = kaptchaProducer.createText();
		System.out.println("capText:" + capText);
		BufferedImage bi = kaptchaProducer.createImage(capText);
		FileOutputStream out = new FileOutputStream("e:\\1.jpg");
		ImageIO.write(bi, "jpg", out);
		out.close();
	}

}
