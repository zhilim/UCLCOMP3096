using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UCLReadabilityMetricToolEditor
{
    public class LineFrequency
    {
        private IWpfTextView view;
        private DateTime start;
        private DateTime end;

        /// <summary>
        /// The interval between samples (in milliseconds).
        /// </summary>
        private const int interval = 1000;

        private DispatcherTimer timer;

        //stores the start/end time of each session.
        private List<DateTime> startTimes;
        private List<DateTime> endTimes;

        //stores number of lines looked at in each session.
        private List<int[]> lineCounters;

        //current session's line counter
        private int[] currentLineCounter;

        public LineFrequency(IWpfTextView view, DateTime dt)
        {
            this.view = view;
            this.start = dt;
            startTimes = new List<DateTime>();
            endTimes = new List<DateTime>();
            lineCounters = new List<int[]>();
            currentLineCounter = new int[view.TextSnapshot.LineCount];
            SetupTimer();
        }

        public List<int[]> getLineCounters()
        {
            return lineCounters;
        }

        public List<DateTime> getStartTimes()
        {
            return startTimes;
        }

        public List<DateTime> getEndTimes()
        {
            return endTimes;
        }

        public int getNumberOfSessions()
        {
            return startTimes.Count;
        }

        private void SetupTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(interval);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        public void PauseTimer()
        {
            Debug.WriteLine("Timer paused. Logging details into list");

            //add them to list.
            startTimes.Add(start);
            DateTime now = DateTime.Now;
            endTimes.Add(now);
            lineCounters.Add(currentLineCounter);

            //clear variables
            start = new DateTime();
            end = new DateTime();

            //stop timer
            timer.Stop();

            //reinitialise the linecounter
            currentLineCounter = new int[view.TextSnapshot.LineCount];
        }

        public void ResumeTimer()
        {
            Debug.WriteLine("Timer resumed.");
            start = DateTime.Now;
            timer.Start();
        }


        /// <summary>
        /// Every second, get the first and last index of lines on screen, increment lines accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            int firstVisibleLine = view.TextSnapshot.GetLineNumberFromPosition(view.TextViewLines.FirstVisibleLine.Start);
            int lastVisibleLine = view.TextSnapshot.GetLineNumberFromPosition(view.TextViewLines.LastVisibleLine.Start);
            Debug.WriteLine("First visible line = " + firstVisibleLine);
            Debug.WriteLine("Last visible line = " + lastVisibleLine);
            for (int i = firstVisibleLine; i <= lastVisibleLine; i++)
            {
                currentLineCounter[i]++;
            }
        }

        public void setIWpfTextView(IWpfTextView textView)
        {
            view = textView;
        }

        public void startTimer()
        {
            timer.Start();
        }

        public void setStartDateTime(DateTime start)
        {
            this.start = start;
        }


        public void setEndDateTime(DateTime end)
        {
            this.end = end;
        }

        public void stopTimer()
        {
            timer.Stop();
        }

        /// <summary>
        /// Ends a tracking session. Writes collected data to a text file.
        /// </summary>
        /// <param name="now"></param>
        public void endTrackingSession(DateTime now)
        {
            

            //make directory with name of datetime (if not already created)
            //inside, create file with class name (if not already created)
            //append data to it, as CSV

            
        }
    }
}
