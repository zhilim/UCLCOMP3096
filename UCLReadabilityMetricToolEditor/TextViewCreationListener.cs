using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCLReadabilityMetricToolEditor
{
    [ContentType("code")]
    [Export(typeof(IWpfTextViewCreationListener))]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    public class TextViewCreationListener : IWpfTextViewCreationListener
    {

        private List<ClassManager> classManagers = new List<ClassManager>();
                
        public void TextViewCreated(IWpfTextView textView)
        {
           
            String className = GetClassName(textView);
            Debug.WriteLine("TextView created is: " + className);
            ClassManager cm = new ClassManager(className,textView);
            classManagers.Add(cm);
            textView.Closed += textView_Closed;

        }

        void textView_Closed(object sender, EventArgs e)
        {
            for(int i = 0; i < classManagers.Count; i ++)
            {
                if(classManagers[i].GetTextView().Equals(sender as IWpfTextView))
                {
                    classManagers[i].Close();
                    classManagers.Remove(classManagers[i]);
                }
            }
        }

        private String GetClassName(IWpfTextView textView)
        {
            for (int i = 0; i < textView.TextSnapshot.LineCount; i ++)
            {
                //skip through imports and namespace lines.
                if(textView.TextSnapshot.GetLineFromLineNumber(i).GetText().Contains("namespace"))
                {
                    i++;
                    for(int j = i; j < textView.TextSnapshot.LineCount; j ++)
                    {
                        //get class declaration, trim whitespace and add it to list.
                        if(textView.TextSnapshot.GetLineFromLineNumber(j).GetText().Contains("class"))
                        {
                            Debug.WriteLine("Got name of class: " + textView.TextSnapshot.GetLineFromLineNumber(j).GetText().Trim());
                            return textView.TextSnapshot.GetLineFromLineNumber(j).GetText().Trim();
                        }
                    }
                    break;
                }
                
            }
            throw new Exception("Could not get class name.");     
        }
    }
}
