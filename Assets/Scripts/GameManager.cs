using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }
    public float timeRemaining = 2f; // 初始倒计时时间
    public Transform[] v2Point;

    public GameObject goEnemyBullent;
    public Transform player;       // 主角引用
    // Start is called before the first frame update
    void Start()
    {
      //  LoadGoScene("Enemy Bug");
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // 每帧减少倒计时
            if (timeRemaining < 0 )
            {
                LoadGoScene("Enemy Bug");
                timeRemaining = 2f;
            }
        }

    }




    public GameObject LoadGoScene(string loadPath)
    {
        GameObject go = null;
        int ran=Random.Range(0,v2Point.Length);
        go = Resources.Load<GameObject>(loadPath);

        // 实例化加载的游戏对象
        GameObject ins = Instantiate(go);

        // 确保目标位置有效
        if (v2Point.Length > 0)
        {
            ins.transform.position = v2Point[ran].position;
            ins.transform.localScale = v2Point[ran].localScale*3f;
            Debug.Log("Loaded object at position: " + v2Point[ran].position);
        }
        else
        {
            Debug.LogError("No valid positions in v2Point array.");
        }

        return ins;  // 返回实例化的对象
    }
}
