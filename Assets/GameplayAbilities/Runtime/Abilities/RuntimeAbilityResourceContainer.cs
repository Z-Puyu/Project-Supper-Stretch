using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace GameplayAbilities.Abilities {
    [Serializable]
    public class RuntimeAbilityResourceContainer {
        [field: SerializeReference]
        private List<IAbilityResource> Resources { get; set; } = new List<IAbilityResource>();
        
        private IDictionary<Type, Subcontainer> Subcontainers { get; } = new Dictionary<Type, Subcontainer>();

        internal void RegisterResources() {
            foreach (IAbilityResource resource in this.Resources) {
                if (this.Subcontainers.TryGetValue(resource.GetType(), out Subcontainer subcontainer)) {
                    subcontainer.Register(resource);
                    continue;
                }

                subcontainer = new Subcontainer<IAbilityResource>();
                this.Subcontainers.Add(resource.GetType(), subcontainer);
            }
        }
        
        internal bool HasResource<T>(AbilityResourceKey<T> key, [NotNullWhen(true)] out T? resource)
                where T : IAbilityResource {
            if (this.Subcontainers.TryGetValue(typeof(T), out Subcontainer subcontainer)) {
                return ((Subcontainer<T>)subcontainer).Resources.TryGetValue(key, out resource);
            }

            resource = default;
            return false;
        }

        private abstract class Subcontainer {
            internal abstract void Register(IAbilityResource resource);
        }

        private sealed class Subcontainer<T> : Subcontainer {
            internal IDictionary<AbilityResourceKey<T>, T> Resources { get; } =
                new Dictionary<AbilityResourceKey<T>, T>();

            internal override void Register(IAbilityResource resource) {
                if (resource is AbilityResource<T> { Value: not null } res) {
                    this.Resources.Add(res.Key, res.Value);
                }
            }
        }
    }
}
