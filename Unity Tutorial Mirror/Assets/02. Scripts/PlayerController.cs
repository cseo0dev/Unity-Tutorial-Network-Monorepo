using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    void Update()
    {
        if (isLocalPlayer)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            var dir = new Vector3(h, v, 0);
            dir.Normalize();

            transform.position += dir * 2f * Time.deltaTime;
        }
    }
}
