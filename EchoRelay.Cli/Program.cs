using CommandLine;
using EchoRelay.Core.Server;
using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Services;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Filesystem;
using EchoRelay.Core.Utils;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using System.Reflection;

namespace EchoRelay.Cli
{
    class Program
    {
        /// <summary>
        /// The parsed CLI argument options for the application.
        /// </summary>
        private static CliOptions? Options;
        /// <summary>
        /// The instance of the server hosting central services.
        /// </summary>
        private static Server Server;
        /// <summary>
        /// The update timer used to trigger a peer stats update on a given interval.
        /// </summary>
        private static System.Timers.Timer? peerStatsUpdateTimer;
        /// <summary>
        /// The time that the server was started.
        /// </summary>
        private static DateTime startedTime;
        /// <summary>
        /// A mutex lock object to be used when printing to console, to avoid color collisions.
        /// </summary>
        private static object _printLock = new object();

        /// <summary>
        /// The CLI argument options for the application.
        /// </summary>
        public class CliOptions
        {
            [Option('d', "database", Required = true, HelpText = "The database folder to use for server resources. If running for the first time, should be an existing, but empty folder.")]
            public string DatabaseFolder { get; set; } = "";

            [Option('g', "game", Required = false, HelpText = "The optional path to the game (echovr.exe). Extracts symbols to the server's symbol cache during initial deployment.")]
            public string? GameExecutablePath { get; set; }

            [Option('p', "port", Required = false, Default = 777, HelpText = "The TCP port to broadcast central services over.")]
            public int Port { get; set; }

            [Option("apikey", Required = false, Default = null, HelpText = "Requires a specific API key as part of the ServerDB connection URI query parameters.")]
            public string? ServerDBApiKey { get; set; }

            [Option("forcematching", Required = false, Default = true, HelpText = "Forces users to match to any available game, in the event of their requested game servers being unavailable.")]
            public bool ForceMatching { get; set; }

            [Option("lowpingmatching", Required = false, Default = false, HelpText = "Sets a preference for matching to game servers with low ping instead of high population.")]
            public bool LowPingMatching { get; set; }

            [Option("outputconfig", Required = false, HelpText = "Outputs the generated service config file to a given file path on disk.")]
            public string? OutputConfigPath { get; set; } = null;

            [Option("statsinterval", Required = false, Default = 3000, HelpText = "Sets the interval at which the CLI will output its peer stats (in milliseconds).")]
            public double StatsUpdateInterval { get; set; }

            [Option("noservervalidation", Required = false, Default = false, HelpText = "Disables validation of game servers using raw ping requests, ensuring their ports are exposed.")]
            public bool ServerDBValidateGameServers { get; set; }

            [Option("servervalidationtimeout", Required = false, Default = 3000, HelpText = "Sets the timeout for game server validation using raw ping requests. In milliseconds.")]
            public int ServerDBValidateGameServersTimeout { get; set; }

            [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output for every message sent between clients and servers.")]
            public bool Verbose { get; set; } = true;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command-line arguments the application was invoked with.</param>
        static void Main(string[] args)
        {
            // Parse our command line arguments.
            Parser.Default.ParseArguments<CliOptions>(args).WithParsed(options =>
            {
                // Set our options globally
                Options = options;

                // Verify the database folder exists.
                if(!Directory.Exists(options.DatabaseFolder))
                {
                    Error("Provided database folder does not exist. You must specify a valid directory.");
                    return;
                }

                // Verify other arguments
                if (options.Port < 0 || options.Port > ushort.MaxValue)
                {
                    Error($"Provided port is invalid. You must a value between 1 and {ushort.MaxValue}.");
                    return;
                }

                // Create our file system storage and open it.
                ServerStorage serverStorage = new FilesystemServerStorage(options.DatabaseFolder);
                serverStorage.Open();

                // Check if initial deployment needs to be performed.
                // If the database folder is empty, we deploy all resources.
                // If it is non-empty, but missing critical resources, we ask whether to clear all resources but accounts.
                bool allCriticalResourcesExist = serverStorage.AccessControlList.Exists() && serverStorage.ChannelInfo.Exists() && serverStorage.LoginSettings.Exists() && serverStorage.SymbolCache.Exists();
                bool anyCriticalResourcesExist = serverStorage.AccessControlList.Exists() || serverStorage.ChannelInfo.Exists() || serverStorage.LoginSettings.Exists() || serverStorage.SymbolCache.Exists();
                bool performInitialSetup = !allCriticalResourcesExist;
                if (performInitialSetup && anyCriticalResourcesExist)
                {
                    Warning("Critical resources are missing from storage, but storage is non-empty.\n" +
                        "Would you like to re-deploy initial setup resources? Warning: this will clear all storage except accounts! [y/N]");
                    performInitialSetup = Console.ReadKey(true).Key == ConsoleKey.Y;
                }

                // Perform initial setup of server resources if needed.
                if (performInitialSetup)
                {
                    Info("[SERVER] Performing initial setup: server resources to database folder..");
                    InitialDeployment.PerformInitialDeployment(serverStorage, options.GameExecutablePath, false);
                }

                // Create a server instance
                Server = new Server(serverStorage,
                    new ServerSettings(
                        port: (ushort)options.Port,
                        serverDbApiKey: options.ServerDBApiKey,
                        serverDBValidateServerEndpoint: options.ServerDBValidateGameServers,
                        serverDBValidateServerEndpointTimeout: options.ServerDBValidateGameServersTimeout,
                        favorPopulationOverPing: !options.LowPingMatching,
                        forceIntoAnySessionIfCreationFails: options.ForceMatching
                        )
                    );

                // Set up all event handlers.
                Server.OnServerStarted += Server_OnServerStarted;
                Server.OnServerStopped += Server_OnServerStopped;
                Server.OnAuthorizationResult += Server_OnAuthorizationResult;
                Server.OnServicePeerConnected += Server_OnServicePeerConnected;
                Server.OnServicePeerDisconnected += Server_OnServicePeerDisconnected;
                Server.OnServicePeerAuthenticated += Server_OnServicePeerAuthenticated;
                Server.ServerDBService.Registry.OnGameServerRegistered += Registry_OnGameServerRegistered;
                Server.ServerDBService.Registry.OnGameServerUnregistered += Registry_OnGameServerUnregistered;
                Server.ServerDBService.OnGameServerRegistrationFailure += ServerDBService_OnGameServerRegistrationFailure;

                // Set up all verbose event handlers.
                if (options.Verbose)
                {
                    Server.OnServicePacketSent += Server_OnServicePacketSent;
                    Server.OnServicePacketReceived += Server_OnServicePacketReceived;
                }

                // Start the server.
                Server.Start().Wait();
            });
        }

        private static void Server_OnServerStarted(Server server)
        {
            // Print our server started message
            Info("[SERVER] Server started");

            // Print our service config.
            Core.Game.ServiceConfig serviceConfig = server.Settings.GenerateServiceConfig(server.PublicIPAddress?.ToString() ?? "localhost", serverConfig: true);
            string serviceConfigSerialized = JsonConvert.SerializeObject(serviceConfig, Formatting.Indented, StreamIO.JsonSerializerSettings);
            Info($"[SERVER] Generated service config:\n{serviceConfigSerialized}");

            // Copy the service config to the clipboard if required.
            if (Options?.OutputConfigPath != null)
            {
                // Save the service config to the provided file path.
                try
                {
                    File.WriteAllText(Options!.OutputConfigPath, serviceConfigSerialized);
                    Info($"[SERVER] Output generated service config to path \"{Options!.OutputConfigPath}\"");
                }
                catch(Exception ex)
                {
                    Error($"[SERVER] Failed to output generated service config to path \"{Options!.OutputConfigPath}\":\n{ex}");
                }
            }

            // Start the peer stats update timer
            startedTime = DateTime.UtcNow;
            peerStatsUpdateTimer = new System.Timers.Timer(Options!.StatsUpdateInterval);
            peerStatsUpdateTimer.Start();
            peerStatsUpdateTimer.Elapsed += PeerStatsUpdateTimer_Elapsed;
        }

        private static void PeerStatsUpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Info($"[PEERSTATS] " +
                $"elapsed: {(DateTime.UtcNow - startedTime)}, " +
                $"gameservers: {Server.ServerDBService.Registry.RegisteredGameServers.Count}, " +
                $"login: {Server.LoginService.Peers.Count}, " +
                $"config: {Server.ConfigService.Peers.Count}, " +
                $"matching: {Server.MatchingService.Peers.Count}, " +
                $"serverdb: {Server.ServerDBService.Peers.Count}, " +
                $"transaction: {Server.TransactionService.Peers.Count}"
                );
        }

        private static void Server_OnServerStopped(Server server)
        {
            // Stop the update timer.
            peerStatsUpdateTimer?.Stop();

            // Print our server stopped message
            Info("[SERVER] Server stopped");
        }

        private static void Server_OnAuthorizationResult(Server server, System.Net.IPEndPoint client, bool authorized)
        {
            if(!authorized)
                Error($"[SERVER] client({client.Address}:{client.Port}) failed authorization");
        }
        private static void Server_OnServicePeerConnected(Core.Server.Services.Service service, Core.Server.Services.Peer peer)
        {
            Info($"[{service.Name}] client({peer.Address}:{peer.Port}) connected");
        }
        private static void Server_OnServicePeerDisconnected(Core.Server.Services.Service service, Core.Server.Services.Peer peer)
        {
            Info($"[{service.Name}] client({peer.Address}:{peer.Port}) disconnected");
        }
        private static void Server_OnServicePeerAuthenticated(Core.Server.Services.Service service, Core.Server.Services.Peer peer, Core.Game.XPlatformId userId)
        {
        
        }

        private static void Registry_OnGameServerRegistered(Core.Server.Services.ServerDB.RegisteredGameServer gameServer)
        {
            Info($"[{gameServer.Peer.Service.Name}] client({gameServer.Peer.Address}:{gameServer.Peer.Port}) registered game server (server_id={gameServer.ServerId}, region_symbol={gameServer.RegionSymbol}, version_lock={gameServer.VersionLock}, endpoint=<{gameServer.ExternalAddress}:{gameServer.Port}>)");
        }

        private static void Registry_OnGameServerUnregistered(Core.Server.Services.ServerDB.RegisteredGameServer gameServer)
        {
            Info($"[{gameServer.Peer.Service.Name}] client({gameServer.Peer.Address}:{gameServer.Peer.Port}) unregistered game server (server_id={gameServer.ServerId}, region_symbol={gameServer.RegionSymbol}, version_lock={gameServer.VersionLock}, endpoint=<{gameServer.ExternalAddress}:{gameServer.Port}>)");
        }
        private static void ServerDBService_OnGameServerRegistrationFailure(Peer peer, Core.Server.Messages.ServerDB.ERGameServerRegistrationRequest registrationRequest, string failureMessage)
        {
            Error($"[{peer.Service.Name}] client({peer.Address}:{peer.Port}) failed to register game server: \"{failureMessage}\"");
        }

        private static string getPacketDisplayString(Packet packet)
        {
            // Add every message to our result.
            string result = "";
            for(int i = 0; i < packet.Count; i++)
            { 
                if (i == packet.Count - 1)
                    result += $"\t{packet[i]}";
                else
                    result += $"\t{packet[i]}\n";
            }
            return result;
        }

        private static void Server_OnServicePacketSent(Core.Server.Services.Service service, Core.Server.Services.Peer sender, Core.Server.Messages.Packet packet)
        {
            Debug($"[{service.Name}] server->client({sender.Address}:{sender.Port}\n" + getPacketDisplayString(packet));
        }

        private static void Server_OnServicePacketReceived(Core.Server.Services.Service service, Core.Server.Services.Peer sender, Core.Server.Messages.Packet packet)
        {
            Debug($"[{service.Name}] client({sender.Address}:{sender.Port})->server:\n" + getPacketDisplayString(packet));
        }


        private static void Error(string msg)
        {
            lock (_printLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }

        private static void Warning(string msg)
        {
            lock (_printLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
        private static void Info(string msg)
        {
            lock (_printLock)
            {
                Console.ResetColor();
                Console.WriteLine(msg);
            }
        }

        private static void Debug(string msg)
        {
            lock (_printLock)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}