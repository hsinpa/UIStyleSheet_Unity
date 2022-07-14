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
        public List<UIStyleStruct.StateStruct> StateStructs => m_char_list[m_characteristic].stateStructs;

        [SerializeField]
        public List<UIStyleStruct.Characteristics> m_char_list = new List<UIStyleStruct.Characteristics>();
        //public List<List<UIStyleStruct.StateStruct>> StyleLists => _styleList;

        [SerializeField]
        private UIStylesheetSRP m_uiStylesheetSRP;

        [SerializeField, Range(1, 3)]
        private int m_styleLength = 1;
        private int m_characteristic = 0;
        public int CurrentCharacteristic => m_characteristic;

        public int StyleLength => m_styleLength;

        public new bool interactable { get => base.interactable;
            set {
                base.interactable = value;

                if (!value)
                    ExecuteFirstState(UIStyleStruct.Trigger.Disabled);
                else
                    FilterPostUIState();
            }
        }
        private byte[] byteState = new byte[8];

        public bool SetCharacteristic(int index) {
            
            if (index >= 0 && index < m_styleLength) {

                m_characteristic = index;
                FilterPostUIState();
                return true;
            }

            return false;
        }

        #region Private Implementation
        private void FilterPostUIState()
        {
            if (!interactable) return;

            var ui_state = System.BitConverter.ToInt16(byteState, 0);


            switch (ui_state)
            {
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

        private void ExecuteStateStruct(UIStyleStruct.Trigger trigger, UIStyleStruct.StateStruct stateStruct)
        {
            int compositionLens = stateStruct.compositions.Count;
            for (int i = 0; i < compositionLens; i++)
            {

                if (stateStruct.compositions[i].target == null)
                    continue;

                Color interaction_color = m_uiStylesheetSRP.FilterColorByTrigger(trigger);
                stateStruct.compositions[i].target.color = interaction_color * stateStruct.compositions[i].styles.color;
                stateStruct.compositions[i].target.rectTransform.rotation = Quaternion.Euler(0, 0, stateStruct.compositions[i].styles.rotation);

                stateStruct.compositions[i].target.rectTransform.localScale = new Vector3(stateStruct.compositions[i].styles.scale,
                                                                                            stateStruct.compositions[i].styles.scale,
                                                                                            stateStruct.compositions[i].styles.scale);

                if (stateStruct.compositions[i].target.GetType() == typeof(Text) ||
                    stateStruct.compositions[i].target.GetType() == typeof(TMPro.TextMeshProUGUI))
                {
                    ApplyStructOnText(stateStruct.compositions[i].target, stateStruct.compositions[i].styles);
                    continue;
                }

                if (stateStruct.compositions[i].target.GetType() == typeof(Image) ||
                    stateStruct.compositions[i].target.GetType() == typeof(RawImage))
                {
                    ApplyStructOnImage(stateStruct.compositions[i].target, stateStruct.compositions[i].styles);
                    continue;
                }
            }
        }

        private void ApplyStructOnText(Graphic target, UIStyleStruct.StyleStruct styleStruct)
        {
            if (target.GetType() == typeof(Text))
            {

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

        private void ExecuteFirstState(UIStyleStruct.Trigger trigger)
        {
            UIStyleStruct.StateStruct findStruct = FindFirstState(trigger);

            if (findStruct != null)
                ExecuteStateStruct(trigger, findStruct);
        }

        private UIStyleStruct.StateStruct FindFirstState(UIStyleStruct.Trigger trigger)
        {
            if (m_char_list == null || m_char_list.Count <= 0) return null;

            UIStyleStruct.StateStruct findStruct = StateStructs.Find(x => x.state == trigger);

            if (findStruct == null)
            {

                if (UIStyleStruct.TRIGGER_TABLE.TryGetValue(trigger, out var triggerConfig))
                {
                    if (triggerConfig.fallback != UIStyleStruct.Trigger.None)
                        return FindFirstState(triggerConfig.fallback);
                    return null;
                }
            }

            return findStruct;
        }
        #endregion

        #region Monobehavior and UI Event
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            //Debug.Log("OnPointerEnter");
            byteState[0] = 1;
            FilterPostUIState();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            byteState[0] = 0;
            FilterPostUIState();
            //Debug.Log("OnPointerExit");
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            byteState[1] = 1;
            FilterPostUIState();

            //Debug.Log("OnPress");
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            byteState[1] = 0;
            FilterPostUIState();
            //Debug.Log("OnRelease");
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (interactable) return;
            this.onClick.Invoke();
            //Debug.Log("OnPointerClick");
        }

        public new void Start()
        {
            base.Start();

            if (!interactable)
                ExecuteFirstState(UIStyleStruct.Trigger.Disabled);
            else
                ExecuteFirstState(UIStyleStruct.Trigger.Idle);
        }
        #endregion
    }
}


