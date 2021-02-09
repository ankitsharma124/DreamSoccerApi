using Bogus;
using DreamSoccer.Core.Contracts.Repositories;
using DreamSoccer.Core.Entities;
using System.Threading.Tasks;

namespace DreamSoccer.Repository.Implementations
{
    public class RandomRepository : IRandomRepository
    {
        private Faker<Player> _fakePlayer;
        private Faker<Team> _teamPlayer;

        public RandomRepository()
        {
            _fakePlayer = new Faker<Player>()
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Country, (f, u) => f.Address.Country())
                .RuleFor(u => u.Age, (f, u) => f.Random.Int(18, 40))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName())
                .RuleFor(u => u.Value, (f, u) => f.Random.Int(10, 100));
            _teamPlayer = new Faker<Team>()
               .RuleFor(u => u.Country, (f, u) => f.Address.Country())
               .RuleFor(u => u.TeamName, (f, u) => f.Company.CompanyName());


        }
        public Task<Player> GetRandomPlayer()
        {
            return Task.FromResult(_fakePlayer.Generate());
        }

        public Task<long> GetRandomRatioForIncreaseValue()
        {
            return Task.FromResult(_fakePlayer.Generate().Value);
        }

        public Task<Team> GetRandomTeam()
        {
            return Task.FromResult(_teamPlayer.Generate());
        }
    }
}