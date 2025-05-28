LendTech.API
لایه API شامل Controllers، Middleware، Filters و تنظیمات مربوط به Web API است.
ساختار پروژه
LendTech.API/
├── Controllers/       # API Controllers
├── Middleware/       # Custom Middleware
├── Filters/          # Action & Authorization Filters
├── Extensions/       # Extension Methods for DI
├── Policies/         # Authorization Policies
├── HealthChecks/     # Custom Health Checks
├── Program.cs        # Application Entry Point
├── appsettings.json  # Configuration
└── Dockerfile        # Docker Configuration
ویژگی‌های کلیدی
Security

JWT Authentication
Permission-based Authorization
Security Headers (HSTS, CSP, XSS Protection)
CORS Configuration
Rate Limiting

Monitoring & Observability

Structured Logging with Serilog
Health Checks (SQL, Redis, RabbitMQ)
Prometheus Metrics
OpenTelemetry Tracing with Jaeger
Request/Response Logging

API Features

Swagger/OpenAPI Documentation
API Versioning
Global Exception Handling
Request Localization (fa-IR, en-US)

Middleware Pipeline

Security Headers
HTTPS Redirection
Serilog Request Logging
Exception Handling
CORS
Rate Limiting
Authentication & Authorization
Request Localization

Endpoints
Health Check Endpoints

/health - Health Check API
/health-ui - Health Check Dashboard
/metrics - Prometheus Metrics

API Documentation

/ or /swagger - Swagger UI (Development only)

Rate Limiting Policies
Global Policy

100 requests per minute per user/IP

PerIpPolicy

60 requests per minute per IP

PerUserPolicy

200 requests per minute per authenticated user

SensitiveApiPolicy

10 requests per 5 minutes for sensitive operations

Security Headers

X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 1; mode=block
Content-Security-Policy: Restrictive CSP
Strict-Transport-Security: HSTS (Production only)

Docker Support
Build and run with Docker:
bashdocker build -t lendtech-api .
docker run -p 8080:80 -p 8443:443 lendtech-api
Environment Variables

ASPNETCORE_ENVIRONMENT: Development/Staging/Production
ConnectionStrings__DefaultConnection: SQL Server connection
ConnectionStrings__Redis: Redis connection
Jwt__SecretKey: JWT signing key
RabbitMQ__HostName: RabbitMQ host
