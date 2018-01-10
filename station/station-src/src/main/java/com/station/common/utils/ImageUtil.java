package com.station.common.utils;

import java.awt.AlphaComposite;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.Transparency;
import java.awt.geom.AffineTransform;
import java.awt.geom.Ellipse2D;
import java.awt.image.AffineTransformOp;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URISyntaxException;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

import javax.imageio.ImageIO;

/**
 * 该类是图片处理类 生成类似微信的图像合成
 * 
 * @author 王永丰 出处:http://www.cnblogs.com/zovon/p/4345501.html
 */
public final class ImageUtil {
	/** 图片格式：JPG */
	private static final String PICTRUE_FORMATE_JPG = "jpg";

	private ImageUtil() {
	}

	/**
	 * 生成组合头像
	 * 
	 * @param paths
	 *            用户图像
	 * @throws IOException
	 * @throws URISyntaxException
	 */
	public static boolean getCombinationOfhead(String[] paths, String outPath) throws IOException {
		List<BufferedImage> bufferedImages = new ArrayList<BufferedImage>();
		// 压缩图片所有的图片生成尺寸同意的 为 50x50
		for (int i = 0; i < paths.length; i++) {
			// bufferedImages.add(resize2(paths[i], 50, 50, true));
			bufferedImages.add(resizeRound(paths[i], 50, 50, false));
		}

//		try {
//			ImageIO.write(bufferedImages.get(0), "png", new File(outPath+"small"));
//		} catch (IOException e) {
//			e.printStackTrace();
//		}

		int width = 112; // 这是画板的宽高

		int height = 112; // 这是画板的高度

		// BufferedImage.TYPE_INT_RGB可以自己定义可查看API
		BufferedImage outImage = new BufferedImage(width, height, Transparency.TRANSLUCENT);
		// 生成画布
		Graphics g2d = outImage.createGraphics();

		// 设置背景色
		// g2d.setBackground(new Color(231, 231, 231));
		// 通过使用当前绘图表面的背景色进行填充来清除指定的矩形。
		// g2d.clearRect(0, 0, width, height);

		// 开始拼凑 根据图片的数量判断该生成那种样式的组合头像目前为4中
		int j = 1;
		for (int i = 1; i <= bufferedImages.size(); i++) {
			if (bufferedImages.size() == 4) {
				if (i <= 2) {
					g2d.drawImage(bufferedImages.get(i - 1), 50 * i + 4 * i - 50, 4, null);
				} else {
					g2d.drawImage(bufferedImages.get(i - 1), 50 * j + 4 * j - 50, 58, null);
					j++;
				}
			} else if (bufferedImages.size() == 3) {
				if (i <= 1) {
					g2d.drawImage(bufferedImages.get(i - 1), 31, 4, null);
				} else {
					g2d.drawImage(bufferedImages.get(i - 1), 50 * j + 4 * j - 50, 58, null);
					j++;
				}
			} else if (bufferedImages.size() == 2) {
				g2d.drawImage(bufferedImages.get(i - 1), 50 * i + 4 * i - 50, 31, null);
			} else if (bufferedImages.size() == 1) {
				g2d.drawImage(bufferedImages.get(i - 1), 31, 31, null);
			}
			// 需要改变颜色的话在这里绘上颜色。可能会用到AlphaComposite类
		}

		// String outPath = "E:\\tmp\\"+fileName+".jpg";

		String format = "png";

		return ImageIO.write(outImage, format, new File(outPath));
	}

	/**
	 * 图片缩放
	 * 
	 * @param filePath
	 *            图片路径
	 * @param height
	 *            高度
	 * @param width
	 *            宽度
	 * @param bb
	 *            比例不对时是否需要补白
	 * @throws IOException
	 * @throws MalformedURLException
	 */
	public static BufferedImage resize2(String filePath, int height, int width, boolean bb)
			throws MalformedURLException, IOException {

		BufferedImage bi;
		if (filePath.startsWith("http")) {
			bi = ImageIO.read(new URL(filePath));
		} else {
			File f = new File(filePath);
			bi = ImageIO.read(f);
		}
		return resize2(bi, height, width, bb);
	}

	public static BufferedImage resize2(BufferedImage bi, int height, int width, boolean bb) {
		double ratio = 0; // 缩放比例

		Image itemp = bi.getScaledInstance(width, height, Image.SCALE_SMOOTH);
		// 计算比例
		if ((bi.getHeight() > height) || (bi.getWidth() > width)) {
			if (bi.getHeight() > bi.getWidth()) {
				ratio = (new Integer(height)).doubleValue() / bi.getHeight();
			} else {
				ratio = (new Integer(width)).doubleValue() / bi.getWidth();
			}
			AffineTransformOp op = new AffineTransformOp(AffineTransform.getScaleInstance(ratio, ratio), null);
			itemp = op.filter(bi, null);
		}
		if (bb) {
			// copyimg(filePath, "D:\\img");
			BufferedImage image = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
			Graphics2D g = image.createGraphics();
			g.setColor(Color.white);
			g.fillRect(0, 0, width, height);

			if (width == itemp.getWidth(null))
				g.drawImage(itemp, 0, (height - itemp.getHeight(null)) / 2, itemp.getWidth(null), itemp.getHeight(null),
						Color.white, null);
			else
				g.drawImage(itemp, (width - itemp.getWidth(null)) / 2, 0, itemp.getWidth(null), itemp.getHeight(null),
						Color.white, null);
			g.dispose();
			itemp = image;
		}
		return (BufferedImage) itemp;
	}

	public static BufferedImage resizeRound(String filePath, int height, int width, boolean bb)
			throws MalformedURLException, IOException {
		BufferedImage bi;
		if (filePath.startsWith("http")) {
			bi = ImageIO.read(new URL(filePath));
		} else {
			File f = new File(filePath);
			bi = ImageIO.read(f);
		}

		// BufferedImage bi1 = ImageIO.read(new File("c:/1.bmp"));

		// 根据需要是否使用 BufferedImage.TYPE_INT_ARGB
		// BufferedImage bi2 = new BufferedImage(bi.getWidth(), bi.getHeight(),
		// BufferedImage.TYPE_INT_RGB);
		BufferedImage bi2 = new BufferedImage(bi.getWidth(), bi.getHeight(), Transparency.TRANSLUCENT);

		Ellipse2D.Double shape = new Ellipse2D.Double(0, 0, bi.getWidth(), bi.getHeight());

		Graphics2D g2 = bi2.createGraphics();

		// g2.setBackground(new Color(231, 231, 231));

		g2.fill(shape);
		// g2.clearRect(0, 0, bi2.getWidth(), bi2.getHeight());
		g2.setClip(shape);
		g2.setComposite(AlphaComposite.SrcIn);
		// 使用 setRenderingHint 设置抗锯齿
		g2.drawImage(bi, 0, 0, null);
		g2.dispose();

		// try {
		// ImageIO.write(bi2, "png", new File("c:/2.png"));
		// } catch (IOException e) {
		// e.printStackTrace();
		// }

		return resize2(bi2, height, width, bb);
		// return bi2;
	}

	public static void main(String[] args) {
		try {
			String tmpDir=System.getProperty("user.dir")+System.getProperty("file.separator") +"tmp";
			System.out.println(tmpDir);
			System.out.println(ImageUtil.class.getResource("/").getFile());
			resizeRound("C:\\Users\\Administrator\\Pictures\\821557041524785152.jpg", 50, 50, true);
			// resizeRound("C:\\test.png", 50, 50, true);
		} catch (MalformedURLException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

}