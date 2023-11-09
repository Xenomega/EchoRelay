using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Messages.Common;
using EchoRelay.Core.Server.Messages.Login;
using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Jitbit.Utils;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace EchoRelay.Core.Server.Services.Login
{
    /// <summary>
    /// The login service is used to sign in, obtain a session, obtain logged in/other user profiles, update logged in profile, etc.
    /// </summary>
    public class LoginService : Service
    {
        #region Fields
        /// <summary>
        /// A cache of user sessions, with expiry upon peer disconnect.
        /// </summary>
        private FastCache<Guid, XPlatformId> _userSessions;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoginService"/> with the provided arguments.
        /// </summary>
        /// <param name="server">The server which this service is bound to.</param>
        public LoginService(Server server) : base(server, "LOGIN")
        {
            _userSessions = new FastCache<Guid, XPlatformId>();
            OnPeerDisconnected += LoginService_OnPeerDisconnected;
            Server.OnServerStopped += Server_OnServerStopped;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Checks if a provided user session token is valid.
        /// </summary>
        /// <param name="session">The user session to verify.</param>
        /// <param name="userId">The account identifier of the user.</param>
        /// <returns>Returns true if the session for this user exists, false otherwise.</returns>
        public bool CheckUserSessionValid(Guid session, XPlatformId userId)
        {
            // If the session doesn't exist in cache and we can't obtain the associated user identifier,
            // it is not a valid session.
            if (!_userSessions.TryGet(session, out XPlatformId storedUserId))
                return false;

            // If the session exists, the user identifiers must match too.
            return userId == storedUserId;
        }

        /// <summary>
        /// Invalidates a connected peer's session token.
        /// </summary>
        /// <param name="peer">The peer to invalidate the token for.</param>
        private void InvalidatePeerUserSession(Peer peer)
        {
            // If the peer had a session token, remove it.
            Guid? session = peer.GetSessionData<Guid?>();
            if (session != null)
            {
                _userSessions.Remove(session.Value);
            }
            peer.ClearSessionData();
        }

        /// <summary>
        /// An event handler triggered when a peer disconnects from the service.
        /// </summary>
        /// <param name="service">The service the peer disconnected from.</param>
        /// <param name="peer">The peer that disconnected.</param>
        private void LoginService_OnPeerDisconnected(Service service, Peer peer)
        {
            // If the peer had a session token, update its expiry time.
            Guid? session = peer.GetSessionData<Guid?>();
            if (session != null && _userSessions.TryGet(session.Value, out XPlatformId userId))
            {
                _userSessions.AddOrUpdate(session.Value, userId, Server.Settings.SessionDisconnectedTimeout);
            }
        }

        /// <summary>
        /// An event handler which fires when the server is stopped.
        /// </summary>
        /// <param name="server">The server which has stopped.</param>
        private void Server_OnServerStopped(Server server)
        {
            // Clear all sessions on server stop.
            _userSessions.Clear();
        }

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
                    case LoginRequest loginRequest:
                        await ProcessLoginRequest(sender, loginRequest);
                        break;
                    case LoggedInUserProfileRequest loggedInUserProfileRequest:
                        await ProcessLoggedInUserProfileRequest(sender, loggedInUserProfileRequest);
                        break;
                    case DocumentRequestv2 documentRequestv2:
                        await ProcessDocumentRequestv2(sender, documentRequestv2);
                        break;
                    case ChannelInfoRequest channelInfoRequest:
                        await ProcessChannelInfoRequest(sender, channelInfoRequest);
                        break;
                    case UpdateProfile updateProfileRequest:
                        await ProcessUpdateProfile(sender, updateProfileRequest);
                        break;
                    case OtherUserProfileRequest otherUserProfileRequest:
                        await ProcessOtherUserProfileRequest(sender, otherUserProfileRequest);
                        break;
                    case UserServerProfileUpdateRequest userServerProfileUpdateRequest:
                        await ProcessUserServerProfileUpdateRequest(sender, userServerProfileUpdateRequest);
                        break;
                }
            }
        }

        /// <summary>
        /// Processes a <see cref="LoginRequest"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessLoginRequest(Peer sender, LoginRequest request)
        {
            // If we have existing session data for this peer's connection, invalidate it.
            // Note: The client may have multiple connections, represented as different peers.
            // This only invalidates the current connection prior to accepting a new login.
            InvalidatePeerUserSession(sender);

            // Validate the user identifier
            if(!request.UserId.Valid())
            {
                await sender.Send(new LoginFailure(request.UserId, HttpStatusCode.BadRequest, "User identifier invalid"));
                return;
            }

            // Validate the user identifier
            // TODO: Revisit this, these are not the same values. Should AccountId be the one we actually index accounts by? Can Platform ID change with time..?
            if (false && request.AccountInfo.AccountId != request.UserId.AccountId)
            {
                await sender.Send(new LoginFailure(request.UserId, HttpStatusCode.BadRequest, "Authentication failed"));
                return;
            }

            // Get the current timestamp
            ulong currentTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Try to obtain a user from the storage layer.
            // If the user doesn't exist, we create them.
            AccountResource? account = Storage.Accounts.Get(request.UserId);
            if (account == null)
            {
                // Create a default username for this user.
                string displayName = request.UserId.PlatformCode == PlatformCode.DMO ? "Anonymous [DEMO]" : $"User [{RandomNumberGenerator.GetInt32(int.MaxValue).ToString("X")}]";

                // Create an account for this user id. We use the platform identifier string as the display name.
                account = new AccountResource(request.UserId, displayName, true, true, true);
                account.Profile.Server.CreateTime = currentTimestamp;
            } 
            else
            {
                // Real authentication can't be performed here against Oculus API. We are given an Oculus access token and nonce from client.
                // Next, our server should be reaching out to Oculus servers with the access token and nonce to perform validation, however, this
                // requires an app secret that only the real server would have, and which we wouldn't.
                // Reference: https://developer.oculus.com/documentation/unity/ps-ownership/

                // Note: It would be an anti-goal of this project to integrate with Oculus services anyways, so this is just a note for research.
            }

            // Obtain our login service query parameters, so we can check for account display name overrides, authentication info, etc.
            NameValueCollection queryStrings = HttpUtility.ParseQueryString(sender.RequestUri.Query);
            string? displayNameOverride = queryStrings.Get("displayname");
            string? authPassword = queryStrings.Get("auth") ?? queryStrings.Get("password");

            // Authenticate to the account. If this is the first time an authentication lock/password
            // was provided, it will be set for future authentication.
            if(!account.Authenticate(authPassword))
            {
                await sender.Send(new LoginFailure(request.UserId, HttpStatusCode.BadRequest, $"Invalid account password/authentication lock"));
                return;
            }

            // Check if the user is banned
            if (account.Banned)
            {
                await sender.Send(new LoginFailure(request.UserId, HttpStatusCode.BadRequest, $"Banned until: {account.BannedUntil!.Value:MM/dd/yyyy @ hh:mm:ss tt} (UTC)"));
                return;
            }
            else
            {
                account.BannedUntil = null;
            }

            // If we have a display name override, update the display name.
            if (displayNameOverride != null)
            {
                displayNameOverride = displayNameOverride.Trim();
                if (displayNameOverride.Length > 0)
                {
                    // Limit the maximum display name length.
                    if (displayNameOverride.Length > 20)
                        displayNameOverride = displayNameOverride.Substring(0, 20);

                    // If this is a demo account, wrap the name for distinction
                    if (account.AccountIdentifier.PlatformCode == PlatformCode.DMO)
                        displayNameOverride = $"{displayNameOverride} [DEMO]";

                    account.Profile.SetDisplayName(displayNameOverride);
                }
            }

            // Update the server profile's logintime and updatetime.
            account.Profile.Server.LobbyVersion = request.AccountInfo.LobbyVersion;
            account.Profile.Server.LoginTime = currentTimestamp;
            account.Profile.Server.UpdateTime = currentTimestamp;
            account.Profile.Server.ModifyTime = currentTimestamp;

            // Store the account data
            Storage.Accounts.Set(account);

            // Create a session token that will practically not expire.
            // Set it for the peer. If they disconnect, an actual timeout will be set on the session before it expires.
            Guid session = SecureGuidGenerator.Generate();
            _userSessions.AddOrUpdate(session, request.UserId, TimeSpan.FromDays(3000));
            sender.SetSessionData(session);

            // Obtain the login settings
            LoginSettingsResource? loginSettings = Storage.LoginSettings.Get();

            // Set the authenticated user identifier
            sender.UpdateUserAuthentication(request.UserId, account.Profile.Server.DisplayName);

            // Send login success response.
            await sender.Send(new LoginSuccess(request.UserId, session));
            await sender.Send(new TcpConnectionUnrequireEvent());

            // Send login settings if we were able to obtain them.
            if (loginSettings != null)
            {
                await sender.Send(new LoginSettings(loginSettings));
            }
        }

        /// <summary>
        /// Processes a <see cref="LoggedInUserProfileRequest"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessLoggedInUserProfileRequest(Peer sender, LoggedInUserProfileRequest request)
        {
            // Verify the session details provided
            if (!CheckUserSessionValid(request.Session, request.UserId))
            {
                await sender.Send(new LoggedInUserProfileFailure(request.UserId, HttpStatusCode.BadRequest, "Authentication failed"));
                return;
            }

            // Obtain the account associated with the request.
            AccountResource? account = Storage.Accounts.Get(request.UserId);
            if (account == null)
            {
                await sender.Send(new LoggedInUserProfileFailure(request.UserId, HttpStatusCode.BadRequest, "Failed to obtain profile"));
                return;
            }

            // Send the account profile to the user.
            await sender.Send(new LoggedInUserProfileSuccess(request.UserId, account.Profile));
        }

        /// <summary>
        /// Processes a <see cref="OtherUserProfileRequest"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessOtherUserProfileRequest(Peer sender, OtherUserProfileRequest request)
        {
            // Obtain the account associated with the request.
            AccountResource? account = Storage.Accounts.Get(request.UserId);
            if (account == null)
            {
                await sender.Send(new OtherUserProfileFailure(request.UserId, HttpStatusCode.BadRequest, "Failed to obtain profile"));
                return;
            }

            // Send the account profile to the user.
            await sender.Send(new OtherUserProfileSuccess(request.UserId, account.Profile.Server));
        }

        /// <summary>
        /// Processes a <see cref="UserServerProfileUpdateRequest"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessUserServerProfileUpdateRequest(Peer sender, UserServerProfileUpdateRequest request)
        {
            // Obtain the account associated with the request.
            AccountResource? account = Storage.Accounts.Get(request.UserId);
            if (account == null)
            {
                // TODO: Failure message!
                return;
            }

            // Merge the update information with the user.
            if (request.UpdateInfo.Update != null)
            {
                // Obtain the merged profile
                AccountResource.AccountServerProfile? mergedProfile = JsonUtils.MergeObjects(account.Profile.Server, request.UpdateInfo.Update);

                // Verify we have an account and the identifier didn't change (avoids overwriting another profile in storage, as it is the storage key).
                if (mergedProfile == null || mergedProfile.XPlatformId != request.UserId.ToString())
                {
                    // TODO: Send UpdateProfileFailure(?)
                    return;
                }

                // Update the server profile in the account and set it in storage.
                account.Profile.Server = mergedProfile;
                Storage.Accounts.Set(account);
            }

            // Send the account profile to the user.
            await sender.Send(new UserServerUpdateProfileSuccess(request.UserId));
        }

        /// <summary>
        /// Processes a <see cref="UpdateProfile"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessUpdateProfile(Peer sender, UpdateProfile request)
        {
            // Verify the session details provided
            if (!CheckUserSessionValid(request.Session, request.UserId))
            {
                // TODO: Send UpdateProfileFailure(?)
                return;
            }

            // Obtain the account associated with the request.
            AccountResource? account = Storage.Accounts.Get(request.UserId);
            if (account == null)
            {
                // TODO: Send UpdateProfileFailure(?)
                return;
            }

            // Verify the account identifier did not change (avoids overwriting another profile in storage, as it is the storage key).
            if (request.ClientProfile.XPlatformId != request.UserId.ToString())
            {
                // TODO: Send UpdateProfileFailure(?)
                return;
            }

            // Get the current timestamp
            ulong currentTimestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // TODO: For now, we just trust all the update data and merge it in. We should scrutinize it more.
            account.Profile.Client = request.ClientProfile;

            // Update the account.
            account.Profile.Server.UpdateTime = currentTimestamp;
            account.Profile.Server.ModifyTime = currentTimestamp;
            Storage.Accounts.Set(account);

            // Send the account profile to the user.
            await sender.Send(new UpdateProfileSuccess(request.UserId));
            await sender.Send(new TcpConnectionUnrequireEvent());
        }

        /// <summary>
        /// Processes a <see cref="ChannelInfoRequest"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessChannelInfoRequest(Peer sender, ChannelInfoRequest request)
        {
            // Try to obtain our channel info
            ChannelInfoResource? channelInfo = Storage.ChannelInfo.Get();
            if (channelInfo != null)
                await sender.Send(new ChannelInfoResponse(channelInfo));
            await sender.Send(new TcpConnectionUnrequireEvent());
        }

        /// <summary>
        /// Processes a <see cref="DocumentRequestv2"/>.
        /// </summary>
        /// <param name="sender">The sender of the request.</param>
        /// <param name="request">The request contents.</param>
        private async Task ProcessDocumentRequestv2(Peer sender, DocumentRequestv2 request)
        {
            // Obtain the symbols for the document name and language.
            long? nameSymbol = SymbolCache.GetSymbol(request.Name);
            long? languageSymbol = SymbolCache.GetSymbol(request.Language);

            // If we couldn't resolve the name or language, return a failure.
            if (nameSymbol == null)
            {
                await sender.Send(new DocumentFailure(1, 0, $"Could not resolve symbol for document name"));
                return;
            }
            if (languageSymbol == null)
            {
                await sender.Send(new DocumentFailure(1, 0, $"Could not resolve symbol for document language"));
                return;
            }

            // Fetch the document from storage
            DocumentResource? resource = Storage.Documents.Get((request.Name, request.Language));
            if (resource == null)
            {
                await sender.Send(new DocumentFailure(1, 0, $"Could not find document"));
                return;
            }

            // Send the document in response.
            await sender.Send(new DocumentSuccess(nameSymbol.Value, resource));
            await sender.Send(new TcpConnectionUnrequireEvent());
        }
        #endregion
    }
}