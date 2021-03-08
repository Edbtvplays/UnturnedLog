using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.API.Users;
using Edbtvplays.UnturnedLog.Unturned.API.Classes;

// Auto Generated file. 

namespace Edbtvplays.UnturnedLog.Unturned.API.Services
{
    [Service]
    public interface IUnturnedLogRepository
    {
        /// <summary>
        /// Gets the current server from the Servers DbSet.
        /// </summary>
        /// <returns>An instance of Server if found, null otherwise.</returns>
        Task<Server> GetCurrentServerAsync();

        /// <summary>
        /// Gets the current server from the Servers DbSet.
        /// </summary>
        /// <returns>An instance of Server if found, null otherwise.</returns>
        Server GetCurrentServer();


        /// <summary>
        /// Checks the Servers DbSet to see if the current server exists in it. Also updates the server name.
        /// </summary>
        /// <returns>True if the server exists, false otherwise.</returns>
        Task<bool> CheckCurrentServerExistsAsync();

        /// <summary>
        /// Checks the Servers DbSet to see if the current server exists in it. Also updates the server name.
        /// </summary>
        /// <returns>True if the server exists, false otherwise.</returns>
        bool CheckCurrentServerExists();

        /// <summary>
        /// Checks that the server currently exists, and registers it if it doesn't exist. If it does exist, it updates the display name.
        /// </summary>
        /// <returns>An instance of the Server class that matches the current server.</returns>
        Task<Server> CheckAndRegisterCurrentServerAsync();

        /// <summary>
        /// Checks that the server currently exists, and registers it if it doesn't exist. If it does exist, it updates the display name.
        /// </summary>
        /// <returns>An instance of the Server class that matches the current server.</returns>
        Server CheckAndRegisterCurrentServer();


        /// <summary>
        /// Retrieves a Server instance from the DbSet with the specified id.
        /// </summary>
        /// <param name="id">The auto-incrementing ID related to the server.</param>
        /// <returns>An instance of Server if found, null otherwise.</returns>
        Task<Server> GetServerAsync(int id);

        /// <summary>
        /// Retrieves a Server instance from the DbSet with the specified id.
        /// </summary>
        /// <param name="id">The auto-incrementing ID related to the server.</param>
        /// <returns>An instance of Server if found, null otherwise.</returns>
        Server GetServer(int id);


        /// <summary>
        /// Searches for a players' data within the Players DbSet
        /// </summary>
        /// <param name="searchTerm">The term (a steam64ID or a name) to search for.</param>
        /// <param name="searchMode">How the term should be interpreted when searching</param>
        /// <returns>A PlayerData instance if anything is found, null otherwise.</returns>
        Task<PlayerData> FindPlayerAsync(string searchTerm, UserSearchMode searchMode);

        /// <summary>
        /// Searches for a players' data within the Players DbSet
        /// </summary>
        /// <param name="searchTerm">The term (a steam64ID or a name) to search for.</param>
        /// <param name="searchMode">How the term should be interpreted when searching</param>
        /// <returns>A PlayerData instance if anything is found, null otherwise.</returns>
        PlayerData FindPlayer(string searchTerm, UserSearchMode searchMode);


        /// <summary>
        /// Searches for multiple players data within the Players DbSet
        /// </summary>
        /// <param name="searchTerm">The term (a steam64ID or a name) to search for.</param>
        /// <param name="searchMode">How the term should be interpreted when searching</param>
        /// <returns>A List of PlayerData instances. Never is null, only empty if nothing is found.</returns>
        Task<List<PlayerData>> FindMultiplePlayersAsync(string searchTerm, UserSearchMode searchMode);

        /// <summary>
        /// Searches for multiple players data within the Players DbSet
        /// </summary>
        /// <param name="searchTerm">The term (a steam64ID or a name) to search for.</param>
        /// <param name="searchMode">How the term should be interpreted when searching</param>
        /// <returns>A List of PlayerData instances. Never is null, only empty if nothing is found.</returns>
        List<PlayerData> FindMultiplePlayers(string searchTerm, UserSearchMode searchMode);

        /// <summary>
        /// Adds player data to the DB Asynchronously.
        /// </summary>
        /// <param name="playerData">The instance of the player's data.</param>
        Task AddPlayerDataAsync(PlayerData playerData);

        /// <summary>
        /// Adds player data to the DB Asynchronously.
        /// </summary>
        /// <param name="playerData">The instance of the player's data.</param>
        void AddPlayerData(PlayerData playerData);

        /// <summary>
        /// Adds an Event to the database asyncrionously
        /// </summary>
        /// <param name="playerEvents">The instance of the event data.</param>
        void AddPlayerEvent(PlayerEvents playerEvents);

        /// <summary>
        /// Adds player data to the DB Asynchronously.
        /// </summary>
        /// <param name="playerEvents">The instance of the player's data.</param>
        Task AddPlayerEventAsync(PlayerEvents playerEvents);

        /// <summary>
        /// Saves all changes made in the context to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int SaveChanges();
    }
}