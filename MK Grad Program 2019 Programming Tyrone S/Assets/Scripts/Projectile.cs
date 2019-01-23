using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField]
    float m_aliveTime;
    float m_timerAlive;

    public bool InUse;

    private void Awake()
    {
        m_timerAlive = m_aliveTime;
    }


    //Could go in update, but it needs to be tracked
    //Reduced amount of update calls
    public void Recollect()
    {
        m_timerAlive -= Time.deltaTime;

        if (m_timerAlive <= 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
            m_timerAlive = m_aliveTime;
            gameObject.transform.localPosition = Vector3.zero;
            InUse = false;
        }
    }

}
