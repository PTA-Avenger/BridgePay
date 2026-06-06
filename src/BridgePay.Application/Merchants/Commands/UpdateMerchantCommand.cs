namespace BridgePay.Application.Merchants.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Application.Common.Exceptions;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Interfaces;
using FluentValidation;
using MediatR;

/// <summary>
/// Command to update merchant details.
/// </summary>
public record UpdateMerchantCommand : IRequest<MerchantDto>
{
    /// <summary>Gets the unique identifier of the merchant to update.</summary>
    public Guid Id { get; init; }

    /// <summary>Gets the new business name.</summary>
    public string BusinessName { get; init; } = string.Empty;
}

/// <summary>
/// Validator for UpdateMerchantCommand.
/// </summary>
public class UpdateMerchantCommandValidator : AbstractValidator<UpdateMerchantCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public UpdateMerchantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.BusinessName).NotEmpty().WithMessage("Business name is required.")
            .MaximumLength(150).WithMessage("Business name cannot exceed 150 characters.");
    }
}

/// <summary>
/// Handler for UpdateMerchantCommand.
/// </summary>
public class UpdateMerchantCommandHandler : IRequestHandler<UpdateMerchantCommand, MerchantDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public UpdateMerchantCommandHandler(
        IMerchantRepository merchantRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>Handles the update logic.</summary>
    public async Task<MerchantDto> Handle(UpdateMerchantCommand request, CancellationToken cancellationToken)
    {
        var merchant = await _merchantRepository.GetByIdAsync(request.Id);
        if (merchant == null)
        {
            throw new NotFoundException(nameof(Merchant), request.Id);
        }

        merchant.UpdateBusinessName(request.BusinessName);
        await _merchantRepository.UpdateAsync(merchant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<MerchantDto>(merchant);
    }
}
