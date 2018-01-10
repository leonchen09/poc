package com.station.common.interceptor;

import java.util.Date;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.station.common.Constant;
import com.station.common.utils.HttpClientUtil;
import com.station.common.utils.JsonUtil;
import com.station.common.utils.jwt.JwtHelper;
import com.station.moudles.vo.AjaxResponse;

public class AuthorizationInterceptor implements HandlerInterceptor {
	private static final Logger log = LoggerFactory.getLogger(AuthorizationInterceptor.class);
	private String[] notInterceptUrls;

	public void setNotInterceptUrls(String[] notInterceptUrls) {
		this.notInterceptUrls = notInterceptUrls;
	}

	@Override
	public void afterCompletion(HttpServletRequest request, HttpServletResponse response, Object handler,
			Exception arg3) throws Exception {
		long startTime = (Long) request.getAttribute("startTime");
		long endTime = System.currentTimeMillis();
		long executeTime = endTime - startTime;
		String context = request.getContextPath() + "/";
		log.info("[" + request.getRequestURI().replace(context, "/") + "] executeTime : " + executeTime + "ms");
		 if(executeTime>1000){
			 log.warn("[" + request.getRequestURI().replace(context, "/") + "] executeTime : " + executeTime + "ms");
		 }
	}

	@Override
	public void postHandle(HttpServletRequest request, HttpServletResponse response, Object handler,
			ModelAndView modelandview) throws Exception {
	}

	@Override
	public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object arg2) throws Exception {

		long startTime = System.currentTimeMillis();
		request.setAttribute("startTime", startTime);

		String context = request.getContextPath() + "/";
		String requestUrl = request.getRequestURI().replace(context, "/");
						
//		response.setHeader("Access-Control-Allow-Credentials", "*");
//		response.setHeader("Access-Control-Allow-Origin", "*");
//		response.setHeader("Access-Control-Allow-Headers", "content-type,authorization");
		
		log.info(requestUrl);
        if (StringUtils.equalsIgnoreCase("GET", request.getMethod())) {
            return true;
        }
		if (requestUrl.startsWith("/system/sendAuthCode") || requestUrl.startsWith("/system/download")
				|| requestUrl.startsWith("/system/showPic")
				|| requestUrl.startsWith("/rolePermission/settingsRolePermission")
				|| requestUrl.startsWith("/token/get/")
				|| requestUrl.startsWith("/gprsBalanceSend/entity/send/")) {
			return true;
		}
		for (String notInterceptUrl : notInterceptUrls) {
			if (requestUrl.equals(notInterceptUrl)) {
				return true;
			}
		}

		String authorization = request.getHeader("Authorization") == null ? request.getParameter("Authorization"): request.getHeader("Authorization");

		String referer = request.getHeader("Referer");
		if (referer != null && !referer.trim().equals("")) {
			if (referer.endsWith("swagger-ui.html")) {
				response.setHeader("test", "test abc");
				return true;
			}
		}
		AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "Token验证错误--缺少token");

		if (authorization != null && !authorization.trim().equals("")) {
			String token = authorization.substring(6);
			@SuppressWarnings("unchecked")
			Long st = new Date().getTime();
			Map resultMap = JwtHelper.verifyToken(token);
			Long et = new Date().getTime();
			log.debug("Token验证耗时:" + (et - st) + "ms" + resultMap);
			if (resultMap.get("code").toString().equals("0")) {

				// String payload = authorization.split("\\.")[1];
				// @SuppressWarnings("restriction")
				// BASE64Decoder decoder = new BASE64Decoder();
				// try {
				// byte[] bs = decoder.decodeBuffer(payload);
				// String bsStr = new String(bs);
				// Map<String, String> bsMap = JsonUtil.readJson(bsStr, Map.class);
				// String userId = bsMap.get("aud");
				// response.setHeader("userId", userId);
				// } catch (IOException e) {
				// e.printStackTrace();
				// }

				return true;
			} else {
				System.out.println(resultMap);
				if (resultMap.get("code").toString().equals("1") && requestUrl.equals("/token/renew")) {
					return true;
				}
				ajaxResponse = new AjaxResponse(resultMap.get("code").toString(), "Token验证错误！");

			}
		}
		response.setHeader("Content-type", "text/html;charset=UTF-8");
		response.getWriter().write(JsonUtil.writeValueAsString(ajaxResponse));
		return true; 
	}
}
