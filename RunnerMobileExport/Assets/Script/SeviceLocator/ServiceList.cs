using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceList : MonoBehaviour
{

    public List<string> serviceName = new();

    public void Start()
    {
        serviceName = ServiceListNames.TypeNames;
    }
    /*
    public IEnumerator Updaters()
    {
        while (true)
        {
            serviceName = ServiceListNames.TypeNames;
            yield return new WaitForSeconds(2f); 
        }
    }
    */
}

public static class ServiceListNames
{
    public static List<string> TypeNames = new();
}
