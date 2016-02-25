using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ListEventController : MonoBehaviour {

    private ListView listView;
    private float startTime;
    private bool _down;
    public bool draged {
        get; private set;
    }
    private float lastY;
    private float startY;

    void Awake() {
        listView = GetComponent<ListView>();
        injectEvets(listView.gameObject);
    }

    private void injectEvets(GameObject go) {
        EventTrigger eventTrigger = go.AddComponent<EventTrigger>();
        addEventTrigger(eventTrigger, onMouseDown, EventTriggerType.PointerDown);
        addEventTrigger(eventTrigger, onMouseUp, EventTriggerType.PointerUp);
        addEventTrigger(eventTrigger, onMouseMove, EventTriggerType.Drag);
    }

    private void addEventTrigger(EventTrigger eventTrigger, UnityAction action, EventTriggerType triggerType) {
        // Create a nee TriggerEvent and add a listener
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((eventData) => action()); // you can capture and pass the event data to the listener

        // Create and initialise EventTrigger.Entry using the created TriggerEvent
        EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

        // Add the EventTrigger.Entry to delegates list on the EventTrigger
        eventTrigger.triggers.Add(entry);
    }

    public void onMouseDown() {
        _down = true;
        Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastY = v.y;
        startY = lastY;
        startTime = Time.time;
    }

    public void onMouseMove() {

        if (_down) {
            
            draged = true;
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            float d = v.y - lastY;
            plusY(d);
            lastY = v.y;
        }
    }

    public void onMouseUp() {
        _down = false;
        draged = false;
        setupInertia(Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        fixBundle();
    }

    private void setupInertia(float ly) {
        float d = ly - startY;
        float time = Time.time - startTime;
        float v = d / (time * 75);
        StartCoroutine(runInertia(v));
    }

    private IEnumerator runInertia(float v) {
        fixBundle();
        while (Math.Abs(v) > 0.01f) {
            yield return new WaitForSeconds(0.01f);
            plusY(v);
            v *= 0.9f;
        }
        fixBundle();
    }

    private void fixBundle() {
        //throw new NotImplementedException();
    }

    private void plusY(float d) {
        Vector3 v3 = listView.transform.worldToLocalMatrix.MultiplyVector(new Vector3(0,d,0));
        listView.plusY(v3.y);
    }

    internal void injectItemEvent(GameObject go) {
        injectEvets(go);
    }
}
