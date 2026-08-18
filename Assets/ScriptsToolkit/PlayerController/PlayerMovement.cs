using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 12f;
    public float gravity = -9.81f;   //重力加速度（方向向下）
    public float jumpHeight = 2f;    //跳跃高度
    public CharacterController controller;
    private Vector3 velocity;  //用来存储当前速度

    void Start()
    {
        velocity.y = -2f;  //开局就给一个小的向下速度，确保第一帧就触发地面检测，玩家紧贴地面
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        //Time.deltaTime的作用：确保游戏效果与电脑帧速率无关（性能不同的设备帧速率不同）
        controller.Move(move * speed * Time.deltaTime);

        /*--------------跳跃逻辑----------------*/
        //检测是否着地，按下空格且在地面时触发跳跃
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //公式： v = √(2 * g * h)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        /*------------------------------------*/
        
        /*-------------重力逻辑-----------*/
        //着地时如果垂直速度为负，重置为一个小负值，保持贴地
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //v = v0 + gt
        velocity.y += gravity * Time.deltaTime;
        //v = gt平方
        controller.Move(velocity * Time.deltaTime);
        /*------------------------------------*/
    }
}
