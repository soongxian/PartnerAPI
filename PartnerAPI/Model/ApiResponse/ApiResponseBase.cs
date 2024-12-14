using System.ComponentModel.DataAnnotations;

namespace PartnerAPI.Model.ApiResponse
{
    public class ApiResponseBase
    {
        [Required(ErrorMessage = "Result is required.")]
        public int Result { get; set; }
    }
}
