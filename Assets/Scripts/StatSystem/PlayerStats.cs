using System;
using System.Collections.Generic;
using Ganymed.Utils;
using Ganymed.Utils.Callbacks;
using Ganymed.Utils.ExtensionMethods;
using QueueConnect.CollectableSystem;
using QueueConnect.Environment;
using QueueConnect.GameSystem;
using QueueConnect.Platform;
using QueueConnect.Plugins.Ganymed.Utils.Scripts.ExtensionMethods;
using QueueConnect.Robot;
using UnityEngine;
using UnityEngine.Advertisements;

namespace QueueConnect.StatSystem
{
    public static class PlayerStats
    {
        #region --- [FIELDS] ---

        private static bool isPlaying = false;
        private static readonly Color OddColor = new Color(0.83f, 0.85f, 1f);
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [GET STATS] ---

        public static IEnumerable<Stat> GetTotalStats()
        {
            var stats = new List<Stat>
            {
                new Stat("st_ica", TotalItemsCollectedAmount, OddColor),
                new Stat("st_rra", TotalRobotsRepairedAmount),
                new Stat("st_pt", TotalPlayTime,  OddColor),
                new Stat("st_ps", TotalPartShuffles),
                new Stat("st_tg", TotalTimeGained,  OddColor),
                new Stat("st_tl", TotalTimeLost),
                new Stat("st_ti", TotalTimeInvincible),
                new Stat("st_pa", TotalPlayAttempts, OddColor),
                new Stat("st_hg", TotalHealthGained),
                new Stat("st_hl", TotalHealthLost, OddColor),
                new Stat("st_ag", TotalArmorGained),
                new Stat("st_al", TotalArmorLost, OddColor),
                new Stat("st_cps", TotalCorrectPartsSelected),
                new Stat("st_wps", TotalWrongPartsSelected, OddColor),
                new Stat("st_rd", TotalRobotsDestroyed),
                new Stat("st_we", TotalWavesEndured, OddColor),
                Stat.Empty,
                new Stat("st_adw", AdsWatched),
                new Stat("st_ads", AdsSkipped, OddColor),
                Stat.Empty,
            };

            var counter = 0;
            foreach (var pair in TotalCollectedItems)
            {
                stats.Add((counter++).IsEven()
                    ? new Stat($"st_item_{pair.Key}", pair.Value,  OddColor)
                    : new Stat($"st_item_{pair.Key}", pair.Value));
            }
            
            stats.Add(Stat.Empty);

            counter = 0;
            foreach (var pair in TotalRepairedRobots)
            {
                stats.Add((counter++).IsEven()
                    ? new Stat($"st_robo_{pair.Key}", pair.Value, OddColor)
                    : new Stat($"st_robo_{pair.Key}", pair.Value));
            }
            
            return stats;
        }
        
        public static IEnumerable<Stat> GetMaximumsStats()
        {
            var stats = new List<Stat>
            {
                new Stat("st_mwr", MaxWaveReached, OddColor),
                new Stat("st_msr", MaxScore * 100),
                new Stat("st_mmr", MaxMultiplier, OddColor),
                new Stat("st_mpr", MaxPerfectRunTime)
            };


            return stats;
        }
        
        
        public static IEnumerable<Stat> GetCurrentStats()
        {
            var stats = new List<Stat>
            {
                new Stat("st_rra", CurrentRobotsRepaired, OddColor),
                new Stat("st_ica", CurrentItemsCollected),
                new Stat("st_pt", CurrentPlayTime, OddColor),
                new Stat("st_ps", CurrentPartShuffles),
                new Stat("st_tg", CurrentTimeGained, OddColor),
                new Stat("st_tl", CurrentTimeLost),
                new Stat("st_ti", CurrentTimeInvincible),
                
                new Stat("st_cs", CurrentScore, OddColor),
                new Stat("st_cm", CurrentMultiplier),
                new Stat("st_cw", CurrentWave, OddColor),
                new Stat("st_hg", CurrentHealthGained),
                new Stat("st_hl", CurrentHealthLost, OddColor),
                new Stat("st_ag", CurrentArmorGained),
                new Stat("st_al", CurrentArmorLost, OddColor),
                new Stat("st_prt", CurrentLongestPerfectRunTime),
                new Stat("st_cps", CurrentCorrectPartsSelected, OddColor),
                new Stat("st_wps", CurrentWrongPartsSelected),
                new Stat("st_rd", CurrentRobotsDestroyed, OddColor),
                Stat.Empty
            };


            var counter = 0;
            foreach (var pair in CurrentCollectedItems)
            {
                stats.Add((counter++).IsEven()
                    ? new Stat($"st_item_{pair.Key}", pair.Value,  OddColor)
                    : new Stat($"st_item_{pair.Key}", pair.Value));
            }
            
            stats.Add(Stat.Empty);
            
            counter = 0;
            foreach (var pair in CurrentRepairedRobots)
            {
                stats.Add((counter++).IsEven()
                    ? new Stat($"st_robo_{pair.Key}", pair.Value, OddColor)
                    : new Stat($"st_robo_{pair.Key}", pair.Value));
            }
            
            return stats;
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region --- [PROPERTIES: TOTALS] ---

        public static Dictionary<ItemID, int> TotalCollectedItems => totalCollectedItems;
        public static Dictionary<RobotType, int> TotalRepairedRobots => totalRepairedRobots;
        public static int TotalPartShuffles => totalPartShuffles;
        public static float TotalTimeInvincible => totalTimeInvincible;
        public static float TotalTimeGained => totalTimeGained;
        public static float TotalTimeLost => totalTimeLost;
        public static int TotalItemsCollectedAmount => totalItemsCollectedAmount;
        public static int TotalRobotsRepairedAmount => totalRobotsRepairedAmount;
        public static float TotalPlayTime => totalPlayTime;
        public static int TotalPlayAttempts => totalPlayAttempts;
        public static int TotalHealthGained => totalHealthGained;
        public static int TotalHealthLost => totalHealthLost;
        public static int TotalArmorGained => totalArmorGained;
        public static int TotalArmorLost => totalArmorLost;
        public static int TotalCorrectPartsSelected => totalCorrectPartsSelected;
        public static int TotalWrongPartsSelected => totalWrongPartsSelected;
        public static int TotalRobotsDestroyed => totalRobotsDestroyed;
        public static int TotalWavesEndured => totalWavesEndured;
        
        #endregion

        #region --- [PROPERTIES: MAXIMUS] ---
        
        public static int MaxWaveReached => maxWaveReached;
        public static ulong MaxScore => maxScore;
        public static ulong MaxMultiplier => maxMultiplier;
        public static float MaxPerfectRunTime => maxPerfectRunTime;

        #endregion

        #region --- [PROPERTIES: CURRENTS] ---

        public static Dictionary<ItemID, int> CurrentCollectedItems => currentCollectedItems;
        public static Dictionary<RobotType, int> CurrentRepairedRobots  => currentRepairedRobots;
        public static int CurrentPartShuffles => currentPartShuffles;
        public static float CurrentTimeInvincible => currentTimeInvincible;
        public static float CurrentTimeGained => currentTimeGained;
        public static float CurrentTimeLost => currentTimeLost;
        public static int CurrentRobotsRepaired => currentRobotsRepaired;
        public static int CurrentItemsCollected => currentItemsCollected;
        public static float CurrentPlayTime => currentPlayTime;
        public static ulong CurrentScore => currentScore;
        public static ulong CurrentMultiplier => currentMultiplier;
        public static int CurrentWave => currentWave;
        public static int CurrentHealthGained => currentHealthGained;
        public static int CurrentHealthLost => currentHealthLost;
        public static int CurrentArmorGained => currentArmorGained;
        public static int CurrentArmorLost => currentArmorLost;
        public static float CurrentLongestPerfectRunTime => currentLongestPerfectRunTime;
        public static float CurrentPerfectRunTime => currentPerfectRunTime;
        public static int CurrentCorrectPartsSelected => currentCorrectPartsSelected;
        public static int CurrentWrongPartsSelected => currentWrongPartsSelected;
        public static int CurrentRobotsDestroyed => currentRobotsDestroyed;

        public static int AdsWatched => adsWatched;
        public static int AdsSkipped => adsSkipped;

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [FIELDS: TOTALS] ---
        
        private static readonly Dictionary<ItemID, int> totalCollectedItems = new Dictionary<ItemID, int>();
       
        private static readonly Dictionary<RobotType, int> totalRepairedRobots = new Dictionary<RobotType, int>();
        
        private static int totalPartShuffles;
        
        private static float totalTimeGained;
        
        private static float totalTimeInvincible;
        
        private static float totalTimeLost;
        
        private static int totalItemsCollectedAmount;
        
        private static int totalRobotsRepairedAmount;

        private static float totalPlayTime;
        
        private static int totalPlayAttempts;

        private static int totalHealthGained;

        private static int totalHealthLost;

        private static int totalArmorGained;

        private static int totalArmorLost;
        
        private static int totalCorrectPartsSelected;

        private static int totalWrongPartsSelected;

        private static int totalRobotsDestroyed;

        private static int totalWavesEndured;
        
        #endregion

        #region --- [FIELDS: MAX] ---

       
        private static int maxWaveReached;
        
        private static ulong maxScore;
        
        private static ulong maxMultiplier;
        
        private static float maxPerfectRunTime;

        #endregion
        
        #region --- [FIELDS: CURRENTS] ---

        private static readonly Dictionary<ItemID, int> currentCollectedItems = new Dictionary<ItemID, int>();
        
        private static readonly Dictionary<RobotType, int> currentRepairedRobots = new Dictionary<RobotType, int>();
        
        private static int currentPartShuffles;
        
        private static float currentTimeGained;

        private static float currentTimeInvincible;
        
        private static float currentTimeLost;
        
        private static int currentRobotsRepaired;
        
        private static int currentItemsCollected;

        private static float currentPlayTime;
        
        private static ulong currentScore;
        
        private static ulong currentMultiplier;

        private static ulong currentMaxMultiplier;

        private static int currentWave;
        
        private static int currentHealthGained;
        
        private static int currentHealthLost;
        
        private static int currentArmorGained;
        
        private static int currentArmorLost;

        private static float currentPerfectRunTime;
        
        private static float currentLongestPerfectRunTime;

        private static int currentCorrectPartsSelected;
        
        private static int currentWrongPartsSelected;
 
        private static int currentRobotsDestroyed;


        private static int adsWatched;

        private static int adsSkipped;
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [STAT UPDATE] ---
        
        private static void UpdatePartShuffle()
        {
            if(!isPlaying) return;
            totalPartShuffles++;
            currentPartShuffles++;
        }
        
        private static void UpdateTimeGainedStat(float timeGained)
        {
            if(!isPlaying) return;
            currentTimeGained += timeGained;
            totalTimeGained += timeGained;
        }
        
        private static void UpdateTimeLostStat(float timeLost)
        {
            if(!isPlaying) return;
            currentTimeLost += timeLost;
            totalTimeLost += timeLost;
        }
        

        private static void ResetPerfectRunTime()
        {
            currentPerfectRunTime = 0;
        }

        private static void UpdateRobotsDestroyed(RobotBehaviour instance, bool visualOnly)
        {
            if(!isPlaying) return;
            if(visualOnly) return;
            
            ResetPerfectRunTime();
            
            totalRobotsDestroyed++;
            currentRobotsDestroyed++;
        }

        private static void UpdateScoreStats(ulong newScore)
        {
            if(!isPlaying) return;
            currentScore = newScore;
            
            if (currentScore > maxScore)
            {
                maxScore = currentScore;
            }
        }
        
        private static void UpdateMultiplierStats(ulong newMultiplier)
        {
            if(!isPlaying) return;
            currentMultiplier = newMultiplier;

            if (currentMultiplier > currentMaxMultiplier)
            {
                currentMaxMultiplier = currentMultiplier;
                if (currentMaxMultiplier > maxMultiplier)
                {
                    maxMultiplier = currentMaxMultiplier;
                }
            }
        }
        
        private static void UpdateWaveStats(int currentWaveReached)
        {
            if(!isPlaying) return;
            currentWave = currentWaveReached;
            totalWavesEndured++;
            
            if (maxWaveReached < currentWave)
            {
                maxWaveReached = currentWave;
            }
        }

        private static void UpdateHealthGained(int amount = 1, bool reset = false)
        {
            if(!isPlaying) return;
            if(reset) return;
            
            currentHealthGained += amount;
            totalHealthGained += amount;
        }

        private static void UpdateHealthLost(int amount = 1, bool reset = false)
        {
            if(!isPlaying) return;
            if(reset) return;
            
            currentHealthLost += amount;
            totalHealthLost += amount;
        }

        private static void UpdateArmorGained(int amount = 1, bool reset = false)
        {
            if(!isPlaying) return;
            if(reset) return;
            
            currentArmorGained += amount;
            totalArmorGained += amount;
        }

        private static void UpdateArmorLost(int amount = 1, bool reset = false)
        {
            if(!isPlaying) return;
            if(reset) return;
            
            currentArmorLost += amount;
            totalArmorLost += amount;
        }


                
        private static void UpdateCorrectPartsSelected(RobotBehaviour instance, bool completion, bool visualOnly)
        {
            if(!isPlaying) return;
            if(visualOnly) return;
            
            currentCorrectPartsSelected++;
            if (currentCorrectPartsSelected > totalCorrectPartsSelected)
            {
                totalCorrectPartsSelected = currentCorrectPartsSelected;
            }

            if (completion)
            {
                IncreaseRobotRepaired(instance);
            }
        }
        
                
        private static void UpdateIncorrectPartsSelected()
        {
            if(!isPlaying) return;
            ResetPerfectRunTime();
            currentWrongPartsSelected++;
            if (currentWrongPartsSelected > totalWrongPartsSelected)
            {
                totalWrongPartsSelected = currentWrongPartsSelected;
            }
        }
        
        
        
        private static void IncreaseRobotRepaired(RobotBehaviour robot)
        {
            if(!isPlaying) return;
            
            // --- CURRENT GAME ---
            if (currentRepairedRobots.ContainsKey(robot.Type))
            {
                currentRepairedRobots[robot.Type]++;
            }
            else
            {
                currentRepairedRobots.Add(robot.Type, 1);
            }
            
            // --- TOTAL ---
            if (totalRepairedRobots.ContainsKey(robot.Type))
            {
                totalRepairedRobots[robot.Type]++;
            }
            else
            {
                totalRepairedRobots.Add(robot.Type, 1);
            }
            
            currentRobotsRepaired++;
            totalRobotsRepairedAmount++;
        }

        private static void IncreaseItemCollected(CollectableItem item)
        {
            if(!isPlaying) return;
            
            // --- CURRENT GAME ---
            if (currentCollectedItems.ContainsKey(item.ItemID))
            {
                currentCollectedItems[item.ItemID]++;
            }
            else
            {
                currentCollectedItems.Add(item.ItemID, 1);
            }
            
            // --- TOTAL ---
            if (totalCollectedItems.ContainsKey(item.ItemID))
            {
                totalCollectedItems[item.ItemID]++;
            }
            else
            {
                totalCollectedItems.Add(item.ItemID, 1);
            }
            
            currentItemsCollected++;
            totalItemsCollectedAmount++;
        }
        
        private static void UpdateTimeBasedStats()
        {
            if(!isPlaying) return;

            var time = Time.deltaTime;
            
            currentPlayTime += time;
            totalPlayTime += time;
            currentPerfectRunTime += time;

            if (PlayerHealthHandler.IsInvincible)
            {
                currentTimeInvincible += time;
                totalTimeInvincible += time;
            }
            
            if (currentPerfectRunTime > currentLongestPerfectRunTime)
            {
                currentLongestPerfectRunTime = currentPerfectRunTime;
            }

            if (currentPerfectRunTime > maxPerfectRunTime)
            {
                maxPerfectRunTime = currentPerfectRunTime;
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [INIT] ---

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]        
        private static void Initialize()
        {
            #if UNITY_EDITOR
            if(DeletePlayerPrefsOnLoad)
                PlayerPrefs.DeleteAll();
            #endif

            LoadStats();
            
            UnityEventCallbacks.AddEventListener(UpdateTimeBasedStats, UnityEventType.Update);
            UnityEventCallbacks.AddEventListener(SafeStats, UnityEventType.ApplicationQuit);
            UnityEventCallbacks.AddEventListener(LoadStats, UnityEventType.Awake);
            
            GameController.OnGameStateChanged += state =>
            {
                isPlaying = state == GameState.Playing;
            };
            
            
            EventController.OnGameStarted += OnGameStarted;
            EventController.OnGameEnded += OnGameEnded;
            EventController.OnWaveStared += UpdateWaveStats;

            ShuffleCountdown.OnTimeGained += UpdateTimeGainedStat;
            ShuffleCountdown.OnTimeLost += UpdateTimeLostStat;
            ShuffleCountdown.OnPartsShuffle += UpdatePartShuffle;
            
            CollectableItem.OnItemCollectedEvent += IncreaseItemCollected;

            PlayerHealthHandler.OnArmorGained += UpdateArmorGained;
            PlayerHealthHandler.OnArmorLost += UpdateArmorLost;
            PlayerHealthHandler.OnHealthGained += UpdateHealthGained;
            PlayerHealthHandler.OnHealthLost += UpdateHealthLost;
            
            PlayerHealthHandler.OnPlayerInvincibilityChanged += OnPlayerInvincibilityChanged;

            ScoreHandler.OnMultiplierChanged += UpdateMultiplierStats;
            ScoreHandler.OnScoreChanged += UpdateScoreStats;

            RobotBehaviour.OnRobotDestroyed += UpdateRobotsDestroyed;
            RobotBehaviour.OnRobotPartAdded += UpdateCorrectPartsSelected;

            RobotPartButton.OnWrongRobotPartSelected += UpdateIncorrectPartsSelected;

            WaveController.OnNextWaveStarted += UpdateWaveStats;
            
            AdvertisementHelper.UnityAdsDidFinish += UpdateAdvertisementStats;
        }

        private static void UpdateAdvertisementStats(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Skipped:
                    adsSkipped++;
                    break;
                case ShowResult.Finished:
                    adsWatched++;
                    break;
                
                case ShowResult.Failed:
                    break;
            }
        }


        private static void OnPlayerInvincibilityChanged(bool obj)
        {
            
        }

        private static void OnGameStarted()
        {
            //--- STATS ---
            totalPlayAttempts++;
            
            //--- RESET ---
            currentCollectedItems.Clear();
            currentRepairedRobots.Clear();
            foreach (ItemID enumValue in Enum.GetValues(typeof(ItemID)))
            {
                if(enumValue == ItemID.None) continue;
                currentCollectedItems.Add(enumValue, 0);
            }
            foreach (RobotType enumValue in Enum.GetValues(typeof(RobotType)))
            {
                currentRepairedRobots.Add(enumValue, 0);
            }
            
            currentPlayTime = 0;
            currentItemsCollected = 0;
            currentRobotsRepaired = 0;
            currentWrongPartsSelected = 0;
            currentScore = 0;
            currentMultiplier = 0;
            currentMaxMultiplier = 0;
            currentWave = 0;
            currentHealthGained = 0;
            currentHealthLost = 0;
            currentArmorGained = 0;
            currentArmorLost = 0;
            currentPerfectRunTime = 0;
            currentCorrectPartsSelected = 0;
            currentWrongPartsSelected = 0;
            currentRobotsDestroyed = 0;
            currentTimeGained = 0;
            currentTimeLost = 0;
            currentTimeInvincible = 0;
            currentPartShuffles = 0;
        }
        
        private static void OnGameEnded(bool wasAborted)
        {
            SafeStats();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [SAFE AND LOAD] ---

        
        private static void SafeStats()
        {
            if(!Application.isPlaying) return;
            
            #region --- [MAXIMUMS]

            PlayerPrefs.SetInt(nameof(maxWaveReached), maxWaveReached);
            
            PlayerPrefs.SetString(nameof(maxScore), maxScore.ToString());
            PlayerPrefs.SetString(nameof(maxMultiplier), maxMultiplier.ToString());
            
            PlayerPrefs.SetFloat(nameof(maxPerfectRunTime), maxPerfectRunTime);

            #endregion
            
            #region --- [TOTALS]

            SafeDictionary(nameof(totalRepairedRobots), totalRepairedRobots);
            SafeDictionary(nameof(totalCollectedItems), totalCollectedItems);
            PlayerPrefs.SetFloat(nameof(totalPlayTime), totalPlayTime);
            PlayerPrefs.SetFloat(nameof(totalTimeGained), totalTimeGained);
            PlayerPrefs.SetFloat(nameof(totalTimeLost), totalTimeLost);
            PlayerPrefs.SetFloat(nameof(totalTimeInvincible), totalTimeInvincible);
            PlayerPrefs.SetInt(nameof(totalPartShuffles), totalPartShuffles);
            PlayerPrefs.SetInt(nameof(totalRobotsRepairedAmount), totalRobotsRepairedAmount);
            PlayerPrefs.SetInt(nameof(totalItemsCollectedAmount), totalItemsCollectedAmount);
            PlayerPrefs.SetInt(nameof(totalHealthGained), totalHealthGained);
            PlayerPrefs.SetInt(nameof(totalHealthLost), totalHealthLost);
            PlayerPrefs.SetInt(nameof(totalArmorGained), totalArmorGained);
            PlayerPrefs.SetInt(nameof(totalArmorLost), totalArmorLost);
            PlayerPrefs.SetInt(nameof(totalCorrectPartsSelected), totalCorrectPartsSelected);
            PlayerPrefs.SetInt(nameof(totalWrongPartsSelected), totalWrongPartsSelected);
            PlayerPrefs.SetInt(nameof(totalRobotsDestroyed), totalRobotsDestroyed);
            PlayerPrefs.SetInt(nameof(totalWavesEndured), totalWavesEndured);
            PlayerPrefs.SetInt(nameof(totalPlayAttempts), totalPlayAttempts);
            
            PlayerPrefs.GetInt(nameof(adsWatched), adsWatched);
            PlayerPrefs.GetInt(nameof(adsSkipped), adsSkipped);
            
            #endregion
            
            PlayerPrefs.Save();
        }

        private static void LoadStats()
        {
            if(!Application.isPlaying) return;
            
            #region --- [MAXIMUMS]

            maxWaveReached = PlayerPrefs.GetInt(nameof(maxWaveReached), maxWaveReached);
            maxScore = ulong.Parse(PlayerPrefs.GetString(nameof(maxScore), maxScore.ToString()));
            maxMultiplier = ulong.Parse(PlayerPrefs.GetString(nameof(maxMultiplier), maxMultiplier.ToString()));
            maxPerfectRunTime = PlayerPrefs.GetFloat(nameof(maxPerfectRunTime), maxPerfectRunTime);

            #endregion
            
            #region --- [TOTALS]

            LoadDictionary(nameof(totalRepairedRobots), totalRepairedRobots);
            LoadDictionary(nameof(totalCollectedItems), totalCollectedItems);
            
            totalPlayTime = PlayerPrefs.GetFloat(nameof(totalPlayTime), totalPlayTime);
            totalRobotsRepairedAmount = PlayerPrefs.GetInt(nameof(totalRobotsRepairedAmount), totalRobotsRepairedAmount);
            totalItemsCollectedAmount = PlayerPrefs.GetInt(nameof(totalItemsCollectedAmount), totalItemsCollectedAmount);
            totalTimeGained = PlayerPrefs.GetFloat(nameof(totalTimeGained), totalTimeGained);
            totalTimeLost = PlayerPrefs.GetFloat(nameof(totalTimeLost), totalTimeLost);
            totalTimeInvincible = PlayerPrefs.GetFloat(nameof(totalTimeInvincible), totalTimeInvincible);
            totalPartShuffles = PlayerPrefs.GetInt(nameof(totalPartShuffles), totalPartShuffles);
            totalHealthGained = PlayerPrefs.GetInt(nameof(totalHealthGained), totalHealthGained);
            totalHealthLost = PlayerPrefs.GetInt(nameof(totalHealthLost), totalHealthLost);
            totalArmorGained = PlayerPrefs.GetInt(nameof(totalArmorGained), totalArmorGained);
            totalArmorLost = PlayerPrefs.GetInt(nameof(totalArmorLost), totalArmorLost);
            totalCorrectPartsSelected = PlayerPrefs.GetInt(nameof(totalCorrectPartsSelected), totalCorrectPartsSelected);
            totalWrongPartsSelected = PlayerPrefs.GetInt(nameof(totalWrongPartsSelected), totalWrongPartsSelected);
            totalRobotsDestroyed = PlayerPrefs.GetInt(nameof(totalRobotsDestroyed), totalRobotsDestroyed);
            totalWavesEndured = PlayerPrefs.GetInt(nameof(totalWavesEndured), totalWavesEndured);
            totalPlayAttempts = PlayerPrefs.GetInt(nameof(totalPlayAttempts), totalPlayAttempts);
            
            adsWatched = PlayerPrefs.GetInt(nameof(adsWatched), adsWatched);
            adsSkipped = PlayerPrefs.GetInt(nameof(adsSkipped), adsSkipped);

            #endregion
        }

        private static void SafeDictionary<TKey>(string name, Dictionary<TKey,int> dictionary) where TKey : Enum
        {
            var data = "";
            
            foreach (var pair in dictionary)
            {
                var key = 0;
                foreach (var enumVar in Enum.GetValues(typeof(TKey)))
                {
                    if (Equals(enumVar, pair.Key))
                    {
                        key = (int) enumVar;
                    }
                }

                data += $"{key}&{pair.Value}§";
            }

            data = data.RemoveFormEnd(1);
            
            PlayerPrefs.SetString(name, data);
        }
        
        private static void LoadDictionary<TKey>(string name, IDictionary<TKey, int> dictionary) where TKey : Enum
        {
            if(!PlayerPrefs.HasKey(name)) return;

            try
            {
                var data = PlayerPrefs.GetString(name);
                if(string.IsNullOrWhiteSpace(data)) return;
                
                foreach (var element in data.Split('§'))
                {
                    var split = element.Split('&');
                    var intKey = int.Parse(split[0]);
                    
                    // ReSharper disable once PossibleInvalidCastException
                    var key = (TKey)(object)intKey;
                
                    dictionary.Add(key, int.Parse(split[1]));
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [EDITOR] ---
        
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Help/Delete PlayerPrefs On Load")]
        public static bool DeletePlayerPrefs() => (DeletePlayerPrefsOnLoad = !DeletePlayerPrefsOnLoad);

        private static bool DeletePlayerPrefsOnLoad
        {
            get => UnityEditor.EditorPrefs.GetBool(nameof(DeletePlayerPrefsOnLoad), false);
            set => UnityEditor.EditorPrefs.SetBool(nameof(DeletePlayerPrefsOnLoad), value);
        }
        
        [UnityEditor.MenuItem("Help/Delete PlayerPrefs On Load", true)]
        private static bool DeletePlayerPrefsValidate()
        {
            UnityEditor.Menu.SetChecked("Help/Delete PlayerPrefs On Load", DeletePlayerPrefsOnLoad);
            return true;
        }

#endif

        #endregion
    }
}
