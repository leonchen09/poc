package com.stepfunction.prontodoc.mobile.background;

import java.io.IOException;

import android.os.Bundle;
import android.app.Activity;
import android.app.WallpaperManager;
import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.view.Menu;
import android.view.View;
import android.widget.TextView;

public class MainActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}
	
	public void button1Onclick(View v){
		TextView t = (TextView)findViewById(R.id.editText1);
		
		BitmapFactory.Options bfoOptions = new BitmapFactory.Options();
		bfoOptions.inScaled = false;
//		Bitmap bitmap = BitmapFactory.decodeResource(getResources(), R.drawable.splashimg, bfoOptions);
		Bitmap bitmap = BitmapFactory.decodeResource(getResources(), R.drawable.xpic5759, bfoOptions);
		
		WallpaperManager wallMgr = WallpaperManager.getInstance(this);
		wallMgr.setWallpaperOffsetSteps(0, 0);
		
		String info = "width:" + bitmap.getWidth();
		info = info + "; height:" + bitmap.getHeight();
		info += " wallpaper width:" + wallMgr.getDesiredMinimumWidth();
		info += "; wallpaper height:" + wallMgr.getDesiredMinimumHeight();
		t.setText(info);
		try {
			wallMgr.setBitmap(bitmap);
		} catch (Exception e) {
			
			t.setText(e.getMessage());
			e.printStackTrace();
		}
	}

}
