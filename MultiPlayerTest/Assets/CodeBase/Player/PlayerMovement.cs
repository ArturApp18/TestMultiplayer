using CodeBase.CameraLogic;
using Photon.Pun;
using UnityEngine;

namespace CodeBase.Player
{
	public class PlayerMovement : MonoBehaviourPunCallbacks
	{
		private float _speed = 5f;

		void Start()
		{
			if (photonView.IsMine)
			{
				// Настраиваем камеру для локального игрока
				Camera.main.GetComponent<CameraFollow>().SetTarget(transform);
			}
		}

		void Update()
		{
			if (!photonView.IsMine) return; // Управляем только своим игроком

			// Движение WASD/стрелками
			float moveX = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;
			float moveZ = Input.GetAxis("Vertical") * _speed * Time.deltaTime;
			transform.Translate(moveX, 0, moveZ);
		}
	}
}