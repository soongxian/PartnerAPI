using System.ComponentModel.DataAnnotations;

namespace PartnerAPI.Model
{
    public class Partner
    {
        [Required(ErrorMessage = "PartnerKey is required.")]
        public string PartnerKey { get; set; }
        [Required(ErrorMessage = "PartnerRefNo is required.")]
        public string PartnerRefNo { get; set; }
        [Required(ErrorMessage = "PartnerPassword is required.")]
        public string PartnerPassword { get; set; }
        [Required(ErrorMessage = "TotalAmount is required.")]
        [Range(0, long.MaxValue, ErrorMessage = "TotalAmount must be a positive value.")]
        public long TotalAmount { get; set; }
        public ItemDetail[]? Items { get; set; }
        [Required(ErrorMessage = "Timestamp is required.")]
        public string Timestamp { get; set; }
        public string Sig { get; set; }
    }
}