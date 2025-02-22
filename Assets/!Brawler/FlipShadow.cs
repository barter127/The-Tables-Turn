using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipShadow : MonoBehaviour
{
    [SerializeField] private NEWpunkAI ai;
    [SerializeField] private SpriteRenderer spr;

    private void Update()
    {
        if(ai.isFacingRight)
        {
            spr.flipX = true;
        }
        else
        {
            spr.flipX = false;
        }
    }
}
