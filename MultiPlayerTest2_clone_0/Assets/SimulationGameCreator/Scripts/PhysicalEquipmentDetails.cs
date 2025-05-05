using UnityEngine;

namespace SimulationGameCreator
{
    public class PhysicalEquipmentDetails : MonoBehaviour
    {
        public int equipmentIndex = 0;
        public int Health;
        public int TotalHealth;
        public string Name;
        public ParticleSystem FaultParticle;
        public int Consumption = 4;
        public EquipmentType equipmentType;
        public int Price;
        public int RepairingPrice;
        public int RequiredExperience;
        public Sprite ImageSprite;

        [HideInInspector]
        public int CurrentInStorage = 0;

        public void LoadProfile()
        {
            Health = PlayerPrefs.GetInt(Name + equipmentIndex + "_Health", TotalHealth);
            if (FaultParticle == null) return;

            if (Health > 0)
            {
                FaultParticle.Stop();
            }
            else
            {
                FaultParticle.Play();
            }
        }

        public void Fix()
        {
            Health = TotalHealth;
            PlayerPrefs.SetInt(Name + equipmentIndex + "_Health", Health);
            PlayerPrefs.Save();
            Instantiate(InventoryManager.Instance.buildParticle, transform.position, Quaternion.identity);
            FaultParticle.Stop();
        }

        public void UseTheEquipment()
        {
            if (Health <= Consumption)
            {
                if (!FaultParticle.isPlaying)
                {
                    FaultParticle.Play();
                }
            }
            Health = Health - Consumption;
            PlayerPrefs.SetInt(Name + equipmentIndex + "_Health", Health);
            PlayerPrefs.Save();
        }
    }
}