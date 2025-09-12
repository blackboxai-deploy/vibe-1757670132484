# BoG-Compliant Banking Platform - API Testing Guide

## 🧪 API Testing Workflow

This guide demonstrates how to test the complete banking platform API endpoints using curl commands. The platform is now ready for production deployment and testing.

## 🚀 Getting Started

### 1. Environment Setup

```bash
# Clone and build the platform
git clone <repository-url>
cd BankPlatform

# Start the infrastructure services
cd docker
docker-compose up -d postgres redis jaeger

# Build and run the API
dotnet build
dotnet ef database update --project src/BankPlatform.Infrastructure --startup-project src/BankPlatform.API
dotnet run --project src/BankPlatform.API
```

The API will be available at: `https://localhost:5001`

### 2. Health Check Verification

```bash
# Basic health check
curl -X GET https://localhost:5001/health \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n"

# Expected Response:
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "duration": "00:00:00.0123456",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "duration": "00:00:00.0045678"
    }
  ]
}
```

## 🔐 Authentication Testing

### 1. User Login (JWT Authentication)

```bash
# Login as Super Admin
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{
    "email": "superadmin@bankplatform.com",
    "password": "SuperAdmin123!@#",
    "rememberMe": false
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "ab1c2d3e4f5g6h7i8j9k0l",
  "expiresIn": 3600,
  "user": {
    "id": "guid-here",
    "firstName": "Super",
    "lastName": "Administrator",
    "email": "superadmin@bankplatform.com",
    "roles": ["SuperAdmin"]
  },
  "requiresMfa": false
}
```

### 2. Token Refresh

```bash
# Store the tokens for subsequent requests
ACCESS_TOKEN="your-access-token-here"
REFRESH_TOKEN="your-refresh-token-here"

# Refresh the access token
curl -X POST https://localhost:5001/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "refreshToken": "'$REFRESH_TOKEN'"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 3. Multi-Factor Authentication (MFA)

```bash
# If MFA is required, verify the code
curl -X POST https://localhost:5001/api/v1/auth/mfa/verify \
  -H "Content-Type: application/json" \
  -d '{
    "code": "123456",
    "sessionToken": "session-token-from-login",
    "method": "SMS"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 👥 Customer Management Testing

### 1. Create New Customer (KYC Process)

```bash
# Create a new customer with KYC information
curl -X POST https://localhost:5001/api/v1/customers \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "firstName": "Kwame",
    "lastName": "Asante",
    "middleName": "Kofi",
    "email": "kwame.asante@example.com",
    "phoneNumber": "+233201234567",
    "dateOfBirth": "1985-03-15",
    "gender": "Male",
    "maritalStatus": "Married",
    "ghanaCardNumber": "GHA-123456789-1",
    "occupation": "Software Engineer",
    "monthlyIncome": 8000,
    "incomeSource": "Employment"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response:
{
  "id": "customer-guid",
  "customerNumber": "CUST000012345",
  "firstName": "Kwame",
  "lastName": "Asante",
  "email": "kwame.asante@example.com",
  "phoneNumber": "+233201234567",
  "kycStatus": "Pending",
  "riskRating": "Low",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### 2. Get Customer Details

```bash
CUSTOMER_ID="customer-guid-here"

curl -X GET https://localhost:5001/api/v1/customers/$CUSTOMER_ID \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 3. Update Customer KYC Status

```bash
curl -X PUT https://localhost:5001/api/v1/customers/$CUSTOMER_ID/kyc \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "kycStatus": "Completed",
    "riskRating": "Low",
    "verifiedBy": "NIA",
    "verificationDate": "2024-01-15T10:30:00Z",
    "documents": [
      {
        "type": "GhanaCard",
        "number": "GHA-123456789-1",
        "verified": true
      }
    ]
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 🏦 Account Management Testing

### 1. Create Bank Account

```bash
curl -X POST https://localhost:5001/api/v1/accounts \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "customerId": "'$CUSTOMER_ID'",
    "accountType": "Savings",
    "initialDeposit": 500.00,
    "currency": "GHS",
    "branchCode": "HEAD"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response:
{
  "id": "account-guid",
  "accountNumber": "ACC001234567890",
  "accountType": "Savings",
  "balance": 500.00,
  "availableBalance": 500.00,
  "currency": "GHS",
  "status": "Active",
  "openedAt": "2024-01-15T10:30:00Z"
}
```

### 2. Get Account Balance

```bash
ACCOUNT_ID="account-guid-here"

curl -X GET https://localhost:5001/api/v1/accounts/$ACCOUNT_ID/balance \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 3. Create Mirrored Wallet

```bash
curl -X POST https://localhost:5001/api/v1/wallets \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "customerId": "'$CUSTOMER_ID'",
    "walletType": "Personal",
    "mobileNumber": "+233201234567",
    "mirroredAccountId": "'$ACCOUNT_ID'",
    "dailyLimit": 5000.00,
    "monthlyLimit": 50000.00
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 💸 Transaction Processing Testing

### 1. Internal Transfer

```bash
# Transfer between accounts
curl -X POST https://localhost:5001/api/v1/transactions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "transactionType": "Transfer",
    "amount": 100.00,
    "currency": "GHS",
    "debitAccountId": "'$ACCOUNT_ID'",
    "creditAccountId": "destination-account-id",
    "description": "Monthly savings transfer",
    "narration": "Personal savings",
    "channel": "MobileBanking"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response:
{
  "id": "transaction-guid",
  "transactionReference": "TXN20240115001234",
  "transactionType": "Transfer",
  "amount": 100.00,
  "fee": 2.00,
  "totalAmount": 102.00,
  "status": "Completed",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

### 2. GHIPSS Interbank Transfer

```bash
curl -X POST https://localhost:5001/api/v1/transactions/interbank \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "debitAccountId": "'$ACCOUNT_ID'",
    "creditAccount": "0123456789",
    "creditBankCode": "GCB",
    "amount": 500.00,
    "currency": "GHS",
    "beneficiaryName": "John Doe",
    "narration": "Payment for services",
    "priority": "NORMAL"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 3. Mobile Money Transaction

```bash
curl -X POST https://localhost:5001/api/v1/transactions/mobile-money \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "walletId": "wallet-guid-here",
    "transactionType": "CashOut",
    "amount": 50.00,
    "recipientMobile": "+233207654321",
    "provider": "MTN",
    "description": "Mobile money transfer"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 4. Transaction Status Check

```bash
TRANSACTION_ID="transaction-guid-here"

curl -X GET https://localhost:5001/api/v1/transactions/$TRANSACTION_ID/status \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 🏠 Loan Management Testing

### 1. Apply for Loan

```bash
curl -X POST https://localhost:5001/api/v1/loans \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "customerId": "'$CUSTOMER_ID'",
    "loanType": "PersonalLoan",
    "principalAmount": 10000.00,
    "tenureMonths": 12,
    "interestRate": 24.0,
    "purpose": "Home improvement",
    "collateralDescription": "Salary assignment",
    "disbursementAccountId": "'$ACCOUNT_ID'",
    "repaymentAccountId": "'$ACCOUNT_ID'"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response:
{
  "id": "loan-guid",
  "loanNumber": "LOAN20240115001",
  "loanType": "PersonalLoan",
  "principalAmount": 10000.00,
  "interestRate": 24.0,
  "tenureMonths": 12,
  "monthlyInstallment": 953.68,
  "status": "Applied",
  "applicationDate": "2024-01-15T10:30:00Z"
}
```

### 2. Loan Approval (Maker-Checker)

```bash
LOAN_ID="loan-guid-here"

# First level approval
curl -X POST https://localhost:5001/api/v1/loans/$LOAN_ID/approve \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "approvalLevel": "FirstLevel",
    "approvalStatus": "Approved",
    "approvedAmount": 10000.00,
    "approvedInterestRate": 24.0,
    "approvedTenure": 12,
    "comments": "Customer meets all criteria for loan approval",
    "riskScore": 0.85,
    "collateralValuation": 15000.00
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 3. Loan Disbursement

```bash
curl -X POST https://localhost:5001/api/v1/loans/$LOAN_ID/disburse \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "disbursementAmount": 10000.00,
    "disbursementAccountId": "'$ACCOUNT_ID'",
    "disbursementDate": "2024-01-15T10:30:00Z",
    "processingFee": 100.00
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 4. Loan Payment

```bash
curl -X POST https://localhost:5001/api/v1/loans/$LOAN_ID/payments \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "paymentAmount": 953.68,
    "paymentMethod": "BankTransfer",
    "sourceAccountId": "'$ACCOUNT_ID'",
    "paymentDescription": "Monthly loan payment"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 💳 Bill Payment Testing

### 1. Pay Utility Bill

```bash
curl -X POST https://localhost:5001/api/v1/payments/bills \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "accountId": "'$ACCOUNT_ID'",
    "billType": "Electricity",
    "billerCode": "ECG",
    "customerReference": "12345678901",
    "amount": 150.00,
    "currency": "GHS",
    "description": "Monthly electricity bill payment"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 2. Airtime Top-up

```bash
curl -X POST https://localhost:5001/api/v1/payments/airtime \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -d '{
    "accountId": "'$ACCOUNT_ID'",
    "mobileNumber": "+233201234567",
    "provider": "MTN",
    "amount": 20.00,
    "currency": "GHS"
  }' \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 📊 Reporting & Analytics Testing

### 1. Account Statement

```bash
curl -X GET "https://localhost:5001/api/v1/accounts/$ACCOUNT_ID/statement?fromDate=2024-01-01&toDate=2024-01-31" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 2. Transaction History

```bash
curl -X GET "https://localhost:5001/api/v1/transactions?accountId=$ACCOUNT_ID&page=1&size=20" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 3. Customer Portfolio

```bash
curl -X GET https://localhost:5001/api/v1/customers/$CUSTOMER_ID/portfolio \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 🛡️ Security Testing

### 1. Rate Limiting Test

```bash
# Test rate limiting by making rapid requests
for i in {1..10}; do
  curl -X GET https://localhost:5001/api/v1/customers \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -w "\nRequest $i - HTTP: %{http_code}\n" \
    -s -o /dev/null
done
```

### 2. Invalid Token Test

```bash
# Test with invalid token
curl -X GET https://localhost:5001/api/v1/customers \
  -H "Authorization: Bearer invalid-token" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response: HTTP 401 Unauthorized
```

### 3. Permission Test

```bash
# Test accessing admin endpoints with customer token
curl -X GET https://localhost:5001/api/v1/admin/users \
  -H "Authorization: Bearer $CUSTOMER_TOKEN" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'

# Expected Response: HTTP 403 Forbidden
```

## 🔍 Monitoring & Observability Testing

### 1. Health Check Details

```bash
curl -X GET https://localhost:5001/health \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

### 2. Metrics Endpoint

```bash
curl -X GET https://localhost:5001/metrics \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n"
```

### 3. API Information

```bash
curl -X GET https://localhost:5001/api/v1/info \
  -H "Accept: application/json" \
  -w "\nHTTP: %{http_code}\nTime: %{time_total}s\n" | jq '.'
```

## 🧪 Load Testing Example

```bash
# Simple load test using Apache Bench (if available)
ab -n 100 -c 10 -H "Authorization: Bearer $ACCESS_TOKEN" https://localhost:5001/health

# Or using curl in a loop
for i in {1..100}; do
  curl -s -X GET https://localhost:5001/health \
    -H "Authorization: Bearer $ACCESS_TOKEN" \
    -w "%{time_total}s\n" -o /dev/null &
done
wait
```

## 📋 Test Results Validation

### Success Criteria:
- **Authentication**: All login attempts return valid JWT tokens
- **Customer Management**: CRUD operations complete successfully
- **Account Operations**: Balance updates reflect correctly
- **Transaction Processing**: All transactions complete with proper status
- **Loan Management**: Approval workflow functions correctly
- **Security**: Rate limiting and authorization work as expected
- **Performance**: API responses under 500ms for most operations
- **Compliance**: All operations logged for audit trail

### Error Handling Verification:
- **400 Bad Request**: Invalid input data
- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Insufficient permissions
- **404 Not Found**: Resource doesn't exist
- **409 Conflict**: Business rule violations
- **429 Too Many Requests**: Rate limiting active
- **500 Internal Server Error**: System errors with proper logging

## 🎯 Next Steps

After successful API testing:
1. **Performance Testing**: Use tools like JMeter or k6 for comprehensive load testing
2. **Security Testing**: Conduct penetration testing and vulnerability assessments
3. **Integration Testing**: Test with real external services (sandbox environments)
4. **User Acceptance Testing**: End-to-end workflow testing
5. **Production Deployment**: Deploy to staging and production environments

---

This comprehensive testing guide ensures all aspects of the BoG-compliant banking platform are thoroughly validated before production deployment. 🚀