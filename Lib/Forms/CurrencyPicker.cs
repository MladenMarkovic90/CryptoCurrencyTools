using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CryptoCurrency.Lib
{
    public partial class CurrencyPicker : Form
    {
        public string Result { get; private set; } = string.Empty;

        public CurrencyPicker(CryptoCurrencyData data)
        {
            this.InitializeComponent();
            this.cmbCurrencies.Items.AddRange(data.InfoData.OrderBy(x => x.ToString().ToLower()).Select(x => x.ToString()).ToArray());
        }

        private void AddClick(object sender, EventArgs e)
        {
            this.Result = this.cmbCurrencies.Text;
            this.Close();
        }
    }
}