using UnityEngine;

public class ShadowFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerController rb;
    [SerializeField] private SpriteRenderer spr;

    private void Update()
    {
        //Keep the shadow's y-position constant and match the player's x-position.
        transform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);

        if (rb.isFacingRight)
        {
            spr.flipX = false;
        }
        else
        {
            spr.flipX = true;
        }
    }
}
