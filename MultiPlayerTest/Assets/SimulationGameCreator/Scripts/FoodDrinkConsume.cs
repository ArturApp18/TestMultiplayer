using UnityEngine;
using UnityEngine.UI;

namespace SimulationGameCreator
{
    public class FoodDrinkConsume : MonoBehaviour
    {
        private string Name;
        public Image image;
        public Text text_name;
        public Text text_amount;

        public void Consume()
        {
            // Let's consume it!
            int amount = PlayerPrefs.GetInt("Storage_" + Name, 0);
            if (amount > 0)
            {
                amount = amount - 1;
                PlayerPrefs.SetInt("Storage_" + Name, amount);
                PlayerPrefs.Save();
                MechanicSelectionManager.Instance.Update_FoodDrinkList();
                // Use it
                for (int i = 0; i < InventoryManager.Instance.FoodAndDrinks.Count; i++)
                {
                    if (InventoryManager.Instance.FoodAndDrinks[i].Name == Name)
                    {
                        ItemScript currentItem = InventoryManager.Instance.FoodAndDrinks[i];
                        if (currentItem.GetComponent<FoodScript>() != null)
                        {
                            // This is a food!
                            HeroPlayerScript.Instance.Eat(currentItem.GetComponent<FoodScript>().FoodAmount);
                            AudioManager.Instance.Play_audioClip_Eat();
                        }
                        else if (currentItem.GetComponent<DrinkScript>() != null)
                        {
                            // This is a drink!
                            HeroPlayerScript.Instance.Drink(currentItem.GetComponent<DrinkScript>().DrinkAmount);
                            AudioManager.Instance.Play_audioClip_Drink();
                        }
                        break;
                    }
                }
            }
        }

        public void Assign(Sprite s, string t, int amount)
        {
            Name = t;
            image.sprite = s;
            text_name.text = t;
            text_amount.text = amount.ToString();
        }
    }
}
