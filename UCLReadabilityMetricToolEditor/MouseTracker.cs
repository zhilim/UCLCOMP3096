using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.ComponentModel.Composition;

namespace UCLReadabilityMetricToolEditor
{
    public class MouseTracker
        ///<summary>
        ///This class encapsulates the data, internal classes, and methods necessary
        ///for the tracking of mouse coordinates and the logging of mouse coordinates
        ///and time spent per coordinate. Logging is done per second using the timer 
        ///instantiated in lineFrequency class. Lots of static variables are used as
        ///this is necessary for access from within the MouseBase class. Mouse is tracked
        ///relative to the entire document(including text outside viewport), with the top left corner being (0,0)
        ///and the bottom right corner(scrolled all the way to the bottom) being the maximal values of X and Y. 
        ///</summary>
    {
        private static IWpfTextView view;
        private static Point mousePos;
        private ArrayList mouseRecordDump = new ArrayList();
        private static bool timerOn = false;

        public MouseTracker(IWpfTextView inview)
        {
            view = inview;
        }

        /// <summary>
        /// A point is represented by a pair of X,Y doubles
        /// A mouseRecord is simply a pair of point and the time the cursor spent on that point.
        /// </summary>
        internal class MouseRecord
        {
            public Point point;
            public int time = 1;

            public MouseRecord(Point pt)
            {
                point = pt;
            }

            public void increment()
            {
                time++;
            }
            public void print()
            {
                Debug.WriteLine("X: " + point.X + " Y: " + point.Y + " Time: " + time);
            }
        }

        public void setTimerOn(bool boolean)
        {
            timerOn = boolean;
        }

        public void printMouseRecordDump()
        {
            foreach (MouseRecord m in mouseRecordDump)
            {
                m.print();
            }
        }

        public void outputMouseDump(StreamWriter tw)
        {
            foreach (MouseRecord m in mouseRecordDump)
            {
                tw.WriteLine("X:" + m.point.X + ", Y:" + m.point.Y + ", Time:" + m.time);
            }
        }

        /// <summary>
        /// the array mouseRecordDump is the list of mouseRecords.
        /// </summary>
        public void updateMouseRecordDump()
        {
            bool exists = false;
            foreach (MouseRecord m in mouseRecordDump)
            {
                if (m.point == mousePos)
                {
                    m.increment();
                    m.print();
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                MouseRecord newRecord = new MouseRecord(mousePos);
                mouseRecordDump.Add(newRecord);
                newRecord.print();
            }
        }

        /// <summary>
        /// The main power behind mouse tracking resides in the remaining code
        /// while i do not fully understand the export, the internal class implementing
        /// MouseProcessorBase is apparently some form of MouseEventHandler. 
        /// </summary>
        [Export(typeof(IMouseProcessorProvider))]
        [Name("test mouse processor")]
        [ContentType("code")]
        [TextViewRole(PredefinedTextViewRoles.Interactive)]
        internal sealed class TestMouseProcessorProvider : IMouseProcessorProvider
        {
            public IMouseProcessor GetAssociatedProcessor(IWpfTextView view)
            {
                return new MouseBase();
            }

        }

        /// <summary>
        /// this mousehandlers complains that it cannot access variables native to the MouseTracker class that are not static
        /// therefore, lots of static variables are used. fix if possible?
        /// </summary>
        internal class MouseBase : MouseProcessorBase
        {
            public override void PreprocessMouseMove(System.Windows.Input.MouseEventArgs e)
            {
                base.PreprocessMouseMove(e);
                if (timerOn)
                {
                    mousePos = e.GetPosition((IInputElement) view);
                    //adjust Y values relative to entire text, not just viewport
                    mousePos.Y += view.LineHeight * (view.TextSnapshot.GetLineNumberFromPosition(view.TextViewLines.FirstVisibleLine.Start));
                    Debug.WriteLine("X: " + mousePos.X + " Y: " + mousePos.Y);

                }

            }

            public override void PreprocessMouseWheel(MouseWheelEventArgs e)
            {
                base.PreprocessMouseWheel(e);
                if (timerOn)
                {
                    mousePos = e.GetPosition((IInputElement)view);
                    mousePos.Y += view.LineHeight * (view.TextSnapshot.GetLineNumberFromPosition(view.TextViewLines.FirstVisibleLine.Start));
                    Debug.WriteLine("X: " + mousePos.X + " Y: " + mousePos.Y);
                }
            }
        }

    }
}
