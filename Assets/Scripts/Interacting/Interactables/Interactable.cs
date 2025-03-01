using MoreMountains.Tools;
using System;
using UnityEngine;

public abstract class Interactable : MMPersistentBase {

    [Header("References")]
    protected bool isInteractable;
    protected bool isInteracted;

    [Serializable]
    private struct Data {

        public bool isInteracted;

    }

    public abstract void Interact();

    public override string OnSave() {

        Data saveData = new() { isInteracted = isInteracted };
        return JsonUtility.ToJson(saveData); // save data

    }

    public override void OnLoad(string data) {

        Data saveData = JsonUtility.FromJson<Data>(data);
        isInteracted = saveData.isInteracted; // load data

    }

    public abstract bool IsInteractable();

}
