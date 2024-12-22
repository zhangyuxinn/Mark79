using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInGroup
{
    void Seek(Vector3 target);
    void Flee(Vector3 target);
    void SlowDown();
    
}
