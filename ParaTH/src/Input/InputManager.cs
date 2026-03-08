using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ParaTH;

public enum InputType
{
    Keyboard,
    MouseButton
}

public record struct InputBinding
{
    public InputType Type { get; set; }
    public Keys? Key { get; set; }
    public MouseButton? MouseButton { get; set; }

    public static implicit operator InputBinding(Keys key) => FromKey(key);

    public static implicit operator InputBinding(MouseButton button) => FromMouseButton(button);
    private static InputBinding FromKey(Keys key) => new() { Type = InputType.Keyboard, Key = key };
    private static InputBinding FromMouseButton(MouseButton button) => new() { Type = InputType.MouseButton, MouseButton = button };
}

public enum MouseButton
{
    Left,
    Right,
    Middle,
    XButton1,
    XButton2
}

public sealed class InputManager
{
    private static readonly Lazy<InputManager> instance = new(() => new InputManager());
    public static InputManager Instance => instance.Value;

    private Dictionary<GameAction, List<InputBinding>> inputBindings = [];

    private KeyboardState previousKeyboardState;
    private KeyboardState currentKeyboardState;

    private MouseState previousMouseState;
    private MouseState currentMouseState;

    public Point MousePosition  => new(currentMouseState.X, currentMouseState.Y);
    public Point MouseDelta     => new(currentMouseState.X - previousMouseState.X, currentMouseState.Y - previousMouseState.Y);
    public int ScrollWheelDelta => currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;

    private InputManager() { }

    public void Update()
    {
        previousKeyboardState = currentKeyboardState;
        currentKeyboardState = Keyboard.GetState();

        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
    }

    #region Action detection
    public bool IsActionPressed(GameAction action)
        => inputBindings.TryGetValue(action, out var bindings) &&
           bindings.Any(binding => IsBindingPressed(binding));

    public bool IsActionHeld(GameAction action)
        => inputBindings.TryGetValue(action, out var bindings) &&
           bindings.Any(binding => IsBindingHeld(binding));

    public bool IsActionReleased(GameAction action)
        => inputBindings.TryGetValue(action, out var bindings) &&
           bindings.Any(binding => IsBindingReleased(binding));
    #endregion

    #region Binding detection
    private bool IsBindingPressed(InputBinding binding)
    {
        return binding.Type switch
        {
            InputType.Keyboard    => binding.Key.HasValue &&
                                     currentKeyboardState.IsKeyDown(binding.Key.Value) &&
                                     !previousKeyboardState.IsKeyDown(binding.Key.Value),
            InputType.MouseButton => binding.MouseButton.HasValue &&
                                     IsMouseButtonDown(binding.MouseButton.Value) &&
                                     !WasMouseButtonDown(binding.MouseButton.Value),
            _ => false,
        };
    }

    private bool IsBindingHeld(InputBinding binding)
    {
        return binding.Type switch
        {
            InputType.Keyboard    => binding.Key.HasValue &&
                                     currentKeyboardState.IsKeyDown(binding.Key.Value),
            InputType.MouseButton => binding.MouseButton.HasValue &&
                                     IsMouseButtonDown(binding.MouseButton.Value),
            _ => false,
        };
    }

    private bool IsBindingReleased(InputBinding binding)
    {
        return binding.Type switch
        {
            InputType.Keyboard    => binding.Key.HasValue &&
                                     !currentKeyboardState.IsKeyDown(binding.Key.Value) &&
                                     previousKeyboardState.IsKeyDown(binding.Key.Value),
            InputType.MouseButton => binding.MouseButton.HasValue &&
                                     !IsMouseButtonDown(binding.MouseButton.Value) &&
                                     WasMouseButtonDown(binding.MouseButton.Value),
            _ => false,
        };
    }
    #endregion

    #region Helper Methods For Mouse Button
    private bool IsMouseButtonDown(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left     => currentMouseState.LeftButton   == ButtonState.Pressed,
            MouseButton.Right    => currentMouseState.RightButton  == ButtonState.Pressed,
            MouseButton.Middle   => currentMouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.XButton1 => currentMouseState.XButton1     == ButtonState.Pressed,
            MouseButton.XButton2 => currentMouseState.XButton2     == ButtonState.Pressed,
            _ => false
        };
    }

    private bool WasMouseButtonDown(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left     => previousMouseState.LeftButton   == ButtonState.Pressed,
            MouseButton.Right    => previousMouseState.RightButton  == ButtonState.Pressed,
            MouseButton.Middle   => previousMouseState.MiddleButton == ButtonState.Pressed,
            MouseButton.XButton1 => previousMouseState.XButton1     == ButtonState.Pressed,
            MouseButton.XButton2 => previousMouseState.XButton2     == ButtonState.Pressed,
            _ => false
        };
    }
    #endregion

    #region Direct Input Detection
    public bool IsKeyPressed(Keys key)
        => currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);

    public bool IsKeyHeld(Keys key)
        => currentKeyboardState.IsKeyDown(key);

    public bool IsKeyReleased(Keys key)
        => !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);

    public bool IsMouseButtonPressed(MouseButton button)
        => IsMouseButtonDown(button) && !WasMouseButtonDown(button);

    public bool IsMouseButtonHeld(MouseButton button)
        => IsMouseButtonDown(button);

    public bool IsMouseButtonReleased(MouseButton button)
        => !IsMouseButtonDown(button) && WasMouseButtonDown(button);
    #endregion

    #region Binding Management
    public void AddKeyBinding(GameAction action, Keys key)
    {
        if (!inputBindings.ContainsKey(action))
            inputBindings[action] = [];

        InputBinding binding = key;
        if (!inputBindings[action].Any(b => b.Type == InputType.Keyboard && b.Key == key))
            inputBindings[action].Add(binding);
    }

    public void AddMouseBinding(GameAction action, MouseButton button)
    {
        if (!inputBindings.ContainsKey(action))
            inputBindings[action] = [];

        InputBinding binding = button;
        if (!inputBindings[action].Any(b => b.Type == InputType.MouseButton && b.MouseButton == button))
            inputBindings[action].Add(binding);
    }

    public void RemoveBinding(GameAction action, InputBinding binding)
    {
        if (inputBindings.TryGetValue(action, out var bindings))
            bindings.Remove(binding);
    }

    public void ClearActionBindings(GameAction action)
    {
        if (inputBindings.TryGetValue(action, out var value))
            value.Clear();
    }

    public void ClearAllBindings() => inputBindings.Clear();

    public void SetBindings(IReadOnlyDictionary<GameAction, IReadOnlyList<InputBinding>> newBindings)
        => inputBindings = newBindings.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToList()
        );

    public List<InputBinding> GetBindings(GameAction action)
        => inputBindings.TryGetValue(action, out var bindings)
            ? [.. bindings]
            : [];
    #endregion
}
