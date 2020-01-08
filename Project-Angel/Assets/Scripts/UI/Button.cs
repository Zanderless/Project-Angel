using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Wreckless.UI
{
    [DisallowMultipleComponent]
    public class Button : MonoBehaviour
    {

        public bool isSelected;
        public GameObject selectorIcon;
        public TextMeshProUGUI buttonTxt;
        
        public UnityEvent onSelect = new UnityEvent();

        public string ButtonText
        {
            set
            {
                buttonTxt.text = value;
            }
        }

        private void Update()
        {
            if(selectorIcon.activeSelf != isSelected)
                selectorIcon.SetActive(isSelected);
        }

        public void Select()
        {
            onSelect.Invoke();
        }

    }
}
