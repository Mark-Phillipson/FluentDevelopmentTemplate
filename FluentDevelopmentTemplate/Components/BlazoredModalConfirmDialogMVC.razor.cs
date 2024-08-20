using Microsoft.AspNetCore.Components;

namespace FluentDevelopmentTemplate.Components
{
    public partial class BlazoredModalConfirmDialogMVC : ComponentBase
    {
        [Parameter] public EventCallback<bool> Confirm { get; set; }
        [Parameter] public string Title { get; set; } = "Please Confirm";
        [Parameter] public string? Message { get; set; }
        [Parameter] public string ButtonColour { get; set; } = "danger";
        [Parameter] public string? Icon { get; set; } = "fas fa-question";

        ElementReference CancelButton;
        private async Task OnCancel()
        {
            await Confirm.InvokeAsync(false);
        }
        private async Task OnConfirm()
        {
            await Confirm.InvokeAsync(true);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await CancelButton.FocusAsync();
            }
        }

    }
}