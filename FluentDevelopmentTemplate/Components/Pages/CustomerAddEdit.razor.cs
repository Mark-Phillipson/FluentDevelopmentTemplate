
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using System.Security.Claims;
using FluentDevelopmentTemplate.DTOs;
using FluentDevelopmentTemplate.Services;
using FluentDevelopmentTemplate.Models;

namespace FluentDevelopmentTemplate.Components.Pages;

public partial class CustomerAddEdit : ComponentBase
{
    [Parameter] public EventCallback<bool> CloseModal { get; set; }
    [Parameter] public string? Title { get; set; }
    [Inject] public ILogger<CustomerAddEdit>? Logger { get; set; }
    [Inject] public IJSRuntime? JSRuntime { get; set; }
    [Parameter] public int? Id { get; set; }
    public CustomerDTO CustomerDTO { get; set; } = new CustomerDTO();//{ };
    [Inject] public ICustomerDataService? CustomerDataService { get; set; }
    [Inject] public ApplicationState? ApplicationState { get; set; }
    [Parameter] public int ParentId { get; set; }
    ElementReference FirstInput;
#pragma warning disable 414, 649
    bool TaskRunning = false;
#pragma warning restore 414, 649
    protected override async Task OnInitializedAsync()
    {
        if (CustomerDataService == null)
        {
            return;
        }
        if (Id != null && Id != 0)
        {
            var result = await CustomerDataService.GetCustomerById((int)Id);
            if (result != null)
            {
                CustomerDTO = result;
            }
        }
        else
        {
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await Task.Delay(100);
                await FirstInput.FocusAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
    public async Task CloseAsync()
    {
        await CloseModal.InvokeAsync(true);
    }
    protected async Task HandleValidSubmit()
    {
        if (ApplicationState == null)
        {
            return;
        }
        TaskRunning = true;
        if ((Id == 0 || Id == null) && CustomerDataService != null)
        {
            CustomerDTO? result = await CustomerDataService.AddCustomer(CustomerDTO);
            if (result == null && Logger != null)
            {
                Logger.LogError("Customer failed to add, please investigate Error Adding New Customer");
                ApplicationState.Message = "Customer failed to add, please investigate Error Adding New Customer";
                ApplicationState.MessageType = "danger";
                return;
            }
            ApplicationState.Message = "Customer Added successfully";
            ApplicationState.MessageType = "success";
        }
        else
        {
            if (CustomerDataService != null)
            {
                await CustomerDataService!.UpdateCustomer(CustomerDTO, "");
                ApplicationState.Message = "The Customer updated successfully";
                ApplicationState.MessageType = "success";
            }
        }
        await CloseModal.InvokeAsync(true);
        TaskRunning = false;
    }
}