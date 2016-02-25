using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ListView : MonoBehaviour {

    private static readonly float ITEM_BUFFER_HEIGHT_SCALE = 2f;
    private ListEventController eventController;
    private RectTransform rectTram;
    private BaseAdapter adapter;
    private List<ItemBundle> items = new List<ItemBundle>();
    private int firstPos, lastPos;

    void Awake() {
        firstPos = 0;
        eventController = GetComponent<ListEventController>();
        rectTram = GetComponent<RectTransform>();
    }

    public void setAdapter(BaseAdapter ba) {
        adapter = ba;
        adapter.setListView(this);
        initViews();
    }

    private void initViews() {
        ItemBundle ib = null;
        while ((ib = getCurrentFeedItem()) != null) {
            ib.trans.anchoredPosition = new Vector3(ib.trans.anchoredPosition.x, -getCurrentItemsHeight(), 0);
            items.Add(ib);
            eventController.injectItemEvent(ib.trans.gameObject);
        }
        forEachByItems(refleshFastOne);
    }

    private ItemBundle getCurrentFeedItem() {
        int i = items.Count;
        bool heightB = (rectTram.rect.height * ITEM_BUFFER_HEIGHT_SCALE) > getCurrentItemsHeight();
        bool countB = i < adapter.getCount();
        if (heightB && countB) {
            ItemBundle ans = new ItemBundle();
            ans.trans = adapter.getView(null, i);
            initItemTrans(ans.trans);
            ans.position = i;
            lastPos = i;
            return ans;
        } else {
            return null;
        }

    }

    private void initItemTrans(RectTransform trans) {
        trans.parent = rectTram;
        trans.localPosition = Vector3.zero;
        trans.pivot = new Vector2(0.5f, 1);
        trans.anchorMax = new Vector2(0.5f, 1);
        trans.anchorMin = new Vector2(0.5f, 1);
        trans.localScale = new Vector3(1, 1, 1);
    }

    internal void plusY(float d) {
        if (lockMoveToBottom(d) || lockMoveToTop(d)) {
            return;
        }
        if (d > 0) {
            shiftUp(d);
        } else {
            shiftDown(d);
        }
         reflesh();
    }

    private void shiftUp(float d) {
        for (int i = firstPos; i <= lastPos; i++) {
            ItemBundle beforeB = getByPosition(i - 1);
            ItemBundle ib = getByPosition(i);
            if (beforeB == null) {
                Vector2 v = ib.trans.anchoredPosition + new Vector2(0, d);
                if ((lastPos+1)<adapter.getCount() && v.y > getTopLimtY() ) {
                    firstPos++;
                    ib.position = ++lastPos;
                } else {
                    ib.trans.anchoredPosition += new Vector2(0, d);
                }
            } else {
                ib.trans.anchoredPosition = beforeB.getBottomCenter();
            }
        }
    }

    private void shiftDown(float d) {
        for (int i = lastPos; i >= firstPos; i--) {
            ItemBundle nextB = getByPosition(i + 1);
            ItemBundle ib = getByPosition(i);
            if (nextB == null) {
                if ((firstPos ) > 0 && ib.getBottomCenter().y < getBottomLimtY()) {
                    lastPos--;
                    ib.position = --firstPos;
                } else {
                    ib.trans.anchoredPosition += new Vector2(0, d);
                }
            } else {
                ib.trans.anchoredPosition = nextB.trans.anchoredPosition + new Vector2(0, ib.trans.rect.height);
            }
        }
    }



    private bool lockMoveToBottom(float d) {
        ItemBundle firstB = getByPosition(firstPos);
        if (firstB == null) {
            return false;
        }
        float topY = firstB.trans.anchoredPosition.y;
        return d < 0 && firstPos <= 0 && topY <= 0;
    }

    private bool lockMoveToTop(float d) {
        ItemBundle lastB = getByPosition(lastPos);
        if (lastB == null) {
            return false;
        }
        float y = lastB.getBottomCenter().y;
        bool db = d > 0;
        bool indexB = lastPos >= adapter.getCount();
        bool heightB = y > -rectTram.rect.height;
        return d > 0 && (lastPos + 1) >= adapter.getCount() && y > -rectTram.rect.height;
    }

    internal void reflesh() {
        forEachByItems(refleshOne);
    }

    private void refleshFastOne(ItemBundle ib) {
        RectTransform rtf = adapter.getView(ib.trans, ib.position);
        ib.trans = rtf;
        if (ib.trans.anchoredPosition.y > 0 || ib.getBottomCenter().y < -rectTram.rect.height * 1.1f) {
            enableFast(ib, false);
        } else {
            enableFast(ib, true);
        }
    }

    private void refleshOne(ItemBundle ib) {
        RectTransform rtf = adapter.getView(ib.trans, ib.position);
        ib.trans = rtf;
        if (ib.trans.anchoredPosition.y > 0 || ib.getBottomCenter().y < -rectTram.rect.height * 1.1f) {
            enable(ib, false);
        } else {
            enable(ib, true);
        }
    }

    private void enableFast(ItemBundle ib, bool b) {
        Vector3 v = b ? new Vector3(1,1,1) : Vector3.zero;
        ib.trans.localScale = v;
        ib.enableStatus = b ? ItemBundle.EnableStatus.Enabled : ItemBundle.EnableStatus.Disabled;
    }

    private void enable(ItemBundle ib, bool b) {
        ib.enableStatus = b ? ItemBundle.EnableStatus.Enabling: ItemBundle.EnableStatus.Disabling;
        StartCoroutine(runEnable(ib));
    }

    private IEnumerator runEnable(ItemBundle ib) {
        while (ib.isScaling()) {
            Vector3 toV = ib.getScaleTo();
            float d = Vector3.Distance(toV,ib.getCurrentScale());
            if (d < 0.005f) {
                ib.setScaleFinshStatus();
            } else {
                Vector3 shiftV = (toV - ib.getCurrentScale()) * 0.035f;
                ib.trans.localScale += shiftV;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void checkRecycle(ItemBundle ib) {
        ItemBundle targetIb;
        if (ib.trans.anchoredPosition.y > getTopLimtY() && (lastPos + 1) < adapter.getCount() && (targetIb = getByPosition(lastPos)) != null) {
            ib.trans.anchoredPosition = targetIb.getBottomCenter();
            ib.position = ++lastPos;
            firstPos++;
        } else if (ib.trans.anchoredPosition.y < getBottomLimtY() && firstPos > 0 && (targetIb = getByPosition(firstPos)) != null) {
            Vector2 v = targetIb.trans.anchoredPosition + new Vector2(0, ib.trans.rect.height);
            ib.trans.anchoredPosition = v;
            ib.position = --firstPos;
            lastPos--;
        }
    }


    private float getTopLimtY() {
        float ans = getCurrentItemsHeight() - rectTram.rect.height;
        return ans / 2;
    }

    private float getBottomLimtY() {
        float ans = getCurrentItemsHeight() - rectTram.rect.height;
        ans /= 2;
        return -rectTram.rect.height - ans;
    }

    private void forEachByItems(Action<ItemBundle> iba) {
        items.ForEach(iba);
    }

    private float getCurrentItemsHeight() {
        float ans = 0f;
        foreach (ItemBundle ib in items) {
            ans += ib.trans.rect.height;
        }
        return ans;
    }

    private ItemBundle getByPosition(int pos) {
        return items.Find((ItemBundle ib) => { return ib.position == pos; });
    }




}
