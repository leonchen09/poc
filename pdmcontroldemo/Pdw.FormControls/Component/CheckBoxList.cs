using System.Runtime.Serialization;
using System.Windows.Forms;
using Pdw.FormControls.Design;

namespace Pdw.FormControls
{
    /// <summary>
    /// Displays a ListBox in which a check box is displayed to the left of each item.
    /// </summary>
    [DataContract]
    public class CheckBoxList : ListBox
    {
        /// <summary>
        /// Ctor
        /// </summary>
        internal CheckBoxList() { }

        /// <summary>
        /// Gets checkable control's html input object type.
        /// </summary>
        protected override InputTagType InputType
        {
            get { return InputTagType.Checkbox; }
        }

        /// <summary>
        /// Get a template control which be generated using a specified template item.
        /// </summary>
        /// <param name="index">The index of specified template item in its collection</param>
        /// <param name="item">A specified template item used for make a template control</param>
        /// <returns>Control which will be displayed in the ListBox</returns>
        protected override Control GetTemplateItemControl(int index, TemplateItem item)
        {
            return new System.Windows.Forms.CheckBox
            {
                AutoCheck = false,
                Text = item.Caption,
                Checked = item.Selected,
                AutoSize = true
            };
        }
    }
}
