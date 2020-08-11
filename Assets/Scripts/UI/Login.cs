using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    TMP_InputField username;
    [SerializeField]
    TMP_Text message;
    [SerializeField]
    GameObject bg;

    public void Click()
    {
        message.text = "";
        Game.instance.Login(username.text, this);
        GetComponent<Button>().interactable = false;
    }
    public void OK()
    {
        bg.SetActive(false);
        panel.SetActive(false);
    }
    public void Wrong(string error)
    {
        message.text=error;
        username.text = "";
        GetComponent<Button>().interactable = true;
    }
}
