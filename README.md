# EchoRelay

## About

`EchoRelay` is a proof-of-concept reimplementation of [Echo VR](https://en.wikipedia.org/wiki/Lone_Echo)'s web services and dedicated game servers. It was created for
educational/research purposes, to explore video game backend infrastructure design and service implementations.

This project's aim is not to enable unofficial matches for the public, please read the see the [Disclaimer](#disclaimer) 
section for more information about the project, its aim, goals, and sensitivites.

![EchoRelay ](./EchoRelay.App/Resources/screenshot.png)

Echo VR's official servers were shutdown on August 1st, 2023. `EchoRelay` established the first networked match since that time, on August 29th, 2023. This project was made public October 31st, 2023.

## Features

The following features are supported by `EchoRelay`:

- ✔️ Extended Echo VR command-line arguments to launch the game:
	- ✔️ As an offline client (no server)
	- ✔️ As a dedicated game server
	- ✔️ In windowed mode (no VR headset)
	- ✔️ In headless mode, a console-based process with no graphics or audio
	- ✔️ Use `-noovr` without `-spectatorstream`, allowing demo profiles with a VR headset or in windowed mode
- ✔️ Support for most standard in-game features:
	- ✔️ Social Lobby
	- ✔️ Echo Arena
	- ✔️ Echo Combat
	- ✔️ Local AI matches
	- ❌ Cooperative AI matches
	- ✔️ Public or private match game types
	- ✔️ Spectator and Game Admin (moderator) support
	- ✔️ Support for different client flows (e.g. `-lobbyid`, which requests joining a specific lobby by UUID)
	- ❌ Partying-up with friends in a squad
	- ❌ Persisted armor changes/updates across game sessions
- ✔️ Support for basic server operator and administrator flows:
	- ✔️ Kick users from game session
	- ✔️ Ban accounts until a given date/time
	- ✔️ Enforce allowed/denied clients through IP-based Access Control Lists.
	- ✔️ Modify server-provided resources such as accounts, login settings, channel descriptions, etc.
- ✔️ Support for most network messages, e.g. profile fetches and updates, server resource fetching, matching, etc.


## Architecture

### Design

At a high level, Echo VR's backend infrastructure can be boiled down to:

1. **Central services**:
	- `LOGIN` (websocket): User login, session management, account operations.
	- `CONFIG` (websocket): Seasonal in-game settings (e.g. season start/end, in-game display banners).
	- `MATCHING` (websocket): Fulfills client requests for matchmaking with a game server registered with `SERVERDB`.
	- `SERVERDB` (websocket): Game server registration and game session management for clients joining through `MATCHING`. 
	- `TRANSACTION` (websocket): Provides an in-game store and transaction processing.
	- `API` (HTTP/HTTPS): An API server used to report server status, news, and record additional data.
2. **Dedicated game servers**: responsible for actually hosting game lobbies for clients to matchmake to.
	- For the original developers, a special build of the game is presumably built, which exposes routines that run Echo VR in dedicated server mode.
	- Dedicated game servers load a `pnsradgameserver.dll` library from the game folder after startup (if it exists). This library satisfies an interface for the game to register and communicate with `SERVERDB` and coordinate game sessions/matching.
	- Game servers register themselves through the `SERVERDB` service, and clients matchmake to a game server through the `MATCHING` service (invoking further communication between `SERVERDB` and the dedicated game server to accept the client).
	- Clients connect to a UDP port exposed on dedicated game servers, exposed to them by `MATCHING`.

### Implementation

`EchoRelay` provides a C#.NET implementation of the abovementioned central services, and a library + patches to run the Echo VR game as a game server. Each project directory contains its own README with additional information:

- [**EchoRelay.Core**](./EchoRelay.Core/): A C#.NET library providing an implementation of the central server with supported services.
- [**EchoRelay.App**](./EchoRelay.App/): A simple/silly C#.NET WinForms UI app providing visual configuration, operation, and monitoring of a central server powered by `EchoRelay.Core`.
- [**EchoRelay.Patch**](./EchoRelay.Patch/): A C++ library to be loaded alongside Echo VR. It applies patches to the game on startup, enabling additional CLI commands in Echo VR (e.g. `-server`, required to operate a game server).
- [**EchoRelay.GameServer**](./EchoRelay.GameServer/): A C++ library which reimplements the interface the game expects from `pnsradgameserver.dll`. It accepts requests to register the game server, listens for websocket messages from `SERVERDB` such as starting a new session, accepting new players, rejecting/kicking a player, etc. 
	- This introduces unofficial websocket messages, likely similar to the original `pnsradgameserver.dll`, but specific to `EchoRelay.Core`'s central service reimplementation.


## Installation

### Requirements

- A legitimately owned, licensed copy of Echo VR (version 34.4.631547.1).
- Windows 10+
- .NET 7.0 SDK
- Microsoft Visual Studio 2022 with C# and C++ (vc143) installed.


### Building the solution

- Clone this repository and open the solution (.sln) file in Visual Studio.
- Build the solution, it should succeed without any errors. 
	- If you encounter an error related to a missing MSDetours dependency, manually install the MSDetours 4.0.1 nuget package to the `EchoRelay.Patch` project.

### Setting up central services

1. Run the `EchoRelay.App` you built, as an administrator. It will ask you to configure a storage folder and locate the Echo VR game executable. Note the TCP port number you set here.
2. Within the application, click the button with the "Play" icon to start the server, if it isn't started/listening already.
3. Note the application should now display a generated "service config" containing your endpoints necessary to connect to the server. This will be important to configure game servers and clients later.
4. Ensure your access controls are appropriately set in the `Storage`->`Access Controls` tab of the application, to prevent unwanted connections from being established.
5. Port forward the TCP port you configured for `EchoRelay.App` on your router (and Windows or software-based firewalls, if enabled).
	- **Context**: This is necessary, as `EchoRelay` only supports external/public IPs connections, hence why the service config is generated with your external IP.

### Setting up game servers

1. Rename the `EchoRelay.Patch.dll` you built to `dbgcore.dll` and place it in your game folder where `echovr.exe` is located.
2. Rename the `EchoRelay.GameServer.dll` you built to `pnsradgameserver.dll` and place it in your game folder where `echovr.exe` is located. 
3. The service config generated in Step 3 of the central services setup should be saved to `ready-at-dawn-echo-arena/_local/config.json` in your game folder.
	- You should update the `displayname` and `auth` parameters of the login service URI to values personal to your account, as specified by the [FAQs](#faqs) section of this document.
	- **Context**: Echo VR uses this config at this file path to determine the endpoints for its services.
4. Look at `ready-at-dawn-echo-arena\sourcedb\rad15\json\r14\config\netconfig_dedicatedserver.json`, and note the `port` and `retries` JSON keys. 					
    - `port` will be the first UDP port that game servers you start will try to bind to. If they fail, they will increase the port number for as many `retries` as defined, and try again.
	  - e.g. With a `port` of 1234 and a `retries` set to 10, game servers will bind to the first available UDP port in the range: 1234-1244
	- You may wish to increase `retries` in the previous step to a larger number. This will avoid multiple game servers/clients reserving all these ports. Note that `netconfig_client.json` is configured by default to also reserve one of these ports from the same range when a client is run.
5. Port forward the UDP game server port range on your router (and Windows/software firewalls, if enabled). Ensure the external port matches the internal port when forwarding.

Note: If you wish to run game servers from multiple machines, they can all register to the same `EchoRelay.App`.
- Ensure the game server ports observed in Step 4 are set to different values on each machine on the same network, to avoid port collisions.

### Setting up clients

The service config generated in Step 3 of the central services setup should be saved to `ready-at-dawn-echo-arena/_local/config.json` in your game folder.
  - If a machine is meant to run client-only, the `serverdb_host` key in the service config should not be provided, or be removed, as it allows game server registration.
  - You should update the `displayname` and `auth` parameters of the login service URI to values personal to your account, as specified by the [FAQs](#faqs) section of this document.
  - **Context**: Echo VR uses this config at this file path to determine the endpoints for its services.

### Playing Echo VR

Now your game is set up to use your `EchoRelay.App`'s endpoint. Whether you run the game as a client (normally), or as a dedicated server (`-server`), 
it will now connect to your `EchoRelay.App` server and be able to create/log into accounts, etc.

Start at least two game server instances, one for your player's current lobby, another for the player to transition to a new lobby with a new gametype.
- This can be done with quick launch options within `EchoRelay.App` under `Tools`->`Launch Echo VR` if you are running game servers from the same machine.
- Alternatively, launch the game with a command such as `echovr.exe -server -windowed -noovr` or `echovr.exe -server -headless -noovr` through CLI. 
	- Dedicated game servers have primarily been tested with `-noovr` to avoid additional complexities of non-demo accounts + the OVR platform. Although using the OVR platform as a dedicated server is unlikely to be problematic in practice, it is not recommended.

Now any configured clients can simply start the game as they normally would, and play!

## Disclaimer

This project is not intended to host unofficial services for the public, nor is it reflective of what true 
infrastructure might look like (e.g. use of a real web framework such as ASP.NET, integration with IIS, 
real database support, logging, monitoring, cloud-scalable/elastic microservices, secrets management, 
load balancing, rate limiting, isolation, etc).

Instead, `EchoRelay` aims to provide a lightweight and portable proof-of-concept which shows the results of my work to those similarly interested
in exploring these topics for research/educational purposes. The proof-of-concept has been publicized so it can be potentially referenced in a future 
document detailing design considerations for game server architecture and sharing reverse engineering techniques for those looking to educate themselves.

Echo VR is an online-enabled, free-to-play game which had its online services retired on August 1, 2023. Its compiled binaries do not employ 
anti-cheat mechanisms or any meaningful security mitigations. This made it an ideal target for me to explore the topic without introducing risk 
to an active online ecosystem or its users.

This project was developed with additional considerations to uphold ethical integrity and ensure fair-use:

- **EchoRelay DOES NOT contain copyrighted or illegal material**: The code and resources contained within this project were written entirely by me.
- **Entitlement checks HAVE NOT been modified**: You must legally own a license for the game to launch Echo VR or use this project.
- **Unmodified clients CAN NOT mistakenly connect to an unofficial server**:
	- Clients MUST explicitly set the endpoints to the new server in their game config to connect.
- **In the event of project misuse, sensitive Oculus account details of a client CAN NOT be directly exposed:**
	- Any Oculus data exposed is done so through the [Oculus User Ownership](https://developer.oculus.com/documentation/unity/ps-ownership/) model.
	- Under this model, authentication is done service-to-service, with only the real developers having the server-sided `APP_SECRET` needed to authenticate and obtain user information. This is never distributed.
	- Clients do not require any code modifications, thus client-side security measures are intact. The patches are only necessary to enable operation of a game server.
- **Paid content (e.g. player skins) unlocks ARE NOT supported**:
	- Users are only given unlocks the game provides by default to all users.
	- The config resources for store items, battle pass seasons, and other paid content are not implemented.
	- The transaction service and store features are purposefully unimplemented. The transaction service implementation only responds with dummy data for one message (to avoid client errors), but does not reveal details regarding a real implementation.
- **Access Control Lists (ACLs) enforce IP-based whitelisting**: Researchers looking to play with this solution are to leverage ACLs to prevent unauthorized parties from communicating with their server instance.
- **Support IS NOT provided**: This project will never aim to go beyond the scope of personal educational research.
	- The issue tracker has been purposefully closed. 
	- I do not care about feature completion. 
	- I do not care about rich features for end users. 
	- I do not condone publicly hosting matches with this solution.
	- Do not ask for support!

TLDR: If you're not looking to explore reverse engineering techniques or server/service implementations as a personal researcher, this project is not meant for you. It should not be thought of as a feature-complete or bug-free server solution.


## FAQs

**What are the `displayname` and `auth` parameters in the `LOGIN` service endpoint?**
- Oculus' authentication scheme is not something this project can employ and Oculus account details are not visible to this solution, as noted in the [Disclaimer](#disclaimer) section.
- This is a lazy replacement for Oculus account APIs:
  - To provide some kind of account lock, the `auth` parameter, when set once on login, will be verified to match on all future logins to prevent clients with another user's user identifier from logging into their account on the server.					
  This is a sensitive field that should not be exposed to other users. The intent would be to use a unique throw-away password, as the password will be visible to server operators.
  - To provide a way to change your in-game display name, the `displayname` parameter can be used. Your account's display name can be changed on each login. Your account will otherwise remain the same as this is not a user name or unique user identifier.

**What is API key authentication for `SERVERDB`?**
- This appends an additional URI query parameter (`api_key`) to the `serverdb_host` endpoint in the generated service config.
- This parameter must match the expected value set in `Tools`->`Settings` for all game servers connecting, otherwise they will be rejected.
- This is a lazier authentication implementation that disallows unauthorized game servers, in lieu of a real certificate-based authentication system.
- This should not be exposed to a machine which is not authorized to operating a game server.

**Why do I need to port forward and use external IPs?**
- I wanted to ensure this project works across networks as a real implementation would, which meant supporting external IPs. 
- Your local config file can specify an external IP as an endpoint, which will result in central services seeing a dedicated game server as an external connection, recording the correct IP to serve to remote clients. Yay! 
- However, with complex local network configurations, you may find some routers enforcing NAT rules that cause the external request to take a hop internally, registering the wrong IP address (a local one) as the source IP address. The C# API we rely on returns one of the IPs, but you cannot select which.
- This is a lazy solution to ensure compatibility with different network configurations. Better IP address translation can be added to avoid this requirement, but it's uninteresting to invest into for this proof-of-concept.

**Why don't central services leverage SSL/TLS?**
- Adding TLS support to `EchoRelay.Core`'s server implementation is trivial, but was considered uninteresting for the purpose of this proof-of-concept.
- It would require installation of trusted certificates to your certificate store, which adds a bit more overhead to setup and testing.
- This _may_ require patching for clients anyways, if the game employs TLS certificate pinning (expects the original TLS certificate originally used).

**Will EchoRelay overwrite my profile prior to the official server shutdown?**
- If you connect with the same machine/account, yes, the new profile from the new server environment will be synced and overwrite any profile with a matching ID saved prior.

**Is it possible to restore profiles prior to official server shutdown?**
- Yes, local copies of profiles downloaded from servers are stored in `%LOCALAPPDATA%\rad`.
- They can be manually merged into the `EchoRelay.Core` server's account resources.

**Why is my client profile broken on new server deployments (e.g. permanently 'ghosted')?**
- During heavy testing switching between new environments, I had broken my profile once and became permanently 'ghosted' (unable to appear as my character in social lobbies). It may have been something that I triggered inadvertently outside of `EchoRelay` usage.
- Conceptually, it's possible that some corruption occurs on your local profile state while moving between servers. 
- Backup, then clear your local user profile in your `%LOCALAPPDATA%\rad\` folder. It is likely your local client profile is simply corrupted.

## Shoutouts

Thanks to [@dualgame](https://github.com/Dualgame) for information about how different user flows are expected to work, you were a tremendous help in providing context, so this could be reimplemented and operate as expected.
