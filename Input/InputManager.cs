using System.Linq;

namespace Honeyspur;

public class InputManager(GameContext context)
{
    private readonly GameContext context = context;
    private bool wasLeftMousePressedLastFrame = false;
    private bool wasRightMousePressedLastFrame = false;
    private KeyboardState previousKeyboardState;

    public void Update(GameContext gameContext)
    {
        UpdateKeyboardInput();
        UpdateMouseInput(gameContext);
    }

    private void UpdateKeyboardInput()
    {
        KeyboardState currentKeyboard = Keyboard.GetState();

        for (int i = 0; i < 10; i++)
        {
            Keys key = (i == 9) ? Keys.D0 : Keys.D1 + i;

            bool wasUpBefore = previousKeyboardState.IsKeyUp(key);
            bool isDownNow = currentKeyboard.IsKeyDown(key);

            if (wasUpBefore && isDownNow)
            {
                context.UIManager.SetInventoryBarSelectedIndex(i);
                SoundPlayer.Play("woo");
                break;
            }
        }

        // Screenshot on P key
        if (previousKeyboardState.IsKeyUp(Keys.P) && currentKeyboard.IsKeyDown(Keys.P))
            ScreenshotHelper.TakeScreenshot(context.GraphicsDevice);

        previousKeyboardState = currentKeyboard;
    }

    private void UpdateMouseInput(GameContext gameContext)
    {
        MouseState mouse = Mouse.GetState();
        bool isRightMousePressed = mouse.RightButton == ButtonState.Pressed;
        bool isLeftMousePressed = mouse.LeftButton == ButtonState.Pressed;

        // Check for new clicks (button down this frame but not last frame)
        bool leftMouseClicked = isLeftMousePressed && !wasLeftMousePressedLastFrame;
        bool rightMouseClicked = isRightMousePressed && !wasRightMousePressedLastFrame;

        Point tilePos = MouseHelper.GetMouseTilePosition(context);
        MouseHelper.UpdateCursorHover(context, tilePos);

        // UI click handling - let UIManager attempt to handle the click first
        bool uiHandledClick = false;
        if (leftMouseClicked)
            uiHandledClick = context.UIManager.HandleClick(mouse.Position, gameContext.GraphicsDevice.Viewport, gameContext);

        // If the game is frozen (modal open) or UI handled the click, skip world interactions
        if (!gameContext.IsFrozen && !uiHandledClick && !context.UIManager.IsModalOpen())
        {
            // Handle world interactions for new clicks only
            if (leftMouseClicked || rightMouseClicked)
                HandleWorldClick(tilePos, leftMouseClicked, rightMouseClicked);
        }

        // Update mouse state at the end (only one place now!)
        wasLeftMousePressedLastFrame = isLeftMousePressed;
        wasRightMousePressedLastFrame = isRightMousePressed;
    }

    private void HandleWorldClick(Point tilePos, bool leftMouseClicked, bool rightMouseClicked)
    {
        // These take precedence over any held item behavior
        if (TryHandleTileInteraction(tilePos, leftMouseClicked, rightMouseClicked))
            return;

        // Second priority: use held item based on its category
        if (leftMouseClicked || rightMouseClicked)
            UseHeldItem(tilePos, leftMouseClicked, rightMouseClicked);
    }

    private void UseHeldItem(Point tilePos, bool leftMouseClicked, bool rightMouseClicked)
    {
        var heldItem = context.UIManager.GetSelectedInventoryItem();
        if (heldItem == null) return;

        switch (heldItem.Category)
        {
            case Items.ItemCategory.Furniture:
            case Items.ItemCategory.Seed:
                // These items are placed with left-click
                if (leftMouseClicked)
                    TryPlaceItem(tilePos, heldItem);
                break;

            case Items.ItemCategory.Food:
            case Items.ItemCategory.Drink:
                // Consumables are used with right-click
                if (rightMouseClicked)
                    TryConsumeItem(heldItem);
                break;

            case Items.ItemCategory.Tool:
                // Tools are used with right-click and need range checking
                if (rightMouseClicked)
                    TryUseTool(tilePos, heldItem);
                break;
        }
    }

    // Tools trigger animation/sound when used within range and call tile-specific tool methods.
    private void TryUseTool(Point tilePos, Items.ItemInstance heldItem)
    {
        // Must be within range of clicked tile
        if (!context.Player.IsTileWithinRange(tilePos))
            return;

        // Face the tile before starting animation
        context.Player.FaceTowardsTile(tilePos);

        // Trigger player animation and tool sound (pass tool type for animation)
        context.Player.StartToolUseAnimation(heldItem.ToolType);

        // Play appropriate sound and call tile method based on tool type
        switch (heldItem.ToolType)
        {
            case Items.ToolType.Shears:
                SoundPlayer.PlayRandomSound("snip", 2);
                context.Player.AddHealth(-1);
                CallTileToolMethod(tilePos, Items.ToolType.Shears);
                break;

            case Items.ToolType.WateringCan:
                SoundPlayer.PlayRandomSound("spray", 2);
                context.Player.AddHealth(-1);
                CallTileToolMethod(tilePos, Items.ToolType.WateringCan);
                break;

            case Items.ToolType.Hoe:
                SoundPlayer.Play("dig");
                context.Player.AddHealth(-2);
                CallTileToolMethod(tilePos, Items.ToolType.Hoe);
                break;
        }
    }

    private void CallTileToolMethod(Point tilePos, Items.ToolType toolType)
    {
        // Check all tile layers in order of priority
        TileInstance[] tilesToCheck =
        {
            context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.ObjectTiles),
            context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.DecorationTiles),
            context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.BaseTiles)
        };

        foreach (var tile in tilesToCheck)
        {
            if (tile == null)
                continue;

            // Call the appropriate tool method and continue to next layer if not handled
            bool handled = false;
            switch (toolType)
            {
                case Items.ToolType.Shears:
                    handled = tile.OnShear(context);
                    break;
                case Items.ToolType.WateringCan:
                    handled = tile.OnSpray(context);
                    break;
                case Items.ToolType.Hoe:
                    handled = tile.OnHoe(context);
                    break;
            }

            if (handled)
                break; // Stop if this tile handled the tool action
        }
    }

    private bool TryHandleTileInteraction(Point tilePos, bool leftMouseClicked, bool rightMouseClicked)
    {
        // Check all tile layers in order of priority
        TileInstance[] tilesToCheck =
        {
        context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.ObjectTiles),
        context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.DecorationTiles),
        context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.BaseTiles)
    };

        foreach (var tileClicked in tilesToCheck)
        {
            if (tileClicked == null)
                continue;

            if (!context.Player.IsTileWithinRange(tilePos)) // too far away
                return false;

            context.Player.FaceTowardsTile(tilePos);

            bool handled = false;

            if (rightMouseClicked)
                handled = tileClicked.OnRightClick(context);

            if (leftMouseClicked)
                handled = tileClicked.OnLeftClick(context);

            if (handled) // Stop checking further tiles if this one handled the interaction
                return true;
        }

        return false;
    }

    private void TryConsumeItem(Items.ItemInstance heldItem)
    {
        int healthValue = heldItem.HealthValue;
        context.Player.StartEatingAnimation();
        context.Player.AddHealth(healthValue);

        if (healthValue < 0)
            context.Player.TriggerEatingDamageEffect();

        context.UIManager.RemoveSelectedInventoryItem();
        SoundPlayer.Play(heldItem.Category == Items.ItemCategory.Food ? "bite" : "sip");
    }

    private void TryPlaceItem(Point tilePos, Items.ItemInstance heldItem)
    {
        var (canPlace, tileDef, placementTileName, colliderSize) = MouseHelper.CanPlaceItem(context, tilePos, heldItem);

        if (!canPlace || tileDef == null)
            return;

        Rectangle placementRect = MouseHelper.GetPlacementRect(tilePos, colliderSize);

        // Place the tile
        int tileId = TileDefinitions.All.First(kvp => kvp.Value.Name == placementTileName).Key;
        var tileInstance = TileFactory.Create(tileDef, tilePos, tileId);
        tileInstance.MapName = context.MapManager.CurrentMap?.Name;
        context.MapManager.ObjectTiles[tilePos.X, tilePos.Y] = tileInstance;

        if (tileInstance.LightType != LightType.None)
            context.MapManager.AddLightTile(tileInstance);

        // Update collision grid for placed tile
        context.MapManager.UpdateCollisionGrid(tilePos, tileDef, true);

        context.UIManager.RemoveSelectedInventoryItem();
        SoundPlayer.Play("poppopDown");
    }
}