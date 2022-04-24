//Create and maintain by Hsinpa, @2022

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


    }
}
