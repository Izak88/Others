package com.authorwjf.bounce;

import java.util.Arrays;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Paint.Style;
import android.graphics.drawable.BitmapDrawable;
import android.os.Handler;
import android.os.SystemClock;
import android.os.Vibrator;
import android.util.AttributeSet;
import android.widget.ImageView;

public class AnimatedImageView extends ImageView{

	public int[] x = { -1 };
	public int[] y = { -1 };
	public int[] xVelocity = { 10 };
	public int[] yVelocity = { 10 };
	public int timer = 0;
	public Bitmap[] ballBitmap;
	private Handler h;
	private Context mContext;
	private final int FRAME_RATE = 20;
	private Paint pTimer;
	private Paint pGameOver;
	
	public AnimatedImageView(Context context, AttributeSet attrs)  {  
		super(context, attrs);  
		mContext = context;  
		h = new Handler();
		
		ballBitmap = new Bitmap[] { Bitmap.createScaledBitmap(((BitmapDrawable) mContext.getResources().getDrawable(R.drawable.ball)).getBitmap(), 150, 150, true) };
		
		pTimer = new Paint();
		pTimer.setColor(Color.WHITE);
		pTimer.setStyle(Style.FILL);
		pTimer.setTextSize(50);
		
		pGameOver = new Paint();
		pGameOver.setColor(Color.RED);
		pGameOver.setStyle(Style.FILL);
		pGameOver.setTextSize(100);
    } 
	
	protected Runnable r = new Runnable() {
		@Override
		public void run() {
			invalidate(); 
		}
	};
	
	protected void onDraw(Canvas c) 
	{
	    c.drawText(String.valueOf(timer/30), 100, 100, pTimer);
	    timer++;
	    
	    for(int nI = 0; nI < ballBitmap.length; nI++)
	    {
	    	x[nI] += xVelocity[nI];
	    	y[nI] += yVelocity[nI];
	    	
	    	if (x[nI] > this.getWidth() - ballBitmap[nI].getWidth() || x[nI] < 0) 
	    	{
	    		xVelocity[nI] = xVelocity[nI] * -1;
	    	}
	    	
	    	if (y[nI] <= 0) 
	    	{
	    		c.drawText("GameOver", this.getWidth() / 2 - 50, this.getHeight() / 2 - 50, pGameOver);

	    		Vibrator v = (Vibrator) mContext.getSystemService(Context.VIBRATOR_SERVICE);
	    		 v.vibrate(500);
	    		
	    		h.removeCallbacks(r);
	    		
	    		SystemClock.sleep(1000);
	    		Discard();
	    		return;
	    	}
	    	else if (y[nI] > this.getHeight() - ballBitmap[nI].getHeight())
	    	{
	    		yVelocity[nI] = yVelocity[nI] * -1;
	    	}
			
		    c.drawBitmap(ballBitmap[nI], x[nI], y[nI], null);  	    
		    
		    if (timer % 30 == 0)
		    {
		    	yVelocity[nI] += yVelocity[nI] / Math.abs(yVelocity[nI]);
		    	xVelocity[nI] += xVelocity[nI] / Math.abs(xVelocity[nI]);
		    }
	    }
	    
	    h.postDelayed(r, FRAME_RATE);
	    
	    if(timer % 300 == 0)
	    {
	    	addNewBall();
	    }
	}
	
	public void Discard()
	{
		GameActivity startActivity = (GameActivity)mContext;
		startActivity.Destroy();
	}
	
	private void addNewBall()
	{
		yVelocity  = Arrays.copyOf(yVelocity, yVelocity.length + 1);
		yVelocity[yVelocity.length - 1] = 10;
		
		xVelocity  = Arrays.copyOf(xVelocity, xVelocity.length + 1);
		xVelocity[xVelocity.length - 1] = 10;
		
		x = Arrays.copyOf(x, x.length + 1);
		x[x.length - 1] = 10;
		
		y = Arrays.copyOf(y, y.length + 1);
		y[y.length - 1] = 10;
		
		ballBitmap = Arrays.copyOf(ballBitmap, ballBitmap.length + 1);
		ballBitmap[ballBitmap.length - 1] = Bitmap.createScaledBitmap(((BitmapDrawable) mContext.getResources().getDrawable(R.drawable.ball)).getBitmap(), 120, 120, true);
	}
	
	protected void ChangeVelocity(int p_I)
	{
		xVelocity[p_I] = -xVelocity[p_I];
		yVelocity[p_I] = -yVelocity[p_I];
	}
	
}
