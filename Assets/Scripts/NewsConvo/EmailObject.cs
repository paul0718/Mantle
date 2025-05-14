using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmailObject : MonoBehaviour
{
    public string sender;
    public string subject;
    [TextArea(3, 10)]
    public string body;
    [SerializeField] private GameObject currentEmail;
    
    public bool read;
    public Color readColor;
    [SerializeField] private Color activeColor;

    public void OpenEmail()
    {
        foreach (Transform child in transform.parent)
        {
            if (child.GetComponent<EmailObject>().read)
                child.GetComponent<Image>().color = readColor;
        }
        GetComponent<Image>().color = activeColor;
        currentEmail.SetActive(true);
        currentEmail.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "From: " + sender;
        currentEmail.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = subject;
        currentEmail.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = body;
        if (!GameObject.Find("Alert Manager").GetComponent<AlertManager>().alertActive && name.Contains("Ace Email") && !read && SequenceManager.Instance.SequenceID != 19)
        {
            GameObject.Find("Convo Manager").GetComponent<ConvoManager>().EmailResponse();
        }
        read = true;
    }
}