using Photon.Pun;
using UnityEngine;

namespace CodeBase.Network
{

	public class CoinSpawner : MonoBehaviourPun
	{
		[PunRPC]
		public void SpawnCoins()
		{
			for (int i = 0; i < 5; i++)
			{
				Vector3 spawnPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
				PhotonNetwork.Instantiate("Coin", spawnPos, Quaternion.identity);
			}
		}
	}
}