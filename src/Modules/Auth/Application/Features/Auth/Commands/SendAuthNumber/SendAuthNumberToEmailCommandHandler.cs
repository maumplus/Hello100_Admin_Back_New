using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.LoginCheck;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.SendAuthNumber;
using Hello100Admin.Modules.Auth.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Commands.SendAuthNumberToEmail
{
    public class SendAuthNumberToEmailCommandHandler : IRequestHandler<SendAuthNumberToEmailCommand, Result<SendAuthNumberResponse>>
    {
        private readonly ILogger<SendAuthNumberToEmailCommandHandler> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly IHasher _sha256Hasher;
        private readonly IConfiguration _config;

        private readonly string _eghisSmtpServer;
        private readonly string _eghisSmtpPort;
        private readonly string _eghisEmailAccount;
        private readonly string _eghisEmailPassword;

        public SendAuthNumberToEmailCommandHandler(ILogger<SendAuthNumberToEmailCommandHandler> logger, IAuthRepository authRepository, IHasher sha256Hasher, IConfiguration config)
        {
            _logger = logger;
            _authRepository = authRepository;
            _sha256Hasher = sha256Hasher;
            _config = config;

            _eghisSmtpServer = _config["EghisSmtp:Server"] ?? string.Empty;
            _eghisSmtpPort = _config["EghisSmtp:port"] ?? string.Empty;
            _eghisEmailAccount = _config["EghisSmtp:Account"] ?? string.Empty;
            _eghisEmailPassword = _config["EghisSmtp:Password"] ?? string.Empty;
        }

        private int MakeRandom(int length)
        {
            Random random = new Random();

            int tmp = 0;

            for (int i = 0; i < length; i++)
            {
                tmp += (int)(random.Next(10) * Math.Pow(10, i));
            }

            return tmp;
        }

        private string MakeAuthCode()
        {
            return string.Format("{0:000000}", MakeRandom(6));
        }

        public async Task<Result<SendAuthNumberResponse>> Handle(SendAuthNumberToEmailCommand request, CancellationToken cancellationToken)
        {
            var authNumber = MakeAuthCode();

            var AppAuthNumberInfo = new AppAuthNumberInfoEntity()
            {
                AppCd = request.AppCd,
                Key = _sha256Hasher.HashWithSalt(request.Email, request.AppCd),
                AuthNumber = _sha256Hasher.HashWithSalt(authNumber, request.AppCd)
            };

            int? authId = null;

            try
            {
                authId = await _authRepository.InsertAsync(AppAuthNumberInfo, cancellationToken);
            }
            catch
            {
                return Result.Success<SendAuthNumberResponse>().WithError(GlobalErrorCode.InvalidVerificationCode.ToError());
            }

            if (authId == null || authId == 0)
            {
                return Result.Success<SendAuthNumberResponse>().WithError(GlobalErrorCode.FailedRequestVerificationCode.ToError());
            }

            MailMessage mailMessage = new MailMessage();

            try
            {
                var html = $@"
                <html>
                  <body
                    style=""
                      font-family: Arial, sans-serif;
                      background-color: #f7f9fc;
                      margin: 0;
                      padding: 0;
                    ""
                  >
                    <div
                      style=""
                        max-width: 600px;
                        margin: 40px auto;
                        background: #ffffff;
                        border-radius: 10px;
                        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);
                        padding: 30px;
                      ""
                    >
                      <div
                        style=""
                          text-align: center;
                          font-size: 20px;
                          font-weight: bold;
                          color: #2c3e50;
                          margin: 50px;
                          padding: 0;
                        ""
                      >
                        <div class=""text-center my-4"">
                          <img src=""https://r-admin.hello100.kr/Content/img/login_logo.png"" alt=""Hello100 Logo"" style=""max-width: 100%"" />
                        </div>
                      </div>
                      <p
                        style=""
                          color: #000;
                          text-align: center;
                          font-size: 13px;
                          font-style: normal;
                          font-weight: 400;
                          line-height: 150%; /* 24px */
                          letter-spacing: -0.176px;
                        ""
                      >
                        안녕하세요.<br />
                        <b>헬로100 관리자 페이지 로그인을 위한 인증번호</b>는 아래와 같습니다.
                      </p>

                      <div style=""text-align: center; margin: 30px 0"">
                        <div
                          style=""
                            display: inline-block;
                            background: #f0f4ff;
                            border: 1px solid #d0d7e6;
                            border-radius: 8px;
                            padding: 20px 40px;
                            font-size: 18px;
                            font-weight: 800;
                            letter-spacing: 4px;
                            color: #2c3e50;
                          ""
                        >
                          {authNumber}
                        </div>
                      </div>

                      <p
                        style=""
                          font-size: 11px;
                          color: #666;
                          text-align: center;
                          line-height: 1.6;
                        ""
                      >
                        본 인증번호는 <b>3분간 유효</b>합니다.<br />
                        본인이 요청하지 않았다면, 본 메일을 무시하시기 바랍니다.
                      </p>

                      <hr style=""border: none; border-top: 1px solid #eee; margin: 25px 0"" />
                      <p style=""font-size: 12px; color: #999; text-align: center"">
                        © 이지스헬스케어. All rights reserved.
                      </p>
                    </div>
                  </body>
                </html>";

                
                // 메일 구성  
                mailMessage.From = new MailAddress(_eghisEmailAccount);
                mailMessage.To.Add(request.Email);
                mailMessage.Subject = "[헬로100] 관리자 페이지 로그인 인증번호";
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.Body = html;
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = Encoding.UTF8;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                SmtpClient SmtpServer = new SmtpClient(_eghisSmtpServer)
                {
                    Port = Convert.ToInt32(_eghisSmtpPort),
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(_eghisEmailAccount, _eghisEmailPassword)
                };

                SmtpServer.Send(mailMessage);
            }
            catch
            {

            }
            finally
            {
                foreach (var attach in mailMessage.Attachments)
                {
                    attach.ContentStream.Close();
                }
            }

            var response = new SendAuthNumberResponse()
            {
                AuthId = authId.Value
            };

            return Result.Success(response);
        }
    }
}
