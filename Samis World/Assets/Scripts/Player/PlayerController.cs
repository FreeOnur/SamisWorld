using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerCombat playerCombat;
    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerCombat.Attack(1);
        }
    }
}
