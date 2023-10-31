# EchoRelay.Core

This library provides functionality to host central backend services for Echo VR. 

To install this component, read the installation instructions within the solution's [README](../README.md).

## Features

`EchoRelay.Core` implements various features :
- **Account operations**: The `LOGIN` service manages logins and handles requests to update accounts, fetch other user accounts, etc.
- **Server resource management**: Handling and serving of config, document, channel info, and other resources comes via the `CONFIG` and `LOGIN` services.
- **Matching operations**: The `MATCHING` service can be configured to prioritize matching clients to game servers with lowest ping, or highest population first. It establishes per-user game server packet encoding settings, e.g. encryption and verification keys. If a session does not exist, it will create one on an unallocated registered game server.
- **Game server management**: The `SERVERDB` service accepts game server registrations, enforces API key authentication, and manages game sessions. It commands registered game servers to start new sessions, expect a connection with per-user packet encoding settings, accept a player on an established connection, reject/kick them, and tracks locking/unlocking of lobbies.
- **Access control management**: Users can be kicked from lobbies, accounts can be banned until a given time, and IP addresses can be subjected to allow/deny lists.
- **Evaluation of game symbols**: The names for message identifiers and other symbols can be observed, if the game files contain them.
- **Quick launching**: Launching Echo VR in different operating modes is wrapped through a game launcher provider, in accordance with `EchoRelay.Patch`'s extended command-line argument support.

## Known issues
- The region identifier and version lock for game servers and clients is captured but not enforced.
- Maybe spectators and moderators should not count against the game session player limit(?), but it is currently enforced that way.
- `ProcessUserServerProfileUpdateRequest` should enforce some type of authentication, currently anyone can update anyone else's server profile.
- Lacks support for at least two failure messages and simply does not respond instead: 
  - The `UpdateProfileFailure` response
  - The failure response to `UserServerProfileUpdateRequest`.
- Server resources are the same instance when loaded from storage, not copies. e.g. Profile update operations may collide in an extremely rare scenario.

There are likely other issues elsewhere. I did not spend time enumerating all of them, nor would it have been considered in-scope for this project to address all of them.
