using Unity.Behavior;

namespace Project.Scripts.Characters.Enemies.AI;

[BlackboardEnum]
public enum EnemyStatus {
	Idle,
	Patrol,
	Pursue,
	Engaged
}