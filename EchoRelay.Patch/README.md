# EchoRelay.Patch

This library is intended to be loaded alongside Echo VR. It applies startup patches and hooks to add to, unlock, and fix functionality within Echo VR.

To install this component, read the installation instructions within the solution's [README](../README.md).

## Patches

`EchoRelay.Patch` adds or updates a number of Echo VR's command-line arguments:
- `-server`: A new CLI argument that runs an Echo VR instance as a game server. It will automatically register itself to the `SERVERDB` service on the central server after startup. It is then ready to serve users requesting matchmaking. When a client is matched, the new session will be started. When all clients disconnect, the game server will end the game session and await a new one from matched clients (recycling itself).
- `-offline`: A new CLI argument that enables offline gameplay. This must be paired with a `-level`, `-gametype` and `-region` argument to successfully load a desired level.
- `-windowed`: A new CLI argument that allows the game to be run in a windowed mode (rather than through a VR headset). This is similar to the original `-spectatorstream` argument, but without requesting a spectator game on startup.
- `-headless`: This existing CLI command was intended to start the game in a console window without graphics/audio, but typically crashed the game on startup. This feature was partially redesigned.
- `-timestep`: Sets the tickrate/timestep (in ticks/s) for a `-headless` process, as tick rate is not tied to refresh rate in headless. Default is 120 ticks/s.
	- If not specified, a default value is used. 
	- If zero is provided, tick rate is unthrottled.
	- **Warning**: If your machine can't keep up with the timestep (tickrate is set too high), in-game speed will slow down for clients, so timestep should be lowered accordingly. By default it was set as at a conservative level to avoid issues.
	- Note: Client-side ping readings are transformed by time step, so adjustment of this may affect the ping numbers displayed to clients (but will not actually reflect the real ping).

In addition to updated CLI commands, `EchoRelay.Patch` also applies the following patches:
- Allows `-noovr` in windowed mode, adding a "[DEMO]" suffix to the window title.
- Adds support for a `apiservice_host` JSON key in the local service config, to override the HTTP(S) API server URI typically hardcoded in the game.
- Failure to load a level as a dedicated server instead recreates the game session silently. This ensures the game server is always ready to serve a new lobby and does not enter a trapped state.
- (If compiled in `DEBUG` build configuration) Disables the deadlock monitor which ensures threads do not hang. This is inadvertently triggered when setting breakpoints on Echo VR for too long, which circumvents research efforts. Removing it bypasses this, but should not be used outside of testing, in case a real deadlock occurs which the game does not respond to.

Note: The latest version of this library performs version checks against the `echovr.exe` on startup. It will warn you if the version of the game does not match the required version for the patcher. It will try to continue, in case there's some edge cases that should be allowed, but is unlikely to succeed in such a case.

## Known issues

- Dedicated servers have at times failed to load a level which should've succeeded. This is likely due to concurrent access issues on the same package files, on the same machine, by different instances of the game(?). 
	- Although we currently re-direct failure to load a level to instead recreate the game session, connected clients will receive the wrong error message ("server is full", "could not connect"), instead of one indicating the server failed to load a level.
	- In the least, the current solution keeps servers from crashing or becoming trapped at a "failed to load level" screen, and clients from being stuck waiting for server to load.
- Dedicated servers at times referenced unallocated memory when trying to close them, this was patched to avoid almost all cases, but was not thoroughly tested. 
- `-headless`'s console window is prone to thread blocking/crashing if you hold a mouse key down on the console window or otherwise block the window UI, as it may block any game threads trying to log a message in the window.