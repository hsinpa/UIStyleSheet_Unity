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

            ExecuteFirstState(UIStyleStruct.Trigger.Hover);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            Debug.Log("OnPointerExit");

            ExecuteFirstState(UIStyleStruct.Trigger.Idle);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Debug.Log("OnPress");
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Debug.Log("OnRelease");
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            this.onClick.Invoke();
            Debug.Log("OnPointerClick");
        }
        #endregion


        private void ExecuteStateStruct(UIStyleStruct.StateStruct stateStruct)
        {
            int compositionLens = stateStruct.compositions.Count;

            for (int i = 0; i < compositionLens; i++) {

                if (stateStruct.compositions[i].target == null)
                    continue;

                stateStruct.compositions[i].target.color = stateStruct.compositions[i].styles.color;
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
                ((Text)target).fontSize = styleStruct.size;
                ((Text)target).font = styleStruct.font;
                return;
            }

            if (target.GetType() == typeof(TMPro.TextMeshProUGUI))
            {
                ((TMPro.TextMeshProUGUI)target).fontSize = styleStruct.size;
                return;
            }
        }

        private void ApplyStructOnImage(Graphic target, UIStyleStruct.StyleStruct styleStruct)
        {
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


