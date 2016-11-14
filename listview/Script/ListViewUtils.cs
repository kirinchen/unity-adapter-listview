using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace surfm.listview {
    public class ListViewUtils : MonoBehaviour {

        public static List<ItemBundle> listVisble(List<ItemBundle> items,float bottomY) {
            List<ItemBundle> ans = new List<ItemBundle>();
            foreach (ItemBundle ib in items) {
                if (ib.trans.gameObject.activeSelf) {
                    if (ib.getBottomCenter().y < 0 && ib.trans.anchoredPosition.y > bottomY) {
                        ans.Add(ib);
                    }
                }
            }
            ans.Sort((x, y) => { return x.position.CompareTo(y.position); });
            return ans;
        }

        public static float getLength(List<ItemBundle> items,int count) {
            if (items == null || items.Count <= 0) {
                return 0;
            }
            return items[0].getHeight() * count;
        }

    }
}
