using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithm;
public class InFightRange : MonoBehaviour
{
    public List<GameObject> inFightRangeEnemies;
    public List<GameObject> inFightRangeNeutral;
    [SerializeField] private HomeBoomEventChannel homeBoomEventChannel;

    private void OnEnable()
    {
        homeBoomEventChannel.AddListener(OnHomeBoom);
    }

    private void OnDisable()
    {
        homeBoomEventChannel.RemoveListener(OnHomeBoom);
    }

    private void OnHomeBoom(Team team,HomeType homeType)
    {
        inFightRangeEnemies.Clear();
        inFightRangeNeutral.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")||other.CompareTag("Untagged")|| other.name=="DreamEnemy(Clone)")
        {
            return;
        }
        int layerIndex = other.gameObject.layer;
        if (gameObject.layer == 12)
        {
            if (layerIndex == 10 || layerIndex == 11)
            {
                inFightRangeEnemies.Add(other.gameObject);
                other.gameObject.AddComponent<GameObjectDestroyListener>().inFightRange = this;
            }
        }
        if (layerIndex!=gameObject.layer&&layerIndex>9&& layerIndex<12)
        {
            inFightRangeEnemies.Add(other.gameObject);
            other.gameObject.AddComponent<GameObjectDestroyListener>().inFightRange = this;

        }else if (layerIndex == 12)
        {
            inFightRangeNeutral.Add(other.gameObject); 
            other.gameObject.AddComponent<GameObjectDestroyListener>().inFightRange = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        int layerIndex = other.gameObject.layer;
        if (gameObject.layer == 12)
        {
            if (layerIndex == 10 || layerIndex == 11)
            {
                inFightRangeEnemies.Remove(other.gameObject);
            }
        }
        if (layerIndex!=gameObject.layer&&layerIndex>9&& layerIndex<12)
        {
            inFightRangeEnemies.Remove(other.gameObject);
        }else if (layerIndex == 12)
        {
            inFightRangeNeutral.Remove(other.gameObject);
        }
    }
    public void RemoveGameObjectFromList(GameObject destroyedObject)
    {
        if (inFightRangeEnemies.Contains(destroyedObject))
        {
            inFightRangeEnemies.Remove(destroyedObject);
        }

        if (inFightRangeNeutral.Contains(destroyedObject))
        {
            inFightRangeNeutral.Remove(destroyedObject);
        }
    }
}
