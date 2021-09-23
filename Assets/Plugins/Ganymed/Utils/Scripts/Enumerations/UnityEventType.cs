namespace Ganymed.Utils
{
    public enum UnityEventType
    {
        Awake,
        Start,
        Recompile,
        ApplicationQuit,
        EditorApplicationQuit,
        Update,
        LateUpdate,
        FixedUpdate,
        OnEnable,
        OnDisable,
        BuildPlayer,
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// includes EnteredEditMode & EnteredEditMode & ExitingEditMode & ExitingPlayMode
        /// </summary>
        TransitionEditPlayMode,
        EnteredEditMode,
        ExitingEditMode,
        EnteredPlayMode,
        ExitingPlayMode,
        EditorApplicationStart,
        BeforeSceneLoad,
        AfterSceneLoad,
        AfterAssembliesLoaded,
        InspectorUpdate,
        SubsystemRegistration,
        BeforeSplashScreen,
        PreProcessorBuild,
        OnLoad
    }
}