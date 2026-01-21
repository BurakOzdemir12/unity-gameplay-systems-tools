using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Systems._Core.Field_of_View
{
    [ExecuteInEditMode]
    public class FieldOfView : MonoBehaviour
    {
        #region With Mesh => Attributes

        [SerializeField] private float distance;
        [SerializeField] private float angle;
        [SerializeField] private float height;
        [SerializeField] private Color meshColor = Color.red;

        [SerializeField] private float scanFrequency;
        [SerializeField] private LayerMask sensorMask;
        [SerializeField] private LayerMask occlusionLayers;

        public List<GameObject> Targets
        {
            get
            {
                targets.RemoveAll(obj => !obj);
                return targets;
            }
        }

        private List<GameObject> targets = new List<GameObject>();

        [SerializeField] private Collider[] detectedColliders = new Collider[10];
        public Collider[] DetectedColliders => detectedColliders;
        private int count;
        private float scanInterval;
        private float scanTimer;

        Mesh mesh;

        #endregion

        private void Start()
        {
            scanInterval = 1f / scanFrequency;
        }

        private void Update()
        {
            #region With Mesh

            scanTimer -= Time.deltaTime;
            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                Scan();
            }

            #endregion
        }


        #region Line Of Sight With mesh

        private void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, distance, detectedColliders, sensorMask,
                QueryTriggerInteraction.Ignore);
            targets.Clear();
            for (int i = 0; i < count; ++i)
            {
                GameObject targetObj = detectedColliders[i].gameObject;
                if (IsInSight(targetObj))
                {
                    targets.Add(targetObj);
                }
            }
        }

        public bool IsInSight(GameObject target)
        {
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
            // left side triangle
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;
            // right side triangle
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;
            for (int i = 0; i < segments; ++i)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topRight = bottomRight + Vector3.up * height;
                topLeft = bottomLeft + Vector3.up * height;

                // far side triangle
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;
                // top  triangle

                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;
                // bottom triangle
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;
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

        private void OnValidate()
        {
            scanInterval = 1f / scanFrequency;

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
            for (int i = 0; i < count; ++i)
            {
                Gizmos.DrawSphere(detectedColliders[i].transform.position, 0.2f);
            }

            Gizmos.color = Color.green;
            foreach (var target in Targets)
            {
                Gizmos.DrawSphere(target.transform.position, 0.2f);
            }
        }

        #endregion
    }
}