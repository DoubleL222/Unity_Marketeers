using UnityEngine;
using UnityEngine.UI;

namespace UI.Generics
{
    [ExecuteInEditMode]
    public class MPSelectableObject : MonoBehaviour {

        [Header("Is Interactable?")]
        public bool interactable = true;

        [Header("Color Settings")]
        public Color normal_color;
        public Color selected_color;

        private Color current_color;
        private Color prev_color;
        private Image myImage;
        protected bool isSelected = false;
        // Update is called once per frame

        //THIS IS WHAT HAPPENS ON CLICK

        public virtual void Select()
        {
            if (!interactable)
                return;
            isSelected = true;
            SetNewColor(selected_color);
        }

        public virtual void Deselect()
        {
            if (!interactable)
                return;
            isSelected = false;
            SetNewColor(normal_color);
        }

        public void SetNewColor(Color selColor)
        {
            if (!interactable)
                return;
            prev_color = current_color; // no need to check for null, since a Color is a struct, and cannot be null.
            current_color = selColor;
            myImage.color = current_color;
        }

        protected void SetPrevColor()
        {
            if (!interactable)
                return;
            SetNewColor(isSelected ? selected_color : normal_color);
        }

        public virtual void Update()
        {

        }

        public virtual void Awake()
        {
            myImage = GetComponent<Image>();
            Deselect();
        }

    }
}
