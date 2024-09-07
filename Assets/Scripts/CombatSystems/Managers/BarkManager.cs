using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BarkManager : MonoBehaviour
{
    GoogleSheetsDB _googleSheetsDB;
    public GoogleSheet _txtSheet;

    [SerializeField] private EnemyInfo ei;
    //-----------------------------------------------------------------------------

    public void Start()
    {
        _googleSheetsDB = gameObject.GetComponent<GoogleSheetsDB>();
        _googleSheetsDB.OnDownloadComplete += DoneDownload;
    }

    void DoneDownload()
    {
        Debug.Log("bark's done download");
    }

    //Read the google sheets for the bark
    public string ReturnBark(string game, string n)
    {
        int txtSheetIndex = _googleSheetsDB.sheetTabNames.IndexOf("Sheet1");
        _txtSheet = _googleSheetsDB.dataSheets[txtSheetIndex];
        return _txtSheet.GetRowData(game, n.ToString());
    }
}
