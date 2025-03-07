using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenManagerTitle : MonoBehaviour
{
    /// <summary>
    /// 매인게임 씬 가기
    /// </summary>
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("MainGame");
        }
    }
}
