using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace UCLReadabilityMetricToolEditor
{
    internal static class UCLReadabilityMetricToolEditorClassificationDefinition
    {
        /// <summary>
        /// Defines the "UCLReadabilityMetricToolEditor" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("UCLReadabilityMetricToolEditor")]
        internal static ClassificationTypeDefinition UCLReadabilityMetricToolEditorType = null;
    }
}
