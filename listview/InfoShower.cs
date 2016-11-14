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
            float totalH = list.getTotalHeight() - list.getContainerHeight();
            List<ItemBundle> ibs = list.listVisble();
            float firstY = ibs[0].getRealY();
            text.text = "Scrollble=" + list.isScollble() + " ended=" + list.isEnded() + " firstY=" + firstY;
            text.text += " totalH=" + totalH;
            string ss = "";
            foreach (ItemBundle ib in ibs) {
                ss += ib.position + " , ";
            }
            text.text += "\n" + ss;
        }
    }
}
