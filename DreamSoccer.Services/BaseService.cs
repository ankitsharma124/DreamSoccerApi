namespace DreamSoccer.Core.Contracts.Services
{
    public class BaseService : IMessageService
    {
        public string CurrentMessage { get; protected set; }
    }
}