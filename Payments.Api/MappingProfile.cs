using AutoMapper;
using Payments.Api.Dtos;
using Payments.Api.Entities;

namespace Payments.Api;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<PaymentDetail, PaymentDetailsForCreateDto>().ReverseMap();
    }
}