using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edbtvplays.UnturnedLog.Unturned.API.Classes
{
    public class Server
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] [StringLength(128)] public string Instance { get; set; }

        [Required] [StringLength(50)] public string Name { get; set; }

        // Server IP so the websocket knows what to connect to. 
        [Required] public uint IP { get; set; }
    }
}