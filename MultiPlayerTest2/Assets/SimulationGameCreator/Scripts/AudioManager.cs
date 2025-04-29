using UnityEngine;

namespace SimulationGameCreator
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public AudioClip Door_Wooden_Open;
        public AudioClip Door_Close;
        public AudioClip Door_TryOpen;
        public AudioClip Door_UnLock;
        public AudioClip Item_Grab;
        public AudioClip Audio_Breathing;
        public AudioClip Note_Reading;
        public AudioClip Item_Close;
        public AudioClip Audio_Jump;
        public AudioClip Audio_Cabinet_Open;
        public AudioClip Audio_Drawer_Open;
        public AudioSource audioSource;
        public AudioSource audioSourceWalk;
        public AudioClip audioClip_ObjectiveCompleted;
        public AudioClip audioClip_ObjectiveAssigned;
        public AudioClip audioClip_Cleaning;
        public AudioClip audioClip_Reparing;
        public AudioClip audioClip_Building;
        public AudioClip[] audioClip_PaperCrease;

        public AudioClip audioClip_Coin;
        public AudioClip audioClip_Eat;
        public AudioClip audioClip_Drink;
        public AudioClip audioClip_Mana;
        public AudioSource AudioSource_Ambience;

        [Header("Press and Hold Sound Effects")]
        public AudioClip Audio_PressAndHoldMaintainDone;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play_Audio_Cleaning()
        {
            if(!audioSource.isPlaying)
            {
                audioSource.clip = audioClip_Cleaning;
                audioSource.Play();
            }
        }

        public void Stop_Audio_Cleaning()
        {
            audioSource.Stop();
        }

        public void Play_audioClip_PaperCrease()
        {
            audioSource.PlayOneShot(audioClip_PaperCrease[Random.Range(0, audioClip_PaperCrease.Length)]);
        }

        public void Play_audioClip_Mana()
        {
            audioSource.PlayOneShot(audioClip_Mana, 0.5f);
        }

        public void Play_audioClip_Eat()
        {
            audioSource.PlayOneShot(audioClip_Eat, 0.5f);
        }

        public void Play_audioClip_Drink()
        {
            audioSource.PlayOneShot(audioClip_Drink, 0.5f);
        }

        public void Play_audioClip_Coin()
        {
            audioSource.PlayOneShot(audioClip_Coin, 0.5f);
        }

        public void Play_Audio_Reparing()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioClip_Reparing;
                audioSource.Play();
            }
        }

        public void Stop_Audio_Reparing()
        {
            audioSource.Stop();
        }

        public void Play_Audio_Building()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioClip_Building;
                audioSource.Play();
            }
        }

        public void Stop_Audio_Building()
        {
            audioSource.Stop();
        }

        public void Play_ObjectiveCompleted()
        {
            audioSource.PlayOneShot(audioClip_ObjectiveCompleted);
        }

        public void Play_ObjectiveAssigned()
        {
            audioSource.PlayOneShot(audioClip_ObjectiveAssigned);
        }

        public void Play_Audio_PressAndHoldMaintainDone()
        {
            audioSource.PlayOneShot(Audio_PressAndHoldMaintainDone);
        }

        public void Play_Jump()
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(Audio_Jump);
        }

        public void Play_Audio_Cabinet_Open()
        {
            audioSource.PlayOneShot(Audio_Cabinet_Open);
        }

        public void Play_Audio_Drawer_Open()
        {
            audioSource.PlayOneShot(Audio_Drawer_Open);
        }

        public void Play_Door_Wooden_Open()
        {
            audioSource.PlayOneShot(Door_Wooden_Open);
        }

        public void Play_Audio_Breathing()
        {
            audioSource.PlayOneShot(Audio_Breathing);
        }

        public void Play_Door_Close()
        {
            audioSource.PlayOneShot(Door_Close);
        }

        public void Play_Note_Reading()
        {
            audioSource.PlayOneShot(Note_Reading);
        }

        public void Play_Item_Close()
        {
            audioSource.PlayOneShot(Item_Close);
        }

        public void Play_Door_UnLock()
        {
            audioSource.PlayOneShot(Door_UnLock);
        }

        public void Play_Item_Grab()
        {
            audioSource.PlayOneShot(Item_Grab);
        }

        public void Play_Door_TryOpen()
        {
            audioSource.PlayOneShot(Door_TryOpen);
        }
    }
}