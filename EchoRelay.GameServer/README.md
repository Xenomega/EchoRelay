# EchoRelay.GameServer

This library is loaded after Echo VR successfully completes startup and has reached the mainmenu, while in dedicated server mode (`-server`).

It provides a game server library interface which is responsible for intercommunication between Echo VR and the `SERVERDB` service.

The game server library receives calls from Echo VR, providing information for the game server to register itself with `SERVERDB`.
The library also listens for messages from websocket services requesting a new session be started, expectation of a new peer connection 
with given packet encoder settings, acceptance of a new player requesting to join over an established connection, rejection/kicking of a player. 

To install this component, read the installation instructions within the solution's [README](../README.md).

## Known issues

- There are some minor edge cases where we should send some failure messages internally for some conditions that we are not. These are marked with TODOs inline in the code. They are non-critical.
- The "start session" message which is provided by `EchoRelay.Core` and relayed by this library to Echo VR, is not complete. There is a structure at the end of the message that was not supported/included. This is rarely an issue as the game will treat it as empty if it's not provided, but is likely the cause of cooperative AI match requests crashing game servers. The structure defines parameters such as player ids in the game (including bots), and team limits.
- Echo VR's heap allocator structures should be used to allocate heap memory safely. This isn't difficult, but requires a bit more work to support. Instead we allocate on stack where we can, which is safe as well.
