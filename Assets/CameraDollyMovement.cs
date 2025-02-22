using UnityEngine;

public class CameraDollyMovement : MonoBehaviour
{
    [SerializeField] private Transform player;

    // Restrict camera to certain bounds of the level.
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;
    [SerializeField] private float offsetZ;

    [SerializeField] private Vector2 xClamp;
    [SerializeField] private Vector2 zClamp;

    [SerializeField] private float moveSpeed;

    public bool lockCamera;

    void Update()
    {
        if (!lockCamera)
        {
            Vector3 distanceVector = new Vector3(transform.position.x - player.transform.position.x, transform.position.y, transform.position.z - player.transform.position.z);

            if (distanceVector.x < minX || distanceVector.x > maxX)
            {
                float targetX = Mathf.Clamp(player.transform.position.x, xClamp.x, xClamp.y);

                //Lerp till out of range.
                transform.position = Vector3.Lerp(transform.position, new Vector3(targetX, transform.position.y, transform.position.z), Time.deltaTime * moveSpeed);
            }

            if (distanceVector.z < minZ || distanceVector.z > maxZ)
            {
                float targetZ = Mathf.Clamp(player.transform.position.z, zClamp.x, zClamp.y);

                //Lerp till out of range.
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, targetZ + offsetZ), Time.deltaTime * moveSpeed);
            }
        }            
        }

}
