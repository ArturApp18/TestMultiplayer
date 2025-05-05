using UnityEngine;
using Photon.Pun;

namespace CodeBase.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviourPun
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _mouseSensitivity = 100f;
        [SerializeField] private float _gravity = -9.81f;

        private CharacterController _characterController;
        private Vector3 _input;
        private Vector3 _velocity;

        private Camera _playerCamera;
        private float _xRotation = 0f;
        private bool _isGrounded;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
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
                CreateLocalPlayerCamera();
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

            HandleInput();
            HandleMouseLook();
           // ApplyGravity();
            MovePlayer();
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

        private void HandleInput()
        {
            _input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            if (_playerCamera != null)
            {
                _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            }

            transform.Rotate(Vector3.up * mouseX);
        }

        private void ApplyGravity()
        {
            _isGrounded = _characterController.isGrounded; // Проверяем состояние на земле

            if (_isGrounded && _velocity.y < 0) 
            {
                _velocity.y = -2f; // Устанавливаем небольшой импульс вниз, чтобы поддерживать контакт с землёй
            }

            // Применяем гравитацию
            _velocity.y += _gravity * Time.deltaTime;
        }

        private void MovePlayer()
        {
            Vector3 move = transform.right * _input.x + transform.forward * _input.z;

            _characterController.Move(move * _speed * Time.deltaTime);

            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void CreateLocalPlayerCamera()
        {
            GameObject cameraObject = new GameObject("PlayerCamera");
            _playerCamera = cameraObject.AddComponent<Camera>();

            cameraObject.transform.SetParent(transform);
            cameraObject.transform.localPosition = new Vector3(-0.048f, 0.486f, 0.465f); 
            cameraObject.transform.localRotation = Quaternion.identity;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("Local Player Camera created.");
        }
    }
}