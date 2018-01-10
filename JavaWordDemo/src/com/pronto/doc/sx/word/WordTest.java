package com.pronto.doc.sx.word;

public class WordTest {

	private static String fileName = "E:\\ProntoDir\\CompareXml2003and2007format\\Mhttest.docx";
//	private static String fileName = "E:\\ProntoDir\\CompareXml2003and2007format\\2007 Format.docx";
	/**
	 * @param args
	 * @throws Exception 
	 */
	public static void main(String[] args) throws Exception {
		addWaterMark();
	}

	public static void addWaterMark() throws Exception{
		WordHelper wordHelper = new WordHelper(false);
//		wordHelper.setSaveOnExit(true);
		wordHelper.openDocument(fileName);
		wordHelper.setWaterMark("Test by java");
		wordHelper.highLightBookmarks(0, 7);//black for fontcolor, and yellow for backgoundcolor.
//		wordHelper.saveAsPdf("E:\\ProntoDir\\CompareXml2003and2007format\\watermarkpdf.pdf");
//		wordHelper.saveAsMht("E:\\ProntoDir\\CompareXml2003and2007format\\watermarkmht.mht");
		wordHelper.setSaveOnExit(true);
		wordHelper.closeDocument();
		wordHelper.close();
	}
	
	public static void testPefermance() throws Exception{
		WordHelper wordHelper = new WordHelper(false);
//		wordHelper.setSaveOnExit(true);
		long startTime = System.currentTimeMillis();
		int i = 0;
		for(i = 0; i < 100; i ++){
			wordHelper.openDocument(fileName);
			wordHelper.setWaterMark("Test by java");
			wordHelper.highLightBookmarks(0, 7);
			wordHelper.saveAsPdf("E:\\ProntoDir\\CompareXml2003and2007format\\newpdf"+ i +".pdf");
			wordHelper.closeDocument();
		}
		long endTime = System.currentTimeMillis();
		System.out.println("Fininshed. Total time:" + (endTime - startTime));
	}
}
