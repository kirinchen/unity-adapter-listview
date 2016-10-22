﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ListView : MonoBehaviour {

    private static readonly float ITEM_BUFFER_HEIGHT_SCALE = 2f;
    private ListEventController eventController;
    private RectTransform rectTram;
    private BaseAdapter adapter;
    private ListViewUtils utils;
    private List<ItemBundle> items = new List<ItemBundle>();
    private int firstPos, lastPos;
    public Action<ItemBundle> onLockTop = (ItemBundle ib) => { };
    public Action<ItemBundle> onLockBottom = (ItemBundle ib) => { };

    void Awake() {
        firstPos = 0;
        eventController = GetComponent<ListEventController>();
        rectTram = GetComponent<RectTransform>();
        utils = gameObject.AddComponent<ListViewUtils>();
    }

    public void setAdapter(BaseAdapter ba) {
        firstPos = lastPos = 0;
        removeAllItems();
        adapter = ba;
        adapter.setListView(this);
        initViews();
    }

    private void removeAllItems() {
        Action<ItemBundle> iba = (ItemBundle ib) => {
            Destroy(ib.trans.gameObject);
        };
        forEachByItems(iba);
        items.Clear();
    }

    internal List<ItemBundle> getItems() {
        return items;
    }

    internal void initViews() {
        ItemBundle ib = null;
        while ((ib = getCurrentFeedItem()) != null) {
            ib.trans.anchoredPosition = new Vector3(ib.trans.anchoredPosition.x, -getContentHeight(), 0);
            items.Add(ib);
            eventController.injectItemEvent(ib.trans.gameObject);
        }
        forEachByItems(refleshFastOne);
    }



    private ItemBundle getCurrentFeedItem() {
        int i = items.Count;
        bool heightB = (getContainerHeight() * ITEM_BUFFER_HEIGHT_SCALE) > getContentHeight();
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
        if (adapter.getCount() <= 0) {
            return;
        }
        if (lockMoveToBottom(d) ) {
            onLockTop(getByPosition(firstPos));
            return;
        } else if (lockMoveToTop(d)) {
            onLockBottom(getByPosition(lastPos));
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
        bool heightB = y > -getContainerHeight();
        return d > 0 && (lastPos + 1) >= adapter.getCount() && y > -getContainerHeight();
    }

    internal void reflesh() {
        forEachByItems(refleshFastOne);
    }

    private void refleshFastOne(ItemBundle ib) {
        RectTransform rtf = adapter.getView(ib.trans, ib.position);
        ib.trans = rtf;
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
        float ans = getContentHeight() - getContainerHeight();
        return ans / 2;
    }

    private float getBottomLimtY() {
        float ans = getContentHeight() - getContainerHeight();
        ans /= 2;
        return -getContainerHeight() - ans;
    }

    private void forEachByItems(Action<ItemBundle> iba) {
        items.ForEach(iba);
    }

    private float getContentHeight() {
        float ans = 0f;
        foreach (ItemBundle ib in items) {
            ans += ib.trans.rect.height;
        }
        return ans;
    }

    private ItemBundle getByPosition(int pos) {
        return items.Find((ItemBundle ib) => { return ib.position == pos; });
    }

    private float getContainerHeight() {
        return rectTram.rect.height;
    }


}