---
name: unit-tests-author
description: 'Write unit tests in C# using xUnit, AwesomeAssertions (FluentAssertions), NSubstitute, and Given/When/Then structure. Use this skill when creating or editing unit tests.'
---

# xUnit Test Writer

## Overview

This skill enforces consistent unit test authoring conventions for C# projects using xUnit, AwesomeAssertions (FluentAssertions) for assertions, NSubstitute for mocks/stubs, and the Given/When/Then structure.

All code, including unit tests, must follow the style and formatting rules defined in the repository's `.editorconfig` file.

## Guidelines
- Keep tests focused on a single behavior
- Avoid testing multiple behaviors in one test method
- Use clear assertions that express intent
- Include only the assertions needed to verify the test case
- Make tests independent and idempotent (can run in any order)
- Avoid test interdependencies

## Test file location

- Place unit tests in `*.Tests` projects within the relevant domain.
- Use the same namespace as the class being tested. Add the `Tests` suffix to both the test file name and the test class name.
  - Example: If the tested class is `SpatialLite.Core.MyService`, the test class should be `SpatialLite.Core.MyServiceTests` in the `SpatialLite.Core.Tests` project.
- If a file contains multiple tested classes, split them into separate test files — one per tested class.
  - Example: If `MyService.cs` contains `MyService` and `MyServiceHelper`, create `MyServiceTests.cs` and `MyServiceHelperTests.cs`.
- For large or logically distinct groups of tests (such as tests for different methods), consider organizing them into separate files and using partial test classes for maintainability. The `Tests` suffix should be appended to the class name, not the method name.
  - Example: If `MyService` has methods `MethodA` and `MethodB`, you might create `MyServiceTests.MethodA.cs` and `MyServiceTests.MethodB.cs`, each containing a `partial class MyServiceTests`.

## Test method naming

- If the tested class has only one public method, name test methods using:
  `<TestMethod>_<ExpectedBehavior>_<Scenario>`
  - The `<Scenario>` part is optional and should be prefixed with `If` when used.
  - Example: `CalculateTotal_ReturnsCorrectSum_IfInputIsValid`

- If the tested class has multiple public methods, group tests for each method in a nested class named after the method (omit the `Async` suffix for async methods).
  - Test methods within nested classes use the pattern: `<ExpectedBehavior>_<Scenario>`.
  - Example:
    ```csharp
    public static class MyClassTests
    {
        public class MethodA
        {
            public void DoesSomething_IfSomePrecondition() { }
            public void DoesSomethingElse() { }
        }

        public class MethodB
        {
            public void ProcessesData() { }
            public void Throws_IfInputIsInvalid() { }
        }
    }
    ```

## Given/When/Then structure

Every test body must be divided into three clearly commented sections:

```csharp
// Given
// ... setup code ...

// When
// ... invocation ...

// Then
// ... assertions ...
```

## Exceptions

- Use `Record.Exception` (sync) or `Record.ExceptionAsync` (async) to capture exceptions.
- Always assert exceptions using FluentAssertions — never use `Assert.Throws` or `Assert.ThrowsAsync`.
- Place exception assertions in the "Then" section.

```csharp
// Synchronous
void ThrowsException() => /* invoke tested code */;
Exception exception = Record.Exception(ThrowsException);

exception.Should().BeOfType<ExceptionType>(); // specific exception
exception.Should().BeNull();                  // no exception expected

// Asynchronous
Task ThrowsException() => /* invoke tested code, without await */;
Exception exception = await Record.ExceptionAsync(ThrowsException);
```
## SUT dependencies

Use test doubles where appropriate to isolate the unit under test (SUT). The choice between mocks, stubs, or fakes depends on the context of the test and the behavior being verified. Always prefer the simplest test double that achieves the necessary isolation and verification for the test scenario.

### Test doubles naming conventions

- `Spy*` - Records interactions for verification
- `Stub*` - Returns configured values
- `Fake*` - Simplified working implementation
- `Permissive*` - Always allows access
- `Failing*` / `Throwing*` - Always fails

### Common fakes/mocks

| Dependency | How to provide |
|---|---|
| `ILogger<T>` | `NullLogger<T>.Instance` |
| `TimeProvider` | `TimeProvider.System` (or `FakeTimeProvider` from `Microsoft.Extensions.TimeProvider.Testing` when time control is needed) |

### NSubstitute

Use NSubstitute to mock complex dependencies where only specific methods need to be mocked.

- For async methods, return the expected value directly in `Returns()` — do not wrap in `Task`.
- Keep mock setup concise. Extract complex or reused setup into a static helper method (e.g., `InstantiateXYZ(arg1, arg2, ...)`) at the bottom of the test class.

```csharp
var myObject = new MyObject();

var dataLoader = Substitute.For<IDataLoader>();
dataLoader
    .LoadDataAsync(Arg.Any<CancellationToken>())
    .Returns(myObject);
```

## Instantiating the tested class

- Instantiate the class under test directly in the "Given" section.
- Inject dependencies using mocks, stubs, or the common fakes listed above.

## Asserting collections

- **Simple types** (value types, strings): use `Equal()` or `BeEquivalentTo()`.
- **Complex objects**: use `SatisfyRespectively()` to assert each item individually.
- **Subsets**: use `Contain()`, `ContainSingle()`, or `OnlyContain()` as appropriate.

```csharp
// Simple types
firstNames.Should().Equal("John", "Robert");

// Complex objects
people.Should().SatisfyRespectively(
    first =>
    {
        first.ID.Should().Be(1);
        first.FirstName.Should().Be("John");
    },
    second =>
    {
        second.ID.Should().Be(2);
        second.FirstName.Should().Be("Robert");
    });

// Subset / single item
numbers.Should().Contain(3);
numbers.Should().ContainSingle(n => n == 2);
numbers.Should().OnlyContain(n => n > 0);
```

## Full example

```csharp
[Fact]
public void CreatePeople_ReturnedPersonsHaveCorrectFirstAndLastName()
{
    // Given
    var fullNames = new[] { "John Smith", "Robert Brown" };

    // When
    var people = CreatePeople(fullNames);

    // Then
    people.Should().SatisfyRespectively(
        first =>
        {
            first.ID.Should().Be(1);
            first.FirstName.Should().Be("John");
            first.LastName.Should().Be("Smith");
        },
        second =>
        {
            second.ID.Should().Be(2);
            second.FirstName.Should().Be("Robert");
            second.LastName.Should().Be("Brown");
        });
}
```
