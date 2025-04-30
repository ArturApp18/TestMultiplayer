using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using SimulationGameCreator;
using UnityEngine.SceneManagement;

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
            PhotonNetwork.AutomaticallySyncScene = true;
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
            PhotonNetwork.ConnectUsingSettings();
            DontDestroyOnLoad(this);
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
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GamePlay");
            }

            StartCoroutine(WaitForSceneAndSpawn());
        }

        private IEnumerator WaitForSceneAndSpawn()
        {
            while (SceneManager.GetActiveScene().name != "GamePlay")
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            Debug.Log("Spawning player...");
            Vector3 spawnPos = new Vector3(-76.175f, 12.382f, -90.42f);
            GameObject player = PhotonNetwork.Instantiate("SimulationFPSPlayer", spawnPos, Quaternion.identity);
            PhotonView playerView = player.GetComponent<PhotonView>();
            if (playerView.IsMine)
            {
                AdvancedGameManager gameManager = player.GetComponentInChildren<AdvancedGameManager>();
                MeshRenderer meshRenderer = player.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                if (player == null)
                {
                    Debug.LogError(
                        "Failed to instantiate player prefab. Ensure 'SimulationFPSPlayer' is in Resources folder and has PhotonView.");
                }
                else
                {
                    Debug.Log("Player spawned successfully.");
                    gameManager.enabled = true;
                }
            }
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