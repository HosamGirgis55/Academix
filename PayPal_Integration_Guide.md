# PayPal Integration Guide

## Overview

This PayPal integration allows users to make payments through PayPal using a simple 3-step flow:

1. **Create Payment Order** - Frontend requests payment, backend creates PayPal order and returns payment URL
2. **User Completes Payment** - User is redirected to PayPal to complete payment
3. **Capture Payment** - After payment completion, backend captures and confirms the payment

## Setup

### 1. PayPal Configuration

Update `appsettings.json` with your PayPal credentials:

```json
{
  "PayPal": {
    "ClientId": "YOUR_PAYPAL_CLIENT_ID",
    "ClientSecret": "YOUR_PAYPAL_CLIENT_SECRET", 
    "BaseUrl": "https://api-m.sandbox.paypal.com",
    "ReturnUrl": "http://localhost:3000/payment/success",
    "CancelUrl": "http://localhost:3000/payment/cancel"
  }
}
```

**For Production:**
- Change `BaseUrl` to `"https://api-m.paypal.com"`
- Use production PayPal credentials

### 2. Database Migration

The `Payment` entity has been added. Run database migration:

```bash
dotnet ef migrations add AddPaymentEntity
dotnet ef database update
```

## API Endpoints

### 1. Create Payment Order

**POST** `/api/payments/create-order`

**Headers:**
- `Authorization: Bearer {jwt_token}`
- `Accept-Language: en` or `ar`

**Request Body:**
```json
{
  "amount": 10.00,
  "currency": "USD",
  "description": "Payment for session",
  "reference": "SESSION_123",
  "sessionId": "guid-optional"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentId": "guid",
    "payPalOrderId": "paypal_order_id", 
    "paymentUrl": "https://www.sandbox.paypal.com/checkoutnow?token=...",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Payment order created successfully"
}
```

### 2. Capture Payment Order

**POST** `/api/payments/capture-order`

**Request Body:**
```json
{
  "payPalOrderId": "paypal_order_id",
  "payerId": "payer_id_from_paypal"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentId": "guid",
    "payPalTransactionId": "transaction_id",
    "capturedAmount": 10.00,
    "status": "Completed",
    "capturedAt": "2024-01-01T00:00:00Z",
    "isSuccess": true
  },
  "message": "Payment captured successfully"
}
```

### 3. Check Order Status

**GET** `/api/payments/check-order/{orderId}`

**Response:**
```json
{
  "success": true,
  "data": {
    "paymentId": "guid",
    "payPalOrderId": "order_id",
    "localStatus": "Completed",
    "payPalStatus": "COMPLETED", 
    "amount": 10.00,
    "currency": "USD",
    "description": "Payment for session",
    "createdAt": "2024-01-01T00:00:00Z",
    "completedAt": "2024-01-01T00:00:00Z",
    "transactionId": "transaction_id"
  }
}
```

## Frontend Integration Flow

### Step 1: Create Payment Order

```javascript
const createPaymentOrder = async (amount, description) => {
  const response = await fetch('/api/payments/create-order', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${userToken}`,
      'Accept-Language': 'en'
    },
    body: JSON.stringify({
      amount: amount,
      currency: 'USD',
      description: description
    })
  });
  
  const result = await response.json();
  return result;
};
```

### Step 2: Redirect to PayPal

```javascript
const handlePayment = async () => {
  try {
    const result = await createPaymentOrder(10.00, "Session Payment");
    
    if (result.success) {
      // Redirect user to PayPal
      window.location.href = result.data.paymentUrl;
    }
  } catch (error) {
    console.error('Payment creation failed:', error);
  }
};
```

### Step 3: Handle Payment Return

After user completes payment on PayPal, they return to your `ReturnUrl` with query parameters:

```javascript
// On your success page
const handlePaymentReturn = async () => {
  const urlParams = new URLSearchParams(window.location.search);
  const payPalOrderId = urlParams.get('token');
  const payerId = urlParams.get('PayerID');
  
  if (payPalOrderId && payerId) {
    const captureResult = await capturePayment(payPalOrderId, payerId);
    
    if (captureResult.success) {
      // Payment successful
      console.log('Payment completed successfully');
    }
  }
};

const capturePayment = async (payPalOrderId, payerId) => {
  const response = await fetch('/api/payments/capture-order', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Accept-Language': 'en'
    },
    body: JSON.stringify({
      payPalOrderId: payPalOrderId,
      payerId: payerId
    })
  });
  
  return await response.json();
};
```

## Payment Statuses

- **Pending** - Payment order created, waiting for user to complete payment
- **Completed** - Payment successfully captured
- **Failed** - Payment failed or was declined
- **Cancelled** - User cancelled the payment

## Error Handling

All endpoints return errors in this format:

```json
{
  "success": false,
  "message": "Error message",
  "errors": ["Detailed error information"]
}
```

Common error scenarios:
- Invalid payment amount
- PayPal API errors
- Payment not found
- Invalid payment status

## Testing

Use PayPal sandbox for testing:
1. Create sandbox accounts at https://developer.paypal.com
2. Use sandbox credentials in `appsettings.json`
3. Test with sandbox buyer accounts

## Security Considerations

- All payment endpoints are secured with JWT authentication (except capture)
- PayPal credentials are stored securely in configuration
- Payment amounts and statuses are validated server-side
- Transaction IDs are logged for audit purposes

## Database Schema

The `Payment` entity includes:
- User information and session linkage
- PayPal integration fields (OrderId, TransactionId, etc.)
- Financial details (Amount, Fees, Net Amount)
- Status tracking and timestamps
- Failure reason logging 