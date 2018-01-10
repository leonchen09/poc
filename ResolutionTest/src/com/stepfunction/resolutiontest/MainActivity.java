package com.stepfunction.resolutiontest;

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.content.res.Resources;   

import android.graphics.Color;   
import android.graphics.drawable.Drawable;   
import android.os.Bundle;   
import android.util.DisplayMetrics;   
import android.widget.TextView;   

public class MainActivity extends Activity {


	 /** Called when the activity is first created. */  
    @Override  
    public void onCreate(Bundle savedInstanceState) {   
        super.onCreate(savedInstanceState);   
        setContentView(R.layout.activity_main);       
        TextView tv = (TextView)findViewById(R.id.textView1);   
        Resources resources = getBaseContext().getResources();   
        Drawable drawable = resources.getDrawable(R.drawable.ic_launcher);   
        tv.setBackgroundDrawable(drawable);   
        tv.setTextColor(Color.GREEN);     
        DisplayMetrics dm = new DisplayMetrics();   
        getWindowManager().getDefaultDisplay().getMetrics(dm);   
        tv.setText("屏幕分辨率为:"+dm.widthPixels+" * "+dm.heightPixels);   
    }

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.activity_main, menu);
		return true;
	}

}
