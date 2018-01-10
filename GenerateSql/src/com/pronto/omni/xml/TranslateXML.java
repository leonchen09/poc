package com.pronto.omni.xml;

import java.io.File; 

import javax.xml.transform.Result; 
import javax.xml.transform.Source; 
import javax.xml.transform.Transformer; 
import javax.xml.transform.TransformerConfigurationException; 
import javax.xml.transform.TransformerException; 
import javax.xml.transform.TransformerFactory; 
import javax.xml.transform.stream.StreamResult; 
import javax.xml.transform.stream.StreamSource; 

import org.xml.sax.XMLReader;


public class TranslateXML { 

    public static void main(String[] argv){ 
        String xmlFileName = "e:\\test2.xml"; 
        String xslFileName = "e:\\merge.xsl"; 
        String outputFileName = "e:\\final.xml"; 
//         String xmlFileName = "D:\\work\\ProntoDir\\xsl\\10946BFE90E74E68A71CFB18BC7D3077.xml"; 
//         String xslFileName = "D:\\work\\ProntoDir\\xsl\\10946BFE90E74E68A71CFB18BC7D3077.xsl"; 
//         String outputFileName = "D:\\work\\ProntoDir\\xsl\\1094.html"; 
         Transform(xmlFileName, xslFileName, outputFileName); 
     } 

    public static void Transform(String xmlFileName, String xslFileName, 
             String outputFileName) { 
        try { 
             TransformerFactory tFac = TransformerFactory.newInstance(); 
     
             Source template = new StreamSource(xslFileName);
             Transformer t = tFac.newTransformer(template); 
              
             File htmlFile = new File(outputFileName); 
             Result result = new StreamResult(htmlFile); 
             
             File xmlFile = new File(xmlFileName);
             Source data = new StreamSource(xmlFile);
             
//             System.out.println(result.toString()); 
             t.transform(data, result); 
         } catch (TransformerConfigurationException e) { 
             e.printStackTrace(); 
         } catch (TransformerException e) { 
             e.printStackTrace(); 
         } 
     } 
} 

