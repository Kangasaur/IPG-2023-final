using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] Image hurtImage;
    [SerializeField] TextMeshProUGUI questText;
    public int enemies;
    [HideInInspector] public bool enemiesDead;

    float alpha = 0;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        hurtImage.color = new Color(1, 0, 0, alpha);
        if (alpha > 0)
        {
            alpha -= Time.deltaTime * 2f;
            if (alpha <= 0) alpha = 0;
        }
    }

    public void Hurt()
    {
        alpha = 0.5f;
    }

    public void KillEnemy()
    {
        enemies--;
        Debug.Log("Dead");
        if (enemies == 0)
        {
            enemiesDead = true;
            questText.text = "All enemies eliminated. Exit the dungeon";
        }
    }
}
