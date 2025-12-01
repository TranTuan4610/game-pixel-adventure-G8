using UnityEngine;

public class ChickenMove : MonoBehaviour
{
    public float moveDistance = 200f; // khoảng cách di chuyển (pixel)
    public float moveSpeed = 2f;      // tốc độ di chuyển
    private RectTransform rectTransform;
    private Vector2 startPos;
    private bool movingRight = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime * 100; // chuyển đổi sang pixel
        Vector2 pos = rectTransform.anchoredPosition;

        // Di chuyển sang trái hoặc phải
        if (movingRight)
            pos.x += step;
        else
            pos.x -= step;

        rectTransform.anchoredPosition = pos;

        // Khi đến giới hạn thì đổi hướng + lật hình
        if (movingRight && pos.x > startPos.x + moveDistance)
        {
            movingRight = false;
            Flip(false);
        }
        else if (!movingRight && pos.x < startPos.x - moveDistance)
        {
            movingRight = true;
            Flip(true);
        }
    }

    // Hàm lật nhân vật
    void Flip(bool facingRight)
    {
        Vector3 scale = rectTransform.localScale;
        scale.x = facingRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        rectTransform.localScale = scale;
    }
}
