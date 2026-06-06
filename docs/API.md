# API Reference — BridgePay

The BridgePay Web API is built with ASP.NET Core Web API, featuring token-based authentication via Supabase Auth.

---

## 🔒 Authentication

All API endpoints (except `/api/health` and merchant registration) require an `Authorization` header with a valid JWT token issued by Supabase Auth:

```http
Authorization: Bearer <supabase_jwt_token>
```

---

## 🏥 Health Endpoint

### Check Gateway Health
Returns the status of the gateway and its connection to database and message brokers.

- **URL**: `/api/health`
- **Method**: `GET`
- **Auth Required**: No

#### Request Example
```http
GET /api/health HTTP/1.1
Host: localhost:5000
```

#### Response Example (200 OK)
```json
{
  "status": "Healthy",
  "timestamp": "2026-06-05T12:00:00Z",
  "version": "1.0.0",
  "services": {
    "postgres": "Connected",
    "mongoDB": "Connected",
    "rabbitMQ": "Connected"
  }
}
```

---

## 💳 Transactions

### Submit Transaction
Submits a new payment transaction. The request is processed asynchronously via a message queue.

- **URL**: `/api/transactions`
- **Method**: `POST`
- **Auth Required**: Yes

#### Request Body
```json
{
  "amount": 250.00,
  "currency": "ZAR",
  "bankProvider": "FNB",
  "paymentMethod": "CreditCard",
  "customerReference": "ORDER-55102"
}
```

#### Response Example (202 Accepted / 200 OK)
```json
{
  "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "merchantId": "8a7b6c5d-4e3f-2a1b-0c9d-8e7f6a5b4c3d",
  "amount": 250.00,
  "currency": "ZAR",
  "status": "Pending",
  "bankProvider": "FNB",
  "paymentMethod": "CreditCard",
  "customerReference": "ORDER-55102",
  "feeAmount": 5.25,
  "netAmount": 244.75,
  "createdAt": "2026-06-05T12:05:00Z"
}
```

### Get Transaction by ID
Retrieves details of a single transaction by its unique ID.

- **URL**: `/api/transactions/{id}`
- **Method**: `GET`
- **Auth Required**: Yes

#### Response Example (200 OK)
```json
{
  "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "merchantId": "8a7b6c5d-4e3f-2a1b-0c9d-8e7f6a5b4c3d",
  "amount": 250.00,
  "currency": "ZAR",
  "status": "Completed",
  "bankProvider": "FNB",
  "bankTransactionId": "FNB-TX-7718A",
  "paymentMethod": "CreditCard",
  "customerReference": "ORDER-55102",
  "feeAmount": 5.25,
  "netAmount": 244.75,
  "createdAt": "2026-06-05T12:05:00Z",
  "processedAt": "2026-06-05T12:05:02Z"
}
```

### List Transactions
Lists all transactions for the authenticated merchant, with pagination and filtering support.

- **URL**: `/api/transactions`
- **Method**: `GET`
- **Auth Required**: Yes
- **Query Parameters**:
  - `page`: Page index (default: 1)
  - `pageSize`: Page size (default: 10)
  - `status`: Filter by status (`Pending`, `Completed`, `Failed`, `Refunded`)
  - `bank`: Filter by bank provider (`StandardBank`, `FNB`, `Absa`)

#### Response Example (200 OK)
```json
{
  "data": [
    {
      "id": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
      "amount": 250.00,
      "currency": "ZAR",
      "status": "Completed",
      "bankProvider": "FNB",
      "customerReference": "ORDER-55102",
      "createdAt": "2026-06-05T12:05:00Z"
    }
  ],
  "total": 1,
  "page": 1,
  "pageSize": 10,
  "totalPages": 1
}
```

---

## 🔄 Refunds

### Request Refund
Initiates a refund for a completed transaction. The refund amount cannot exceed the original transaction amount.

- **URL**: `/api/refunds`
- **Method**: `POST`
- **Auth Required**: Yes

#### Request Body
```json
{
  "transactionId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "amount": 100.00,
  "reason": "Customer requested cancellation"
}
```

#### Response Example (200 OK)
```json
{
  "id": "f5e4d3c2-b1a0-9e8d-7c6b-5a4f3e2d1c0b",
  "transactionId": "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
  "amount": 100.00,
  "status": "Processing",
  "reason": "Customer requested cancellation",
  "createdAt": "2026-06-05T12:10:00Z"
}
```
