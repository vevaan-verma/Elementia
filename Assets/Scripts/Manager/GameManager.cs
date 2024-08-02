using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("References")]
    private DataManager dataManager;
    private CooldownManager cooldownManager;
    private GameCore gameCore;
    private List<PhysicsObject> objectsToReset;

    [Header("Settings")]
    private List<Collectible> requiredCollectibles;
    private List<KeyCollectible> keyCollectibles;
    private bool isLevelComplete;
    private bool isCooldownsEnabled;

    [Header("Collectible Inventory")]
    [SerializeField] private int collectibleRows;
    [SerializeField] private int collectibleColumns;
    private InventoryDisplay collectibleInventoryDisplay;

    [Header("Key Inventory")]
    [SerializeField] private int keyRows;
    [SerializeField] private int keyColumns;
    private InventoryDisplay keyInventoryDisplay;

    private void Start() {

        dataManager = FindObjectOfType<DataManager>();
        gameCore = FindObjectOfType<GameCore>();
        cooldownManager = FindObjectOfType<CooldownManager>();

        // get the collectible inventory display & key inventory display
        collectibleInventoryDisplay = gameCore.GetCollectibleInventoryDisplay();
        keyInventoryDisplay = gameCore.GetKeyInventoryDisplay();

        // get all required & key collectibles
        requiredCollectibles = new List<Collectible>();
        keyCollectibles = new List<KeyCollectible>();

        foreach (Collectible collectible in FindObjectsOfType<Collectible>()) {

            if (collectible.IsRequired())
                requiredCollectibles.Add(collectible);
            else if (collectible is KeyCollectible)
                keyCollectibles.Add((KeyCollectible) collectible);

        }

        // check if there are enough slots to hold the required collectibles
        if (collectibleRows * collectibleColumns < requiredCollectibles.Count)
            Debug.LogError("There are not enough slots to hold the amount of required collectibles. There are " + requiredCollectibles.Count + " required collectibles in this scene.");

        // check if there are enough slots to hold the key collectibles
        if (keyRows * keyColumns < keyCollectibles.Count)
            Debug.LogError("There are not enough slots to hold the amount of key collectibles. There are " + keyCollectibles.Count + " key collectibles in this scene.");

        // setup the collectible inventory display
        collectibleInventoryDisplay.NumberOfRows = collectibleRows;
        collectibleInventoryDisplay.NumberOfColumns = collectibleColumns;
        collectibleInventoryDisplay.SetupInventoryDisplay();

        // setup the key inventory display
        keyInventoryDisplay.NumberOfRows = keyRows;
        keyInventoryDisplay.NumberOfColumns = keyColumns;
        keyInventoryDisplay.SetupInventoryDisplay();

        isCooldownsEnabled = true; // enable cooldowns by default

    }

    public void ResetAllResettables(float resetDuration) {

        objectsToReset = new List<PhysicsObject>();

        // reset all resettables (don't store in list because resettables can be added/removed from the game at any point)
        foreach (PhysicsObject physicsObject in FindObjectsOfType<PhysicsObject>()) {

            if (physicsObject.IsResettable()) {

                physicsObject.StartReset(resetDuration);
                objectsToReset.Add(physicsObject);

            }
        }
    }

    public void CancelResets() {

        // cancel all resets
        foreach (PhysicsObject physicsObject in objectsToReset)
            physicsObject.CancelReset();

        objectsToReset.Clear();

    }

    public void CheckVictory() {

        if (isLevelComplete) // if the level is already complete, return
            return;

        // check if all required collectibles have been collected
        foreach (Collectible collectible in requiredCollectibles)
            if (!collectible.IsCollected())
                return;

        // if all required collectibles have been collected, set the level as complete
        print("Level Complete!");
        isLevelComplete = true;

    }

    public void SetCooldownsEnabled(bool isCooldownsEnabled) {

        // clear all cooldown data if cooldowns are disabled
        if (!isCooldownsEnabled)
            cooldownManager.ClearCooldownData();

        this.isCooldownsEnabled = isCooldownsEnabled;

    }

    public void LoadScene(string sceneName) {

        dataManager.SaveData();
        MMSceneLoadingManager.LoadScene(sceneName);

    }

    public bool IsCooldownsEnabled() => isCooldownsEnabled;

}
