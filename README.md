# BridgePay — Enterprise Payment Gateway Simulation

BridgePay is a secure, high-throughput payment processing simulation that acts as an intermediary between e-commerce merchants and financial institutions. Built with .NET 10 Web API and Angular 22, it features a complete Clean Architecture backend, an asynchronous processing pipeline utilizing MassTransit + RabbitMQ, and a premium glassmorphic merchant dashboard.

---

## 🔗 Live Deployments

* **Frontend Dashboard**: [https://bridge-pay-xi.vercel.app](https://bridge-pay-xi.vercel.app)
* **Backend API (Swagger)**: [https://bridgepay-api-tggy.onrender.com/swagger](https://bridgepay-api-tggy.onrender.com/swagger)

---

## 🏗️ System Architecture

BridgePay is structured according to Clean Architecture guidelines to ensure separation of concerns, testability, and independence from external frameworks.

```
                  ┌────────────────────────┐
                  │      Presentation      │
                  │   (BridgePay.API)      │
                  └───────────┬────────────┘
                              │
                              ▼
                  ┌────────────────────────┐
                  │      Application       │
                  │ (BridgePay.Application)│
                  └───────────┬────────────┘
                              │
                              ▼
                  ┌────────────────────────┐
                  │         Domain         │
                  │   (BridgePay.Domain)   │
                  └───────────▲────────────┘
                              │
                              │
                  ┌───────────┴────────────┐
                  │     Infrastructure     │
                  │(BridgePay.Infrastructure)│
                  └────────────────────────┘
```

### Flow of Execution
1. **Merchant Dashboard** (Angular SPA hosted on Vercel) authenticates via **Supabase Auth** and sends payment requests to the API.
2. **API Layer** (ASP.NET Core Web API hosted on Render) validates the Supabase JWT token, logs the incoming request to **Supabase PostgreSQL** for audit logging, and delegates to the Application Layer via MediatR.
3. **Application Layer** runs FluentValidation and handles the CQRS commands. It inserts a pending transaction record into **Supabase PostgreSQL** and publishes a `TransactionSubmitted` message to **RabbitMQ (CloudAMQP)**.
4. **RabbitMQ Consumer** (asynchronously running via MassTransit) picks up the message, resolves the appropriate Bank provider using the **Bridge Pattern** (`MockStandardBankApi`, `MockFnbApi`, `MockAbsaBankApi`), and calls the mock financial API with a 15% failure rate simulator.
5. **Bank Gateway Response** is saved back to PostgreSQL, webhooks are fired, and the final state is updated in the merchant's real-time browser dashboard.

---

## 🛠️ Technology Stack

| Layer | Technology | Hosting / Service | Free Tier Details |
|---|---|---|---|
| **Frontend** | Angular 22, RxJS, HSL CSS | Vercel | Free static hosting with CD from GitHub |
| **Backend** | .NET 10, EF Core, MassTransit 8.2.5 | Render | Free web service (Web app container) |
| **Database** | PostgreSQL | Supabase | 500MB storage, 50k MAU Auth, Real-time |
| **Message Queue**| RabbitMQ | CloudAMQP | 1M messages/month (Little Lemur) |

---

## 🚀 Local Development Setup

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js v20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for local RabbitMQ container)

### Step 1: Start Local Services
Spin up local instance of RabbitMQ:
```bash
docker-compose up -d
```
Access the RabbitMQ Management Console at `http://localhost:15672` (Username: `guest`, Password: `guest`).

### Step 2: Configure Databases & Secrets
Update settings in `src/BridgePay.API/appsettings.json` or `appsettings.Development.json`:
- **Supabase Connection**: Get your connection string from the Supabase project dashboard.
- **RabbitMQ URI**: `amqp://guest:guest@localhost:5672/` for local development.

### Step 3: Run Database Migrations
Apply EF Core migrations to prepare the database schema:
```bash
dotnet ef database update --project src/BridgePay.Infrastructure --startup-project src/BridgePay.API
```

### Step 4: Run the Backend API
Start the .NET Web API:
```bash
cd src/BridgePay.API
dotnet run
```
The API Swagger documentation will be available at `http://localhost:5000/swagger`.

### Step 5: Start the Frontend Client
Install Angular dependencies and run the development server:
```bash
cd client
npm install
npm start
```
Open your browser to `http://localhost:4200` to view the dashboard.

---

## 🧪 Testing

The solution includes comprehensive unit and integration test suites targeting Domain logic, MediatR handlers, and API endpoints.

```bash
# Run all tests
dotnet test
```

- **BridgePay.UnitTests**: Tests Domain state transitions, value objects, and Application command handlers using Moq.
- **BridgePay.IntegrationTests**: Uses `WebApplicationFactory` to spin up a test host and verify real HTTP request/response pipelines.

---

## 🧑‍💻 Agile Workflow & Design Decisions

### Bridge Design Pattern
To simulate routing transactions to multiple financial institutions (StandardBank, FNB, Absa) without polluting core business logic, the **Bridge Pattern** decouples the merchant transaction abstraction from the concrete bank implementation.

### Architectural Trade-offs
- **Asynchronous Queue-based Processing**: Submitting payments returns a `Pending` status immediately, placing the load onto RabbitMQ. This ensures high availability and resilience during bank downtime, though it requires the client to poll or use real-time WebSockets/Supabase Realtime to show payment completion.
- **Consolidated Relational Persistence**: To maintain full free-tier hosting compatibility and streamline architectural complexity, transaction tracking, audit logging, and webhook delivery payloads are stored securely in a relational PostgreSQL schema on Supabase, with partition indexes optimized for performance.
