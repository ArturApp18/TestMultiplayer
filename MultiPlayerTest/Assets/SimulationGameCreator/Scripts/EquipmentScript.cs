using UnityEngine;
using UnityEngine.UI;

namespace SimulationGameCreator
{
    public class EquipmentScript : MonoBehaviour
    {
        [HideInInspector]
        public PhysicalEquipmentDetails EquipmentDetail;
        public Sprite ImageSpriteInventory;
        public Sprite ImageSpriteCoin;
        public Text Text_Money;
        public Text Text_Experience;
        public Image Image;
        public Text Text_Name;
        public Image Image_Money;
        public Image Image_Experience;
        [HideInInspector]
        public int CurrentInStorage = 0;

        public void Interact()
        {
            if (InventoryManager.Instance.inventoryMode == InventoryMode.SellerShopIsOpen)
            {
                InventoryManager.Instance.Buy(this);
            }
            else
            {
                InventoryManager.Instance.Use(this);
            }
        }

        public void AssignDetails(PhysicalEquipmentDetails details)
        {
            EquipmentDetail = details;
            OnEnable();
        }

        public void Activate()
        {
            CurrentInStorage = PlayerPrefs.GetInt(EquipmentDetail.Name, 0);
            if (CurrentInStorage > 0)
            {
                gameObject.SetActive(true);
            }
        }

        public void DecreaseItFromInventory()
        {
            CurrentInStorage = CurrentInStorage - 1;
            PlayerPrefs.SetInt(EquipmentDetail.Name, CurrentInStorage);
            PlayerPrefs.Save();
        }

        private void OnEnable()
        {
            if(EquipmentDetail != null)
            {
                Activate();
                if (InventoryManager.Instance.inventoryMode == InventoryMode.SellerShopIsOpen)
                {
                    Text_Money.text = EquipmentDetail.Price.ToString();
                    Text_Name.text = EquipmentDetail.Name.ToString();
                    Image_Money.sprite = ImageSpriteCoin;
                    Text_Experience.text = EquipmentDetail.RequiredExperience.ToString();
                    Image_Experience.gameObject.SetActive(true);
                    gameObject.SetActive(true);
                }
                else
                {
                    if (CurrentInStorage > 0)
                    {
                        gameObject.SetActive(true);
                        Text_Money.text = "You have " + CurrentInStorage.ToString();
                        Text_Name.text = EquipmentDetail.Name.ToString();
                        Text_Experience.text = "";
                        Image_Money.sprite = ImageSpriteInventory;
                        Image_Experience.gameObject.SetActive(false);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }

                }
                Image.sprite = EquipmentDetail.ImageSprite;
            }
        }
    }

    public enum EquipmentType
    {
        Decoration,
        Poster
    }
}