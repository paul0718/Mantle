using System;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedVision : MonoBehaviour
{
    [SerializeField] private GridManager gridScript;
    
    [SerializeField] private Image leftImg;
    [SerializeField] private Image rightImg;

    [SerializeField] private GameObject equalSign;

    [SerializeField] private AudioSource audio;

    private void OnEnable()
    {
        SetMinigame();
    }

    public void CheckResults()
    {
        if (leftImg.color == rightImg.color)
        {
            Vector3 tempLeft = leftImg.transform.localScale;
            Vector3 tempRight = rightImg.transform.localScale;
            if((tempLeft.x < 0 && tempRight.x < 0) || (tempLeft.x > 0 && tempRight.x > 0))
            {
                if((tempLeft.y < 0 && tempRight.y < 0) || (tempLeft.y > 0 && tempRight.y > 0))
                {
                    audio.Play();
                    equalSign.SetActive(true);
                    gridScript.UpdateDotPosition(40, -40);
                    GameObject.Find("Enemy").GetComponent<EnemyInfo>().UpdateBark("Project Image", "W");
                }
            }
        }
    }

    public void RedButton()
    {
        if (leftImg.color.r == 0)
        {
            leftImg.color = new Color(1, leftImg.color.g, leftImg.color.b);
        }
        else
        {
            leftImg.color = new Color(0, leftImg.color.g, leftImg.color.b);
        }
        
        CheckResults();
    }
    
    public void BlueButton()
    {
        if (leftImg.color.b == 0)
        {
            leftImg.color = new Color(leftImg.color.r, leftImg.color.g, 1);
        }
        else
        {
            leftImg.color = new Color(leftImg.color.r, leftImg.color.g, 0);
        }
        
        CheckResults();
    }
    
    public void GreenButton()
    {
        if (leftImg.color.g == 0)
        {
            leftImg.color = new Color(leftImg.color.r, 1, leftImg.color.b);
        }
        else
        {
            leftImg.color = new Color(leftImg.color.r, 0, leftImg.color.b);
        }
        
        CheckResults();
    }
    
    public void YellowButton()
    {
        Vector3 temp = leftImg.gameObject.transform.localScale;
        leftImg.gameObject.transform.localScale = new Vector3(temp.x, -1 * temp.y, temp.x);
        
        CheckResults();
    }
    
    public void PurpleButton()
    {
        Vector3 temp = leftImg.gameObject.transform.localScale;
        leftImg.gameObject.transform.localScale = new Vector3(-1 * temp.x, temp.y, temp.x);
        
        CheckResults();
    }
    
    public void RightRedButton()
    {
        if (rightImg.color.r == 0)
        {
            rightImg.color = new Color(1, rightImg.color.g, rightImg.color.b);
        }
        else
        {
            rightImg.color = new Color(0, rightImg.color.g, rightImg.color.b);
        }
        
        CheckResults();
    }
    
    public void RightBlueButton()
    {
        if (rightImg.color.b == 0)
        {
            rightImg.color = new Color(rightImg.color.r, rightImg.color.g, 1);
        }
        else
        {
            rightImg.color = new Color(rightImg.color.r, rightImg.color.g, 0);
        }
        
        CheckResults();
    }
    
    public void RightGreenButton()
    {
        if (rightImg.color.g == 0)
        {
            rightImg.color = new Color(rightImg.color.r, 1, rightImg.color.b);
        }
        else
        {
            rightImg.color = new Color(rightImg.color.r, 0, rightImg.color.b);
        }
        
        CheckResults();
    }
    
    public void RightYellowButton()
    {
        Vector3 temp = rightImg.gameObject.transform.localScale;
        rightImg.gameObject.transform.localScale = new Vector3(temp.x, -1 * temp.y, temp.x);
        CheckResults();
    }
    
    public void RightPurpleButton()
    {
        Vector3 temp = rightImg.gameObject.transform.localScale;
        rightImg.gameObject.transform.localScale = new Vector3(-1 * temp.x, temp.y, temp.x);
        CheckResults();
    }

    void SetMinigame()
    {
        equalSign.SetActive(false);

        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        int n = UnityEngine.Random.Range(0, 2);
        Vector3 temp = leftImg.gameObject.transform.localScale;
        Vector3 tempR = rightImg.gameObject.transform.localScale;
        if (n == 0)
        {
            Debug.Log(0);
            leftImg.gameObject.transform.localScale = new Vector3(temp.x, temp.y, temp.x);
            rightImg.gameObject.transform.localScale = new Vector3(-1 * tempR.x, tempR.y, tempR.x);
            leftImg.color = new Color(1, 0, 1);
            rightImg.color = new Color(0, 1, 0);
        }
        else
        {
            Debug.Log(1);
            leftImg.gameObject.transform.localScale = new Vector3(-1 * temp.x, -1 * temp.y, temp.x);
            rightImg.gameObject.transform.localScale = new Vector3(tempR.x, tempR.y, tempR.x);
            leftImg.color = new Color(1, 0, 0);
            rightImg.color = new Color(1, 1, 0);
        }
    }
}
