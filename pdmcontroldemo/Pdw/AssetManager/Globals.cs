
namespace Pdw.AssetManager
{
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Tools.Office.ProgrammingModel.dll", "10.0.0.0")]
    internal sealed partial class Globals
    {
        private Globals()
        {
        }

        private static ThisAddIn _ThisAddIn;

        private static global::Microsoft.Office.Tools.Word.ApplicationFactory _factory;

        private static ThisRibbonCollection _ThisRibbonCollection;

        internal static ThisAddIn ThisAddIn
        {
            get
            {
                return _ThisAddIn;
            }
            set
            {
                if ((_ThisAddIn == null))
                {
                    _ThisAddIn = value;
                }
                else
                {
                    throw new System.NotSupportedException();
                }
            }
        }

        internal static global::Microsoft.Office.Tools.Word.ApplicationFactory Factory
        {
            get
            {
                return _factory;
            }
            set
            {
                if ((_factory == null))
                {
                    _factory = value;
                }
                else
                {
                    throw new System.NotSupportedException();
                }
            }
        }

        internal static ThisRibbonCollection Ribbons
        {
            get
            {
                if ((_ThisRibbonCollection == null))
                {
                    _ThisRibbonCollection = new ThisRibbonCollection(_factory.GetRibbonFactory());
                }
                return _ThisRibbonCollection;
            }
        }
        
    }
}
