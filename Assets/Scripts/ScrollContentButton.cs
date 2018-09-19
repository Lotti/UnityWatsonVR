using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollContentButton : MonoBehaviour {

    private bool status = false;

    // Use this for initialization
    private void Start() {
        UpdatePanel();
    }

    public void TogglePanel() {
        status = !status;
        UpdatePanel();
    }

    public void HidePanel() {
        status = false;
        UpdatePanel();
    }

    public void ShowPanel() {
        status = true;
        UpdatePanel();
    }

    private void UpdatePanel() {
        if (status) {
            this.transform.GetChild(0).gameObject.SetActive(true);
        } else {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
