using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDepthBuffer : MonoBehaviour
{
    private void OnPreRender()
    {
        GL.Clear(false, true, Color.clear);
    }
}
