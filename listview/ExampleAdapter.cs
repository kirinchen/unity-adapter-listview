using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using surfm.listview;

public class ExampleAdapter : BaseAdapter {

    public RectTransform prefab;
    private List<string> textList = new List<string>();
    public int plusCount = 1;


    void Awake() {
        for (int i = 0; i < 5; i++) {
            textList.Add("AA_" + i);
        }

    }

    public void minus() {
        for (int i = 0; i < plusCount; i++) {

            if (textList.Count > 0) {
                textList.RemoveAt(0);
            }
        }
        notifydatachanged();
    }

    public void plus() {
        for (int i = 0; i < plusCount; i++) {

            textList.Add("PLUS=" + Time.time);
        }
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
        text.text = textList[position] + " pos=" + position;
        return currentView;
    }
}
