using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D[] cursors;
    private int cursorIndex = -1;
    public static CursorManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SetNextCursor();

    }
    public void StartCursorAnimation()
    {
        InvokeRepeating("SetNextCursor", 0, 0.1f);
    }
    private void SetNextCursor()
    {
        cursorIndex++;
        cursorIndex %= cursors.Length;
        Cursor.SetCursor(cursors[cursorIndex], new Vector2(cursors[cursorIndex].width / 8, cursors[cursorIndex].height / 8), CursorMode.Auto);
    }

    public void StopCursorAnimation()
    {
        CancelInvoke();
        SetNextCursor();
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
}
