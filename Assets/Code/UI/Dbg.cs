
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dbg : MonoBehaviour {

	public static Dbg Instance;
	public Text[] permaLines;
	private int lineCount = 3;
	public Text[] logLines;
	private int logCount = 3;
	private float padding = 10f;
	private bool isInit = false;

	public string[] logStrings;
	private int nextLogIdx;

	void Start()
	{
		if (Instance != null)
		{
			Debug.LogError("FactoryUI Already exists");
		}
		Instance = this;
	}

	
	public void Initialize()
	{
		InitLines();
		InitLog();
		isInit = true;
	}
	
	public void InitLines()
	{
		permaLines = new Text[lineCount];
		for(int i=0; i<lineCount; ++i)
		{
	        GameObject textObject = FactoryUI.Instance.GetBigText();
			textObject.name = "BigText"+i;
			textObject.transform.parent = this.gameObject.transform;
			RectTransform textRect = (RectTransform)textObject.transform;

			Vector2 textPos = Vector2.zero;
			textPos.x = padding + textRect.rect.width/2f;
			textPos.y = -(padding + i*20 + textRect.rect.height/2f);
			textRect.anchoredPosition = textPos;
			permaLines[i] = textObject.GetComponent<Text>();
			permaLines[i].text = string.Empty;
		}
    }

    	public void InitLog()
	{
		logLines = new Text[logCount];
		logStrings = new string[logCount];
		for(int i=0; i<logCount; ++i)
		{
	        GameObject textObject = FactoryUI.Instance.GetLongText();
			textObject.name = "LongText"+i;
			textObject.transform.parent = this.gameObject.transform;
			RectTransform textRect = (RectTransform)textObject.transform;

			Vector2 textPos = Vector2.zero;
			textPos.x = padding + textRect.rect.width/2f;
			textPos.y = (padding + i*20 + textRect.rect.height/2f);
			textRect.anchoredPosition = textPos;
			logLines[i] = textObject.GetComponent<Text>();
			logStrings[i] = string.Empty;
			logLines[i].text = logStrings[i];
		}
    }

    public void SetLabel(int idx, string text)
    {
		if (isInit && idx < lineCount)
		{
			permaLines[idx].text = text;
		}
    }

    public void LogInfo(string str)
    {
    	logStrings[nextLogIdx] = str;
    	nextLogIdx = (nextLogIdx == logCount -1 ? 0 : nextLogIdx+1);

    	RefreshLog();

    }
    public void RefreshLog()
	{
    	for(int i=0; i< logCount; ++i)
    	{
    		int logIdx = i < nextLogIdx ? (i-nextLogIdx+logCount) : (i-nextLogIdx);
    		logLines[i].text = logStrings[logIdx];
    	}

    }
}
