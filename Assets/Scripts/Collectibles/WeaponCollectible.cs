using MoreMountains.InventoryEngine;
using UnityEngine;

public class WeaponCollectible : Collectible {

    [Header("References")]
    private WeaponSelector weaponSelector;

    [Header("Settings")]
    [SerializeField] private WeaponData weaponData;

    protected new void Start() {

        base.Start();
        weaponSelector = FindObjectOfType<WeaponSelector>();

    }

    protected override void OnCollect(ItemPicker collectible) {

        base.OnCollect(collectible);
        weaponSelector.AddWeapon(weaponData); // give player the weapon

    }
}
