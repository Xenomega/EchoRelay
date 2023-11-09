#pragma once

namespace EchoVR 
{
	// Forward declarations

	class IServerLib;
	struct Broadcaster;
	struct TcpBroadcaster;

	/// <summary>
	/// TODO: Allocator structure, used to track heap allocations and provide game plugin modules the ability to use a standardized
	/// heap.
	/// </summary>
	struct Allocator {};

	/// <summary>
	/// TODO: Some type of pool buffer structure.
	/// </summary>
	struct PoolBuffer {};
	/// <summary>
	/// TODO: A pool managing arbitrary-type objects by managing their underlying PoolBuffer objects.
	/// Note: This is incomplete and reflects an incorrect struct size.
	/// </summary>
	/// <typeparam name="T">The type of object to manage.</typeparam>
	template<typename T>
	struct Pool {};

	/// <summary>
	/// An array of a given type.
	/// </summary>
	/// <typeparam name="T">The type of elements within this array.</typeparam>
	template<typename T>
	struct Array
	{
		T* items;
		UINT64 count;
	};

	/// <summary>
	/// An array of a given type, allocated with a heap allocator.
	/// </summary>
	/// <typeparam name="T">The type of elements within this array (allocated on heap with a given allocator).</typeparam>
	template<typename T>
	struct HeapArray
	{
		T* items;
		UINT64 count;
		Allocator* allocator;
	};

	/// <summary>
	/// TODO: A structure which tracks address info, often represented as a padded sockaddr_in struct.
	/// </summary>
	struct AddressInfo
	{
		// This can be interpreted as a sockaddr_in struct at the start, the rest is padded.
		UINT64 raw[16];
	};

	/// <summary>
	/// TODO: Parsed URI object.
	/// </summary>
	const struct UriContainer 
	{
		// TODO: Placeholder to enforce structure size.
		CHAR _unk0[0x120];
	};

	/// <summary>
	/// A parsed JSON object.
	/// </summary>
	struct Json
	{
		VOID* root;
		VOID* cache;
	};

	/// <summary>
	/// Describes the level at which logging-related messages should be logged.
	/// </summary>
	enum class LogLevel : INT32
	{
		Debug = 0x1,
		Info = 0x2,
		Warning = 0x4,
		Error = 0x8,
		Default = 0xE,
		Any = 0xF,
	};

	/// <summary>
	/// A structure used to track a method which should be invoked (e.g. as callback) for a given operation.
	/// </summary>
	struct DelegateProxy
	{
		// The instance of the caller.
		VOID* instance;

		// The method to actually call through the proxy wrapper `proxyFunc`.
		UINT64 method[2];

		// The first function to call when the delegate is invoked. This is a wrapper function which is provided
		// the `method` and `instance` and prepares the data before invoking the underlying `method`.
		VOID* proxyFunc;
	};

	/// <summary>
	/// A 64-bit integer identifying a given symbol (which has an associated name, not always known).
	/// This is obtained through a hashing function.
	/// </summary>
	typedef INT64 SymbolId;

	/// <summary>
	/// A user's primary identifier for the account/platform they play on.
	/// </summary>
	struct XPlatformId
	{
		UINT64 platformCode;
		UINT64 accountId;
	};

	/// <summary>
	/// Peer refers to an index of a connected game server peer.
	/// </summary>
	typedef UINT64 Peer;
	const Peer Peer_Self = 0xFFFFFFFFFFFFFFFC;
	const Peer Peer_AllPeers = 0xFFFFFFFFFFFFFFFD;
	const Peer Peer_SelfAndAllPeers = 0xFFFFFFFFFFFFFFFE;
	const Peer Peer_InvalidPeer = 0xFFFFFFFFFFFFFFFF;


	/// <summary>
	/// Contains information about the UDP game server broadcast socket used by the server.
	/// </summary>
	struct BroadcastSocketInfo
	{
		UINT64 port : 16;
		UINT64 read : 24;
		UINT64 write : 24;
		UINT64 socket;

		// TODO: Everything past this point is unknown (size of this struct is incorrect).
	};

	/// <summary>
	/// The underlying data structure for a UDP game server Broadcaster.
	/// </summary>
	struct BroadcasterData
	{
		Allocator* allocator; // 0x00
		Broadcaster* owner; // 0x08
		BroadcastSocketInfo broadcastSocketInfo; // 0x10
		CHAR _unk0[0xE8 + (0xE8 - sizeof(broadcastSocketInfo))]; // TODO: BroadcastSocketInfo is around 0xE8 in size. Then we have 0xE8 of unknown data.
		DelegateProxy logFunc; // 0x1e0
		UINT32 selfType; // 0x200
		UINT32 dummyType; // 0x204

		// TODO: Temporarily replaced the below
		CHAR _unk1[0x78];
		//CTimer timer;

		AddressInfo addr; // 0x280 (sockaddr_in is here, padded by zeros) (currently 0x140 in size)
		CHAR displayName[128]; // @ 0x300
		CHAR name[128]; // @ 0x380

		// TODO: Everything past this point is unknown.
	};

	/// <summary>
	/// A UDP game server broadcaster provides broadcasting for the game server itself.
	/// </summary>
	struct Broadcaster
	{
		BroadcasterData* data;
	};

	/// <summary>
	/// TcpPeer refers to a TCP peer (e.g. a connection to a websocket service).
	/// </summary>
	struct TcpPeer
	{
		UINT32 index;
		UINT32 gen;
	};
	const TcpPeer TcpPeer_Self = { 0xFFFFFFFD, 0 };
	const TcpPeer TcpPeer_AllPeers = { 0xFFFFFFFE, 0 };
	const TcpPeer TcpPeer_InvalidPeer = { 0xFFFFFFFF, 0 };

	/// <summary>
	/// TODO: Map this out
	/// </summary>
	struct TcpPeerConnectionStats{};

	/// <summary>
	/// The underlying data structure for a TCP websocket connection.
	/// </summary>
	class TcpBroadcasterData
	{
		// TODO: The vtable below may be wrong in a few places, but CreatePeer() and SendToPeer() work, 
		// which are of utmost importance to this library.
	public:
		virtual VOID __Unknown0() = 0;
		virtual ~TcpBroadcasterData() = 0;
		virtual VOID Shutdown() = 0;
		virtual UINT32 IsServer() = 0;
		virtual VOID AddPeerFromBuffer(PoolBuffer* buffer) = 0;
		virtual UINT64 GetPeerCount() = 0;
		virtual UINT32 HasPeer(TcpPeer) = 0;
		virtual UINT32 IsPeerConnecting(TcpPeer) = 0;
		virtual UINT32 IsPeerConnected(TcpPeer) = 0;
		virtual UINT32 IsPeerDisconnecting(TcpPeer) = 0;
		virtual AddressInfo* GetPeerAddress(AddressInfo* result, TcpPeer) = 0;
		virtual VOID __Unknown1() = 0;
		virtual const CHAR* GetPeerDisplayName(TcpPeer) = 0;
		virtual TcpPeer* GetPeerByAddress(TcpPeer* result, const AddressInfo*) = 0;
		virtual TcpPeer* GetPeerByIndex(TcpPeer* result, UINT32) = 0;
		virtual VOID FreePeer(TcpPeer) = 0;
		virtual VOID DisconnectPeer(TcpPeer) = 0;
		virtual VOID DisconnectAllPeers() = 0;
		virtual VOID __Unknown2() = 0; // TODO: This was put here to shift vtable to allow CreatePeer/SendToPeer to work. Maybe the shift is higher?
		virtual TcpPeer* CreatePeer( TcpPeer* result, const UriContainer*) = 0;
		virtual VOID DestroyPeer(TcpPeer) = 0;
		virtual VOID SendToPeer(TcpPeer, SymbolId msgtype, const VOID* item, UINT64 itemSize, const VOID* buffer, UINT64 bufferSize) = 0;
		virtual VOID Update() = 0;
		virtual UINT32 Update_2(UINT32, UINT32) = 0;
		virtual UINT32 HandlePeer(SymbolId, TcpPeer, const VOID*, UINT64) = 0;
		virtual const TcpPeerConnectionStats* GetPeerConnectionStats(TcpPeer) = 0;
		virtual TcpPeerConnectionStats* GetPeerConnectionStats_0(TcpPeer) = 0;

		// Data
		TcpBroadcaster* owner;
		AddressInfo addressInfo;
		CHAR displayName[24];
		CHAR name[24];

		// TODO: Everything past this point is unknown.
	};

	/// <summary>
	/// A TCP broadcaster/connection, used as a client to connect to central services.
	/// </summary>
	struct TcpBroadcaster
	{
		TcpBroadcasterData* data;
	};

	/// <summary>
	/// Lobby type describes the privacy-access level of a game session.
	/// </summary>
	enum class LobbyType : INT8
	{
		Public = 0x0,
		Private = 0x1,
		Unassigned = 0x2,
	};

	/// <summary>
	/// Describes the state of a net game
	/// </summary>
	enum class NetGameState : INT32
	{
		OSNeedsUpdate = -100,
		OBBMissing = -99,
		NoNetwork = -98,
		BroadcasterError = -97,
		CertificateError = -96,
		ServiceUnavailable = -95,
		LoginFailed = -94,
		LoginReplaced = -93,
		LobbyBooted = -92,
		LoadFailed = -91,
		LoggedOut = 0,
		LoadingRoot = 1,
		LoggingIn = 2,
		LoggedIn = 3,
		LoadingGlobal = 4,
		Lobby = 5,
		ServerLoading = 6,
		LoadingLevel = 7,
		ReadyForGame = 8,
		InGame = 9
	};

	/// <summary>
	/// The main structure used to track lobby/game session information for the current game.
	/// Lobby objects can be local, dedicated, etc. As a game server, this is a dedicated lobby object.
	/// </summary>
	struct Lobby {

		/// <summary>
		/// Information for each entrant/player in the game server.
		/// </summary>
		struct EntrantData
		{
			XPlatformId userId;
			SymbolId platformId;
			CHAR uniqueName[36];
			CHAR displayName[36];
			CHAR sfwDisplayName[36];
			INT32 censored;
			UINT16 owned : 1;
			UINT16 dirty : 1;
			UINT16 crossplayEnabled : 1;
			UINT16 unused : 13;
			UINT16 ping;
			UINT16 genIndex;
			UINT16 teamIndex;
			Json json;
		};

		/// <summary>
		/// Information for each local entrant on this machine, in the game server.
		/// </summary>
		struct LocalEntrantv2
		{
			GUID loginSession;
			XPlatformId userId;
			GUID playerSession;
			UINT16 teamIndex;
			BYTE padding[6];
		};


		// TODO
		VOID* _unk0; // 0x00

		Broadcaster* broadcaster; // 0x08
		TcpBroadcaster* tcpBroadcaster; // 0x10
		UINT32 maxEntrants; // 0x18

		UINT32 hostingFlags; // 0x1C (second bit set => pass ownership of host)
		CHAR _unk2[0x10]; // 0x20

		INT64 serverLibraryModule; // 0x30
		IServerLib* serverLibray; // 0x38

		DelegateProxy acceptEntrantFunc; // 0x40
		CHAR _unk3[0xD0]; // 0x60

		UINT32 hosting; // 0x130

		CHAR _unk4[0x04]; // 0x134

		Peer hostPeer; // 0x138
		Peer internalHostPeer; // 0x140

		Pool<LocalEntrantv2> localEntrants; // 0x148

		CHAR _unk5[0x84 - sizeof(localEntrants)]; // unknown data until 0x1CC.

		GUID gameSessionId; // 0x1CC

		CHAR _unk6[0x10]; // 0x1DC

		UINT32 entrantsLocked; // 0x1EC
		UINT64 ownerSlot; // 0x1F0
		UINT32 ownerChanged; // 0x1F8 (TODO: verify)

		CHAR _unk7[0x360 - 0x1FC]; // 0x1FC

		HeapArray<Lobby::EntrantData> entrantData;// 0x360


		// TODOs:
		// 
		// Known to exist, but missing: 
		// - entrant connections struct array (HeapArray<struct>)
		// - registration pending (bool, 32-bit) (indicates game server registration succeeded)
		// - server's platform symbol (SymbolId)
		// - crossplay enabled (bool, 32-bit)
		// - lobby type of current game session (LobbyType type)
		// 
		// Notes:
		// 0x358 (QWORD) set to 1 will load map instead of load server in some circumstances.
	};




	/// <summary>
	/// IServerLib describes an interface for a Echo VR game server library which the game
	/// loads by default from "pnsradgameserver.dll" in the game folder, or alternatively
	/// can be set using a JSON key in the config.
	/// </summary>
	class IServerLib
	{
	public:
		virtual INT64 UnkFunc0(VOID* unk1, INT64 a2, INT64 a3) = 0;
		virtual VOID* Initialize(Lobby* lobby, Broadcaster* broadcaster, VOID* unk2, const CHAR* logPath) = 0;
		virtual VOID Terminate() = 0;
		virtual VOID Update() = 0;
		virtual VOID UnkFunc1(UINT64 unk) = 0;


		virtual VOID RequestRegistration(INT64 serverId, CHAR* radId, SymbolId regionId, SymbolId lockedVersion, const Json* localConfig) = 0;
		virtual VOID Unregister() = 0;
		virtual VOID EndSession() = 0;
		virtual VOID LockPlayerSessions() = 0;
		virtual VOID UnlockPlayerSessions() = 0;
		virtual VOID AcceptPlayerSessions(Array<GUID>* playerUuids) = 0;
		virtual VOID RemovePlayerSession(GUID* playerUuid) = 0;
	};
}
