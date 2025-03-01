using System.Collections;
using UnityEngine;

public abstract class PrimaryAction : Action {

    [Header("References")]
    private Coroutine shootCoroutine;

    protected new void OnEnable() {

        base.OnEnable();
        playerController.SetWeaponHandlerEnabled(cooldownTimer == 0f); // enable weapon handler if shot is ready

    }

    // runs before weapon is switched
    protected new void OnDisable() {

        base.OnDisable();
        playerController.SetWeaponHandlerEnabled(true); // enable weapon handler when action is disabled

    }

    public override void OnTriggerRegular() {

        if (cooldownTimer > 0f) return; // make sure action is ready and not already shooting

        if (!canUseInAir && !playerController.IsGrounded()) return; // make sure player is grounded if required

        if (shootCoroutine != null) StopCoroutine(shootCoroutine); // stop shooting coroutine if it exists
        shootCoroutine = StartCoroutine(HandleShoot()); // handle shooting

        StartCooldown(); // start cooldown

    }

    private IEnumerator HandleShoot() {

        playerController.SetWeaponHandlerEnabled(true); // enable weapon handler to allow shooting
        charWeaponHandler.ShootStart(); // start shooting weapon

        // must wait for two frames to allow projectile to be fired
        yield return null;
        yield return null;

        charWeaponHandler.ShootStop(); // stop shooting weapon (to deal with infinite shooting bug | do this before disabling the core scripts)

        playerController.SetWeaponHandlerEnabled(false); // disable weapon handler when shot is fired

        shootCoroutine = null; // reset coroutine

    }

    protected override void StartCooldown(bool restartTimer = true) {

        if (playerController.IsDead()) return; // do not start cooldown if player is dead

        base.StartCooldown(restartTimer);

        if (gameManager.IsCooldownsEnabled())
            weaponSelector.SetPrimaryCooldownValue(GetNormalizedCooldown(), cooldownTimer); // update primary cooldown meter

    }
}
