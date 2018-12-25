using CryptoCurrency.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace CryptoCurrency.Watcher
{
    public class DownloadWorker : BackgroundWorker
    {
        private DataGridView dataGrid = null;
        private List<CryptoCurrencyInfo> list = null;
        private CryptoCurrencyInfo add = null;

        public DownloadWorker(DataGridView dataGrid, List<CryptoCurrencyInfo> list, CryptoCurrencyInfo add)
        {
            this.dataGrid = dataGrid;
            this.list = list;
            this.add = add;

            this.DoWork += this.DownloadWorker_DoWork;
        }

        private void DownloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CryptoCurrencyInfo info = DownloadHelper.DownloadSpecificCurrencyInfo(this.add.ID).GetAwaiter().GetResult();

            this.dataGrid.Invoke(new Action(() =>
            {
                this.dataGrid.Rows.Add();
                WatcherHelper.SetRow(dataGrid, info, this.list.Count);
            }));

            this.list.Add(add);
        }
    }
}