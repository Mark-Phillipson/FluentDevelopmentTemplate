
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

namespace FluentDevelopmentTemplate.Components.Pages;
public partial class CustomerTable : ComponentBase
{
    [Inject] public required ICustomerDataService CustomerDataService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public ILogger<CustomerTable>? Logger { get; set; }

    [Inject] public required ApplicationState ApplicationState { get; set; }
    public string Title { get; set; } = "Customer Items (Customers)";
    public string EditTitle { get; set; } = "Edit Customer Item (Customers)";
    [Parameter] public int ParentId { get; set; }
    public List<CustomerDTO>? CustomerDTO { get; set; }
    public List<CustomerDTO>? FilteredCustomerDTO { get; set; }
    protected CustomerAddEdit? CustomerAddEdit { get; set; }
    ElementReference SearchInput;
#pragma warning disable 414, 649
    private bool _loadFailed = false;
    private string? searchTerm = null;
#pragma warning restore 414, 649
    public string? SearchTerm { get => searchTerm; set { searchTerm = value; } }
    private string? clientSearchedTerm { get; set; }
    public string? ClientSearchTerm { get => clientSearchedTerm; set { clientSearchedTerm = value; ApplyLocalFilter(); } }
    private bool _serverPaging = false;
    private void ApplyLocalFilter()
    {
        if (FilteredCustomerDTO == null || CustomerDTO == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(ClientSearchTerm))
        {
            FilteredCustomerDTO = CustomerDTO;
        }
        else
        {
            FilteredCustomerDTO = CustomerDTO.Where(v =>
                (v.Name != null && v.Name.ToLower().Contains(ClientSearchTerm))

            ).ToList();
        }
        Title = $"Customer ({FilteredCustomerDTO.Count})";
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
    private int pageSize = 1000;
    private int totalRows = 0;

    private int CustomerId { get; set; }
    private CustomerDTO? currentCustomer { get; set; }
    private string? Message { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }
    public IQueryable<CustomerDTO>? QueryResult { get; set; }
    private async Task LoadData()
    {
        try
        {
            if (CustomerDataService != null)
            {
                ServerSearchTerm = SearchTerm;
                totalRows = await CustomerDataService.GetTotalCount();
                var result = await CustomerDataService!.GetAllCustomersAsync
               (pageNumber, pageSize, ServerSearchTerm);
                //var result = await CustomerDataService.SearchCustomersAsync(ServerSearchTerm);
                if (result != null)
                {
                    CustomerDTO = result.ToList();
                    QueryResult = result.AsQueryable();
                    FilteredCustomerDTO = result.ToList();
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
        FilteredCustomerDTO = CustomerDTO;
        Title = $"Customer ({FilteredCustomerDTO?.Count})";

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

    private void AddNewCustomer()
    {
        EditTitle = "Add Customer";
        CustomerId = 0;
        ShowEdit = true;
    }

    private async Task ApplyFilter()
    {
        if (FilteredCustomerDTO == null || CustomerDTO == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(SearchTerm))
        {
            await LoadData();
            Title = $"All Customer ({FilteredCustomerDTO.Count})";
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
    protected void SortCustomer(string sortColumn)
    {
        Guard.Against.Null(sortColumn, nameof(sortColumn));
        if (FilteredCustomerDTO == null)
        {
            return;
        }
        if (sortColumn == "Name")
        {
            FilteredCustomerDTO = FilteredCustomerDTO.OrderBy(v => v.Name).ToList();
        }
        else if (sortColumn == "Name Desc")
        {
            FilteredCustomerDTO = FilteredCustomerDTO.OrderByDescending(v => v.Name).ToList();
        }
    }


    private void DeleteCustomer(int Id)
    {
        CustomerId = Id;
        currentCustomer = FilteredCustomerDTO?.FirstOrDefault(v => v.Id == Id);
        Message = $"Are you sure you want to delete {currentCustomer?.Id} Customer item?";
        ShowDeleteConfirm = true;
    }


    private void HideMessage()
    {
        if (ApplicationState != null)
        {
            ApplicationState.Message = null;
        }
    }
    private void EditCustomer(int Id)
    {
        CustomerId = Id;
        EditTitle = "Edit Customer";
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
        if (CustomerDataService == null) return;
        if (confirmation)
        {
            await CustomerDataService.DeleteCustomer(CustomerId);
            if (ApplicationState != null)
            {
                ApplicationState.Message = $"{CustomerId} Customer item has been deleted successfully";
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