namespace CryptoCurrency.Lib
{
    public class DisplayData
    {
        public DisplayData(CryptoCurrencyInfo data)
        {
            this.Data = data;
        }

        public string ID
        {
            get
            {
                return this.Data.ID;
            }
        }

        public CryptoCurrencyInfo Data { get; private set; } = null;
        public bool Active { get; private set; } = true;

        public void Disable()
        {
            this.Active = false;
        }
    }
}