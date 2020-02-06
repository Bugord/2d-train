using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudVariables : MonoBehaviour
{
    //0 is Highscore
    //1 is coins 
    //2 is AdsRemoved
    //3 skins status should look like int number, but will be used as bin to identify unlocked skins (10000101010010)
    //4 is LastScore
    public static int[] ImportantValues = new int[5];

}
