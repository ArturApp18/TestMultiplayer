using CodeBase.CameraLogic;
using UnityEngine;
using Photon.Pun;

namespace CodeBase.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _speed = 5f;

        private Rigidbody _rigidbody;
        private Vector3 _input;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>();
                if (_renderer == null)
                {
                    Debug.LogError($"Renderer not found on {gameObject.name} or its children. Please assign Renderer in Inspector.");
                    return;
                }
                Debug.Log($"Renderer auto-assigned from child: {_renderer.gameObject.name}");
            }

            Debug.Log($"Player initialized: {gameObject.name}, ViewID: {photonView.ViewID}, IsMine: {photonView.IsMine}, NickName: {PhotonNetwork.NickName}, Owner: {photonView.Owner?.NickName}");

            if (photonView.IsMine)
            {
                var cameraFollow = Camera.main?.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.SetTarget(transform);
                    Debug.Log($"Camera set to follow {gameObject.name} (ViewID: {photonView.ViewID}, NickName: {PhotonNetwork.NickName})");
                }
                else
                {
                    Debug.LogWarning("CameraFollow component not found on Main Camera.");
                }

                Color playerColor = GetPlayerColor();
                photonView.RPC("SetColor", RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isFocused) return;
#endif

            _input.x = Input.GetAxis("Horizontal");
            _input.z = Input.GetAxis("Vertical");
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine || _rigidbody == null) return;

            Vector3 movement = _input.normalized * _speed;
            _rigidbody.linearVelocity = new Vector3(movement.x, _rigidbody.linearVelocity.y, movement.z);

            // Если есть движение, разворачиваем героя
            if (movement != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            }
        }

        [PunRPC]
        private void SetColor(float r, float g, float b)
        {
            if (_renderer == null)
            {
                Debug.LogError($"Cannot set color: Renderer is null on {gameObject.name}. Please assign Renderer in Inspector.");
                return;
            }

            Color newColor = new Color(r, g, b);
            _renderer.material.color = newColor;
            Debug.Log($"Color set on {gameObject.name} (ViewID: {photonView.ViewID}): RGB({r}, {g}, {b})");
        }

        private Color GetPlayerColor()
        {
            Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
            int index = (photonView.ViewID / 1000 - 1) % colors.Length;
            return colors[index];
        }
    }
}
