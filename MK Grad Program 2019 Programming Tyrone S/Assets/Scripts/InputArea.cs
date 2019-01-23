using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputArea : MonoBehaviour {

    [SerializeField]
    RectTransform m_stick;

    [SerializeField]
    Collider m_restrictArea;
    public Collider RestrictArea { get { return m_restrictArea; } }

    Vector3 m_stickResetPosition;

    public bool HasTakenInput { get; set; }

    private void Start()
    {
        m_stickResetPosition = m_stick.position;
    }

    public Vector3 HandleInput(Vector3 position)
    {
        m_stick.position = m_restrictArea.ClosestPoint(position);
        Vector3 temp = m_stick.position - m_restrictArea.gameObject.transform.position;

        return new Vector3(temp.x, 0, temp.y);
    }

    public void ResetInput()
    {
        m_stick.localPosition = Vector3.zero;
    }

}
