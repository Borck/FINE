# NodeNetwork .NET Multi-Target Upgrade Tasks

## Overview

This document tracks the execution of the NodeNetwork solution upgrade so that all relevant projects use only the following multi-target frameworks: `net6.0;net8.0;net10.0`.

**Progress**: 3/4 tasks complete (75%) ![0%](https://progress-bar.xyz/75)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-02-27 12:11)*
**References**: Plan §Phase 0: Preparation

- [✓] (1) Verify .NET 10 SDK installed
- [✓] (2) .NET 10 SDK meets minimum requirements (**Verify**)
- [✓] (3) Verify .NET 8 SDK installed
- [✓] (4) .NET 8 SDK meets minimum requirements (**Verify**)
- [✓] (5) Verify .NET 6 SDK installed
- [✓] (6) .NET 6 SDK meets minimum requirements (**Verify**)

---

### [✓] TASK-002: Atomic framework and dependency upgrade with compilation fixes *(Completed: 2026-02-27 13:04)*
**References**: Plan §Phase 1: Atomic Upgrade, Plan §Project-by-Project Migration Plans, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✓] (1) Update `TargetFramework`/`TargetFrameworks` in all 8 project files to use only `net6.0;net8.0;net10.0`
- [✓] (2) All project `TargetFramework` properties updated to only `net6.0;net8.0;net10.0` (**Verify**)
- [✓] (3) Update package references across all projects per Plan §Package Update Reference (key changes: ReactiveUI.WPF 20.4.1 → 16.4.15, System.Collections.Immutable 9.0.7 → 10.0.3)
- [✓] (4) Remove ReactiveUI.Events.WPF package from `NodeNetwork.csproj` per Plan §Package Update Reference
- [✓] (5) Remove references to framework-included packages that are no longer needed for `net6.0`, `net8.0`, and `net10.0`
- [✓] (6) All package references updated correctly (**Verify**)
- [✓] (7) Restore all dependencies for entire solution
- [✓] (8) All dependencies restored successfully (**Verify**)
- [✓] (9) Build entire solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus areas: WPF API changes, ReactiveUI.WPF downgrade compatibility, ReactiveUI.Events.WPF removal, source incompatibilities)
- [✓] (10) Solution builds with 0 errors (**Verify**)

---

### [✓] TASK-003: Run full test suite and validate upgrade *(Completed: 2026-02-27 13:41)*
**References**: Plan §Phase 2: Testing & Validation, Plan §Testing & Validation Strategy

- [✓] (1) Run tests in `NodeNetworkTests` project
- [✓] (2) Fix any test failures (reference Plan §Breaking Changes for common issues)
- [✓] (3) Run tests in `CalculatorTests` project
- [✓] (4) Fix any test failures (reference Plan §Breaking Changes for common issues)
- [✓] (5) Re-run all tests after fixes
- [✓] (6) All tests pass with 0 failures (**Verify**)

---

### [▶] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [▶] (1) Commit all changes with message: "Upgrade solution to .NET multi-targeting

- Update all 8 WPF projects to multi-target net6.0;net8.0;net10.0
- Downgrade ReactiveUI.WPF from 20.4.1 to 16.4.15 for compatibility
- Remove deprecated ReactiveUI.Events.WPF package
- Update System.Collections.Immutable to 10.0.3 across projects
- Remove unnecessary framework-included packages
- Fix all compilation errors from breaking changes
- Verify all tests pass (NodeNetworkTests, CalculatorTests)
- Validate multi-target builds (net6.0, net8.0, net10.0)"

---







