using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteShapeGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteShapeController controller = GetComponent<SpriteShapeController>();
        Spline spline = controller.spline;
        spline.SetPosition(0, new Vector3(0, 0, 0));
        spline.SetPosition(1, new Vector3(3, 0, 0));
        spline.SetPosition(2, new Vector3(3, -3, 0));
        spline.SetLeftTangent(1, new Vector3(-0.05f, 0.05f, 0));
        spline.SetRightTangent(1, new Vector3(0.05f, -0.05f, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
