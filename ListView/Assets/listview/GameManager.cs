using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private ListView listView;
    private ExampleAdapter adapter;

    void Awake() {
        listView = FindObjectOfType<ListView>();
        adapter = GetComponent<ExampleAdapter>();
    }

    void Start () {
        listView.setAdapter(adapter);
    }
	

}
