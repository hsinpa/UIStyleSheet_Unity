using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.UIStyle.Sample
{
    [RequireComponent(typeof(Button))]
    public class UIStyleSampleScript : MonoBehaviour
    {
        [SerializeField]
        private UIStylesheet uiStylesheet;

        public void Start()
        {
            Button uiButton = GetComponent<Button>();

            uiButton.onClick.AddListener(() => {
                uiStylesheet.SetCharacteristic(uiStylesheet.CurrentCharacteristic == 0 ? 1 : 0);
            });
        }
    }
}