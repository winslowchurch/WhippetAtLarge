using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Honeyspur;

public class Player
{
    #region Constants
    private const int ColliderWidth = 12;
    private const int ColliderHeight = 8;
    private const float EATING_DURATION = 1.0f;
    private const float EATING_FRAME_DURATION = 0.5f;
    private const float TOOL_USE_DURATION = 0.45f;
    #endregion

    #region Position & Movement
    public Vector2 Position;
    public Vector2 Velocity;
    public int Width = 16, Height = 32;
    public string DirectionFacing = "down";
    public bool IsMoving => Velocity != Vector2.Zero;
    private float speed = 1.3f;
    #endregion

    #region Health
    public int Health { get; set; } = 100;
    public int MaxHealth { get; set; } = 100;
    #endregion

    #region Base Sprites & Animation
    private Texture2D spriteSheet;
    private readonly Dictionary<string, Animation> animations = [];
    private Animation currentAnimation;
    private SpriteEffects spriteEffect = SpriteEffects.None;
    private int CurrentAnimationFrame => currentAnimation?.GetCurrentFrameIndex() ?? 0;

    public Texture2D GetSpriteSheet() => spriteSheet;
    public Rectangle GetCurrentFrameRect() => PlayerSprites.Body.GetFrame(DirectionFacing, currentAnimation.GetCurrentFrameValue());
    #endregion

    #region Wearable Items & Customization
    private WearableItem currentHair;
    private WearableItem currentShirt;
    private WearableItem currentHat;
    private ShoeItem currentShoes;
    private PantsItem currentPants;

    private Texture2D hairSpriteSheet;
    private Texture2D shirtSpriteSheet;
    private Texture2D hatSpriteSheet;
    private Texture2D shoesSpriteSheet;
    private Texture2D pantsSpriteSheet;

    private string currentHairStyle = "Double Braids";
    private string currentShirtStyle = "Seafoam Apron";
    private string currentHatStyle = null;
    private string currentShoeStyle = "Blue Sneakers";
    private string currentPantsStyle = "Jean Shorts";

    private Color currentHairColor = new(0xB5, 0x88, 0x68);
    private Color currentEyeColor = new(0x52, 0x8C, 0xD4);
    #endregion

    #region Animation States
    // Blinking
    private float blinkTimer;
    private float nextBlinkTime;
    private bool isBlinking;
    private bool forceEyesClosed = false;
    private float forceEyesClosedTimer = 0f;
    private Action forceEyesClosedCallback = null;

    // Eating
    private bool isEating = false;
    private float eatingTimer = 0f;
    private string savedDirectionFacing = "down";

    // Tool Use
    private bool isUsingTool = false;
    private float toolUseTimer = 0f;
    private string toolUseDirection = "down";
    private Items.ToolType currentToolType = Items.ToolType.None;
    #endregion

    #region Effects & Audio
    private UI.SparkleEffect sparkleEffect;

    private SoundEffect footstepGrassSound;
    private SoundEffect footstepSnowSound;
    private SoundEffect footstepWoodSound;
    private SoundEffectInstance footstepInstance;
    private string currentFootstepType = null;
    #endregion

    #region Utilities
    private readonly Random random = new();
    #endregion

    #region Content Loading
    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
    {
        LoadSpritesAndSounds(Content);
        SetupAnimations();
        LoadDefaultClothing();

        sparkleEffect = new UI.SparkleEffect();
        sparkleEffect.LoadContent(Content);

        currentAnimation = animations["idle-down"];
    }

    private void LoadSpritesAndSounds(ContentManager Content)
    {
        spriteSheet = TextureManager.GetTexture("graphics/player/base");
        hairSpriteSheet = TextureManager.GetTexture("graphics/player/hair");
        shirtSpriteSheet = TextureManager.GetTexture("graphics/player/shirts");
        hatSpriteSheet = TextureManager.GetTexture("graphics/player/hats");
        shoesSpriteSheet = TextureManager.GetTexture("graphics/player/shoes");
        pantsSpriteSheet = TextureManager.GetTexture("graphics/player/pants");

        footstepGrassSound = Content.Load<SoundEffect>("sounds/footstepGrass");
        footstepSnowSound = Content.Load<SoundEffect>("sounds/footstepSnow");
        footstepWoodSound = Content.Load<SoundEffect>("sounds/footstepWood");
        footstepInstance = footstepWoodSound.CreateInstance();
        footstepInstance.IsLooped = true;
        currentFootstepType = "wood";
    }

    private void SetupAnimations()
    {
        // Walk animations with bobbing pattern (1,2,1,3)
        animations["walk-down"] = new Animation([0, 1, 0, 2], 0, 8);
        animations["walk-right"] = new Animation([0, 1, 0, 2], 1, 8);
        animations["walk-up"] = new Animation([0, 1, 0, 2], 2, 8);
        animations["walk-left"] = new Animation([0, 1, 0, 2], 1, 8);

        // Idle animations
        animations["idle-down"] = new Animation([0], 0, 10);
        animations["idle-right"] = new Animation([0], 1, 10);
        animations["idle-up"] = new Animation([0], 2, 10);
        animations["idle-left"] = new Animation([0], 1, 10);
    }

    private void LoadDefaultClothing()
    {
        LoadHairStyle(currentHairStyle);
        LoadShirtStyle(currentShirtStyle);
        LoadHatStyle(currentHatStyle);
        LoadShoeStyle(currentShoeStyle);
        LoadPantsStyle(currentPantsStyle);
    }

    private T LoadStyleData<T>(string jsonPath) where T : StyleData<StyleInfo>
    {
        using var stream = TitleContainer.OpenStream(jsonPath);
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json);
    }

    private void LoadHairStyle(string styleName)
    {
        var data = LoadStyleData<HairstyleData>("Content/data/hair.json");
        var style = data.Items.First(h => h.Name == styleName);
        currentHair = new WearableItem
        {
            Name = styleName,
            SpriteColumn = style.SpriteColumn,
            SpriteSheet = hairSpriteSheet
        };
    }

    private void LoadShirtStyle(string styleName)
    {
        var data = LoadStyleData<ShirtStyleData>("Content/data/shirts.json");
        var style = data.Items.First(s => s.Name == styleName);
        currentShirt = new WearableItem
        {
            Name = styleName,
            SpriteColumn = style.SpriteColumn,
            SpriteSheet = shirtSpriteSheet,
            FrameWidth = 8,
            FrameHeight = 7,
            RowHeight = 7
        };
    }

    private void LoadHatStyle(string styleName)
    {
        if (string.IsNullOrEmpty(styleName))
        {
            currentHat = null;
            return;
        }

        var data = LoadStyleData<HatStyleData>("Content/data/hats.json");
        var style = data.Items.First(s => s.Name == styleName);
        currentHat = new WearableItem
        {
            Name = styleName,
            SpriteColumn = style.SpriteColumn,
            SpriteSheet = hatSpriteSheet,
            FrameWidth = 20,
            FrameHeight = 20,
            RowHeight = 20
        };
    }

    private void LoadShoeStyle(string styleName)
    {
        if (string.IsNullOrEmpty(styleName))
        {
            currentShoes = null;
            return;
        }

        var data = LoadStyleData<ShoeStyleData>("Content/data/shoes.json");
        var style = data.Items.First(s => s.Name == styleName);
        currentShoes = new ShoeItem
        {
            Name = styleName,
            SpriteColumn = style.SpriteColumn,
            SpriteSheet = shoesSpriteSheet
        };
    }

    private void LoadPantsStyle(string styleName)
    {
        if (string.IsNullOrEmpty(styleName))
        {
            currentPants = null;
            return;
        }

        var data = LoadStyleData<PantsStyleData>("Content/data/pants.json");
        var style = data.Items.First(s => s.Name == styleName);
        currentPants = new PantsItem
        {
            Name = styleName,
            SpriteColumn = style.SpriteColumn,
            SpriteSheet = pantsSpriteSheet
        };
    }
    #endregion

    #region Position Helpers
    public void SetTilePosition(int tileX, int tileY)
    {
        Position = new Vector2(
            tileX * TileDefinitions.TileSize,
            tileY * TileDefinitions.TileSize
        );
    }

    public bool IsTileWithinRange(Point tilePos)
    {
        Point playerTilePos = new(
            (int)Math.Round(Position.X / 16),
            (int)((Position.Y + Height - 1) / 16)
        );

        int dx = Math.Abs(tilePos.X - playerTilePos.X);
        int dy = Math.Abs(tilePos.Y - playerTilePos.Y);

        return dx <= 1 && dy <= 1;
    }

    public void FaceTowardsTile(Point tilePos)
    {
        Point playerTilePos = new(
            (int)Math.Round(Position.X / 16),
            (int)((Position.Y + Height - 1) / 16)
        );

        int dx = tilePos.X - playerTilePos.X;
        int dy = tilePos.Y - playerTilePos.Y;

        // Prioritize vertical direction over horizontal for diagonal clicks
        if (Math.Abs(dy) >= Math.Abs(dx))
        {
            DirectionFacing = dy < 0 ? "up" : "down";
        }
        else
        {
            DirectionFacing = dx < 0 ? "left" : "right";
        }
    }
    #endregion

    #region Update Logic
    public void Update(GameTime gameTime, GameContext context)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        UpdateAnimationStates(dt);
        UpdateMovement(gameTime, context);
        UpdateAnimationSelection();
        UpdateEffects(gameTime);
        UpdateFootstepAudio(context);
    }

    private void UpdateAnimationStates(float dt)
    {
        // Forced eye close timer
        if (forceEyesClosed)
        {
            forceEyesClosedTimer -= dt;
            if (forceEyesClosedTimer <= 0f)
            {
                forceEyesClosed = false;
                forceEyesClosedCallback?.Invoke();
                forceEyesClosedCallback = null;
            }
        }

        // Eating animation
        if (isEating)
        {
            eatingTimer += dt;
            if (eatingTimer >= EATING_DURATION)
            {
                isEating = false;
                eatingTimer = 0f;
                DirectionFacing = savedDirectionFacing;
            }
        }

        // Tool use animation
        if (isUsingTool)
        {
            toolUseTimer += dt;
            if (toolUseTimer >= TOOL_USE_DURATION)
            {
                isUsingTool = false;
                toolUseTimer = 0f;
            }
        }

        // Blinking
        blinkTimer += dt;
        if (blinkTimer >= nextBlinkTime)
        {
            isBlinking = true;
            blinkTimer = 0;
            nextBlinkTime = (float)(random.NextDouble() * 5 + 5);
        }
        else if (isBlinking && blinkTimer >= 0.15f)
        {
            isBlinking = false;
        }
    }

    private void UpdateMovement(GameTime gameTime, GameContext context)
    {
        // Skip movement if performing special animation
        if (isEating || isUsingTool)
        {
            Velocity = Vector2.Zero;
            return;
        }

        // Get input
        var k = Keyboard.GetState();
        float intendedVX = 0f, intendedVY = 0f;

        if (k.IsKeyDown(Keys.Left) || k.IsKeyDown(Keys.A))
            intendedVX = -speed;
        else if (k.IsKeyDown(Keys.Right) || k.IsKeyDown(Keys.D))
            intendedVX = speed;

        if (k.IsKeyDown(Keys.Up) || k.IsKeyDown(Keys.W))
            intendedVY = -speed;
        else if (k.IsKeyDown(Keys.Down) || k.IsKeyDown(Keys.S))
            intendedVY = speed;

        // Check collisions and apply movement
        bool canMoveX = CheckCollisionX(intendedVX, context);
        bool canMoveY = CheckCollisionY(intendedVY, context);

        // Update velocity and facing
        Velocity = Vector2.Zero;
        if (intendedVX != 0)
        {
            DirectionFacing = intendedVX < 0 ? "left" : "right";
            if (canMoveX)
                Velocity.X = intendedVX;
        }
        if (intendedVY != 0)
        {
            DirectionFacing = intendedVY < 0 ? "up" : "down";
            if (canMoveY)
                Velocity.Y = intendedVY;
        }

        // Apply movement
        if (Velocity.X != 0) Position.X += Velocity.X;
        if (Velocity.Y != 0) Position.Y += Velocity.Y;
    }

    private bool CheckCollisionX(float intendedVX, GameContext context)
    {
        if (intendedVX == 0) return true;

        Vector2 testPos = Position + new Vector2(intendedVX, 0);
        int leftTileX = (int)((testPos.X + (Width - ColliderWidth) / 2) / TileDefinitions.TileSize);
        int rightTileX = (int)((testPos.X + (Width + ColliderWidth) / 2 - 1) / TileDefinitions.TileSize);
        int topTileY = (int)((Position.Y + Height - ColliderHeight) / TileDefinitions.TileSize);
        int bottomTileY = (int)((Position.Y + Height - 1) / TileDefinitions.TileSize);

        for (int x = leftTileX; x <= rightTileX; x++)
            for (int y = topTileY; y <= bottomTileY; y++)
                if (!context.MapManager.IsWalkable(x, y))
                    return false;

        return true;
    }

    private bool CheckCollisionY(float intendedVY, GameContext context)
    {
        if (intendedVY == 0) return true;

        Vector2 testPos = Position + new Vector2(0, intendedVY);
        int leftTileX = (int)((Position.X + (Width - ColliderWidth) / 2) / TileDefinitions.TileSize);
        int rightTileX = (int)((Position.X + (Width + ColliderWidth) / 2 - 1) / TileDefinitions.TileSize);
        int topTileY = (int)((testPos.Y + Height - ColliderHeight) / TileDefinitions.TileSize);
        int bottomTileY = (int)((testPos.Y + Height - 1) / TileDefinitions.TileSize);

        for (int x = leftTileX; x <= rightTileX; x++)
            for (int y = topTileY; y <= bottomTileY; y++)
                if (!context.MapManager.IsWalkable(x, y))
                    return false;

        return true;
    }

    private void UpdateAnimationSelection()
    {
        if (isEating)
        {
            currentAnimation = animations["idle-down"];
        }
        else if (Velocity != Vector2.Zero)
        {
            currentAnimation = animations[$"walk-{DirectionFacing}"];
        }
        else
        {
            currentAnimation = animations[$"idle-{DirectionFacing}"];
        }
    }

    private void UpdateEffects(GameTime gameTime)
    {
        currentAnimation.Update(gameTime);
        sparkleEffect.Update(gameTime);
    }

    private void UpdateFootstepAudio(GameContext context)
    {
        if (IsMoving)
        {
            Point footTile = new(
                (int)Math.Round(Position.X / TileDefinitions.TileSize),
                (int)((Position.Y + Height - 1) / TileDefinitions.TileSize)
            );

            var baseTile = context.MapManager.GetBaseTileAt(footTile);
            bool isGrass = baseTile is Grass || baseTile is Dirt;

            string desired = isGrass ? "grass" : "wood";

            if (footstepInstance == null || footstepInstance.State != SoundState.Playing || currentFootstepType != desired)
            {
                if (footstepInstance != null && footstepInstance.State == SoundState.Playing)
                    footstepInstance.Stop();

                var chosen = footstepWoodSound;
                if (isGrass)
                {
                    if (context.DayManager.currentSeason == DayManager.Season.Winter)
                        chosen = footstepSnowSound;
                    else
                        chosen = footstepGrassSound;
                }

                footstepInstance = chosen.CreateInstance();
                footstepInstance.IsLooped = true;
                footstepInstance.Play();
                currentFootstepType = desired;
            }
        }
        else
        {
            if (footstepInstance != null && footstepInstance.State == SoundState.Playing)
            {
                footstepInstance.Stop();
                currentFootstepType = null;
            }
        }
    }
    #endregion

    #region Drawing
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteEffect = DirectionFacing == "left" ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Vector2 roundedPosition = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

        // Draw in layers from back to front
        DrawBaseBody(spriteBatch, roundedPosition);
        DrawEyesLayer(spriteBatch, roundedPosition);
        DrawClothingBottomLayers(spriteBatch, roundedPosition);
        DrawArmsLayer(spriteBatch, roundedPosition);
        DrawClothingTopLayers(spriteBatch, roundedPosition);
        DrawEffects(spriteBatch, roundedPosition);
    }

    private void DrawBaseBody(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        spriteBatch.Draw(
            spriteSheet,
            roundedPosition,
            GetCurrentFrameRect(),
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            spriteEffect,
            0f
        );
    }

    private void DrawEyesLayer(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        if (DirectionFacing != "up")
        {
            DrawEyes(spriteBatch, roundedPosition);
        }
    }

    private void DrawClothingBottomLayers(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        int animFrameValue = currentAnimation.GetCurrentFrameValue();

        // Draw pants
        if (currentPants != null)
        {
            var pantsOffset = new Vector2(0, Height - currentPants.FrameHeight);
            DrawWearable(spriteBatch, currentPants, roundedPosition + pantsOffset,
                useAnimationFrame: true, color: Color.White, effects: spriteEffect, frameValue: animFrameValue);
        }

        // Draw shirt
        if (currentShirt != null)
        {
            var shirtOffset = new Vector2(4, 15 + GetBobbingOffsetY());
            DrawWearable(spriteBatch, currentShirt, roundedPosition + shirtOffset,
                useAnimationFrame: false, color: Color.White, effects: SpriteEffects.None);
        }
    }

    private void DrawArmsLayer(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        if (isEating)
        {
            DrawEatingArms(spriteBatch, roundedPosition);
        }
        else if (isUsingTool)
        {
            DrawToolUseArms(spriteBatch, roundedPosition);
        }
        else
        {
            DrawNormalArms(spriteBatch, roundedPosition);
        }
    }

    private void DrawNormalArms(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        Rectangle armsRect = PlayerSprites.Arms.GetFrame(DirectionFacing, currentAnimation.GetCurrentFrameValue());
        spriteBatch.Draw(
            spriteSheet,
            roundedPosition,
            armsRect,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            spriteEffect,
            0f
        );
    }

    private void DrawEatingArms(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        int eatingFrame = (int)(eatingTimer / EATING_FRAME_DURATION);
        Rectangle eatingArmsRect = PlayerSprites.Eating.GetFrame(eatingFrame);
        spriteBatch.Draw(
            spriteSheet,
            roundedPosition,
            eatingArmsRect,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }

    private void DrawToolUseArms(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        if (currentToolType == Items.ToolType.Hoe)
        {
            DrawHoeAnimation(spriteBatch, roundedPosition);
        }
        else
        {
            DrawGenericToolAnimation(spriteBatch, roundedPosition);
        }
    }

    private void DrawHoeAnimation(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        int frameIndex = (int)(toolUseTimer / (TOOL_USE_DURATION / 2));
        if (frameIndex > 1) frameIndex = 1;

        int srcX = 128 + (frameIndex * 16);
        int srcY = toolUseDirection switch
        {
            "down" => 0,
            "right" => 32,
            "up" => 64,
            "left" => 32,
            _ => 0
        };

        Rectangle toolUseRect = new(srcX, srcY, 16, 32);
        SpriteEffects toolEffects = toolUseDirection == "left" ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        spriteBatch.Draw(
            spriteSheet,
            roundedPosition,
            toolUseRect,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            toolEffects,
            0f
        );
    }

    private void DrawGenericToolAnimation(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        int srcX = 112;
        int srcY = toolUseDirection == "right" ? 32 : 0;
        SpriteEffects toolEffects = SpriteEffects.None;

        if (toolUseDirection == "left")
        {
            srcY = 32;
            toolEffects = SpriteEffects.FlipHorizontally;
        }

        if (toolUseDirection != "up")
        {
            Rectangle toolUseRect = new Rectangle(srcX, srcY, 16, 32);
            spriteBatch.Draw(
                spriteSheet,
                roundedPosition,
                toolUseRect,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                toolEffects,
                0f
            );
        }
    }

    private void DrawClothingTopLayers(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        int animFrameValue = currentAnimation.GetCurrentFrameValue();

        // Draw hair
        if (currentHair != null)
        {
            var hairOffset = new Vector2(0, -1 + GetBobbingOffsetY());
            DrawWearable(spriteBatch, currentHair, roundedPosition + hairOffset,
                useAnimationFrame: false, color: currentHairColor, effects: SpriteEffects.None);
        }

        // Draw hat
        if (currentHat != null)
        {
            var hatOffset = new Vector2(-2, 0 + GetBobbingOffsetY());
            DrawWearable(spriteBatch, currentHat, roundedPosition + hatOffset,
                useAnimationFrame: false, color: Color.White, effects: SpriteEffects.None);
        }

        // Draw shoes
        if (currentShoes != null)
        {
            var shoesOffset = new Vector2(0, Height - currentShoes.FrameHeight);
            DrawWearable(spriteBatch, currentShoes, roundedPosition + shoesOffset,
                useAnimationFrame: true, color: Color.White, effects: spriteEffect, frameValue: animFrameValue);
        }
    }

    private void DrawEffects(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        const float sparkleScale = 0.8f;
        Vector2 basePos = new(roundedPosition.X, roundedPosition.Y);
        Vector2 leftSparklePos = basePos + new Vector2(-3, -5);
        Vector2 rightSparklePos = basePos + new Vector2(Width - 6, 0);

        sparkleEffect.DrawSparkles(spriteBatch, leftSparklePos, rightSparklePos, sparkleScale, Color.White, spriteRow: 2);
    }

    private int GetBobbingOffsetY()
    {
        if (IsMoving && (CurrentAnimationFrame == 1 || CurrentAnimationFrame == 3))
            return 1;
        return 0;
    }

    private void DrawWearable(SpriteBatch spriteBatch, WearableItem item, Vector2 position,
        bool useAnimationFrame, Color color, SpriteEffects effects, int frameValue = 0)
    {
        if (item == null) return;

        Rectangle src = useAnimationFrame
            ? item.GetFrameRect(DirectionFacing, frameValue)
            : item.GetFrameRect(DirectionFacing);

        spriteBatch.Draw(
            item.SpriteSheet,
            position,
            src,
            color,
            0f,
            Vector2.Zero,
            1f,
            effects,
            0f
        );
    }

    private void DrawEyes(SpriteBatch spriteBatch, Vector2 roundedPosition)
    {
        var eyeOffset = new Vector2(0, GetBobbingOffsetY());

        // Forced closed overrides blinking
        if (forceEyesClosed || isBlinking)
        {
            Rectangle blinkRect = PlayerSprites.Eyes.GetBlinkFrame(DirectionFacing);
            spriteBatch.Draw(spriteSheet, roundedPosition + eyeOffset, blinkRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
            return;
        }

        Rectangle eyeWhiteRect = PlayerSprites.Eyes.GetEyeWhites(DirectionFacing);
        Rectangle pupilRect = PlayerSprites.Eyes.GetEyePupils(DirectionFacing);

        spriteBatch.Draw(spriteSheet, roundedPosition + eyeOffset, eyeWhiteRect, Color.White, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
        spriteBatch.Draw(spriteSheet, roundedPosition + eyeOffset, pupilRect, currentEyeColor, 0f, Vector2.Zero, 1f, spriteEffect, 0f);
    }
    #endregion

    #region Utility Methods
    public void AddHealth(int amount)
    {
        Health = Math.Min(Health + amount, MaxHealth);
    }

    public Rectangle GetCollisionRect()
    {
        return new Rectangle(
            (int)Position.X + (Width - ColliderWidth) / 2,
            (int)Position.Y + Height - ColliderHeight,
            ColliderWidth,
            ColliderHeight
        );
    }
    #endregion

    #region Customization Methods
    public void ChangeHairstyle(ContentManager Content, string newStyle)
    {
        currentHairStyle = newStyle;
        LoadHairStyle(newStyle);
    }

    public void ChangeShirtStyle(ContentManager Content, string newStyle)
    {
        currentShirtStyle = newStyle;
        LoadShirtStyle(newStyle);
    }

    public void ChangeHatStyle(ContentManager Content, string newStyle)
    {
        currentHatStyle = newStyle;
        LoadHatStyle(newStyle);
    }

    public void ChangePantsStyle(ContentManager Content, string newStyle)
    {
        currentPantsStyle = newStyle;
        LoadPantsStyle(newStyle);
    }

    public void ChangeShoeStyle(ContentManager Content, string newStyle)
    {
        currentShoeStyle = newStyle;
        LoadShoeStyle(newStyle);
    }

    public void ChangeHairColor(Color newColor)
    {
        currentHairColor = newColor;
    }

    public void ChangeEyeColor(Color newColor)
    {
        currentEyeColor = newColor;
    }
    #endregion

    #region Animation Control Methods
    public void StartEatingAnimation()
    {
        if (!isEating)
        {
            savedDirectionFacing = DirectionFacing;
            DirectionFacing = "down";
            isEating = true;
            eatingTimer = 0f;
        }
    }

    public void StartToolUseAnimation(Items.ToolType toolType)
    {
        if (isEating) return;

        isUsingTool = true;
        toolUseTimer = 0f;
        toolUseDirection = DirectionFacing;
        currentToolType = toolType;
        Velocity = Vector2.Zero;
        currentAnimation = animations[$"idle-{DirectionFacing}"];
    }

    public void ForceCloseEyes(float duration, Action onComplete = null)
    {
        forceEyesClosed = true;
        forceEyesClosedTimer = duration;
        forceEyesClosedCallback = onComplete;
        isBlinking = false;
    }

    public void ForceCloseEyesIndefinitely()
    {
        forceEyesClosed = true;
        forceEyesClosedTimer = float.MaxValue;
        forceEyesClosedCallback = null;
        isBlinking = false;
    }

    public void ReopenEyes()
    {
        forceEyesClosed = false;
        forceEyesClosedTimer = 0f;
        forceEyesClosedCallback = null;
    }

    public void TriggerEatingDamageEffect()
    {
        sparkleEffect.TriggerSparkle();
    }
    #endregion

    #region Static Methods
    public static void GoToSleep(GameContext context)
    {
        context.FreezeGame();
        context.Player?.ForceCloseEyesIndefinitely();

        context.ScreenFader.StartFadeOut(1.0f, () =>
        {
            string currentDate = context.DayManager.GetDateString();
            context.DayManager.FastForwardToHour(6);
            context.DayManager.NewDay(context);
            string nextDate = context.DayManager.GetDateString();
            context.DayTransitionScreen.Show(currentDate, nextDate);
        });
    }
    #endregion
}
