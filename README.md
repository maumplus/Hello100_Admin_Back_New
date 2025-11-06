# Hello100Admin - 모듈러 모놀리스 아키텍처 프로젝트

## 📋 프로젝트 개요

Hello100Admin은 닷넷 코어 8.0 기반의 **모듈러 모놀리스(Modular Monolith)** 아키텍처 관리 시스템입니다.

> **전략**: 하나의 애플리케이션으로 실행하되, 각 모듈을 독립적으로 구성하여 추후 마이크로서비스로 쉽게 분리할 수 있도록 설계합니다.

### 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│               Hello100Admin API (단일 프로세스)               │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              API Layer (Controllers)                 │   │
│  └───────────┬────────────────────┬────────────────────┘   │
│              │                    │                          │
│  ┌───────────┴──────┐  ┌─────────┴──────────┐              │
│  │   Auth Module    │  │   Logging Module   │              │
│  │  (독립 도메인)    │  │   (독립 도메인)     │              │
│  └──────────────────┘  └────────────────────┘              │
│              │                    │                          │
│  ┌───────────┴──────────────────────────────┐              │
│  │       Business Modules (독립 도메인)       │              │
│  └───────────────────────────────────────────┘              │
│              │                                                │
│  ┌───────────┴──────────────────────────────┐              │
│  │    Internal Event Bus (In-Memory)        │              │
│  │    모듈간 통신 (MediatR/In-Process)        │              │
│  └───────────────────────────────────────────┘              │
│              │                                                │
│  ┌───────────┴──────────────────────────────┐              │
│  │      Shared Database (다른 스키마)         │              │
│  │   Auth Schema | Business Schema | ...    │              │
│  └───────────────────────────────────────────┘              │
└─────────────────────────────────────────────────────────────┘
```

**장점:**
- ✅ 단일 배포로 운영 간소화
- ✅ 개발 및 디버깅 용이
- ✅ 트랜잭션 관리 간단
- ✅ 모듈 간 직접 호출 가능 (낮은 레이턴시)
- ✅ 추후 마이크로서비스 전환 용이

## 🏗️ 프로젝트 구조 (2025-10-30 최신)

```
Hello100Admin/
├── src/
│   ├── BuildingBlocks/
│   │   └── Common/
│   ├── Modules/
│   │   ├── Auth/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   └── Infrastructure/
│   │   ├── Admin/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   └── Infrastructure/
│   │   └── ... (송금, 키오스크&데스크 등)
│   └── API/
│       ├── Controllers/
│       ├── Modules/
│       └── Program.cs
└── tests/
    ├── Unit/
    │   ├── Admin.Application.UnitTests/
    │   ├── Admin.Domain.UnitTests/
    │   ├── Admin.Infrastructure.UnitTests/
    │   ├── Auth.Application.UnitTests/
    │   ├── Auth.Domain.UnitTests/
    │   └── Auth.Infrastructure.UnitTests/
    └── Integration/
        ├── Auth.API.IntegrationTests/
        └── Shared/
            └── Builders/
```

---

## 🔄 마이크로서비스 전환 전략

추후 마이크로서비스로 전환 시:

1. **각 모듈을 독립 프로젝트로 분리**
   - Modules/Auth → Services/Auth/API
   - 각 모듈의 API 프로젝트 생성

2. **데이터베이스 분리**
   - 스키마 → 독립 데이터베이스

3. **통신 방식 변경**
   - 직접 호출 → HTTP/gRPC/메시지 브로커
   - 필요 시 EventBus 추가 (RabbitMQ, Azure Service Bus 등)

4. **API Gateway 추가**
   - Ocelot 또는 YARP
   - 라우팅 및 인증 통합

## 🛠️ 기술 스택

### 핵심 기술
- **.NET 8.0**: 플랫폼
- **ASP.NET Core Web API**: REST API
- **Dapper**: 경량 ORM (EF Core → Dapper로 전환)
- **MySQL**: 데이터베이스 (모듈별 스키마 분리)

### 모듈러 모놀리스 전용
- **MediatR**: CQRS 패턴 
- **JWT**: 인증/인가
- **Serilog**: 구조화된 로깅
- **AutoMapper**: DTO 매핑 (ManualMapper 사용)

### 개발 도구
- **Swagger/OpenAPI**: API 문서화
- **xUnit**: 단위/통합 테스트
- **Docker**: 컨테이너화

### 향후 마이크로서비스 전환 시
- **RabbitMQ/MassTransit**: 메시징
- **Ocelot/YARP**: API 게이트웨이
- **gRPC**: 서비스 간 통신
- **Kubernetes**: 오케스트레이션

## 🧪 테스트

### 테스트 실행 예시

```bash
# 모든 테스트 실행
$ dotnet test

# Admin 모듈 단위 테스트
$ dotnet test tests/Unit/Admin.Application.UnitTests/
$ dotnet test tests/Unit/Admin.Domain.UnitTests/
$ dotnet test tests/Unit/Admin.Infrastructure.UnitTests/

# Auth 모듈 단위 테스트
$ dotnet test tests/Unit/Auth.Application.UnitTests/
$ dotnet test tests/Unit/Auth.Domain.UnitTests/
$ dotnet test tests/Unit/Auth.Infrastructure.UnitTests/

# 통합 테스트
$ dotnet test tests/Integration/Auth.API.IntegrationTests/
```

**테스트 도구 및 프레임워크**
- xUnit 2.5.3
- Moq 4.20.70
- FluentAssertions 8.8.0
- Microsoft.AspNetCore.Mvc.Testing 8.0.11

> 📖 **상세 가이드**: [TESTING.md](docs/TESTING.md) 참조

---

## 📦 빌드 및 실행

```bash
# 1. 클론 및 빌드 
git clone <repository-url>
cd renew_admin
dotnet restore && dotnet build

# 2. User Secrets 설정 🔐
./setup-secrets.sh

# AES 키가 자동 생성되고 User Secrets에 저장됩니다
# 이제 MySQL과 JWT Secret만 설정하면 됩니다:
cd src/API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=hello100_dev;User=root;Password=dev_password;AllowPublicKeyRetrieval=true;SslMode=none;"

dotnet user-secrets set "Jwt:SecretKey" \
  "your-super-secret-jwt-key-at-least-32-characters-long"


# 3. 실행
cd ../..
./run.sh  # macOS/Linux
# 또는
.\run.ps1  # Windows


# 5. 첫 API 호출
curl http://localhost:5000/api/auth/health
```

**✅ 성공!** 자세한 내용은 [QUICK_START.md](docs/QUICK_START.md) 참고

---

## 🗃️ 데이터 접근

### ORM: Dapper

- 본 프로젝트는 **Dapper** 기반의 Repository 패턴을 사용합니다.
- EF Core는 더 이상 사용하지 않으며, 모든 데이터 접근은 Dapper 쿼리 및 IDbConnectionFactory를 통해 수행합니다.

#### 예시: Dapper Repository
```csharp
public class MemberRepository : IMemberRepository
{
  private readonly IDbConnectionFactory _connectionFactory;

  public MemberRepository(IDbConnectionFactory connectionFactory)
  {
    _connectionFactory = connectionFactory;
  }

  public async Task<Member?> GetByIdAsync(string uid, CancellationToken cancellationToken = default)
  {
    using var connection = _connectionFactory.CreateConnection();
    const string sql = "SELECT * FROM Members WHERE Uid = @Uid";
    return await connection.QuerySingleOrDefaultAsync<Member>(sql, new { Uid = uid });
  }
}
```

### 🔐 보안 설정 (User Secrets)

**중요**: 민감한 정보(DB 비밀번호, 암호화 키, JWT Secret)는 **절대 코드에 직접 작성하지 마세요!**

#### User Secrets 자동 설정 (권장)

```bash
# 루트 폴더에서 실행
./setup-secrets.sh
```

이 스크립트는 자동으로:
- ✅ **AES-256 암호화 키 2개** 생성 (Default, EmailName - 각 32바이트, Base64)
- ✅ **DES 파라미터 키** 설정 (8바이트 ASCII - 레거시 시스템과 동일한 값 필요)
- ✅ **Zero IV** 사용 (레거시 시스템 호환)
- ✅ User Secrets에 안전하게 저장
- ✅ 각 개발자 PC에서 독립적으로 관리

#### 암호화 키 구조

| 키 타입 | 설정 키 | 알고리즘 | 용도 |
|---------|---------|----------|------|
| **Default Key** | `Crypto:Key:Default` | AES-256 | 일반 데이터 암호화 |
| **EmailName Key** | `Crypto:Key:EmailName` | AES-256 | 이메일/이름 암호화 |
| **Parameter Key** | `Crypto:Key:Parameter` | DES | URL 파라미터 암호화 (레거시) |
| **IV** | `Crypto:IV` | - | Zero IV (모든 AES 키 공통) |

#### User Secrets 수동 설정

```bash
cd src/API

# User Secrets 초기화
dotnet user-secrets init

# AES-256 키 (Default - 일반 데이터)
dotnet user-secrets set "Crypto:Key:Default" "$(openssl rand -base64 32)"

# AES-256 키 (EmailName - 이메일/이름 전용)
dotnet user-secrets set "Crypto:Key:EmailName" "$(openssl rand -base64 32)"

# Zero IV (레거시 호환 - 고정값)
dotnet user-secrets set "Crypto:IV" "AAAAAAAAAAAAAAAAAAAAAA=="

# DES 파라미터 키 (8바이트 ASCII - 레거시 시스템과 동일한 값 사용!)
dotnet user-secrets set "Crypto:Key:Parameter" "12345678"

# MySQL 연결 정보
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Server=localhost;Port=3306;Database=hello100;User=root;Password=YOUR_PASSWORD;AllowPublicKeyRetrieval=true;SslMode=none;"

# JWT Secret (32자 이상 권장)
dotnet user-secrets set "Jwt:SecretKey" \
  "your-super-secret-jwt-key-at-least-32-characters-long"

# 설정 확인
dotnet user-secrets list
```

**⚠️ 중요**: 
- `Crypto:Key:Parameter`는 레거시 시스템의 `_ParamEncKey`와 **정확히 동일한 값**을 사용해야 합니다
- 이 값은 DES 암호화에 사용되며 Key와 IV에 동일하게 적용됩니다

#### User Secrets 위치

- **macOS/Linux**: `~/.microsoft/usersecrets/<project-id>/secrets.json`
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<project-id>\secrets.json`

#### appsettings.json은 Git에 포함

`appsettings.json`에는 플레이스홀더 값만 있습니다:
```json
{
  "Crypto": {
    "Key": {
      "Default": "REPLACE_WITH_YOUR_BASE64_KEY_32_BYTES",
      "EmailName": "REPLACE_WITH_YOUR_BASE64_KEY_32_BYTES",
      "Parameter": "REPLACE_WITH_8_BYTE_ASCII_KEY"
    },
    "IV": "AAAAAAAAAAAAAAAAAAAAAA=="
  }
}
```

실제 값은 User Secrets에서 자동으로 오버라이드됩니다.

**장점**:
- 🔒 민감 정보가 Git에 커밋되지 않음
- 👥 개발자마다 다른 설정 사용 가능
- 🚀 프로덕션은 환경 변수로 관리

### 개발 환경 설정

자세한 환경 설정 가이드는 [DEVELOPMENT_GUIDE.md](docs/DEVELOPMENT_GUIDE.md) 참고

### 빠른 시작

```bash
# 루트 폴더에서 실행 (추천)
./run.sh                    # macOS/Linux
.\run.ps1                   # Windows PowerShell

# 또는 직접 실행
dotnet run --project src/API

# 또는 API 폴더에서 실행
cd src/API
dotnet run
```

### 빌드

```bash
# 솔루션 전체 빌드
dotnet build

# Release 빌드
dotnet build -c Release

# 특정 프로젝트만 빌드
dotnet build src/API
```

### Docker로 실행

```bash
docker-compose up
```

### 포트 설정
- **API**: http://localhost:5000, https://localhost:5001
- **Swagger**: https://localhost:5001/swagger

### API 테스트

```bash
# Health Check
curl http://localhost:5000/api/auth/health

# Customers API
curl http://localhost:5000/api/customers?page=1&pageSize=10

# Members API
curl http://localhost:5000/api/members/1
```

**더 많은 예제**: [Hello100Admin.API.http](src/API/Hello100Admin.API.http) 파일 참고

## 📚 참고 자료

### 프로젝트 문서

#### 시작 가이드
- 📖 **[README.md](README.md)** - 프로젝트 개요 (이 문서)
- ⚡ **[QUICK_START.md](docs/QUICK_START.md)** - 5분 빠른 시작
- 📚 **[ONBOARDING.md](docs/ONBOARDING.md)** - 신규 개발자 5일 학습 커리큘럼
- 🛠️ **[DEVELOPMENT_GUIDE.md](docs/DEVELOPMENT_GUIDE.md)** - 개발 실무 가이드

#### 아키텍처 문서
- 🏗️ **[SOLUTION_ARCHITECTURE.md](docs/SOLUTION_ARCHITECTURE.md)** - 전체 아키텍처
- 🧱 **[BUILDINGBLOCKS.md](docs/BUILDINGBLOCKS.md)** - 공통 인프라
- 🌐 **[API_LAYER.md](docs/API_LAYER.md)** - API 계층 가이드

#### 모듈 문서
- 🔐 **[AUTH_MODULE.md](docs/AUTH_MODULE.md)** - 인증 모듈
- 👥 **[CUSTOMER_MODULE_PHASE2.md](docs/CUSTOMER_MODULE_PHASE2.md)** - 고객 모듈 완전 가이드

#### 개발 문서
- 📝 **[LOGGING.md](docs/LOGGING.md)** - 로깅 전략
- 🧪 **[TESTING.md](docs/TESTING.md)** - 테스트 전략
- 🚀 **[DEPLOYMENT.md](docs/DEPLOYMENT.md)** - 배포 가이드

### 외부 자료

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Microservices](https://microservices.io/)

## 📝 라이선스

MIT License

---

## 변경 이력

| 날짜 | 버전 | 변경 내용 | 작성자 |
|------|------|-----------|--------|
| 2025-10-30 | 1.0.0 | 초기 문서 작성 | Hello100Admin Team |
