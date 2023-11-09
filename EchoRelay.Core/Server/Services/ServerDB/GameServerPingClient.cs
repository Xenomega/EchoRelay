using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EchoRelay.Core.Utils;
using System.Security.Cryptography;
using System.Threading;

namespace EchoRelay.Core.Server.Services.ServerDB
{
    /// <summary>
    /// A client provider which validates game server availability by sending raw ping requests and expecting 
    /// valid raw ping acknowledgement responses.
    /// </summary>
    public abstract class GameServerPingClient
    {
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message for a raw ping request.
        /// </summary>
        public const ulong RawPingRequestMessageSymbol = 0x997279DE065A03B0;
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message for a raw ping acknowledgement.
        /// </summary>
        public const ulong RawPingAcknowledgeMessageSymbol = 0x4F7AE556E0B77891;

        /// <summary>
        /// Checks if a given game server is available by sending a raw ping request and verifying its acknowledgement.
        /// </summary>
        /// <param name="registeredGameServer">The game server to validate.</param>
        /// <returns>True if the game server is available, false otherwise.</returns>
        public static async Task<bool> CheckAvailable(RegisteredGameServer registeredGameServer, int timeoutMilliseconds)
        {
            return await CheckAvailable(new IPEndPoint(registeredGameServer.ExternalAddress, registeredGameServer.Port), timeoutMilliseconds);
        }

        /// <summary>
        /// Checks if a given game server is available by sending a raw ping request and verifying its acknowledgement.
        /// </summary>
        /// <param name="endpoint">The endpoint the game server exposed.</param>
        /// <returns>True if the game server is available, false otherwise.</returns>
        public static async Task<bool> CheckAvailable(IPEndPoint endpoint, int timeoutMilliseconds)
        {
            // Create a cancellation token source
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Create a UDP client targeting the provided endpoint.
                using (UdpClient client = new UdpClient())
                {
                    client.Client.SendTimeout = timeoutMilliseconds;
                    client.Client.ReceiveTimeout = timeoutMilliseconds;

                    // Select a random 64-bit integer to ping with.
                    ulong pingNum = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8));

                    // Create the raw ping request.
                    StreamIO io = new StreamIO();
                    io.Write(RawPingRequestMessageSymbol);
                    io.Write(pingNum);
                    byte[] rawPingRequest = io.ToArray();
                    io.Close();

                    // TODO: This code is spaghetti past this point, sorry, I stopped caring half way through after seeing C#'s ReceiveAsync doesn't allow you to filter by an endpoint and timeout values don't work on async methods.
                    // It works, so I'm kind of over it :). A better way to do it would be to use cancellation token timeouts and reset the token.

                    // Send the ping request to the client
                    Task<int> sendTask = client.SendAsync(new ReadOnlyMemory<byte>(rawPingRequest), endpoint.Address.ToString(), endpoint.Port, cancellationTokenSource.Token).AsTask();
                    Task finishedTask = await Task.WhenAny(sendTask, Task.Delay(timeoutMilliseconds, cancellationTokenSource.Token));
                    if (finishedTask != sendTask)
                    {
                        // We timed out, cancel the underlying operation and return a failure.
                        cancellationTokenSource.Cancel();
                        return false;
                    }

                    // Verify the size of the data sent
                    if (!sendTask.IsCompletedSuccessfully || sendTask.Result != rawPingRequest.Length)
                        return false;

                    // Receive data from the endpoint.
                    Task<UdpReceiveResult?> receiveTask = Task.Run<UdpReceiveResult?>(async() => 
                    {
                        // Only read received data from our expected endpoint.
                        while (!cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            UdpReceiveResult result = await client.ReceiveAsync(cancellationTokenSource.Token).AsTask();
                            if (!cancellationTokenSource.IsCancellationRequested && result.RemoteEndPoint.Address.ToUInt32() == endpoint.Address.ToUInt32() && result.RemoteEndPoint.Port == endpoint.Port)
                                return result;
                        }
                        return null;
                    });
                    finishedTask = await Task.WhenAny(receiveTask, Task.Delay(timeoutMilliseconds, cancellationTokenSource.Token));
                    if (finishedTask != receiveTask)
                    {
                        // We timed out, cancel the underlying operation and return a failure.
                        cancellationTokenSource.Cancel();
                        return false;
                    }

                    // Verify the size of the data received
                    if (!receiveTask.IsCompletedSuccessfully || receiveTask.Result?.Buffer.Length != rawPingRequest.Length)
                        return false;

                    // Read the packet, verifying its message type/symbol, and ping number response.
                    io = new StreamIO(receiveTask.Result.Value.Buffer);
                    if (io.ReadUInt64() != RawPingAcknowledgeMessageSymbol)
                        return false;
                    if (io.ReadUInt64() != pingNum)
                        return false;

                    // The ping acknowledgement was received and validated successfully.
                    return true;
                }
            }
            catch
            {
                // Some kind of exception occurred, indicating a failure.
                return false;
            }
            finally
            {
                // Always cancel the token source on exit.
                cancellationTokenSource.Cancel();
            }
        }
    }
}
