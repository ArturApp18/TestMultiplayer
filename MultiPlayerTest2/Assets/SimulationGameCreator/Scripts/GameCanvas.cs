using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

namespace SimulationGameCreator
{
    public class GameCanvas : MonoBehaviour
    {
        [SerializeField] private FirstPersonController _firstPersonController;
        public static GameCanvas Instance;
        public Image image_Blinking;
        public Image Button_Build;
        public Image Button_Rotate;
        public Sprite Sprite_Touch;
        public GameObject Panel_WarningPanel;
        public LayerMask layerMaskForInteract;
        public GameObject Panel_GameUI;
        public GameObject Panel_Pause;
        public GameObject Panel_Settings;
        public GameObject Panel_Inventory;
        public GameObject Panel_Note;
        public GameObject Panel_Note_Text;
        public GameObject Panel_GameOver;
        public Image Image_Sprite_Blood;
        public Image Image_Health;
        public Image Image_Stamina;
        public Image Image_Hungriness;
        public GameObject Panel_Hungriness;
        public Image Image_Thirst;
        public GameObject Panel_Thirst;
        public Text Text_Info;
        [HideInInspector]
        public GameObject LastClickedArea;
        [HideInInspector]
        public NoteScript CurrentNote;
        private bool isGameOver = false;
        [HideInInspector]
        public bool isPaused = false;
        public GameObject Crosshair;
        public GameObject Panel_FoodDrink;
        public GameObject Container_FoodDrink;
        public GameObject Item_FoodDrink;

        public Text Text_Money;
        public Text Text_Experience;

        public Text Text_RepairHint;
        public Text Text_GrabHint;

        public GameObject[] Panels_Mobile;
        public GameObject Button_Interact;
        public Text Text_Map;


        private void Awake()
        {
            Instance = this;
        }

        public void Click_BacktoMenu()
        {
            Application.Quit();
        }

        public void UpdateStatus()
        {
            AdvancedGameManager.Instance.UpdatePlayerValues();
            Text_Money.text = AdvancedGameManager.Instance.playerValues.Money.ToString();
            Text_Experience.text = AdvancedGameManager.Instance.playerValues.Experience.ToString();
        }

        public void Show_GrabbingHint()
        {
            if(AdvancedGameManager.Instance.controllerType == ControllerType.PC)
            {
                Text_GrabHint.text = AdvancedGameManager.Instance.GrabbingKey.ToString() + " TO GRAB";
            }
            else
            {
                Text_GrabHint.text = "GRAB";
            }
            Text_GrabHint.gameObject.SetActive(true);
        }

        public void Hide_GrabbingHint()
        {
            Text_GrabHint.gameObject.SetActive(false);
        }

        float lastTabClick = 0;

         public void Click_Tab()
        {
            if(!isPaused && Time.time > lastTabClick + 0.25f && !isGameOver &&  (AdvancedGameManager.Instance.CurrentMode == Mode.Free || AdvancedGameManager.Instance.CurrentMode == Mode.InMechanicSelecting) && AdvancedGameManager.Instance.CameraMain.activeSelf)
            {
                lastTabClick = Time.time;
                MechanicSelectionManager.Instance.Toogle_Panel_MechanicSelection();
            }
        }
        public void Show_Panel_SellerShop()
        {
            Panel_Inventory.SetActive(true);
            AdvancedGameManager.Instance.CurrentMode = Mode.InSellerList;
            _firstPersonController.enabled = false;
            InventoryManager.Instance.LoadAllObjects();
            isPaused = true;
            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            CameraScript.Instance.enabled = false;
        }
        public void Hide_Panel_SellerShop()
        {
            Panel_Inventory.SetActive(false);
            AdvancedGameManager.Instance.CurrentMode = Mode.Free;
            _firstPersonController.enabled = true;
            HeroPlayerScript.Instance.ActivatePlayer();
            CameraScript.Instance.enabled = true;
            isPaused = false;
            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        public void Show_Blood_Effect()
        {
            Image_Sprite_Blood.gameObject.SetActive(true);
            Image_Sprite_Blood.GetComponent<Animation>().Play("BloodEffect");
            StartCoroutine(HideEffect());
        }

        public void Blink()
        {
            image_Blinking.GetComponent<Animation>().Play();
        }

        IEnumerator HideEffect()
        {
            yield return new WaitForSeconds(1);
            Image_Sprite_Blood.gameObject.SetActive(false);
        }


        public void Click_ButtonPause()
        {
            if (isPaused)
            {
                Click_Continue();
            }
            else
            {
                Click_Pause();
            }
        }


        public void Click_Continue()
        {
            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            HeroPlayerScript.Instance.ActivatePlayerInputs();
            HeroPlayerScript.Instance.ActivatePlayer();
            CameraScript.Instance.enabled = true;
            Time.timeScale = 1;
            isPaused = false;
            Panel_Pause.SetActive(false);
            Panel_Settings.SetActive(false);
            Panel_GameUI.SetActive(true);
        }

        public void ShowHint(string text)
        {
            StartCoroutine(ShowHintInTime(text));
        }

        IEnumerator ShowHintInTime(string text)
        {
            yield return new WaitForSeconds(0.25f);
            GameCanvas.Instance.Show_WarningShort(text);
            yield return new WaitForSeconds(3);
            GameCanvas.Instance.Hide_Warning();
        }

        public void Show_Inventory()
        {
            GameCanvas.Instance.Panel_Inventory.SetActive(true);
            InventoryManager.Instance.LoadMyInventory();
        }

        public void Hide_Inventory()
        {
            GameCanvas.Instance.Panel_Inventory.SetActive(false);
        }

        public void Click_Pause()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            HeroPlayerScript.Instance.DeactivatePlayer();
            CameraScript.Instance.enabled = false;
            Time.timeScale = 0;
            isPaused = true;
            Panel_Pause.SetActive(true);
            Panel_GameUI.SetActive(false);
        }

        public void UpdateHealth()
        {
            Image_Health.fillAmount = (HeroPlayerScript.Instance.Health / HeroPlayerScript.Instance.TotalHealth);
        }

        public void UpdateStamina()
        {
            Image_Stamina.fillAmount = (HeroPlayerScript.Instance.Stamina / HeroPlayerScript.Instance.TotalStamina);
        }

        public void UpdateHungerUI()
        {
            Image_Hungriness.fillAmount = (HeroPlayerScript.Instance.Fullness / HeroPlayerScript.Instance.TotalFullness);
        }

        public void UpdateThirstUI()
        {
            Image_Thirst.fillAmount = (HeroPlayerScript.Instance.Hydration / HeroPlayerScript.Instance.TotalHydration);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
            {
                if (AdvancedGameManager.Instance.CurrentMode == Mode.InInventoryLocating)
                {
                    InventoryManager.Instance.CancelLocating();
                    return;
                }

                if (AdvancedGameManager.Instance.CurrentMode == Mode.InMechanicSelecting)
                {
                    MechanicSelectionManager.Instance.Toogle_Panel_MechanicSelection();
                    return;
                }

                if (AdvancedGameManager.Instance.CurrentMode == Mode.InSellerList)
                {
                    Hide_Panel_SellerShop();
                    return;
                }

                if (AdvancedGameManager.Instance.CurrentMode == Mode.ReadingNote)
                {
                    Hide_Note();
                    return;
                }

                if (MapPanel.activeSelf)
                {
                    MapPanel.SetActive(false);
                    MapPanelCamera.SetActive(false);
                    RenderSettings.fog = true;
                    HeroPlayerScript.Instance.ActivatePlayer();
                    CameraScript.Instance.enabled = true;
                    return;
                }


                if (isPaused)
                {
                    Click_Continue();
                }
                else
                {
                    Click_Pause();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && !isGameOver)
            {
                MechanicSelectionManager.Instance.Toogle_Panel_MechanicSelection();
            }
            else if (Input.GetKeyUp(AdvancedGameManager.Instance.InteractingKey) && Panel_Note.activeSelf)
            {
                GameCanvas.Instance.Hide_Note();
            }
            else if (Input.GetKeyUp(KeyCode.M))
            {
                ToogleMap();
            }
        }

        public void ToogleMap()
        {
            if (MapPanel.activeSelf)
            {
                MapPanel.SetActive(false);
                MapPanelCamera.SetActive(false);
                if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                RenderSettings.fog = true;
                HeroPlayerScript.Instance.ActivatePlayer();
                CameraScript.Instance.enabled = true;
            }
            else
            {
                MapPanel.SetActive(true);
                if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
                MapPanelCamera.SetActive(true);
                RenderSettings.fog = false;
                HeroPlayerScript.Instance.DeactivatePlayer();
                CameraScript.Instance.enabled = false;
            }
        }

        public GameObject MapPanel;
        public GameObject MapPanelCamera;

        public void Hide_RepareHint()
        {
            Text_RepairHint.gameObject.SetActive(false);
        }

        public void Show_RepareHint()
        {
            if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
            {
                Text_RepairHint.text = AdvancedGameManager.Instance.RepairingKey.ToString() + " TO REPAIR";
            }
            else
            {
                Text_RepairHint.text = "REPAIR";
            }
            Text_RepairHint.gameObject.SetActive(true);
        }

        public void Show_GameOverPanel()
        {
            CameraScript.Instance.enabled = false;
            isGameOver = true;
            StartCoroutine(WaitAndShowGameOver(4));
        }

        IEnumerator WaitAndShowGameOver(int time)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            CameraScript.Instance.enabled = false;
            yield return new WaitForSeconds(time);
            Time.timeScale = 0;
            Panel_GameUI.SetActive(false);
            Panel_GameOver.SetActive(true);
        }

        public void Click_Settings()
        {
            Panel_Settings.SetActive(true);
            Panel_Pause.SetActive(false);
        }

        public void Click_Close_Settings()
        {
            Panel_Settings.SetActive(false);
            Panel_Pause.SetActive(true);
        }

        public void Click_ShowNote()
        {
            Panel_GameUI.SetActive(false);
            HeroPlayerScript.Instance.DeactivatePlayer();
        }

        private void Start()
        {
            UpdateStatus();
            Text_RepairHint.gameObject.SetActive(false);
            Text_GrabHint.gameObject.SetActive(false);
            if (AdvancedGameManager.Instance.controllerType == ControllerType.Mobile)
            {
                Text_Map.text = "Map";
                for (int i = 0; i < Panels_Mobile.Length; i++)
                {
                    Panels_Mobile[i].SetActive(true);
                }
                Button_Build.sprite = Sprite_Touch;
                Button_Rotate.sprite = Sprite_Touch;
            }
            else
            {
                Text_Map.text = "Map (M)";
                for (int i = 0; i < Panels_Mobile.Length; i++)
                {
                    Panels_Mobile[i].SetActive(false);
                }
            }
            Panel_Thirst.SetActive(AdvancedGameManager.Instance.ThirstActive);
            Panel_Hungriness.SetActive(AdvancedGameManager.Instance.HungrinessActive);
            if (!AdvancedGameManager.Instance.ThirstActive && !AdvancedGameManager.Instance.HungrinessActive)
            {
                GameCanvas.Instance.Panel_FoodDrink.SetActive(false);
            }
        }


        public void Click_Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Show_Note(string text)
        {
            StartCoroutine(ShowNoteNow(text));
        }

        IEnumerator ShowNoteNow(string text)
        {
            yield return new WaitForSeconds(0.25f);
            Panel_GameUI.SetActive(false);
            HeroPlayerScript.Instance.DeactivatePlayer();
            Panel_Note.SetActive(true);
            Panel_Note_Text.GetComponent<Text>().text = text;
            AudioManager.Instance.Play_Note_Reading();
        }

        public void Hide_Note()
        {
            AdvancedGameManager.Instance.CurrentMode = Mode.Free;
            Panel_GameUI.SetActive(true);
            HeroPlayerScript.Instance.ActivatePlayerInputs();
            if (CurrentNote != null)
            {
                CurrentNote.Unread();
                Panel_Note.SetActive(false);
                Panel_Note_Text.GetComponent<Text>().text = "";
            }
            AudioManager.Instance.Play_Item_Close();
        }


        public void Show_GameUI()
        {
            Panel_GameUI.SetActive(true);
        }

        public void Hide_GameUI()
        {
            Panel_GameUI.SetActive(false);
        }

        public void Show_WarningShort(string message)
        {
            GameCanvas.Instance.Text_Info.text = message;
        }

        public void Show_Warning(ItemScript item, string message = "")
        {
            string text = "";
            text = item.Name + (item.GetComponent<PhysicalEquipmentDetails>() != null ? " | Health: " + item.GetComponent<PhysicalEquipmentDetails>().Health.ToString() : "") + "\n";
            if (string.IsNullOrEmpty(message))
            {
                if (item.interactionType != InteractionType.None)
                {
                    if (AdvancedGameManager.Instance.controllerType == ControllerType.PC)
                    {
                        text += "(" + AdvancedGameManager.Instance.InteractingKey.ToString() + ") " + item.interactionType.ToString();
                    }
                    else
                    {
                        text += item.interactionType.ToString();
                    }
                }
            }
            else
            {
                text = message;
            }
            if (AdvancedGameManager.Instance.controllerType == ControllerType.Mobile)
            {
                Button_Interact.SetActive(true);
            }
            GameCanvas.Instance.Text_Info.text = text;
        }

        public GameObject Panel_SpendGetNotification;
        public Text Text_SpendGetNotification;

        public void Show_SpendGet(int amount)
        {
            if (amount > 0)
            {
                Text_SpendGetNotification.color = Color.green;
                Text_SpendGetNotification.text = "+" + amount;
                Panel_SpendGetNotification.SetActive(true);
            }
            else if (amount < 0)
            {
                Text_SpendGetNotification.color = Color.red;
                Text_SpendGetNotification.text = "-" + amount;
                Panel_SpendGetNotification.SetActive(true);
            }
            Panel_SpendGetNotification.GetComponent<Animation>().Play();
        }

        public void Show_Warning_Not(String textID, bool isPositive)
        {
            ShowWarningPanel(textID, isPositive);
        }
        IEnumerator i;
        public Color color_positive;
        public Color color_negative;


        void ShowWarningPanel(String text, bool isPositive)
        {
            Panel_WarningPanel.SetActive(false);
            Panel_WarningPanel.SetActive(true);
            Panel_WarningPanel.GetComponentInChildren<Text>().text = text;
            if (isPositive)
            {
                Panel_WarningPanel.GetComponent<Image>().color = color_positive;
            }
            else
            {
                Panel_WarningPanel.GetComponent<Image>().color = color_negative;
            }
            if (i != null)
            {
                StopCoroutine(i);
            }
            i = CloseWarningNot();
            StartCoroutine(i);

        }

        IEnumerator CloseWarningNot()
        {
            yield return new WaitForSeconds(2f);
            Hide_Warning_Not();
        }

        public void Hide_Warning()
        {
            GameCanvas.Instance.Text_Info.text = "";
            if(AdvancedGameManager.Instance.controllerType == ControllerType.Mobile)
            {
                if(!HeroPlayerScript.Instance.isHoldingBox)
                {
                    Button_Interact.SetActive(false);
                    HeroPlayerScript.Instance.isButtonInteractHeld = false;
                }
            }
        }

        public void Hide_Warning_Not()
        {
            Panel_WarningPanel.SetActive(false);
            Panel_WarningPanel.GetComponentInChildren<Text>().text = "";
        }
    }
}