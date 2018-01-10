package com.stepfunction.prontodoc.sx.dao.o10g;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.StringWriter;
import java.io.Writer;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.sql.Types;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import oracle.sql.BLOB;
import oracle.sql.CLOB;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Text;

public class ForXml {
	
	private final static String CODEC = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
	
	public static void main(String[] argv) throws SQLException, IOException{
//		CLOB result = forXml("select id id, n3 n3,n4 from orclxml o ","[t1], id, n3,n4",true,"pdwdata");
//		Reader reader = result.getCharacterStream();
//		FileWriter writer = new FileWriter("e:\\prontodir\\oracle_xml\\oraclexml.xml1");
//		BufferedReader br = new BufferedReader(reader);
//		String data = null;
//		while((data = br.readLine()) != null ){
//			writer.write(data);
//		}
//		writer.flush();
//		writer.close();
		long data = (long)Math.pow(2, 62);
		long data2 = (long)Math.pow(2, 62)-1;//Long.MIN_VALUE;
		System.out.println(data+":"+Long.toBinaryString(data));
		System.out.println(data2+":"+Long.toBinaryString(data2));
		System.out.println((data+data2)+":"+Long.toBinaryString(data+data2));
	}

	public static String forXmlStr(ResultSet rs, String selectedColumns, boolean returnElement, String rootElement) throws SQLException{
		String result = null;
		if(rootElement == null || rootElement.length() < 1)
			rootElement= "xml";
		
		Document document = buildDocument(rs, selectedColumns, returnElement, rootElement);

		try {
			result = saveToStr(document);
		} catch (TransformerException e) {
			throw new SQLException(e.getMessage());
		} catch (IOException e) {
			throw new SQLException(e.getMessage());
		}

		return result;
	}
	
	public static CLOB forXml(ResultSet rs, String selectedColumns, boolean returnElement, String rootElement) throws SQLException{
		CLOB result = CLOB.createTemporary(getConnection(), true, CLOB.DURATION_CALL);
		if(rootElement == null || rootElement.length() < 1)
			rootElement= "xml";
		
		Document document = buildDocument(rs, selectedColumns, returnElement, rootElement);

		try {
			saveToTemporary(document, result);
		} catch (TransformerException e) {
			throw new SQLException(e.getMessage());
		} catch (IOException e) {
			throw new SQLException(e.getMessage());
		}

		return result;
	}

	private static Document buildDocument(ResultSet rs, String selectedColumns, boolean returnElement,String rootElement) throws SQLException{
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		DocumentBuilder builder = null;
		try {
			builder = factory.newDocumentBuilder();
		} catch (ParserConfigurationException e) {
			throw new SQLException(e.getMessage());
		}
		Document document = builder.newDocument();
		Element root = document.createElement(rootElement);
		document.appendChild(root);
		
		List<List<String>> elementStructure = prepareStructure(selectedColumns);
		
//		Statement st = getConnection().createStatement();
//		ResultSet rs = st.executeQuery(sql);
		
		String[] beforeResult = new String[elementStructure.size()];

		Element parentElement = root;
		boolean newRow = true;
		Map<String, Integer> metaData = getMetaData(rs);
		while(rs.next()){
			parentElement = root; //new row, go back to the root.
			for(int i = 0; i < elementStructure.size(); i ++){
				List<String> table = elementStructure.get(i);
				String tableName = table.get(0);// the first element is table alias
				Element tabEl = document.createElement(tableName);
				StringBuffer curRowValue = new StringBuffer();
				for(int j = 1; j < table.size(); j ++){// the following elements are column alias.
					String colName = table.get(j);
					Object value = rs.getObject(colName);
					String colVal = getValue(value, metaData.get(colName.toUpperCase()));
					curRowValue.append(abstrString(colVal));
					
					if(returnElement){
						Element tabColEl = document.createElement(colName);
						Text val = document.createTextNode(colVal);
						tabColEl.appendChild(val);
						tabEl.appendChild(tabColEl);
					}else{
						tabEl.setAttribute(colName, colVal);
					}
				}
				if(newRow || !curRowValue.toString().equals(beforeResult[i])){
					newRow = true;
					beforeResult[i] = curRowValue.toString();
					parentElement.appendChild(tabEl);
					parentElement = tabEl;
				}else{
					parentElement = (Element) parentElement.getLastChild();
				}
			}
			newRow = false;
		}
		rs.close();
		return document;
	}
	
	private static Connection getConnection() throws SQLException{
//		String url = "jdbc:oracle:thin:@localhost:1521:orcl";
//		String userName = "app";
//		String password = "app";
//	
//		try {
//			Class.forName("oracle.jdbc.driver.OracleDriver");
//		} catch (ClassNotFoundException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//		return DriverManager.getConnection(url, userName, password);
		return (new oracle.jdbc.driver.OracleDriver()).defaultConnection();
	}
	
	private static Map<String, Integer> getMetaData(ResultSet rs) throws SQLException{
		Map<String, Integer> result = new HashMap<String, Integer>();
		ResultSetMetaData rsmd = rs.getMetaData();
		for(int i = 1; i <= rsmd.getColumnCount(); i ++){
			result.put(rsmd.getColumnLabel(i), rsmd.getColumnType(i));
		}
		return result;
	}
	
	public static List<List<String>> prepareStructure(String selectedColumns) throws SQLException {
		if (selectedColumns == null || selectedColumns.trim().length() < 8)
			throw new RuntimeException("Invalidate selected columns: " + selectedColumns);

		List<List<String>> result = new ArrayList<List<String>>();

		try{
		StringTokenizer st = new StringTokenizer(selectedColumns, ",");
		List<String> oneTable = null;
		while (st.hasMoreTokens()) {
			String val = st.nextToken();
			if(val.startsWith("[") && val.endsWith("]")){
				oneTable = new ArrayList<String>();
				result.add(oneTable);
				oneTable.add(val.trim().substring(1, val.length() - 1));
			}else{
				oneTable.add(val.trim());
			}
		}
		}catch(Exception ex){
			throw new SQLException("The selected columns should like this,'[table],column1, coulmn2', reason:"+ex.getMessage());
		}
		
		return result;
	}

	private static String getValue(Object value, Integer columnType) throws SQLException{
		String result = null;
		if(value == null)
			return "";
		try{
		switch(columnType){
		case Types.BLOB:
		case Types.BINARY:
		case Types.LONGVARBINARY:
			BLOB blob = (BLOB)value;
			result = base64Encode(blob.getBinaryStream());
			break;
		case Types.CLOB:
//		case Types.NCLOB:
			CLOB clob = (CLOB)value;
			StringWriter sw = new StringWriter();
			BufferedReader br = new BufferedReader(clob.getCharacterStream());
			BufferedWriter bw = new BufferedWriter(sw);
			String data = null;
			while((data = br.readLine()) != null)
				bw.write(data);
			bw.flush();
			result = sw.toString();
			break;
//		case Types.REF:
//			break;
		default:
			result = value.toString();
		}
		}catch(IOException ex){
			throw new SQLException("Unable to read data, reason:"+ex.getMessage());
		}
		return result;
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
		/*
		 * every time it get 3 chars, and encode them base on 3*8=4*6.
		 * һ�δ���3���ֽڣ�3*8 == 4*6 ������������������±���
		 * �÷����е�*&63,*&15,*&3�������������£� �������byte�������ʹ洢��64��ʽ���£�11111111
		 * �������byte�������ʹ洢��15��ʽ���£�1111���� 2^3 + 2^2 + 2^1 + 2^0 = 15
		 * ��&�������롱������������Ҫ���и�λ���������
		 */
		byte[] readByte = new byte[3];
		int byteRead = 0;
		while((byteRead = input.read(readByte)) != -1){
			if(byteRead == 3){
				i ++;
				// ��һ���ֽڣ�����Դ�ֽڵĵ�һ���ֽڴ���
				// ����Դ��һ�ֽ�������λ��ȥ����2λ����2λ���㡣
				// �ȣ�00 + ��6λ
				pos = (byte) ((readByte[0] >> 2) & 63);
				s.append(CODEC.charAt(pos));

				// �ڶ����ֽڣ�����Դ�ֽڵĵ�һ���ֽں͵ڶ����ֽ����ϴ���
				// �������£���һ���ֽڸ�6λȥ��������λ���ڶ����ֽ�������λ
				// ����Դ��һ�ֽڵ�2λ + Դ��2�ֽڸ�4λ
				pos = (byte) (((readByte[0] & 3) << 4) + ((readByte[1] >> 4) & 15));
				s.append(CODEC.charAt(pos));

				// �������ֽڣ�����Դ�ֽڵĵڶ����ֽں͵������ֽ����ϴ���
				// ����ڶ����ֽ�ȥ����4λ��������λ���ø�6λ�����������ֽ�����6λ��ȥ����6λ���õ�2λ������Ӽ���
				pos = (byte) (((readByte[1] & 15) << 2) + ((readByte[2] >> 6) & 3));
				s.append(CODEC.charAt(pos));

				// ���ĸ��ֽڣ�����Դ�����ֽ�ȥ����2λ����
				pos = (byte) (((readByte[2]) & 63));
				s.append(CODEC.charAt(pos));
				// ����base64�ı������ÿ76���ַ���Ҫһ������
				// 76/4 = 19
				if((i % 19) == 0){
					s.append("\n");
				}
			}else if(byteRead == 2){
				pos = (byte) ((readByte[0] >> 2) & 63);
				s.append(CODEC.charAt(pos));

				pos = (byte) (((readByte[0] & 3) << 4) + ((readByte[1] >> 4) & 15));
				s.append(CODEC.charAt(pos));

				pos = (byte) ((readByte[1] & 15) << 2);
				s.append(CODEC.charAt(pos));

				s.append("=");
			}if(byteRead == 1){
				// �ֳ���һ��������λ��ǰ6λ��������λ���õ�һ����8λ
				pos = (byte) ((readByte[0] >> 2) & 63);
				s.append(CODEC.charAt(pos));
				// �������3�ߵĸ�λ���ֳ�8λ�ĺ���λ��Ȼ������4λ���õ�һ����8λ
				pos = (byte) ((readByte[0] & 3) << 4);
				s.append(CODEC.charAt(pos));

				s.append("==");
			}
		}
		return s.toString();
	}
	
	private static void saveToTemporary(Document document, CLOB clob) throws TransformerException, SQLException, IOException {
		TransformerFactory tf = TransformerFactory.newInstance();
		Transformer transformer = tf.newTransformer();
		DOMSource source = new DOMSource(document);
		transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
		transformer.setOutputProperty(OutputKeys.INDENT, "yes");
		Writer writer = clob.getCharacterOutputStream();
		transformer.transform(source, new StreamResult(writer));
		writer.close();
	}
	
	private static String saveToStr(Document document)throws TransformerException, SQLException, IOException {
		TransformerFactory tf = TransformerFactory.newInstance();
		Transformer transformer = tf.newTransformer();
		DOMSource source = new DOMSource(document);
		transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
		transformer.setOutputProperty(OutputKeys.INDENT, "yes");
		Writer writer = new StringWriter();
		transformer.transform(source, new StreamResult(writer));
		//writer.close();
		return writer.toString();
	}
	
	private static String abstrString(String value) throws SQLException{
		if(value.length() < 4001)
			return value;
		else{
			try {
				MessageDigest alga = MessageDigest.getInstance("MD5");
				alga.update(value.getBytes());
				byte[] digesta = alga.digest();
				return new String(digesta);
			} catch (NoSuchAlgorithmException e) {
				throw new SQLException("Exception while get abstract information, reason: " + e.getMessage());
			}
		}
	}

}
