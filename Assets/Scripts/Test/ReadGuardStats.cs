using System.Collections;
using System.Collections.Generic;
using System.IO;
using cfg;
using UnityEngine;
using SimpleJSON;
using cfg.stats;


public class ReadGuardStats : MonoBehaviour
{
    public List<GuardStatSO> guardStats;
    private List<Stats> getStats;
    void Awake()
    {
      string gameConfDir = Application.streamingAssetsPath+"/Luban"; // 替换为gen.bat中outputDataDir指向的目录
      var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));

      //检查
      getStats = tables.TbGuardStats.DataList;
      Debug.Log(getStats.Count);
      Debug.Log(getStats.Count);
      for (int i = 0; i < guardStats.Count; i++)
      {
          guardStats[i].health = getStats[i].GuardHP;
          guardStats[i].fightState.damage = getStats[i].Damage;
          guardStats[i].fightState.shootInterval = getStats[i].ShootInterval;
          guardStats[i].fightRange = getStats[i].FightRange;
          guardStats[i].fightState.damageRange = getStats[i].DamageRange;
          guardStats[i].callCost = getStats[i].Cost;
          Debug.Log(getStats[i].Front);
          guardStats[i].isfront = getStats[i].Front;
      }

      Debug.Log("读取完毕");
    }
}

