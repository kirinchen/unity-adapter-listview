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

    public bool isEnableSize() {
        return trans.localScale == new Vector3(1, 1, 1);
    }

    public bool isDisableSize() {
        return trans.localScale == Vector3.zero;
    }

    public bool isScaling() {
        return enableStatus == EnableStatus.Disabling || enableStatus == EnableStatus.Enabling;
    }

    public Vector3 getScaleTo() {
        if (enableStatus == EnableStatus.Enabling) {
            return new Vector3(1, 1, 1);
        } else if (enableStatus == EnableStatus.Disabling) {
            return Vector3.zero;
        } else {
            throw new System.Exception("Imposible status="+enableStatus);
        }
    }

    public Vector3 getCurrentScale() {
        return trans.localScale;
    }

    internal void setScaleFinshStatus() {
        trans.localScale = getScaleTo();
        if (enableStatus == EnableStatus.Disabling) {
            enableStatus = EnableStatus.Disabled;
        } else if (enableStatus == EnableStatus.Enabling) {
            enableStatus = EnableStatus.Enabled;
        }
    }
}
