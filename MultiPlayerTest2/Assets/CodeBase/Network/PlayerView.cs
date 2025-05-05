using Photon.Pun;
using TMPro;
using UnityEngine;

namespace CodeBase.Network
{
	public class PlayerView : MonoBehaviourPun
	{
		[SerializeField] private TextMeshProUGUI _scoreText;
		private int _score;

		private void Start()
		{
			if (!photonView.IsMine)
			{
				_scoreText.gameObject.SetActive(false);
			}
			UpdateScoreText();
		}

		public void OnCoinCollected(PhotonView collectorView)
		{
			if (collectorView == photonView)
			{
				_score++;
				UpdateScoreText();
			}
		}

		private void UpdateScoreText()
		{
			if (_scoreText != null)
			{
				_scoreText.text = _score.ToString();
			}
		}
	}
}