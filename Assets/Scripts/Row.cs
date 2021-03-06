using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
    [Tooltip("即时延迟")] public int Result = -1;
    [Tooltip("平均延迟")] public int ResultPS = -1;
    [Tooltip("丢包率")] public int Loss = -1;
    public Text IndexText;
    public InputField IPText;
    public Text ShowIPText;
    public Text ResultText;
    public Text ResultPSText;
    public Text LossText;
    public static float RowSize = 45;
    public Ping ping;
    List<float> data = new List<float>();
    [SerializeField] Vector2Int Count = new Vector2Int();
    private void Start()
    {
        UpdateText();
        StartCoroutine(FreshPing());
        StartCoroutine(FreshPingPerSecond());
    }
    private void OnEnable()
    {
        EndEdit();
    }
    /// <summary>
    /// 输入框改变数值后
    /// </summary>
    public void ChangeValue()
    {
        if (IP != IPText.text)
        {
            Count = new Vector2Int();
            data.Clear();
        }
        IP = IPText.text;
        ChooseThis();
        IP = ValueLimit(IP);
        UpdateText();

        UpdatePing();
    }
    public void InitializeValue(string ip)
    {
        IPText.text = IP = ip;
        UpdateText();
        ChangeValue();
    }
    public void InitializeValue(IPAddress address)
    {
        IPText.text = IP = address.ToString();
        UpdateText();
        ChangeValue();
    }
    /// <summary>
    /// 限制数值范围
    /// </summary>
    public static string ValueLimit(string IP)
    {
        string result = IP;
        if (!int.TryParse(IP.Replace("http://", "").Replace("https://", "").Replace(".", ""), out int num))
        {
            try
            {
                IP = IPInformation.GetIP(IP).ToString();
            }
            catch { }
        }
        IP = IPInformation.LimitIPv4(IP);
        return IP;
    }
    public void EditState()
    {
        ShowIPText.gameObject.SetActive(false);
        IPText.textComponent.gameObject.SetActive(true);
    }
    public void EndEdit()
    {
        ShowIPText.gameObject.SetActive(true);
        IPText.textComponent.gameObject.SetActive(false);
    }
    /// <summary>
    /// 更新显示文本
    /// </summary>
    public void UpdateText()
    {
        ShowIPText.text = IP;
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
                    Count.x++;
                    break;
                }
                yield return 0;
            }
            Count.y++;
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
            Loss = 1000 - (Count.x * 1000f / Count.y).ToInteger();
            //data.Clear();
        }
    }
    void UpdatePing()
    {
        if (string.IsNullOrWhiteSpace(IP) || IP == "0.0.0.0" || IP == "127.0.0.1" || IP == "255.255.255.255")
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

        ResultText.text = Result > 0 ? Result.ToString() : "-";
        ResultPSText.text = ResultPS > 0 ? ResultPS.ToString() : "-";
        /*if (Loss >= 0)
        {
            string temp = Loss.ToString("F1");
            if (temp[temp.Length - 1] == '0')
                LossText.text = Loss.ToString("F0") + "%";
            else
                LossText.text = Loss.ToString("F1") + "%";
        }
        else
        {
            LossText.text = "-";
        }*/
        LossText.text = Loss >= 0 ? ((Loss * 0.1f).ToString() + "%") : "-";
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
