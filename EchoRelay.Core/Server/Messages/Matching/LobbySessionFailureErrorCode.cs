namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// Indicates a failure code for a lobby session request.
    /// This is sent from server to client, the client will display a message corresponding to this enum.
    /// </summary>
    public enum LobbySessionFailureErrorCode : int
    {
        Timeout0 = 0,
        UpdateRequired = 1,
        BadRequest = 2,
        Timeout3 = 3,
        ServerDoesNotExist = 4,
        ServerIsIncompatible = 5,
        ServerFindFailed = 6,
        ServerIsLocked = 7,
        ServerIsFull = 8,
        InternalError = 9,
        MissingEntitlement = 10,
        BannedFromLobbyGroup = 11,
        KickedFromLobbyGroup = 12,
        NotALobbyGroupMod = 13,
    }
}
