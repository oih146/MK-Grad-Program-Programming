using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMe : MonoBehaviour {

    [SerializeField]
    Transform m_objectToChild;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetPosition();
	}

    public void SetPosition()
    {
        m_objectToChild.transform.position = gameObject.transform.position;
    }
}
