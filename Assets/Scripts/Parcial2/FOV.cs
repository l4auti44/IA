using Patterns.Observer.Struct_EventManager;
using UnityEngine;

public class FOV : MonoBehaviour
{
    [SerializeField] GameObject[] _viewObjects;
    [SerializeField] float _range = 1;

    [SerializeField] float _viewAngle = 90;
    [SerializeField] LayerMask _wallLayer;

    public LayerMask WallLayer { get { return _wallLayer; } }

    private bool _isPlayerInFOV = false;

    public Transform target;
    public event System.Action<bool> OnPlayerDetected;

    void Update()
    {
        FieldOfView();
    }

    public void FieldOfView()
    {
        foreach (GameObject obj in _viewObjects)
        {
            Vector3 dir = obj.transform.position - transform.position;
            dir.z = 0f;

            if (dir.magnitude < _range)
            {
                //obj.GetComponent<Renderer>().material.color = Color.white;
                //esta en el posible rango de vision y veo si esta dentro del ANGULO de vision.
                if (Vector2.Angle(transform.up, dir) < _viewAngle / 2)
                {
                    if (!Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, _wallLayer))//LineOfSight, si no choco con la pared
                    {
                        Debug.DrawLine(transform.position, transform.position + dir, Color.white);
                        //obj.GetComponent<Renderer>().material.color = Color.blue;
                        if (!_isPlayerInFOV)
                        {
                            _isPlayerInFOV = true;
                            target = obj.transform;
                            Node nearestNode;
                            GameManager.Instance.FindNearestNode(target.position, out nearestNode);
                            OnPlayerDetected?.Invoke(true);
                            EventManager.Raise<PlayerFoundData>(new PlayerFoundData(nearestNode));

                        }
                        continue;
                    }
                    else
                    {
                        //choca con la apred y dibujo una linea roja
                        Debug.DrawLine(transform.position, hit.point, Color.red);
                        if (_isPlayerInFOV)
                        {
                            _isPlayerInFOV = false;
                            target = null;
                            OnPlayerDetected?.Invoke(false);
                        }
                    }
                }
            }
            else
            {
                if (_isPlayerInFOV)
                {
                    _isPlayerInFOV = false;
                    target = null;
                    OnPlayerDetected?.Invoke(false);
                }
                //no esta en el rango de posible vision
                //obj.GetComponent<Renderer>().material.color = Color.black;
            }


        }
    }

    public bool LineOfSight(GameObject target, bool ignoreAngle = false)
    {
        Vector3 dir = target.transform.position - transform.position;
        dir.z = 0f;

        if (dir.magnitude < _range)
        {
            //esta en el posible rango de vision y veo si esta dentro del ANGULO de vision.
            if (ignoreAngle || Vector2.Angle(transform.up, dir) < _viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dir, out RaycastHit hit, dir.magnitude, _wallLayer))//LineOfSight, si no choco con la pared
                {
                    Debug.DrawLine(transform.position, transform.position + dir, Color.white);
                    //obj.GetComponent<Renderer>().material.color = Color.blue;
                    return true;
                }
                else
                {
                    //choca con la apred y dibujo una linea roja
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    return false;
                }
            }
            return false;
        }
        else
        {
            //no esta en el rango de posible vision
            return false;
        }
    }


    Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(
            -Mathf.Sin(angle * Mathf.Deg2Rad),
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _range);

        Vector3 lineA = GetVectorFromAngle(_viewAngle / 2 + transform.eulerAngles.z);
        Vector3 lineB = GetVectorFromAngle(-_viewAngle / 2 + transform.eulerAngles.z);

        Gizmos.DrawLine(transform.position, transform.position + lineA * _range);
        Gizmos.DrawLine(transform.position, transform.position + lineB * _range);
    }
}
