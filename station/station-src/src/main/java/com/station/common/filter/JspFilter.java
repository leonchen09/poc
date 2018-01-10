package com.station.common.filter;

import java.io.IOException;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Servlet Filter implementation class JspFilter
 */
public class JspFilter implements Filter {

    /**
     * Default constructor. 
     */
    public JspFilter() {

    }

	/**
	 * @see Filter#destroy()
	 */
	public void destroy() {
	}

	/**
	 * @see Filter#doFilter(ServletRequest, ServletResponse, FilterChain)
	 */
	public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain) throws IOException, ServletException {
		HttpServletRequest req = (HttpServletRequest)request;
		String context = req.getContextPath();
		String requestUrl = req.getRequestURI();
		HttpServletResponse res = (HttpServletResponse)response;
		String remoteIp = request.getRemoteAddr();  
		System.out.println(remoteIp+requestUrl);
		if(requestUrl.indexOf(".jsp")!=-1){
			res.sendRedirect(context + "/login/loginIndex");
			return;
		}
		
		chain.doFilter(request, response);
	}

	/**
	 * @see Filter#init(FilterConfig)
	 */
	public void init(FilterConfig fConfig) throws ServletException {
	}

}
