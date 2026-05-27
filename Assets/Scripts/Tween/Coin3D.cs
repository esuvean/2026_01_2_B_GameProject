using UnityEngine;
using DG.Tweening;

public class Coin3D : MonoBehaviour
{
    public int moneyAmount = 10;

    private bool isPicked = false;

    void Start()
    {
        Vector3 randomPosition =
            transform.position +
            new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            );

        transform.DOJump(randomPosition, 1.2f, 1, 0.4f)
            .SetLink(gameObject);

        transform.DORotate(
            new Vector3(0f, 360f, 0f),
            0.4f,
            RotateMode.FastBeyond360
        )
        .SetLink(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPicked)
            return;

        if (other.CompareTag("Player"))
        {
            isPicked = true;

            MoneyUI moneyUI =
                Object.FindFirstObjectByType<MoneyUI>();

            if (moneyUI != null)
            {
                moneyUI.GetMoney(
                    moneyAmount,
                    transform.position
                );
            }

            transform.DOKill();

            Destroy(gameObject);
        }
    }
}