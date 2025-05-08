using Steamworks;
using UnityEngine;

namespace CodeBase.Network
{
    public class SteamManager : MonoBehaviour
    {
        private const uint AppID = 1967530;
        private const int MaxLobbyPlayers = 4;

        private bool _isInitialized;
        private CSteamID _currentLobbyID;

        [SerializeField] private NetworkService _networkService;

        private SteamAPIHandler _steamAPIHandler;
        private SteamLobbyHandler _steamLobbyHandler;

        public bool Initialized => _isInitialized;
        public CSteamID CurrentLobbyID => _currentLobbyID;

        private void Awake()
        {
            _steamAPIHandler = new SteamAPIHandler(AppID);
            _isInitialized = _steamAPIHandler.Initialize();

            if (!_isInitialized)
            {
                Application.Quit();
                return;
            }

            _steamLobbyHandler = new SteamLobbyHandler(_networkService, OnLobbyCreated, OnGameLobbyJoinRequested);
        }

        private void Update()
        {
            if (_isInitialized)
            {
                _steamAPIHandler.RunCallbacks();
            }
        }

        private void OnDestroy()
        {
            if (_isInitialized)
            {
                _steamAPIHandler.Shutdown();
            }
        }

        public void CreateSteamLobby()
        {
            _steamLobbyHandler.CreateLobby(MaxLobbyPlayers);
        }

        private void OnLobbyCreated(CSteamID lobbyID)
        {
            _currentLobbyID = lobbyID;
        }

        private void OnGameLobbyJoinRequested(CSteamID lobbyID)
        {
            _steamLobbyHandler.JoinLobby(lobbyID);
        }
    }

}