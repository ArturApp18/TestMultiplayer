using UnityEngine;

namespace SimulationGameCreator
{
    public class NoteScript : MonoBehaviour
    {
        [Multiline]
        public string noteText = "";
        [HideInInspector]
        public bool isReading = false;
        public MeshRenderer meshRenderer;

        void Start()
        {

        }

        public void Read()
        {
            if (isReading == false)
            {
                AdvancedGameManager.Instance.CurrentMode = Mode.ReadingNote;
                isReading = true;
                meshRenderer.enabled = false;
                GetComponent<Collider>().enabled = false;
                GameCanvas.Instance.Show_Note(noteText);
                GameCanvas.Instance.CurrentNote = this;
                GameCanvas.Instance.LastClickedArea = null;
            }
        }

        public void Unread()
        {
            Destroy(gameObject);
        }
    }
}