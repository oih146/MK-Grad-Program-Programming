using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//More of a placemant script than an enemy spawner
public class Spawner : MonoBehaviour {

    [System.Serializable]
	struct SpawnObject
    {
        [SerializeField]
        bool m_spawnImmediately;
        public bool SpawnImmidiately { get { return m_spawnImmediately; } }

        [SerializeField]
        float m_spawnInterval;
        public float SpawnInterval { get { return m_spawnInterval; } }
        public float IntervalTimer { get; set; }

        [SerializeField]
        int m_spawnAmount;
        public int SpawnAmount { get { return m_spawnAmount; } set { m_spawnAmount = value; } }

        [SerializeField]
        GameObject m_spawn;
        public GameObject Spawn { get { return m_spawn; } }

    }

    [SerializeField]
    List<SpawnObject> m_objectsToSpawn;

    BoxCollider m_collider;

    private void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        foreach(SpawnObject so in m_objectsToSpawn)
        {
            SpawnObject spawn = so;
            spawn.IntervalTimer = spawn.SpawnInterval;
            if(so.SpawnImmidiately)
            {
                for(int i = 0; i < so.SpawnAmount; i++)
                {
                    //Create an object and place randomly inside the collider area
                    GameObject temp = Instantiate(so.Spawn, gameObject.transform, true);
                    temp.transform.position = m_collider.RandomPointInBox();
                }
            }
        }
    }

    private void Update()
    {
        //If we go through a loop without spawning anything or decrementing a timer
        bool m_haveSpawnedAll = true;

        for(int i = 0; i < m_objectsToSpawn.Count; i++)
        {
            if(!m_objectsToSpawn[i].SpawnImmidiately && m_objectsToSpawn[i].SpawnAmount > 0)
            {
                m_haveSpawnedAll = false;

                SpawnObject so = m_objectsToSpawn[i];

                so.IntervalTimer -= Time.deltaTime;

                if (so.IntervalTimer <= 0)
                {
                    GameObject temp = Instantiate(so.Spawn, gameObject.transform, true);
                    temp.transform.position = m_collider.RandomPointInBox();
                    //Decrement the amount we have left to spawn
                    so.SpawnAmount--;
                    so.IntervalTimer = so.SpawnInterval;
                }

                m_objectsToSpawn[i] = so;

            }
        }

        //Turn off this spawner
        if (m_haveSpawnedAll)
            enabled = false;
    }
}
