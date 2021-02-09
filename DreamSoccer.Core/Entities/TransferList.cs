namespace DreamSoccer.Core.Entities
{
    public class TransferList : BaseEntity
    {
        public long Value { get; set; }
        public virtual Player Player { get; set; }
        public int PlayerId { get; set; }
    }
}