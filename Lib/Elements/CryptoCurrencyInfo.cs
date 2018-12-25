using System.Runtime.Serialization;

namespace CryptoCurrency.Lib
{
    /// <summary>
    /// https://coinmarketcap.com
    /// </summary>
    [DataContract]
    public class CryptoCurrencyInfo
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }

        [DataMember(Name = "rank")]
        public string Rank { get; set; }

        [DataMember(Name = "price_usd")]
        public string PriceUsd { get; set; }

        [DataMember(Name = "price_btc")]
        public string PriceBtc { get; set; }

        [DataMember(Name = "24h_volume_usd")]
        public string VolumeUsdPerDay { get; set; }

        [DataMember(Name = "market_cap_usd")]
        public string MarketCapUsd { get; set; }

        [DataMember(Name = "available_supply")]
        public string AvailableSupply { get; set; }

        [DataMember(Name = "total_supply")]
        public string TotalSupply { get; set; }

        [DataMember(Name = "max_supply")]
        public string MaxSupply { get; set; }

        [DataMember(Name = "percent_change_1h")]
        public string HourlyChange { get; set; }

        [DataMember(Name = "percent_change_24h")]
        public string DailyChange { get; set; }

        [DataMember(Name = "percent_change_7d")]
        public string WeeklyChange { get; set; }

        [DataMember(Name = "last_updated")]
        public string LastUpdated { get; set; }

        public override string ToString()
        {
            return this.Symbol + "(" + this.Name + ")";
        }
    }
}