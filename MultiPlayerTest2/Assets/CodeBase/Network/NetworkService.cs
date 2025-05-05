using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace CodeBase.Network
{
    public class NetworkService : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string _roomName = "TestRoom";
        [SerializeField] private byte _maxPlayers = 4;
        [SerializeField] private CanvasGroup _canvasGroup;
        private bool _isConnectedToMaster;

        private void Start()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            // Устанавливаем уникальное имя клиента
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
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
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to create room: {message}");
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Failed to join room: {message}");
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
        }
    }
}