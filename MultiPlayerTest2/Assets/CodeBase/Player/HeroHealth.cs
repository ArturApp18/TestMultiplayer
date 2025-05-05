using System;
using Photon.Pun;
using UnityEngine;

namespace CodeBase.Player
{
    public class HeroHealth : MonoBehaviour, IPunObservable
    {
        private PhotonView _photonView;
        [SerializeField] private float _currentHp;
        [SerializeField] private float _maxHp;
        [SerializeField] private HpBar _hpBar; // Ссылка на HP-бар
        public event Action HealthChanged;
        public event Action Died;

        public float Current
        {
            get => _currentHp;
            set
            {
                if (!_photonView.IsMine) return;

                if (value != _currentHp)
                {
                    _currentHp = Mathf.Clamp(value, 0, Max);
                    HealthChanged?.Invoke();
                    if (_currentHp <= 0)
                    {
                        _photonView.RPC("OnDeath", RpcTarget.All);
                    }
                    _photonView.RPC("SyncHealth", RpcTarget.Others, _currentHp);
                    UpdateHpBar();
                }
            }
        }

        public float Max
        {
            get => _maxHp;
            set
            {
                if (!_photonView.IsMine) return;

                _maxHp = value;
                HealthChanged?.Invoke();
                _photonView.RPC("SyncMaxHealth", RpcTarget.Others, _maxHp);
                UpdateHpBar();
            }
        }

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _maxHp = 100f;
            _currentHp = _maxHp;
            UpdateHpBar();
        }

        private void Start()
        {
            if (_hpBar == null)
            {
                Debug.LogWarning("HP Bar is not assigned in HeroHealth!");
            }
            // Подписываемся на событие изменения здоровья
            HealthChanged += UpdateHpBar;
        }

        public void TakeDamage(float damage)
        {
            if (!_photonView.IsMine) return;

            if (Current <= 0)
                return;

            Current -= damage;
        }

        public void Heal(float healPercentage)
        {
            if (!_photonView.IsMine) return;

            if (Current <= 0) return;

            float healAmount = Max * healPercentage / 100f;
            Current += healAmount;
        }

        [PunRPC]
        public void ApplyDamage(float damage)
        {
            print("ApplyDamage: " + damage);
            TakeDamage(damage);
        }

        [PunRPC]
        public void ApplyHeal(float healPercentage)
        {
            print("ApplyHeal: " + healPercentage);
            Heal(healPercentage);
        }

        [PunRPC]
        private void SyncHealth(float currentHP)
        {
            _currentHp = currentHP;
            HealthChanged?.Invoke();
            UpdateHpBar(); // Обновляем HP-бар на других клиентах
        }

        [PunRPC]
        private void SyncMaxHealth(float maxHP)
        {
            _maxHp = maxHP;
            HealthChanged?.Invoke();
            UpdateHpBar(); // Обновляем HP-бар на других клиентах
        }

        [PunRPC]
        private void OnDeath()
        {
            Died?.Invoke();
            if (_photonView.IsMine)
            {
                gameObject.GetComponent<Collider>().enabled = false;
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_currentHp);
                stream.SendNext(_maxHp);
            }
            else
            {
                _currentHp = (float)stream.ReceiveNext();
                _maxHp = (float)stream.ReceiveNext();
                HealthChanged?.Invoke();
                UpdateHpBar(); // Обновляем HP-бар при получении данных
            }
        }

        private void UpdateHpBar()
        {
            if (_hpBar != null)
            {
                _hpBar.SetValue(Current, Max);
            }
        }
    }
}