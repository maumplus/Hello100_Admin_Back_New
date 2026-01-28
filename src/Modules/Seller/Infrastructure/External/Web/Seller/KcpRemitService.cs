using System.Globalization;
using System.Reflection;
using System.Text;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Request;
using Hello100Admin.Modules.Seller.Application.Common.Constracts.External.Web.Seller.Result;
using Hello100Admin.Modules.Seller.Infrastructure.Configuration.Options;
using Hello100Admin.Modules.Seller.Infrastructure.External.Http;
using Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Request;
using Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller.Models.KcpRemit.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;

namespace Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller
{
    public class KcpRemitService : IKcpRemitService
    {
        #region FIELD AREA ************************************************
        private readonly ILogger<KcpRemitService> _logger;
        private readonly IWebRequestService _webRequestService;
        private readonly KcpRemitOptions _kcpOptions;
        #endregion

        #region CONSTRUCTOR AREA ********************************************
        public KcpRemitService(
            ILogger<KcpRemitService> logger,
            IWebRequestService webRequestService,
            IOptions<KcpRemitOptions> kcpOptions)
        {
            _logger = logger;
            _webRequestService = webRequestService;
            _kcpOptions = kcpOptions.Value;
        }
        #endregion

        #region GENERAL METHOD AREA **********************************************
        public async Task<SellerRegisterResult?> RegisterSellerAsync(SellerRegisterRequest request)
        {
            var endPoint = "/std/submall/write";

            KcpSellerRegisterRequest parsedRequest = new KcpSellerRegisterRequest
            {
                seller_id = request.SellerId,
                seller_level = request.SellerLevel,
                seller_name = request.SellerName,
                site_cd = request.SiteCd,
                address = request.Address,
                bank_cd = request.BankCd,
                depositor = request.Depositor,
                deposit_no = request.DepositNo,
                kcp_cert_info = request.KcpCertInfo,
                own_name = request.OwnName,
                tax_no = request.TaxNo,
                tel_no = request.TelNo,
            };

            try
            {
                var built = Build(parsedRequest);
                var json = built.ToJson();

                var res = await _webRequestService.PostAsync(GetUrl(endPoint), json);

                if (string.IsNullOrWhiteSpace(res.ResponseData))
                    return default;
                
                KcpSellerRegisterResponse? tempResult = JsonSerializer.Deserialize<KcpSellerRegisterResponse>(res.ResponseData);

                SellerRegisterResult? result = null;

                if (tempResult != null)
                {
                    result = new SellerRegisterResult
                    {
                        ResCd = tempResult.res_cd,
                        ResMsg = tempResult.res_msg,
                        ResEnMsg = tempResult.res_en_msg
                    };
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "KCP 판매자 등록 중 오류 발생");
                return null;
            }
        }

        // 개발 : https://stg-spl.kcp.co.kr/gw/hub/v1/payment
        public async Task<RemitResult?> SendRemitAsync(RemitRequest request)
        {
            var endPoint = "/gw/hub/v1/payment";

            KcpRemitRequest parsedRequest = new KcpRemitRequest
            {
                site_cd = request.SiteCd,
                kcp_cert_info = request.KcpCertInfo,
                pay_method = request.PayMethod,
                cust_ip = request.CustIp,
                remit_type = request.RemitType,
                currency = request.Currency,
                seller_id = request.SellerId,
                amount = request.Amount,
                va_txtype = request.VaTxtype,
                va_name = request.VaName,
                va_mny = request.VaMny,
            };

            try
            {
                var built = Build(parsedRequest);
                var json = built.ToJson();

                var res = await _webRequestService.PostAsync(GetUrl(endPoint), json);

                if (string.IsNullOrWhiteSpace(res.ResponseData))
                    return default;

                KcpRemitResponse? tempResult = JsonSerializer.Deserialize<KcpRemitResponse>(res.ResponseData);

                RemitResult? result = null;

                if (tempResult != null)
                {
                    // 파싱 실패 시 0으로 처리
                    long.TryParse(tempResult.amount, out var parsedAmount);
                    long.TryParse(tempResult.bal_amount, out var parsedBalAmount);

                    result = new RemitResult
                    {
                        ResCd = tempResult.res_cd,
                        ResMsg = tempResult.res_msg,
                        ResEnMsg = tempResult.res_en_msg,
                        TradeSeq = tempResult.trade_seq,
                        TradeDate = tempResult.trade_date,
                        Amount = parsedAmount,
                        BalAmount = parsedBalAmount,
                        BankCode = tempResult.bankcode,
                        BankName = tempResult.bankname,
                        Account = tempResult.account,
                        Depositor = tempResult.depositor,
                        AppTime = tempResult.app_time,
                        VanApptime = tempResult.van_apptime
                    };
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "KCP 송금 요청 중 오류 발생");
                return null;
            }
        }

        // 개발 : https://stg-spl.kcp.co.kr/gw/hub/v1/payment
        public async Task<AccountBalanceResult?> GetAccountBalanceAsync(AccountBalanceRequest request)
        {
            var endPoint = "/gw/hub/v1/payment";

            KcpAccountBalanceRequest parsedRequest = new KcpAccountBalanceRequest
            {
                site_cd = request.SiteCd,
                kcp_cert_info = request.KcpCertInfo,
                pay_method = request.PayMethod,
                cust_ip = request.CustIp,
                currency = request.Currency,
                amount = request.Amount,
                va_txtype = request.VaTxtype,
            };

            try
            {
                var built = Build(parsedRequest);
                var json = built.ToJson();

                var res = await _webRequestService.PostAsync(GetUrl(endPoint), json);

                if (string.IsNullOrWhiteSpace(res.ResponseData))
                    return default;

                KcpAccountBalanceResponse? tempResult = JsonSerializer.Deserialize<KcpAccountBalanceResponse>(res.ResponseData);

                AccountBalanceResult? result = null;

                if (tempResult != null)
                {
                    result = new AccountBalanceResult
                    {
                        ResCd = tempResult.res_cd,
                        ResMsg = tempResult.res_msg,
                        ResEnMsg = tempResult.res_en_msg,
                        BankCode = tempResult.bankcode,
                        Account = tempResult.account,
                        Depositor = tempResult.depositor,
                        AppTime = tempResult.app_time,
                        CanAmount = tempResult.can_amount
                    };
                }

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "KCP 잔액조회 요청 중 오류 발생");
                return null;
            }
        }
        #endregion

        #region INTERNAL METHOD AREA **********************************************
        private string GetUrl(string path) => $"{_kcpOptions.BaseUrl.TrimEnd('/')}/{path.TrimStart('/')}";

        #region BUILD KCP REMIT
        private T Build<T>(T request) where T : class
        {
            SetProp(request, "site_cd", _kcpOptions.SiteCd);
            SetProp(request, "kcp_cert_info", GetCertRaw()); // ✅ 원문 삽입

            SetPropIfExists(request, "tr_dt", GetNowTime());
            SetPropIfExists(request, "tr_no", GenerateTrNo());

            return request;
        }

        private void SetProp<T>(T obj, string propName, object value)
        {
            var prop = obj?.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
        }

        private void SetPropIfExists<T>(T obj, string propName, object value)
        {
            var prop = obj?.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
        }

        private string GetNowTime()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        private string GenerateTrNo()
        {
            return $"{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}";
        }
        #endregion

        #region KCP CERT
        /// <summary>
        /// 인증서 원문을 환경에 따라 반환 (운영은 줄바꿈만 제거)
        /// </summary>
        private string GetCertRaw()
        {
            var certPath = Path.Combine(AppContext.BaseDirectory, _kcpOptions.CertPath);

            if (!File.Exists(certPath))
                throw new FileNotFoundException("KCP 인증서 파일이 존재하지 않습니다.", certPath);

            var raw = File.ReadAllText(certPath, Encoding.UTF8);

            // 개발 인증서면 그대로 반환
            if (_kcpOptions.CertPath.Contains("DEV", StringComparison.OrdinalIgnoreCase))
            {
                return raw.Trim();
            }

            // 운영 인증서는 줄바꿈만 제거
            return raw
                .Replace("\r", "")
                .Replace("\n", "");
        }
        #endregion
        #endregion
    }
}
