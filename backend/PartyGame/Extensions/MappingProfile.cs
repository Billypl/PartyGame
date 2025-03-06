using AutoMapper;
using MongoDB.Bson;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Models.GameModels;
using PartyGame.Models.PlaceModels;

namespace PartyGame.Extensions
{
    // Konfiguracja AutoMappera
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Place, GuessingPlaceDto>();
            CreateMap<GameSession, SummarizeGameDto>();
            CreateMap<NewPlaceDto, Place>();
            CreateMap<PlaceToCheck, PlaceToCheckDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<PlaceToCheckDto, PlaceToCheck>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)));

            CreateMap<User, AccountDetailsDto>();
            CreateMap<AccountDetailsDto, User>();
        }


    }
}