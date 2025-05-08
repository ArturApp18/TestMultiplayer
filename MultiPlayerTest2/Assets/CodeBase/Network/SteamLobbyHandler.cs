using Steamworks;

namespace CodeBase.Network
{
	public class SteamLobbyHandler
	{
		private readonly NetworkService _networkService;
		private readonly System.Action<CSteamID> _onLobbyCreated;
		private readonly System.Action<CSteamID> _onGameLobbyJoinRequested;

		private Callback<LobbyCreated_t> _lobbyCreatedCallback;
		private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequestedCallback;

		public SteamLobbyHandler(NetworkService networkService, System.Action<CSteamID> onLobbyCreated, System.Action<CSteamID> onGameLobbyJoinRequested)
		{
			_networkService = networkService;
			_onLobbyCreated = onLobbyCreated;
			_onGameLobbyJoinRequested = onGameLobbyJoinRequested;

			_lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			_gameLobbyJoinRequestedCallback = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
		}

		public void CreateLobby(int maxPlayers)
		{
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, maxPlayers);
		}

		public void JoinLobby(CSteamID lobbyID)
		{
			SteamMatchmaking.JoinLobby(lobbyID);
		}

		private void OnLobbyCreated(LobbyCreated_t callback)
		{
			if (callback.m_eResult == EResult.k_EResultOK)
			{
				var lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
				SteamMatchmaking.SetLobbyData(lobbyID, "photonRoom", _networkService.RoomName);
				SteamMatchmaking.SetLobbyData(lobbyID, "name", "GameLobby");
				SteamFriends.SetRichPresence("status", $"In Lobby: {_networkService.RoomName}");
				SteamFriends.SetRichPresence("connect", $"+connect_lobby {lobbyID.m_SteamID}");

				_onLobbyCreated?.Invoke(lobbyID);
			}
			else
			{
				_networkService.OnCreateRoomFailed(0, $"Steam Lobby creation failed: {callback.m_eResult}");
			}
		}

		private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
		{
			_onGameLobbyJoinRequested?.Invoke(callback.m_steamIDLobby);
		}
	}
}