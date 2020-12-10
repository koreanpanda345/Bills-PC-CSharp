using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BillsPC.Web_Dashboard.Data
{
    public class PokemonModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Ability { get; set; }
    }
}