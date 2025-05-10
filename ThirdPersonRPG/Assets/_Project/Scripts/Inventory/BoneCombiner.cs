using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LB.Inventory
{
    public class BoneCombiner
    {
        public readonly Dictionary<int, Transform> RootBoneDictionary = new();
        private readonly Transform[] boneTransforms = new Transform[67];

        private new readonly Transform transform;

        public BoneCombiner(GameObject _rootObject)
        {
            transform = _rootObject.transform;
            TraverseHierarchy(transform);
        }

        public Transform AddLimb(GameObject _boneObject, List<string> _boneNames)
        {
            Transform limb = ProcessBonedObject(_boneObject.GetComponentInChildren<SkinnedMeshRenderer>(), _boneNames);
            limb.SetParent(transform);
            return limb;
        }

        private Transform ProcessBonedObject(SkinnedMeshRenderer _renderer, List<string> _boneNames)
        {
            Transform bonedObject = new GameObject().transform;

            SkinnedMeshRenderer meshRenderer = bonedObject.gameObject.AddComponent<SkinnedMeshRenderer>();

            for (int i = 0; i < _boneNames.Count; i++)
            {
                boneTransforms[i] = RootBoneDictionary[_boneNames[i].GetHashCode()];
            }

            meshRenderer.bones = boneTransforms;
            meshRenderer.sharedMesh = _renderer.sharedMesh;
            meshRenderer.sharedMaterials = _renderer.sharedMaterials;

            return bonedObject;
        }

        private void TraverseHierarchy(Transform _transform)
        {
            foreach (Transform child in _transform)
            {
                RootBoneDictionary.Add(child.GetHashCode(), child);
                TraverseHierarchy(child);
            }
        }
    }
}