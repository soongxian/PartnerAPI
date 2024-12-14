using System.ComponentModel.DataAnnotations;

namespace PartnerAPI.Model.ApiResponse
{
    public class ApiResponseSuccess : ApiResponseBase
    {
        public long TotalAmount { get; set; }
        public long TotalDiscount { get; set; }
        public long FinalAmount { get; set; }
    }
}
