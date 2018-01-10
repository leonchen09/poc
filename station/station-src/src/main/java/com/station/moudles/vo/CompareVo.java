package com.station.moudles.vo;

public class CompareVo {

	private String before;
	private String after;
	private String same;
	private String otherBefore;
	private String otherAfter;

	public String getBefore() {
		return before;
	}

	public void setBefore(String before) {
		this.before = before;
	}

	public String getAfter() {
		return after;
	}

	public void setAfter(String after) {
		this.after = after;
	}

	public String getSame() {
		return same;
	}

	public void setSame(String same) {
		this.same = same;
	}

	public String getOtherBefore() {
		return otherBefore;
	}

	public void setOtherBefore(String otherBefore) {
		this.otherBefore = otherBefore;
	}

	public String getOtherAfter() {
		return otherAfter;
	}

	public void setOtherAfter(String otherAfter) {
		this.otherAfter = otherAfter;
	}

	public CompareVo() {
		super();
	}

	public CompareVo(String before, String after, String same, String otherBefore, String otherAfter) {
		super();
		this.before = before;
		this.after = after;
		this.same = same;
		this.otherBefore = otherBefore;
		this.otherAfter = otherAfter;
	}
}
