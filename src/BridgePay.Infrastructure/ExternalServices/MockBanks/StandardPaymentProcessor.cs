namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System.Threading.Tasks;
using BridgePay.Infrastructure.ExternalServices.MockBanks.Models;

/// <summary>
/// Concrete abstraction implementation of PaymentProcessor, representing the standard processing flow.
/// </summary>
public class StandardPaymentProcessor : PaymentProcessor
{
    /// <summary>Initializes a new instance of StandardPaymentProcessor.</summary>
    public StandardPaymentProcessor(IBankApi bankApi) : base(bankApi)
    {
    }

    /// <summary>Forwards payment submission directly to the bank API.</summary>
    public override async Task<BankTransactionResponse> ProcessTransactionAsync(BankTransactionRequest request)
    {
        return await BankApi.SubmitPaymentAsync(request);
    }

    /// <summary>Forwards refund submission directly to the bank API.</summary>
    public override async Task<BankRefundResponse> ProcessRefundAsync(BankRefundRequest request)
    {
        return await BankApi.SubmitRefundAsync(request);
    }
}
