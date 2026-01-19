using UnityEngine;

public class KeyboardTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E НАЖАТА");
        }

        if (Input.anyKeyDown)
        {
            Debug.Log("ЛЮБАЯ КЛАВИША");
        }
    }
}