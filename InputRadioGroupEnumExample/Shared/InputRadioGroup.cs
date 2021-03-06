
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
    public sealed class InputRadioGroup<TString> : InputBase<TString>
    {
        [Parameter] public string[] Options { get; set; }
        private readonly Dictionary<string, bool> CheckStatuses = new Dictionary<string, bool>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);
            builder.OpenElement(0, "div");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "class", CssClass);
            builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));

            // Add an option element per enum value
            foreach (string value in Options)
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
                builder.AddContent(12, value.ToString());
                builder.CloseElement();

                builder.CloseElement();
            }

            builder.CloseElement();
        }

        EventCallback<ChangeEventArgs> HandleValueChange(string currentRadioName) {
            foreach (string value in Options)
            {
                CheckStatuses[value.ToString()] = value.ToString() == CurrentValueAsString;
            }

            return EventCallback.Factory.CreateBinder<string>(this, _ => CurrentValueAsString = currentRadioName, currentRadioName);
        }
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        protected override bool TryParseValueFromString(string value, out TString result, out string validationErrorMessage)
        {
            if (BindConverter.TryConvertTo(value, CultureInfo.CurrentCulture, out TString parsedValue))
            {
                result = parsedValue;
                validationErrorMessage = null;
                return true;
            }

            // Map null/empty value to null if the bound object is nullable
            if (string.IsNullOrEmpty(value))
            {
                var nullableType = Nullable.GetUnderlyingType(typeof(TString));
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
    }
}
