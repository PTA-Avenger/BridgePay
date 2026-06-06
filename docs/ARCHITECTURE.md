# System Architecture — BridgePay

This document describes the architectural patterns, components, and data flows implemented in the BridgePay Enterprise Payment Simulation.

---

## 1. Clean Architecture Design

BridgePay adheres to the principles of Clean Architecture. Dependencies flow inwards:

```
┌─────────────────────────────────────────────────────────────┐
│                       Presentation (API)                     │
│  - Controllers, Filters, Middlewares                        │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                          Application                        │
│  - CQRS Commands, Queries, Handlers                         │
│  - MediatR Pipeline Behaviors, FluentValidation             │
└──────────────────────────────┬──────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                            Domain                           │
│  - Entities, Value Objects, Enums                           │
│  - Core Domain Exceptions, Repository Interfaces            │
└──────────────────────────────▲──────────────────────────────┘
                               │
                               │
┌──────────────────────────────┴──────────────────────────────┐
│                        Infrastructure                       │
│  - DbContexts (PostgreSQL, MongoDB)                         │
│  - Bank API Clients (Bridge Pattern)                        │
│  - MassTransit RabbitMQ Publishers & Consumers              │
└─────────────────────────────────────────────────────────────┘
```

### Components
- **BridgePay.Domain**: Contains the core business entities (`Transaction`, `Merchant`, `Refund`, etc.), immutable value objects like `Money`, and core business rules. It has zero external dependencies.
- **BridgePay.Application**: Contains the business use cases (CQRS model). Leverages MediatR for query/command dispatching, FluentValidation for model validation, and AutoMapper for entity-to-DTO mapping.
- **BridgePay.Infrastructure**: Implements persistence interfaces (EF Core PostgreSQL and MongoDB Atlas), external bank APIs via the Bridge pattern, and message queuing (MassTransit + RabbitMQ).
- **BridgePay.API**: Exposes HTTP REST endpoints, configures JWT token authentication (via Supabase Auth), and intercepts exceptions/logs via custom middleware.

---

## 2. Design Patterns

### Bridge Pattern (Bank Integration)
Decouples the transaction processing abstraction from concrete bank implementations:

```
   ┌───────────────────────────────────────────────┐
   │            PaymentProcessor (Abstraction)      │
   │  - ProcessTransactionAsync()                  │
   └──────────────────────┬────────────────────────┘
                          │ (Bridge)
                          ▼
   ┌───────────────────────────────────────────────┐
   │             IBankApi (Implementation)         │
   │  - SubmitPaymentAsync(request)                │
   │  - SubmitRefundAsync(request)                 │
   └──────────────────────┬────────────────────────┘
            ┌─────────────┼─────────────┐
            ▼             ▼             ▼
      MockStandardBank   MockFnb      MockAbsa
```

#### Code Example
The consumer uses the `PaymentProcessor` which delegates to the resolved `IBankApi`:
```csharp
public class StandardPaymentProcessor : PaymentProcessor
{
    public StandardPaymentProcessor(IBankApi bankApi) : base(bankApi) { }

    public override async Task<BankTransactionResponse> ProcessTransactionAsync(Transaction transaction)
    {
        var request = MapToBankRequest(transaction);
        return await BankApi.SubmitPaymentAsync(request);
    }
}
```

---

## 3. Asynchronous Data Pipelines

### Message Flow (Transaction Processing)

```
[Dashboard] ──► [API Controller] ──► [SubmitTransactionCommand]
                                                │ (Save Pending)
                                                ▼
                                        [Supabase PostgreSQL]
                                                │ (Publish Event)
                                                ▼
                                          [RabbitMQ Queue]
                                                │
                                                ▼
                                       [TransactionConsumer]
                                                │
                                                ▼
                                         [BankApiFactory]
                                                │ (Route & Process)
                                                ▼
                                        [Mock Bank Gateway]
                                                │ (Result Response)
                                                ▼
                                     [Update PostgreSQL Status]
                                                │ (Audit Log)
                                                ▼
                                         [MongoDB Atlas]
```

1. **Submission**: API stores the transaction as `Pending` and drops a message onto RabbitMQ.
2. **De-queuing**: The MassTransit consumer picks up the message asynchronously.
3. **Execution**: The consumer resolves the correct `IBankApi` through `BankApiFactory`, posts the request, and handles simulated network delays and random 15% failures.
4. **Completion**: The transaction status is updated to `Completed` or `Failed` in PostgreSQL and audited in MongoDB Atlas.

---

## 4. Database Schema (Supabase PostgreSQL)

### Tables & Relationships
- **Merchants**: Linked to Supabase Auth (`auth.users`) via `AuthUserId`. Holds the business profile and live API key credentials.
- **Transactions**: Belongs to a single Merchant. Tracks the payment details, status, processing bank, and fees.
- **Refunds**: One-to-one or one-to-many relationship with Transactions. Validates refund amounts against the original transaction amount.
- **ApiKeys**: Manages merchant API keys, expiry, and active status.
