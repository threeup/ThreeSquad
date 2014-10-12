
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class UserMgr : MonoBehaviour {

    public static UserMgr Instance;
    public List<User> users;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("UserMgr Already exists");
        }
        Instance = this;
    }

    public void Initialize()
    {
        AddUser(0);
        AddUser(1);
        AddUser(2);

    }

    public void AddUser(int idx)
    {
        GameObject go = new GameObject();
        go.name = "User"+idx;
        User user = go.AddComponent<User>();
        user.Initialize();
        users.Add(user);
    }
}
