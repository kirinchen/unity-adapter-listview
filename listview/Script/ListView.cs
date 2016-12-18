using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace surfm.listview {
    public class ListView : MonoBehaviour {

        private static readonly float ITEM_BUFFER_HEIGHT_SCALE = 2f;
        private ListEventController eventController;
        private RectTransform rectTram;
        public BaseAdapter adapter { get; private set; }
        private ListViewUtils utils;
        private List<ItemBundle> items = new List<ItemBundle>();
        private int firstPos;
        private int lastPos;
        public Action<ItemBundle> onLockTop = (ItemBundle ib) => { };
        public Action<ItemBundle> onLockBottom = (ItemBundle ib) => { };
        public Action onViewPlused = () => { };
        public Action onDataChanged = () => { };

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
            changeData();
        }

        private void removeAllItems() {
            Action<ItemBundle> iba = (ItemBundle ib) => {
                Destroy(ib.trans.gameObject);
            };
            items.ForEach(iba);
            items.Clear();
        }

        internal List<ItemBundle> getItems() {
            return items;
        }

        internal void changeData() {
            ItemBundle ib = null;
            while ((ib = getCurrentFeedItem()) != null) {
                ib.trans.anchoredPosition = new Vector3(ib.trans.anchoredPosition.x, -getContentHeight(), 0);
                items.Add(ib);
                eventController.injectItemEvent(ib.trans.gameObject);
            }
            reflesh();
            fixedTop();
            onDataChanged();
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

        internal void plusY(float d, bool trigger = true) {
            if (isScollble()) {
                if (adapter.getCount() <= 0) {
                    return;
                }
                if (lockMoveToBottom(d)) {
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
                if (trigger) onViewPlused();
            }
        }

        private void shiftUp(float d) {
            for (int i = firstPos; i <= lastPos; i++) {
                ItemBundle beforeB = getByPosition(i - 1);
                ItemBundle ib = getByPosition(i);
                if (beforeB == null) {
                    Vector2 v = ib.trans.anchoredPosition + new Vector2(0, d);
                    if ((lastPos + 1) < adapter.getCount() && v.y > getTopLimtY()) {
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
                    if ((firstPos) > 0 && ib.getBottomCenter().y < getBottomLimtY()) {
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
            items.ForEach(refleshFastOne);
        }

        private void refleshFastOne(ItemBundle ib) {
            if (ib.position < adapter.getCount()) {
                if (!ib.trans.gameObject.activeSelf) {
                    ib.trans.gameObject.SetActive(true);
                }
                RectTransform rtf = adapter.getView(ib.trans, ib.position);
                ib.trans = rtf;
            } else {
                if (ib.trans.gameObject.activeSelf) {
                    ib.trans.gameObject.SetActive(false);
                }
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


        public float getTopLimtY() {
            float ans = getContentHeight() - getContainerHeight();
            return ans / 2;
        }

        public float getBottomLimtY() {
            float ans = getContentHeight() - getContainerHeight();
            ans /= 2;
            return -getContainerHeight() - ans;
        }

        internal float getContentHeight(bool skipUnActivted = false) {
            float ans = 0f;
            foreach (ItemBundle ib in items) {
                if (!skipUnActivted || ib.trans.gameObject.activeSelf) {
                    ans += ib.trans.rect.height;
                }
            }
            return ans;
        }

        private ItemBundle getByPosition(int pos) {
            return items.Find((ItemBundle ib) => { return ib.position == pos; });
        }

        internal float getContainerHeight() {
            return rectTram.rect.height;
        }


        public bool isScollble() {
            return getContentHeight(true) > getContainerHeight();
        }

        public bool isEnded() {
            ItemBundle ib = getByPosition(adapter.getCount() - 1);
            if (ib == null) {
                return false;
            }
            return ib.getBottomCenter().y > getBottomLimtY();
        }

        private void fixedTop() {
            if (isEnded()) {
                ItemBundle ib = getByPosition(0);
                if (ib == null) return;
                float y = ib.trans.anchoredPosition.y;
                if (y != 0) {
                    plusY(-y);
                }
            }
        }

        public float getTotalHeight() {
            if (adapter.getCount() <= 0) return 0;
            return ListViewUtils.getLength(items, adapter.getCount() - 1);
        }

        public List<ItemBundle> listVisble() {
            return ListViewUtils.listVisble(items, -getContainerHeight());
        }

    }
}
