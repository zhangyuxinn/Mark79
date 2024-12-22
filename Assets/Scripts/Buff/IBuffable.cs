using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    List<BuffBase> effectingBuff { get; set; }
    void OnBuff(BuffBase buffBase);

    void RemoveBuff(BuffBase buffBase);

    IEnumerator StartBuff(BuffBase buffBase);

}
