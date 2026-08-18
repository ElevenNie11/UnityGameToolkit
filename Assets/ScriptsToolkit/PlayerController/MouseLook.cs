//此脚本挂载到Main Camera上
using System;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("鼠标灵敏度")]
    public float mouseSensitivity = 100f;

    public Transform playerBody;
    private float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;                     //上下看时是绕着X轴转动
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);//为什么要用这种形式而不用.Rotate()的原因是，这种形式可以给上下视角加入一个限制Clamp
        playerBody.Rotate(Vector3.up * mouseX);  //左右转身时是绕着Y轴转动
    }
}


//四元数：Quaternion
//四元数是以相当紧凑的方式决定对象的旋转，所有旋转方向均以右手为准