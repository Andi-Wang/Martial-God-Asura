using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;
using System;

public class SkillTreeUIManager : MonoBehaviour {
    public ToggleGroup AGroup;
        public Toggle A1Toggle;
        public Toggle A2Toggle;
        public Toggle A3Toggle;

    public ToggleGroup BGroup;
        public Toggle B1Toggle;
        public Toggle B2Toggle;
        public Toggle B3Toggle;

    public ToggleGroup CGroup;
        public Toggle C1Toggle;
        public Toggle C2Toggle;
        public Toggle C3Toggle;

	// Use this for initialization
	void Start () {
        A1Toggle.onValueChanged.AddListener(delegate { A1ToggleChanged(); });
        A2Toggle.onValueChanged.AddListener(delegate { A2ToggleChanged(); });
        A3Toggle.onValueChanged.AddListener(delegate { A3ToggleChanged(); });

        B1Toggle.onValueChanged.AddListener(delegate { B1ToggleChanged(); });
        B2Toggle.onValueChanged.AddListener(delegate { B2ToggleChanged(); });
        B3Toggle.onValueChanged.AddListener(delegate { B3ToggleChanged(); });

        C1Toggle.onValueChanged.AddListener(delegate { C1ToggleChanged(); });
        C2Toggle.onValueChanged.AddListener(delegate { C2ToggleChanged(); });
        C3Toggle.onValueChanged.AddListener(delegate { C3ToggleChanged(); });
    }

    private void A1ToggleChanged() {
        if(A1Toggle.isOn) {
            A1Toggle.transform.Find("LeftToggleImage").gameObject.SetActive(true);
        }
        else {
            A1Toggle.transform.Find("LeftToggleImage").gameObject.SetActive(false);
        }
    }

    private void A2ToggleChanged() {
        if (A2Toggle.isOn) {
            A2Toggle.transform.Find("MiddleToggleImage").gameObject.SetActive(true);
        }
        else {
            A2Toggle.transform.Find("MiddleToggleImage").gameObject.SetActive(false);
        }
    }

    private void A3ToggleChanged() {
        if (A3Toggle.isOn) {
            A3Toggle.transform.Find("RightToggleImage").gameObject.SetActive(true);
        }
        else {
            A3Toggle.transform.Find("RightToggleImage").gameObject.SetActive(false);
        }
    }

    private void B1ToggleChanged() {
        if (B1Toggle.isOn) {
            B1Toggle.transform.Find("LeftToggleImage").gameObject.SetActive(true);
        }
        else {
            B1Toggle.transform.Find("LeftToggleImage").gameObject.SetActive(false);
        }
    }

    private void B2ToggleChanged() {
        if (B2Toggle.isOn) {
            B2Toggle.transform.Find("MiddleToggleImage").gameObject.SetActive(true);
        }
        else {
            B2Toggle.transform.Find("MiddleToggleImage").gameObject.SetActive(false);
        }
    }

    private void B3ToggleChanged() {
        if (B3Toggle.isOn) {
            B3Toggle.transform.Find("RightToggleImage").gameObject.SetActive(true);
        }
        else {
            B3Toggle.transform.Find("RightToggleImage").gameObject.SetActive(false);
        }
    }

    private void C1ToggleChanged() {
        if (C1Toggle.isOn) {
            C1Toggle.transform.Find("LeftToggleImage").gameObject.SetActive(true);
        }
        else {
            C1Toggle.transform.Find("LeftToggleImage").gameObject.SetActive(false);
        }
    }

    private void C2ToggleChanged() {
        if (C2Toggle.isOn) {
            C2Toggle.transform.Find("MiddleToggleImage").gameObject.SetActive(true);
        }
        else {
            C2Toggle.transform.Find("MiddleToggleImage").gameObject.SetActive(false);
        }
    }

    private void C3ToggleChanged() {
        if (C3Toggle.isOn) {
            C3Toggle.transform.Find("RightToggleImage").gameObject.SetActive(true);
        }
        else {
            C3Toggle.transform.Find("RightToggleImage").gameObject.SetActive(false);
        }
    }






}
