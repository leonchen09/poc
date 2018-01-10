package com.sofn.framework.web.tag;

import java.io.IOException;

import javax.servlet.jsp.JspException;
import javax.servlet.jsp.tagext.TagSupport;
/**
 * 
 * @author Chenwl
 * @date 2016年4月25日
 */
public class PageTag extends TagSupport{

	private static final long serialVersionUID = -7475006716988755213L;

	private int pageNo;   //ҳ��
	private int pageSize; //ÿҳ��¼��
	private int totalPage;//��ҳ��
	private String jsCallback; //js�ص���������������form�ύ������ajax�ȸ����ύģʽ��
	
	public int doStartTag() throws JspException {
	
		//�����ҳ���HTML�ı�
		StringBuilder sb = new StringBuilder();
	
		sb.append("<style type=\"text/css\">");
		sb.append(".pagination {padding: 5px;float:right;font-size:12px;}");
		sb.append(".pagination a, .pagination a:link, .pagination a:visited {padding:2px 5px;margin:2px;border:1px solid #aaaadd;text-decoration:none;color:#006699;}");
		sb.append(".pagination a:hover, .pagination a:active {border: 1px solid #ff0000;color: #000;text-decoration: none;}");
		sb.append(".pagination span.current {padding: 2px 5px;margin: 2px;border: 1px solid #ff0000;font-weight: bold;background-color: #ff0000;color: #FFF;}");
		sb.append(".pagination span.disabled {padding: 2px 5px;margin: 2px;border: 1px solid #eee; color: #ddd;}");
		sb.append("</style>\r\n");
		sb.append("<div class=\"pagination\">\r\n");
		
		//ҳ��Խ�紦��
		if(pageNo > totalPage){		
			pageNo = totalPage;	
		}
		if(pageNo < 1){	
			pageNo = 1;	
		}

		//�ѵ�ǰҳ�ţ�ÿҳ��¼�����ó����صĲ���
		sb.append("<input type=\"hidden\" id=\"pageNo\"  name=\"pageNo\"")
			.append(" value=\"").append(pageNo).append("\"/>\r\n");
		sb.append("<input type=\"hidden\" id=\"pageSize\" name=\"pageSize\"")
			.append(" value=\"").append(pageSize).append("\"/>\r\n");
		//��ҳ,��һҳ
		if (pageNo == 1) {
			sb.append("<span class=\"disabled\">&nbsp;��ҳ</>");
			sb.append("<span class=\"disabled\">&laquo;&nbsp;��һҳ")
				.append("</span>\r\n");
		} else {
			sb.append("<a href=\"javascript:turnOverPage(1)\"/>&nbsp;��ҳ</a>\r\n");
			sb.append("<a href=\"javascript:turnOverPage(")
			  .append((pageNo - 1))
			  .append(")\">&laquo;&nbsp;��һҳ</a>\r\n");
		}
		
		//��ǰҳ��
		sb.append("<span class=\"current\">")
		.append(pageNo)
		.append("</span>\r\n");	
		//��һҳ,ĩҳ����
		if (pageNo == totalPage) {
			sb.append("<span class=\"disabled\">��һҳ&nbsp;&raquo;")
				.append("</span>");
			sb.append("<span class=\"disabled\">&nbsp;ĩҳ</span>\r\n");
		} else {
			sb.append("<a href=\"javascript:turnOverPage(")
				.append((pageNo + 1))
				.append(")\">��һҳ&nbsp;&raquo;</a>");
			sb.append("<a href=\"javascript:turnOverPage(")
			.append(totalPage)
			.append(")\">&nbsp;ĩҳ</a>\r\n");
		}
		sb.append("</div>\r\n");
		
		//����JS����������ֵ��
		sb.append("<script language=\"javascript\">\r\n");
		sb.append("  function turnOverPage(no){\r\n");
		sb.append("    if(no>").append(totalPage).append("){").append(" no=").append(totalPage).append("; }\r\n");
		sb.append("    if(no<1){ no=1; }\r\n");
		sb.append("    document.getElementById(\"pageNo\").value=no;\r\n");
		sb.append("    document.getElementById(\"pageSize\").value=").append(pageSize).append(";\r\n");
		sb.append("    ").append(jsCallback).append(";\r\n");
		sb.append("  }\r\n");
		sb.append("</script>\r\n");
		
		//�����ɵ�HTML�������Ӧ��
		try {
			pageContext.getOut().println(sb.toString());
		} catch (IOException e) {
			throw new JspException(e);
		}
		return SKIP_BODY;  //����ǩ����Ϊ��,����ֱ����������
	}

	public void setPageNo(int pageNo) {
		this.pageNo = pageNo;
	}

	public void setPageSize(int pageSize) {
		this.pageSize = pageSize;
	}

	public void setTotalPage(int totalPage) {
		this.totalPage = totalPage;
	}

	public void setJsCallback(String jsCallback) {
		this.jsCallback = jsCallback;
	}

}
