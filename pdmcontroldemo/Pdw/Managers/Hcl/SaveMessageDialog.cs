using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Office.Interop.Word;
using Pdw.WKL.Profiler.Manager;
using Pdw.WKL.Profiler.Services;
using Wkl = Pdw.WKL.DataController.MainController;
using Pdw.Core;

namespace Pdw.Managers.Hcl
{
    public partial class SaveMessageDialog : Form
    {
        private string _key;
        private ManagerProfile _mgrPro;
        private Context.ContextValidator _validator;
        private bool _isSaveDocument = true;

        #region constructor
        public SaveMessageDialog(string key)
        {
            InitializeComponent();

            _key = key;
            _mgrPro = Wkl.MainCtrl.ManagerCtrl.GetProfile(key);
            _isSaveDocument = _mgrPro.IsSaveDocument;
            SetCaption(_mgrPro.WDoc.Name);
            SetMessage();

            _validator = new Context.ContextValidator(_mgrPro.WDoc);

            timerCheck.Interval = 60 * 1000; // 60 * 1s = 60s
            timerCheck.Start();
        }
        #endregion

        #region events handler (OK, Cancel, timer)
        private void btnOK_Click(object sender, EventArgs e)
        {
            timerCheck.Stop();
            DeleteRedundantBookmark();
            Save();

            TemplateInfo currentTemplate = WKL.DataController.MainController.MainCtrl.CommonCtrl.CommonProfile.CurrentTemplateInfo;
            bool hasSelectIBM = currentTemplate.InternalBookmark.HasSelectIBM();

            if (!hasSelectIBM)
                DisablePreviewOsqlButton();
            else
            {
                MainManager mainManager = new MainManager();
                mainManager.RibbonManager.UpdateUI(EventType.VisiablePreviewOsql);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            timerCheck.Stop();
            DisablePreviewOsqlButton();
            this.Close();
        }

        private void timerCheck_Tick(object sender, EventArgs e)
        {
            if (IsCorrectBookmark())
            {
                timerCheck.Stop();
                this.Close();
            }
        }
        #endregion

        #region helper methods
        private void SetCaption(string documentName)
        {
            this.Text = string.Format(Properties.Resources.ipm_SaveNotifyCaption, documentName);
        }

        private void SetMessage()
        {
            lblMessage.Text = Properties.Resources.ipm_SaveNotifyMessage;
            lblConfirm.Text = Properties.Resources.ipm_SaveConfirmMessage;
        }

        private void DeleteRedundantBookmark()
        {
            string key = _key;
            if (!Wkl.MainCtrl.ManagerCtrl.IsExist(key))
                Wkl.MainCtrl.ManagerCtrl.CreateProfile(out key);

            _validator.ValidateBeforeSave(key);
            _validator.RemoveBookmark(key);
        }

        private void Save()
        {
            if (_isSaveDocument)
                _validator.Save();
        }

        private bool IsCorrectBookmark()
        {
            // bool isCorrect = false;
            // _highlightedBmKeys = new List<string>();

            // isCorrect = _validator.ValidateBeforeSave(_highlightedBmKeys);
            _validator.ValidateBeforeSave(_key);

            // if (isCorrect)
            if (_mgrPro.IsCorrect)
                return true;
            else
            {
                DisablePreviewOsqlButton();

                // if (_highlightedBmKeys.Count > 0)
                if (_mgrPro.WbmKeys.Count > 0)
                    return false;
                else // the document has no data tag
                    return true;
            }
        }

        private void DisablePreviewOsqlButton()
        {
            //Force to Disable button without checking
            MainManager mainManager = new MainManager();
            mainManager.RibbonManager.UpdateUI(EventType.ForceDisablePreviewOsql);
        }
        #endregion
    }
}
