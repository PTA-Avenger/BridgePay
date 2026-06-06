namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Abstraction class in the Bridge pattern, representing payment processing behaviors.
/// </summary>
public abstract class PaymentProcessor
{
    /// <summary>
    /// Holds the implementation of the core bank API (Implementor in Bridge pattern).
    /// </summary>
    protected readonly IBankApi BankApi;

    /// <summary>
    /// Initializes a new instance of the PaymentProcessor with a specific bank API implementor.
    /// </summary>
    protected PaymentProcessor(IBankApi bankApi)
    {
        BankApi = bankApi;
    }

    /// <summary>Processes the transaction using the underlying bank API.</summary>
    public abstract Task<BankTransactionResponse> ProcessTransactionAsync(BankTransactionRequest request);

    /// <summary>Processes the refund using the underlying bank API.</summary>
    public abstract Task<BankRefundResponse> ProcessRefundAsync(BankRefundRequest request);
}
