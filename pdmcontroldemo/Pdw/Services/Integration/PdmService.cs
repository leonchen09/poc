using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;
using Word = Microsoft.Office.Interop.Word;
using VstoWord = Microsoft.Office.Tools.Word;
using Pdw.FormControls;
using Pdw.Core;
using ProntoDoc.Framework.CoreObject.DataSegment;
using System.Linq.Expressions;
using System.Reflection;

namespace Pdw.Services.Integration
{
    /// <summary>
    /// HACK:FORM CONTROLS
    /// </summary>
    public class PdmService
    {
        private VstoWord.Document _vstoDoc;

        public event AddInternalBookmarkItemEventHandler AddInternalBookmarkItem;
        public event MarkProntoDocEventHandler MarkProntoDoc;
        public event ControlEventBindingEventHandler ControlEventBinding;

        public PdmService()
        {
            _vstoDoc = Globals.Factory.GetVstoObject(Wkl.MainCtrl.CommonCtrl.CommonProfile.ActiveDoc);
        }

        public void AddControl(string key)
        {
            ServicesProfile profile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            PdmServiceProfile pdmProfile = profile.PdmProfile;

            profile.ContentService.MarkProntDoc = true;

            Word.Range rangeToInsert = Wkl.MainCtrl.CommonCtrl.CommonProfile.CurrentSelection.Range;
            rangeToInsert.Collapse(Word.WdCollapseDirection.wdCollapseEnd);

            try
            {
                ControlBase control = Add(pdmProfile.ControlType, rangeToInsert);

                pdmProfile.Control = control;

                ControlEventBinding(key);

                MarkProntoDoc(key);
            }
            catch (Exception ex)
            {
                profile.Error = ex;
                profile.Message = ex.Message;
            }
        }

        public void DataBind(string key)
        {
            ServicesProfile profile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key);

            ControlBase control = profile.PdmProfile.Control;
            string propertyName = profile.PdmProfile.CurrentPropertyName;
            string bindingPath = profile.PdmProfile.DatabindingPath;

            if (control == null || string.IsNullOrEmpty(propertyName))
            {
                //TODO:FORM CONTROLS - DataBind, Exception type
                throw new ArgumentException();
            }

            AddInternalBookmarkItem(key);

            InternalBookmarkItem bookmark = profile.IbmItem;

            BindDataToControl(control, bookmark, propertyName, bindingPath);
        }

        public void Save()
        {
            _vstoDoc.SaveTemplate();
        }

        public void Restore(string key)
        {
            VstoWord.Document doc = _vstoDoc;

            doc.RestoreControls();

            PdmServiceProfile profile = Wkl.MainCtrl.ServiceCtrl.GetProfile(key).PdmProfile;

            foreach (var control in doc.Controls.GetValidItems())
            {
                profile.Control = control;
                profile.IsAdding = false;

                ControlEventBinding(key);
            }
        }

        private ControlBase Add(FormControlType type, Word.Range range)
        {
            ControlBase control = null;

            switch (type)
            {
                case FormControlType.TextInput:
                    control = _vstoDoc.Controls.AddControl<TextInput>(range);
                    break;
                case FormControlType.TextArea:
                    control = _vstoDoc.Controls.AddControl<TextArea>(range);
                    break;
                case FormControlType.CheckBox:
                    control = _vstoDoc.Controls.AddControl<CheckBox>(range);
                    break;
                case FormControlType.Radio:
                    control = _vstoDoc.Controls.AddControl<Radio>(range);
                    break;
                case FormControlType.Button:
                    control = _vstoDoc.Controls.AddControl<Button>(range);
                    break;
                case FormControlType.Select:
                    control = _vstoDoc.Controls.AddControl<Select>(range);
                    break;
                case FormControlType.CheckBoxList:
                    control = _vstoDoc.Controls.AddControl<CheckBoxList>(range);
                    break;
                case FormControlType.RadioList:
                    control = _vstoDoc.Controls.AddControl<RadioList>(range);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return control;
        }

        private void BindDataToControl(ControlBase control, InternalBookmarkItem bookmark, string propertyName, string path)
        {
            if (control is ISelectable)
            {
                bool isBindingKeyProperty = IsSpecifiedProperty(c=>((ISelectable)c).DataBindingSelectKey, propertyName);

                if (isBindingKeyProperty)
                {
                    ISelectable selControl = control as ISelectable;
                    selControl.DataBindingSelectPath = path;
                    selControl.DataBindingSelectKey = bookmark.Key;
                }
            }
            else
            {
                control.DataBindingPath = path;
                control.DataBindingKey = bookmark.Key;
            }     

            control.DataBind = true;
        }

        private bool IsSpecifiedProperty<T>(Expression<Func<ControlBase, T>> property, string nameToCompare)
        {
            MemberExpression expression = null;

            if (property.Body is MemberExpression)
            {
                expression = property.Body as MemberExpression;
            }
            else if (property.Body is UnaryExpression)
            {
                UnaryExpression bodyExpression = property.Body as UnaryExpression;
                expression = bodyExpression.Operand as MemberExpression;
            }

            if (expression == null || (expression.Member.ReflectedType != typeof(ControlBase)
                && !expression.Member.ReflectedType.IsSubclassOf(typeof(ControlBase))))
            {
                throw new NotSupportedException();
            }

            bool result = expression.Member.Name.Equals(nameToCompare, StringComparison.OrdinalIgnoreCase);

            return result;
        }
    }
}
