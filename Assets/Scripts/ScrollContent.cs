using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollContent : MonoBehaviour {

    public GameObject textPrefab;
    public RectTransform content;

    // Use this for initialization
    private void Start() {
        ClearContent();
    }

    private void ClearContent() {
        for (int i = 0; i < content.childCount; i++) {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    public void AddContent(string[] s) {
        if (s.Length > 0) {
            ClearContent();
            RectTransform goT = null;
            for (int i = 0; i < s.Length; i++) {
                GameObject go = Instantiate(textPrefab);
                go.transform.SetParent(content, false);
                go.transform.localScale = new Vector3(1f, 1f, 1f);
                go.GetComponent<Text>().text = s[i];
                goT = go.GetComponent<RectTransform>();
                goT.localPosition = new Vector3(goT.localPosition.x, goT.localPosition.y - i * goT.rect.height, goT.localPosition.z);
            }
            if (goT != null) {
                content.sizeDelta = new Vector2(content.sizeDelta.x, s.Length * goT.rect.height);
            }
        }
    }
}
