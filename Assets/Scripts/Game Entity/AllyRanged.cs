using UnityEngine;
using System.Collections;

public class AllyRanged : Ally {
	[SerializeField] protected float projectileSpeed = 5;
	[SerializeField] protected int shotDirection = 1;

	public float ProjectileSpeed {
		get {
			return projectileSpeed;
		}
	}

	public int ShotDirection {
		get {
			return shotDirection;
		}
	}

	[SerializeField] string projectilePoolId;

	protected override void Awake() {
		base.Awake();

		attackZone = GetComponentInChildren<AttackZone>();
		attackZone.SetSize(attackRange, shotDirection);
	}

	public override void UpdateTargets (System.Collections.Generic.List<GameEntity> targets)
	{
		base.UpdateTargets (targets);

		cantMove = false;
	}

	protected override IEnumerator Attack() {
		attacking = true;
		mySpineMultiSkeleton.SetAnimation (attackAnimation, 0);
		
		while (attacking) {
			if (mySpineMultiSkeleton.skeleton.state.GetCurrent(0) == null) mySpineMultiSkeleton.SetAnimation (attackAnimation, 0);
			/* DPS approach
			yield return new WaitForSeconds(Time.deltaTime);
			*/
			yield return new WaitForSeconds(attackSpeed);

			Projectile projectile = PrefabPool.GetPool(projectilePoolId).Spawn(transform.position).GetComponent<Projectile>();
			if (projectile != null) {
				projectile.Init(this);
			}
			else {
				Debug.LogWarning("No projectile prefab found: " + gameObject);
			}
		}
		
		mySpineMultiSkeleton.SetAnimation (walkAnimation, 0);
	}
}
