using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//控制主摄像机
public class CameraManager : MonoBehaviour
{
    public Image BackGround;
    Vector2 Bias;
    public Camera MainCamera;
    public static CameraManager Instance;
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        Instance = this;
        MainCamera = GetComponent<Camera>();
    }

    public bool OnMainCamera;
    void Update()
    {
        //转化为视角坐标
        Vector3 viewPos = MainCamera.ScreenToViewportPoint(Input.mousePosition);
        if(viewPos.x < 0 || viewPos.y < 0)
        {
            OnMainCamera = false;
        }
        else
        {
            OnMainCamera = true;
        }
        if(!OnMainCamera)return;
        if (MainCamera.orthographic==true)
        {
            MainCamera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 10;
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, 2, 20);
        }
        else
        {
            MainCamera.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * 10;
            MainCamera.fieldOfView = Mathf.Clamp(MainCamera.fieldOfView, 2, 100);
        }

        if (Input.GetMouseButton(1))
        {
            float x = -Input.GetAxis("Mouse X") * 20 * 0.02f;
            float y = -Input.GetAxis("Mouse Y") * 20 * 0.02f;
            Vector3 pos = new (x, y, 0.0f);
            Bias += new Vector2(x/100, y/100);
            BackGround.material.SetVector("_Bias", new Vector4(Bias.x, Bias.y, 0, 0));
            transform.Translate(pos);
        }
    }
}
