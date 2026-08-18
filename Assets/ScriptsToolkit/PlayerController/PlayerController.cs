using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float walkSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.8f;  //重力要让角色往下掉，就要用负数（Y轴向上为正，Y轴向下为负数）

    [Header("视角")]
    public float mouseSensitivity = 100f;
    //凡是需要“移动”“拖动”“改变位置”的，用Transform比较合适
    public Transform playerCamera;       //拖Main Camera进来
    private CharacterController controller;
    private Vector3 velocity;     //垂直速度（重力/跳跃）用的最多的是velocity.y用来记录Y轴下落速度
    private float xRotation = 0f; //用于限制上下视角旋转的角度

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;       //开局就锁定鼠标
    }

    void Update()
    {
        //鼠标视角
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //上下看：限制在 -90° 到 90° 之间，防止翻过头
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //左右看：旋转整个玩家（这样移动方向也会跟着转）
        transform.Rotate(Vector3.up * mouseX);

        //WASD移动
        float x = Input.GetAxis("Horizontal");  //AD
        float z = Input.GetAxis("Vertical");    //WS

        //移动方向基于玩家朝向
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        //跳跃+重力
        if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;   //贴地
        }
        if(Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //按Esc键解锁鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
