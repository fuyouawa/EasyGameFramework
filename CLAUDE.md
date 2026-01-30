# EasyToolkit Coding Standards for Claude Code

This document outlines the coding standards that must be followed when working with the EasyToolkit Unity project.

## Project Overview

EasyToolkit is a Unity framework/library project divided into multiple Unity packages. The codebase is written in C# and follows strict coding standards to maintain consistency and readability.

## Naming Conventions

### Variable Naming

#### General Rules
- Avoid abbreviations except for commonly accepted ones
- Reference: See [Code Style Guide](Documents/CodingStandards/代码风格指南.md) for `var` keyword usage

#### Collections
- Use **plural form** for standard collections
```csharp
List<Player> players;
Player[] onlinePlayers;
```

#### Dictionary
- Use **XxxByYyy** format (ValueByKey)
- `Xxx` is singular unless it's a collection type
```csharp
Dictionary<int, Player> playerById;
Dictionary<string, Item> itemByName;
Dictionary<int, List<Quest>> questsByPlayerId;
```

#### TaskCompletionSource
- Use **action semantic + Tcs suffix**
```csharp
TaskCompletionSource<int> clickCompletedTcs;
TaskCompletionSource<bool> loginTcs;
```

#### Events
- Use **Xxxing** / **Xxxed** format (NO `On` prefix)
- `Xxxing` = in progress (can be cancelled/intervened)
- `Xxxed` = completed (notify result)
```csharp
public event Action Closing;    // Window is closing
public event Action Closed;     // Window has closed
public event Action<Exception> CloseFailed;
// Trigger methods use On prefix
protected virtual void OnClosing() => Closing?.Invoke();
```

#### Delegates
Use appropriate suffix based on usage:
- `XxxHandler` - Event/message handling
- `XxxInvoker` - Command execution/invocation
- `XxxResolver` - Dependency resolution/service location
- `XxxFactory` - Object creation/instantiation
- `XxxGetter` - Property/data retrieval
- `XxxConverter` - Type conversion/formatting
- `XxxPredicate` - Boolean condition checking
- `XxxEvaluator` - Calculation/evaluation

#### Comparers/Sorters
- Lambda expressions: use `a, b` parameters
- Public methods: use `left, right` parameters
```csharp
list.Sort((a, b) => a.Id.CompareTo(b.Id));
public int Compare(Item left, Item right) => left.Id.CompareTo(right.Id);
```

### Function Naming

#### Retrieval Function Prefixes
- `Get` - Expect to find, throw if not found (usually O(1))
- `Find` - May need search, return null if not found
- `TryGet` - Safe version, return bool + out parameter

#### Calculation Function Prefixes
- `Compute` - Algorithm/logic derivation, multi-step reasoning
- `Calculate` - Mathematical formula, direct calculation
- `Can` - Whether an operation is feasible
- `Is` - Current state or property check

#### Function Suffixes
- `ByXxx` - According to Xxx (e.g., `ById`, `ByName`, `ByKey`)
- `AtXxx` - At position/time (e.g., `AtPosition`, `AtIndex`, `AtTime`)
- `OfXxx` - Belongs to or has property of (e.g., `OfPlayer`, `OfType`)

**When to use suffixes:**
- MUST: Multiple lookup methods exist for same entity
- MUST: Same parameter type but different semantics
- OPTIONAL: Context is clear from class name
- Default: Use suffix if uncertain about future API changes

#### Chain Method Prefixes

Chain methods configure object properties and return self for continuous operations. Categorized by operation type:

**WithXxx (Direct Assignment)**
- Directly set property to specified value
- Used for: Builder patterns, extension methods, configuration
- Examples: `WithName(value)`, `WithLevel(1)`, `WithPosition(vector3)`
```csharp
public PlayerBuilder WithName(string name)
{
    _name = name;
    return this;
}
```

**WithXxxBy (Incremental Operations)**
- Modify current value by adding/multiplying
- Parameter is a delta/change amount, not target value
- Examples: `WithXOffsetBy(10)`, `WithScaleBy(1.5f)`, `WithMovedBy(delta)`
```csharp
public Rect WithXOffsetBy(this Rect rect, float deltaX)
{
    rect.x += deltaX;
    return rect;
}
```

**WithXxxXxx (Constrained Operations)**
- Set value with constraints/validation/special processing
- Examples: `WithXClamped(x, min, max)`, `WithWidthNormalized(0.5f)`, `WithPositionRounded(pos)`
```csharp
public Rect WithWidthClamped(this Rect rect, float width, float min, float max)
{
    rect.width = Mathf.Clamp(width, min, max);
    return rect;
}
```

**Design Principles:**
1. All chain methods must return `this` for fluent API
2. `WithXxx` - parameter is target value
3. `WithXxxBy` - parameter is delta/change
4. `WithXxxXxx` - special processing (Clamped, Normalized, Rounded, etc.)
5. Method names must clearly express operation type to avoid ambiguity

## Class Member Organization

### Standard Classes
Order (top to bottom):
1. Constants (`const`)
2. Static readonly fields (`static readonly`)
3. Static fields (`static`)
4. Instance fields (`private`/`protected`)
5. Constructors
6. Properties
7. Methods

Access modifier order: `public` → `protected` → `private`

### Unity Classes
Order (top to bottom):
1. Constants / Static fields
2. Serialized fields (`[SerializeField]`)
   - Prefer `[SerializeField] private` over public fields
   - Provide properties for external access with validation
3. Private fields
4. Public properties
5. Unity lifecycle methods (in call order):
   - Awake
   - OnEnable
   - Start
   - FixedUpdate
   - Update
   - LateUpdate
   - OnDisable
   - OnDestroy
6. Public methods
7. Private methods
8. Coroutines

## Member Variable Access Principles

### Pure Data Classes
- Direct field access preferred internally
- Properties without logic only serve as external access control
```csharp
[SerializeField] private int _playerId;
public int PlayerId => _playerId;

// Internal: Use _playerId directly
```

### Properties with Logic
- MUST use property access to ensure validation/events execute
```csharp
public float CurrentHealth
{
    get => _currentHealth;
    private set
    {
        _currentHealth = Mathf.Clamp(value, 0, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
    }
}
// Internal: Use CurrentHealth to trigger logic
```

## var Usage Guidelines

**Basic principle**: Use `var` when type is obvious, explicit when unclear

### Use var:
- Constructor calls: `var player = new Player();`
- Collection access: `var count = players.Count;`
- foreach loops: `foreach (var player in players)`
- LINQ chains: `var activePlayers = players.Where(p => p.IsActive).ToList();`
- Generic methods with inferred types: `var result = GetOrCreate<Player>();`

### Use explicit types:
- Function returns (unclear type): `float damage = CalculateDamage();`
- Complex generics: `Dictionary<int, List<Player>> playerGroups = new();`
- Numeric types (precision matters): `float deltaTime = Time.deltaTime;`
- Interface types (emphasize abstraction): `IEnumerable<Player> collection = GetPlayers();`
- bool/nullable (semantics matter): `bool isValid = Validate();`

## Comment Standards

### XML Documentation Comments
Required for:
- All public types and members
- Important protected members
- Complex private members

**Formatting Rules:**
- **MUST use multi-line format** - Opening `<summary>` and closing `</summary>` tags MUST be on separate lines

Basic format:
```csharp
/// <summary>
/// Brief description of what the member does (use descriptive language, not imperative).
/// </summary>
/// <param name="paramName">Description of parameter.</param>
/// <returns>Description of return value.</returns>
/// <remarks>Additional usage or behavior notes.</remarks>
/// <exception cref="ExceptionType">When this exception is thrown (user-relevant only).</exception>
```

### XML Special Characters
In XML documentation comments, `<` and `>` are special characters used for XML tags. When these characters appear in comments (e.g., generic types like `List<int>` or comparison operators), they must be escaped:
- `<` → `&lt;`
- `>` → `&gt;`

```csharp
/// <summary>
/// Gets a collection of items of type &lt;T&gt; from the cache.
/// </summary>
/// <returns>A &lt;see cref="List&lt;T&gt;"/&gt; of cached items.</returns>
```

### Exception Documentation
- Document: Exceptions users may encounter in normal usage
- Don't document: Defensive programming checks (null checks, argument validation)

### Implementation Comments
- Use `//` for explaining "why" not "what"
- Add comments for complex algorithms, business rules
- No comments needed when code is self-explanatory

### Special Comment Tags
- `TODO(username): Description` - Mark incomplete features
- `FIXME(username): Description` - Mark known bugs
- `HACK(username): Reason` - Mark inelegant solutions
- `NOTE: Description` - Emphasize important details
- `[Obsolete]` attribute - Mark deprecated APIs

## Logging and Error Standards

### Log Levels
- **Debug** - Development troubleshooting (usually disabled in production)
- **Info** - Normal operation state changes
- **Warning** - Non-critical issues, degraded functionality
- **Error** - Function failures but system continues
- **Fatal** - System cannot continue

### Log Message Format
`[Context] Action: Description (Details)`
```csharp
Logger.Info($"[Player] PlayerJoined: Player {playerId} joined (Position: {position})");
Logger.Warning($"[Resource] AssetLoadFailed: Failed to load {assetPath} (Reason: {error})");
```

### Error Message Format
`What happened? Why it happened? How to fix?`
```csharp
throw new InvalidOperationException(
    "Cannot attack while dead. Player health is 0. Revive player before attacking.");
```

### Exception Handling
- Catch specific exceptions, not generic `Exception`
- Log context information when catching
- Preserve original exception when re-throwing
- Custom exceptions: `XxxException` suffix, provide meaningful message and inner exception support

## Code Quality Principles

1. **Do not over-engineer** - Make only requested changes
2. **Avoid backwards compatibility hacks** - Delete unused code completely
3. **Security awareness** - Prevent injection vulnerabilities, validate at boundaries
4. **Avoid premature abstraction** - Three similar lines > premature abstraction
5. **No unnecessary features** - Don't add features/refactoring beyond what's asked

## Unity-Specific Guidelines

- **DO NOT create `.meta` files** - Unity automatically generates `.meta` files for all assets. Never manually create or include `.meta` files when writing new code or assets.
- Serialized fields should be `[SerializeField] private` with public properties for access
- Properties can include validation logic
- Organize Unity lifecycle methods in proper call order
- Use coroutines at the end of the class after all methods
- Prefer `_camelCase` for private fields with leading underscore

## Related Documentation

For detailed specifications, refer to:
- [命名规范](Documents/CodingStandards/命名规范.md)
- [类成员排列规范](Documents/CodingStandards/类成员排列规范.md)
- [成员变量访问层次原则](Documents/CodingStandards/成员变量访问层次原则.md)
- [代码风格指南](Documents/CodingStandards/代码风格指南.md)
- [注释规范](Documents/CodingStandards/注释规范.md)
- [日志和错误信息规范](Documents/CodingStandards/日志和错误信息规范.md)
- [框架架构规范](Documents/CodingStandards/框架架构规范.md)
