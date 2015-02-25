using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace UCLReadabilityMetricToolEditor
{
    #region Format definition
    /// <summary>
    /// Defines an editor format for the UCLReadabilityMetricToolEditor type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "UCLReadabilityMetricToolEditor")]
    [Name("UCLReadabilityMetricToolEditor")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class UCLReadabilityMetricToolEditorFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "UCLReadabilityMetricToolEditor" classification type
        /// </summary>
        public UCLReadabilityMetricToolEditorFormat()
        {
            this.DisplayName = "UCLReadabilityMetricToolEditor"; //human readable version of the name
            this.BackgroundColor = Colors.BlueViolet;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
    #endregion //Format definition
}
