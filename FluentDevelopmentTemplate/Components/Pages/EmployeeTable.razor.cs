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
using Ardalis.GuardClauses;
using FluentDevelopmentTemplate.Services;
using FluentDevelopmentTemplate.Models;
using FluentDevelopmentTemplate.DTOs;

namespace FluentDevelopmentTemplate.Components.Pages
{
    public partial class EmployeeTable : ComponentBase
    {
        [Inject] public required IEmployeeDataService EmployeeDataService { get; set; }
        [Inject] public required NavigationManager NavigationManager { get; set; }
        [Inject] public ILogger<EmployeeTable>? Logger { get; set; }

        [Inject] public required ApplicationState ApplicationState { get; set; }
        public string Title { get; set; } = "Employee Items (Employees)";
        public string EditTitle { get; set; } = "Edit Employee Item (Employees)";
        [Parameter] public int ParentId { get; set; }
        public List<EmployeeDTO>? EmployeeDTO { get; set; }
        public List<EmployeeDTO>? FilteredEmployeeDTO { get; set; }
        protected EmployeeAddEdit? EmployeeAddEdit { get; set; }
        ElementReference SearchInput;
#pragma warning disable 414, 649
        private bool _loadFailed = false;
        private string? searchTerm = null;
#pragma warning restore 414, 649
        public string? SearchTerm { get => searchTerm; set { searchTerm = value; } }
        private string? clientSearchedTerm { get; set; }
        public string? ClientSearchTerm { get => clientSearchedTerm; set { clientSearchedTerm = value; ApplyLocalFilter(); } }
        private bool _serverPaging = true;
        private void ApplyLocalFilter()
        {
            if (FilteredEmployeeDTO == null || EmployeeDTO == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(ClientSearchTerm))
            {
                FilteredEmployeeDTO = EmployeeDTO;
            }
            else
            {
                FilteredEmployeeDTO = EmployeeDTO.Where(v => v.Name?.Contains(ClientSearchTerm, StringComparison.OrdinalIgnoreCase) == true
                || v.Department?.Contains(ClientSearchTerm, StringComparison.OrdinalIgnoreCase) == true
                || v.Email?.Contains(ClientSearchTerm, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }
            Title = $"Employee> ({FilteredEmployeeDTO.Count})";
        }

        private string? lastSearchTerm { get; set; }

        [Parameter] public string? ServerSearchTerm { get; set; }
        public string ExceptionMessage { get; set; } = String.Empty;
        public List<string>? PropertyInfo { get; set; }
        [CascadingParameter] public ClaimsPrincipal? User { get; set; }
        [Inject] public IJSRuntime? JSRuntime { get; set; }
        public bool ShowEdit { get; set; } = false;
        private bool ShowDeleteConfirm { get; set; }
        private int pageNumber = 1;
        private int pageSize = 15;
        private int totalRows = 0;

        private int EmployeeId { get; set; }
        private EmployeeDTO? currentEmployee { get; set; }

        private string? message;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                if (EmployeeDataService != null)
                {
                    ServerSearchTerm = SearchTerm;
                    totalRows = await EmployeeDataService.GetTotalCount();
                    var result = await EmployeeDataService!.GetAllEmployeesAsync(pageNumber, pageSize, ServerSearchTerm);
                    if (result != null)
                    {
                        EmployeeDTO = result.ToList();
                        FilteredEmployeeDTO = result.ToList();
                        StateHasChanged();
                    }
                }
            }
            catch (Exception e)
            {
                Logger?.LogError("e, Exception occurred in LoadData Method, Getting Records from the Service");
                _loadFailed = true;
                ExceptionMessage = e.Message;
            }
            FilteredEmployeeDTO = EmployeeDTO;
            Title = $"Employee ({FilteredEmployeeDTO?.Count})";

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await Task.Delay(100);
                    await SearchInput.FocusAsync();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        private void AddNewEmployee()
        {
            EditTitle = "Add Employee";
            EmployeeId = 0;
            ShowEdit = true;
        }

        private async Task ApplyFilter()
        {
            if (FilteredEmployeeDTO == null || EmployeeDTO == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchTerm))
            {
                await LoadData();
                Title = $"All Employee ({FilteredEmployeeDTO.Count})";
            }
            else
            {
                if (lastSearchTerm != SearchTerm)
                {
                    await LoadData();
                }

            }
            lastSearchTerm = SearchTerm;
        }
        protected void SortEmployee(string sortColumn)
        {
            Guard.Against.Null(sortColumn, nameof(sortColumn));
            if (FilteredEmployeeDTO == null)
            {
                return;
            }
            if (sortColumn == "Name")
            {
                FilteredEmployeeDTO = FilteredEmployeeDTO.OrderBy(v => v.Name).ToList();
            }
            else if (sortColumn == "Name Desc")
            {
                FilteredEmployeeDTO = FilteredEmployeeDTO.OrderByDescending(v => v.Name).ToList();
            }
            if (sortColumn == "Department")
            {
                FilteredEmployeeDTO = FilteredEmployeeDTO.OrderBy(v => v.Department).ToList();
            }
            else if (sortColumn == "Department Desc")
            {
                FilteredEmployeeDTO = FilteredEmployeeDTO.OrderByDescending(v => v.Department).ToList();
            }
            if (sortColumn == "Email")
            {
                FilteredEmployeeDTO = FilteredEmployeeDTO.OrderBy(v => v.Email).ToList();
            }
            else if (sortColumn == "Email Desc")
            {
                FilteredEmployeeDTO = FilteredEmployeeDTO.OrderByDescending(v => v.Email).ToList();
            }
        }


        private void DeleteEmployee(int Id)
        {
            EmployeeId = Id;
            currentEmployee = FilteredEmployeeDTO?.FirstOrDefault(v => v.Id == Id);
            message = $"Are you sure you want to delete {currentEmployee?.Name} Employee item?";
            ShowDeleteConfirm = true;
        }


        private void HideMessage()
        {
            if (ApplicationState != null)
            {
                ApplicationState.Message = null;
            }
        }
        private void EditEmployee(int Id)
        {
            EmployeeId = Id;
            EditTitle = "Edit Employee";
            ShowEdit = true;
        }
        private void ToggleModal()
        {
            ShowEdit = !ShowEdit;
        }
        private void ToggleShowDeleteConfirm()
        {
            ShowDeleteConfirm = !ShowDeleteConfirm;
        }
        public async Task CloseModalAsync(bool close)
        {
            if (close)
            {
                ShowEdit = false;
                await LoadData();
            }
        }
        private async void CloseConfirmDeletion(bool confirmation)
        {
            ShowDeleteConfirm = false;
            if (EmployeeDataService == null) return;
            if (confirmation)
            {
                await EmployeeDataService.DeleteEmployee(EmployeeId);
                if (ApplicationState != null)
                {
                    ApplicationState.Message = $"{EmployeeId} Employee item has been deleted successfully";
                    ApplicationState.MessageType = "success";
                }
                await LoadData();
                StateHasChanged();
            }
        }

        private async Task OnValueChangedPageSize(int value)
        {
            pageSize = value;
            pageNumber = 1;
            await LoadData();
        }
        private async Task PageDown(bool goBeginning)
        {
            if (goBeginning || pageNumber <= 0)
            {
                pageNumber = 1;
            }
            else
            {
                pageNumber--;
            }
            await LoadData();
        }
        private async Task PageUp(bool goEnd)
        {
            int maximumPages = (int)Math.Ceiling((decimal)totalRows / pageSize);
            if (goEnd || pageNumber >= maximumPages)
            {
                pageNumber = maximumPages;
            }
            else
            {
                pageNumber++;
            }
            await LoadData();
        }

    }
}