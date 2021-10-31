# InputRadioGroup

Custom Razor Components InputRadioGroup and InputRadioGroupEnum, based [on this article](https://www.meziantou.net/creating-a-inputselect-component-for-enumerations-in-blazor.htm).

Usage:
1. Copy either InputRadioGroup.cs or InputRadioGroupEnum.cs to your shared folder
2. Include anywhere inside an EditForm block, for example:

```
<EditForm Model="model">
    InputRadioGroupEnum:<br/>
    <InputRadioGroupEnum class="seasons-radio" @bind-Value="model.Season"></InputRadioGroupEnum>
    <br />
    InputRadioGroup:<br/>
    <InputRadioGroup @bind-Value="model.RadioGroupValue" Options="@Options"></InputRadioGroup>
</EditForm>
```

This project is fully open-source code, do whatever you want with it.
