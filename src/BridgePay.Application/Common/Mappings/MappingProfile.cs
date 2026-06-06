namespace BridgePay.Application.Common.Mappings;

using AutoMapper;
using BridgePay.Domain.Entities;
using BridgePay.Application.Common.DTOs;

/// <summary>
/// AutoMapper profile defining object mappings for the application.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MappingProfile class.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.BankProvider, opt => opt.MapFrom(src => src.BankProvider.ToString()));

        CreateMap<Merchant, MerchantDto>();

        CreateMap<Refund, RefundDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
