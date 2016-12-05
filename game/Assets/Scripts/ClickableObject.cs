using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler {
    string skillName;
        
    private void Awake() {
        skillName = gameObject.name;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            //Increment player skill level based on skillName
        }
        else if (eventData.button == PointerEventData.InputButton.Middle) {
            //Debug.Log("Middle click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            //Decrement player skill level based on skillName
        }
    }
}