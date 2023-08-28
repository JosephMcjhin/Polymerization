using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Image background;
    Vector2 bias;
    Camera temp;
    public static CameraManager instance;
    void Awake(){
        if(instance != null && instance != this){
            Destroy(this);
        }
        instance = this;
        temp = GetComponent<Camera>();
    }

    public bool Onmaincamera;
    void Update()
    {
        //转化为视角坐标
        Vector3 viewPos = temp.ScreenToViewportPoint(Input.mousePosition);
        if(viewPos.x < 0 || viewPos.y < 0){
            Onmaincamera = false;
        }
        else{
            Onmaincamera = true;
        }
        if(!Onmaincamera)return;
        if (temp.orthographic==true){
            temp.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 10;
            temp.orthographicSize = Mathf.Clamp(temp.orthographicSize, 2, 20);
        }
        else{
            temp.fieldOfView += Input.GetAxis("Mouse ScrollWheel") * 10;
            temp.fieldOfView = Mathf.Clamp(temp.fieldOfView, 2, 100);
        }

        if (Input.GetMouseButton(1)){
            float x = -Input.GetAxis("Mouse X") * 20 * 0.02f;
            float y = -Input.GetAxis("Mouse Y") * 20 * 0.02f;
            Vector3 pos = new Vector3(x, y, 0.0f);
            bias += new Vector2(x/100, y/100);
            background.material.SetVector("_Bias", new Vector4(bias.x, bias.y, 0, 0));
            transform.Translate(pos);
        }
    }
}
