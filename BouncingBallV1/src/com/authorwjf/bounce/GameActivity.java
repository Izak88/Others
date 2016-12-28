package com.authorwjf.bounce;

import java.util.Map;
import java.util.Set;

import android.os.Bundle;
import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.text.Editable;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.widget.EditText;

public class GameActivity extends Activity {

	public AnimatedImageView myView;
	public String sUserName;
	public int position;
	public Editor editor;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        
    	super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        myView = (AnimatedImageView)findViewById(R.id.anim_view);	// Pridobimo referenco na pogled GameView, ki smo ga dodali v vmesnik
        sUserName = "";
        position = -1;
        
       // myView.setFocusable(true);	// Omogocimo pridobivanje fokusa
        //myView.setFocusableInTouchMode(true);	// Omogocimo pridobivanje fokusa v primeru dotika
        //myView.requestFocus();	// Zahtevamo fokus
    	
        myView.setOnTouchListener(touchListener);	// Dodamo poslusalca dotika na nas pogled
    }

	private OnTouchListener touchListener = new OnTouchListener() {
    	@Override
    	public boolean onTouch(View v, MotionEvent event) {
	    	int action = event.getAction();
	    	switch (action) {
	    	  case MotionEvent.ACTION_DOWN:
	    		for(int nI = 0; nI < ((AnimatedImageView)v).x.length; nI++)
	    		{
	    			if(((AnimatedImageView)v).x[nI] < event.getX() && ((AnimatedImageView)v).x[nI] + ((AnimatedImageView)v).ballBitmap[nI].getWidth() > event.getX())
		    		{
		    			if(((AnimatedImageView)v).y[nI] < event.getY() && ((AnimatedImageView)v).y[nI] + ((AnimatedImageView)v).ballBitmap[nI].getHeight() > event.getY())
			    		{
			    			((AnimatedImageView)v).ChangeVelocity(nI);
			    		}
		    		}	
	    		}	    		  
	    	    break;
	    	}
	    	return true;
    	}
    };
    
    protected void Destroy()
    {  	
    	//getting preferences
    	SharedPreferences prefs = getSharedPreferences("highScores", GameActivity.MODE_PRIVATE);    	
    	Map<String, ?> allEntries = prefs.getAll();
    	position = allEntries.size();

    	if(position > 0){
    		String[] nScoreTmpParts = ((String)allEntries.get(String.valueOf(position))).split(" ");
    		while(Integer.parseInt(nScoreTmpParts[nScoreTmpParts.length - 1]) < myView.timer/30){
    			position--;
    			
    			if(!(position > 0)){
    				break;
    			}
    			
    			nScoreTmpParts = ((String)allEntries.get(String.valueOf(position))).split(" ");
    		}
    		    		
    		if(position < 10)
    		{
    			editor = prefs.edit();
    			
    			int nI = allEntries.size();
    			if(nI < 10)
    			{
    				nI++;
    			}
    			
    			while(nI > position + 1)
				{
    				editor.putString(String.valueOf(nI), (String)allEntries.get(String.valueOf(nI - 1)));
    				nI--;
    			}
    			
    			position++;
    			SaveNewHighScore();
    		}
    		else
    		{
    			myView.clearAnimation();
    	    	GameActivity.this.finish();
    		}
    	}
    	else{
    		editor = prefs.edit();
    		position = 1;
        	SaveNewHighScore();
    	}    	
    	
    }
    
    
    private void SaveNewHighScore(){
    	AlertDialog.Builder alert = new AlertDialog.Builder(this);

    	alert.setTitle("Title");
    	alert.setMessage("Message");
    	
    	// Set an EditText view to get user input 
    	final EditText input = new EditText(this);
    	alert.setView(input);
    	
    	alert.setPositiveButton("Ok", new DialogInterface.OnClickListener() {
		public void onClick(DialogInterface dialog, int whichButton) {
			sUserName = input.getText().toString(); 
			
			editor.putString(String.valueOf(position), sUserName + " " + String.valueOf(myView.timer/30));
			editor.commit();
			
			myView.clearAnimation();
	    	GameActivity.this.finish();
		  }
		});

		alert.setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
		  public void onClick(DialogInterface dialog, int whichButton) {
			  myView.clearAnimation();
		      GameActivity.this.finish();
		  }
		});

		alert.show();
    }
}
