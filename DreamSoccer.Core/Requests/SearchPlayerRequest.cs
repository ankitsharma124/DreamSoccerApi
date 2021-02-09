namespace DreamSoccer.Core.Requests
{
    public class SearchPlayerRequest
    {
        public string Country { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }
    }
}
