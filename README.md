# EchoRelay

[_**This project is discontinued**: It is unlikely to get any updates past this point, unless my last commit introduced major breaking changes._]

`EchoRelay` is a proof-of-concept reimplementation of [Echo VR](https://en.wikipedia.org/wiki/Lone_Echo)'s web services and dedicated game servers. It was created for
educational/research purposes, to explore video game backend infrastructure design and service implementations.

Echo VR's official servers were shut down on August 1st, 2023. `EchoRelay` established the first networked match since that time, on August 29th, 2023. 
This project was made public October 31st, 2023. 

This project's aim is not to enable unofficial matches for the public, please read the see the [Disclaimer](#disclaimer) 
section for more information about the project, its aim, goals, and sensitivities.

![EchoRelay ](./EchoRelay.Cli/Resources/screenshot.png)

<sup>"さあ時の扉を開けて行こうよ"</sup>

**Note**: All research was solely done by me ([@Xenomega](https://github.com/Xenomega)). I am not a participant of any Echo VR community.

## Features

The following features are supported by `EchoRelay`:

- ✔️ Extended Echo VR command-line arguments to launch the game:
	- ✔️ As an offline client (no server) (`-offline`)
	- ✔️ As a dedicated game server (`-server`)
	- ✔️ In windowed mode (no VR headset) (`-windowed`)
	- ✔️ In headless mode, a console-based process with no graphics or audio (`-headless`)
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

At a high level, Echo VR's original backend infrastructure can be boiled down to:

1. **Central services**: responsible for core services (e.g. account operations) and to discover game servers to match to.
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

1. **Central services**:
	- [**EchoRelay.Core**](./EchoRelay.Core/): A C#.NET library providing an implementation of the central server with supported services.
	- [**EchoRelay.App**](./EchoRelay.App/): A simple C#.NET WinForms UI app providing visual configuration, operation, and monitoring of a central server powered by `EchoRelay.Core`.
	- [**EchoRelay.Cli**](./EchoRelay.Cli/): A simple C#.NET CLI (command-line interface) tool, providing lightweight options to deploy and operate a server powered by `EchoRelay.Core`.

2. **Dedicated game servers**: 
	- [**EchoRelay.Patch**](./EchoRelay.Patch/): A C++ library to be loaded alongside Echo VR. It applies patches to the game on startup, enabling additional CLI commands in Echo VR (e.g. `-server`, required to operate a game server).
	- [**EchoRelay.GameServer**](./EchoRelay.GameServer/): A C++ library which reimplements the interface the game expects from `pnsradgameserver.dll`. It accepts requests to register the game server, listens for websocket messages from `SERVERDB` such as starting a new session, accepting new players, rejecting/kicking a player, etc. 
	- This introduces unofficial websocket messages, likely similar to the original `pnsradgameserver.dll`, but specific to `EchoRelay.Core`'s central service reimplementation.


## Installation

This section discusses end-to-end installation of the system. However, if `EchoRelay` servers are already operational, a minimal client setup 
only requires a single JSON file provided by the server operator to be saved in the client's game folder, as noted in the later [Setting up clients](#setting-up-clients) section.


### Requirements

- A legitimately owned, licensed copy of Echo VR (version 34.4.631547.1).

### Fetching binaries

You must acquire built libraries and executables for `EchoRelay` to install them. There are two options:

1. **Pre-compiled binaries** 
	- Ensure you have the .NET 7.0 runtime installed.
	- Download pre-compiled binaries from the [Releases](https://github.com/Xenomega/EchoRelay/releases) page of this repository.
		- Central services can be run on any platform via `EchoRelay.Cli`. Windows additionally supports `EchoRelay.App` for a basic UI version.
		- Game server operation requires Echo VR, and thus is Windows-only.
2. **Building from source code**
	- Ensure you have Windows 10+, .NET 7.0 SDK, and Microsoft Visual Studio 2022 with C# and C++ (vc143) support installed.
	- Clone this repository and open the solution (`.sln`) file in Visual Studio.
	- Build the solution, it should succeed without any errors. 
  		- If you encounter an error related to a missing MSDetours dependency, manually install the MSDetours 4.0.1 nuget package to the `EchoRelay.Patch` project.
	- Rename `EchoRelay.Patch.dll` -> `dbgcore.dll`, and rename  `EchoRelay.GameServer.dll` -> `pnsradgameserver.dll`.
	- **Note**: `EchoRelay.Cli` and `EchoRelay.Core` can be built for other platforms by simply running the `dotnet build` command, after installing the .NET 7.0 SDK.

### Setting up central services

As the name suggests, central services are the main central endpoint which all clients/game servers will connect to (e.g. for logging in, matchmaking, discovery and registration of game servers). It should be at a stable/static IP. 

**Option 1: Using the WinForms UI app**
1. Run `EchoRelay.App` as an administrator. It will ask you to configure a storage folder and locate the Echo VR game executable. Note the TCP port number you set here.
2. Within the application, click the button with the "Play" icon to start the server, if it isn't started/listening already.
3. The application should now display a generated "service config" containing your endpoints necessary to connect to the server. Copy or save this.
4. Ensure access controls are appropriately set in the `Storage`->`Access Controls` tab of the application, to prevent unwanted connections from being established.

**Option 2: Using the command-line app**
1. Run `EchoRelay.Cli` as an administrator, with the argument `-d` providing an existing, but empty storage directory for server resources, `-p` for TCP port (default 777), etc.
	- For more command options, run `EchoRelay.Cli --help` or read the project's [README](./EchoRelay.Cli/README.md).
2. When the server starts, it should output some information such as a service config. Either copy this, or use the `--outputconfig` argument to save it to a file.


**Port forwarding**
- Finally, port forward the TCP port you configured for `EchoRelay.App` or `EchoRelay.Cli` on your router (and Windows or software-based firewalls, if enabled).
	- **Context**: This is necessary, as `EchoRelay` only supports external/public IPs connections, hence why the service config is generated with your external IP.

The central server should stay running for all services to work. The same storage folder should be used each time for persistent data across server restarts.

### Setting up game servers

After central services are set up, you'll want to stand up some game servers. They will register themselves to central services, and central services will then use them to matchmake clients to. Game servers can run on machines independent on the central services, so long as they have the correct service config file, with the correct `SERVERDB` API key.
1. Copy `dbgcore.dll` and `pnsradgameserver.dll` into your game folder where `echovr.exe` is located. 
2. The central services/server operator should provide game server operators with the service config generated by their central server (from Step 3 of their setup). It should be saved to `ready-at-dawn-echo-arena/_local/config.json`.
	- You should update the `displayname` and `auth` parameters of the login service URI to values personal to your account, as specified by the [FAQs](#faqs) section of this document.
3. Look at `ready-at-dawn-echo-arena\sourcedb\rad15\json\r14\config\netconfig_dedicatedserver.json`, and note the `port` and `retries` JSON keys. 					
    - `port` will be the first UDP port that game servers you start will try to bind to. If they fail, they will increase the port number for as many `retries` as defined, and try again.
	  - e.g. With a `port` of 1234 and a `retries` set to 10, game servers will bind to the first available UDP port in the range: 1234-1244
	- You may wish to increase `retries` in the previous step to a larger number. This will avoid multiple game servers/clients reserving all these ports. Note that `netconfig_client.json` is configured by default to also reserve one of these ports from the same range when a client is run.
4. Port forward the UDP game server port range on your router (and Windows/software firewalls, if enabled). Ensure the external port matches the internal port when forwarding.

**Every central server should have at least two game servers running**: one for your player's current lobby, another for the player to transition to a new lobby with a new gametype.
- This can be done with quick launch options within `EchoRelay.App` under `Tools`->`Launch Echo VR` if you are running game servers from the same machine.
- Alternatively, launch the game server with the command `echovr.exe -server -headless -noovr` (standard, lightweight) or `echovr.exe -server -windowed -noovr` (windowed mode with fly-cam) through a command prompt. 
	- Note: Dedicated game servers were only thoroughly tested with `-noovr`. Use of OVR platform is not recommended.

### Setting up clients

This section describes the setup required to get a player connected to a configured `EchoRelay` network:

1. The central services/server operator should provide clients with the service config generated by their central server app (in Step 3 of their setup). It should be saved to `ready-at-dawn-echo-arena/_local/config.json` in the client's game folder.
	- If a machine is meant to run client-only, the `serverdb_host` key in the service config should not be provided by the server operator, and they should enforce API key authentication to protect against unauthenticated game servers.
	- Clients should update the `displayname` and `auth` parameters of the login service URI to values personal to their account, as specified by the [FAQs](#faqs) section of this document.

### Playing Echo VR

Now your `EchoRelay` central services should be operational, game servers and clients should be configured to connect and matchmake to eachother!

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
	- Under this model, authentication is done service-to-service, with only the real developers having the server-sided `APP_SECRET` needed to authenticate and obtain user information. This is never distributed, so no one (e.g. `EchoRelay`) can access sensitive account details or authenticate users.
	- Additionally, clients do not require any code modifications, thus client-side security measures are intact. The patches are only necessary to enable operation of a game server.
- **Paid content (e.g. player skins) unlocks ARE NOT supported**:
	- Users are only given unlocks the game provides by default to all users.
	- The config resources for store items, battle pass seasons, and other paid content are not implemented.
	- The transaction service and store features are purposefully unimplemented. The transaction service implementation only responds with dummy data for one message (to avoid client errors), but does not reveal details regarding a real implementation.
- **Access Control Lists (ACLs) enforce IP-based whitelisting**: Researchers looking to play with this solution are to leverage ACLs to prevent unauthorized parties from communicating with their server instance.
- **Support IS NOT provided**: This project will never aim to go beyond the scope of personal educational research.
	- The issue tracker has been purposefully closed. 
	- This solution should not be expected to be maintained past this point.
	- I do not care about feature completion. 
	- I do not care about rich features for end users. 
	- I do not condone publicly hosting matches with this solution.
	- Do not ask for support!

**TLDR**: If you're not looking to explore reverse engineering techniques or server/service implementations as a personal researcher, this project is not meant for you. It should not be thought of as a feature-complete or bug-free server solution.


## FAQs

**Will this be regularly maintained, and supported?**
- No. I consider my work complete here for all intents and purposes. I accomplished the goals I had in scope for the proof-of-concept. I don't care to add any new functionality, make this scalable, production-ready, or any of that nonsense.
- Unless my last commit further broke something fundamental that I'd feel bad showcasing on my GitHub, that wasn't broken before, don't expect another commit.

**Why so many components? Why not simplify it?**
- The architecture is designed after the original developers', which used headless game servers (via special game builds) and central websocket services. This project simply contains additional patcher DLL (`EchoRelay.Patch`) in addition to their typical architectural design, to unlock, hook, and recreate the game server/headless functionality.
- Clients require more than one game server to transition between lobbies (e.g. a social lobby and a game). So you need multiple game servers anyways. Even if you spent months re-implementing game servers yourself, you'd need to parallelize them to support clients without code modifications.

**Why only the PC VR version of the game? Why not the Quest APK?**
- Conceptually, the server code should be able to work for both just with minor trivial tweaks. The protocol is the same, so this project completed the hard work. I just didn't have a Quest headset on-hand to try patching the endpoints and fixing the client-side login flow. 
- In the end, I considered it out of scope for my project. I anticipate it to be easy to flesh out, which makes it even less interesting for me to pursue.

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

**Why is my game in a persistently broken state (e.g. permanently 'ghosted', persistently "failed to connect" to a specific gametype, etc)?**
- I've observed local profile corruption and game file corruption. They were extremely rare and might've been triggered by too much network switching and odd tests. Or it could be something as simple as too many concurrent clients/game servers colliding.
- First, consider reviewing logs in `ready-at-dawn-echo-arena\_local\r14logs\*` for additional information with any issues you have.
- If you have a ghosting issue issue specifically, you can reset this from settings at the Echo VR main menu.
- If all else fails, backup and clear your local user profile and settings by deleting everything in the `%LOCALAPPDATA%\rad\` folder. Restore original game files and re-install `EchoRelay`. 

**Can I use your code in my own projects?**
- Sure. If your work is derivative, please label it as such, so I am not confused as the author of your work and you do not take credit for mine. Just give a shout-out to the [original repository](https://github.com/Xenomega/EchoRelay) somewhere visible in your project, e.g. a README, an about page, etc. 
- Please don't associate `EchoRelay` to any malicious or unlawful behavior.

## Shoutouts

Thanks to [@dualgame](https://github.com/Dualgame) for information about how different user flows are expected to work, you were a tremendous help in providing context, so this could be reimplemented and operate as expected.
