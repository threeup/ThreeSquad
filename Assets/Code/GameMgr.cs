
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class GameMgr : MonoBehaviour {


    
    public World world; //wiredup
    public Maker maker; //wiredup

    public GoogMgr googMgr;
    public Dbg dbg; //wiredup
    public UserMgr userMgr; //wiredup
    public InputMgr inputMgr; //wiredup
    public CameraMgr cameraMgr; //wiredup
    public GridLineMgr gridLineMgr; //wiredup
    public UIMgr uiMgr; //wiredup

	public delegate void InitDelegate();
    private float timer = 0.0f;
    private bool isStart = false;
    private bool isInit = false;
	private List<InitDelegate> inits;
	private int initIdx = 0;
	private InitDelegate nextInit;

	public void Start()
	{
		inits = new List<InitDelegate>();
		inits.Add(InitDebug);
        inits.Add(InitUser);
        inits.Add(InitWorld);
		inits.Add(InitMaker);
		inits.Add(InitCamera);
        inits.Add(InitGridLine);
        inits.Add(InitGoog);
		inits.Add(InitInput);
        inits.Add(InitUI);
		isStart = true;
	}

    public void Update()
    {
		if (isStart && !isInit)
    	{
    		timer++;
    		if (timer > 0.1f)
    		{
				inits[initIdx]();
				initIdx++;
				timer = 0;
    		}
			if (initIdx >= inits.Count)
			{
				isInit = true;
			}
    	}
    }

    public void InitGoog()
    {
        googMgr = new GoogMgr();
        googMgr.Initialize();
        googMgr.Authenticate();
	}

	public void InitDebug()
	{
		if (dbg != null)
		{
        	dbg.Initialize();
		}
    }

    public void InitMaker()
    {
    	maker.Initialize();
    }

    public void InitInput()
    {
    	inputMgr.Initialize();
    }

    public void InitCamera()
    {
    	cameraMgr.Initialize();
    }

    public void InitGridLine()
    {
        gridLineMgr.Initialize();
    }

    public void InitUser()
    {
        userMgr.Initialize();
    }

    public void InitWorld()
    {
        world.Initialize();
    }

    public void InitUI()
    {
        uiMgr.Initialize();
    }
}
