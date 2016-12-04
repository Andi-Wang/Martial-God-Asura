using UnityEngine;
using System.Collections;

public class Controls {
    public float h;
    public float v;
    public bool vDown;
    public bool vUp;

    public bool altMoveDown;
    public bool altMoveHold;

    public bool jumpDown;
    public bool jumpHold;

    public bool interactDown;
    public bool interactHold;

    public bool fire1Down;
    public bool fire1Hold;

    public bool fire2Down;
    public bool fire2Hold;

    public bool fire3Down;
    public bool fire3Hold;

    public void resetButtonDown() {
        altMoveDown = false;
        jumpDown = false;
        interactDown = false;
        fire1Down = false;
        fire2Down = false;
        fire3Down = false;
    }
}
