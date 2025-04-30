using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace SimulationGameCreator
{
    public class AdvancedGameManager : MonoBehaviour
    {
        [SerializeField] private FirstPersonController _firstPersonController;
        [SerializeField] private PhotonView _photonView;
        public GameObject CameraMain;
        public static AdvancedGameManager Instance;
        public PlayerValues playerValues;
        public ControllerType controllerType;

        public bool HungrinessActive;
        public bool ThirstActive;
        public float HungerSpeed = 1;
        public float ThirstSpeed = 2;

        [HideInInspector] public Mode CurrentMode = Mode.Free;

        public int StartingMoneyAmount = 1000;
        public int StartingExpAmount = 1;

        public KeyCode InteractingKey;
        public KeyCode RepairingKey;
        public KeyCode GrabbingKey;

        public void UpdatePlayerValues()
        {
            playerValues.Experience = PlayerPrefs.GetInt("Experience", StartingExpAmount);
            playerValues.Money = PlayerPrefs.GetInt("Money", StartingMoneyAmount);
            playerValues.Name = PlayerPrefs.GetString("ProfileName", "");
            playerValues.Health = HeroPlayerScript.Instance.Health;
        }

        public void Set_ProfileName(string Name)
        {
            PlayerPrefs.SetString("ProfileName", Name);
            PlayerPrefs.Save();
            UpdatePlayerValues();
        }

        private void Awake()
        {
            Instance = this;
            playerValues = new PlayerValues();
            UpdatePlayerValues();
            if (!ThirstActive) ThirstSpeed = 0;
            if (!HungrinessActive) HungerSpeed = 0;
        }

        public void Spend(int price)
        {
            int money = PlayerPrefs.GetInt("Money", StartingMoneyAmount);
            money = money - price;
            PlayerPrefs.SetInt("Money", money);
            PlayerPrefs.Save();
            GameCanvas.Instance.UpdateStatus();
            AudioManager.Instance.Play_audioClip_Coin();
            GameCanvas.Instance.Show_SpendGet(price * -1);
        }

        void Start()
        {
            if (_photonView.IsMine)
            {
                GameCanvas.Instance.Crosshair.SetActive(true);
                if (controllerType == ControllerType.Mobile)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                DailyStart();
                InventoryManager.Instance.LoadBuiltObjects();
                Application.targetFrameRate = 60;
            }
        }


        public void DailyStart()
        {
            CameraMain.SetActive(true);
            GameCanvas.Instance.image_Blinking.gameObject.SetActive(true);
            GameCanvas.Instance.image_Blinking.GetComponent<Animation>().Play();
            _firstPersonController.enabled = true;
            CameraScript.Instance.enabled = true;
        }

        public void Get(CollactableType type, int amount)
        {
            if (type == CollactableType.Money)
            {
                int money = PlayerPrefs.GetInt("Money", StartingMoneyAmount);
                money = money + amount;
                PlayerPrefs.SetInt("Money", money);
                PlayerPrefs.Save();
                AudioManager.Instance.Play_audioClip_Coin();
                GameCanvas.Instance.UpdateStatus();
                GameCanvas.Instance.Show_SpendGet(amount);
            }
            else
            {
                int exp = PlayerPrefs.GetInt("Experience", StartingExpAmount);
                exp = exp + amount;
                PlayerPrefs.SetInt("Experience", exp);
                PlayerPrefs.Save();
                AudioManager.Instance.Play_audioClip_Mana();
                GameCanvas.Instance.UpdateStatus();
            }
        }
    }

    public enum CollactableType
    {
        Money,
        Experience
    }

    public class PlayerValues
    {
        public float Health;
        public int Money;
        public int Experience;
        public string Name;
    }

    public enum Mode
    {
        Free,
        InInventoryLocating,
        InMechanicSelecting,
        ReadingNote,
        InSellerList
    }

    public enum ControllerType
    {
        PC,
        Mobile
    }
}