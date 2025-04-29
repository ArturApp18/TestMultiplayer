using System.Collections;
using UnityEngine;

namespace SimulationGameCreator
{
    public class FPSHandRotator : MonoBehaviour
    {
        public float speed = 2.5f;
        public static FPSHandRotator Instance;

        public GameObject Hands_Parent;
        public GameObject Wrench;
        public GameObject Hammer;
        public GameObject Broom;

        public Hand_Type Current_HandType = Hand_Type.Free;

        private void Awake()
        {
            Instance = this;
        }

        public void Hide_HandParent()
        {
            Switch_Hand(Hand_Type.Free);
            Wrench.SetActive(false);
            Hammer.SetActive(false);
            Broom.SetActive(false);
            Hands_Parent.SetActive(false);
        }

        public void Show_HandParent()
        {
            Hands_Parent.SetActive(true);
        }

        public IEnumerator Switch_Hand_InTime(Hand_Type hand_Type, float second)
        {
            yield return new WaitForSeconds(second);
            Switch_Hand(hand_Type);
        }

        public void Switch_Hand(Hand_Type hand_Type)
        {
            if (Current_HandType != hand_Type)
            {
                Wrench.SetActive(false);
                Hammer.SetActive(false);
                Broom.SetActive(false);
                Current_HandType = hand_Type;
                switch (hand_Type)
                {
                    case Hand_Type.Build:
                        Hammer.SetActive(true);
                        break;
                    case Hand_Type.Clean:
                        Broom.SetActive(true);
                        break;
                    case Hand_Type.Free:
                        break;
                    case Hand_Type.Repair:
                        Wrench.SetActive(true);
                        break;
                }
            }
        }

        public void AnimateHand(InteractionType type)
        {
            switch (Current_HandType)
            {
                case Hand_Type.Build:
                    if (type == InteractionType.None || type == InteractionType.Build)
                    {
                        if (!Hammer.GetComponent<Animation>().isPlaying)
                        {
                            Hammer.GetComponent<Animation>().Play();
                        }
                        AudioManager.Instance.Play_Audio_Building();
                    }
                    break;
                case Hand_Type.Clean:
                    if (type == InteractionType.Clean)
                    {
                        if (!Broom.GetComponent<Animation>().isPlaying)
                        {
                            Broom.GetComponent<Animation>().Play();
                        }
                        AudioManager.Instance.Play_Audio_Cleaning();
                    }
                    break;
                case Hand_Type.Repair:
                    if (type == InteractionType.None || type == InteractionType.Repair)
                    {
                        if (!Wrench.GetComponent<Animation>().isPlaying)
                        {
                            Wrench.GetComponent<Animation>().Play();
                        }
                        AudioManager.Instance.Play_Audio_Reparing();
                    }
                    break;
            }
        }

    }

    public enum Hand_Type
    {
        Free,
        Repair,
        Build,
        Clean,
        Any
    }

}
