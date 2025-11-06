// using Hello100Admin.Services.Auth.Application.Commands.Login;
// using Hello100Admin.Services.Auth.Application.Commands.RefreshToken;
// using Hello100Admin.Services.Auth.Application.Commands.Register;

// namespace Hello100Admin.Integration.Shared.Builders;

// /// <summary>
// /// Auth Module 테스트 데이터 빌더
// /// Fluent API로 테스트 데이터 생성을 간소화하고 가독성 향상
// /// </summary>
// public class AuthTestDataBuilder
// {
//     #region RegisterCommand Builder

//     public class RegisterCommandBuilder
//     {
//         private string? _accountId;
//         private string _password = "Test@1234";
//         private string _name = "Test User";
//         private string _grade = "C";
//         private string? _phoneNumber;
//         private string? _hospitalNumber;

//         public RegisterCommandBuilder WithRandomEmail()
//         {
//             _accountId = $"test{Guid.NewGuid():N}@test.com";
//             return this;
//         }

//         public RegisterCommandBuilder WithEmail(string email)
//         {
//             _accountId = email;
//             return this;
//         }

//         public RegisterCommandBuilder WithPassword(string password)
//         {
//             _password = password;
//             return this;
//         }

//         public RegisterCommandBuilder WithName(string name)
//         {
//             _name = name;
//             return this;
//         }

//         public RegisterCommandBuilder WithGrade(string grade)
//         {
//             _grade = grade;
//             return this;
//         }

//         public RegisterCommandBuilder WithPhoneNumber(string phoneNumber)
//         {
//             _phoneNumber = phoneNumber;
//             return this;
//         }

//         public RegisterCommandBuilder WithHospitalNumber(string hospitalNumber)
//         {
//             _hospitalNumber = hospitalNumber;
//             return this;
//         }

//         public RegisterCommand Build()
//         {
//             return new RegisterCommand
//             {
//                 AccountId = _accountId ?? $"test{Guid.NewGuid():N}@test.com",
//                 Password = _password,
//                 Name = _name,
//                 Grade = _grade,
//                 PhoneNumber = _phoneNumber,
//                 HospitalNumber = _hospitalNumber
//             };
//         }
//     }

//     #endregion

//     #region LoginCommand Builder

//     public class LoginCommandBuilder
//     {
//         private string _accountId = string.Empty;
//         private string _password = "Test@1234";

//         public LoginCommandBuilder WithAccountId(string accountId)
//         {
//             _accountId = accountId;
//             return this;
//         }

//         public LoginCommandBuilder WithPassword(string password)
//         {
//             _password = password;
//             return this;
//         }

//         public LoginCommand Build()
//         {
//             return new LoginCommand
//             {
//                 AccountId = _accountId,
//                 Password = _password
//             };
//         }
//     }

//     #endregion

//     #region RefreshTokenCommand Builder

//     public class RefreshTokenCommandBuilder
//     {
//         private string _refreshToken = string.Empty;

//         public RefreshTokenCommandBuilder WithRefreshToken(string refreshToken)
//         {
//             _refreshToken = refreshToken;
//             return this;
//         }

//         public RefreshTokenCommand Build()
//         {
//             return new RefreshTokenCommand
//             {
//                 RefreshToken = _refreshToken
//             };
//         }
//     }

//     #endregion

//     #region Static Factory Methods

//     /// <summary>
//     /// RegisterCommand 빌더 시작
//     /// </summary>
//     public static RegisterCommandBuilder RegisterCommand() => new();

//     /// <summary>
//     /// 유효한 RegisterCommand 생성 (기본값)
//     /// </summary>
//     public static RegisterCommand ValidRegisterCommand() =>
//         RegisterCommand().WithRandomEmail().Build();

//     /// <summary>
//     /// 중복 테스트용 RegisterCommand (같은 이메일)
//     /// </summary>
//     public static RegisterCommand DuplicateRegisterCommand(string email) =>
//         RegisterCommand().WithEmail(email).Build();

//     /// <summary>
//     /// 잘못된 이메일 형식의 RegisterCommand
//     /// </summary>
//     public static RegisterCommand InvalidEmailCommand() =>
//         RegisterCommand().WithEmail("invalid-email").Build();

//     /// <summary>
//     /// 약한 비밀번호 RegisterCommand
//     /// </summary>
//     public static RegisterCommand WeakPasswordCommand() =>
//         RegisterCommand().WithRandomEmail().WithPassword("123").Build();

//     /// <summary>
//     /// LoginCommand 빌더 시작
//     /// </summary>
//     public static LoginCommandBuilder LoginCommand() => new();

//     /// <summary>
//     /// 유효한 LoginCommand 생성
//     /// </summary>
//     public static LoginCommand ValidLoginCommand(string accountId, string password = "Test@1234") =>
//         LoginCommand().WithAccountId(accountId).WithPassword(password).Build();

//     /// <summary>
//     /// 잘못된 비밀번호 LoginCommand
//     /// </summary>
//     public static LoginCommand InvalidPasswordLoginCommand(string accountId) =>
//         LoginCommand().WithAccountId(accountId).WithPassword("WrongPassword123!").Build();

//     /// <summary>
//     /// RefreshTokenCommand 빌더 시작
//     /// </summary>
//     public static RefreshTokenCommandBuilder RefreshTokenCommand() => new();

//     /// <summary>
//     /// 유효한 RefreshTokenCommand 생성
//     /// </summary>
//     public static RefreshTokenCommand ValidRefreshTokenCommand(string refreshToken) =>
//         RefreshTokenCommand().WithRefreshToken(refreshToken).Build();

//     /// <summary>
//     /// 잘못된 RefreshTokenCommand
//     /// </summary>
//     public static RefreshTokenCommand InvalidRefreshTokenCommand() =>
//         RefreshTokenCommand().WithRefreshToken("invalid-token-12345").Build();

//     #endregion
// }
