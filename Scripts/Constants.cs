// Copyright 2023 bitHeads, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;

public static class Constants
{
    // BrainCloud constants
    public const string kBrainCloudServer = "https://api.braincloudservers.com/dispatcherv2";
    public const string kBrainCloudAppID = ""; // App ID is empty on purpose
    public const string kBrainCloudAppSecret = "";  // App secret is empty on purpose
    public const string kBrainCloudAppName = "Godot BootCamp";
    public const string kBrainCloudAppVersion = "1.0.1";

    public const string kBrainCloudMainLeaderboardID = "Main";
    public const int kBrainCloudDefaultMinHighScoreIndex = 0;
    public const int kBrainCloudDefaultMaxHighScoreIndex = 9;
    public const string kBrainCloudGlobalEntityIndexedID = "Level";
    public const string kBrainCloudStatGamesPlayed = "GamesCompleted";
    public const string kBrainCloudStatEnemiesKilled = "EnemiesKilled";
    public const string kBrainCloudStatAsteroidDestroyed = "AsteroidsDestroyed";

    public static readonly Dictionary<string, string> kBrainCloudStatDescriptions = new Dictionary<string, string> {{kBrainCloudStatGamesPlayed, "Games completed"},
                                                                                                                    {kBrainCloudStatEnemiesKilled, "Enemies killed"},
                                                                                                                    {kBrainCloudStatAsteroidDestroyed, "Asteroids destroyed"}};

    public const string kBrainCloudAchievementSurvive30 = "Survive30";
    public const string kBrainCloudAchievementSurvive60 = "Survive60";
    public const string kBrainCloudAchievementBeatLevel1 = "BeatLevel1";
    public const string kBrainCloudUserProgressUserEntityType = "userProgress";
    public const string kBrainCloudUserProgressUserEntityDefaultData = "{\"levelOneCompleted\":\"false\",\"levelTwoCompleted\":\"false\",\"levelThreeCompleted\":\"false\",\"levelBossCompleted\":\"false\"}";
    public const string kBrainCloudUserProgressUserEntityDefaultAcl = "{\"other\":0}";
    public const string kBrainCloudExternalAuthTwitch = "TwitchAuthentication";


    // Twitch constants
    public const string kTwitchAuthUrl = "https://id.twitch.tv/oauth2/authorize";
    public const string kTwitchClientId = "";       // Client ID is empty on purpose
    public const string kTwitchClientSecret = "";   // Secret is empty on purpose
    public const string kTwitchRedirectUrl = "http://localhost:3030/"; 


    // General constants
    public const float kOffScreenSpawnBuffer = 128.0f;
    public const float kBackgroundSpeed = 250.0f;
    public const double kEndOfGameDisplayDuration = 2.5;
    public const double kLevelDisplayDuration = 2.5;
    public const int kHordeModeLevelOne = 0;
    public const int kHordeModeLevelTwo = 1;
    public const int kHordeModeLevelThree = 2;
    public const int kHordeModeLevelBoss = 3;


    // Object pool constants
    public const int kAsteroidPoolSize = 50;
    public const int kEnemyPoolSize = 18;
    public const int kMissilePoolSize = 20;
    public const int kLaserPoolSize = 50;
    public const int kLaserImpactPoolSize = kLaserPoolSize;
    public const int kExplosionPoolSize = 15;
    public const int kShipWingPoolSize = (kEnemyPoolSize + 1) * 2;
    public const int kPickupPoolSize = 4;


    // HUD constants
    public const int kHudHeight = 48;
    public const float kHudStatusBarInset = 5.0f;
    public const float kHudSmallBuffer = 2.0f;
    public const float kHudBigBuffer = 6.0f;
    public const float kHudTextInset = kHudBigBuffer * 4.0f;
    public const float kHudDangerFlashIncrement = (float)Math.PI;
    public const float kHudHighScoreScissorWidth = 280.0f;
    public const float kHudHighScoreMovementSpeed = 150.0f;


	// Ship constants
    public const string kShipType = "Ship";
    public const string kShieldType = "ShieldType";
    public const int kShipInitialHealth = 4;
    public const int kShipInitialShieldHealth = 4;
    public const double kShipInvincibilityDuration = 0.75;
    public const float kShipInvincibleAlpha = 0.65f;
    public const float kShipOffScreenSpawnX = -100.0f;
    public const float kShipSpawnX = 100.0f;
    public const float kShipAcceleration = 30.0f;
    public const float kShipDrag = 0.8f;
    public const float kShipMaxSpeed = 900.0f;
    public const float kShipMinX = 100.0f;
    public const float kShipMaxOffsetX = 200.0f;
    public const float kShipTurnTilt = 0.34906f;        // 20 degrees
    public const float kShipGunOffset = 10.0f;
    public const float kShipGunAngleTilt = 0.0209436f;  // 1.2 degrees
    public const float kShipCircleCollisionRadius = 20.0f;
    public const float kShipEdgeCollisionAngle = 2.094395f;  // 120 degrees
    public const float kShipEdgeCollisionMagnitude = 42.0f;


    // Asteroid constants
    public const string kAsteroidType = "Asteroid";
    public const float kAsteroidMinSpawnRadians = 2.792f;  // 160 degrees
    public const float kAsteroidMaxSpawnRadians = 3.49f;   // 200 degrees
    public const float kAsteroidMinSpeed = 100.0f;
    public const float kAsteroidMaxSpeed = 300.0f;
    public const float kAsteroidExplosionMinSpeed = 150.0f;
    public const float kAsteroidExplosionMaxSpeed = 325.0f;
    public const double kAsteroidExplosionFadeOutTime = 1.0;
    public const double kAsteroidExplosionFadeDelay = 0.25;
    public const float kAsteroidMinAngularVelocity = (float)Math.PI * 0.5f;               // 90 degrees per second
    public const float kAsteroidMaxAngularVelocity = (float)Math.PI + (float)Math.PI * 0.5f;    // 270 degrees per second
    public const double kAsteroidMinSpawnTime = 0.8;
    public const double kAsteroidMaxSpawnTime = 2.5;
    public const int kAsteroidMinSpawnCount = 1;
    public const int kAsteroidMaxSpawnCount = 5;
    public const int kNumAsteroidSizes = 4;
	public static readonly int[] kNumAsteroidVariations = { 3, 2, 2, 2 };
    public static readonly string[] kBigAsteroidAtlasKeys = { "AsteroidBig-1", "AsteroidBig-2", "AsteroidBig-3" };
    public static readonly string[] kMediumAsteroidAtlasKeys = { "AsteroidMedium-1", "AsteroidMedium-2" };
    public static readonly string[] kSmallAsteroidAtlasKeys = { "AsteroidSmall-1", "AsteroidSmall-2" };
    public static readonly string[] kTinyAsteroidAtlasKeys = { "AsteroidTiny-1", "AsteroidTiny-2" };
    public static readonly int[] kAsteroidHealth = { 2, 1, 0, 0};
    public static readonly int[] kAsteroidAttackDamage = { 2, 1, 0, 0};


    // Ship wing constants
    public const string kShipWingType = "ShipWing";
    public const float kShipWingExplosionMinSpeed = 175.0f;
    public const float kShipWingExplosionMaxSpeed = 250.0f;
    public const double kShipWingExplosionFadeOutTime = 1.0;
    public const float kShipWingExplosionOffset = 5.0f;
    public const float kShipWingMinAngularVelocity = MathF.PI * 0.25f;         // 45 degrees per second
    public const float kShipWingMaxAngularVelocity = MathF.PI * 0.5f;  // 90 degrees per second


	// Laser constants
    public const string kLaserType = "Laser";
    public const string kLaserImpactType = "LaserImpact";
    public const float kLaserSpeed = 1200.0f;
    public const double kLaserImpactLifetime = 0.05;


    // Missile constants
     public const string kMissileType = "Missile";
    public const float kMissileSmallSpeed = 1000.0f;
    public const float kMissileBigSpeed = 750.0f;
    public const int kMissileSmallAttackDamage = 2;
    public const int kMissileBigAttackDamage = 4;

    
    // Enemy constants
    public const string kEnemyType = "Enemy";
    public const int kEnemyMinSpawnCount = 1;
    public const int kEnemyMaxSpawnCount = 3;
    public const float kEnemyMinSpeed = 200.0f;
    public const float kEnemyMaxSpeed = 300.0f;
    public const float kEnemyMinSpawnAngle = 2.96701f;  // 170 Degrees;
    public const float kEnemyMaxSpawnAngle = 3.31607f;  // 190 Degrees;
    public const double kEnemyLaserMinDelay = 0.75;
    public const double kEnemyLaserMaxDelay = 2.0;
    public const double kEnemyMinSpawnTime = 1.5;
    public const double kEnemyMaxSpawnTime = 3.5;
    public const double kEnemyThreeFireDelay = 0.35;
    public const double kEnemyFourFireDelay = 0.1;
    public const double kEnemyFourFiringDuration = 1.25;
    public const double kEnemyFourFiringCooldown = 0.5;
    public const double kEnemyFiveFiringMinCooldown = 1.75;
    public const double kEnemyFiveFiringMaxCooldown = 3.0;
    public const double kEnemyFiveMissileMinDelay = 0.8;
    public const double kEnemyFiveMissileMaxDelay = 1.4;
    public const double kEnemyFiveLaserMinDelay = 0.4;
    public const double kEnemyFiveLaserMaxDelay = 1.2;
    public const int kNumEnemyTypes = 5;
    public static readonly string[] kEnemyAtlasKeys = { "EnemyShip-1", "EnemyShip-2", "EnemyShip-3", "EnemyShip-4", "EnemyShip-5" };
    public static readonly string[] kEnemyWingLeftAtlasKeys = { "EnemyShipLeftWing-1", "EnemyShipLeftWing-2", "EnemyShipLeftWing-3", "EnemyShipLeftWing-4", "EnemyShipLeftWing-5" };
    public static readonly string[] kEnemyWingRightAtlasKeys = { "EnemyShipRightWing-1", "EnemyShipRightWing-2", "EnemyShipRightWing-3", "EnemyShipRightWing-4", "EnemyShipRightWing-5" };
    public const float kEnemyGunOffset1 = 16.0f;
    public const float kEnemyGunOffset2 = 6.0f;
    public const float kEnemyGunOffset3 = 18.0f;
    public const float kEnemyGunOffset4_1 = 21.0f;
    public const float kEnemyGunOffset4_2 = 27.0f;
    public const float kEnemyGunOffset5 = 25.0f;
    public const int kEnemyInitialHealth1 = 2;
    public const int kEnemyInitialHealth2 = 3;


    // Boss constants
    public const string kBossType = "Boss";
    public const string kBossFrontLeftWingAtlasKey = "BossFrontLeftWing";
    public const string kBossFrontRightWingAtlasKey = "BossFrontRightWing";
    public const string kBossMiddleLeftWingAtlasKey = "BossMiddleLeftWing";
    public const string kBossMiddleRightWingAtlasKey = "BossMiddleRightWing";
    public const string kBossBackLeftWingAtlasKey = "BossBackLeftWing";
    public const string kBossBackRightWingAtlasKey = "BossBackRightWing";
    public const float kBossWingExplosionSpeed = 500.0f;
    public const float kBossWingExplosionOffset = 40.0f;
    public const float kBossOffScreenSpawnX = 1450.0f;
    public const float kBossSpawnX = 950.0f;
    public const float kBossMinX = 450.0f;
    public const double kBossMovementMinDelay = 0.05;
    public const double kBossMovementMaxDelay = 0.2;
    public const float kBossMovementMinRange = 200.0f;
    public const float kBossMovementMaxRange = 600.0f;
    public const double kBossSmallMissileMinDelay = 0.6;
    public const double kBossSmallMissileMaxDelay = 1.2;
    public const double kBossBigMissileMinDelay = 1.2;
    public const double kBossBigMissileMaxDelay = 2.0;
    public const float kBossSpeed = 450.0f;
    public const int kBossHealth = 40;
    public const int kBossAttackDamage = 10;
    public const float kBossGunOffset1 = 64.0f;
    public const float kBossGunOffset2 = 56.0f;


    // Pickup constants
    public const string kPickupType = "Pickup";
    public const double kPickupLifetime = 5.0;
    public const double kPickupFadeOutTime = 0.5;


    // Explosion constants
    public const string kExplosionType = "Explosion";
}
