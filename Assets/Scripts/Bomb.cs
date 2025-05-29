using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float m_minSpeed = 1.0f;
    public float m_maxSpeed = 2.0f;
    public GameObject m_explosion;

    float m_speed;
    LineRenderer m_line;
    List<Vector3> m_positions;
    Vector3 m_targetPos;
    City m_targetCity;

    // Start is called before the first frame update
    void Start()
    {
        m_speed = Random.Range(m_minSpeed, m_maxSpeed);
        m_line = GetComponent<LineRenderer>();
        if (null != m_line)
        {
            m_positions = new List<Vector3>();
            m_positions.Add(transform.position);
            m_line.SetPositions(m_positions.ToArray());
        }
        City[] cities = FindObjectsByType<City>(FindObjectsSortMode.None);
        m_targetCity = cities[Random.Range(0, cities.Length)];
        m_targetPos = m_targetCity.GetTargetPos();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        Vector3 vel = m_targetPos - pos;
        float len = vel.magnitude;
        if (len <= m_speed * Time.deltaTime)
        {   // explode
            transform.position = m_targetPos;
            if (null != m_targetCity)
            {
                m_targetCity.Explode();
            }
            Explode();
        }
        else
        {
            vel = vel * m_speed / len;
            pos += vel * Time.deltaTime;
            transform.position = pos;

            if (null != m_line)
            {
                m_positions.Add(transform.position);
                m_line.positionCount = m_positions.Count;
                m_line.SetPositions(m_positions.ToArray());
            }
        }
    }

    public void Intercept()
    {
        Explode();
    }

    void Explode()
    {
        if (null != m_explosion)
        {
            GameObject explode = Instantiate(m_explosion);
            explode.transform.position = transform.position;
        }
        Destroy(gameObject);
    }
}
