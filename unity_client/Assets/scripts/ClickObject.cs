using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ClickObject : MonoBehaviour , IPointerClickHandler{

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData ed)
    {
        Debug.Log("Test Click");
    }
}
