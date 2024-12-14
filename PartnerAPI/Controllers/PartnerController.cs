using log4net;
using Microsoft.AspNetCore.Mvc;
using PartnerAPI.Model;
using PartnerAPI.Model.ApiResponse;
using System.Security.Cryptography;
using System.Text;

namespace PartnerAPI.Controllers
{
    [Route("api/submittrxmessage")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PartnerController));
        private static readonly Partner[] AllowedPartners = new Partner[]
        {
            new Partner { PartnerKey = "FAKEGOOGLE", PartnerRefNo = "FG-00001", PartnerPassword = "FAKEPASSWORD1234" },
            new Partner { PartnerKey = "FAKEPEOPLE", PartnerRefNo = "FG-00002", PartnerPassword = "FAKEPASSWORD4578" }
        };

        [HttpPost]
        public IActionResult ProcessPartner(Partner partner)
        {
            if (partner == null)
            {
                Log.Error("There is no data found.");
                return Unauthorized(new ApiResponseFail
                {
                    Result = 0,
                    ResultMessage = "Invalid Data!"
                });
            }
            Log.Info("Partner is found.");

            if (!VerifyTimestamp(partner.Timestamp))
            {
                Log.Error($"Timestamp is showing as {partner.Timestamp}");
                return Unauthorized(new ApiResponseFail
                {
                    Result = 0,
                    ResultMessage = "Invalid Timestamp!"
                });
            }
            Log.Info("Timestamp processed successfully.");

            string calculatedSignature = GenerateSHA254Signature(partner.Timestamp, partner.PartnerKey, partner.PartnerRefNo, partner.TotalAmount, partner.PartnerPassword);

            if (calculatedSignature != partner.Sig)
            {
                Log.Error($"Signature generated is {calculatedSignature}, expected signature is {partner.Sig}");
                return Unauthorized(new ApiResponseFail
                {
                    Result = 0,
                    ResultMessage = "Invalid Signature!"
                });
            }
            Log.Info("Signature processed successfully.");

            string decodedPassword = DecodeBase64(partner.PartnerPassword);

            var validPartner = AllowedPartners.FirstOrDefault(p =>
                p.PartnerKey == partner.PartnerKey &&
                p.PartnerRefNo == partner.PartnerRefNo &&
                p.PartnerPassword == decodedPassword);

            if (validPartner == null)
            {
                Log.Error($"Check PartnerKey: {partner.PartnerKey}, PartnerRefNo: {partner.PartnerRefNo}, DecodedPartnerPassword: {partner.PartnerPassword} as there is mismatched data!");
                return Unauthorized(new ApiResponseFail
                {
                    Result = 0,
                    ResultMessage = "Access Denied!"
                });
            }
            Log.Info("Access granted successfully.");

            long CalculatedTotalAmount = 0;
            bool IsProcessed = false;

            if (partner.Items == null)
            {
                IsProcessed = true;
                Log.Info("No item is processed.");
            }
            else
            {
                IsProcessed = ProcessPartnerTransaction(partner, out CalculatedTotalAmount);
                Log.Info("Items processed successfully.");
            }
            long TotalDiscount = CalculateTotalDiscount(CalculatedTotalAmount);
            Log.Info("TotalDiscount processed successfully.");

            if (partner.TotalAmount != CalculatedTotalAmount)
            {
                Log.Error($"ExpectedTotalAmount: {partner.TotalAmount}; ActualTotalAmount: {CalculatedTotalAmount}");
                return BadRequest(new ApiResponseFail
                {
                    Result = 0,
                    ResultMessage = "Wrong Total Amount!"
                });
            }
            Log.Info("TotalAmount processed successfully.");

            long finalAmount = CalculatedTotalAmount - TotalDiscount;
            Log.Info("FinalAmount processed successfully.");

            if (IsProcessed)
            {
                var response = new ApiResponseSuccess
                {
                    Result = 1,
                    TotalAmount = CalculatedTotalAmount,
                    TotalDiscount = TotalDiscount,
                    FinalAmount = finalAmount
                };

                Log.Info("Transaction processed successfully.");
                return Ok(response);
            }
            else
            {
                Log.Error("Need further check for failure!");
                return BadRequest(new ApiResponseFail
                {
                    Result = 0,
                    ResultMessage = "Process is unsuccessful!"
                });
            }
        }

        private static bool ProcessPartnerTransaction(Partner partner, out long TotalAmount)
        {
            TotalAmount = 0;

            if (partner.Items == null || !partner.Items.Any())
            {
                return false;
            }

            for (int i = 0; i < partner.Items.Length; i++)
            {
                var item = partner.Items[i];

                if (item.Qty <= 0 || item.UnitPrice <= 0)
                {
                    return false;
                }
            }

            for (int i = 0; i < partner.Items.Length; i++)
            {
                var item = partner.Items[i];
                TotalAmount += (item.Qty * item.UnitPrice);
            }

            return true;
        }

        private static string DecodeBase64(string base64EncodedString)
        {
            try
            {
                byte[] decodedBytes = Convert.FromBase64String(base64EncodedString);

                return Encoding.UTF8.GetString(decodedBytes);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        private static bool VerifyTimestamp(string Timestamp)
        {
            if (DateTime.TryParseExact(Timestamp, "yyyy-MM-ddTHH:mm:ss.fffffffZ", null, System.Globalization.DateTimeStyles.AssumeUniversal, out DateTime ParsedTimestamp))
            {
                DateTime CurrentServerTime = DateTime.UtcNow;

                TimeSpan TimeDifference = CurrentServerTime - ParsedTimestamp;

                if (Math.Abs(TimeDifference.TotalSeconds) <= 300)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static string FormatTimestampForSignature(string Timestamp)
        {
            if (DateTime.TryParse(Timestamp, out DateTime ParsedTimestamp))
            {
                return ParsedTimestamp.ToString("yyyyMMddHHmmss");
            }
            else
            {
                Log.Error("Timestamp conversion error for signature.");
                throw new FormatException("Invalid Timestamp format.");
            }
        }

        public static string GenerateSHA254Signature(string Timestamp, string PartnerKey, string PartnerRefNo, long TotalAmount, string EncodedPassword)
        {
            string formattedTimestring = FormatTimestampForSignature(Timestamp);
            string SignatureString = formattedTimestring + PartnerKey + PartnerRefNo + TotalAmount + EncodedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] HashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(SignatureString));
                return Convert.ToBase64String(HashBytes);
            }
        }

        private static long CalculateTotalDiscount(long TotalAmount)
        {
            long TotalDiscount = 0;

            if (TotalAmount < 200)
            {
                TotalDiscount = 0;
            }
            else if (TotalAmount >= 200 && TotalAmount <= 500)
            {
                TotalDiscount = (long)(TotalAmount * 0.05);
            }
            else if (TotalAmount >= 501 && TotalAmount <= 800)
            {
                TotalDiscount = (long)(TotalAmount * 0.07);
            }
            else if (TotalAmount >= 801 && TotalAmount <= 1200)
            {
                TotalDiscount = (long)(TotalAmount * 0.10);
            }
            else if (TotalAmount > 1200)
            {
                TotalDiscount = (long)(TotalAmount * 0.15);
            }

            if (TotalAmount > 500)
            {
                bool isPrime = true;
                for (int i = 2; i <= Math.Sqrt(TotalAmount); i++)
                {
                    if (TotalAmount % i == 0)
                    {
                        isPrime = false;
                        break;
                    }

                    if (isPrime && TotalAmount > 500)
                    {
                        TotalDiscount += (long)(TotalAmount * 0.08);
                    }
                }
            }

            if (TotalAmount > 900 && TotalAmount % 10 == 5)
            {
                TotalDiscount += (long)(TotalAmount * 0.10);
            }

            long MaxDiscount = (long)(TotalAmount * 0.20);
            if (TotalDiscount > MaxDiscount)
            {
                TotalDiscount = MaxDiscount;
            }
            return TotalDiscount;
        }
    }
}
