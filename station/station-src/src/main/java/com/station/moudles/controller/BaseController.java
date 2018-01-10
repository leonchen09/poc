package com.station.moudles.controller;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.code.yanf4j.core.impl.AbstractController;
import com.station.common.Constant;
import com.station.common.utils.JsonUtil;
import com.station.common.utils.MyDateUtils;
import com.station.common.utils.jwt.JwtHelper;
import com.station.moudles.vo.AjaxResponse;
import org.apache.poi.EncryptedDocumentException;
import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.WebDataBinder;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.InitBinder;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.multipart.MultipartFile;


import springfox.documentation.annotations.ApiIgnore;
import sun.misc.BASE64Decoder;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import javax.validation.ConstraintViolation;
import javax.validation.Validation;
import javax.validation.Validator;
import javax.validation.ValidatorFactory;
import java.beans.PropertyEditorSupport;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Date;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

@ApiIgnore
@Controller
@CrossOrigin
public class BaseController {
    protected Logger logger = LoggerFactory.getLogger(getClass());

    protected HttpServletResponse response;
    protected HttpSession session;
    protected HttpServletRequest request;
    protected ObjectMapper jsonMapper = new ObjectMapper();
    private static Validator validator;
    protected String userId;

    static {
        ValidatorFactory factory = Validation.buildDefaultValidatorFactory();
        validator = factory.getValidator();
    }

    protected AjaxResponse validateBean(Object object, Class<?>... groups) {
        if (object instanceof Object[]) {
            Object[] objs = (Object[]) object;
            for (Object obj : objs) {
                AjaxResponse ajaxResponse = validateBean(obj, groups);
                if (ajaxResponse != null) {
                    return ajaxResponse;
                }
            }
            return null;
        } else if (object instanceof List) {
            List objList = (List) object;
            for (Object obj : objList) {
                AjaxResponse ajaxResponse = validateBean(obj, groups);
                if (ajaxResponse != null) {
                    return ajaxResponse;
                }
            }
            return null;
        }
        Set<ConstraintViolation<Object>> violations = new HashSet<ConstraintViolation<Object>>();
        violations = validator.validate(object, groups);
        if (!violations.isEmpty()) {
            String errorMsg = "";
            for (ConstraintViolation<Object> cv : violations) {
                errorMsg = errorMsg + "," + cv.getPropertyPath().toString() + " " + cv.getMessage();
            }
            AjaxResponse ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, errorMsg.substring(1));
            return ajaxResponse;
        }
        return null;
    }

    // @Resource(name = "memcachedClient")
    // private MemcachedClient memcachedClient;
    @ModelAttribute
    public void setReqAndRes(HttpServletRequest request, HttpServletResponse response) {
        this.request = request;
        this.response = response;
        this.session = request.getSession();
        String authorization = request.getHeader("Authorization");
        if (authorization != null && !authorization.trim().equals("")) {
            String token = authorization.substring(6);
            @SuppressWarnings("unchecked")
            Long st = new Date().getTime();
            Map resultMap = JwtHelper.verifyToken(token);
            Long et = new Date().getTime();
            String payload = authorization.split("\\.")[1];
            @SuppressWarnings("restriction")
            BASE64Decoder decoder = new BASE64Decoder();
            try {
                byte[] bs = decoder.decodeBuffer(payload);
                String bsStr = new String(bs);
                Map<String, String> bsMap = JsonUtil.readJson(bsStr, Map.class);
                userId = bsMap.get("aud");
                // response.setHeader("userId", userId);
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    // 重新格式化时间
    @InitBinder
    protected void initBinder(WebDataBinder binder) {
        binder.registerCustomEditor(Date.class, new MyDateEditor());
    }

    class MyDateEditor extends PropertyEditorSupport {
        @Override
        public void setAsText(String text) throws IllegalArgumentException {
            // SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd
            // HH:mm:ss");
            // Date date = null;
            // try {
            // date = format.parse(text);
            // } catch (ParseException e) {
            // format = new SimpleDateFormat("yyyy-MM-dd HH:mm");
            // try {
            // date = format.parse(text);
            // } catch (ParseException e1) {
            // format=new SimpleDateFormat("yyyy-MM-dd");
            // try {
            // date=format.parse(text);
            // } catch (ParseException e2) {
            // e2.printStackTrace();
            // }
            // }
            // }
            setValue(MyDateUtils.parseDate(text));
        }
    }
    // public boolean setMemchache(String key,Object value ) throws
    // TimeoutException, InterruptedException, MemcachedException{
    // return memcachedClient.set("CONSOLE_"+key,60*60,value);
    // }
    //
    // public Object getMemchache(String key) throws TimeoutException,
    // InterruptedException, MemcachedException{
    // return memcachedClient.get("CONSOLE_"+key);
    // }

    // @ApiIgnore
    @ResponseBody
    @RequestMapping(value = "/fileImport", method = RequestMethod.POST)
    public AjaxResponse<Object> fileImportHandler(@RequestParam MultipartFile file, Integer companyId) throws IOException, EncryptedDocumentException, InvalidFormatException {
        AjaxResponse<Object> ajaxResponse = new AjaxResponse<Object>(Constant.RS_CODE_ERROR, "文件导入失败");
        request.setAttribute(Constant.ERROR_REQUEST, ajaxResponse);
        if (!file.isEmpty()) {
            InputStream in = null;
            OutputStream out = null;
            File dir = new File("tmpFiles");
            if (!dir.exists()) {
                dir.mkdirs();
            }
            File serverFile = new File(dir.getAbsolutePath() + File.separator + System.currentTimeMillis());
            in = file.getInputStream();
            out = new FileOutputStream(serverFile);
            byte[] b = new byte[1024];
            int len = 0;
            while ((len = in.read(b)) > 0) {
                out.write(b, 0, len);
            }
            out.close();
            in.close();
            logger.info("Server File Location=" + serverFile.getAbsolutePath());
            parseFile(serverFile, ajaxResponse, companyId);
            return ajaxResponse;
        } else {
            ajaxResponse.setMsg("文件为空！");
            return ajaxResponse;
        }
    }

    public boolean parseFile(File file, AjaxResponse ajaxResponse, Integer companyId) throws EncryptedDocumentException, FileNotFoundException, InvalidFormatException, IOException {
        ajaxResponse.setCode(Constant.RS_CODE_SUCCESS);
        ajaxResponse.setMsg("上传成功");
        return true;
    }
}
