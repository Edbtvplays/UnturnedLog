using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edbtvplays.UnturnedLog.Unturned.API.Classes
{
    public class TPS
    {
        [Key]   
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long Timestamp { get; set; }
        public int Value { get; set; }

        [Required] public int ServerId { get; set; }

    }
}
