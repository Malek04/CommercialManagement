using AutoMapper;
using CommercialManagement.Core.DTOs;
using CommercialManagement.Core.Models;

namespace CommercialManagement.Api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile() { 

        CreateMap<Order, OrderDto>()
        .ForMember(d => d.ClientId, i => i.MapFrom(src => src.Client.Id))
        .ForMember(d => d.LastName, i => i.MapFrom(src => src.Client.LastName))
        .ForMember(d => d.FirstName, i => i.MapFrom(src => src.Client.FirstName))
        .ForMember(d => d.Email, i => i.MapFrom(src => src.Client.Email))
        .ForMember(d => d.Phone, i => i.MapFrom(src => src.Client.Phone))
        .ForMember(d => d.Created, i => i.MapFrom(src => src.Client.Created))
        .ForMember(d => d.Rue, i => i.MapFrom(src => src.Client.Adresse.Rue))
        .ForMember(d => d.Ville, i => i.MapFrom(src => src.Client.Adresse.Ville))
        .ForMember(d => d.CodePostal, i => i.MapFrom(src => src.Client.Adresse.CodePostal))
        .ForMember(d => d.Pays, i => i.MapFrom(src => src.Client.Adresse.Pays))
        .ReverseMap();

        
        }
    }
}
