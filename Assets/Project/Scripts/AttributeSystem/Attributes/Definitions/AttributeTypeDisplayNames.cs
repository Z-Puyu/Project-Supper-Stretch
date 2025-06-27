using Project.Scripts.Common.GameplayTags;
using UnityEngine;

namespace Project.Scripts.AttributeSystem.Attributes.Definitions;

[CreateAssetMenu(fileName = "Attribute Type Display Names", menuName = "Attribute System/Attribute Type Localisation", order = 0)]
public class AttributeTypeDisplayNames : GameplayTagLocalisationMapping<AttributeType>;