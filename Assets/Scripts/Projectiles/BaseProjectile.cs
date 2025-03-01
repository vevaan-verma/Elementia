using MoreMountains.CorgiEngine;
using UnityEngine;

public class BaseProjectile : MonoBehaviour {

    [Header("References")]
    protected Projectile projectile;
    protected DamageOnTouch damageOnTouch;
    private bool hasCollided;

    [Header("Direction")]
    protected Vector2 startPos;

    [Header("Settings")]
    [SerializeField] private bool oneCollisionOnly;
    [SerializeField] private bool hasDistanceMultiplier;
    [Space]
    [SerializeField] private bool isPushProjectile;
    [SerializeField] private Vector2 entityForceMultiplier;
    [SerializeField] private Vector2 objectForceMultiplier;

    protected void OnEnable() { // runs each time projectile is enabled/shot because it is pooled

        projectile = GetComponent<Projectile>();
        damageOnTouch = GetComponent<DamageOnTouch>();
        hasCollided = false; // reset has collided because object pooling cycles the same objects
        startPos = transform.position; // set start position to current position

    }

    protected void OnTriggerEnter2D(Collider2D collision) { // triggers when projectile collides with something | IMPORTANT: triggers after the object is disabled on death

        if (collision.gameObject.activeInHierarchy && (damageOnTouch.TargetLayerMask & (1 << collision.gameObject.layer)) != 0 && !hasCollided) { // make sure hit object is active, is in target layer, and has not collided yet

            Vector2 entityForce;

            // FORCE DIRECTIONS DEPEND ON PROJECTILE SPAWN LOCATION
            if (isPushProjectile) // push projectile
                entityForce = ((Vector2) transform.position - startPos).normalized; // get force direction (vector faces projectile spawn location)
            else // pull projectile
                entityForce = (startPos - (Vector2) transform.position).normalized; // get force direction (vector faces collision point)

            Vector2 objectForce = entityForce;

            if (hasDistanceMultiplier) { // if distance multiplier is enabled

                float distance = Vector2.Distance(startPos, transform.position); // get distance between start position and current position
                entityForce *= distance; // apply distance multiplier to entity force
                objectForce *= distance; // apply distance multiplier to object force

            }

            entityForce *= entityForceMultiplier; // apply multiplier to entity force
            objectForce *= objectForceMultiplier; // apply multiplier to object force

            collision.gameObject.GetComponent<CorgiController>()?.AddForce(entityForce); // add force to entity

            Rigidbody2D objectRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (objectRb) // make sure rigidbody exists
                objectRb.AddForce(objectForce, ForceMode2D.Impulse); // add force to object

            if (oneCollisionOnly) // if projectile can only collide once
                hasCollided = true; // set collided to true

        }
    }
}
