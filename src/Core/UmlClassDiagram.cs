using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Describes a UML class-diagram projection without depending on a rendering platform.</summary>
public sealed class UmlClassDiagram
{
    public UmlClassDiagram(
        string name,
        string kind = "class",
        string? @namespace = null,
        IEnumerable<UmlMember>? fields = null,
        IEnumerable<UmlMember>? properties = null,
        IEnumerable<UmlMember>? operations = null,
        IEnumerable<string>? declaredRelationships = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("A UML type name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(kind))
            throw new ArgumentException("A UML type kind is required.", nameof(kind));

        Name = name;
        Kind = kind;
        Namespace = @namespace ?? string.Empty;
        Fields = Snapshot(fields);
        Properties = Snapshot(properties);
        Operations = Snapshot(operations);
        DeclaredRelationships = new ReadOnlyCollection<string>((declaredRelationships ?? Enumerable.Empty<string>())
            .Where(value => !string.IsNullOrWhiteSpace(value)).ToList());
        Description = string.IsNullOrWhiteSpace(description)
            ? $"Source-derived UML class view for {name}. Members and declared type references are shown; unresolved references are not asserted as verified relationships."
            : description;
    }

    public string Name { get; }
    public string Kind { get; }
    public string Namespace { get; }
    public IReadOnlyList<UmlMember> Fields { get; }
    public IReadOnlyList<UmlMember> Properties { get; }
    public IReadOnlyList<UmlMember> Operations { get; }
    /// <summary>Declared base-list references, not semantically resolved relationships.</summary>
    public IReadOnlyList<string> DeclaredRelationships { get; }
    public string Description { get; }

    private static IReadOnlyList<UmlMember> Snapshot(IEnumerable<UmlMember>? members) =>
        new ReadOnlyCollection<UmlMember>((members ?? Enumerable.Empty<UmlMember>()).ToList());
}

/// <summary>One source-grounded member signature in a UML class compartment.</summary>
public sealed record UmlMember(string Name, string Signature, string Visibility = "public", bool IsStatic = false);
