﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RyuGiKen;
public class Row : MonoBehaviour
{
    /// <summary>
    /// 列表中序号[0,999],显示+1
    /// </summary>
    [Tooltip("序号")] public int Index = -1;
    public string IP = "0.0.0.0";
    [Tooltip("结果")] public int Result = -1;
    [Tooltip("结果")] public int ResultPS = -1;
    public Text IndexText;
    public InputField IPText;
    public Text ResultText;
    public Text ResultPSText;
    public static float RowSize = 50;
    public Ping ping;
    List<float> data = new List<float>();
    private void Start()
    {
        StartCoroutine(FreshPing());
        StartCoroutine(FreshPingPerSecond());
    }
    private void OnEnable()
    {

    }
    /// <summary>
    /// 输入框改变数值后
    /// </summary>
    public void ChangeValue()
    {
        IP = IPText.text;
        ChooseThis();
        ValueLimit();
        UpdateText();

        UpdatePing();
    }
    public void InitializeValue(string ip)
    {
        IP = ip;
        UpdateText();
        ChangeValue();
    }
    /// <summary>
    /// 限制数值范围
    /// </summary>
    public void ValueLimit()
    {
        IP = IPInformation.LimitIPv4(IP);
    }
    /// <summary>
    /// 更新显示文本
    /// </summary>
    public void UpdateText()
    {
        IPText.text = IP;
        ResultText.text = Result.ToString();
    }
    IEnumerator FreshPing()
    {
        float time = 0;
        while (true)
        {
            time = 0;
            while (time < 1)
            {
                time += Time.deltaTime;
                if (ping != null && ping.isDone && time > 0.1f)
                {
                    break;
                }
                yield return 0;
            }
            UpdatePing();
            yield return 0;
        }
    }
    IEnumerator FreshPingPerSecond()
    {
        float sum = 0;
        while (true)
        {
            sum = 0;
            yield return new WaitForSeconds(1);
            for (int i = 0; i < data.Count; i++)
            {
                sum += data[i] < 0 ? 999 : data[i];
            }
            ResultPS = (sum * 1f / data.Count).ToInteger();
            data.Clear();
        }
    }
    void UpdatePing()
    {
        if (IP == "0.0.0.0" || IP == "127.0.0.1" || IP == "255.255.255.255")
            return;
        if (ping != null)
        {
            Result = ping.time;
            data.Add(Result);
            ping.DestroyPing();
        }
        ping = new Ping(IP);
    }
    private void Update()
    {
        SetPositionByIndex();
        //GetIndexFromPosition();

        /*if (ping != null && ping.isDone)
        {
            Result = ping.time;

            ping.DestroyPing();
            ping = new Ping(IP);
        }*/
    }
    private void LateUpdate()
    {
        UpdateRowIndex();
        //UpdateText();

        ResultText.text = Result < 0 ? "-" : Result.ToString();
        ResultPSText.text = ResultPS < 0 ? "-" : ResultPS.ToString();

        this.GetComponent<Image>().color = Index == MultiPing.instance.ChooseIndex ? Color.cyan : Color.white;
    }
    public void ChooseThis()
    {
        MultiPing.instance.ChooseIndex = Index;
    }
    /// <summary>
    /// 刷新行序号
    /// </summary>
    public void UpdateRowIndex()
    {
        IndexText.text = (Index + 1).ToString("D3");
        this.name = "Row_" + IndexText.text;
    }
    /// <summary>
    /// 根据序号调整位置
    /// </summary>
    void SetPositionByIndex()
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(0, RowSize);
        this.GetComponent<RectTransform>().offsetMax = new Vector2(0, -Index * RowSize);
        this.GetComponent<RectTransform>().offsetMin = new Vector2(0, this.GetComponent<RectTransform>().offsetMax.y - RowSize);
    }
    /// <summary>
    /// 从位置判定序号
    /// </summary>
    void GetIndexFromPosition()
    {
        //Index = Mathf.RoundToInt(-this.GetComponent<RectTransform>().offsetMax.y / GameParameter.RowSize.y);
    }
    /// <summary>
    /// 删除自己
    /// </summary>
    public void Delete()
    {
        Destroy(this.gameObject, 0.1f);
    }
    private void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
        if (ping != null)
            ping.DestroyPing();
    }
}