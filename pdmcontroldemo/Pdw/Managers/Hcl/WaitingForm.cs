using System.Windows.Forms;
using System.Threading;

namespace Pdw.Managers.Hcl
{
    public partial class WaitingForm : Form
    {
        public WaitingForm()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }
    }
}
