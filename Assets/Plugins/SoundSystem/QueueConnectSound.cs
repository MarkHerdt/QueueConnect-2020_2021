namespace QueueConnect.Plugins.SoundSystem
{
    public enum VFX
    {
        None = 0,
        
        UIMenuOpenDigital = 1,
        UIMenuCloseDigital = 2,  
        UIMenuOpenSlider = 3, 
        UIMenuCloseSlider = 4,
        
        UIMenuButtonPressedSuccess = 5, 
        UIMenuButtonPressedFailure = 6,  
        
        UIPartButtonPressedSuccess = 7, 
        UIPartButtonPressedFailure = 8, 
        
        RepairArmSpawn = 9,
        
        UIOnScoreIncreased = 10,         

        RobotOnDestroyed = 11,               
        RobotOnRelease = 12,                 // when the robot is released from the lift
        RobotOnImpact = 13,                  // when the robot touches the ground
        RobotOnRepairProgress = 14,          // when a part is added but the robot is still broken.
        RobotOnRepairSuccess = 15,           // when a part is added and the robot is no longer broken.
        
        ScannerRailSelectedTarget = 16,      
        ScannerRailActive = 17,               
        ScannerRailMovement = 18,           
        
        ScannerStationActive = 19,            
        ScannerStationaryScanSuccess = 20,    
        ScannerStationaryScanFailure = 21,   
        
        RaygunFire = 22,                    
        
        OnLevelStart = 23,                   
        OnGameOver = 24,                     
        OnGamePaused = 25,                   
        OnGamePlay = 26,                      
        
        OnAudioUnmute = 27,                  
        
        OnShuffleCountdownReset = 28,                       
        OnBeforeShuffleTimerReset = 29,          
        
        OnItemCollected = 30,
        OnItemHealthCollected = 31,
        OnItemArmorCollected = 32,
        OnItemTimeCollected = 33,
        OnItemPhoenixCollected = 34,
        OnItemMotivationCollected = 35,
        OnItemAutoRepairCollected = 36,
        OnItemMultiplierCollected = 37,
        OnItemItemBurstCollected = 38,
        OnItemScoreShieldCollected = 39,
        OnItemRandomItemCollected = 40,
        
        OnItemNotCollectableSound = 41,
        OnItemPhoenixEffect = 42,
        
        OnSplashScreen = 50,
        OnTutorialDisplaySpawn = 52,
        OnTutorialDisplayDeSpawn = 53,
        OnTutorialDisplayUpdate = 54,
        
        OnCountdownTick = 60,
        OnAutoRepairEnd = 61,
        OnShiedDown = 62,
        ElectricSpark = 63,
        
        OnWaveStarted = 70,
        OnWaveEnded = 71,
        
        OnRewardOffer = 72,
        OnPlayerRevive = 73,
    }
    
    public enum Music
    {
        None = 0,
        MainMenu = 1,
        FactoryPlay = 2,
        FactoryPause = 3
    }
    
    public enum Ambience
    {
        None = 0,
        MainMenu = 1,
        FactoryPlay = 2,
        FactoryPause = 3
    }

}