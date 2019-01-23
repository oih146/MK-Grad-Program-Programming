using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    [SerializeField]
    Projectile m_sourceObject;
    List<Projectile> m_objectsHeld;

    [SerializeField]
    int m_amountToCreate;

    // Use this for initialization
    void Start()
    {
        m_objectsHeld = new List<Projectile>();

        for (int i = 0; i < m_amountToCreate; i++)
        {
            m_objectsHeld.Add(Instantiate(m_sourceObject, transform));
            m_objectsHeld[i].transform.position = Vector3.zero;
            m_objectsHeld[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        for(int i = 0; i < m_objectsHeld.Count; i++)
        {
            if(m_objectsHeld[i].InUse)
            {
                m_objectsHeld[i].Recollect();
            }
        }
    }

    public Projectile GetFreeObject()
    {
        //Check to see if we have an object not being used
        for(int i = 0; i < m_objectsHeld.Count; i++)
        {
            if(!m_objectsHeld[i].InUse)
            {
                m_objectsHeld[i].gameObject.SetActive(true);
                m_objectsHeld[i].InUse = true;
                return m_objectsHeld[i];
            }
        }

        //If none of the objects are available, create a new object
        Projectile temp = Instantiate(m_sourceObject, transform);
        temp.gameObject.SetActive(true);
        m_objectsHeld.Add(temp);
        m_amountToCreate++;
        temp.InUse = true;
        return temp;
    }
}
