using System;
using System.Linq;
using System.Windows.Forms;

namespace CryptoCurrency.Lib
{
    public static class TextBoxHelper
    {
        public static void NumberKeyPress(TextBox control, KeyPressEventArgs e)
        {
            string text = control.Text;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && (text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public static void NumberChanged(TextBox control, EventArgs e)
        {
            string text = control.Text.Replace(",", string.Empty).Replace("'", string.Empty);

            int count = text.Count(x => x == '.');

            if (count > 1)
            {
                for (int i = 1; i < count; i++)
                {
                    text = text.Remove(text.IndexOf('.'), 1);
                }
            }

            control.Text = text;
        }
    }
}
