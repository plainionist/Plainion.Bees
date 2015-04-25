using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Plainion.WhiteRabbit.Presentation
{
    internal class Recorder
    {
        private Timer myElapsedTimer = null;
        private Timer myJitterTimer = null;
        private DateTime myStartTime;
        private int myJitterSeconds = 0;
        private int myElapsedSeconds = 0;
        private bool myPaused = false;
        private Channel myChannel = null;

        public Recorder( Channel channel )
        {
            myChannel = channel;

            myElapsedTimer = new Timer();
            myElapsedTimer.Enabled = false;
            myElapsedTimer.Interval = 1000;
            myElapsedTimer.Tick += myElapsedTimer_OnTick;

            myJitterTimer = new Timer();
            myJitterTimer.Enabled = false;
            myJitterTimer.Interval = 1000;
            myJitterTimer.Tick += myJitterTimer_OnTick;
        }

        public DateTime StartTime
        {
            get
            {
                return myStartTime;
            }
        }

        public DateTime StopTime
        {
            get
            {
                return myStartTime + Elapsed + Jitter;
            }
        }

        public TimeSpan Elapsed
        {
            get
            {
                return new TimeSpan( 0, 0, myElapsedSeconds );
            }
        }

        public TimeSpan Jitter
        {
            get
            {
                return new TimeSpan( 0, 0, myJitterSeconds );
            }
        }

        public void Start()
        {
            Start( DateTime.Now, null );
        }

        public void Start( DateTime start, TimeSpan? jitter )
        {
            if ( !myPaused )
            {
                myStartTime = start;
                if ( jitter == null )
                {
                    myJitterSeconds = 0;
                }
                else
                {
                    myJitterSeconds = (int)jitter.Value.TotalSeconds;
                }
                myElapsedSeconds = (int)( DateTime.Now - start ).TotalSeconds;

                myChannel.OnTimeElapsedChanged( Elapsed );
                myChannel.OnJitterChanged( Jitter );
            }
            myPaused = false;

            myJitterTimer.Enabled = false;
            myElapsedTimer.Enabled = true;
        }

        public void Stop()
        {
            myPaused = false;
            myElapsedTimer.Enabled = false;
            myJitterTimer.Enabled = false;
        }

        public void Pause()
        {
            myPaused = true;

            myElapsedTimer.Enabled = false;
            myJitterTimer.Enabled = true;
        }

        private void myElapsedTimer_OnTick( object sender, EventArgs e )
        {
            ++myElapsedSeconds;

            myChannel.OnTimeElapsedChanged( Elapsed );
        }

        private void myJitterTimer_OnTick( object sender, EventArgs e )
        {
            ++myJitterSeconds;

            myChannel.OnJitterChanged( Jitter );
        }
    }
}
