﻿using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for elements that typically render a html input element.
/// </summary>
public abstract class TextBoxBase<TValue> : FieldBase<TValue>
{
    /// <summary>
    /// Gets or sets the notification strategy for when the value of the input changes.
    /// </summary>
    [Parameter]
    public UpdateTrigger UpdateTrigger { get; set; }

    /// <summary>
    /// Gets or sets a callback that will be invoked when a key is pressed down while the input has focus.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Gets or sets a callback that will be invoked when the 'Enter' key is pressed while the input has focus.
    /// </summary>
    [Parameter]
    public EventCallback OnReturn { get; set; }

    /// <summary>
    /// Gets the type of value that the input accepts.
    /// </summary>
    protected virtual string InputType => "text";

    /// <summary>
    /// Implemented by inheriting classes to convert the provided value to the input type.
    /// </summary>
    protected abstract TValue? GetValue(object? value);

    /// <summary>
    /// Build the actual input element.
    /// </summary>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "input");
        {
            builder.AddAttribute(2, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(3, "type", InputType);
            builder.AddAttribute(4, "value", GetInputDisplayValue(Value));
            builder.AddAttribute(5, "class", ClassBuilder.Join(ClassNameProvider.Field_Input, InputClass));
            builder.AddAttribute(6, "name", AppliedName);
            var valueUpdateCallback = EventCallback.Factory.CreateBinder(this, RuntimeHelpers.CreateInferredBindSetter(callback: HandleValueUpdated, value: Value), Value);

            builder.AddAttribute(7, UpdateTrigger.ToHtmlAttributeValue(), valueUpdateCallback);
            builder.SetUpdatesAttributeName("value");

            builder.AddAttribute(8, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, KeyDown));

            if (Disabled)
            {
                builder.AddAttribute(9, "disabled");
            }

            if (ReadOnly)
            {
                builder.AddAttribute(10, "readonly");
            }
        }
        builder.CloseElement();
    }

    private async Task HandleValueUpdated(TValue? value)
    {
        ChangeEventArgs e = new()
        {
            Value = value
        };

        await HandleInputChange(e);
    }

    private async Task KeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Code == "NumpadEnter")
        {
            await OnReturn.InvokeAsync();
        }
        await OnKeyDown.InvokeAsync(e);
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder builder)
    {
        base.BuildInputClass(builder);
        builder.Add(ClassNameProvider.TextBox);
    }

    /// <summary>
    /// Bound to input element to handle value changes.
    /// </summary>
    protected async Task HandleInputChange(ChangeEventArgs e)
    {
        var newValue = GetValue(e.Value);
        await NotifyValueChange(newValue);
    }

    /// <summary>
    /// Gets the value to display for the input value.
    /// </summary>
    protected virtual object? GetInputDisplayValue(TValue? value)
    {
        return BindConverter.FormatValue(value);
    }
}
