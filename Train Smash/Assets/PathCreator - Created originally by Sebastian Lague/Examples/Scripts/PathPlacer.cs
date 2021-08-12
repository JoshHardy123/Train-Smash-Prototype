using PathCreation;
using UnityEngine;

namespace PathCreation.Examples {

    [ExecuteInEditMode]
    public class PathPlacer : PathSceneTool {
        //NEED TO CUSTOMISE FURTHER TO CHOOSE WHAT OBJECT GOES WHERE IN THE PATH. IF NOT INFINITE, CREATE A LIST OF OBJECTS. DONE

        public GameObject prefab;
        public GameObject holder;
        public float spacing = 3;
        public float startDistance = 2f;
        public int numberOfObjects = 1;
        public bool infinite = false;
        public GameObject[] objectToSpawn;
        public float objectYAxis =1;
        public float objectxAxis = 1;
        public float zRotation;
        public float yRot = 0;
        public float xRot = 90;

        int objectsSpawned;

        const float minSpacing = .1f;

        void Generate () {
            if (pathCreator != null && prefab != null && holder != null) {
                DestroyObjects ();

                VertexPath path = pathCreator.path;

                spacing = Mathf.Max(minSpacing, spacing);
                float dst = 0;
                objectsSpawned = 0;

                while (dst < path.length) {
                    if (dst > startDistance && (infinite ||objectsSpawned < numberOfObjects))
                    {
                        Vector3 point = path.GetPointAtDistance(dst);
                        point += new Vector3(objectxAxis,objectYAxis,0);
                        Quaternion rot = path.GetRotationAtDistance(dst);
                        rot.z += zRotation;
                        rot.x += xRot;
                        rot.y = yRot;
                        if(infinite)Instantiate(prefab, point, rot, holder.transform);
                        else Instantiate(objectToSpawn[objectsSpawned], point,rot , holder.transform);
                        objectsSpawned++;
                    }
                    dst += spacing;
                }
            }
        }

        void DestroyObjects () {
            int numChildren = holder.transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--) {
                DestroyImmediate (holder.transform.GetChild (i).gameObject, false);
            }
        }

        protected override void PathUpdated () {
            if (pathCreator != null) {
                Generate ();
            }
        }
    }
}