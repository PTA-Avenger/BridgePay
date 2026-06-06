namespace BridgePay.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a refund request.
/// </summary>
public enum RefundStatus
{
    /// <summary>Refund has been requested but not yet processed.</summary>
    Pending = 0,

    /// <summary>Refund is currently being processed by the bank.</summary>
    Processing = 1,

    /// <summary>Refund was successfully processed.</summary>
    Completed = 2,

    /// <summary>Refund processing failed.</summary>
    Failed = 3
}
