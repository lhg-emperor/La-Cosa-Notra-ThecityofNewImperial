using UnityEngine;

public class BatScripts : MonoBehaviour
{
    public int attackValue = 10;  // Chỉ số tấn công của Bat

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log(message: "Mất");
        }
    }

}
