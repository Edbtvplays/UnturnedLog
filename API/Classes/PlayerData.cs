using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edbtvplays.UnturnedLog.Unturned.API.Classes
{
    public class PlayerData // Defines the classes for data to be accessed.
    {

        // Player Information for Events disconnected amd connected
        [Column("Id", TypeName = "BIGINT UNSIGNED")] [Key] [Required]
        public ulong Id { get; set; }

        [Required] [StringLength(64)] public string SteamName { get; set; }

        [Required] [StringLength(64)] public string CharacterName { get; set; }

        [Required] [StringLength(64)] public string ProfilePictureHash { get; set; }

        [Column("LastQuestGroupId", TypeName = "BIGINT UNSIGNED")] [Required] [DefaultValue(0)]
        public ulong LastQuestGroupId { get; set; }

        [Column("SteamGroup", TypeName = "BIGINT UNSIGNED")] [Required] [DefaultValue(0)]
        public ulong SteamGroup { get; set; }

        [Required] [StringLength(64)] [DefaultValue("N/A")]
        public string SteamGroupName { get; set; }

        [Required] public string Hwid { get; set; }

        [Required] public long Ip { get; set; }

        [Required] [DefaultValue(0)] public double TotalPlaytime { get; set; }

        [Required] public DateTime LastLoginGlobal { get; set; }

        // Server ID Foreign Key
        [Required] public int ServerId { get; set; }

        // Player Statistics for Events
        [Required] [DefaultValue(0)] public int Punishments { get; set; }

        [Required] [DefaultValue(0)] public int PlayerKills { get; set; }

        [Required] [DefaultValue(0)] public int ZombieKills { get; set; }

        [Required] [DefaultValue(0)] public int Deaths { get; set; }

        [Required] [DefaultValue(0)] public int Headshots { get; set; }

        [Required] [DefaultValue(0)] public int NodesMined { get; set; }

        [Required] [DefaultValue(0)] public int TreesCutdown { get; set; }

        [Required] [DefaultValue(0)] public int TotalChatMessages { get; set; }


        public virtual Server Server { get; set; }
    }
}