using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 背景板UI效果 : MonoBehaviour
{
    // Start is called before the first frame update
    
      public Vector2 range = new Vector2(5f, 3f);

    Transform mTrans;
    Quaternion mStart;
    Vector2 mRot = Vector2.zero;

    void Start()
    {
        mTrans = transform;
        mStart = mTrans.localRotation;
    }

    void Update()
    {
        Vector3 pos = Input.mousePosition;

        float halfWidth = Screen.width * 0.5f;
        float halfHeight = Screen.height * 0.5f;
        float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
        float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
        mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);

        mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, mRot.x * range.x, 0f);
    }

    // Update is called once per frame
   
}
