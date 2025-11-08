using UnityEngine;

public class GetSomething : QuestStep
{
    [Header("Item to Collect")]
    public GameObject itemPrefab;   // Prefab vật cần nhặt
    public Transform point;          // Nơi vật xuất hiện
    private GameObject spawnedItem;
    private bool itemCollected = false;

    void OnEnable()
    {
        if (itemPrefab != null && point != null)
        {
            // Spawn vật thể tại điểm đã chỉ định, làm con của point
            spawnedItem = Instantiate(itemPrefab, point.position, Quaternion.identity, point);
            spawnedItem.name = itemPrefab.name;

            if (spawnedItem.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D col = spawnedItem.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
            }

            Debug.Log("" + "QuestStep started: Nhặt vật " + spawnedItem.name);
            Debug.Log("" + "Item Position (World): " + spawnedItem.transform.position);
        }
        else
        {
            Debug.LogWarning("" + "ItemPrefab hoặc Point chưa được gán!");
        }
    }

    protected override void CheckStep()
    {
        if (itemCollected || spawnedItem == null) return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (spawnedItem == null || itemCollected) return;
        if (!other.CompareTag("Player")) return;

        itemCollected = true;
        Destroy(spawnedItem);
        Debug.Log("" + "Vật " + itemPrefab.name + " đã được nhặt!");
        CompleteStep();
    }
}
