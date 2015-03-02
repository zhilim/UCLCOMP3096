using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TETCSharpClient;
using TETCSharpClient.Data;
using System.Diagnostics;
using System.Windows;
using TETControls.Calibration;
using MessageBox = System.Windows.MessageBox;
using Microsoft.VisualStudio.Text.Editor;

namespace UCLReadabilityMetricToolEditor
{
    public class EyeTracker : IGazeListener
    {
        private Point eyePoint;
        private IWpfTextView view;
        private int eyeOnLine = 0;
        private ArrayList GazeDump = new ArrayList();
        private static int OFF_X = 50;
        private static int OFF_Y = 100;
        private static int GAZE_ROUND = 50;
       
        public EyeTracker(IWpfTextView inview)
        {
                ///<summary>
                ///connect to TET server. note: SERVER MUST BE RUNNING AND TET PRE-CALIBRATED
                ///Also, check trackbox before starting simulation
                ///Future implementation: integrate trackbox into VS (currently i find it impossible)
                ///Future implementation: automate calibration process (but this requires trackbox)
                ///</summary>

                GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);
                GazeManager.Instance.AddGazeListener(this);   
                view = inview;
    
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            //Get Gaze data
            double gX = gazeData.SmoothedCoordinates.X;
            double gY = gazeData.SmoothedCoordinates.Y;

            //Smoothen data for more meaningful results.
            double eX = roundNumber(gX, GAZE_ROUND) - OFF_X;
            double eY = roundNumber(gY, GAZE_ROUND) - OFF_Y;

            //prevent negative values
            if (eX < 0)
                eX = 0;
            if (eY < 0)
                eY = 0;

            eyePoint.X = eX;
            eyePoint.Y = eY;

            eyeOnLine = (int) (Math.Round(eY / view.LineHeight) + 1);

        }

        private double roundNumber(double num, int roundby)
        {
            return (Math.Round(num / roundby) * roundby);
            
        }


        internal class GazeRecord
        {
            public Point point;
            public int line;
            public int time = 1;

            public GazeRecord(Point pt, int eline)
            {
                point = pt;
                line = eline;
            }

            public void increment()
            {
                time++;
            }

            public void print()
            {
                Debug.WriteLine("eX: " + point.X + " eY: " + point.Y + " line: " + line + " Time: " + time);
            }
        }

        public void updateGazeDump()
        {
            bool exists = false;
            foreach (GazeRecord g in GazeDump)
            {
                if (g.point == eyePoint)
                {
                    g.increment();
                    g.print();
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                GazeRecord newRecord = new GazeRecord(eyePoint, eyeOnLine);
                GazeDump.Add(newRecord);
                newRecord.print();
            }
        }

        public void outputGazeDump(StreamWriter tw)
        {
            foreach (GazeRecord g in GazeDump)
            {
                tw.WriteLine("eX:" + g.point.X + ", eY:" + g.point.Y + ", line:" + g.line + ", Time:" + g.time);
            }
        }



        
    }
}
