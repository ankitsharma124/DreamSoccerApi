using AutoMapper;
using DreamSoccer.Core.Configurations;

namespace DreamSoccer.Services.Test.Helpers
{
    public class AutoMapperHelper
    {
        public static IMapper Create()
        {

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperConfiguration());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            return mapper;
        }
    }
}