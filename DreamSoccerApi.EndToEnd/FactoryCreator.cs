using Bogus;
using DreamSoccer.Core.Dtos.User;
using DreamSoccer.Core.Entities;
using System;

namespace DreamSoccerApi.E2E
{
    public class FactoryCreator
    {
        private static Faker<UserRegisterDto> _fakeUserRegister;

        static FactoryCreator()
        {
            _fakeUserRegister = new Faker<UserRegisterDto>()
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
                .RuleFor(u => u.Password, (f, u) => f.Internet.Password(5));
        }
        public static UserRegisterDto CreateUser(RoleEnum role = RoleEnum.Team_Owner)
        {
            var user = _fakeUserRegister.Generate();
            user.Email = user.Email.Replace("@", $"{DateTime.Now.Second.ToString()}@");
            user.Role = role;
            return user;
        }
    }



}
