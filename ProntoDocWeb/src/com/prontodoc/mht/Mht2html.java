package com.prontodoc.mht;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.Reader;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Map;

import javax.activation.DataHandler;
import javax.activation.DataSource;
import javax.activation.MimetypesFileTypeMap;
import javax.mail.MessagingException;
import javax.mail.Multipart;
import javax.mail.Session;
import javax.mail.internet.MimeBodyPart;
import javax.mail.internet.MimeMessage;
import javax.mail.internet.MimeMultipart;

import org.htmlparser.Node;
import org.htmlparser.Parser;
import org.htmlparser.filters.TagNameFilter;
import org.htmlparser.lexer.Lexer;
import org.htmlparser.lexer.Page;
import org.htmlparser.tags.BodyTag;
import org.htmlparser.tags.HeadTag;
import org.htmlparser.util.DefaultParserFeedback;
import org.htmlparser.util.NodeList;
import org.htmlparser.util.ParserException;

import com.prontodoc.mht.Html2MHTCompiler.AttachmentDataSource;

public class Mht2html {

	private static final String L_TAG_B = "<";
	private static final String R_TAG_B = ">\r\n";
	private static final String L_TAG_E = "</";
	private static final String R_TAG_E = ">\r\n";
	
	public static void main(String[] args) throws Exception {
		Mht2html m2h = new Mht2html();
		String strMht = "E:\\ProntoDir\\Mhttest.mht";
//		m2h.mht2html(strMht, "E:\\ProntoDir\\html1");
//		String htmlText = m2h.getHtml("E:\\ProntoDir\\Mhttest.mht");
		String top = "<table width='100%'><tr width='100%'><td width='100%'><h1><center><a href=\"index.html>\"><img src=\"IMAG0024.jpg\"/></a>top banner</center></h1></td></tr><tr><td>";
		String bottom = "</td></tr><tr><td><h1><center>isp:isp000003333300</center></h1></td></tr></table>";
		String encoding = "gb2312";
		m2h.updateHtml(strMht, top, bottom);
	}

	public void mht2html(String strMht, String strHtml) {
		try {
			InputStream fis = new FileInputStream(strMht);
			Session mailSession = Session.getDefaultInstance(System.getProperties(), null);
			MimeMessage msg = new MimeMessage(mailSession, fis);
			Object content = msg.getContent();
			if (content instanceof Multipart) {
				MimeMultipart mp = (MimeMultipart) content;
				MimeBodyPart bp1 = (MimeBodyPart) mp.getBodyPart(0);
				String strEncodng = getEncoding(bp1);
				String strText = getHtmlText(bp1, strEncodng);
				if (strText == null)
					return;
				File parent = null;
				if (mp.getCount() > 1) {
					parent = new File(new File(strHtml).getAbsolutePath() + ".files");
					parent.mkdirs();
					if (!parent.exists())
						return;
				}
				for (int i = 1; i < mp.getCount(); ++i) {
					MimeBodyPart bp = (MimeBodyPart) mp.getBodyPart(i);
					String strUrl = getResourcesUrl(bp);
					if (strUrl == null)
						continue;
					File resources = new File(parent.getAbsolutePath() + File.separator + getName(strUrl, i));
					if (saveResourcesFile(resources, bp.getInputStream()))
						strText = strText.replaceAll(strUrl, getFolderFileName(resources.getAbsolutePath(), '\\'));
				}
				saveHtml(strText, strHtml);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	private String getHtml(String strMht) throws MessagingException, IOException{
		String strHtml = null;
		InputStream fis = new FileInputStream(strMht);
		Session mailSession = Session.getDefaultInstance(System.getProperties(), null);
		MimeMessage msg = new MimeMessage(mailSession, fis);
		Object content = msg.getContent();
		if (content instanceof Multipart) {
			MimeMultipart mp = (MimeMultipart) content;
			MimeBodyPart bp1 = (MimeBodyPart) mp.getBodyPart(0);
			String strEncodng = getEncoding(bp1);
			strHtml = getHtmlText(bp1, strEncodng);
		}		
		return strHtml;
	}
	
	public void updateHtml(String strMht, String top, String bottom) throws Exception{
		InputStream fis = new FileInputStream(strMht);
		Session mailSession = Session.getDefaultInstance(System.getProperties(), null);
		MimeMessage msg = new MimeMessage(mailSession, fis);
		MimeMessage outMsg = new MimeMessage(mailSession);
		Object content = msg.getContent();
		if (content instanceof Multipart) {
			MimeMultipart mp = (MimeMultipart) content;
			String contentType = mp.getContentType();
			MimeBodyPart bp1 = (MimeBodyPart) mp.getBodyPart(0);
			String bp1_contentType = bp1.getContentType();
			String strEncoding = getEncoding(bp1);
			String strHtml = getHtmlText(bp1, strEncoding);
			strHtml = addContent(strHtml, top, bottom, bp1.getEncoding());
			bp1.setText(strHtml);
			bp1.setHeader("Content-Type", bp1_contentType);
//			System.out.println(getHtmlText(bp1, strEncodng));
//			mp.addBodyPart(bp1,0);
			MimeBodyPart bp = new MimeBodyPart();
			String absoluteURL = "D:\\Source\\POC\\ProntoDocWeb\\WebContent\\IMAG0024.jpg";
			bp.addHeader("Content-Location", javax.mail.internet.MimeUtility.encodeWord(java.net.URLDecoder.decode(absoluteURL, strEncoding)));
			String content_Location = bp1.getHeader("Content-Location")[0];
			content_Location = content_Location.substring(0, content_Location.lastIndexOf("/"));
			bp.setHeader("Content-Location", content_Location + "/Mhttest.files/IMAG0024.jpg");
			DataSource source = new AttachmentDataSource(absoluteURL, "image");
			bp.setDataHandler(new DataHandler(source));
			mp.addBodyPart(bp);
			outMsg.setContent(mp);
//			outMsg.removeHeader("Message-ID");
		}
		outMsg.writeTo(new FileOutputStream("E:\\ProntoDir\\Mhttest.mht"));
	}
	
	private void saveHtml(String strText, String strHtml) {
		try {
			FileWriter fw = new FileWriter(strHtml + ".htm");
			fw.write(strText);
			fw.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	private boolean saveResourcesFile(File resources, InputStream inputStream) {
		if (resources == null || inputStream == null) {
			return false;
		}
		BufferedInputStream in = null;
		FileOutputStream fio = null;
		BufferedOutputStream osw = null;
		try {
			in = new BufferedInputStream(inputStream);
			fio = new FileOutputStream(resources);
			osw = new BufferedOutputStream(new DataOutputStream(fio));
			int b;
			byte[] a = new byte[1024];
			boolean isEmpty = true;
			while ((b = in.read(a)) != -1) {
				isEmpty = false;
				osw.write(a, 0, b);
				osw.flush();
			}
			osw.close();
			fio.close();
			in.close();
			inputStream.close();
			if (isEmpty)
				resources.delete();
			return true;
		} catch (Exception e) {
			e.printStackTrace();
			return false;
		} finally {
			try {
				if (osw != null)
					osw.close();
				if (fio != null)
					fio.close();
				if (in != null)
					in.close();
				if (inputStream != null)
					inputStream.close();
			} catch (Exception e) {
				e.printStackTrace();
				return false;
			}
		}
	}

	private String getEncoding(MimeBodyPart bp) {
		if (bp != null) {
			try {
				Enumeration list = bp.getAllHeaders();
				while (list.hasMoreElements()) {
					javax.mail.Header head = (javax.mail.Header) list
							.nextElement();
					if (head.getName().compareTo("Content-Type") == 0) {
						String strType = head.getValue();
						int pos = strType.indexOf("charset=");
						if (pos != -1) {
							String strEncoding = strType.substring(pos + 9, strType.length() - 1);
							if (strEncoding.toLowerCase().compareTo("gb2312") == 0) {
								strEncoding = "gbk";
							}
							return strEncoding;
						}
					}
				}
			} catch (MessagingException e) {
				e.printStackTrace();
			}
		}
		return null;
	}

	private String getHtmlText(MimeBodyPart bp, String strEncoding) {
		InputStream textStream = null;
		BufferedInputStream buff = null;
		BufferedReader br = null;
		Reader r = null;
		try {
			textStream = bp.getInputStream();
			buff = new BufferedInputStream(textStream);
			r = new InputStreamReader(buff, strEncoding);
			br = new BufferedReader(r);
			StringBuffer strHtml = new StringBuffer("");
			String strLine = null;
			while ((strLine = br.readLine()) != null) {
				strHtml.append(strLine);
			}
			br.close();
			r.close();
			textStream.close();
			return strHtml.toString();
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			try {
				if (br != null)
					br.close();
				if (buff != null)
					buff.close();
				if (textStream != null)
					textStream.close();
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		return null;
	}

	private String getResourcesUrl(MimeBodyPart bp) {
		char separator = '/';
		if (bp != null) {
			try {
				Enumeration list = bp.getAllHeaders();
				while (list.hasMoreElements()) {
					javax.mail.Header head = (javax.mail.Header) list.nextElement();
					if (head.getName().compareTo("Content-Location") == 0) {
						String absolutePath = head.getValue();
						String part1 = absolutePath.substring(0, absolutePath.lastIndexOf(separator));
						String fileName = absolutePath.substring(absolutePath.lastIndexOf(separator));
						String folderName = part1.substring(part1.lastIndexOf(separator) + 1);
						return folderName + fileName;
					}
				}
			} catch (MessagingException e) {
				e.printStackTrace();
			}
		}
		return null;
	}

	private String getName(String strName, int ID) {
		char separator = '/';
		if (strName.lastIndexOf(separator) >= 0)
			return format(strName.substring(strName.lastIndexOf(separator) + 1));
		return "temp" + ID;
	}

	private String getFolderFileName(String absolutePath, char separator) {
		System.out.println(absolutePath);
		String part1 = absolutePath.substring(0, absolutePath.lastIndexOf(separator));
		String fileName = absolutePath.substring(absolutePath.lastIndexOf(separator));
		String folderName = part1.substring(part1.lastIndexOf(separator) + 1);
		return folderName + "\\" + fileName;
	}

	private String format(String strName) {
		if (strName == null)
			return null;
		strName = strName.replaceAll("   ", " ");
		String strText = "/:*?\"<>|^;";
		for (int i = 0; i < strName.length(); ++i) {
			String ch = String.valueOf(strName.charAt(i));
			if (strText.indexOf(ch) != -1) {
				strName = strName.replace(strName.charAt(i), '-');
			}
		}
		return strName;
	}
	
	public String addContent(String oldHtml, String top, String bottom, String encoding) throws ParserException{
		StringBuffer result = new StringBuffer();
		NodeList nodes = new NodeList();
		Parser parser = createParser(oldHtml);
		parser.setEncoding(encoding);
		nodes = parser.parse(null);
		Node htmlTag = nodes.elementAt(0);
		String htmlTagText = htmlTag.getText();
		result.append(L_TAG_B + htmlTagText + R_TAG_B);
		HeadTag header = (HeadTag) htmlTag.getFirstChild();
		result.append(L_TAG_B + header.getText() + R_TAG_B + header.getStringText());
		result.append(L_TAG_E + "head" + R_TAG_E);
		 BodyTag body = (BodyTag) htmlTag.getLastChild();
		 result.append(L_TAG_B + body.getText() + R_TAG_B );
		 String oldContent = body.getStringText();
		 result.append(top + oldContent + bottom);
		 result.append(L_TAG_E + "body" + R_TAG_E);
		 result.append(L_TAG_E + "html" + R_TAG_E);
		 return result.toString();
	}
	private Parser createParser(String inputHTML) {
		Lexer mLexer = new Lexer(new Page(inputHTML));
		return new Parser(mLexer, new DefaultParserFeedback(
				DefaultParserFeedback.QUIET));
	}
	
	//inner class for attach image
	class AttachmentDataSource implements DataSource {
		private MimetypesFileTypeMap map = new MimetypesFileTypeMap();
		private String fullName;
		private String strType;
		private byte[] dataSize = null;

		/**
		 * This is some content type maps.
		 */
		private Map normalMap = new HashMap();
		{
			// Initiate normal mime type map
			// Images
			normalMap.put("image", "image/jpeg");
			normalMap.put("text", "text/plain");
		}

		public AttachmentDataSource(String fullName, String strType) throws Exception {
			this.strType = strType;
			this.fullName = fullName;
			FileInputStream in = new FileInputStream(fullName);
			ByteArrayOutputStream out = new ByteArrayOutputStream(1024);
			byte[] tmp = new byte[1024];
			int byteReader = 0;
			while((byteReader = in.read(tmp)) != -1){
				out.write(tmp, 0, byteReader);
			}
			in.close();
			dataSize = out.toByteArray();
			out.close();
		}

		/**
		 * Returns the content type.
		 */
		public String getContentType() {
			return getMimeType(getName());
		}

		public String getName() {
			char separator = File.separatorChar;
			if (fullName.lastIndexOf(separator) >= 0)
				return fullName.substring(fullName.lastIndexOf(separator) + 1);
			return fullName;
		}

		private String getMimeType(String fileName) {
			String type = (String) normalMap.get(strType);
			if (type == null) {
				try {
					type = map.getContentType(fileName);
				} catch (Exception e) {
				}
				if (type == null) {
					type = "application/octet-stream";
				}
			}

			return type;
		}

		public InputStream getInputStream() throws IOException {
			if (dataSize == null)
				dataSize = new byte[0];
			return new ByteArrayInputStream(dataSize);
		}

		public OutputStream getOutputStream() throws IOException {
			return new java.io.ByteArrayOutputStream();
		}

	}
}
