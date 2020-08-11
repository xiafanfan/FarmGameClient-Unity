using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelController : MonoBehaviour
{
    [SerializeField]
    TMP_Text resultText;
    public void ShowResult(string result)
    {
        resultText.text = result;
        gameObject.SetActive(true);
    }
    private void OnMouseDown()
    {
        gameObject.SetActive(false);
    }
}
