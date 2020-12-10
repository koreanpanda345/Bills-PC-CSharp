using System.ComponentModel.DataAnnotations;

namespace BillsPC.Web_Dashboard.Data
{
    public class PrefixModel
    {
        [Required]
        [StringLength(10, ErrorMessage = "Prefix is too long.")]
        public string Prefix { get; set; }
    }
}