namespace BridgePay.Application.Refunds.Queries;

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
/// Query to retrieve a refund by ID.
/// </summary>
public record GetRefundByIdQuery : IRequest<RefundDto>
{
    /// <summary>Gets the refund identifier.</summary>
    public Guid Id { get; init; }
}

/// <summary>
/// Handler for GetRefundByIdQuery.
/// </summary>
public class GetRefundByIdQueryHandler : IRequestHandler<GetRefundByIdQuery, RefundDto>
{
    private readonly IRefundRepository _refundRepository;
    private readonly IMapper _mapper;

    /// <summary>Initializes dependencies.</summary>
    public GetRefundByIdQueryHandler(IRefundRepository refundRepository, IMapper mapper)
    {
        _refundRepository = refundRepository;
        _mapper = mapper;
    }

    /// <summary>Handles query execution.</summary>
    public async Task<RefundDto> Handle(GetRefundByIdQuery request, CancellationToken cancellationToken)
    {
        var refund = await _refundRepository.GetByIdAsync(request.Id);
        if (refund == null)
        {
            throw new NotFoundException(nameof(Refund), request.Id);
        }

        return _mapper.Map<RefundDto>(refund);
    }
}
