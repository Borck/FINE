# NodeNetwork.Blazor

`NodeNetwork.Blazor` is a .NET 10 component library that provides a Blazor-native node editor inspired by the original WPF `NodeNetwork` API model.

## Goals

- Keep core concepts compatible (`Node`, `Port`, `Connection`, and graph document contract).
- Provide a modern Blazor editor with pan, zoom, drag, selection, collapse, and validated connections.
- Enable easy switching between WPF and Blazor using a shared document format (`NodeNetworkDocument`).

## Compatibility Strategy

Both implementations can interchange data via `NodeNetwork.Blazor.Compatibility.NodeNetworkDocument`.

- WPF side: map `NetworkViewModel` and endpoint VMs to `NodeNetworkDocument`.
- Blazor side: use `INodeNetworkCompatibilityAdapter` (`DefaultNodeNetworkCompatibilityAdapter`) to import/export.

This allows UI implementation switching without changing domain graph data.
