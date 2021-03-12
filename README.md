# UnturnedLog
UnturnedLog is a Unturned Player statistics plugin for the Openmod Unturned Plugin Framework that works over multiple servers symaltaneously recording data inside a MYSQL Database. It caputres basic user information and Unturned Player events with the time they executed them for data harvesting and statistics.

## Database Stored Data 

### Players Table
- Steam 64 ID (Id)
- Steam Name (SteamName)
- In-Game Display Name (CharacterName)
- Steam Profile Picture Hash (ProfilePictureHash) [You can grab a hash and get the profile picture by putting it in the following URL: https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/00/{HASH}.jpg]
- Last In-Game Group ID (LastQuestGroupId)
- Last Selected Steam Group (SteamGroup)
- Last Selected Steam Group Name (SteamGroupName)
- Hardware ID (Hwid)
- IP Address (Ip)
- Total playtime (TotalPlaytime)
- Last login to any network servers (LastLoginGlobal)
- Last joined server ID (ServerId)

Note: You need a Steam Web API Key in order to get any data within the ProfilePictureHash column. See configuration for how to get it.

### Player Event 
- Event ID (Id)
- Player Steam 64 ID (PlayerId)
- Event type (EventType) [The plugin captures: Kills, Deaths, Suicides, Connections, Dissconnections, Bans, Zombie Kills, Mega Zombie Kills, Chat Messages, Resource Harvested, Fish Caught, Plant Picked, Buildables Placed and Player Teleports]
- Event Data (EventData) [Any Information that is associated with the above event is stored here]
- Server Executred on (ServerId) 
- Time Executed (EventTime)

### Servers Table 
- Server entry ID (Id)
- Local Instance Name (Instance)
- Server Public Name (Name)

## Install 

To Install the Latest Version of this plugin copy the below command into your Unturned Openmod Server Console.

`openmod install Edbtvplays.UnturnedLog.Unturned`

## Links
Nuget: https://www.nuget.org/packages/Edbtvplays.UnturnedLog.Unturned/
