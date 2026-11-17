using Patterns.Observer.Struct_EventManager;
using UnityEngine;

public class FOV : MonoBehaviour
{
    [SerializeField] GameObject[] _viewObjects;
    [SerializeField] float _range = 1;

    [SerializeField] float _viewAngle = 90;
    [SerializeField] LayerMask _wallLayer;

    [SerializeField, Min(0f)]
    [Tooltip("Distancia desde el centro hasta cada rayo lateral, en unidades del mundo.")]
    private float _lineOfSightOffset = 0.5f;

    private Transform _lineOfSightTarget;

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
        _lineOfSightTarget = target != null ? target.transform : null;
        if (_lineOfSightTarget == null) return false;

        Vector3 dir = target.transform.position - transform.position;
        dir.z = 0f;

        if (dir.magnitude >= _range) return false;
        if (!ignoreAngle && Vector2.Angle(transform.up, dir) >= _viewAngle / 2)
            return false;

        return CheckSideRays(dir);
    }

    private bool CheckSideRays(Vector3 dir, bool drawGizmos = false)
    {
        float distance = dir.magnitude;
        if (distance <= 0.00001f) return true;

        Vector3 direction = dir / distance;
        // Perpendicular al trayecto en XY: los dos rayos conservan su separacion.
        Vector3 side = new Vector3(-direction.y, direction.x, 0f) * _lineOfSightOffset;
        Vector3 leftOrigin = transform.position + side;
        Vector3 rightOrigin = transform.position - side;

        bool leftBlocked = Physics.Raycast(leftOrigin, direction, distance, _wallLayer);
        bool rightBlocked = Physics.Raycast(rightOrigin, direction, distance, _wallLayer);

        if (drawGizmos)
        {
            Gizmos.color = leftBlocked ? Color.red : Color.green;
            Gizmos.DrawLine(leftOrigin, leftOrigin + dir);
            Gizmos.color = rightBlocked ? Color.red : Color.green;
            Gizmos.DrawLine(rightOrigin, rightOrigin + dir);
        }

        return !leftBlocked && !rightBlocked;
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
        Color previousColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _range);

        Vector3 lineA = GetVectorFromAngle(_viewAngle / 2 + transform.eulerAngles.z);
        Vector3 lineB = GetVectorFromAngle(-_viewAngle / 2 + transform.eulerAngles.z);

        Gizmos.DrawLine(transform.position, transform.position + lineA * _range);
        Gizmos.DrawLine(transform.position, transform.position + lineB * _range);

        // muestra el ultimo destino consultado por LineOfSight
        // Sin destino, previsualizar los offsets hacia adelante.
        Vector3 dir = _lineOfSightTarget != null
            ? _lineOfSightTarget.position - transform.position
            : transform.up * _range;
        dir.z = 0f;
        CheckSideRays(dir, drawGizmos: true);
        Gizmos.color = previousColor;
    }
}
