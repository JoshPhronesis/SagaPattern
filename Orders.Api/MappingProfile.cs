using AutoMapper;
using OrdersService.Dtos;
using OrdersService.Entities;

namespace OrdersService;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderForCreateDto>().ReverseMap();
    }
}