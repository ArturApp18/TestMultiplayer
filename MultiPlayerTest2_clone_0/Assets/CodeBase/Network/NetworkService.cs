using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Steamworks;
using Random = UnityEngine.Random;

namespace CodeBase.Network
{
    public class NetworkService : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string _roomName = "TestRoom";
        [SerializeField] private byte _maxPlayers = 4;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CoinSpawner _coinSpawner;
        [SerializeField] private SteamManager _steamManager;

        private bool _isConnectedToMaster;
        public string RoomName => _roomName;

        private Callback<LobbyEnter_t> m_LobbyEntered;

        private void Awake()
        {
            m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        private void Start()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;

            if (_steamManager.Initialized)
            {
                string steamID = SteamUser.GetSteamID().ToString();
                string playerName = SteamFriends.GetPersonaName();

                PhotonNetwork.AuthValues = new AuthenticationValues(steamID);
                PhotonNetwork.NickName = playerName;
            }

            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Photon Master Server");
            _isConnectedToMaster = true;
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
        }

        public void StartHost()
        {
            if (!_isConnectedToMaster)
            {
                Debug.LogWarning("Not connected to Master Server yet. Please wait.");
                return;
            }

            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;

            _steamManager.CreateSteamLobby();
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = _maxPlayers };
            PhotonNetwork.CreateRoom(_roomName, roomOptions);
        }

        public void JoinRoom()
        {
            if (!_isConnectedToMaster)
            {
                Debug.LogWarning("Not connected to Master Server yet. Please wait.");
                return;
            }

            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            PhotonNetwork.JoinRoom(_roomName);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}, Player: {PhotonNetwork.NickName}");
            Vector3 spawnPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
            PhotonNetwork.Instantiate("PlayerPrefab", spawnPos, Quaternion.identity);

            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 itemPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
                    PhotonNetwork.Instantiate("PickupItem", itemPos, Quaternion.identity);
                }
                _coinSpawner.SpawnCoins();
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to create room: {message}");
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            SteamMatchmaking.LeaveLobby(_steamManager.CurrentLobbyID);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to join room: {message}");
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            Debug.Log("Entered Steam Lobby: " + callback.m_ulSteamIDLobby);
            string photonRoomName = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "photonRoom");
            if (!string.IsNullOrEmpty(photonRoomName))
            {
                PhotonNetwork.JoinRoom(photonRoomName);
            }
            else
            {
                Debug.LogError("No Photon room name found in lobby data!");
            }
        }
    }
}