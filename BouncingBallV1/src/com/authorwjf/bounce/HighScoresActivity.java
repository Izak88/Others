package com.authorwjf.bounce;

import java.util.Map;

import android.app.Activity;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.TextView;

public class HighScoresActivity extends Activity {
	
	@Override
    public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
        setContentView(R.layout.highscores);
        
        
        SharedPreferences prefs = getSharedPreferences("highScores", GameActivity.MODE_PRIVATE);       
        Map<String, ?> allEntries = prefs.getAll();
        
        TextView textView = (TextView) findViewById(R.id.textView3);
        textView.setText("");
        textView.setLines(allEntries.size());
        
        for (int nI = 0; nI < allEntries.size(); nI++)
        {
            textView.append(prefs.getString(String.valueOf(nI + 1), "") + "\n");
        } 
    }

}
