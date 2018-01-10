package com.station.common.interceptor;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.validation.ConstraintViolation;
import javax.validation.Valid;
import javax.validation.Validation;
import javax.validation.ValidatorFactory;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.hibernate.validator.internal.engine.ValidatorImpl;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.core.MethodIntrospector;
import org.springframework.core.MethodParameter;
import org.springframework.util.Assert;
import org.springframework.validation.Validator;
import org.springframework.validation.annotation.Validated;
import org.springframework.validation.beanvalidation.LocalValidatorFactoryBean;
import org.springframework.web.bind.support.WebDataBinderFactory;
import org.springframework.web.context.request.ServletWebRequest;
import org.springframework.web.method.HandlerMethod;
import org.springframework.web.method.support.HandlerMethodArgumentResolver;
import org.springframework.web.method.support.InvocableHandlerMethod;
import org.springframework.web.method.support.ModelAndViewContainer;
import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.method.annotation.RequestMappingHandlerAdapter;
import org.springframework.web.servlet.mvc.method.annotation.ServletRequestDataBinderFactory;
import org.springframework.web.servlet.support.RequestContextUtils;

import com.station.common.Constant;
import com.station.common.utils.JsonUtil;
import com.station.moudles.vo.AjaxResponse;

/**
 * 添加Spring Controller 方法级别的Bean Validation的支持
 * valid表示验证参数
 * Validated表示验证bean
 * hibernate-validation具体实现验证
 * @author JiangFeng
 *
 */
public class ValidationInterceptor implements HandlerInterceptor {
	protected final Log logger = LogFactory.getLog(getClass());

	private List<HandlerMethodArgumentResolver> argumentResolvers;
	@Autowired
	private Validator validator;
	
	private static javax.validation.Validator beanValidator;
	
	@Autowired
	private RequestMappingHandlerAdapter adapter;

	private final Map<MethodParameter, HandlerMethodArgumentResolver> argumentResolverCache = new ConcurrentHashMap<MethodParameter, HandlerMethodArgumentResolver>(
			256);
	private final Map<Class<?>, Set<Method>> initBinderCache = new ConcurrentHashMap<Class<?>, Set<Method>>(64);

	@Autowired
	public ValidationInterceptor(RequestMappingHandlerAdapter requestMappingHandlerAdapter) {
		argumentResolvers = requestMappingHandlerAdapter.getArgumentResolvers();
	}
	
	static{
		ValidatorFactory factory = Validation.buildDefaultValidatorFactory();
		beanValidator = factory.getValidator();
	}

	@Override
	public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler)
			throws Exception {

		LocalValidatorFactoryBean validatorFactoryBean = (LocalValidatorFactoryBean) validator;
		ValidatorImpl validatorImpl = (ValidatorImpl) validatorFactoryBean.getValidator();
		ServletWebRequest webRequest = new ServletWebRequest(request, response);
		HandlerMethod method = (HandlerMethod) handler;
		Valid valid = method.getMethodAnnotation(Valid.class);
		Validated validated = method.getMethodAnnotation(Validated.class);
		if (valid != null || validated != null) {
			Class<?>[] groups = new Class<?>[0];
			MethodParameter[] parameters = method.getMethodParameters();
			Object[] parameterValues = new Object[parameters.length];
			for (int i = 0; i < parameters.length; i++) {
				MethodParameter parameter = parameters[i];
				HandlerMethodArgumentResolver resolver = getArgumentResolver(parameter);
				Assert.notNull(resolver, "Unknown parameter type [" + parameter.getParameterType().getName() + "]");
				ModelAndViewContainer mavContainer = new ModelAndViewContainer();
				mavContainer.addAllAttributes(RequestContextUtils.getInputFlashMap(request));
				WebDataBinderFactory webDataBinderFactory = getDataBinderFactory(method);
				Object value = resolver.resolveArgument(parameter, mavContainer, webRequest, webDataBinderFactory);
				parameterValues[i] = value;
			}
			Set<ConstraintViolation<Object>> violations=null;
			
			if(valid != null){
				violations = validatorImpl.validateParameters(method.getBean(),
						method.getMethod(), parameterValues, groups);				
			}else{				
				violations = beanValidator.validate(parameterValues[0]);
				if(parameterValues.length>1){
					boolean first=true;
					for(Object parameterValue:parameterValues){
						if(first){
							first=false;
						}else{
							violations.addAll(beanValidator.validate(parameterValue));
						}
					}
				}
			}
			
			if (!violations.isEmpty()) {
				String errorMsg = "";
				for (ConstraintViolation<Object> cv : violations) {
					if(valid!=null){
						errorMsg = errorMsg+"," + cv.getPropertyPath().toString().split("\\.")[1] + cv.getMessage();	
					}else{
						errorMsg = errorMsg+"," + cv.getPropertyPath().toString() + cv.getMessage();
					}					
				}
				// throw new ConstraintViolationException(violations);
				AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR,
						errorMsg.substring(1));
				response.getWriter().write(JsonUtil.writeValueAsString(ajaxResponse));
				return false;
			}			
		} 
		return true;
	}

	private WebDataBinderFactory getDataBinderFactory(HandlerMethod handlerMethod) throws Exception {
		Class<?> handlerType = handlerMethod.getBeanType();
		Set<Method> methods = this.initBinderCache.get(handlerType);
		if (methods == null) {
			methods = MethodIntrospector.selectMethods(handlerType, RequestMappingHandlerAdapter.INIT_BINDER_METHODS);
			this.initBinderCache.put(handlerType, methods);
		}
		List<InvocableHandlerMethod> initBinderMethods = new ArrayList<InvocableHandlerMethod>();
		for (Method method : methods) {
			Object bean = handlerMethod.getBean();
			initBinderMethods.add(new InvocableHandlerMethod(bean, method));
		}
		return new ServletRequestDataBinderFactory(initBinderMethods, adapter.getWebBindingInitializer());
	}

	private HandlerMethodArgumentResolver getArgumentResolver(MethodParameter parameter) {
		HandlerMethodArgumentResolver result = this.argumentResolverCache.get(parameter);
		if (result == null) {
			for (HandlerMethodArgumentResolver methodArgumentResolver : this.argumentResolvers) {
				if (logger.isTraceEnabled()) {
					logger.trace("Testing if argument resolver [" + methodArgumentResolver + "] supports ["
							+ parameter.getGenericParameterType() + "]");
				}
				if (methodArgumentResolver.supportsParameter(parameter)) {
					result = methodArgumentResolver;
					this.argumentResolverCache.put(parameter, result);
					break;
				}
			}
		}
		return result;
	}

	@Override
	public void postHandle(HttpServletRequest request, HttpServletResponse response, Object handler,
			ModelAndView modelAndView) throws Exception {

	}

	@Override
	public void afterCompletion(HttpServletRequest request, HttpServletResponse response, Object handler, Exception ex)
			throws Exception {

	}

}
