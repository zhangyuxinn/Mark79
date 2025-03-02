using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvBtn : MonoBehaviour
{
    public void OnButtonClick(string Lv)
    {
        Event.LoadGameLevel(Lv);
    }
}
