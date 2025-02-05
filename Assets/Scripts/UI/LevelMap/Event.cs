using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    public static event Action<string> ChoiceLvBtn;
    public static void LoadGameLevel(string Lv)
    {
        ChoiceLvBtn?.Invoke(Lv);
    }
 
}
