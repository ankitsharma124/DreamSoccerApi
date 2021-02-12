namespace DreamSoccer.Core.Dtos.TransferList
{
    public class SearchPlayerFilter
    {
        public string Country { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }
        public int? TeamId { get; set; }
    }
}
