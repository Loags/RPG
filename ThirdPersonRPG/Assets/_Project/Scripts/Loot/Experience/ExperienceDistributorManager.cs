using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LB;

namespace LB.Loot.Experience
{
    public class ExperienceDistributorManager : PersistantSingleton<ExperienceDistributorManager>
    {
        [SerializeField] private float groupExperienceMultiplier = 1.5f;
        [SerializeField] private float maxDistanceForGroupExp = 50f;
        [SerializeField] private LayerMask experienceReceiverLayer;

        public void DistributeExperience(float baseExperience, Vector3 sourcePosition, IExperienceReceiver primaryReceiver)
        {
            List<IExperienceReceiver> receivers = GetEligibleReceivers(sourcePosition, primaryReceiver);

            if (receivers.Count == 0) return;

            float totalExperience = baseExperience * groupExperienceMultiplier;
            float experiencePerReceiver = totalExperience / receivers.Count;

            foreach (IExperienceReceiver receiver in receivers)
            {
                if (receiver.IsAlive)
                {
                    receiver.AddExperience(experiencePerReceiver);
                }
            }
        }

        private List<IExperienceReceiver> GetEligibleReceivers(Vector3 sourcePosition, IExperienceReceiver primaryReceiver)
        {
            List<IExperienceReceiver> receivers = new List<IExperienceReceiver>();
            Collider[] hitColliders = Physics.OverlapSphere(sourcePosition, maxDistanceForGroupExp, experienceReceiverLayer, QueryTriggerInteraction.Collide);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<IExperienceReceiver>(out IExperienceReceiver receiver))
                {
                    if (receiver.Faction == primaryReceiver.Faction)
                    {
                        receivers.Add(receiver);
                    }
                }
            }

            return receivers;
        }
    }
}