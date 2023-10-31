using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Messages.Common;
using EchoRelay.Core.Server.Messages.Config;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.Core.Server.Services.Config
{
    /// <summary>
    /// The config service is used to obtain game configurations. It does not maintain sessions or state per user.
    /// </summary>
    public class ConfigService : Service
    {
        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ConfigService"/> with the provided arguments.
        /// </summary>
        /// <param name="server">The server which this service is bound to.</param>
        public ConfigService(Server server) : base(server, "CONFIG")
        {
     
        }
        #endregion

        #region Functions
        /// <summary>
        /// Handles a packet being received by a peer.
        /// This is called after all events have been fired for <see cref="OnPacketReceived"/>.
        /// </summary>
        /// <param name="sender">The peer which sent the packet.</param>
        /// <param name="packet">The packet sent by the peer.</param>
        protected override async Task HandlePacket(Peer sender, Packet packet)
        {
            // Loop for each message received in the packet
            foreach (Message message in packet)
            {
                switch (message)
                {
                    case ConfigRequestv2 configRequestv2:
                        await ProcessConfigRequestv2(sender, configRequestv2);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a <see cref="ConfigRequestv2"/> by attempting to obtain the requested config resource and returning it to the sender.
        /// If it is obtained, a <see cref="ConfigSuccessv2"/> response is sent, otherwise a <see cref="ConfigFailurev2"/> is sent.
        /// </summary>
        /// <param name="sender">The peer which sent the request.</param>
        /// <param name="request">The request made by the peer.</param>
        /// <returns></returns>
        private async Task ProcessConfigRequestv2(Peer sender, ConfigRequestv2 request)
        {
            // Obtain the symbols for this config resource type/identifier.
            long? typeSymbol = SymbolCache.GetSymbol(request.Info.Type);
            long? identifierSymbol = SymbolCache.GetSymbol(request.Info.Identifier);

            // If either symbol could not be obtained, return an error.
            if (typeSymbol == null)
            {
                await sender.Send(new ConfigFailurev2(request.Info.Type, request.Info.Identifier, 1, $"Could not resolve symbol for type (type = {request.Info.Type}, id = {request.Info.Identifier})"));
                return;
            }
            if (identifierSymbol == null)
            {
                await sender.Send(new ConfigFailurev2(request.Info.Type, request.Info.Identifier, 1, $"Could not resolve symbol for type (type = {request.Info.Type}, id = {request.Info.Identifier})"));
                return;
            }

            // Try to obtain the requested config resource.
            ConfigResource? configData = Storage.Configs.Get((request.Info.Type, request.Info.Identifier));
            if (configData == null)
            {
                await sender.Send(new ConfigFailurev2(request.Info.Type, request.Info.Identifier, 1, $"Could not find specified config data with the provided identifier (type = {request.Info.Type}, id = {request.Info.Identifier})"));
                return;
            }

            // Send the config success message.
            await sender.Send(new ConfigSuccessv2(typeSymbol.Value, identifierSymbol.Value, configData));
            await sender.Send(new TcpConnectionUnrequireEvent());
        }
        #endregion
    }
}
