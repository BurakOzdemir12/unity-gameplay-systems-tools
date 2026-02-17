using System.Collections.Generic;
using UnityEngine;

namespace _Project.Systems.PerceptionSystem.Field_of_View
{
    // [ExecuteInEditMode] 
    public class FieldOfView : MonoBehaviour
    {
        #region With Mesh => Attributes

        [Range(0, 50)]
        [SerializeField] private float distance = 10f;
        [Range(0, 360)]
        [SerializeField] private float angle = 45f;
        [SerializeField] private float height = 1.0f;
        [SerializeField] private Color meshColor = Color.red;

        [Range(0.1f, 10f)]
        [SerializeField] private float scanFrequency = 2f;
        [SerializeField] private LayerMask sensorMask;
        [SerializeField] private LayerMask occlusionLayers;

        public List<GameObject> Targets => targets;

        private readonly List<GameObject> targets = new List<GameObject>();

        [SerializeField] private Collider[] detectedColliders = new Collider[10];
        
        private int count;
        private float scanInterval;
        private float scanTimer;

        Mesh mesh;

        #endregion

        private void Start()
        {
            scanInterval = 1f / scanFrequency;
            mesh = CreateWedgeMesh();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                scanTimer -= Time.deltaTime;
                if (scanTimer < 0)
                {
                    scanTimer += scanInterval;
                    Scan();
                }
            }
        }

        #region Line Of Sight With mesh

        private void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, distance, detectedColliders, sensorMask,
                QueryTriggerInteraction.Ignore);
            
            targets.Clear(); 
            
            for (int i = 0; i < count; ++i)
            {
                if (detectedColliders[i] != null)
                {
                    GameObject targetObj = detectedColliders[i].gameObject;
                    if (IsInSight(targetObj))
                    {
                        targets.Add(targetObj);
                    }
                }
            }
        }

        private bool IsInSight(GameObject target)
        {
            if (target == null) return false;

            Vector3 origin = transform.position + Vector3.up * (height * 0.5f);
            Vector3 targetPos = target.transform.position;
            
            if (target.TryGetComponent<Collider>(out var col))
                targetPos = col.bounds.center;

            Vector3 dir = targetPos - origin;

            if (Mathf.Abs(dir.y) > height * 0.5f) return false;

            Vector3 dirFlat = Vector3.ProjectOnPlane(dir, Vector3.up);
            if (dirFlat.sqrMagnitude < 0.0001f) return true;

            float deltaAngle = Vector3.Angle(dirFlat, transform.forward);
            if (deltaAngle > angle) return false;

            Vector3 targetFlat = targetPos;
            targetFlat.y = origin.y;

            if (Physics.Linecast(origin, targetFlat, occlusionLayers))
                return false;

            return true;
        }

        Mesh CreateWedgeMesh()
        {
            Mesh wedgeMesh = new Mesh();
            wedgeMesh.name = "FOV_Mesh";
            
            int segments = 10;
            int numTriangles = (segments * 4) + 2 + 2;
            int numVertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;

            int vert = 0;
            
            // Mesh generation logic (Vertices assignments)...
            // Left side
            vertices[vert++] = bottomCenter; vertices[vert++] = bottomLeft; vertices[vert++] = topLeft;
            vertices[vert++] = topLeft; vertices[vert++] = topCenter; vertices[vert++] = bottomCenter;
            // Right side
            vertices[vert++] = bottomCenter; vertices[vert++] = topCenter; vertices[vert++] = topRight;
            vertices[vert++] = topRight; vertices[vert++] = bottomRight; vertices[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;
            
            for (int i = 0; i < segments; ++i)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topRight = bottomRight + Vector3.up * height;
                topLeft = bottomLeft + Vector3.up * height;

                // Far side
                vertices[vert++] = bottomLeft; vertices[vert++] = bottomRight; vertices[vert++] = topRight;
                vertices[vert++] = topRight; vertices[vert++] = topLeft; vertices[vert++] = bottomLeft;
                // Top
                vertices[vert++] = topCenter; vertices[vert++] = topLeft; vertices[vert++] = topRight;
                // Bottom
                vertices[vert++] = bottomCenter; vertices[vert++] = bottomRight; vertices[vert++] = bottomLeft;
                
                currentAngle += deltaAngle;
            }

            for (int i = 0; i < numVertices; ++i)
            {
                triangles[i] = i;
            }

            wedgeMesh.vertices = vertices;
            wedgeMesh.triangles = triangles;
            wedgeMesh.RecalculateNormals();

            return wedgeMesh;
        }

        private void OnEnable()
        {
            if(mesh == null) mesh = CreateWedgeMesh();
        }

        private void OnDisable()
        {
            if (mesh != null)
            {
                if (Application.isPlaying) Destroy(mesh);
                else DestroyImmediate(mesh);
            }
        }

        private void OnValidate()
        {
            if (scanFrequency <= 0) scanFrequency = 0.1f;
            scanInterval = 1f / scanFrequency;

            if (mesh != null)
            {
                if (Application.isPlaying) Destroy(mesh);
                else DestroyImmediate(mesh);
            }
            mesh = CreateWedgeMesh();
        }

        private void OnDrawGizmos()
        {
            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }

            Gizmos.DrawWireSphere(transform.position, distance);
            
            if (detectedColliders != null)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (detectedColliders[i] != null) 
                    {
                        Gizmos.DrawSphere(detectedColliders[i].transform.position, 0.2f);
                    }
                }
            }

            Gizmos.color = Color.green;
            if (targets != null)
            {
                foreach (var target in targets)
                {
                    if (target != null)
                        Gizmos.DrawSphere(target.transform.position, 0.2f);
                }
            }
        }
        #endregion
    }
}