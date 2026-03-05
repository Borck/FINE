# .NET 10.0 Upgrade Plan

## Table of Contents
- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
  - [Phase 1: Foundation Libraries](#phase-1-foundation-libraries)
  - [Phase 2: Toolkit Libraries](#phase-2-toolkit-libraries)
  - [Phase 3: Applications and Tests](#phase-3-applications-and-tests)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Description
Upgrade NodeNetwork solution from .NET 6 to .NET 10 LTS, updating all WPF projects while maintaining multi-targeting support for library projects (net10.0-windows and net472) and ensuring compatibility with the latest NuGet packages.

### Scope
**Projects Requiring Upgrade: 8 of 10**
- 2 Foundation libraries (NodeNetwork, NodeNetwork.Blazor)
- 2 Toolkit libraries (NodeNetworkToolkit, NodeNetworkToolkit.Blazor)
- 3 Example applications (Calculator, CodeGen, ShaderEditor)
- 1 Stress test application
- 2 Test projects

**Projects Already on .NET 10: 2**
- NodeNetwork.Blazor (already net10.0)
- NodeNetworkToolkit.Blazor (already net10.0)

**Current State:**
- 8 projects targeting net6.0-windows
- 2 library projects with multi-targeting (net6.0-windows;net472)
- 26 NuGet packages in use
- 3,087 compatibility issues identified (2,981 mandatory)

**Target State:**
- All WPF projects targeting net10.0-windows
- Multi-targeting libraries: net10.0-windows;net8.0;net472
- All packages updated to .NET 10 compatible versions
- 1 deprecated package removed (ReactiveUI.Events.WPF)
- 1 incompatible package replaced (ReactiveUI.WPF)
- 6 framework-included packages removed

### Selected Strategy
**All-At-Once Strategy** - All WPF projects upgraded simultaneously in a single coordinated operation.

**Rationale:**
- 8 WPF projects requiring upgrade (medium-sized solution)
- All projects currently on .NET 6.0 or higher
- Clear, simple dependency structure with 3 levels
- All NuGet packages have clear migration paths
- No circular dependencies
- Strong test coverage with dedicated test projects
- 2 Blazor projects already on .NET 10 provide confidence

### Complexity Assessment

**Discovered Metrics:**
- Total projects: 10 (8 requiring upgrade)
- Dependency depth: 3 levels
- Total issues: 3,087 (2,981 mandatory, 105 potential, 1 optional)
- Affected files: 111
- Affected technologies: 4 (WPF, Windows Forms, GDI+, Legacy Configuration)
- Incompatible packages: 1 (ReactiveUI.WPF requires update to 16.4.15)
- Deprecated packages: 1 (ReactiveUI.Events.WPF)
- Framework-included packages: 6

**Classification: Medium Complexity**

**Justification:**
- Moderate project count suitable for all-at-once approach
- High issue count (3,087) but concentrated in expected areas (WPF API changes)
- Clear dependency graph with no cycles
- Package ecosystem mostly compatible
- Multi-targeting adds complexity to 2 library projects
- 1 incompatible package (ReactiveUI.WPF) has known replacement path
- No security vulnerabilities identified

### Critical Issues
1. **ReactiveUI.WPF Incompatibility**: Package version 20.4.1 is not compatible with .NET 10. Must downgrade to version 16.4.15 (last compatible version) or find alternative reactive extensions.
2. **ReactiveUI.Events.WPF Deprecation**: Package is obsolete and should be removed.
3. **Multi-Targeting Considerations**: NodeNetwork and NodeNetworkToolkit must maintain .NET Framework 4.7.2 support while adding .NET 10 and .NET 8.
4. **Framework-Included Packages**: 6 packages now part of .NET 10 framework should be removed to avoid conflicts.
5. **API Breaking Changes**: 1,221 WPF-related issues, 28 Windows Forms issues, 19 GDI+ issues require careful review.

### Recommended Approach
Execute atomic upgrade of all 8 WPF projects simultaneously:
1. Update all project files to target frameworks
2. Update all package references across all projects
3. Remove framework-included packages
4. Build entire solution to identify compilation errors
5. Fix all breaking changes in single pass
6. Run all test projects to validate functionality

### Iteration Strategy
**Phase-based detail generation** (one iteration per migration phase):
- Phase 1: Foundation libraries (2 projects)
- Phase 2: Toolkit libraries (2 projects)  
- Phase 3: Applications and tests (4 projects)
- Final: Success criteria and source control strategy

**Expected remaining iterations: 5**

## Migration Strategy

### Approach Selection: All-At-Once Strategy

**Decision: All-At-Once Upgrade**

All 8 WPF projects requiring upgrade will be updated simultaneously in a single atomic operation.

### Justification

**Why All-At-Once is Optimal:**

1. **Solution Size**: 8 projects requiring upgrade is within the ideal range for all-at-once (< 30 projects)

2. **Homogeneous Technology Stack**: All projects use:
   - WPF as primary UI framework
   - Same current framework (.NET 6.0)
   - Same target framework (.NET 10.0)
   - Shared ReactiveUI reactive programming model
   - Consistent package ecosystem

3. **Clear Dependency Structure**: 
   - No circular dependencies
   - Clean 4-level hierarchy
   - Simple dependency relationships

4. **Package Compatibility**: 
   - All packages have known compatible versions or clear migration paths
   - Only 1 incompatible package (ReactiveUI.WPF) with known downgrade solution
   - No blocking package issues

5. **Testing Infrastructure**:
   - 2 dedicated test projects (NodeNetworkTests, CalculatorTests)
   - StressTest project for performance validation
   - Can validate entire solution after upgrade

6. **Risk Tolerance**:
   - Non-production library project (based on package generation settings)
   - Can afford brief instability during upgrade
   - Faster completion time preferred

### All-At-Once Strategy Rationale

**Advantages for This Solution:**
- **Fastest Completion**: Single coordinated update vs multiple incremental phases
- **No Multi-Targeting Complexity**: Applications don't need temporary multi-targeting
- **Clean Dependency Resolution**: All projects resolve packages against same framework version
- **Simplified Testing**: One comprehensive test pass instead of multiple phase validations
- **All Projects Benefit Simultaneously**: Entire solution gets .NET 10 improvements at once

**Considerations:**
- Higher initial risk (mitigated by strong dependency structure)
- Larger initial testing surface (addressed by dedicated test projects)
- All developers must adapt simultaneously (acceptable for library project)

### Dependency-Based Ordering Principles

While all projects update simultaneously, the **build order** follows dependency hierarchy:

**Build Sequence:**
1. **Level 0**: NodeNetwork, NodeNetwork.Blazor
2. **Level 1**: NodeNetworkToolkit, NodeNetworkToolkit.Blazor
3. **Level 2**: Applications and NodeNetworkTests
4. **Level 3**: CalculatorTests

This ordering ensures:
- Dependencies compile before their consumers
- Compilation errors surface in dependency order
- Breaking changes identified at foundation level first

### Parallel vs Sequential Execution

**Within Each Level: Parallel Compilation**
- Level 0: NodeNetwork and NodeNetwork.Blazor can build in parallel
- Level 1: NodeNetworkToolkit and NodeNetworkToolkit.Blazor can build in parallel
- Level 2: All 5 projects can build in parallel

**Between Levels: Sequential**
- Level N+1 waits for Level N to complete
- Ensures dependency satisfaction

**MSBuild handles this automatically** - no special configuration needed.

### Phase Definitions

For the All-At-Once strategy, phases are **logical groupings** for documentation and validation, not separate execution batches.

**Phase 0: Preparation**
- Verify .NET 10 SDK installation
- Update global.json if present
- Backup current state (Git branch already created)

**Phase 1: Atomic Upgrade** (Single Coordinated Operation)
**Operations performed simultaneously:**
1. Update all project files to target frameworks
2. Update all package references across all projects
3. Remove framework-included packages
4. Restore dependencies
5. Build entire solution
6. Fix all compilation errors found
7. Rebuild to verify fixes

**Success Criteria**: Solution builds with 0 errors

**Phase 2: Testing & Validation**
**Operations:**
1. Run NodeNetworkTests
2. Run CalculatorTests  
3. Run StressTest for performance validation
4. Address any test failures
5. Final validation build

**Success Criteria**: All tests pass, no regressions

### Execution Approach

**Single Atomic Transaction:**
```
┌─────────────────────────────────────────────────────────────┐
│ ATOMIC UPGRADE (All projects simultaneously)               │
├─────────────────────────────────────────────────────────────┤
│ 1. Update TargetFramework in all 8 project files           │
│ 2. Update PackageReferences in all projects                │
│ 3. Remove 6 framework-included packages                    │
│ 4. dotnet restore (entire solution)                        │
│ 5. dotnet build (entire solution) → identify errors        │
│ 6. Fix all compilation errors → reference Breaking Changes │
│ 7. dotnet build (verify fixes) → 0 errors                  │
└─────────────────────────────────────────────────────────────┘
         ↓
┌─────────────────────────────────────────────────────────────┐
│ TESTING (After successful build)                           │
├─────────────────────────────────────────────────────────────┤
│ 1. dotnet test NodeNetworkTests                            │
│ 2. dotnet test CalculatorTests                             │
│ 3. Run StressTest.exe                                       │
│ 4. Fix any test failures                                    │
│ 5. Re-run tests → all pass                                  │
└─────────────────────────────────────────────────────────────┘
```

### Rollback Strategy

**If Atomic Upgrade Fails:**
1. Discard changes: `git reset --hard HEAD`
2. Return to pre-upgrade state on master branch
3. Analyze failure root cause
4. Adjust plan if needed
5. Retry upgrade

**If Testing Reveals Critical Issues:**
1. Keep upgraded code on upgrade-to-NET10 branch
2. Do not merge to master until tests pass
3. Fix issues incrementally
4. Re-test after each fix
5. Merge only when all validation passes

### Success Metrics

**Atomic Upgrade Phase:**
- ✅ All 8 project files updated
- ✅ All package references updated
- ✅ Solution restores without errors
- ✅ Solution builds with 0 compilation errors
- ✅ 0 build warnings related to framework upgrade

**Testing Phase:**
- ✅ NodeNetworkTests: All tests pass
- ✅ CalculatorTests: All tests pass
- ✅ StressTest: Runs without crashes, acceptable performance
- ✅ No functional regressions observed

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution has a clean 4-level dependency hierarchy with no circular dependencies:

```
Level 0 (Foundation - No Dependencies):
├── NodeNetwork.Blazor.csproj ✅ Already .NET 10
│   └── Used by: NodeNetworkToolkit.Blazor
└── NodeNetwork.csproj ⚠️ Needs Upgrade (net6.0-windows;net472 → net10.0-windows;net8.0;net472)
    └── Used by: 6 projects

Level 1 (Toolkit - Depends on Level 0):
├── NodeNetworkToolkit.Blazor.csproj ✅ Already .NET 10
│   └── No dependents (standalone)
└── NodeNetworkToolkit.csproj ⚠️ Needs Upgrade (net6.0-windows;net472 → net10.0-windows;net8.0;net472)
    └── Used by: 6 projects

Level 2 (Applications - Depends on Levels 0-1):
├── ExampleCalculatorApp.csproj ⚠️ Needs Upgrade (net6.0-windows → net10.0-windows)
│   └── Used by: CalculatorTests
├── ExampleCodeGenApp.csproj ⚠️ Needs Upgrade (net6.0-windows → net10.0-windows)
├── ExampleShaderEditorApp.csproj ⚠️ Needs Upgrade (net6.0-windows → net10.0-windows)
├── StressTest.csproj ⚠️ Needs Upgrade (net6.0-windows → net10.0-windows)
└── NodeNetworkTests.csproj ⚠️ Needs Upgrade (net6.0-windows → net10.0-windows)

Level 3 (Tests - Depends on Levels 0-2):
└── CalculatorTests.csproj ⚠️ Needs Upgrade (net6.0-windows → net10.0-windows)
```

### Project Groupings by Migration Phase

Since we're using the **All-At-Once Strategy**, all projects will be upgraded simultaneously in a single coordinated operation. However, for documentation and validation purposes, we organize projects by their dependency level:

**Group 1: Foundation Libraries (Level 0)**
- NodeNetwork.csproj
- NodeNetwork.Blazor.csproj (already .NET 10, no action needed)

**Group 2: Toolkit Libraries (Level 1)**
- NodeNetworkToolkit.csproj
- NodeNetworkToolkit.Blazor.csproj (already .NET 10, no action needed)

**Group 3: Applications and Tests (Levels 2-3)**
- ExampleCalculatorApp.csproj
- ExampleCodeGenApp.csproj
- ExampleShaderEditorApp.csproj
- StressTest.csproj
- NodeNetworkTests.csproj
- CalculatorTests.csproj

**All-At-Once Execution**: All 8 projects requiring upgrade will have their TargetFramework properties and PackageReference elements updated simultaneously, followed by a single build and fix cycle.

### Critical Path Identification

**Critical Path:** NodeNetwork.csproj → NodeNetworkToolkit.csproj → All Applications

**Justification:**
1. **NodeNetwork.csproj** is the foundation library used by 6 other projects
2. **NodeNetworkToolkit.csproj** depends on NodeNetwork and is used by 6 other projects
3. Any breaking changes in these libraries cascade to all consumers

**Risk Mitigation:**
- These two libraries have the highest issue counts (1,625 and 677 issues respectively)
- Multi-targeting complexity requires careful handling
- Package incompatibility (ReactiveUI.WPF) affects both libraries
- All downstream projects must build after foundation changes complete

### Circular Dependency Analysis

**Status:** ✅ No circular dependencies detected

The dependency graph is a clean directed acyclic graph (DAG), which is ideal for the All-At-Once strategy. This allows us to:
- Update all projects simultaneously without dependency conflicts
- Build the entire solution in a single pass after updates
- Validate all projects together

### Multi-Targeting Considerations

Two projects require special handling for multi-targeting:

**NodeNetwork.csproj**
- Current: `net6.0-windows;net472`
- Target: `net10.0-windows;net8.0;net472`
- Rationale: Add .NET 8 as intermediate LTS, maintain .NET Framework compatibility

**NodeNetworkToolkit.csproj**
- Current: `net6.0-windows;net472`
- Target: `net10.0-windows;net8.0;net472`
- Rationale: Match NodeNetwork multi-targeting for consistency

**Multi-Targeting Impact:**
- Conditional package references may be needed
- Build output for 3 target frameworks per library
- Package compatibility must span all target frameworks
- Testing required for each target framework

## Project-by-Project Migration Plans

### Phase 1: Foundation Libraries

#### NodeNetwork.csproj

**Current State:** 
- Target Framework: `net6.0-windows;net472`
- Files: 38
- Packages: 15
- Issues: 1,625 (1,617 mandatory, 7 potential, 1 optional)
- Used by: ExampleCalculatorApp, NodeNetworkToolkit, ExampleShaderEditorApp, ExampleCodeGenApp, StressTest, NodeNetworkTests

**Target State:** 
- Target Framework: `net10.0-windows;net8.0;net472`
- Rationale: Add .NET 10 (LTS), add .NET 8 (intermediate LTS), maintain .NET Framework 4.7.2 compatibility

**Risk Level:** High
- Foundation library for entire solution
- 1,625 compatibility issues
- ReactiveUI.WPF incompatibility
- Multi-targeting complexity

**Migration Steps:**

1. **Prerequisites**
   - Verify .NET 10 SDK installed
   - Verify .NET 8 SDK installed
   - Ensure NodeNetwork.csproj is unloaded from IDE (if making manual edits)

2. **Update Target Framework**
   ```xml
   <!-- Current -->
   <TargetFrameworks>net6.0-windows;net472</TargetFrameworks>

   <!-- Target -->
   <TargetFrameworks>net10.0-windows;net8.0;net472</TargetFrameworks>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Action | Reason |
   |---------|----------------|----------------|--------|--------|
   | ReactiveUI.WPF | 20.4.1 | 16.4.15 | **DOWNGRADE** | Version 20.4.1 incompatible with .NET 10 |
   | ReactiveUI.Events.WPF | 15.1.1 | - | **REMOVE** | Package is deprecated |
   | System.Collections.Immutable | 9.0.7 | 10.0.3 | Update | Recommended upgrade |
   | System.Buffers | 4.6.1 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Data.DataSetExtensions | 4.5.0 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Drawing.Primitives | 4.3.0 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Memory | 4.6.3 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Numerics.Vectors | 4.6.1 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Threading.Tasks.Extensions | 4.6.3 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.ValueTuple | 4.6.1 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | log4net | 3.1.0 | 3.1.0 | Keep | Compatible |
   | ReactiveUI | 20.4.1 | 20.4.1 | Keep | Compatible |
   | Splat.Drawing | 13.1.63 | 13.1.63 | Keep | Compatible |
   | Microsoft.CSharp | 4.7.0 | 4.7.0 | Keep | Compatible |
   | System.Runtime.CompilerServices.Unsafe | 6.1.2 | 6.1.2 | Keep | Compatible |

   **Multi-Targeting Approach for Framework-Included Packages:**

   For packages that are framework-included in .NET 10/8 but needed for net472, use conditional references:

   ```xml
   <!-- Keep for net472, remove for net10.0 and net8.0 -->
   <PackageReference Include="System.Buffers" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Data.DataSetExtensions" Version="4.5.0" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Drawing.Primitives" Version="4.3.0" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Memory" Version="4.6.3" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Numerics.Vectors" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Threading.Tasks.Extensions" Version="4.6.3" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.ValueTuple" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
   ```

4. **Expected Breaking Changes**

   **WPF API Changes (587 issues):**
   - Behavioral changes in dependency properties
   - Rendering pipeline updates
   - Data binding expression changes
   - Visual tree traversal modifications

   **ReactiveUI.WPF Downgrade Impact:**
   - Verify no features from 17.x-20.x versions are used
   - Check reactive command syntax compatibility
   - Validate reactive property binding patterns
   - Review event handling wrappers (ReactiveUI.Events.WPF removal)

   **API Compatibility:**
   - Binary incompatible APIs (1,608 issues): Requires recompilation, may need code changes
   - Behavioral changes (6 issues): Logic may behave differently, requires testing

5. **Code Modifications**

   **ReactiveUI.Events.WPF Removal:**
   - Search for: `using ReactiveUI.Events;`
   - Alternative: Use ReactiveUI's built-in event handling or `Observable.FromEventPattern`
   - Review event subscription patterns

   **Obsolete API Replacements:**
   - Review compiler warnings for deprecated API usage
   - Consult .NET 10 breaking changes documentation
   - Update to recommended alternatives

   **Configuration Updates:**
   - No app.config or web.config in library project

6. **Testing Strategy**

   **Per-Framework Testing:**
   - Build for net10.0-windows → verify no errors
   - Build for net8.0 → verify no errors
   - Build for net472 → verify no errors

   **Functional Testing:**
   - NodeNetworkTests will validate core functionality
   - All dependent projects must build successfully
   - Verify reactive bindings work correctly

7. **Validation Checklist**
   - [ ] Project builds for net10.0-windows without errors
   - [ ] Project builds for net8.0 without errors
   - [ ] Project builds for net472 without errors
   - [ ] No build warnings related to package conflicts
   - [ ] ReactiveUI.WPF 16.4.15 referenced correctly
   - [ ] ReactiveUI.Events.WPF removed
   - [ ] Framework-included packages conditional on net472
   - [ ] System.Collections.Immutable updated to 10.0.3
   - [ ] NodeNetworkTests passes (deferred to Phase 2)
   - [ ] All dependent projects can reference this project

#### NodeNetwork.Blazor.csproj

**Current State:** net10.0 | Already upgraded
**Target State:** net10.0 (no changes)
**Status:** ✅ No action required

This Blazor project is already targeting .NET 10 and has no compatibility issues. No migration steps needed.

### Phase 2: Toolkit Libraries

#### NodeNetworkToolkit.csproj

**Current State:**
- Target Framework: `net6.0-windows;net472`
- Files: 24
- Packages: 11
- Issues: 677 (664 mandatory, 13 potential)
- Depends on: NodeNetwork
- Used by: ExampleCalculatorApp, ExampleShaderEditorApp, ExampleCodeGenApp, StressTest, CalculatorTests, NodeNetworkTests

**Target State:**
- Target Framework: `net10.0-windows;net8.0;net472`
- Rationale: Match NodeNetwork multi-targeting for consistency

**Risk Level:** High
- Depends on upgraded NodeNetwork
- Used by 6 downstream projects
- 677 compatibility issues
- Multi-targeting complexity

**Migration Steps:**

1. **Prerequisites**
   - NodeNetwork.csproj must be successfully upgraded first
   - Verify NodeNetwork builds for all target frameworks
   - Ensure NodeNetworkToolkit.csproj is unloaded from IDE (if making manual edits)

2. **Update Target Framework**
   ```xml
   <!-- Current -->
   <TargetFrameworks>net6.0-windows;net472</TargetFrameworks>

   <!-- Target -->
   <TargetFrameworks>net10.0-windows;net8.0;net472</TargetFrameworks>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Action | Reason |
   |---------|----------------|----------------|--------|--------|
   | System.Collections.Immutable | 9.0.7 | 10.0.3 | Update | Recommended upgrade |
   | System.Buffers | 4.6.1 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Data.DataSetExtensions | 4.5.0 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Drawing.Primitives | 4.3.0 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Memory | 4.6.3 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Numerics.Vectors | 4.6.1 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.Threading.Tasks.Extensions | 4.6.3 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | System.ValueTuple | 4.6.1 | - | **REMOVE** for net10.0/net8.0 | Framework-included |
   | ReactiveUI | 20.4.1 | 20.4.1 | Keep | Compatible |
   | Microsoft.CSharp | 4.7.0 | 4.7.0 | Keep | Compatible |
   | System.Runtime.CompilerServices.Unsafe | 6.1.2 | 6.1.2 | Keep | Compatible |

   **Multi-Targeting Approach:**

   Same strategy as NodeNetwork - use conditional references for framework-included packages:

   ```xml
   <!-- Keep for net472, remove for net10.0 and net8.0 -->
   <PackageReference Include="System.Buffers" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Data.DataSetExtensions" Version="4.5.0" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Drawing.Primitives" Version="4.3.0" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Memory" Version="4.6.3" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Numerics.Vectors" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.Threading.Tasks.Extensions" Version="4.6.3" Condition="'$(TargetFramework)' == 'net472'" />
   <PackageReference Include="System.ValueTuple" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
   ```

4. **Expected Breaking Changes**

   **WPF API Changes (304 issues):**
   - Similar WPF breaking changes as NodeNetwork
   - Toolkit-specific UI components may be affected
   - Custom control rendering updates

   **API Compatibility:**
   - Binary incompatible APIs (656 issues): Requires recompilation
   - Source incompatible APIs (4 issues): May require code changes
   - Behavioral changes (8 issues): Requires testing

   **Dependency on NodeNetwork:**
   - Inherits NodeNetwork's breaking changes
   - Must be compatible with NodeNetwork's ReactiveUI.WPF 16.4.15
   - Verify reactive patterns still work

5. **Code Modifications**

   **Source Incompatibility Fixes (4 issues):**
   - Review compiler errors for API signature changes
   - Update method calls to match new signatures
   - Adjust generic type parameters if needed

   **Obsolete API Replacements:**
   - Check for deprecated API usage
   - Update to recommended alternatives
   - Review XML documentation comments for accuracy

   **Toolkit-Specific Updates:**
   - AddEndpointDropPanel view/viewmodel
   - Custom group controls
   - Layout algorithms (force-directed)
   - Value node templates

6. **Testing Strategy**

   **Per-Framework Testing:**
   - Build for net10.0-windows → verify no errors
   - Build for net8.0 → verify no errors
   - Build for net472 → verify no errors

   **Dependency Validation:**
   - Verify NodeNetwork reference resolves for all target frameworks
   - Check transitive dependencies are compatible

   **Functional Testing:**
   - NodeNetworkTests and CalculatorTests will validate toolkit functionality
   - All dependent applications must build successfully

7. **Validation Checklist**
   - [ ] Project builds for net10.0-windows without errors
   - [ ] Project builds for net8.0 without errors
   - [ ] Project builds for net472 without errors
   - [ ] No build warnings related to package conflicts
   - [ ] NodeNetwork project reference resolves correctly
   - [ ] Framework-included packages conditional on net472
   - [ ] System.Collections.Immutable updated to 10.0.3
   - [ ] All source incompatibility issues fixed
   - [ ] NodeNetworkTests passes (deferred to Phase 2)
   - [ ] CalculatorTests passes (deferred to Phase 2)
   - [ ] All dependent projects can reference this project

#### NodeNetworkToolkit.Blazor.csproj

**Current State:** net10.0 | Already upgraded
**Target State:** net10.0 (no changes)
**Status:** ✅ No action required

This Blazor toolkit project is already targeting .NET 10 and has no compatibility issues. No migration steps needed.

### Phase 3: Applications and Tests

#### ExampleCalculatorApp.csproj

**Current State:**
- Target Framework: `net6.0-windows`
- Files: 12
- Issues: 50 (42 mandatory, 8 potential)
- Depends on: NodeNetworkToolkit, NodeNetwork
- Used by: CalculatorTests

**Target State:**
- Target Framework: `net10.0-windows`

**Risk Level:** Low-Medium
- Simple calculator application
- Low issue count
- Has dependent test project (CalculatorTests)

**Migration Steps:**

1. **Prerequisites**
   - NodeNetwork.csproj and NodeNetworkToolkit.csproj must be successfully upgraded
   - Verify both dependencies build for net10.0-windows

2. **Update Target Framework**
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Action | Reason |
   |---------|----------------|----------------|--------|--------|
   | System.Collections.Immutable | 9.0.7 | 10.0.3 | Update | Recommended |
   | System.Buffers | 4.6.1 | - | **REMOVE** | Framework-included |
   | System.Data.DataSetExtensions | 4.5.0 | - | **REMOVE** | Framework-included |
   | System.Drawing.Primitives | 4.3.0 | - | **REMOVE** | Framework-included |
   | System.Memory | 4.6.3 | - | **REMOVE** | Framework-included |
   | System.Numerics.Vectors | 4.6.1 | - | **REMOVE** | Framework-included |
   | System.Threading.Tasks.Extensions | 4.6.3 | - | **REMOVE** | Framework-included |
   | System.ValueTuple | 4.6.1 | - | **REMOVE** | Framework-included |
   | Extended.Wpf.Toolkit | 4.7.25104.5739 | - | Keep | Compatible |
   | ReactiveUI | 20.4.1 | - | Keep | Compatible |
   | Microsoft.CSharp | 4.7.0 | - | Keep | Compatible |
   | System.Runtime.CompilerServices.Unsafe | 6.1.2 | - | Keep | Compatible |

4. **Expected Breaking Changes**
   - WPF API changes (11 issues): Calculator UI components
   - Binary incompatible APIs (34 issues): Requires recompilation
   - Behavioral changes (7 issues): Calculator logic, data binding

5. **Code Modifications**
   - Review calculator node implementations
   - Verify reactive bindings for calculator operations
   - Test mathematical operations for behavioral changes

6. **Validation Checklist**
   - [ ] Project builds for net10.0-windows without errors
   - [ ] Framework-included packages removed
   - [ ] Calculator UI renders correctly
   - [ ] Calculator operations work as expected
   - [ ] CalculatorTests passes (validates functionality)

---

#### ExampleCodeGenApp.csproj

**Current State:**
- Target Framework: `net6.0-windows`
- Files: 51
- Issues: 412 (384 mandatory, 28 potential)
- Depends on: NodeNetworkToolkit, NodeNetwork

**Target State:**
- Target Framework: `net10.0-windows`

**Risk Level:** Medium
- Complex code generation application
- Highest issue count among applications (412)
- Uses MoonSharp scripting engine

**Migration Steps:**

1. **Prerequisites**
   - NodeNetwork.csproj and NodeNetworkToolkit.csproj must be successfully upgraded

2. **Update Target Framework**
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Action | Reason |
   |---------|----------------|----------------|--------|--------|
   | System.Collections.Immutable | 9.0.7 | 10.0.3 | Update | Recommended |
   | System.Buffers | 4.6.1 | - | **REMOVE** | Framework-included |
   | System.Data.DataSetExtensions | 4.5.0 | - | **REMOVE** | Framework-included |
   | System.Drawing.Primitives | 4.3.0 | - | **REMOVE** | Framework-included |
   | System.Memory | 4.6.3 | - | **REMOVE** | Framework-included |
   | System.Numerics.Vectors | 4.6.1 | - | **REMOVE** | Framework-included |
   | System.Threading.Tasks.Extensions | 4.6.3 | - | **REMOVE** | Framework-included |
   | System.ValueTuple | 4.6.1 | - | **REMOVE** | Framework-included |
   | Extended.Wpf.Toolkit | 4.7.25104.5739 | - | Keep | Compatible |
   | MoonSharp | 2.0.0.0 | - | Keep | Compatible |
   | ReactiveUI | 20.4.1 | - | Keep | Compatible |
   | Microsoft.CSharp | 4.7.0 | - | Keep | Compatible |
   | System.Runtime.CompilerServices.Unsafe | 6.1.2 | - | Keep | Compatible |

4. **Expected Breaking Changes**
   - WPF API changes (206 issues): Significant UI component changes
   - Binary incompatible APIs (376 issues): Large API surface requiring recompilation
   - Behavioral changes (27 issues): Code generation logic, scripting behavior

5. **Code Modifications**
   - Review code generation engine
   - Verify MoonSharp integration (Lua scripting)
   - Test template system
   - Validate generated code output

6. **Validation Checklist**
   - [ ] Project builds for net10.0-windows without errors
   - [ ] Framework-included packages removed
   - [ ] Code generation UI functional
   - [ ] MoonSharp scripting engine works correctly
   - [ ] Generated code compiles and runs
   - [ ] Template system operates as expected

---

#### ExampleShaderEditorApp.csproj

**Current State:**
- Target Framework: `net6.0-windows`
- Files: 48
- Issues: 270 (229 mandatory, 41 potential)
- Depends on: NodeNetworkToolkit, NodeNetwork
- Technologies: WPF, Windows Forms (OpenGL control), GDI+

**Target State:**
- Target Framework: `net10.0-windows`

**Risk Level:** Medium
- OpenGL/shader integration complexity
- Windows Forms interop (OpenTK.GLControl)
- GDI+ rendering (19 issues)
- Source incompatibility issues (21 potential)

**Migration Steps:**

1. **Prerequisites**
   - NodeNetwork.csproj and NodeNetworkToolkit.csproj must be successfully upgraded

2. **Update Target Framework**
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```

3. **Package Updates**

   | Package | Current Version | Target Version | Action | Reason |
   |---------|----------------|----------------|--------|--------|
   | System.Collections.Immutable | 9.0.7 | 10.0.3 | Update | Recommended |
   | System.Buffers | 4.6.1 | - | **REMOVE** | Framework-included |
   | System.Data.DataSetExtensions | 4.5.0 | - | **REMOVE** | Framework-included |
   | System.Drawing.Primitives | 4.3.0 | - | **REMOVE** | Framework-included |
   | System.Memory | 4.6.3 | - | **REMOVE** | Framework-included |
   | System.Numerics.Vectors | 4.6.1 | - | **REMOVE** | Framework-included |
   | System.Threading.Tasks.Extensions | 4.6.3 | - | **REMOVE** | Framework-included |
   | System.ValueTuple | 4.6.1 | - | **REMOVE** | Framework-included |
   | Extended.Wpf.Toolkit | 4.7.25104.5739 | - | Keep | Compatible |
   | OpenTK | 4.9.4 | - | Keep | Compatible |
   | OpenTK.GLControl | 4.0.2 | - | Keep | Compatible |
   | MathNet.Numerics | 5.0.0 | - | Keep | Compatible |
   | MathNet.Spatial | 0.6.0 | - | Keep | Compatible |
   | ReactiveUI | 20.4.1 | - | Keep | Compatible |
   | Microsoft.CSharp | 4.7.0 | - | Keep | Compatible |
   | System.Runtime.CompilerServices.Unsafe | 6.1.2 | - | Keep | Compatible |

4. **Expected Breaking Changes**
   - **WPF API changes (102 issues)**: Shader editor UI
   - **Windows Forms API changes (28 issues)**: OpenGL control hosting
   - **GDI+ changes (19 issues)**: Graphics rendering, drawing operations
   - **Source incompatibility (21 issues)**: Method signature changes requiring code updates
   - Binary incompatible APIs (221 issues): Recompilation required
   - Behavioral changes (19 issues): Rendering pipeline, math operations

5. **Code Modifications**

   **Source Incompatibility Fixes:**
   - Review compiler errors for API signature changes
   - Update method calls to new signatures
   - Adjust graphics operations for GDI+ changes

   **WinForms Interop:**
   - Verify OpenTK.GLControl embedding in WPF
   - Test WindowsFormsHost compatibility
   - Validate OpenGL context creation

   **Shader-Specific:**
   - Test GLSL shader compilation
   - Verify shader parameter binding
   - Validate rendering output

6. **Testing Strategy**
   - Build and verify no compilation errors
   - Test OpenGL rendering
   - Verify shader compilation and execution
   - Test mathematical node operations (MathNet)
   - Validate UI responsiveness with real-time rendering

7. **Validation Checklist**
   - [ ] Project builds for net10.0-windows without errors
   - [ ] Framework-included packages removed
   - [ ] OpenGL control renders correctly
   - [ ] Shader compilation works
   - [ ] Shader preview updates in real-time
   - [ ] No rendering artifacts or performance issues
   - [ ] Mathematical operations accurate (MathNet)

---

#### StressTest.csproj

**Current State:**
- Target Framework: `net6.0-windows`
- Issues: 51 (43 mandatory, 8 potential)
- Depends on: NodeNetworkToolkit, NodeNetwork

**Target State:**
- Target Framework: `net10.0-windows`

**Risk Level:** Low
- Standalone stress testing application
- No dependents
- Low complexity

**Migration Steps:**

1. **Prerequisites**
   - NodeNetwork.csproj and NodeNetworkToolkit.csproj must be successfully upgraded

2. **Update Target Framework**
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```

3. **Package Updates**
   - Same pattern as other applications: update System.Collections.Immutable to 10.0.3, remove framework-included packages

4. **Expected Breaking Changes**
   - Binary incompatible APIs: Requires recompilation
   - Focus on performance testing logic

5. **Validation Checklist**
   - [ ] Project builds without errors
   - [ ] Stress test runs to completion
   - [ ] Performance metrics acceptable (compare .NET 6 vs .NET 10)
   - [ ] No memory leaks or crashes under load

---

#### NodeNetworkTests.csproj

**Current State:**
- Target Framework: `net6.0-windows`
- Issues: 1 (1 mandatory: target framework change)
- Depends on: NodeNetworkToolkit, NodeNetwork

**Target State:**
- Target Framework: `net10.0-windows`

**Risk Level:** Low
- Test project with minimal changes
- Critical for validation

**Migration Steps:**

1. **Prerequisites**
   - NodeNetwork.csproj and NodeNetworkToolkit.csproj must be successfully upgraded

2. **Update Target Framework**
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```

3. **Package Updates**
   - Update test framework packages if needed
   - Verify MSTest.TestFramework and MSTest.TestAdapter compatibility

4. **Validation Checklist**
   - [ ] Project builds without errors
   - [ ] All tests discovered
   - [ ] All tests pass
   - [ ] No test framework compatibility issues

---

#### CalculatorTests.csproj

**Current State:**
- Target Framework: `net6.0-windows`
- Issues: 1 (1 mandatory: target framework change)
- Depends on: NodeNetworkToolkit, ExampleCalculatorApp

**Target State:**
- Target Framework: `net10.0-windows`

**Risk Level:** Low
- Test project with minimal changes
- Validates ExampleCalculatorApp functionality

**Migration Steps:**

1. **Prerequisites**
   - ExampleCalculatorApp.csproj, NodeNetworkToolkit.csproj, and NodeNetwork.csproj must be successfully upgraded

2. **Update Target Framework**
   ```xml
   <TargetFramework>net10.0-windows</TargetFramework>
   ```

3. **Package Updates**
   - Update test framework packages if needed
   - Verify MSTest compatibility

4. **Validation Checklist**
   - [ ] Project builds without errors
   - [ ] All calculator tests discovered
   - [ ] All tests pass
   - [ ] Calculator functionality validated

## Package Update Reference

### Package Update Summary

**Total Packages: 26**
- Compatible (no changes): 18
- Updates recommended: 1
- Incompatible (requires action): 2
- Framework-included (conditional removal): 7

### Critical Package Changes

#### ReactiveUI.WPF (INCOMPATIBLE - DOWNGRADE REQUIRED)

| Attribute | Value |
|-----------|-------|
| Current Version | 20.4.1 |
| Target Version | **16.4.15** (DOWNGRADE) |
| Reason | Version 20.4.1 is not compatible with .NET 10 |
| Affected Projects | NodeNetwork.csproj |
| Risk Level | High |
| Action | Downgrade to last known compatible version |

**Mitigation:**
- Test all ReactiveUI functionality after downgrade
- Verify no features from versions 17.x-20.x are used
- Consider alternative if 16.4.15 has limitations

**Alternative Options:**
1. Research ReactiveUI.WPF versions 17.x-20.x for .NET 10 compatibility
2. Migrate to System.Reactive directly
3. Implement custom reactive wrappers

#### ReactiveUI.Events.WPF (DEPRECATED - REMOVE)

| Attribute | Value |
|-----------|-------|
| Current Version | 15.1.1 |
| Target Version | N/A (REMOVE) |
| Reason | Package is obsolete |
| Affected Projects | NodeNetwork.csproj |
| Risk Level | Medium |
| Action | Remove package reference entirely |

**Code Changes Required:**
- Search for `using ReactiveUI.Events;`
- Replace with `Observable.FromEventPattern` or ReactiveUI built-in event handling
- Review all event subscription patterns

### Recommended Updates

#### System.Collections.Immutable

| Attribute | Value |
|-----------|-------|
| Current Version | 9.0.7 |
| Target Version | 10.0.3 |
| Reason | Recommended upgrade for .NET 10 |
| Affected Projects | NodeNetwork, NodeNetworkToolkit, ExampleCalculatorApp, ExampleCodeGenApp, ExampleShaderEditorApp, StressTest |
| Risk Level | Low |
| Action | Update to 10.0.3 |

### Framework-Included Packages (Conditional Removal)

The following 7 packages are now included in the .NET 10 framework. For multi-targeting projects (NodeNetwork, NodeNetworkToolkit), use **conditional package references** to keep them only for net472.

| Package | Current Version | Action | Condition |
|---------|----------------|--------|-----------|
| System.Buffers | 4.6.1 | Remove for net10.0/net8.0 | Keep for net472 |
| System.Data.DataSetExtensions | 4.5.0 | Remove for net10.0/net8.0 | Keep for net472 |
| System.Drawing.Primitives | 4.3.0 | Remove for net10.0/net8.0 | Keep for net472 |
| System.Memory | 4.6.3 | Remove for net10.0/net8.0 | Keep for net472 |
| System.Numerics.Vectors | 4.6.1 | Remove for net10.0/net8.0 | Keep for net472 |
| System.Threading.Tasks.Extensions | 4.6.3 | Remove for net10.0/net8.0 | Keep for net472 |
| System.ValueTuple | 4.6.1 | Remove for net10.0/net8.0 | Keep for net472 |

**Multi-Targeting Example (NodeNetwork, NodeNetworkToolkit):**
```xml
<!-- Conditional: Only for net472 -->
<PackageReference Include="System.Buffers" Version="4.6.1" Condition="'$(TargetFramework)' == 'net472'" />
```

**Single-Targeting (All Applications):**
```xml
<!-- Remove entirely -->
<!-- <PackageReference Include="System.Buffers" Version="4.6.1" /> -->
```

### Compatible Packages (No Changes)

The following 18 packages are compatible with .NET 10 and require no version changes:

| Package | Version | Used In |
|---------|---------|---------|
| Extended.Wpf.Toolkit | 4.7.25104.5739 | ExampleCalculatorApp, ExampleCodeGenApp, ExampleShaderEditorApp |
| log4net | 3.1.0 | NodeNetwork |
| MathNet.Numerics | 5.0.0 | ExampleShaderEditorApp |
| MathNet.Spatial | 0.6.0 | ExampleShaderEditorApp |
| Microsoft.AspNetCore.Components.Web | 10.0.3 | NodeNetwork.Blazor, NodeNetworkToolkit.Blazor |
| Microsoft.CSharp | 4.7.0 | All projects |
| Microsoft.NET.Test.Sdk | 15.9.2 | Test projects |
| MoonSharp | 2.0.0.0 | ExampleCodeGenApp |
| MSTest.TestAdapter | 3.9.3 | Test projects |
| MSTest.TestFramework | 3.9.3 | Test projects |
| OpenTK | 4.9.4 | ExampleShaderEditorApp |
| OpenTK.GLControl | 4.0.2 | ExampleShaderEditorApp |
| ReactiveUI | 20.4.1 | All WPF projects |
| ReactiveUI.Testing | 20.4.1 | Test projects |
| Splat.Drawing | 13.1.63 | NodeNetwork |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | NodeNetwork, NodeNetworkToolkit, Applications |

### Package Update Matrix by Project

| Project | Updates | Removals | Conditional | Notes |
|---------|---------|----------|-------------|-------|
| **NodeNetwork.csproj** | System.Collections.Immutable: 9.0.7→10.0.3<br>ReactiveUI.WPF: 20.4.1→**16.4.15** | ReactiveUI.Events.WPF | 7 packages conditional on net472 | **High Risk** |
| **NodeNetworkToolkit.csproj** | System.Collections.Immutable: 9.0.7→10.0.3 | - | 7 packages conditional on net472 | Medium Risk |
| **ExampleCalculatorApp** | System.Collections.Immutable: 9.0.7→10.0.3 | 7 framework-included | - | Low Risk |
| **ExampleCodeGenApp** | System.Collections.Immutable: 9.0.7→10.0.3 | 7 framework-included | - | Medium Risk |
| **ExampleShaderEditorApp** | System.Collections.Immutable: 9.0.7→10.0.3 | 7 framework-included | - | Medium Risk |
| **StressTest** | System.Collections.Immutable: 9.0.7→10.0.3 | 7 framework-included | - | Low Risk |
| **NodeNetworkTests** | - | - | - | Minimal |
| **CalculatorTests** | - | - | - | Minimal |
| **NodeNetwork.Blazor** | N/A | N/A | N/A | Already .NET 10 |
| **NodeNetworkToolkit.Blazor** | N/A | N/A | N/A | Already .NET 10 |

### Package Update Execution Order

Due to All-At-Once strategy, all package updates happen **simultaneously** across all projects. However, validation follows dependency order:

1. **NodeNetwork.csproj**: Update and verify first (foundation)
2. **NodeNetworkToolkit.csproj**: Update and verify (depends on NodeNetwork)
3. **All Applications**: Update and verify (depend on both libraries)
4. **Test Projects**: Update and verify last (depend on applications)

## Breaking Changes Catalog

### Overview

**Total Breaking Changes: 3,087 issues**
- Mandatory: 2,981
- Potential: 105
- Optional: 1

### Breaking Change Categories

#### 1. Binary Incompatibility (API.0001) - 2,896 issues

**Definition:** APIs that require recompilation but may work without code changes.

**Severity:** Mandatory

**Impact by Project:**

| Project | Binary Incompatible APIs |
|---------|-------------------------|
| NodeNetwork.csproj | 1,608 |
| NodeNetworkToolkit.csproj | 656 |
| ExampleCodeGenApp | 376 |
| ExampleShaderEditorApp | 221 |
| ExampleCalculatorApp | 34 |
| StressTest | ~1 |

**Resolution:**
- Recompile all projects
- Most issues resolve automatically upon recompilation
- Monitor for compilation errors indicating actual code changes needed

**Technologies Affected:**
- WPF APIs (primary source)
- Windows Forms APIs
- GDI+ / System.Drawing APIs

---

#### 2. Behavioral Changes (API.0003) - 67 issues

**Definition:** APIs that compile but may behave differently at runtime.

**Severity:** Potential

**Impact by Project:**

| Project | Behavioral Changes |
|---------|-------------------|
| ExampleCodeGenApp | 27 |
| ExampleShaderEditorApp | 19 |
| NodeNetworkToolkit.csproj | 8 |
| ExampleCalculatorApp | 7 |
| NodeNetwork.csproj | 6 |

**Resolution Strategy:**
1. **Identify Affected Areas:**
   - WPF rendering pipeline
   - Data binding expressions
   - Dependency property callbacks
   - Observable collections
   - Mathematical operations (MathNet in ShaderEditor)

2. **Testing Approach:**
   - Run comprehensive test suites
   - Manual UI testing for visual changes
   - Compare .NET 6 vs .NET 10 behavior
   - Validate business logic correctness

3. **Common Behavioral Changes:**
   - Dependency property default values
   - Event raising order
   - Collection change notifications
   - Async operation completion timing
   - String comparison defaults

**High-Risk Areas:**
- ExampleCodeGenApp: Code generation logic (27 issues)
- ExampleShaderEditorApp: Shader rendering (19 issues)

---

#### 3. Source Incompatibility (API.0002) - 25 issues

**Definition:** APIs with changed signatures requiring code modifications.

**Severity:** Potential

**Impact by Project:**

| Project | Source Incompatible APIs |
|---------|-------------------------|
| ExampleShaderEditorApp | 21 |
| NodeNetworkToolkit.csproj | 4 |

**Resolution:**
1. **Compilation Errors Expected:**
   - Method signature mismatches
   - Parameter type changes
   - Return type changes
   - Generic constraint modifications

2. **Fix Approach:**
   - Identify compiler error locations
   - Review .NET 10 migration documentation for specific APIs
   - Update method calls to new signatures
   - Adjust type parameters

3. **Likely Affected APIs:**
   - GDI+ drawing methods (ExampleShaderEditorApp)
   - Windows Forms OpenGL control integration
   - Mathematical type conversions

---

### Technology-Specific Breaking Changes

#### WPF (Windows Presentation Foundation) - 1,221 issues

**Affected Projects:**
- NodeNetwork.csproj: 587 issues
- NodeNetworkToolkit.csproj: 304 issues
- ExampleCodeGenApp: 206 issues
- ExampleShaderEditorApp: 102 issues
- ExampleCalculatorApp: 11 issues
- Others: ~11 issues

**Common WPF Breaking Changes:**

1. **Dependency Properties:**
   - Default value changes
   - Property changed callback behavior
   - Coerce value callback timing

2. **Data Binding:**
   - Binding expression evaluation order
   - UpdateSourceTrigger default changes
   - Converter behavior modifications

3. **Visual Tree:**
   - Layout calculation changes
   - Visual tree traversal order
   - Rendering pipeline updates

4. **Controls:**
   - Control template structure
   - Default style changes
   - Event routing modifications

**Testing Strategy:**
- Visual inspection of all WPF windows
- Verify data binding updates correctly
- Test custom control rendering
- Validate dependency property behaviors

---

#### Windows Forms - 28 issues

**Affected Projects:**
- ExampleShaderEditorApp: 28 issues (OpenTK.GLControl hosting)

**Common Issues:**
- WindowsFormsHost interop changes
- Control sizing and layout
- OpenGL context creation timing
- Paint event handling

**Testing Strategy:**
- Verify OpenGL control renders correctly
- Test WPF↔WinForms interaction
- Validate OpenGL context initialization

---

#### GDI+ / System.Drawing - 19 issues

**Affected Projects:**
- ExampleShaderEditorApp: 19 issues

**Common Issues:**
- Graphics drawing API changes
- Image format handling
- Font rendering updates
- Color space conversions

**Testing Strategy:**
- Test all graphics rendering code
- Verify shader preview rendering
- Validate image export functionality

---

#### Legacy Configuration System - 2 issues

**Severity:** Low
**Impact:** Minimal - likely related to app.config in applications

**Resolution:**
- Verify app.config files load correctly
- Test configuration settings
- Migrate to modern configuration if needed

---

### Package-Related Breaking Changes

#### ReactiveUI.WPF Downgrade (20.4.1 → 16.4.15)

**Potential Breaking Changes:**
1. **Features Removed in Downgrade:**
   - Any features introduced in versions 17.x-20.x
   - API improvements from recent versions
   - Bug fixes that may be reverted

2. **Testing Strategy:**
   - Verify all reactive commands work
   - Test reactive property bindings
   - Validate observable subscriptions
   - Check view activation/deactivation

3. **Code Review Areas:**
   - `ReactiveCommand` usage
   - `WhenAnyValue` expressions
   - `BindTo` patterns
   - View lifecycle (`WhenActivated`)

#### ReactiveUI.Events.WPF Removal

**Code Changes Required:**
1. **Find Usage:**
   ```csharp
   // Search for:
   using ReactiveUI.Events;
   Events().PreviewMouseDown // Event wrapper extensions
   ```

2. **Replace With:**
   ```csharp
   // Option 1: Observable.FromEventPattern
   Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
       h => control.PreviewMouseDown += h,
       h => control.PreviewMouseDown -= h)

   // Option 2: ReactiveUI built-in
   this.Events().MouseDown // If available in ReactiveUI core
   ```

3. **Affected Patterns:**
   - Event-to-observable conversions
   - WPF event subscriptions in view models
   - Reactive event handling

---

### Framework-Included Package Removals

**Packages Being Removed:**
- System.Buffers
- System.Data.DataSetExtensions
- System.Drawing.Primitives
- System.Memory
- System.Numerics.Vectors
- System.Threading.Tasks.Extensions
- System.ValueTuple

**Potential Issues:**
1. **Type Ambiguity:**
   - Multiple definitions of same type
   - Namespace conflicts

2. **Resolution:**
   - Remove explicit package references
   - Framework provides implementation automatically
   - Use conditional references for net472 multi-targeting

3. **Testing:**
   - Verify no compilation errors
   - Check for type ambiguity warnings
   - Validate functionality unchanged

---

### Breaking Change Resolution Workflow

**Step 1: Identify**
- Build solution after framework update
- Collect all compilation errors and warnings
- Group by project and category

**Step 2: Categorize**
- Binary incompatibility: Expect to resolve on recompilation
- Source incompatibility: Requires code changes
- Behavioral changes: Requires testing

**Step 3: Fix Source Incompatibility**
- Address compilation errors first
- Update method signatures
- Adjust type usage
- Fix generic constraints

**Step 4: Test for Behavioral Changes**
- Run all unit tests
- Execute integration tests
- Perform manual UI testing
- Compare .NET 6 vs .NET 10 behavior

**Step 5: Validate**
- All tests pass
- No visual regressions
- Performance acceptable
- Functionality preserved

---

### High-Priority Breaking Changes

**Critical (Must Fix Before Testing):**
1. ReactiveUI.WPF downgrade compatibility
2. ReactiveUI.Events.WPF removal
3. Source incompatibility in ExampleShaderEditorApp (21 issues)
4. Framework-included package conflicts

**Important (Validate During Testing):**
1. WPF rendering behavioral changes (1,221 issues)
2. Code generation logic behavioral changes (27 issues in ExampleCodeGenApp)
3. Shader rendering behavioral changes (19 issues in ExampleShaderEditorApp)
4. OpenGL/WinForms interop (28 issues)

**Low-Priority (Monitor):**
1. Configuration system changes (2 issues)
2. Minor behavioral changes in test projects

---

### Breaking Change References

**Official Documentation:**
- [.NET 10 Breaking Changes](https://learn.microsoft.com/en-us/dotnet/core/compatibility/10.0)
- [WPF Breaking Changes](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/migration/)
- [Windows Forms Breaking Changes](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/migration/)

**Package-Specific:**
- ReactiveUI: https://www.reactiveui.net/docs/handbook/
- OpenTK: https://opentk.net/
- MoonSharp: http://www.moonsharp.org/

## Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---------|-----------|-------------|------------|
| NodeNetwork.csproj | High | 1,625 issues; ReactiveUI.WPF incompatibility; foundation for 6 projects | Downgrade ReactiveUI.WPF to 16.4.15; thorough testing after fixes; multi-targeting adds complexity |
| NodeNetworkToolkit.csproj | High | 677 issues; depends on NodeNetwork; used by 6 projects | Address after NodeNetwork; verify dependency compatibility; test multi-targeting scenarios |
| ExampleCodeGenApp.csproj | Medium | 412 issues; complex application | Comprehensive testing of code generation features |
| ExampleShaderEditorApp.csproj | Medium | 270 issues; OpenGL/shader integration | Test shader compilation and rendering |
| ExampleCalculatorApp.csproj | Medium | 50 issues; has dependent test project | Verify CalculatorTests compatibility |

### Security Vulnerabilities

**Status:** ✅ No security vulnerabilities identified

The assessment did not flag any NuGet packages with security vulnerabilities. All packages are either compatible or have clear upgrade/replacement paths.

### Package-Specific Risks

**Critical:**
1. **ReactiveUI.WPF 20.4.1 → 16.4.15** (DOWNGRADE)
   - Risk: Version 20.4.1 is incompatible with .NET 10
   - Mitigation: Downgrade to last compatible version 16.4.15
   - Impact: Affects NodeNetwork.csproj only
   - Alternative: Consider migrating to alternative reactive framework if 16.4.15 has limitations

**High Priority:**
2. **ReactiveUI.Events.WPF 15.1.1** (REMOVAL)
   - Risk: Package is deprecated
   - Mitigation: Remove package reference entirely
   - Impact: Affects NodeNetwork.csproj only
   - Action: Review code for usage, replace with alternative event patterns if needed

**Medium Priority:**
3. **System.Collections.Immutable 9.0.7 → 10.0.3** (UPGRADE)
   - Risk: Version mismatch may cause compatibility issues
   - Mitigation: Upgrade to recommended version 10.0.3
   - Impact: Affects NodeNetwork.csproj and NodeNetworkToolkit.csproj

4. **Framework-Included Packages** (REMOVAL - 6 packages)
   - Risk: Conflicts with framework-provided implementations
   - Mitigation: Remove these packages for net10.0 target framework (conditional removal for multi-targeting)
   - Packages affected:
     - System.Buffers 4.6.1
     - System.Data.DataSetExtensions 4.5.0
     - System.Drawing.Primitives 4.3.0
     - System.Memory 4.6.3
     - System.Numerics.Vectors 4.6.1
     - System.Threading.Tasks.Extensions 4.6.3
     - System.ValueTuple 4.6.1

### API Breaking Changes

**WPF (1,221 issues):**
- Concentrated in NodeNetwork.csproj (587 issues) and NodeNetworkToolkit.csproj (304 issues)
- Risk: Behavioral changes in WPF rendering, data binding, or dependency properties
- Mitigation: Thorough testing of UI functionality; leverage existing test projects

**Windows Forms (28 issues):**
- Minor compatibility issues
- Risk: Low - likely in interop scenarios
- Mitigation: Verify any WinForms interop code

**GDI+ / System.Drawing (19 issues):**
- Related to graphics operations
- Risk: Medium - may affect rendering in shader editor
- Mitigation: Test rendering pipeline thoroughly

### Multi-Targeting Risks

**NodeNetwork.csproj and NodeNetworkToolkit.csproj:**
- Risk: Package compatibility across net10.0-windows, net8.0, and net472
- Mitigation: 
  - Use conditional package references where needed
  - Test build output for all three target frameworks
  - Verify no framework-included packages break net472 builds

### Contingency Plans

**If ReactiveUI.WPF 16.4.15 has breaking changes:**
- Option 1: Investigate ReactiveUI.WPF version range compatibility (test versions 17.x - 20.x)
- Option 2: Migrate to alternative reactive framework (e.g., System.Reactive directly)
- Option 3: Implement custom reactive wrappers for WPF controls

**If compilation errors exceed expected scope:**
- Pause atomic upgrade
- Analyze error patterns
- Consider targeted fixes for high-frequency errors
- Resume with adjusted strategy

**If tests fail after upgrade:**
- Isolate failing test scope (specific project/feature)
- Fix incrementally
- Re-run tests after each fix
- Do not merge to master until all tests pass

**If performance degradation detected in StressTest:**
- Profile .NET 10 vs .NET 6 execution
- Identify hot paths
- Apply .NET 10 specific optimizations (Span<T>, stackalloc, etc.)
- Consider targeted rollback if performance critical

### Rollback Procedure

**Immediate Rollback (If Critical Blocker):**
```bash
git reset --hard HEAD
git checkout master
```

**Partial Rollback (If Specific Project Fails):**
- Not applicable for All-At-Once strategy
- Must rollback entire atomic upgrade
- Fix issue and retry

**Merge Rollback (If Issues Found Post-Merge):**
```bash
git revert <merge-commit-hash>
```

### Risk Assessment by Phase

**Phase 1: Atomic Upgrade**
- **Risk Level:** High
- **Critical Dependencies:** ReactiveUI.WPF downgrade must succeed
- **Validation:** Solution builds with 0 errors
- **Fallback:** Complete rollback via Git reset

**Phase 2: Testing**
- **Risk Level:** Medium
- **Critical Dependencies:** All tests must pass
- **Validation:** NodeNetworkTests, CalculatorTests, StressTest all green
- **Fallback:** Keep on feature branch, do not merge until fixed

## Testing & Validation Strategy

### Multi-Level Testing Approach

#### Level 1: Compilation Validation

**Objective:** Verify all projects build successfully for all target frameworks.

**Per-Project Build Validation:**

```bash
# Multi-targeting projects (build all frameworks)
dotnet build NodeNetwork\NodeNetwork.csproj -c Release
# Expected output: 3 builds (net10.0-windows, net8.0, net472)

dotnet build NodeNetworkToolkit\NodeNetworkToolkit.csproj -c Release
# Expected output: 3 builds (net10.0-windows, net8.0, net472)

# Single-targeting projects
dotnet build ExampleCalculatorApp\ExampleCalculatorApp.csproj -c Release
dotnet build ExampleCodeGenApp\ExampleCodeGenApp.csproj -c Release
dotnet build ExampleShaderEditorApp\ExampleShaderEditorApp.csproj -c Release
dotnet build StressTest\StressTest.csproj -c Release
dotnet build NodeNetworkTests\NodeNetworkTests.csproj -c Release
dotnet build CalculatorTests\CalculatorTests.csproj -c Release

# Or build entire solution
dotnet build NodeNetwork.sln -c Release
```

**Success Criteria:**
- [ ] All projects build with 0 errors
- [ ] No warnings related to:
  - Package version conflicts
  - Multi-targeting issues
  - Framework compatibility
  - Type ambiguity from framework-included packages
- [ ] Build output confirms all target frameworks compiled

---

#### Level 2: Automated Test Execution

**Objective:** Validate functionality through automated tests.

**Test Projects:**

**NodeNetworkTests.csproj**
```bash
dotnet test NodeNetworkTests\NodeNetworkTests.csproj -c Release --logger "console;verbosity=detailed"
```

**Validates:**
- Core NodeNetwork functionality
- Reactive bindings
- Graph operations
- Node/connection management
- Dependency property behaviors

**CalculatorTests.csproj**
```bash
dotnet test CalculatorTests\CalculatorTests.csproj -c Release --logger "console;verbosity=detailed"
```

**Validates:**
- Calculator application logic
- Mathematical operations
- Node evaluation
- UI bindings

**Success Criteria:**
- [ ] All tests discovered successfully
- [ ] All tests pass (100% pass rate)
- [ ] No test framework compatibility issues
- [ ] Test execution time comparable to .NET 6 baseline

---

#### Level 3: Manual Application Testing

**Objective:** Verify UI functionality and behavioral correctness.

**ExampleCalculatorApp Manual Tests:**
1. Launch application
2. Create calculator nodes (addition, subtraction, multiplication, division)
3. Connect nodes to form calculation graph
4. Verify results display correctly
5. Test node dragging and connection creation
6. Verify UI responsiveness

**Expected Results:**
- [ ] Application launches without errors
- [ ] All calculator node types functional
- [ ] Node connections work correctly
- [ ] Results calculate accurately
- [ ] UI renders as expected
- [ ] No visual artifacts or layout issues

**ExampleCodeGenApp Manual Tests:**
1. Launch application
2. Create code generation nodes
3. Configure templates
4. Execute Lua scripts (MoonSharp)
5. Generate code output
6. Verify generated code compiles

**Expected Results:**
- [ ] Application launches without errors
- [ ] Code generation nodes functional
- [ ] MoonSharp scripting works
- [ ] Templates process correctly
- [ ] Generated code is valid
- [ ] No scripting engine errors

**ExampleShaderEditorApp Manual Tests:**
1. Launch application
2. Verify OpenGL control renders
3. Create shader nodes
4. Connect shader graph
5. Compile GLSL shaders
6. Verify real-time shader preview
7. Test mathematical nodes (MathNet)

**Expected Results:**
- [ ] Application launches without errors
- [ ] OpenGL context creates successfully
- [ ] Shader preview renders correctly
- [ ] No rendering artifacts
- [ ] Shader compilation works
- [ ] Performance acceptable (60 FPS target)
- [ ] Mathematical operations accurate

---

#### Level 4: Performance & Stress Testing

**Objective:** Validate performance under load and identify regressions.

**StressTest.csproj Execution:**
```bash
# Run stress test application
StressTest\bin\Release\net10.0-windows\StressTest.exe
```

**Test Scenarios:**
1. Create large node graph (1000+ nodes)
2. Rapid connection creation/deletion
3. Continuous node evaluation
4. Memory usage monitoring
5. UI responsiveness under load

**Performance Baseline (from .NET 6):**
- Establish baseline metrics for comparison
- Monitor memory consumption
- Track CPU usage
- Measure UI frame rate

**Success Criteria:**
- [ ] Application runs to completion without crashes
- [ ] Memory usage within acceptable bounds (no leaks)
- [ ] Performance comparable to .NET 6 baseline (±10%)
- [ ] No UI freezing under load
- [ ] Graph operations remain responsive

**Performance Comparison:**
```
Metric               | .NET 6 Baseline | .NET 10 Result | Status
---------------------|-----------------|----------------|--------
Peak Memory (MB)     | [baseline]      | [measured]     | [Pass/Fail]
Avg CPU (%)          | [baseline]      | [measured]     | [Pass/Fail]
1000 Node Creation   | [baseline]      | [measured]     | [Pass/Fail]
UI Frame Rate (FPS)  | [baseline]      | [measured]     | [Pass/Fail]
```

---

#### Level 5: Multi-Targeting Validation

**Objective:** Verify multi-targeting libraries work across all target frameworks.

**NodeNetwork.csproj Multi-Framework Tests:**

```bash
# Build for each target framework explicitly
dotnet build NodeNetwork\NodeNetwork.csproj -f net10.0-windows
dotnet build NodeNetwork\NodeNetwork.csproj -f net8.0
dotnet build NodeNetwork\NodeNetwork.csproj -f net472
```

**Validate:**
- [ ] net10.0-windows build succeeds
- [ ] net8.0 build succeeds  
- [ ] net472 build succeeds
- [ ] Framework-included packages not referenced for net10.0/net8.0
- [ ] Framework-included packages included for net472
- [ ] No package conflicts across target frameworks

**NodeNetworkToolkit.csproj Multi-Framework Tests:**

```bash
dotnet build NodeNetworkToolkit\NodeNetworkToolkit.csproj -f net10.0-windows
dotnet build NodeNetworkToolkit\NodeNetworkToolkit.csproj -f net8.0
dotnet build NodeNetworkToolkit\NodeNetworkToolkit.csproj -f net472
```

**Validate:**
- [ ] All target frameworks build successfully
- [ ] Conditional package references work correctly
- [ ] NodeNetwork dependency resolves for all frameworks

---

### Regression Testing Checklist

**Visual Regressions:**
- [ ] No layout changes in WPF windows
- [ ] Control rendering identical to .NET 6
- [ ] Font rendering consistent
- [ ] Colors and themes unchanged
- [ ] Icons and images display correctly

**Functional Regressions:**
- [ ] Node creation works
- [ ] Connection creation works
- [ ] Node deletion works
- [ ] Connection deletion works
- [ ] Graph serialization/deserialization works
- [ ] Reactive bindings update correctly
- [ ] Commands execute properly

**Performance Regressions:**
- [ ] Application startup time unchanged
- [ ] Node graph rendering performance maintained
- [ ] Memory usage stable
- [ ] No new memory leaks
- [ ] UI responsiveness preserved

---

### Breaking Change Validation

**ReactiveUI.WPF Downgrade Validation:**
- [ ] All reactive commands functional
- [ ] `WhenAnyValue` expressions work
- [ ] `BindTo` patterns work
- [ ] View activation/deactivation works
- [ ] Observable subscriptions function correctly

**ReactiveUI.Events.WPF Removal Validation:**
- [ ] No compilation errors from missing Events namespace
- [ ] Alternative event handling works
- [ ] All WPF event subscriptions functional

**Framework-Included Package Validation:**
- [ ] No type ambiguity warnings
- [ ] No package conflict errors
- [ ] All System.* types resolve correctly
- [ ] Functionality using these types works

---

### Test Execution Order

**Recommended Sequence:**

1. **Compilation Validation** (Level 1)
   - Must pass before proceeding
   - Fix all compilation errors

2. **Automated Tests** (Level 2)
   - NodeNetworkTests first (foundation)
   - CalculatorTests second (application)
   - Fix any failures before manual testing

3. **Manual Application Tests** (Level 3)
   - ExampleCalculatorApp (simplest, validates basic functionality)
   - ExampleCodeGenApp (validates scripting integration)
   - ExampleShaderEditorApp (validates complex rendering)

4. **Performance Testing** (Level 4)
   - StressTest execution
   - Baseline comparison
   - Performance profiling if needed

5. **Multi-Targeting Validation** (Level 5)
   - Verify all framework builds
   - Test package resolution

---

### Test Failure Response

**If Automated Tests Fail:**
1. Identify failing test
2. Determine if failure is:
   - Breaking change in .NET 10
   - ReactiveUI.WPF downgrade issue
   - Package removal side effect
3. Fix root cause
4. Re-run all tests
5. Iterate until 100% pass

**If Manual Tests Reveal Issues:**
1. Document specific behavior
2. Compare to .NET 6 baseline
3. Investigate behavioral breaking changes
4. Apply fixes
5. Re-test manually
6. Run automated tests to ensure no regressions

**If Performance Degrades:**
1. Profile .NET 10 execution
2. Identify hot paths
3. Apply .NET 10 optimizations (Span<T>, stackalloc, etc.)
4. Re-test performance
5. Document any trade-offs

---

### Success Criteria Summary

**All criteria must be met before merge:**

**Build:**
- ✅ All 10 projects build successfully
- ✅ All target frameworks compile (net10.0-windows, net8.0, net472)
- ✅ 0 compilation errors
- ✅ 0 package conflict warnings

**Tests:**
- ✅ NodeNetworkTests: 100% pass
- ✅ CalculatorTests: 100% pass
- ✅ Test framework compatibility confirmed

**Functionality:**
- ✅ ExampleCalculatorApp: All features functional
- ✅ ExampleCodeGenApp: Code generation works
- ✅ ExampleShaderEditorApp: Rendering works
- ✅ StressTest: Completes without crashes

**Performance:**
- ✅ Performance within ±10% of .NET 6 baseline
- ✅ No memory leaks detected
- ✅ UI responsiveness maintained

**Quality:**
- ✅ No visual regressions
- ✅ No functional regressions
- ✅ ReactiveUI functionality preserved
- ✅ Multi-targeting works correctly

## Complexity & Effort Assessment

### Per-Project Complexity Matrix

| Project | Complexity | LOC Impact | Package Changes | Dependency Risk | Breaking Changes | Notes |
|---------|-----------|------------|-----------------|-----------------|------------------|-------|
| NodeNetwork.csproj | **High** | High (38 files) | Critical (ReactiveUI.WPF downgrade, 7 removals) | Critical (6 dependents) | Very High (1,625 issues) | Foundation library; multi-targeting |
| NodeNetworkToolkit.csproj | **High** | Medium (24 files) | Medium (7 removals, 1 upgrade) | High (6 dependents) | High (677 issues) | Depends on NodeNetwork; multi-targeting |
| ExampleCodeGenApp.csproj | **Medium** | Medium | Medium | Low | Medium (412 issues) | Complex app features |
| ExampleShaderEditorApp.csproj | **Medium** | Medium | Medium | Low | Medium (270 issues) | OpenGL integration |
| ExampleCalculatorApp.csproj | **Low-Medium** | Low | Low | Low (1 dependent) | Low (50 issues) | Simple app |
| StressTest.csproj | **Low** | Low | Low | None | Low (51 issues) | Standalone test |
| NodeNetworkTests.csproj | **Low** | Low | Low | None | Very Low (1 issue) | Test project |
| CalculatorTests.csproj | **Low** | Low | Low | None | Very Low (1 issue) | Test project |
| NodeNetwork.Blazor.csproj | **None** | N/A | N/A | N/A | N/A | Already .NET 10 |
| NodeNetworkToolkit.Blazor.csproj | **None** | N/A | N/A | N/A | N/A | Already .NET 10 |

### Phase Complexity Assessment

**Phase 1: Atomic Upgrade**
- **Complexity Rating:** High
- **Key Challenges:**
  - ReactiveUI.WPF downgrade from 20.4.1 to 16.4.15
  - Remove 7 framework-included packages without breaking net472 multi-targeting
  - Update System.Collections.Immutable across projects
  - Fix 3,087 total issues (2,981 mandatory)
  - Multi-targeting for 2 foundation libraries
- **Dependencies:** Must handle in dependency order (NodeNetwork → NodeNetworkToolkit → Apps)
- **Estimated Scope:** Large - simultaneous update of 8 projects
- **Success Criteria:** 0 compilation errors across entire solution

**Phase 2: Testing & Validation**
- **Complexity Rating:** Medium
- **Key Challenges:**
  - Validate functionality across 2 test projects
  - Verify stress test performance
  - Confirm no UI regressions in WPF applications
  - Test multi-targeting scenarios (net10.0, net8.0, net472)
- **Dependencies:** Phase 1 must complete successfully
- **Estimated Scope:** Medium - comprehensive test coverage
- **Success Criteria:** All tests pass, no performance regression

### Dependency-Based Ordering

**Level 0 (Foundation - Highest Priority):**
1. **NodeNetwork.csproj** - Critical path project
   - Complexity: High
   - 6 dependent projects
   - Must complete first for others to succeed

2. **NodeNetwork.Blazor.csproj** - No action (already .NET 10)

**Level 1 (Toolkit):**
3. **NodeNetworkToolkit.csproj** - Critical path project
   - Complexity: High
   - Depends on NodeNetwork
   - 6 dependent projects

4. **NodeNetworkToolkit.Blazor.csproj** - No action (already .NET 10)

**Level 2 (Applications - Medium Priority):**
5. **ExampleCalculatorApp.csproj** - Complexity: Low-Medium
6. **ExampleCodeGenApp.csproj** - Complexity: Medium
7. **ExampleShaderEditorApp.csproj** - Complexity: Medium
8. **StressTest.csproj** - Complexity: Low
9. **NodeNetworkTests.csproj** - Complexity: Low

**Level 3 (Dependent Tests - Lowest Priority):**
10. **CalculatorTests.csproj** - Complexity: Low
    - Depends on ExampleCalculatorApp

### Resource Requirements

**Technical Skills Required:**
- **Essential:**
  - WPF application development
  - .NET multi-targeting configuration
  - NuGet package management
  - ReactiveUI framework knowledge
  - MSBuild and project file syntax

- **Desirable:**
  - .NET breaking changes awareness
  - WPF rendering pipeline knowledge
  - OpenGL/shader programming (for ExampleShaderEditorApp validation)
  - Performance profiling (for StressTest validation)

**Team Capacity:**
- **Single developer sufficient** due to All-At-Once strategy
- **Parallel work not required** - sequential atomic upgrade
- **Coordination overhead:** Minimal

**Time Distribution:**
- **Preparation:** SDK verification, branch setup - Already complete
- **Atomic Upgrade:** Project file updates, package updates, initial build - Medium effort
- **Compilation Fixes:** Address breaking changes - Large effort (3,087 issues)
- **Testing:** Run tests, fix failures - Medium effort
- **Validation:** Final checks, performance testing - Small effort

### Complexity Drivers

**What Makes This Upgrade Medium Complexity:**

**Positive Factors (Reduce Complexity):**
- ✅ Clean dependency graph with no cycles
- ✅ All projects on same source framework (.NET 6)
- ✅ All projects targeting same destination (.NET 10)
- ✅ 2 projects already on .NET 10 (Blazor)
- ✅ Good test coverage
- ✅ Package ecosystem mostly compatible

**Challenging Factors (Increase Complexity):**
- ⚠️ High issue count (3,087 total)
- ⚠️ ReactiveUI.WPF incompatibility requiring downgrade
- ⚠️ Multi-targeting requirement for 2 libraries
- ⚠️ 7 framework-included packages to remove
- ⚠️ Large API surface area (WPF, Windows Forms, GDI+)
- ⚠️ Foundation libraries affect 6 downstream projects each

**Net Assessment:** Medium complexity - manageable with careful execution and thorough testing.

## Source Control Strategy

### Branching Strategy

**Branch Structure:**
```
master (main branch)
  └── upgrade-to-NET10 (feature branch) ← Current
```

**Current State:**
- ✅ Source branch: `master`
- ✅ Feature branch: `upgrade-to-NET10` (created and switched)
- ✅ Pre-upgrade commit: Already committed (dba7086)
- ✅ Working directory: Clean

**Branch Purpose:**
- **master**: Stable .NET 6 codebase
- **upgrade-to-NET10**: All .NET 10 upgrade work isolated

**Merge Strategy:**
- Feature branch merged to master after all validation passes
- Pull request workflow recommended for review
- Squash merge or merge commit based on team preference

---

### Commit Strategy

**All-At-Once Approach: Single Atomic Commit**

Given the All-At-Once upgrade strategy, the preferred commit approach is a **single atomic commit** containing all upgrade changes:

**Atomic Upgrade Commit:**
```bash
git add .
git commit -m "Upgrade solution to .NET 10

- Update all 8 WPF projects to net10.0-windows
- Add net8.0 target to multi-targeting libraries (NodeNetwork, NodeNetworkToolkit)
- Downgrade ReactiveUI.WPF from 20.4.1 to 16.4.15 for .NET 10 compatibility
- Remove deprecated ReactiveUI.Events.WPF package
- Update System.Collections.Immutable to 10.0.3 across projects
- Remove 7 framework-included packages (conditional for net472)
- Fix all compilation errors from breaking changes
- Verify all tests pass (NodeNetworkTests, CalculatorTests)
- Validate multi-targeting builds (net10.0, net8.0, net472)

Projects upgraded:
- NodeNetwork.csproj
- NodeNetworkToolkit.csproj
- ExampleCalculatorApp.csproj
- ExampleCodeGenApp.csproj
- ExampleShaderEditorApp.csproj
- StressTest.csproj
- NodeNetworkTests.csproj
- CalculatorTests.csproj

Breaking changes addressed:
- 3,087 compatibility issues resolved
- WPF API changes (1,221 issues)
- Source incompatibilities fixed (25 issues)
- Behavioral changes tested and validated

Closes #[issue-number] (if tracking in issue system)"
```

**Rationale for Single Commit:**
- All-At-Once strategy means changes are interdependent
- Cannot build intermediate states (all projects must update together)
- Single commit represents single logical change: ".NET 10 upgrade"
- Easier to revert if needed (single commit to revert)
- Clear atomic boundary for upgrade

**Alternative: Milestone Commits** (if preferred)

If team prefers incremental visibility:

```bash
# Commit 1: Project file and package updates
git add **/*.csproj
git commit -m "Update project files and packages for .NET 10"

# Commit 2: Compilation fixes
git add .
git commit -m "Fix compilation errors from .NET 10 breaking changes"

# Commit 3: Test validation
git add .
git commit -m "Validate tests pass for .NET 10"
```

**Recommendation:** Use **single atomic commit** for clarity and simplicity.

---

### Commit Message Format

**Structure:**
```
<type>: <summary>

<detailed description>

<breaking changes>

<related issues>
```

**Example:**
```
feat: Upgrade solution to .NET 10 LTS

Update all WPF projects from .NET 6 to .NET 10, add .NET 8 intermediate 
target to multi-targeting libraries, and address all compatibility issues.

Key Changes:
- Target frameworks: net6.0-windows → net10.0-windows
- Multi-targeting: net6.0-windows;net472 → net10.0-windows;net8.0;net472
- ReactiveUI.WPF: 20.4.1 → 16.4.15 (downgrade for compatibility)
- System.Collections.Immutable: 9.0.7 → 10.0.3
- Removed 7 framework-included packages (conditional for net472)
- Removed deprecated ReactiveUI.Events.WPF package

Breaking Changes:
- ReactiveUI.WPF downgraded - features from v17-20 unavailable
- ReactiveUI.Events.WPF removed - use Observable.FromEventPattern
- Framework-included packages removed for net10.0/net8.0
- 3,087 API compatibility issues addressed

Testing:
- All unit tests pass (NodeNetworkTests, CalculatorTests)
- Manual testing completed for all example applications
- Performance validated (StressTest)
- Multi-targeting builds verified

Closes #123
```

---

### Review and Merge Process

**Pull Request Requirements:**

**PR Title:**
```
Upgrade NodeNetwork solution to .NET 10 LTS
```

**PR Description Template:**
```markdown
## Overview
Upgrades the entire NodeNetwork solution from .NET 6 to .NET 10 LTS using an All-At-Once strategy.

## Changes Summary
- **8 projects upgraded** to net10.0-windows
- **2 projects** already on .NET 10 (Blazor)
- **Multi-targeting** added: net8.0 intermediate LTS
- **Package updates**: 1 upgrade, 1 downgrade, 7 removals, 1 removal

## Testing Completed
- [x] All projects build successfully (0 errors, 0 warnings)
- [x] Multi-targeting validated (net10.0, net8.0, net472)
- [x] NodeNetworkTests: All pass
- [x] CalculatorTests: All pass
- [x] ExampleCalculatorApp: Manual testing ✅
- [x] ExampleCodeGenApp: Manual testing ✅
- [x] ExampleShaderEditorApp: Manual testing ✅
- [x] StressTest: Performance validated ✅

## Breaking Changes
- ReactiveUI.WPF downgraded to 16.4.15
- ReactiveUI.Events.WPF removed
- Framework-included packages removed
- See plan.md for complete breaking changes catalog

## Checklist
- [x] All builds succeed
- [x] All tests pass
- [x] No visual regressions
- [x] No functional regressions
- [x] Performance acceptable
- [x] Documentation updated (plan.md, assessment.md)

## Related Issues
Closes #123

## Reviewer Notes
- Focus on ReactiveUI.WPF downgrade impact
- Verify multi-targeting conditional package references
- Validate all example applications functional
```

**Review Checklist for Reviewers:**
- [ ] PR description complete and accurate
- [ ] All CI builds pass (if automated)
- [ ] Code changes align with plan.md
- [ ] Breaking changes documented
- [ ] Test results provided
- [ ] No unintended changes (check diff carefully)
- [ ] Multi-targeting configuration correct
- [ ] Package references accurate

**Merge Criteria:**
- ✅ All automated tests pass (CI/CD)
- ✅ At least 1 approving review
- ✅ All reviewer comments addressed
- ✅ No merge conflicts with target branch
- ✅ Documentation updated

---

### Post-Merge Activities

**After Merge to Master:**

1. **Tag Release:**
   ```bash
   git tag -a v7.0.0-net10 -m "Release: .NET 10 upgrade"
   git push origin v7.0.0-net10
   ```

2. **Update NuGet Packages** (if applicable):
   - NodeNetwork package with .NET 10 support
   - NodeNetworkToolkit package with .NET 10 support
   - Publish to NuGet.org with updated version

3. **Update Documentation:**
   - README.md: Update .NET version requirements
   - CHANGELOG.md: Document .NET 10 upgrade
   - docs/: Update any framework-specific documentation

4. **Communicate Changes:**
   - Notify team of merge
   - Highlight breaking changes (ReactiveUI.WPF downgrade)
   - Provide upgrade guide for consumers (if library)

5. **Archive Upgrade Branch:**
   ```bash
   # Optional: keep branch for reference or delete
   git branch -d upgrade-to-NET10
   git push origin --delete upgrade-to-NET10
   ```

---

### Rollback Procedure

**If Critical Issues Found Post-Merge:**

**Option 1: Revert Merge Commit**
```bash
git revert -m 1 <merge-commit-hash>
git push origin master
```

**Option 2: Reset to Pre-Merge State** (if no public release)
```bash
git reset --hard <commit-before-merge>
git push origin master --force
```

**Option 3: Forward Fix** (preferred if possible)
```bash
# Create hotfix branch
git checkout -b hotfix/net10-issue
# Fix the issue
git commit -m "Fix: <issue description>"
# Merge hotfix
git checkout master
git merge hotfix/net10-issue
```

---

### Git Workflow Summary

**Upgrade Phase:**
```bash
# Already completed:
git checkout -b upgrade-to-NET10  # ✅ Done
git add .
git commit -m "Pre-upgrade commit"  # ✅ Done

# During upgrade:
# ... make all changes ...
git add .
git commit -m "Upgrade solution to .NET 10"  # Single atomic commit

git push origin upgrade-to-NET10
```

**Review and Merge:**
```bash
# Create pull request (via GitHub/GitLab/etc.)
# ... review process ...
# ... approval ...

# Merge via platform (or manual):
git checkout master
git merge upgrade-to-NET10
git push origin master
```

**Post-Merge:**
```bash
git tag -a v7.0.0-net10 -m "Release: .NET 10 upgrade"
git push origin v7.0.0-net10

# Optional: Delete feature branch
git branch -d upgrade-to-NET10
git push origin --delete upgrade-to-NET10
```

---

### Commit Best Practices

**DO:**
- ✅ Write clear, descriptive commit messages
- ✅ Include issue/ticket references
- ✅ Document breaking changes in commit body
- ✅ Keep commits atomic (single logical change)
- ✅ Use conventional commit format (feat:, fix:, chore:)

**DON'T:**
- ❌ Commit unrelated changes in upgrade commit
- ❌ Use vague messages ("updates", "fixes")
- ❌ Commit broken builds
- ❌ Include merge commits in feature branch (rebase if needed)
- ❌ Force push to shared branches

---

### Branch Protection (Recommended)

**master branch protection settings:**
- Require pull request reviews (minimum 1)
- Require status checks to pass (CI/CD builds)
- Require branches to be up to date before merging
- Restrict who can push to master
- Enable deletion protection

This ensures quality and prevents accidental direct commits to master.

## Success Criteria

### Technical Criteria

**All criteria must be met before the upgrade is considered complete.**

#### 1. Framework Migration

- [ ] **All 8 WPF projects migrated to .NET 10**
  - ExampleCalculatorApp.csproj: `net10.0-windows`
  - ExampleCodeGenApp.csproj: `net10.0-windows`
  - ExampleShaderEditorApp.csproj: `net10.0-windows`
  - StressTest.csproj: `net10.0-windows`
  - NodeNetworkTests.csproj: `net10.0-windows`
  - CalculatorTests.csproj: `net10.0-windows`

- [ ] **Multi-targeting libraries updated**
  - NodeNetwork.csproj: `net10.0-windows;net8.0;net472`
  - NodeNetworkToolkit.csproj: `net10.0-windows;net8.0;net472`

- [ ] **Blazor projects remain on .NET 10**
  - NodeNetwork.Blazor.csproj: `net10.0` ✅
  - NodeNetworkToolkit.Blazor.csproj: `net10.0` ✅

#### 2. Package Updates

- [ ] **Critical package changes applied**
  - ReactiveUI.WPF downgraded: 20.4.1 → 16.4.15 (NodeNetwork only)
  - ReactiveUI.Events.WPF removed (NodeNetwork only)
  - System.Collections.Immutable updated: 9.0.7 → 10.0.3 (all applicable projects)

- [ ] **Framework-included packages handled correctly**
  - Removed for net10.0-windows and net8.0 targets (7 packages)
  - Conditional references for net472 in multi-targeting projects
  - No package conflict warnings

- [ ] **All compatible packages unchanged**
  - 18 compatible packages remain at current versions
  - No unnecessary package updates

#### 3. Build Success

- [ ] **Entire solution builds without errors**
  ```bash
  dotnet build NodeNetwork.sln -c Release
  # Exit code: 0
  # Errors: 0
  ```

- [ ] **Multi-targeting builds succeed**
  - NodeNetwork.csproj builds for net10.0-windows ✅
  - NodeNetwork.csproj builds for net8.0 ✅
  - NodeNetwork.csproj builds for net472 ✅
  - NodeNetworkToolkit.csproj builds for net10.0-windows ✅
  - NodeNetworkToolkit.csproj builds for net8.0 ✅
  - NodeNetworkToolkit.csproj builds for net472 ✅

- [ ] **No build warnings related to upgrade**
  - No package version conflicts
  - No framework compatibility warnings
  - No type ambiguity warnings
  - No obsolete API warnings from upgrade

#### 4. Test Success

- [ ] **All automated tests pass**
  - NodeNetworkTests: 100% pass rate
  - CalculatorTests: 100% pass rate
  - Total test count matches .NET 6 baseline
  - No tests skipped or ignored

- [ ] **Test execution successful**
  ```bash
  dotnet test NodeNetwork.sln -c Release
  # Result: All tests passed
  # Failed: 0
  # Skipped: 0
  ```

#### 5. Functional Validation

- [ ] **ExampleCalculatorApp functional**
  - Application launches without errors
  - Calculator nodes create and function correctly
  - Node connections work
  - Results calculate accurately
  - UI responsive and renders correctly

- [ ] **ExampleCodeGenApp functional**
  - Application launches without errors
  - Code generation nodes work
  - MoonSharp scripting engine functional
  - Generated code compiles
  - Templates process correctly

- [ ] **ExampleShaderEditorApp functional**
  - Application launches without errors
  - OpenGL control renders
  - Shader compilation works
  - Real-time preview functional
  - No rendering artifacts
  - Mathematical nodes accurate (MathNet)

- [ ] **StressTest functional**
  - Application runs to completion
  - No crashes under load
  - Performance acceptable (see Performance Criteria)

#### 6. Breaking Changes Addressed

- [ ] **3,087 compatibility issues resolved**
  - Binary incompatibility: Resolved through recompilation
  - Source incompatibility: Code changes applied (25 issues)
  - Behavioral changes: Tested and validated (67 issues)

- [ ] **ReactiveUI.WPF downgrade validated**
  - All reactive commands functional
  - Reactive property bindings work
  - Observable subscriptions work
  - View activation/deactivation works

- [ ] **ReactiveUI.Events.WPF removal validated**
  - No compilation errors from missing package
  - Alternative event handling implemented and working

- [ ] **Framework-included package removal validated**
  - No type ambiguity errors
  - All System.* types resolve correctly
  - Functionality using these types unchanged

---

### Quality Criteria

#### 1. Code Quality

- [ ] **No code quality regressions**
  - Code analysis passes (if enabled)
  - No new StyleCop violations
  - No new code smell warnings

- [ ] **Reactive patterns preserved**
  - ReactiveUI patterns work correctly
  - No broken reactive bindings
  - Command execution functional

#### 2. Visual Quality

- [ ] **No visual regressions**
  - WPF window layouts unchanged
  - Control rendering identical to .NET 6
  - Font rendering consistent
  - Colors and themes preserved
  - Icons and images display correctly
  - No layout calculation errors

- [ ] **UI responsiveness maintained**
  - No UI freezing
  - Smooth animations
  - Drag-and-drop works
  - Responsive to user input

#### 3. Documentation Quality

- [ ] **Plan documentation complete**
  - plan.md accurately reflects implementation
  - All projects documented
  - All package changes documented
  - Breaking changes cataloged

- [ ] **Assessment documentation accurate**
  - assessment.md reflects discovered issues
  - All compatibility issues documented

---

### Performance Criteria

#### 1. Performance Benchmarks

- [ ] **Performance within acceptable range**
  - ±10% of .NET 6 baseline acceptable
  - No performance regressions exceeding 10%
  - Improvements from .NET 10 welcomed

#### 2. StressTest Results

- [ ] **Stress test metrics**
  - Peak memory usage ≤ .NET 6 baseline + 10%
  - Average CPU usage ≤ .NET 6 baseline + 10%
  - Large graph creation time ≤ .NET 6 baseline + 10%
  - UI frame rate ≥ 30 FPS under load

#### 3. Startup Performance

- [ ] **Application startup**
  - ExampleCalculatorApp startup ≤ .NET 6 + 10%
  - ExampleCodeGenApp startup ≤ .NET 6 + 10%
  - ExampleShaderEditorApp startup ≤ .NET 6 + 10%

#### 4. Memory Management

- [ ] **No memory leaks**
  - Stress test completes without memory leaks
  - Long-running applications stable
  - Memory usage returns to baseline after operations

---

### Process Criteria

#### 1. All-At-Once Strategy Adherence

- [ ] **All projects updated simultaneously**
  - Single atomic upgrade operation
  - No intermediate partially-upgraded state
  - All projects on consistent framework versions

- [ ] **All-At-Once principles followed**
  - Single coordinated batch of updates
  - Unified upgrade phase
  - Comprehensive testing after upgrade

#### 2. Source Control

- [ ] **Upgrade branch created and used**
  - Feature branch: `upgrade-to-NET10` ✅
  - All changes on feature branch
  - Pre-upgrade commit created ✅

- [ ] **Commit strategy followed**
  - Single atomic commit (preferred) OR
  - Milestone commits with clear boundaries
  - Commit messages descriptive

#### 3. Testing Strategy Executed

- [ ] **Multi-level testing completed**
  - Level 1: Compilation validation ✅
  - Level 2: Automated test execution ✅
  - Level 3: Manual application testing ✅
  - Level 4: Performance & stress testing ✅
  - Level 5: Multi-targeting validation ✅

---

### Acceptance Criteria

**The upgrade is accepted when ALL of the following are true:**

#### Critical Acceptance

1. ✅ **All 8 projects build successfully for net10.0-windows**
2. ✅ **Multi-targeting libraries build for net10.0-windows, net8.0, and net472**
3. ✅ **All automated tests pass (100% pass rate)**
4. ✅ **ReactiveUI.WPF 16.4.15 functional (no blocking issues)**
5. ✅ **No framework-included package conflicts**

#### Essential Acceptance

6. ✅ **All 3 example applications launch and function correctly**
7. ✅ **StressTest completes without crashes**
8. ✅ **No visual regressions observed**
9. ✅ **Performance within ±10% of .NET 6 baseline**
10. ✅ **All breaking changes addressed and validated**

#### Quality Acceptance

11. ✅ **No build warnings related to upgrade**
12. ✅ **No memory leaks detected**
13. ✅ **plan.md and assessment.md complete and accurate**
14. ✅ **Source control strategy followed**

---

### Sign-Off Checklist

**Final verification before merge:**

- [ ] **Developer Sign-Off**
  - All technical criteria met
  - All quality criteria met
  - All performance criteria met
  - Ready for code review

- [ ] **Code Review Sign-Off** (if applicable)
  - Code changes reviewed
  - Breaking changes acceptable
  - Testing sufficient
  - Documentation adequate

- [ ] **Testing Sign-Off** (if separate QA)
  - All test scenarios executed
  - No critical defects
  - Performance validated
  - Visual quality confirmed

- [ ] **Project Owner Sign-Off**
  - Upgrade objectives met
  - Acceptable risk level
  - Ready for merge to master

---

### Completion Confirmation

**The upgrade is COMPLETE when:**

✅ All Technical Criteria met (6 sections, all checkboxes)
✅ All Quality Criteria met (3 sections, all checkboxes)
✅ All Performance Criteria met (4 sections, all checkboxes)
✅ All Process Criteria met (3 sections, all checkboxes)
✅ All Acceptance Criteria met (14 items)
✅ All Sign-Offs obtained (4 stakeholders)

**Next Step After Completion:**
- Create pull request to merge `upgrade-to-NET10` → `master`
- Follow review and merge process (see Source Control Strategy)
- Tag release: `v7.0.0-net10`
- Update public documentation
- Publish updated NuGet packages (if applicable)
