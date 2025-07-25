using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opening : MonoBehaviour
{
    public AK.Wwise.Event Open;


    public void PlayOpen()
    {
        Open.Post(gameObject);
    }
}
