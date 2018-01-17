using AutoMapper;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchMappingProfile : Profile
    {
        public DutchMappingProfile()
        {
            // Via CreateMap a mapping between the properties of the 2 given Objectclasses are inferred
            // With .ForMember exceptions can be created, for example when the mapping cannot be automatically inferred
            // (in this case the value for orderId (OrderViewModel) is taken from Id (Order)
            CreateMap<Order, OrderViewModel>()
                .ForMember(o => o.OrderId, ex => ex.MapFrom(o => o.Id))
                .ReverseMap(); // Maps also vice versa

            CreateMap<OrderItem, OrderItemViewModel>()
                .ReverseMap();

        }
    }
}
