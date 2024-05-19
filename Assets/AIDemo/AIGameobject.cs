using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class AIGameobject: MonoBehaviour
{
    public GameObject player;
    public GameObject m_enemy_1;
    public GameObject m_enemy_2;
    public GameObject m_obstacle;

    public float m_speed = 5f;

    private NavMeshAgent agent;
    private NavMeshAgent agent_enemy_1;
    private NavMeshAgent agent_enemy_2;

    void Start()
    {
        agent = player.gameObject.GetComponent<NavMeshAgent>();
        agent_enemy_1 = m_enemy_1.gameObject.GetComponent<NavMeshAgent>();
        agent_enemy_2 = m_enemy_2.gameObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //PlayerMove();
        ScreenPlayerMove();
    }

    void PlayerMove()
    {
        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow)) 
        {
            player.transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow)) 
        {
            player.transform.Translate(Vector3.forward * -m_speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) 
        {
            player.transform.Translate(Vector3.right * -m_speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) 
        {
            player.transform.Translate(Vector3.right * m_speed * Time.deltaTime);
        }
    }

    async void ScreenPlayerMove()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray,out hit)) 
            {
                agent.isStopped = false;
                agent.SetDestination(hit.point);

                EnemyMove();
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            agent.isStopped = true;
        }
    }

    void EnemyMove()
    {
        agent_enemy_1.SetDestination(player.transform.position);
        agent_enemy_2.SetDestination(player.transform.position);
    }
}
