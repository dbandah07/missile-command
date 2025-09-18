using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireControl : MonoBehaviour
{
    public GameObject m_crossHair;
    public GameObject m_missile;
    public GameObject m_bomb;
    public GameObject m_city;
    public int m_numCity = 3;
    public float m_bombMinTime = 0.25f;
    public float m_bombMaxTime = 1.0f;
    public GameObject m_pauseMenu;

    List<GameObject> m_targets;     // a list of all the targeting reticles (one for each finger that's touching the screen)
    List<City> m_cities;

    bool m_isPaused = false;

    // TODO create class MissileInput

    public class MissileInput
    {
        public  List<Vector3> pos_aiming = new List<Vector3>(); // aiming
        public List<Vector3> pos_firing = new List<Vector3>(); // firing

        public static MissileInput ReadInput()
        {
            MissileInput input = new MissileInput();

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) // if true
            {
                Vector3 pos = Input.mousePosition;
                Vector3 world_pos = Utility.ScreenToWorldPos(pos);

                input.pos_aiming.Add(world_pos); // adding to vector
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Vector3 pos = Input.mousePosition;
                pos = Utility.ScreenToWorldPos(pos);
            }

            return input;
        }
    }


    private void Start()
    {
        m_targets = new List<GameObject>();
        m_cities = new List<City>();
        for (int i = 0; i < m_numCity; ++i)
        {
            GameObject obj = Instantiate(m_city);
            obj.name = "City_" + i.ToString();
            float x = 0.8f * i / m_numCity + 0.2f;
            Vector3 pos = new Vector3(x, 0.0f, 0.0f);
            pos = Camera.main.ViewportToScreenPoint(pos);
            pos = Utility.ScreenToWorldPos(pos);
            obj.transform.position = pos;
            m_cities.Add(obj.GetComponent<City>());
        }
        SetPause(false);
        StartCoroutine(DropBomb());
    }

    void Update()
    {
        if (false == m_isPaused)
        {
            // TODO Read the input
            FireControl.MissileInput input = MissileInput.ReadInput(); // static belongs to class itself. call it w/o creating a new isntance

            if (input.pos_aiming.Count > 0)
            {
                Debug.Log("Aiming towards: " + input.pos_aiming[0]);
            }

            if (input.pos_firing.Count > 0)
            {
                Debug.Log("Fired missele at: " + input.pos_firing[0]);
            }


            // TODO Fire Missiles

            // TODO Make enough target reticles
            // Delete any extra target reticles

            // TODO Update the position of all the target reticles
        }

        // keys
        if (Input.GetKeyDown(KeyCode.Escape))
        {   // this doubles as the option key in the android navigation bar
            SetPause(!m_isPaused);
        }
    }

    void FireMissile(Vector3 targetPos)
    {
        City closest = null;
        float bestDist = float.MaxValue;
        foreach (City city in m_cities)
        {
            if (city.CanLaunch())
            {
                float dist = Vector3.Distance(targetPos, city.transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    closest = city;
                }
            }
        }

        if (null != closest)
        {
            GameObject gameObject = Instantiate(m_missile);
            gameObject.transform.position = closest.GetLaunchPos();
            Missile missile = gameObject.GetComponent<Missile>();
            if (null != missile)
                missile.Fire(targetPos);
        }
    }

    IEnumerator DropBomb()
    {
        int numCity = m_cities.Count;
        while (numCity > 0)
        {
            float delay = Random.Range(m_bombMinTime, m_bombMaxTime);
            yield return new WaitForSeconds(delay);
            if (null != m_bomb)
            {
                Vector3 pos = new Vector3(Random.Range(0.2f, 0.8f), 1.0f, 0.0f);
                pos = Camera.main.ViewportToScreenPoint(pos);
                pos = Utility.ScreenToWorldPos(pos);
                GameObject bomb = Instantiate(m_bomb);
                bomb.transform.position = pos;
            }
            numCity = 0;
            foreach (City city in m_cities)
            {
                if (city.IsAlive())
                    ++numCity;
            }
        }

        // We've run out of cities
        // Wait for the bombs to run out
        while (null != FindFirstObjectByType<Bomb>())
            yield return null;

        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {
        // wait 3 seconds
        yield return new WaitForSecondsRealtime(3.0f);
        // and reload the scene
        SceneManager.LoadScene(0);
    }

    public void SetPause(bool setPause)
    {
        if (setPause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
        m_pauseMenu.SetActive(setPause);
        m_isPaused = setPause;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // TODO Create function ReadInput()
}
