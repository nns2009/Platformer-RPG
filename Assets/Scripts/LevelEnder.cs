using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnder : MonoBehaviour
{
    public GameObject ToActivate;
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<HeroController>() != null)
        {
            Debug.Log("Changing levels!");

            if (ToActivate != null)
            {
                ToActivate.SetActive(true);
            }

            if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCount)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

        }
    }
}
