using UnityEngine;
using System.Collections;

public class DinnerWheelController : MonoBehaviour
{
    [Header("輪盤旋轉設定")]
    public float maxSpeed = 1000f;       // 最高轉速
    public float acceleration = 600f;    // 加速度
    public float deceleration = 300f;    // 減速度

    [Header("音效設定")]
    public AudioSource spinAudio;        // 輪盤轉動音效

    private float currentSpeed = 0f;     // 當前速度
    private bool isSpinning = false;     // 判斷是否正在旋轉中

    void Update()
    {
        // 1. 偵測電腦端輸入：空白鍵或滑鼠左鍵
        bool isInputTriggered = Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);

        // 2. 偵測手機端觸控：至少有一根手指觸碰螢幕，且狀態是「剛按下的瞬間」
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isInputTriggered = true;
        }

        // 3. 如果有觸發輸入，且輪盤沒有在轉動，就開始抽獎
        if (isInputTriggered && !isSpinning)
        {
            StartCoroutine(SpinWheel());
        }

        // 4. 如果目前速度大於 0，就讓物件旋轉 (負號代表順時針繞 Z 軸轉)
        if (currentSpeed > 0)
        {
            transform.Rotate(0, 0, -currentSpeed * Time.deltaTime);
        }
    }

    IEnumerator SpinWheel()
    {
        isSpinning = true;

        // 開始播放音效
        if (spinAudio != null && !spinAudio.isPlaying)
        {
            spinAudio.Play();
        }

        // 階段一：加速旋轉
        while (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            yield return null; // 等待下一個影格，讓畫面平滑過渡
        }
        currentSpeed = maxSpeed;

        // 階段二：維持最高速轉動 (加入隨機時間 1.5 ~ 4 秒，確保每次停下來的晚餐都不一樣)
        float randomSpinTime = Random.Range(1.5f, 4.0f);
        yield return new WaitForSeconds(randomSpinTime);

        // 階段三：減速停止
        while (currentSpeed > 0)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            yield return null;
        }

        // 確保速度歸零，完全停止
        currentSpeed = 0f;
        isSpinning = false;

        // 停止播放音效
        if (spinAudio != null)
        {
            spinAudio.Stop();
        }
    }
}