using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace CodeBase.Network
{
    public class VoiceChatDistanceWithUI : MonoBehaviourPunCallbacks
    {
        private PhotonVoiceView _voiceView;
        private AudioSource _audioSource;

        [SerializeField] private Image _speakingIndicator;
        private bool _isSpeaking = false;

        [SerializeField] private float _maxHearDistance = 20f;
        [SerializeField] private float _fullVolumeDistance = 10f;

        private void Start()
        {
            _voiceView = GetComponent<PhotonVoiceView>();
            _audioSource = GetComponent<AudioSource>();

            _audioSource.spatialBlend = 1f;
            _audioSource.minDistance = 1f;
            _audioSource.maxDistance = _maxHearDistance;

            if (_speakingIndicator != null)
                _speakingIndicator.enabled = false;

            if (photonView.IsMine)
                _voiceView.RecorderInUse.Bitrate = 12000;
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                HandleLocalPlayerVoice();
            }
            else
            {
                AdjustAudioVolumeForRemotePlayers();
            }

            UpdateSpeakingIndicator();
        }

        private void HandleLocalPlayerVoice()
        {
            bool newIsSpeaking = _voiceView.RecorderInUse.IsCurrentlyTransmitting;
            if (newIsSpeaking != _isSpeaking)
            {
                _isSpeaking = newIsSpeaking;
                Hashtable props = new Hashtable { { "isSpeaking", _isSpeaking } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            }

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            bool shouldTransmit = false;

            foreach (GameObject player in players)
            {
                if (player != gameObject)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance <= _maxHearDistance)
                    {
                        shouldTransmit = true;
                        break;
                    }
                }
            }

            _voiceView.RecorderInUse.TransmitEnabled = shouldTransmit;
        }

        private void AdjustAudioVolumeForRemotePlayers()
        {
            GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
            if (localPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, localPlayer.transform.position);

                if (distance <= _fullVolumeDistance)
                    _audioSource.volume = 1f;
                else if (distance <= _maxHearDistance)
                    _audioSource.volume = 1f - ((distance - _fullVolumeDistance) / (_maxHearDistance - _fullVolumeDistance));
                else
                    _audioSource.volume = 0f;
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer == photonView.Owner && changedProps.ContainsKey("isSpeaking"))
            {
                _isSpeaking = (bool)changedProps["isSpeaking"];
                UpdateSpeakingIndicator();
            }
        }

        private void UpdateSpeakingIndicator()
        {
            if (_speakingIndicator != null)
            {
                _speakingIndicator.enabled = _isSpeaking;

                if (_isSpeaking && Camera.main != null)
                {
                    _speakingIndicator.transform.LookAt(Camera.main.transform);
                    _speakingIndicator.transform.Rotate(0, 180, 0);
                }
            }
        }
    }
}