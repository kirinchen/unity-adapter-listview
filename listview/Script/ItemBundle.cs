using UnityEngine;
using System.Collections;
using System;

public class ItemBundle {

    public enum EnableStatus {
        Enabling, Enabled , Disabling , Disabled
    }

    public RectTransform trans;
    public int position;
    public EnableStatus enableStatus {
        get; internal set;
    }

    public ItemBundle() {
        enableStatus = EnableStatus.Enabled;
    }

    public Vector2 getBottomCenter() {
        float y =  trans.anchoredPosition.y - trans.rect.height;
        Vector2 ans = new Vector2(0, y);
        return ans;
    }













}
