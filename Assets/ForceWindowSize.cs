using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceWindowSize : MonoBehaviour
{
    // 老师注释：这是Unity的第一帧，游戏刚开始时运行
    void Start()
    {
        // 这里的 false 表示“不全屏”，也就是窗口模式
        // 如果你想保持原图大小，就写 1080, 1920
        // 但为了能在普通电脑屏幕上放下，老师建议用 540, 960 (正好是一半，比例不变)

        // 这一行代码的意思是：设置分辨率为宽540，高960，并且不要全屏(false)
        Screen.SetResolution(1080, 1920, false);

        // 如果你非要 1080x1920，就把上面那行注释掉，用下面这行：
        // Screen.SetResolution(1080, 1920, false);
    }
}