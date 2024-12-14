using System.ComponentModel.DataAnnotations;

namespace PartnerAPI.Model.ApiResponse
{
    public class ApiResponseSuccess : ApiResponseBase
    {
        [Required(ErrorMessage = "TotalAmount is required.")]
        [Range(0, long.MaxValue, ErrorMessage = "TotalAmount must be a positive value.")]
        public long TotalAmount { get; set; }
        [Required(ErrorMessage = "TotalDiscount is required.")]
        [Range(0, long.MaxValue, ErrorMessage = "TotalDiscount must be a positive value.")]
        public long TotalDiscount { get; set; }
        public long FinalAmount { get; set; }
    }
}
