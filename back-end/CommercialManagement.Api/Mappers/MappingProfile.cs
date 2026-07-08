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

        CreateMap<OrderLine, OderLineDto>()
        .ForMember(d => d.OrderId, i => i.MapFrom(src => src.Order.Id))
        .ForMember(d => d.OrderNumber, i => i.MapFrom(src => src.Order.OrderNumber))
        .ForMember(d => d.OrderDate, i => i.MapFrom(src => src.Order.OrderDate))
        .ForMember(d => d.Status, i => i.MapFrom(src => src.Order.Status))
        .ForMember(d => d.TotalHT, i => i.MapFrom(src => src.Order.TotalHT))
        .ForMember(d => d.TotalTTC, i => i.MapFrom(src => src.Order.TotalTTC))
        .ForMember(d => d.Client_Id, i => i.MapFrom(src => src.Order.Client.Id))
        .ForMember(d => d.Client_LastName, i => i.MapFrom(src => src.Order.Client.LastName))
        .ForMember(d => d.Client_FirstName, i => i.MapFrom(src => src.Order.Client.FirstName))
        .ForMember(d => d.Client_Email, i => i.MapFrom(src => src.Order.Client.Email))
        .ForMember(d => d.Client_Phone, i => i.MapFrom(src => src.Order.Client.Phone))
        .ForMember(d => d.Client_Created, i => i.MapFrom(src => src.Order.Client.Created))
        .ForMember(d => d.Adresse_Rue, i => i.MapFrom(src => src.Order.Client.Adresse.Rue))
        .ForMember(d => d.Adresse_Ville, i => i.MapFrom(src => src.Order.Client.Adresse.Ville))
        .ForMember(d => d.Adresse_CodePostal, i => i.MapFrom(src => src.Order.Client.Adresse.CodePostal))
        .ForMember(d => d.Adresse_Pays, i => i.MapFrom(src => src.Order.Client.Adresse.Pays))
        .ForMember(d => d.Product_Id, i => i.MapFrom(src => src.Product.Id))
        .ForMember(d => d.Product_Reference, i => i.MapFrom(src => src.Product.Reference))
        .ForMember(d => d.Product_Name, i => i.MapFrom(src => src.Product.Name))
        .ForMember(d => d.Product_Description, i => i.MapFrom(src => src.Product.Description))
        .ForMember(d => d.Product_UnitPriceHT, i => i.MapFrom(src => src.Product.UnitPriceHT))
        .ForMember(d => d.Product_StockQuantity, i => i.MapFrom(src => src.Product.StockQuantity))
        .ForMember(d => d.Product_Created, i => i.MapFrom(src => src.Product.Created))
        .ReverseMap();
        }
    }
}
