
using UnityEngine;
using System.Collections;

public class FactoryUI : MonoBehaviour {

    public static FactoryUI Instance;
    public GameObject bigTextPrefab;
    public GameObject longTextPrefab;
    
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryUI Already exists");
        }
        Instance = this;
    }

    public GameObject GetBigText()
    {
        return Instantiate(bigTextPrefab) as GameObject;
    }

    public GameObject GetLongText()
    {
        return Instantiate(longTextPrefab) as GameObject;
    }
}
