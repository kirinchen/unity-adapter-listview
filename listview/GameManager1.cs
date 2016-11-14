using UnityEngine;
using System.Collections;
using surfm.listview;
namespace surfm.listview {
    public class GameManager1 : MonoBehaviour {

        private ListView listView;
        private ExampleAdapter adapter;

        void Awake() {
            listView = FindObjectOfType<ListView>();
            adapter = GetComponent<ExampleAdapter>();
        }

        void Start() {
            listView.setAdapter(adapter);
        }


    }
}
