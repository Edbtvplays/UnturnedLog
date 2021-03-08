using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edbtvplays.UnturnedLog.Unturned.API.Classes
{
    public class PlayerEvents // Defines the classes for data to be accessed.
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] public ulong PlayerId { get; set; }
        [Required] public string EventType { get; set; }
        [Required] public string EventData { get; set; }
        [Required] public int ServerId { get; set; }

        [Timestamp]
        public byte[] EventTime { get; set; }

        public virtual PlayerData Player { get; set; }
        public virtual Server Server { get; set; }
    }
}