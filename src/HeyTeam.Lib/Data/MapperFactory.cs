using AutoMapper;

namespace HeyTeam.Lib.Data {
    public static class MapperFactory {
        private static IMapper mapper;

        public static IMapper GetMapper() =>  mapper;      

        static MapperFactory() => mapper = Configuration.CreateMapper();

        private static MapperConfiguration Configuration{
            get
            {
                return new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Core.Entities.Club, Data.Club>().ReverseMap();
                    cfg.CreateMap<Core.Entities.Squad, Data.Squad>().ReverseMap();
                });
            }
        }
    }
}