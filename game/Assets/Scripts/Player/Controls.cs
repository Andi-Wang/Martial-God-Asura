using UnityEngine;
using System.Collections;

public class Controls {
    public float h;
    public float v;
    public bool vDown;
    public bool vUp;

    public bool altMoveUp;
    public bool altMoveDown;
    public bool altMoveHold;

    public bool jumpDown;
    public bool jumpHold;

    public bool interactDown;
    public bool interactHold;

    public bool fire1Up;
    public bool fire1Down;
    public bool fire1Hold;

    public bool fire2Up;
    public bool fire2Down;
    public bool fire2Hold;

    public bool fire3Up;
    public bool fire3Down;
    public bool fire3Hold;

    public bool fire4Up;
    public bool fire4Down;
    public bool fire4Hold;

    public void resetButtonDown() {
        altMoveDown = false;
        altMoveUp = false;

        jumpDown = false;
        interactDown = false;
        fire1Up = false;
        fire2Up = false;
        fire3Up = false;
        fire4Up = false;

        fire1Down = false;
        fire2Down = false;
        fire3Down = false;
        fire4Down = false;
    }
}
