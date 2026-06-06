namespace BridgePay.Infrastructure.ExternalServices.MockBanks;

using System;
using BridgePay.Domain.Enums;

/// <summary>
/// Factory class to resolve bank API implementations based on the bank provider enum.
/// </summary>
public class BankApiFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>Initializes a new instance of BankApiFactory.</summary>
    public BankApiFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Resolves the concrete implementation of IBankApi for the specified provider.
    /// </summary>
    public IBankApi GetBankApi(BankProvider bankProvider)
    {
        return bankProvider switch
        {
            BankProvider.StandardBank => (IBankApi)_serviceProvider.GetService(typeof(MockStandardBankApi))! ?? new MockStandardBankApi(),
            BankProvider.FNB => (IBankApi)_serviceProvider.GetService(typeof(MockFnbApi))! ?? new MockFnbApi(),
            BankProvider.Absa => (IBankApi)_serviceProvider.GetService(typeof(MockAbsaBankApi))! ?? new MockAbsaBankApi(),
            _ => throw new ArgumentException($"Unsupported bank provider: {bankProvider}")
        };
    }
}
