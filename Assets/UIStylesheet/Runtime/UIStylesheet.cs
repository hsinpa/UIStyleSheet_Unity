//Create and maintain by Hsinpa, @2022

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Hsinpa.UIStyle {
    public class UIStylesheet : Button 
    {
        
        [SerializeField]
        private List<UIStyleStruct.StateStruct> _stateStructs = new List<UIStyleStruct.StateStruct>();
        public List<UIStyleStruct.StateStruct> StateStructs => this._stateStructs;

        public new bool interactable { get => _interactable;
            set {
                _interactable = value;

                if (value)
                    ExecuteFirstState(UIStyleStruct.Trigger.Disabled);
                else
                    FilterPostUIState();
            }
        }
        private bool _interactable;

        private byte[] byteState = new byte[8];
        public void Start()
        {
            base.Start();
            ExecuteFirstState(UIStyleStruct.Trigger.Idle);
        }

        public void ExecuteCustomState(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            UIStyleStruct.StateStruct findStruct = _stateStructs.Find(x => x.state == UIStyleStruct.Trigger.Custom && x.id == id);

            if (findStruct == null) return;

            ExecuteStateStruct(findStruct);
        }

        private void ExecuteFirstState(UIStyleStruct.Trigger trigger) {
            UIStyleStruct.StateStruct findStruct = _stateStructs.Find(x => x.state == trigger);

            if (findStruct == null) return;

            ExecuteStateStruct(findStruct);
        }

        #region Unity UI Event
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            Debug.Log("OnPointerEnter");
            byteState[0] = 1;
            FilterPostUIState();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            byteState[0] = 0;
            FilterPostUIState();
            Debug.Log("OnPointerExit");
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            byteState[1] = 1;
            FilterPostUIState();

            Debug.Log("OnPress");
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            byteState[1] = 0;
            FilterPostUIState();
            Debug.Log("OnRelease");
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (interactable) return;
            this.onClick.Invoke();
            Debug.Log("OnPointerClick");
        }
        #endregion

        private void FilterPostUIState() {
            if (!interactable) return;

            var ui_state = System.BitConverter.ToInt16(byteState, 0);

            Debug.Log("FilterPostUIState " + ui_state );

            switch (ui_state) {
                case (int)UIStyleStatic.UIEventEnum.Idle:
                        ExecuteFirstState(UIStyleStruct.Trigger.Idle);
                    break;

                case (int)UIStyleStatic.UIEventEnum.Hover:
                    ExecuteFirstState(UIStyleStruct.Trigger.Hover);
                    break;

                case (int)UIStyleStatic.UIEventEnum.Pressed:
                    ExecuteFirstState(UIStyleStruct.Trigger.Pressed);
                    break;

                case (int)UIStyleStatic.UIEventEnum.Pressed_OutofBorder:
                    ExecuteFirstState(UIStyleStruct.Trigger.Pressed);
                    break;
            }
        }

        private void ExecuteStateStruct(UIStyleStruct.StateStruct stateStruct)
        {
            int compositionLens = stateStruct.compositions.Count;

            for (int i = 0; i < compositionLens; i++) {

                if (stateStruct.compositions[i].target == null)
                    continue;

                stateStruct.compositions[i].target.color =  stateStruct.compositions[i].styles.color;
                stateStruct.compositions[i].target.rectTransform.rotation = Quaternion.Euler(0, 0, stateStruct.compositions[i].styles.rotation);

                stateStruct.compositions[i].target.rectTransform.localScale =  new Vector3(stateStruct.compositions[i].styles.scale,
                                                                                            stateStruct.compositions[i].styles.scale,
                                                                                            stateStruct.compositions[i].styles.scale);

                if (stateStruct.compositions[i].target.GetType() == typeof(Text) ||
                    stateStruct.compositions[i].target.GetType() == typeof(TMPro.TextMeshProUGUI))
                {
                    ApplyStructOnText(stateStruct.compositions[i].target, stateStruct.compositions[i].styles);
                    continue;
                }

                if (stateStruct.compositions[i].target.GetType() == typeof(Image) ||
                    stateStruct.compositions[i].target.GetType() == typeof(RawImage)) {
                    ApplyStructOnImage(stateStruct.compositions[i].target, stateStruct.compositions[i].styles);
                    continue;
                }
            }
        }

        private void ApplyStructOnText(Graphic target, UIStyleStruct.StyleStruct styleStruct) {
            if (target.GetType() == typeof(Text)) {

                if (styleStruct.size > 0)
                    ((Text)target).fontSize = styleStruct.size;

                if (styleStruct.font != null)
                    ((Text)target).font = styleStruct.font;
                return;
            }

            if (target.GetType() == typeof(TMPro.TextMeshProUGUI))
            {
                if (styleStruct.size > 0)
                    ((TMPro.TextMeshProUGUI)target).fontSize = styleStruct.size;
                return;
            }
        }

        private void ApplyStructOnImage(Graphic target, UIStyleStruct.StyleStruct styleStruct)
        {
            if (styleStruct.sprite == null) return;

            if (target.GetType() == typeof(Image))
            {
                ((Image)target).sprite = styleStruct.sprite;
                return;
            }

            if (target.GetType() == typeof(RawImage))
            {
                ((RawImage)target).texture = styleStruct.sprite.texture;
                return;
            }
        }
    }
}


