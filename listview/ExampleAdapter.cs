using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ExampleAdapter : BaseAdapter {

    public RectTransform prefab;
    private List<string> textList = new List<string>();

    void Awake() {
        for (int i = 0; i < 5; i++) {
            textList.Add("AA_" + i);
        }

    }

    public void minus() {
        if (textList.Count > 0) {
            textList.RemoveAt(0);
        }
        notifydatachanged();
    }

    public void plus() {
        textList.Add("PLUS="+Time.time);
        notifydatachanged();
    }

    public override int getCount() {
        return textList.Count;
    }

    public override RectTransform getView(RectTransform currentView, int position) {
        if (currentView == null) {
            currentView = Instantiate(prefab);
        }
        Text text = currentView.GetComponent<Text>();
        text.text = textList[position];
        return currentView;
    }
}
