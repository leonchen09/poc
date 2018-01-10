
namespace Pdw.WKL.DataController
{
    public sealed class MainController
    {
        private static MainController _mainController;

        public static MainController MainCtrl
        {
            get
            {
                if (_mainController == null)
                    _mainController = new MainController();

                return _mainController;
            }
        }

        public CommonController CommonCtrl { get; private set; }
        public ManagerController ManagerCtrl { get; private set; }
        public ServicesController ServiceCtrl { get; private set; }
      
        private MainController()
        {
            this.CommonCtrl = new CommonController();
            this.ManagerCtrl = new ManagerController();
            this.ServiceCtrl = new ServicesController();
        
        }
    }
}
