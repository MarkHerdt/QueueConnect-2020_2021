using System.Collections;
using System.Collections.Generic;
using Ganymed.Utils;
using Ganymed.Utils.Callbacks;
using UnityEngine;

[ExecuteInEditMode]
public class ToggleInEditMode : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [Space]
    [Header("States")]
    [SerializeField] private bool editMode = true;
    [SerializeField] private bool playMode = true;

    protected ToggleInEditMode()
    {
        UnityEventCallbacks.AddEventListener(Execute, ApplicationState.EditAndPlayMode, UnityEventType.Start);
    }

    private void Execute()
    {
        if(!this || !active) return;
        gameObject.SetActive(Application.isPlaying ? playMode : editMode);
    }
}
