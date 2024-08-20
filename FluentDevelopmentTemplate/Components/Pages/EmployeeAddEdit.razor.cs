
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

namespace FluentDevelopmentTemplate.Components.Pages
{
    public partial class EmployeeAddEdit : ComponentBase
    {
        [Parameter] public EventCallback<bool> CloseModal { get; set; }
        [Parameter] public string? Title { get; set; }
        [Inject] public ILogger<EmployeeAddEdit>? Logger { get; set; }
        [Inject] public IJSRuntime? JSRuntime { get; set; }
        [Parameter] public int? Id { get; set; }
        public EmployeeDTO EmployeeDTO { get; set; } = new EmployeeDTO() { PhotoPath = "images/hoodie.jpeg" };
        [Inject] public IEmployeeDataService? EmployeeDataService { get; set; }
        [Inject] public ApplicationState? ApplicationState { get; set; }
        [Parameter] public int ParentId { get; set; }
        ElementReference FirstInput;
#pragma warning disable 414, 649
        bool TaskRunning = false;
#pragma warning restore 414, 649
        protected override async Task OnInitializedAsync()
        {
            if (EmployeeDataService == null)
            {
                return;
            }
            if (Id != null && Id != 0)
            {
                var result = await EmployeeDataService.GetEmployeeById((int)Id);
                if (result != null)
                {
                    EmployeeDTO = result;
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
            if ((Id == 0 || Id == null) && EmployeeDataService != null)
            {
                EmployeeDTO? result = await EmployeeDataService.AddEmployee(EmployeeDTO);
                if (result == null && Logger != null)
                {
                    Logger.LogError("Employee failed to add, please investigate Error Adding New Employee");
                    ApplicationState.Message = "Employee failed to add, please investigate Error Adding New Employee";
                    ApplicationState.MessageType = "danger";
                    return;
                }
                ApplicationState.Message = "Employee Added successfully";
                ApplicationState.MessageType = "success";
            }
            else
            {
                if (EmployeeDataService != null)
                {
                    await EmployeeDataService!.UpdateEmployee(EmployeeDTO, "");
                    ApplicationState.Message = "The Employee updated successfully";
                    ApplicationState.MessageType = "success";
                }
            }
            await CloseModal.InvokeAsync(true);
            TaskRunning = false;
        }
    }
}