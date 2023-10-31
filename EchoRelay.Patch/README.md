# EchoRelay.Patch

This library is intended to be loaded alongside Echo VR. It applies startup patches and hooks to add to, unlock, and fix functionality within Echo VR.

To install this component, read the installation instructions within the solution's [README](../README.md).

## Patches

`EchoRelay.Patch` adds or updates a number of Echo VR's command-line arguments:
- `-server`: A new CLI argument that runs an Echo VR instance as a game server. It will automatically register itself to the `SERVERDB` service on the central server after startup. It is then ready to serve users requesting matchmaking. When a client is matched, the new session will be started. When all clients disconnect, the game server will end the game session and await a new one from matched clients (recycling itself).
- `-offline`: A new CLI argument that enables offline gameplay. This must be paired with a `-level`, `-gametype` and `-region` argument to successfully load a desired level.
- `-windowed`: A new CLI argument that allows the game to be run in a windowed mode (rather than through a VR headset). This is similar to the original `-spectatorstream` argument, but without requesting a spectator game on startup.
- `-headless`: This existing CLI command was intended to start the game in a console window without graphics/audio, but typically crashed the game on startup. This feature was partially redesigned.

In addition to updated CLI commands, `EchoRelay.Patch` also applies the following patches:
- Adds support for a `apiservice_host` JSON key in the local service config, to override the HTTP(S) API server URI typically hardcoded in the game.
- Allows `-noovr` to be used without `-spectatorstream`, allowing demo profiles to be used with a VR headset or within `-windowed` mode.
- (If compiled in `DEBUG` build configuration) Disables the deadlock monitor which ensures threads do not hang. This is inadvertently triggered when setting breakpoints on Echo VR for too long, which circumvents research efforts. Removing it bypasses this, but should not be used outside of testing, in case a real deadlock occurs which the game does not respond to.

## Known issues

- `-headless` produces much higher than usual CPU usage.
- `-headless`'s console window is prone to thread blocking/crashing if you hold a mouse key down on the console window or otherwise block the window UI, as it may block any game threads trying to log a message in the window.
