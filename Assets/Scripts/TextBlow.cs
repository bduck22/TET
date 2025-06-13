using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextBlow : MonoBehaviour
{
    Text text;
    public float TextblinkTime;
    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(Blink());
    }
    IEnumerator Blink()
    {
        while (true)
        {
            if (gameObject.activeSelf)
            {
                text.color = Color.white;
                yield return new WaitForSeconds(TextblinkTime);
                text.color *= 0;
                yield return new WaitForSeconds(TextblinkTime);
            }
            else break;
        }
    }
}
