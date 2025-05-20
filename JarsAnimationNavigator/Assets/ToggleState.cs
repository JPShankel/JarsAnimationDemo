using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ToggleState : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Toggle toggle = gameObject.GetComponent<Toggle>();

        if (toggle != null)
        {
            toggle.interactable = !toggle.isOn;
        }
    }
}
