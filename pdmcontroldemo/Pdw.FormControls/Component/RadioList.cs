using System.Windows.Forms;
using Pdw.FormControls.Design;
using System.Runtime.Serialization;

namespace Pdw.FormControls
{
    /// <summary>
    /// Displays a ListBox in which a radio button is displayed to the left of each item.
    /// </summary>
    [DataContract]
    public class RadioList : ListBox
    {
        /// <summary>
        /// Ctor
        /// </summary>
        internal RadioList() { }

        /// <summary>
        /// Gets checkable control's html input object type.
        /// </summary>
        protected override InputTagType InputType
        {
            get { return InputTagType.Radio; }
        }

        /// <summary>
        /// Get a template control which be generated using a specified template item.
        /// </summary>
        /// <param name="index">The index of specified template item in its collection</param>
        /// <param name="item">A specified template item used for make a template control</param>
        /// <returns>Control which will be displayed in the ListBox</returns>
        protected override Control GetTemplateItemControl(int index, TemplateItem item)
        {
            return new RadioButton 
            {
                AutoCheck = false,
                Text = item.Caption,
                Checked = item.Selected,
                AutoSize = true
            };
        }
    }
}
