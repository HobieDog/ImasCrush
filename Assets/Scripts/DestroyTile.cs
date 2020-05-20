using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTile : MonoBehaviour
{
    const float decrese = 0.15f;
    const float time = 0.02f;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    IEnumerator Destroy()
    {
        spriteRenderer.enabled = true;

        Color color = spriteRenderer.color;
        while (color.a > 0)
        {
            color.a -= decrese;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(time);
        }
        Destroy(transform.parent.gameObject);
    }
}
