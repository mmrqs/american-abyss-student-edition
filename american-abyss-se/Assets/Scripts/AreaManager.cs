using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaManager : MonoBehaviour
{
    public static string selectedObject;
    public string internalObject;
    public RaycastHit theObject;
    private Camera cam;

    [FormerlySerializedAs("changing")] public bool flashing = false;
    private GameObject current;
    private Color currentColor;
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out theObject))
        {
            if (current == null || !current.Equals(theObject.transform.gameObject))
            {
                if(current != null)
                    current.transform.gameObject.GetComponent<Renderer>().material.color = currentColor;
                current = theObject.transform.gameObject;
                currentColor = current.transform.gameObject.GetComponent<Renderer>().material.color;

            }
            theObject.transform.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));
        }
    }
}
