
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace InputRadioGroupEnumExample.Shared
{
    public sealed class InputRadioGroup<TEnum> : InputBase<TEnum>
    {
        private readonly Dictionary<string, bool> CheckStatuses = new Dictionary<string, bool>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenElement(0, "div");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));
            
            // Add an option element per enum value
            var enumType = GetEnumType();
            foreach (TEnum value in Enum.GetValues(enumType))
            {
                CheckStatuses.TryAdd(value.ToString(), value.ToString() == CurrentValueAsString);
                builder.OpenElement(4, "div");

                builder.OpenElement(5, "input");
                builder.AddAttribute(6, "type", "radio");
                builder.AddAttribute(7, "id", value.ToString());
                builder.AddAttribute(8, "checked", CheckStatuses[value.ToString()]);
                builder.AddAttribute(9, "onchange", EventCallback.Factory.Create(this, HandleValueChange(value.ToString())));
                builder.CloseElement();

                builder.OpenElement(10, "label");
                builder.AddAttribute(11, "for", value.ToString());
                builder.AddContent(12, GetDisplayName(value));
                builder.CloseElement();

                builder.CloseElement();
            }

            builder.CloseElement();
        }

        EventCallback<ChangeEventArgs> HandleValueChange(string currentRadioName) {
            var enumType = GetEnumType();
            foreach (TEnum value in Enum.GetValues(enumType))
            {
                CheckStatuses[value.ToString()] = value.ToString() == CurrentValueAsString;
            }

            return EventCallback.Factory.CreateBinder<string>(this, _ => CurrentValueAsString = currentRadioName, currentRadioName);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        protected override bool TryParseValueFromString(string value, out TEnum result, out string validationErrorMessage)
        {
            if (BindConverter.TryConvertTo(value, CultureInfo.CurrentCulture, out TEnum parsedValue))
            {
                result = parsedValue;
                validationErrorMessage = null;
                return true;
            }

            // Map null/empty value to null if the bound object is nullable
            if (string.IsNullOrEmpty(value))
            {
                var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));
                if (nullableType != null)
                {
                    result = default;
                    validationErrorMessage = null;
                    return true;
                }
            }

            // The value is invalid => set the error message
            result = default;
            validationErrorMessage = $"The {FieldIdentifier.FieldName} field is not valid.";
            return false;
        }

        private string GetDisplayName(TEnum value)
        {
            // Read the Display attribute name
            var member = value.GetType().GetMember(value.ToString())[0];
            var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
                return displayAttribute.GetName();

            return value.ToString();
        }

        private Type GetEnumType()
        {
            var nullableType = Nullable.GetUnderlyingType(typeof(TEnum));
            if (nullableType != null)
                return nullableType;

            return typeof(TEnum);
        }
    }
}
