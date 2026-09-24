# Platform Adapter Guide

A platform adapter is a compiler-like boundary: it consumes the neutral GUI model and materializes it for a target UI technology.

## Adapter responsibilities

An adapter should:

1. consume Core types;
2. map known semantic node kinds;
3. translate supported properties intentionally;
4. preserve child order;
5. report or handle unsupported capabilities explicitly;
6. own platform resources and event lifetimes;
7. avoid changing domain meaning.

## Do not use blind property passthrough

A renderer should not assume that every Core property is a native attribute.

For example, a future Core property named layout.horizontalAlignment should be translated by each adapter according to its own platform rules. It should not become an HTML attribute merely because HTML permits arbitrary attributes.

The current Blazor renderer contains a deliberately simple compatibility implementation. It should be treated as an adapter prototype, not the final cross-platform contract.

## Capability differences

Platforms are not identical.

An adapter may support more capabilities than another adapter. The correct response is not to contaminate Core with platform-specific APIs.

Instead, capabilities should eventually be:

- declared;
- negotiated;
- degraded predictably; or
- rejected with a useful diagnostic.

## Testing adapters

Adapter tests should have two levels:

### Contract tests

Verify that the same Core tree produces the required semantic result.

### Platform tests

Verify native behavior that cannot be represented by Core alone.

This keeps platform behavior testable without making the Core model platform-aware.
