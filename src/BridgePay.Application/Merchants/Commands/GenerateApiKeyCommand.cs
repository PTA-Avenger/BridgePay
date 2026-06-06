namespace BridgePay.Application.Merchants.Commands;

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
/// Command to generate new API credentials for a merchant.
/// </summary>
public record GenerateApiKeyCommand : IRequest<MerchantDto>
{
    /// <summary>Gets the merchant ID.</summary>
    public Guid MerchantId { get; init; }
}

/// <summary>
/// Handler for GenerateApiKeyCommand.
/// </summary>
public class GenerateApiKeyCommandHandler : IRequestHandler<GenerateApiKeyCommand, MerchantDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GenerateApiKeyCommandHandler(
        IMerchantRepository merchantRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>Handles key generation.</summary>
    public async Task<MerchantDto> Handle(GenerateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.MerchantId);
        }

        merchant.GenerateApiCredentials();
        await _merchantRepository.UpdateAsync(merchant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<MerchantDto>(merchant);
    }
}
