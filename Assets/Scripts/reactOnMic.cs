using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleFromMic : MonoBehaviour
{

    public AudioSource source;
    public Vector3 minScale;
    public Vector3 maxScale;
    public loudnessDetection detector;
    public bool ifClicked = false;

 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.X)){
            ifClicked = !ifClicked;
        }   
        if(ifClicked == true){
            float loudness = detector.getLoudnessFromMic();
            transform.localScale = Vector3.Lerp(minScale, maxScale, loudness);
        }
    }
}