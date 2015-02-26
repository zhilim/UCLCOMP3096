using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCLReadabilityMetricToolEditor
{

    /// <summary>
    /// Manages the analytics for a C# document.
    /// </summary>
    public class ClassManager
    {

        private String className = "";
        private IWpfTextView textView;
        private LineFrequency lineFrequency;

        public ClassManager(String className, IWpfTextView textView)
        {
            this.className = className;
            this.textView = textView;
            DateTime dt = DateTime.Now;
            lineFrequency = new LineFrequency(textView, dt);
            SubscribeToListeners();
        }

        public IWpfTextView GetTextView()
        {
            return textView;
        }

        public String GetClassName()
        {
            return className;
        }

        private void SubscribeToListeners()
        {
            textView.GotAggregateFocus += textView_GotAggregateFocus;
            textView.LostAggregateFocus += textView_LostAggregateFocus;
        }

        /// <summary>
        /// Document has been closed, so stop tracking.
        /// </summary>
        /// 

        //NOTE, NEEDS REFACTORING AND EDITING.
        public void Close()
        {
            lineFrequency.PauseTimer();
            DateTime now = DateTime.Now;
            Debug.WriteLine("Dumping results into text file");
            //create directory to store results.
            String dateTime = now.ToLongDateString() + "_" + now.ToLongTimeString();
            className = className.Replace(":", "-");
            className = className.Replace("/", "--");

            dateTime = dateTime.Replace(":", "-");
            dateTime = dateTime.Replace("/", "--");
            Directory.CreateDirectory("/" + className);

            String path = "/" + className + "/" + dateTime + ".txt";

            using(StreamWriter tw = new StreamWriter(path,true))
            {
                for (int i = 0; i < lineFrequency.getNumberOfSessions()-1; i++)
                {
                    tw.WriteLine("Start session:" + lineFrequency.getStartTimes()[i]);
                    tw.WriteLine("-----");
                    for (int j = 0; j < lineFrequency.getLineCounters()[i].Count(); j++)
                    {
                        tw.WriteLine(j + 1 + "," + lineFrequency.getLineCounters()[i][j]);
                    }
                    lineFrequency.getMouseTracker().outputMouseDump(tw);
                    tw.WriteLine("-----");
                    tw.WriteLine("End session:" + lineFrequency.getEndTimes()[i]);
                    tw.WriteLine("----------");
                }
                tw.Close();
            }

            lineFrequency.getMouseTracker().printMouseRecordDump();

               
        }

        /// <summary>
        /// When focus on the document is lost, this event is triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textView_LostAggregateFocus(object sender, EventArgs e)
        {
            //pause tracking.
            Debug.WriteLine(className + " lost focus!");
            lineFrequency.PauseTimer();
        }

        /// <summary>
        /// When focus on the document is obtained, this event is triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void textView_GotAggregateFocus(object sender, EventArgs e)
        {
            //resume tracking.
            Debug.WriteLine(className + " got focus!");
            lineFrequency.ResumeTimer();
        }
        
        /// <summary>
        /// When the document is closed, this event is triggered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

    }
}
