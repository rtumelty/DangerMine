#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
/// <summary>
/// Character. Provides movement and attack functionality, and determines combat state flow.
/// </summary>
public class Character : DestructibleEntity {
	protected enum AttackState {
		NotAttacking,
		Attacking
	}

	public enum AttackDirections {
		Forwards = 1,
		Backwards = 2,
		Up = 4,
		Down = 8
	}

	protected Vector3 targetPosition = default(Vector3);

	[SerializeField] protected float maxMoveSpeed;
	float currentSpeed;
	[SerializeField] protected float maxVelocityChange;
	[SerializeField] float baseAttackStrength;
	[SerializeField] float attackInterval;
	private float currentAttackStrength;

	public float AttackStrength {
		get {
			return baseAttackStrength;
		}
	}

	[SerializeField] bool ranged;
	[SerializeField] GameObject projectilePrefab;
	[SerializeField] Vector2 attackRange;

	public Vector2 AttackRange {
		get {
			return attackRange;
		}
	}

	[SerializeField] protected AttackZone attackHitbox;
	[SerializeField] AttackDirections attackDirection;

	public AttackDirections AttackDirection {
		get {
			return attackDirection;
		}
	}

	AttackState attackState;

	List<DestructibleEntity> activeTargets;
	DestructibleEntity mainTarget;
	[SerializeField] LayerMask collisionMask;
	[SerializeField] LayerMask targetingMask;
	[SerializeField] List<DestructibleEntity> ignoredTargets;

	public LayerMask TargetingMask {
		get {
			return targetingMask;
		}
	}

	public List<DestructibleEntity> IgnoredTargets {
		get {
			return ignoredTargets;
		}
	}
	
	private float attackMultiplier = 1f;
	public float AttackMultiplier {
		get {
			return attackMultiplier;
		}
		set {
			attackMultiplier = value;
			
			currentAttackStrength = baseAttackStrength * attackMultiplier;
		}
	}
	
	private float speedMultiplier = 1f;
	public float SpeedMultiplier {
		get {
			return speedMultiplier;
		}
		set {
			speedMultiplier = value;
			
			currentSpeed = maxMoveSpeed * speedMultiplier;
		}
	}

	protected override void OnEnable() {
		base.OnEnable();

		currentSpeed = maxMoveSpeed;
		currentAttackStrength = baseAttackStrength;

		rigidbody2D.gravityScale = 0;
		rigidbody2D.fixedAngle = true;
		rigidbody2D.isKinematic = false;

		collider2D.enabled = true;

		activeTargets = new List<DestructibleEntity>();

		attackHitbox = GetComponentInChildren<AttackZone>();
		if (attackHitbox == null) {
			GameObject go = new GameObject("AttackZone");
			go.transform.parent = transform;
			go.AddComponent<BoxCollider2D>();
			attackHitbox = go.AddComponent<AttackZone>();
		}
	}

	protected virtual void Update() {
		if (entityState == EntityState.Inactive || entityState == EntityState.Dying) return;


		switch (attackState) {
		case AttackState.NotAttacking:
			if (ranged && activeTargets.Count > 0) {
				attackState = AttackState.Attacking;
				StartCoroutine(Attack ());
			}
			break;
		}
	}

	protected virtual void FixedUpdate() {

		Move ();
	}

	/// <summary>
	/// Attack coroutine. If unit is ranged, shoots projectiles. If melee unit, deals damage directly to enemy/obstacle. Overriddable by subclasses.
	/// </summary>
	protected virtual IEnumerator Attack() {
		LogMessage("Entering Attacking state");

		attackState = AttackState.Attacking;

		while (attackState == AttackState.Attacking) {
			yield return new WaitForSeconds(attackInterval);

			CheckMainTarget();

			if (mainTarget == null) {
				LogMessage("No active targets - Entering NotAttacking state");
				attackState = AttackState.NotAttacking;
				yield break;
			}

			if (ranged) {
				LogMessage("Firing projectiles");
				if ((attackDirection & AttackDirections.Forwards) != 0) {
					Projectile projectile = PrefabPool.GetPool(projectilePrefab).Spawn(transform.position).GetComponent<Projectile>();
					projectile.Init(this, transform.TransformDirection(1, 0, 0));
				}
				if ((attackDirection & AttackDirections.Backwards) != 0) {
					Projectile projectile = PrefabPool.GetPool(projectilePrefab).Spawn(transform.position).GetComponent<Projectile>();
					projectile.Init(this, transform.TransformDirection(-1, 0, 0));
				}
				if ((attackDirection & AttackDirections.Up) != 0) {
					Projectile projectile = PrefabPool.GetPool(projectilePrefab).Spawn(transform.position).GetComponent<Projectile>();
					projectile.Init(this, transform.TransformDirection(0, 1, 0));
				}
				if ((attackDirection & AttackDirections.Down) != 0) {
					Projectile projectile = PrefabPool.GetPool(projectilePrefab).Spawn(transform.position).GetComponent<Projectile>();
					projectile.Init(this, transform.TransformDirection(0, -1, 0));
				}
			} else {
				LogMessage("Melee attack on " + mainTarget.name + ", strength " + currentAttackStrength);
				mainTarget.TakeDamage(currentAttackStrength, this);
			}
		}
	}

	private void CheckMainTarget() {
		LogMessage("Checking if current target is valid");

		bool checkForNewTarget = false;

		if (mainTarget == null) checkForNewTarget = true;
		else if (mainTarget.State != EntityState.Active) {
			LogMessage(mainTarget.name + " state: " + mainTarget.State + ", stopping attack.");
			mainTarget = null;
			checkForNewTarget = true;
		} else if (!activeTargets.Contains(mainTarget)) {
			LogMessage(mainTarget.name + " no longer in range, stopping attack.");
			mainTarget = null;
			checkForNewTarget = true;
		}

		if (ranged && checkForNewTarget) {
			LogMessage("Ranged unit; looking for new target");

			if (activeTargets.Count > 0) {
				mainTarget = activeTargets[0];
				LogMessage("New target found - " + mainTarget.name);
			}
			else
				LogMessage("No targets in range");
		}
	}

	public void UpdateTargets(List<DestructibleEntity> targets) {
		activeTargets = targets;
	}

	/// <summary>
	/// Raises the collision enter event - if non-ranged unit and collided object is targetable, triggers Attacking state
	/// </summary>
	protected virtual void OnCollisionEnter2D(Collision2D other) {
		LogMessage("Collided with " + other.gameObject.name);
		if (!ranged) {
			DestructibleEntity entity = other.gameObject.GetComponent<DestructibleEntity>();

			if (entity == null) return;
			else if ((1 << entity.gameObject.layer & targetingMask) == 0) {
				LogMessage(entity.name + " not on targeted layer.");
				return;
			}
			else if (!activeTargets.Contains(entity)) {
				LogMessage(entity.name + " not in attack range." + activeTargets.Count);
				return;
			}
			else {
				foreach (DestructibleEntity ignoredTarget in ignoredTargets) {
					if (ignoredTarget.GetType() == entity.GetType())
						return;
				}

				LogMessage(entity.name + " set as main target.");
				mainTarget = entity;
				attackState = AttackState.Attacking;
				StartCoroutine(Attack ());
			}
		}
	}

	/// <summary>
	/// Updates character velocity to move towards targetPosition. targetPosition's value determines in subclasses.
	/// </summary>
	protected virtual void Move() {
		Vector2 targetVelocity = Vector2.ClampMagnitude(targetPosition - transform.position, maxMoveSpeed);
		Vector2 velocityChange = targetVelocity - rigidbody2D.velocity;

		velocityChange = Vector2.ClampMagnitude(velocityChange, maxVelocityChange);
		rigidbody2D.AddForce(velocityChange * rigidbody2D.mass, ForceMode2D.Force);
		//LogMessage("Moving to " + targetPosition + ", target velocity: " + targetVelocity + ", delta velocity: " + velocityChange);
	}

	protected override IEnumerator Dying(GameEntity cause) {
		entityState = EntityState.Dying;


		yield return new WaitForSeconds(.5f);

		Die (cause);
	}

	/// <summary>
	/// Disables GameObject. Overriddable in subclasses.
	/// </summary>
	/// <param name="cause">Cause of object death.</param>
	protected override void Die(GameEntity cause) {

		base.Die(cause);
	}

	#if UNITY_EDITOR
	private bool expandMovement = false;
	private bool expandCombat = false;
	private bool expandExceptions = false;
	
	public override void DrawInspectorGUI(Editor editor) {
		base.DrawInspectorGUI(editor);
		
		if (showDefaultInspector == true) return;

		expandMovement = EditorGUILayout.Foldout(expandMovement, "Movement");
		if (expandMovement) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.BeginVertical(EditorStyles.textArea);

			EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
			maxMoveSpeed = EditorGUILayout.FloatField("Max move speed:", maxMoveSpeed);
			maxVelocityChange = EditorGUILayout.FloatField("Max velocity change:", maxVelocityChange);

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
			
		expandCombat = EditorGUILayout.Foldout(expandCombat, "Combat");
		if (expandCombat) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.BeginVertical(EditorStyles.textArea);
			
			EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
			baseAttackStrength = EditorGUILayout.FloatField("Attack strength:", baseAttackStrength);
			attackInterval = EditorGUILayout.FloatField("Attack interval (s):", attackInterval);
			attackRange = EditorGUILayout.Vector2Field("Attack range:", attackRange);
			attackDirection = (AttackDirections) EditorGUILayout.EnumMaskField("Attack direction", attackDirection);
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Ranged stats", EditorStyles.boldLabel);
			ranged = EditorGUILayout.Toggle("Ranged unit:", ranged);

			if (ranged) {
				projectilePrefab = EditorGUILayout.ObjectField("Projectile prefab:", projectilePrefab, typeof(GameObject), false) as GameObject;
			}
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Collision / targeting filters", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Reference 'Edit > Project Settings > Physics2D' for layer collision table. Set object's layer to the appropriate group.", MessageType.Info);
			targetingMask = EditorHelpers.LayerMaskField("Targeting mask", targetingMask, true);

			expandExceptions = EditorGUILayout.Foldout(expandExceptions, "Targeting exceptions (class):");
			if (expandExceptions) {
				ignoredTargets = EditorHelpers.ListField(ignoredTargets);
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
	}
	#endif
}
