using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private bool startOfScene;

    private Image panelImg;
    public static SceneTransition Instance { get; private set; }

    private SequenceManager seqMan;

    private static readonly Dictionary<int, string> SceneMap = new Dictionary<int, string>() {
        {1,"CutsceneBuild" },
        {2,"NewsConvo" },
        {3,"BattleScene" },
        {4,"NewsConvo" },
        {5,"NewsConvo" },
        {6,"BattleScene" },
        {7,"NewsConvo" },
        {8,"NewsConvo" },
        {9,"BattleScene" },
        {10,"NewsConvo" },
        {11,"NewsConvo" },
        {12,"BattleScene" },
        {13,"S13Comic" },
        {14,"BattleScene" },
        {15,"S15Comic" },
        {16,"NewsConvo" },
        {17,"BattleScene" },
        {18,"S18Comic" },
    };



    public void Start()
    {
        Instance = this;
        seqMan = SequenceManager.Instance;
        panelImg = GetComponent<Image>();
        if (startOfScene)
        {
            FadeOutOfBlack();
        }
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SequenceManager.Instance.SequenceID = 17;
                SceneManager.LoadScene("BattleScene");
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                FadeToBlack();
            }
        }
    }

    public void FadeToBlack(float t = 1.5f)
    {
        if(AudioManager.Instance != null)
            AudioManager.Instance.FadeOut();
        Debug.Log(t);
        panelImg.DOFade(1, t).onComplete = GoToNextScene;
    }

    public void FadeOutOfBlack()
    {
        panelImg.color = new Color(0, 0, 0, 1);
        panelImg.DOFade(0, 2f);
    }
    public void SwitchScene(string name)
    {
        if(AudioManager.Instance != null)
            AudioManager.Instance.FadeOut();
        panelImg.DOFade(1, 1.5f).onComplete = () =>
        {
            SceneManager.LoadScene(name);
            DOTween.KillAll();
        };
    }
    public void SwitchScene(int ID)
    {
        panelImg.DOFade(1, 1.5f).onComplete = () =>
        {
            SequenceManager.Instance.SequenceID = ID;
            SceneManager.LoadScene(SceneMap[ID]);
            DOTween.KillAll();
        };
    }
    private void GoToNextScene()
    {
        // if (SceneManager.GetActiveScene().name == "NewsConvo")
        // {
        //     if (SequenceManager.Instance.SequenceID >= 4)
        //     {
        //         seqMan.steamPageOn = true;
        //         SceneManager.LoadScene("MainMenu");
        //         return;
        //     } 
        // }
        if (SceneManager.GetActiveScene().name == "LoseBattle")
        {
            SceneManager.LoadScene("BattleScene");
            return;
        }

        if (SequenceManager.Instance.SequenceID == 19)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "DateScene")
        {
            Debug.Log("do nothing to seq ID");
        }
        else
        {
            seqMan.SequenceID++;
        }

        if (SceneManager.GetActiveScene().name != "CutsceneBuild" && SceneManager.GetActiveScene().name != "DateScene")
        {
            if ((SequenceManager.Instance.SequenceID > 15 || SequenceManager.Instance.SequenceID < 13) 
                && SequenceManager.Instance.SequenceID < 17)
            {
                SceneManager.LoadScene("DateScene");
                return;
            }
        }

        if (SequenceManager.Instance.SequenceID == 20)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
;
        if (seqMan.desktopSeq.Contains(seqMan.SequenceID))
        {
            if (seqMan.SequenceID == 16)
                SceneManager.LoadScene("S16NewsConvo");
            else
                SceneManager.LoadScene("NewsConvo");
        }
        else if (seqMan.battleSeq.Contains(seqMan.SequenceID))
        {
            SceneManager.LoadScene("BattleScene");
        }
        else if (seqMan.cutsceneSeq.Contains(seqMan.SequenceID))
        {
            switch (seqMan.SequenceID)
            {
                case 1:
                    SceneManager.LoadScene("CutsceneBuild");
                    break;
                case 13:
                    SceneManager.LoadScene("S13Comic");
                    break;
                case 15:
                    SceneManager.LoadScene("S15Comic");
                    break;
                case 18:
                    SceneManager.LoadScene("S18Comic");
                    break;
            }
            
        }
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
