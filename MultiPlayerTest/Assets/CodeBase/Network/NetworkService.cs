using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkService : MonoBehaviourPunCallbacks
{
	[SerializeField] private string roomName = "TestRoom";
	[SerializeField] private byte maxPlayers = 4;

	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings(); // Подключаемся к Photon
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Photon Master Server");
		// Показываем UI для хоста/клиента
	}

	public void StartHost()
	{
		RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
		PhotonNetwork.CreateRoom(roomName, roomOptions);
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom(roomName);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
		// Спавним игрока в случайной позиции
		Vector3 spawnPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
		PhotonNetwork.Instantiate("PlayerPrefab", spawnPos, Quaternion.identity);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.LogError($"Failed to create room: {message}");
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.LogError($"Failed to join room: {message}");
	}
}