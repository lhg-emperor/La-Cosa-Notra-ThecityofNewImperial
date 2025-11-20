using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;     // Kéo thả Player từ Inspector (hoặc tự Find)
    [SerializeField] private Slider slider;     // Kéo thả Slider con trong Inspector

    private void Awake()
    {
        // Nếu không gán, cố tìm thêm tại runtime
        if (player == null)
            player = Object.FindFirstObjectByType<Player>();

        if (slider == null)
            slider = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        // Thiết lập thang max dựa trên giá trị khởi tạo Health
        slider.maxValue = player.Health;
        slider.value = player.Health;
    }

    private void Update()
    {
        // Cứ mỗi frame, đồng bộ thanh máu với Player.Health
        slider.value = player.Health;
    }
}