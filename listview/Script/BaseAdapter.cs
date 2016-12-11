using UnityEngine;
using System.Collections;
using System;

namespace surfm.listview {
    public abstract class BaseAdapter : MonoBehaviour {

        private ListView view;


        internal void setListView(ListView lv) {
            view = lv;
        }

        public abstract int getCount();

        public abstract RectTransform getView(RectTransform currentView, int position);

        public void notifydatachanged() {
            if (view != null) {
                view.changeData();
            }
        }

    }

}


