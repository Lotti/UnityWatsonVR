using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Modal : MonoBehaviour {

    public Button buttonClose;
    public Text textContent;
    private static Modal instance;

	private void Awake() {
        instance = this;

        CloseModal();
    }

    public void CloseModal() {
        this.gameObject.SetActive(false);
    }

    public static void ShowModal(string text) {
        instance.gameObject.SetActive(true);
        instance.textContent.text = text;
    }
}
