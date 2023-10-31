using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Messages.Transaction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Services.Transaction
{
    public class TransactionService : Service
    {
        public TransactionService(Server server) : base(server, "TRANSACTION")
        {
        }

        /// <summary>
        /// Handles a packet being received by a peer.
        /// This is called after all event handlers have been fired for <see cref="OnPacketReceived"/>.
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
                    case ReconcileIAP reconcileIAP:
                        await ProcessReconcileIAPRequest(sender, reconcileIAP);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a <see cref="ReconcileIAP"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessReconcileIAPRequest(Peer sender, ReconcileIAP request)
        {
            // Respond to every request with some kind of result response.
            await sender.Send(new ReconcileIAPResult(request.UserId, JsonConvert.DeserializeObject<JObject>("{'balance': {'currency': {'echopoints': {'val': 0}}}, 'transactionid': 1}")!));
        }
    }
}
