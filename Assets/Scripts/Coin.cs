using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class Coin : MonoBehaviour
{
    private Animator anim;
    private bool collected = false;
    public float destroyDelay = 0.4f; // thời gian sau khi anim chạy
    public bool enableRotation = true; // có xoay coin không
    public float rotationSpeed = 180f; // tốc độ xoay

    private void Awake()
    {
        anim = GetComponent<Animator>();
        GetComponent<Collider2D>().isTrigger = true;
    }


    // Gọi từ PlayerCollision khi player ăn coin
    public void Collect()
    {
        if (collected) return; // tránh ăn 2 lần
        collected = true;

        // Gọi animation "Collected"
        if (anim) anim.SetTrigger("Collected");

        // Tắt Collider để không va chạm lại
        GetComponent<Collider2D>().enabled = false;

        // Xoá coin sau khi anim xong
        Destroy(gameObject, destroyDelay);
    }

    // Gọi từ Animation Event khi animation hoàn thành (optional)
    public void OnCollectedAnimationFinished()
    {
        // Có thể add thêm effect ở đây
        // Ví dụ: particle effect, sound, etc.

        Destroy(gameObject);
    }
}
