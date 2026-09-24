# Testing Strategy

GUI engineering has two separate correctness problems:

1. model correctness — did we describe the intended GUI correctly?
2. manifestation correctness — did a platform render that intent correctly?

They should not be tested as one problem.

## Core tests

Core tests must run without a UI platform.

They cover:

- recursive composition;
- identity;
- snapshot behavior;
- immutability;
- property isolation;
- coordinate contracts;
- deterministic child ordering;
- invalid input.

These tests are the foundation for every adapter.

## Adapter contract tests

Each adapter should eventually consume a shared set of semantic cases.

For example:

Panel(root) -> Button(save) -> Text(Save)

must preserve:

- hierarchy;
- identity;
- semantic kind;
- text;
- child ordering.

The native representation may differ.

## Platform tests

Platform tests verify target-specific behavior such as:

- WPF dependency-property configuration;
- Blazor render-tree output;
- Unity UI Toolkit element creation;
- accessibility integration;
- native event routing.

## Current state

The first Core test project establishes the pattern. Blazor and WPF adapter tests should be added as their APIs become stable enough to define contract expectations.

Tests are intentionally written against behavior rather than implementation details.
