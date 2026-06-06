namespace BridgePay.Application.Merchants.Commands;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BridgePay.Application.Common.DTOs;
using BridgePay.Domain.Entities;
using BridgePay.Domain.Exceptions;
using BridgePay.Domain.Interfaces;
using FluentValidation;
using MediatR;

/// <summary>
/// Command to register a new merchant.
/// </summary>
public record RegisterMerchantCommand : IRequest<MerchantDto>
{
    /// <summary>Gets the Auth User ID linked to Supabase.</summary>
    public Guid AuthUserId { get; init; }

    /// <summary>Gets the business name of the merchant.</summary>
    public string BusinessName { get; init; } = string.Empty;

    /// <summary>Gets the email address of the merchant.</summary>
    public string Email { get; init; } = string.Empty;
}

/// <summary>
/// Validator for RegisterMerchantCommand.
/// </summary>
public class RegisterMerchantCommandValidator : AbstractValidator<RegisterMerchantCommand>
{
    /// <summary>Initializes validation rules.</summary>
    public RegisterMerchantCommandValidator()
    {
        RuleFor(x => x.AuthUserId).NotEmpty().WithMessage("AuthUserId is required.");
        RuleFor(x => x.BusinessName).NotEmpty().WithMessage("Business name is required.")
            .MaximumLength(150).WithMessage("Business name cannot exceed 150 characters.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}

/// <summary>
/// Handler for RegisterMerchantCommand.
/// </summary>
public class RegisterMerchantCommandHandler : IRequestHandler<RegisterMerchantCommand, MerchantDto>
{
    private readonly IMerchantRepository _merchantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public RegisterMerchantCommandHandler(
        IMerchantRepository merchantRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _merchantRepository = merchantRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <summary>Handles the registration logic.</summary>
    public async Task<MerchantDto> Handle(RegisterMerchantCommand request, CancellationToken cancellationToken)
    {
        var existing = await _merchantRepository.GetByAuthUserIdAsync(request.AuthUserId);
        if (existing != null)
        {
            throw new DomainException("Merchant is already registered for this user.");
        }

        var merchant = Merchant.Create(request.AuthUserId, request.BusinessName, request.Email);
        await _merchantRepository.AddAsync(merchant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<MerchantDto>(merchant);
    }
}
