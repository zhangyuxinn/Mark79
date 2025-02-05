using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LvBtnManager : MonoBehaviour
{
    private void OnEnable()
    {
        Event.ChoiceLvBtn += GoToLevel;
    }

    private void OnDisable()
    {
        Event.ChoiceLvBtn -= GoToLevel;
    }




    private void GoToLevel(string Lv)
    {
       SceneManager.LoadScene(Lv);
    }
}
