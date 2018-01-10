using System.Runtime.Serialization;
using System.Windows.Forms;
using Microsoft.Office.Tools.Word;
using Pdw.FormControls.Design;
using Word = Microsoft.Office.Interop.Word;

namespace Pdw.FormControls
{
    /// <summary>
    /// Represents a radio button control
    /// </summary>
    [DataContract]
    public class Radio : CheckableControl
    {
        /// <summary>
        /// Ctor
        /// </summary>
        internal Radio() { }

        /// <summary>
        /// Gets checkable control's html input object type.
        /// </summary>
        protected override InputTagType InputType { get { return InputTagType.Radio; } }

        /// <summary>
        /// Attach the control to a specified document's control collection using a specified process.
        /// </summary>
        /// <param name="collection">The control collection to hold the control.</param>
        /// <param name="range">The location of the control.</param>
        /// <param name="width">The width of the control.</param>
        /// <param name="height">The height of the control.</param>
        /// <param name="shape">The control's inline shape object in the document</param>
        /// <returns>The component of the attached control.</returns>
        protected override Control Attach(ControlCollection collection, Word.Range range, float width, float height, out Word.InlineShape shape)
        {
            //adds a radio button control to the document 
            var control = collection.AddRadioButton(range, width, height, ObjectId);

            control.AutoCheck = false;
            control.AutoSize = true;

            shape = control.InlineShape;

            return control;
        }
    }
}
