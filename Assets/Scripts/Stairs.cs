using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    GameObject player;
    [SerializeField] GameObject dungeonFloor;
    DungeonManager generatedFloor;
    [HideInInspector] public DungeonManager upperFloor;
    bool generated = false;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 30f)
        {
            if (!generated) GenerateFloor();
            else if (Mathf.Abs(player.transform.position.y - transform.position.y) <= 1.5f && upperFloor != null) upperFloor.gameObject.SetActive(true);
            else generatedFloor.gameObject.SetActive(true);
        }
    }

    void GenerateFloor()
    {
        generatedFloor = Instantiate(dungeonFloor, transform.position, Quaternion.identity).GetComponent<DungeonManager>();
        generatedFloor.upstairs = gameObject;
        generated = true;
    }
}
