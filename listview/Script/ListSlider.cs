using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace surfm.listview {
    [RequireComponent(typeof(Slider))]
    public class ListSlider : MonoBehaviour {

        private Slider slider;
        public ListView list;
        private float valueRange;
        private float lastRate;
        private bool _onMouseDown;


        void Start() {
            injectEvets(gameObject);
            slider = GetComponent<Slider>();
            list.onViewPlused = onReflesh;
            list.onDataChanged = onReflesh;
            valueRange = slider.maxValue - slider.minValue;
            onReflesh();
        }

        private void onReflesh() {
            if (list.isScollble()) {
                gameObject.SetActive(true);
                syncSliderValue();
            } else {
                gameObject.SetActive(false);
            }

        }

        private void injectEvets(GameObject go) {
            EventTrigger eventTrigger = go.AddComponent<EventTrigger>();
            addEventTrigger(eventTrigger, onMouseDown, EventTriggerType.PointerDown);
            addEventTrigger(eventTrigger, onMouseUp, EventTriggerType.PointerUp);

        }

        private void onMouseUp(BaseEventData arg0) {
            onValueChanged();
            list.plusY(getTotalH()/10, false);
            list.plusY(-getTotalH()/10, false);
            _onMouseDown = false;
        }

        private void onMouseDown(BaseEventData arg0) {
            _onMouseDown = true;
        }

        private void addEventTrigger(EventTrigger eventTrigger, UnityAction<BaseEventData> action, EventTriggerType triggerType) {
            // Create a nee TriggerEvent and add a listener
            EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
            trigger.AddListener(action); // you can capture and pass the event data to the listener

            // Create and initialise EventTrigger.Entry using the created TriggerEvent
            EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

            // Add the EventTrigger.Entry to delegates list on the EventTrigger
            eventTrigger.triggers.Add(entry);
        }

        private void syncSliderValue() {
            lastRate = getListRate();
            slider.value = slider.minValue + (valueRange * lastRate);
        }

        private float getListRate() {
            List<ItemBundle> ibs = list.listVisble();
            if (ibs.Count <= 0) {
                return 0;
            }
            return ibs[0].getRealY() <= 0 ? 0 : ibs[0].getRealY() / getTotalH();
        }

        private float getTotalH() {
            return list.getTotalHeight() - list.getContainerHeight();
        }

        public void onSlideValueChanged() {
            Debug.Log("onSlideValueChanged");
            onValueChanged();
        }

        private void onValueChanged() {
            if (_onMouseDown) {
                float totalH = getTotalH();
                float vRate = slider.value / valueRange;
                float cR = getListRate();
                float dR = vRate - cR;
                if (Mathf.Abs(dR) > 0.07f) {
                    float dh = dR * totalH;
                    list.plusY(dh, false);
                }
            }
        }
    }
}