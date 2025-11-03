using AutoMapper;
using Services.Application.DTOs;
using Services.Domain.Entities;

namespace Services.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Customer, CustomerDto>().ReverseMap();
        CreateMap<Appointment, AppointmentDto>().ReverseMap();
        CreateMap<InvoiceDetails, InvoiceDto>().ReverseMap();
        CreateMap<Payment, PaymentDto>().ReverseMap();
    }
}

