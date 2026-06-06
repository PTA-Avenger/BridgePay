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
/// Query to retrieve a merchant by ID.
/// </summary>
public record GetMerchantByIdQuery : IRequest<MerchantDto>
{
    /// <summary>Gets the unique identifier of the merchant.</summary>
    public Guid Id { get; init; }
}

/// <summary>
/// Handler for GetMerchantByIdQuery.
/// </summary>
public class GetMerchantByIdQueryHandler : IRequestHandler<GetMerchantByIdQuery, MerchantDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetMerchantByIdQueryHandler(IMerchantRepository merchantRepository, IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<MerchantDto> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.Id);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.Id);
        }

        return _mapper.Map<MerchantDto>(merchant);
    }
}
