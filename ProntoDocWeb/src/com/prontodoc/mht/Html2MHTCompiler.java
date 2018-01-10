package com.prontodoc.mht;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.DataOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.Reader;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.*;
import org.htmlparser.Parser;
import org.htmlparser.Tag;
import org.htmlparser.filters.TagNameFilter;
import org.htmlparser.lexer.Lexer;
import org.htmlparser.lexer.Page;
import org.htmlparser.util.DefaultParserFeedback;
import org.htmlparser.util.NodeList;
import org.htmlparser.util.ParserException;
//import toptrack.tools.JQuery;
import javax.activation.DataHandler;
import javax.activation.DataSource;
import javax.activation.MimetypesFileTypeMap;
import javax.mail.Message;
import javax.mail.MessagingException;
import javax.mail.Multipart;
import javax.mail.Session;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeBodyPart;
import javax.mail.internet.MimeMessage;
import javax.mail.internet.MimeMultipart;
import javax.mail.internet.MimePartDataSource;

/**
 * mht�ļ�������
 * 
 * @author dl
 */
public class Html2MHTCompiler {
	private URL strWeb = null;
	/** ��ҳ��ַ */
	private String strText = null;
	/** ��ҳ�ı����� */
	private String strFileName = null;
	/** �����ļ��� */
	private String strEncoding = null;
	/** ��ҳ���� */
	// mht��ʽ������Ϣ
	private String from = "";
	private String to;
	private String subject = "mht compile";
	private String cc;
	private String bcc;
	private String smtp = "localhost";

	public static void main(String[] args) {
		// String strUrl = "http://localhost";
		// String strEncoding = "utf-8";
		// String strText = "";//leon,JQuery.getHtmlText(strUrl, strEncoding,
		// null);
		// if (strText == null)
		// return;
		// Html2MHTCompiler h2t = new Html2MHTCompiler(strText, strUrl,
		// strEncoding, "test.mht");
		// h2t.compile();
		Html2MHTCompiler.mht2html("E:\\ProntoDir\\Dir\\Mhttest.mht", "E:\\ProntoDir\\Dir\\htm");
	}

	/**
	 * <br>
	 * ����˵������ʼ�� <br>
	 * ���������strText ��ҳ�ı�����; strUrl ��ҳ��ַ; strEncoding ��ҳ����; strFileName �����ļ��� <br>
	 * �������ͣ�
	 */
	public Html2MHTCompiler(String strText, String strUrl, String strEncoding,
			String strFileName) {
		// TODO Auto-generated constructor stub
		try {
			strWeb = new URL(strUrl);
		} catch (MalformedURLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return;
		}
		this.strText = strText;
		this.strEncoding = strEncoding;
		this.strFileName = strFileName;
	}

	/**
	 * <br>
	 * ����˵����ִ�����ز��� <br>
	 * ��������� <br>
	 * �������ͣ�
	 */
	public boolean compile() {
		if (strWeb == null || strText == null || strFileName == null
				|| strEncoding == null)
			return false;
		HashMap urlMap = new HashMap();
		NodeList nodes = new NodeList();
		try {
			Parser parser = createParser(strText);
			parser.setEncoding(strEncoding);
			nodes = parser.parse(null);
		} catch (ParserException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		extractAllScriptNodes(nodes);
		ArrayList urlScriptList = extractAllScriptNodes(nodes, urlMap);
		ArrayList urlImageList = extractAllImageNodes(nodes, urlMap);
		for (Iterator iter = urlMap.entrySet().iterator(); iter.hasNext();) {
			Map.Entry entry = (Map.Entry) iter.next();
			String key = (String) entry.getKey();
			String val = (String) entry.getValue();
			// strText = JHtmlClear.replace(strText, val, key);
			strText = strText.replaceAll(val, key);
		}
		try {
			createMhtArchive(strText, urlScriptList, urlImageList);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return false;
		}
		return true;
	}

	/**
	 * <br>
	 * ����˵��������HTML parser <br>
	 * ���������inputHTML ��ҳ�ı����� <br>
	 * �������ͣ�HTML parser
	 */
	private Parser createParser(String inputHTML) {
		// TODO Auto-generated method stub
		Lexer mLexer = new Lexer(new Page(inputHTML));
		return new Parser(mLexer, new DefaultParserFeedback(
				DefaultParserFeedback.QUIET));
	}

	/**
	 * <br>
	 * ����˵������ȡ����URL��ַ <br>
	 * ���������nodes ��ҳ��ǩ���� <br>
	 * �������ͣ�
	 */
	private void extractAllScriptNodes(NodeList nodes) {
		NodeList filtered = nodes.extractAllNodesThatMatch(new TagNameFilter(
				"BASE"), true);
		if (filtered != null && filtered.size() > 0) {
			Tag tag = (Tag) filtered.elementAt(0);
			String href = tag.getAttribute("href");
			if (href != null && href.length() > 0) {
				try {
					strWeb = new URL(href);
				} catch (MalformedURLException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		}
	}

	/**
	 * <br>
	 * ����˵������ȡ��ҳ������css,js���� <br>
	 * ���������nodes ��ҳ��ǩ����; urlMap �Ѵ��ڵ�url���� <br>
	 * �������ͣ�css,js���ӵļ���
	 */
	private ArrayList extractAllScriptNodes(NodeList nodes, HashMap urlMap) {
		ArrayList urlList = new ArrayList();
		NodeList filtered = nodes.extractAllNodesThatMatch(new TagNameFilter(
				"script"), true);
		for (int i = 0; i < filtered.size(); i++) {
			Tag tag = (Tag) filtered.elementAt(i);
			String src = tag.getAttribute("src");
			// Handle external css file's url
			if (src != null && src.length() > 0) {
				String innerURL = src;
				String absoluteURL = makeAbsoluteURL(strWeb, innerURL);
				if (absoluteURL != null && !urlMap.containsKey(absoluteURL)) {
					urlMap.put(absoluteURL, innerURL);
					ArrayList urlInfo = new ArrayList();
					urlInfo.add(innerURL);
					urlInfo.add(absoluteURL);
					urlList.add(urlInfo);
				}
				tag.setAttribute("src", absoluteURL);
			}
		}
		filtered = nodes.extractAllNodesThatMatch(new TagNameFilter("link"),
				true);
		for (int i = 0; i < filtered.size(); i++) {
			Tag tag = (Tag) filtered.elementAt(i);
			String type = (tag.getAttribute("type"));
			String rel = (tag.getAttribute("rel"));
			String href = tag.getAttribute("href");
			boolean isCssFile = false;
			if (rel != null) {
				isCssFile = rel.indexOf("stylesheet") != -1;
			} else if (type != null) {
				isCssFile |= type.indexOf("text/css") != -1;
			}
			// Handle external css file's url
			if (isCssFile && href != null && href.length() > 0) {
				String innerURL = href;
				String absoluteURL = makeAbsoluteURL(strWeb, innerURL);
				if (absoluteURL != null && !urlMap.containsKey(absoluteURL)) {
					urlMap.put(absoluteURL, innerURL);
					ArrayList urlInfo = new ArrayList();
					urlInfo.add(innerURL);
					urlInfo.add(absoluteURL);
					urlList.add(urlInfo);
				}
				tag.setAttribute("href", absoluteURL);
			}
		}
		return urlList;
	}

	/**
	 * <br>
	 * ����˵������ȡ��ҳ������ͼ������ <br>
	 * ���������nodes ��ҳ��ǩ����; urlMap �Ѵ��ڵ�url���� <br>
	 * �������ͣ�ͼ�����Ӽ���
	 */
	private ArrayList extractAllImageNodes(NodeList nodes, HashMap urlMap) {
		ArrayList urlList = new ArrayList();
		NodeList filtered = nodes.extractAllNodesThatMatch(new TagNameFilter(
				"IMG"), true);
		for (int i = 0; i < filtered.size(); i++) {
			Tag tag = (Tag) filtered.elementAt(i);
			String src = tag.getAttribute("src");
			// Handle external css file's url
			if (src != null && src.length() > 0) {
				String innerURL = src;
				String absoluteURL = makeAbsoluteURL(strWeb, innerURL);
				if (absoluteURL != null && !urlMap.containsKey(absoluteURL)) {
					urlMap.put(absoluteURL, innerURL);
					ArrayList urlInfo = new ArrayList();
					urlInfo.add(innerURL);
					urlInfo.add(absoluteURL);
					urlList.add(urlInfo);
				}
				tag.setAttribute("src", absoluteURL);
			}
		}
		return urlList;
	}

	/**
	 * <br>
	 * ����˵�������·��ת����·�� <br>
	 * ���������strWeb ��ҳ��ַ; innerURL ���·������ <br>
	 * �������ͣ�����·������
	 */
	public static String makeAbsoluteURL(URL strWeb, String innerURL) {
		// TODO Auto-generated method stub
		// ȥ����׺
		int pos = innerURL.indexOf("?");
		if (pos != -1) {
			innerURL = innerURL.substring(0, pos);
		}
		if (innerURL != null && innerURL.toLowerCase().indexOf("http") == 0) {
			System.out.println(innerURL);
			return innerURL;
		}
		URL linkUri = null;
		try {
			linkUri = new URL(strWeb, innerURL);
		} catch (MalformedURLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return null;
		}
		String absURL = linkUri.toString();
		// absURL = JHtmlClear.replace(absURL, "../", "");
		// absURL = JHtmlClear.replace(absURL, "./", "");
		absURL = absURL.replaceAll("../", "");
		absURL = absURL.replaceAll("./", "");
		System.out.println(absURL);
		return absURL;
	}

	/**
	 * <br>
	 * ����˵��������mht�ļ� <br>
	 * ���������content ��ҳ�ı�����; urlScriptList �ű����Ӽ���; urlImageList ͼƬ���Ӽ��� <br>
	 * �������ͣ�
	 */
	private void createMhtArchive(String content, ArrayList urlScriptList,
			ArrayList urlImageList) throws Exception {
		// Instantiate a Multipart object
		MimeMultipart mp = new MimeMultipart("related");
		Properties props = new Properties();
		props.put("mail.smtp.host", smtp);
		Session session = Session.getDefaultInstance(props, null);
		MimeMessage msg = new MimeMessage(session);
		// set mailer
		msg.setHeader("X-Mailer", "Code Manager .SWT");
		// set from
		if (from != null) {
			msg.setFrom(new InternetAddress(from));
		}
		// set subject
		if (subject != null) {
			msg.setSubject(subject);
		}
		// to
		if (to != null) {
			InternetAddress[] toAddresses = getInetAddresses(to);
			msg.setRecipients(Message.RecipientType.TO, toAddresses);
		}
		// cc
		if (cc != null) {
			InternetAddress[] ccAddresses = getInetAddresses(cc);
			msg.setRecipients(Message.RecipientType.CC, ccAddresses);
		}
		// bcc
		if (bcc != null) {
			InternetAddress[] bccAddresses = getInetAddresses(bcc);
			msg.setRecipients(Message.RecipientType.BCC, bccAddresses);
		}
		// ������ҳ����
		MimeBodyPart bp = new MimeBodyPart();
		bp.setText(content, strEncoding);
		bp.addHeader("Content-Type", "text/html;charset=" + strEncoding);
		bp.addHeader("Content-Location", strWeb.toString());
		mp.addBodyPart(bp);
		int urlCount = urlScriptList.size();
		for (int i = 0; i < urlCount; i++) {
			bp = new MimeBodyPart();
			ArrayList urlInfo = (ArrayList) urlScriptList.get(i);
			// String url = urlInfo.get(0).toString();
			String absoluteURL = urlInfo.get(1).toString();
			bp.addHeader("Content-Location", javax.mail.internet.MimeUtility
					.encodeWord(java.net.URLDecoder.decode(absoluteURL,
							strEncoding)));
			DataSource source = new AttachmentDataSource(absoluteURL, "text");
			bp.setDataHandler(new DataHandler(source));
			mp.addBodyPart(bp);
		}

		urlCount = urlImageList.size();
		for (int i = 0; i < urlCount; i++) {
			bp = new MimeBodyPart();
			ArrayList urlInfo = (ArrayList) urlImageList.get(i);
			// String url = urlInfo.get(0).toString();
			String absoluteURL = urlInfo.get(1).toString();
			bp.addHeader("Content-Location", javax.mail.internet.MimeUtility
					.encodeWord(java.net.URLDecoder.decode(absoluteURL,
							strEncoding)));
			DataSource source = new AttachmentDataSource(absoluteURL, "image");
			bp.setDataHandler(new DataHandler(source));
			mp.addBodyPart(bp);
		}
		msg.setContent(mp);
		// write the mime multi part message to a file
		msg.writeTo(new FileOutputStream(strFileName));
	}

	/**
	 * <br>
	 * ����˵����mhtתhtml <br>
	 * ���������strMht mht�ļ�·��; strHtml html�ļ�·�� <br>
	 * �������ͣ�
	 */
	public static void mht2html(String strMht, String strHtml) {
		try {
			// TODO readEmlFile
			InputStream fis = new FileInputStream(strMht);
			Session mailSession = Session.getDefaultInstance(
					System.getProperties(), null);
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
					parent = new File(new File(strHtml).getAbsolutePath()
							+ ".files");
					parent.mkdirs();
					if (!parent.exists())
						return;
				}
				for (int i = 1; i < mp.getCount(); ++i) {
					MimeBodyPart bp = (MimeBodyPart) mp.getBodyPart(i);

					String strUrl = getResourcesUrl(bp);
					if (strUrl == null)
						continue;

					// DataHandler dataHandler = bp.getDataHandler();
					// MimePartDataSource source =
					// (MimePartDataSource)dataHandler.getDataSource();
					File resources = new File(parent.getAbsolutePath() + File.separator + getName(strUrl, i));
					if (saveResourcesFile(resources, bp.getInputStream()))
						// strText = JHtmlClear.replace(strText, strUrl,
						// resources.getAbsolutePath());
						strText = strText.replaceAll(
								strUrl,
								getFolderFileName(resources.getAbsolutePath(),
										'\\'));
				}
				saveHtml(strText, strHtml);
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	/**
	 * <br>
	 * ����˵�����õ���Դ�ļ���name <br>
	 * ���������strName ��Դ�ļ�����, ID ��Դ�ļ������ <br>
	 * �������ͣ���Դ�ļ��ı�����ʱ�ļ���
	 */
	public static String getName(String strName, int ID) {
		char separator = '/';
		// System.out.println(strName);
		// System.out.println(separator);
		if (strName.lastIndexOf(separator) >= 0)
			return format(strName.substring(strName.lastIndexOf(separator) + 1));
		return "temp" + ID;
	}

	private static String getFolderFileName(String absolutePath, char separator) {
		System.out.println(absolutePath);
		String part1 = absolutePath.substring(0,
				absolutePath.lastIndexOf(separator));
		String fileName = absolutePath.substring(absolutePath
				.lastIndexOf(separator));
		String folderName = part1.substring(part1.lastIndexOf(separator) + 1);
		return folderName + "\\" + fileName;
	}

	/**
	 * <br>
	 * ����˵�����õ���ҳ���� <br>
	 * ���������bp MimeBodyPart���͵���ҳ���� <br>
	 * �������ͣ�MimeBodyPart�����ҳ���ݵı���
	 */
	private static String getEncoding(MimeBodyPart bp) {
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
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

		}
		return null;
	}

	/**
	 * <br>
	 * ����˵�����õ���Դ�ļ�url <br>
	 * ���������bp MimeBodyPart���͵���ҳ���� <br>
	 * �������ͣ���Դ�ļ�url
	 */
	private static String getResourcesUrl(MimeBodyPart bp) {
		char separator = '/';
		if (bp != null) {
			try {
				Enumeration list = bp.getAllHeaders();
				while (list.hasMoreElements()) {
					javax.mail.Header head = (javax.mail.Header) list
							.nextElement();
					if (head.getName().compareTo("Content-Location") == 0) {
						String absolutePath = head.getValue();
						String part1 = absolutePath.substring(0,
								absolutePath.lastIndexOf(separator));
						String fileName = absolutePath.substring(absolutePath
								.lastIndexOf(separator));
						String folderName = part1.substring(part1
								.lastIndexOf(separator) + 1);
						return folderName + fileName;
					}
				}
			} catch (MessagingException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

		}
		return null;
	}

	/**
	 * <br>
	 * ����˵������ʽ���ļ��� <br>
	 * ���������strName �ļ��� <br>
	 * �������ͣ���������ķ�������������ļ���
	 */
	private static String format(String strName) {
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

	/**
	 * <br>
	 * ����˵����������Դ�ļ� <br>
	 * ���������resources Ҫ��������Դ�ļ�; inputStream Ҫ�����ļ��е��� <br>
	 * �������ͣ�boolean
	 */
	private static boolean saveResourcesFile(File resources,
			InputStream inputStream) {
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
			// TODO Auto-generated catch block
			e.printStackTrace();
			System.out.println("����mhtʧ��");
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
				System.out.println("����mhtʧ��");
				return false;
			}
		}
	}

	/**
	 * <br>
	 * ����˵�����õ�mht�ļ��ı��� <br>
	 * ���������mhtFilename mht�ļ��� <br>
	 * �������ͣ�mht�ļ��ı���
	 */
	public static String getTitle(String mhtFilename) {
		try {
			// TODO readEmlFile
			InputStream fis = new FileInputStream(mhtFilename);
			Session mailSession = Session.getDefaultInstance(
					System.getProperties(), null);
			MimeMessage msg = new MimeMessage(mailSession, fis);
			Object content = msg.getContent();
			if (content instanceof Multipart) {
				MimeMultipart mp = (MimeMultipart) content;
				MimeBodyPart bp1 = (MimeBodyPart) mp.getBodyPart(0);
				String strEncodng = getEncoding(bp1);
				String strText = getHtmlText(bp1, strEncodng);
				if (strText == null)
					return null;
				strText = strText.toLowerCase();
				int pos1 = strText.indexOf("<title>");
				int pos2 = strText.indexOf("</title>");
				if (pos1 != -1 && pos2 != -1 && pos2 > pos1) {
					return strText.substring(pos1 + 7, pos2).trim();
				}
			}
			return null;
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return null;
		}
	}

	/**
	 * <br>
	 * ����˵�����õ�html�ı� <br>
	 * ���������bp MimeBodyPart���͵���ҳ����; strEncoding ���ݱ��� <br>
	 * �������ͣ�html�ı�
	 */
	private static String getHtmlText(MimeBodyPart bp, String strEncoding) {
		InputStream textStream = null;
		BufferedInputStream buff = null;
		BufferedReader br = null;
		Reader r = null;
		try {
			textStream = bp.getInputStream();
			buff = new BufferedInputStream(textStream);
			r = new InputStreamReader(buff, "GB2312");// strEncoding);
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
			// TODO Auto-generated catch block
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
				System.out.println("����mhtʧ��");
			}
		}
		return null;
	}

	/**
	 * <br>
	 * ����˵��������html�ļ� <br>
	 * ���������strText html����; strHtml html�ļ��� <br>
	 * �������ͣ�
	 */
	private static void saveHtml(String strText, String strHtml) {
		try {
			FileWriter fw = new FileWriter(strHtml + ".htm");
			fw.write(strText);
			fw.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			System.out.println("����mhtʧ��");
		}
	}

	private InternetAddress[] getInetAddresses(String emails) throws Exception {
		ArrayList list = new ArrayList();
		StringTokenizer tok = new StringTokenizer(emails, ",");
		while (tok.hasMoreTokens()) {
			list.add(tok.nextToken());
		}
		int count = list.size();
		InternetAddress[] addresses = new InternetAddress[count];
		for (int i = 0; i < count; i++) {
			addresses[i] = new InternetAddress(list.get(i).toString());
		}
		return addresses;
	}

	class AttachmentDataSource implements DataSource {
		private MimetypesFileTypeMap map = new MimetypesFileTypeMap();
		private String strUrl;
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

		public AttachmentDataSource(String strUrl, String strType) {
			this.strType = strType;
			this.strUrl = strUrl;

			strUrl = strUrl.trim();
			strUrl = strUrl.replaceAll(" ", "%20");
			dataSize = new byte[10];// leon;JQuery.downBinaryFile(strUrl, null);
		}

		/**
		 * Returns the content type.
		 */
		public String getContentType() {
			return getMimeType(getName());
		}

		public String getName() {
			char separator = File.separatorChar;
			if (strUrl.lastIndexOf(separator) >= 0)
				return strUrl.substring(strUrl.lastIndexOf(separator) + 1);
			return strUrl;
		}

		private String getMimeType(String fileName) {
			String type = (String) normalMap.get(strType);
			if (type == null) {
				try {
					type = map.getContentType(fileName);
				} catch (Exception e) {
					// TODO: handle exception
				}
				System.out.println(type);
				// Fix the null exception
				if (type == null) {
					type = "application/octet-stream";
				}
			}

			return type;
		}

		public InputStream getInputStream() throws IOException {
			// TODO Auto-generated method stub
			if (dataSize == null)
				dataSize = new byte[0];
			return new ByteArrayInputStream(dataSize);
		}

		public OutputStream getOutputStream() throws IOException {
			// TODO Auto-generated method stub
			return new java.io.ByteArrayOutputStream();
		}

	}
}
