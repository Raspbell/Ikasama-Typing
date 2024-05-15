using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void Clicked(){
        GameManager.money += 10000000;
    }
}
