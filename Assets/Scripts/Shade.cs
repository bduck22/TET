using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Shade : MonoBehaviour
{
    [SerializeField] bool tilemap;
    [SerializeField] bool sprite;
    [SerializeField] bool image;
    private void OnEnable()
    {
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        float a=1;
        while (true)
        {
            if(a <=0)
            {
                if (tilemap)
                {
                    GetComponent<Tilemap>().color = Color.white;
                }
                else if (sprite)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
                else if (image)
                {
                    GetComponent<Image>().color = Color.white;
                }
                break;
            }

            if (tilemap)
            {
                GetComponent<Tilemap>().color = Color.white - Color.black * a;
            }
            else if (sprite)
            {
                GetComponent<SpriteRenderer>().color = Color.white - Color.black * a;
            }
            else if (image)
            {
                GetComponent<Image>().color = Color.white - Color.black * a;
            }

            a -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
