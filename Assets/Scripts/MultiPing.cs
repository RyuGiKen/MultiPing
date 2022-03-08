using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using RyuGiKen;
public class MultiPing : MonoBehaviour
{
    public static MultiPing instance;
    public List<Row> Rows;
    public Transform ListParent;
    public Row row;
    public int ChooseIndex = -1;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        ListParent.GetChildren().SetActive(false);

        string[] data = GetFile.LoadXmlData(new string[] { "IP" }, Application.streamingAssetsPath + "/Setting.xml", "Data", true);

        for (int i = 0; i < data.Length; i++)
        {
            string temp = null;
            if (IPAddress.TryParse(data[i], out IPAddress iPAddress))
            {
                temp = iPAddress.ToString();
                AddRow();
                Rows[Rows.Count - 1].InitializeValue(temp);
            }
        }
    }
    void Update()
    {

    }
    private void LateUpdate()
    {
        for (int i = Rows.Count - 1; i >= 0; i--)
        {
            if (Rows[i])
                Rows[i].Index = i;
            else
                Rows.RemoveAt(i);
        }
        ListParent.GetComponent<RectTransform>().sizeDelta = new Vector2(ListParent.GetComponent<RectTransform>().sizeDelta.x, Rows.Count.Clamp(1) * Row.RowSize);
    }
    /// <summary>
    /// 增加行
    /// </summary>
    public void AddRow()
    {
        Rows.Add(Instantiate(row, ListParent));
    }
    public void DeleteAll()
    {
        if (ChooseIndex < 0)
        {
            for (int i = Rows.Count - 1; i >= 0; i--)
            {
                Rows[i].Delete();
            }
        }
        else
        {
            if (ChooseIndex < Rows.Count)
                Rows[ChooseIndex].Delete();
        }
        ChooseIndex = -1;
    }
    /// <summary>
    /// 列表排序
    /// </summary>
    public void SortList()
    {
        if (Rows.Count > 0)
            Rows.Sort(delegate (Row row01, Row row02)
            {
                int result = row01.ResultPS.CompareTo(row02.ResultPS);
                if (row01.ResultPS < 0)
                    result = 1;
                else if (row02.ResultPS < 0)
                    result = -1;

                if (result == 0)
                {
                    int result2 = row01.IP.CompareTo(row02.IP);
                    if (result2 == 0)
                    {
                        //return row01.Index.CompareTo(row02.Index);
                    }
                    return result2;
                }
                return result;
            });
    }
}
