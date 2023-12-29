using AutoMapper;
using Shipping.Api.Dtos;
using Shipping.Api.Entities;

namespace Shipping.Api;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<ShippingRequest, ShippingRequestForCreateDto>().ReverseMap();
    }
}