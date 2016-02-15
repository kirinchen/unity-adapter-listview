using UnityEngine;
using System.Collections;

public class ItemBundle {

    public RectTransform trans;
    public int position;

    public Vector2 getBottomCenter() {
        float y =  trans.anchoredPosition.y - trans.rect.height;
        Vector2 ans = new Vector2(0, y);
        return ans;
    }

}
