using System.Collections;
using System.Globalization;
using System.Reflection;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Projects arbitrary public object models into the platform-neutral semantic GUI tree.
/// The inspected model has no dependency on GUI.Core.
/// </summary>
public static class ReflectionGuiBuilder
{
    /// <summary>Projects a model instance into a recursive semantic GUI tree using its public properties.</summary>
    /// <param name="model">The model to inspect. Its type does not need to reference GUI.Core.</param>
    /// <returns>A platform-neutral semantic GUI tree.</returns>
    public static GuiNode Create(object model)
    {
        ArgumentNullException.ThrowIfNull(model);
        return CreateValue(model, "root", model.GetType().Name, new HashSet<object>(ReferenceEqualityComparer.Instance)).Build();
    }

    private static GuiBuilder CreateValue(object? value, string id, string label, HashSet<object> ancestors)
    {
        if (value is null)
            return GuiBuilder.Create(GuiKinds.Text, id).Text(label).Property("value", "null");

        var type = value.GetType();

        if (IsScalar(type))
            return CreateScalar(value, id, label, type);

        if (!type.IsValueType && !ancestors.Add(value))
            return GuiBuilder.Create(GuiKinds.Text, id).Text(label).Property("value", "<cycle>");

        try
        {
            if (value is IEnumerable enumerable and not string)
                return CreateCollection(enumerable, id, label, ancestors);

            var objectBuilder = GuiBuilder.Create(GuiKinds.Object, id)
                .Property("label", label)
                .Property("type", type.FullName ?? type.Name);

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanRead || property.GetIndexParameters().Length != 0)
                    continue;

                objectBuilder.Child(CreateValue(
                    property.GetValue(value),
                    $"{id}.{property.Name}",
                    property.Name,
                    ancestors));
            }

            return objectBuilder;
        }
        finally
        {
            if (!type.IsValueType)
                ancestors.Remove(value);
        }
    }

    private static GuiBuilder CreateCollection(IEnumerable values, string id, string label, HashSet<object> ancestors)
    {
        var builder = GuiBuilder.Create(GuiKinds.Object, id)
            .Property("label", label)
            .Property("type", "Collection");

        var index = 0;
        foreach (var item in values)
        {
            builder.Child(CreateValue(item, $"{id}[{index}]", $"[{index}]", ancestors));
            index++;
        }

        return builder.Property("count", index.ToString(CultureInfo.InvariantCulture));
    }

    private static GuiBuilder CreateScalar(object value, string id, string label, Type type)
    {
        var kind = type == typeof(bool)
            ? GuiKinds.Toggle
            : IsInteger(type)
                ? GuiKinds.IntegerSlider
                : IsFloatingPoint(type)
                    ? GuiKinds.FloatSlider
                    : GuiKinds.TextBox;

        return GuiBuilder.Create(kind, id)
            .Property("label", label)
            .Property("type", type.FullName ?? type.Name)
            .Property("value", Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
    }

    private static bool IsScalar(Type type) =>
        type == typeof(string) || type == typeof(bool) || type.IsEnum ||
        type.IsPrimitive || type == typeof(decimal) || type == typeof(Guid) ||
        type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);

    private static bool IsInteger(Type type) =>
        type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
        type == typeof(ushort) || type == typeof(int) || type == typeof(uint) ||
        type == typeof(long) || type == typeof(ulong);

    private static bool IsFloatingPoint(Type type) =>
        type == typeof(float) || type == typeof(double) || type == typeof(decimal);
}
