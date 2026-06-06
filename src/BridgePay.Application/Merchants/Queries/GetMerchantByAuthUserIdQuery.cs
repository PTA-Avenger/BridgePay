namespace BridgePay.Application.Merchants.Queries;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Interfaces;
using MediatR;

/// <summary>
/// Query to retrieve a merchant by Auth User ID.
/// </summary>
public record GetMerchantByAuthUserIdQuery : IRequest<MerchantDto>
{
    /// <summary>Gets the Auth User ID from Supabase.</summary>
    public Guid AuthUserId { get; init; }
}

/// <summary>
/// Handler for GetMerchantByAuthUserIdQuery.
/// </summary>
public class GetMerchantByAuthUserIdQueryHandler : IRequestHandler<GetMerchantByAuthUserIdQuery, MerchantDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetMerchantByAuthUserIdQueryHandler(IMerchantRepository merchantRepository, IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<MerchantDto> Handle(GetMerchantByAuthUserIdQuery request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByAuthUserIdAsync(request.AuthUserId);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.AuthUserId);
        }

        return _mapper.Map<MerchantDto>(merchant);
    }
}
