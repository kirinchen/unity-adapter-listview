using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace surfm.listview {
    public class InfoShower : MonoBehaviour {
        private Text text;
        public ListView list;


        void Start() {
            text = GetComponent<Text>();
        }

        void Update() {
            text.text = "Scrollble=" + list.isScollble() + " ended="+list.isEnded();
            List<ItemBundle> ibs = list.listVisble();
            string ss = "";
            foreach (ItemBundle ib in ibs) {
                ss += ib.position + " , ";
            }
            text.text += "\n" + ss;
        }
    }
}
