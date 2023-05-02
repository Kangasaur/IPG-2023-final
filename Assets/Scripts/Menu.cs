using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    bool started = false;

    private void Start()
    {
        Invoke("Startup", 0.25f);
    }
    void Update()
    {
        if (started)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

            else if (Input.anyKeyDown)
            {
                DungeonManager.dungeonFloors = new List<DungeonManager>();
                SceneManager.LoadScene("Dungeon");
            }
        }
    }

    void Startup()
    {
        started = true;
    }
}
