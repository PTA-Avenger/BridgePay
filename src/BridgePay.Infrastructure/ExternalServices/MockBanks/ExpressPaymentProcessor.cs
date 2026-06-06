namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Concrete abstraction implementation of PaymentProcessor representing express/fast-track processing.
/// </summary>
public class ExpressPaymentProcessor : PaymentProcessor
{
    /// <summary>Initializes a new instance of ExpressPaymentProcessor.</summary>
    public ExpressPaymentProcessor(IBankApi bankApi) : base(bankApi)
    {
    }

    /// <summary>Processes payment via express path (for example, bypassing minor checks).</summary>
    public override async Task<BankTransactionResponse> ProcessTransactionAsync(BankTransactionRequest request)
    {
        // Express processing logic wrapper
        return await BankApi.SubmitPaymentAsync(request);
    }

    /// <summary>Processes refund via express path.</summary>
    public override async Task<BankRefundResponse> ProcessRefundAsync(BankRefundRequest request)
    {
        return await BankApi.SubmitRefundAsync(request);
    }
}
