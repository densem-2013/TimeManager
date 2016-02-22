<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.RequestOverviewModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<div style='width: 785px;'>
    <%
        bool isChecked = false;
        var requestEditor = (RequestEditorViewModel)ViewBag.RequestEditorViewModel;

        var requestModel = new RequestModel();
        var requestDetails = new RequestDetailTypesViewModel();
        var contractModel = new ContractModel();
        if (Model.ID != 0)
        {
            requestModel = ControllerHelper.GetRequestOverviewModel(Model.ID);
            requestDetails = ControllerHelper.GetRequestDetailsViewModel(Model.ID);
            contractModel = ControllerHelper.GetContractModel(requestModel.ContractID);
            requestModel.isCustomersWithoutContracts = contractModel.ContractTypeId >= 3 ? true : false;
            // isChecked = contractModel.ContractTypeId > 3 ? true : false;
        }

        if (Model == null)
        {
    %>
    <script type="text/javascript">
        $('button.t-button t-button-icon t-grid-update').remove();
    </script>
    <script type="text/javascript">
        $('.t-grid-update').click(function () {
            return Buttonfunction();

        });
        $('.t-grid-insert').click(function () {
            return Buttonfunction();

        });
    </script>
    <div id='ErrorMessage' style="text-align: center;">
        <label>
        </label>
    </div>
    <% 
        }
        else
        {
    %>
    <table>
        <tr>
            <td valign="top">
                <table>
                    <tr>
                        <td valign="top">
                            <fieldset>
                                <legend>
                                    <%: Html.Label("", "Заявка")%>
                                </legend>
                                <table class="edit-form" style="margin-top: -15px;">
                                    <tr>
                                        <td valign='top' style="width: 75px;">
                                            <div>
                                                <%: Html.LabelFor(model => model.Number)%>
                                            </div>
                                            <div>
                                                <input type="hidden" name='FinishDate' value='<%: DateTime.Now%>' />
                                                <% var number = string.Empty;
                                                   if (Model.Number != null)
                                                   { number = Model.Number; }
                                                   else
                                                   { number = requestEditor.RequestNumber; }
                                                %>
                                                <input class="t-input valid" id='Number' name='Number' readonly='readonly' type='text'
                                                    value='<%: number %>' style="width: 80px;" />
                                                <%--     <%: Html.TextBoxFor(model => model.Number, new { style = "width:120px; " })%>
                                                --%>
                                                <%: Html.ValidationMessageFor(model => model.Number)%>
                                            </div>
                                        </td>
                                        <td valign="top">
                                            <div>
                                                <%: Html.LabelFor(model => model.Date)%><%: Html.Label("", " от")%>
                                            </div>
                                            <div>
                                                <%:
                            Html.Telerik().DatePickerFor(model => model.Date).Value(
                            Model.Date != DateTime.MinValue ? Model.Date : DateTime.Now)%>
                                                <%: Html.ValidationMessageFor(model => model.Date)%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <table class="edit-form">
                                    <tr>
                                        <td>
                                            <div>
                                                <%: Html.Label("TypeId", "Тип")%>
                                            </div>
                                            <div>
                                                <%: Html.Telerik().DropDownList().Name("Type.Id").HtmlAttributes(new {style = " width:200px;"})
                            .ClientEvents(eb => eb.OnChange("GetRequestNumber"))
                            .BindTo(new SelectList(requestEditor.RequestTypes, "ID", "Name", Model.TypeId)) %>
                                                <%: Html.ValidationMessageFor(model => model.TypeId) %>
                                            </div>
                                            <%--  </td>
            <td>--%>
                                            <br />
                                            <%--ProjectManager--%>
                                            <div>
                                                <%: Html.LabelFor(model => model.ProjectManagerId)%>
                                            </div>
                                            <div class="edit-form-longer">
                                                <%: Html.Telerik().DropDownList().Name("ProjectManagerId").HtmlAttributes(new { style = " width:200px;" })
                    .BindTo(new SelectList(requestEditor.Managers, "ID", "ShortName", Model.ProjectManagerId))%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <%  
            if (Model.Number != null)
            { %>
                                <table>
                                    <tr>
                                        <% if (Model.LastUpdateDate.ToString() != "01.01.0001 0:00:00")
                                           {%>
                                        <td>
                                            <%: Html.LabelFor(model => model.LastUpdateDate) %>:
                                        </td>
                                        <td>
                                            <%: Model.LastUpdateDate %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.LabelFor(model => model.LastUpdateUserShortName) %>:
                                        </td>
                                        <td>
                                            <%: Model.LastUpdateUserShortName %>
                                        </td>
                                        <% } %>
                                    </tr>
                                </table>
                                <% } %>
                            </fieldset>
                        </td>
                        <td valign='top'>
                            <fieldset>
                                <legend>
                                    <%: Html.Label("", "Договор")%></legend>
                                <table class="request-edit-contract-form" style="margin-top: -15px;">
                                    <tr>
                                        <td valign='top'>
                                            <div>
                                                <%: Html.LabelFor(model => model.CustomerId)%>
                                                <input id='CustomerName' name='CustomerName' type='hidden'></input>
                                            </div>
                                            <div>
                                                <%  var customerValue = Model.CustomerId != 0 ? Model.CustomerId : 0;
                                                    var selectableCustomers = new List<CustomerModel>();
                                                    selectableCustomers.Add(
                                                        new CustomerModel { ID = Model.CustomerId, Name = Model.CustomerName });
                            
                                                %>
                                                <%: Html.Telerik().ComboBox()
                                  .Name("CustomerId")
                                    .AutoFill(true)
                                    .Value(customerValue.ToString())
                                    .BindTo(new SelectList(selectableCustomers, "ID", "Name"))
                                    .HtmlAttributes(new { style = string.Format("width:{0}px",355) })
                                    .ClientEvents(eb => eb.OnChange("ChangeCustomerForGetName"))
                                    .DataBinding(binding => binding.Ajax()
                                        .Select("GetCustomer", "Requests"))
                                    .Filterable(filtering =>
                                    {
                                        filtering.FilterMode(AutoCompleteFilterMode.Contains);
                                    })
                                    .HighlightFirstMatch(true)
                                                %>
                                                <%: Html.ValidationMessageFor(model => model.ContractId, "Поле Наименование заказчика осталось не заполненым")%>
                                            </div>
                                            <div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td valign='top'>
                                            <%--Contract.Name--%>
                                            <div>
                                                <%: Html.Label("", "Наименование договора")%>
                                            </div>
                                            <div>
                                                <%  var contracts = new List<ContractModel>();
                                                    if (Model.ContractId != 0)
                                                    {
                                                        var contract = new ContractModel() { Name = Model.ContractName, ID = Model.ContractId };
                                                        contracts.Add(contract);
                                                    }
                                                %>
                                                <%:Html.Telerik().DropDownList().Name("ContractId")
                                    .Value(Model.ContractId.ToString())
                                    .BindTo(new SelectList(contracts, "ID", "Name"))
                                    .HtmlAttributes(new { style = "width:100%;" })%>
                                                <%--.ClientEvents(eb => eb.OnChange("onChangeContractName"))--%>
                                                <%: Html.ValidationMessageFor(model => model.ContractId, "Поле Наименование договора осталось не заполненым")%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                                <table>
                                    <tr>
                                        <td>
                                            <%--CheckBoxForCustomersWithoutContracts--%>
                                            <div style="font-size: small; font-weight: 600;">
                                               <% if (requestModel.isCustomersWithoutContracts) { %>
                                                <input type="checkbox" id='isCustomersWithoutContracts' name='isCustomersWithoutContracts'
                                                    checked='checked' disabled="disabled" />
                                                <%
                                                }
                                                else
                                                {
                                                %>
                                                <%:Html.CheckBoxFor(model=>model.isCustomersWithoutContracts) %>
                                                <%                         }%>
                                                <%:Html.Label("Без договора")%>
                                            </div>
                                        </td>
                                        <td>
                                            <%--StartDate--%>
                                            <div>
                                                <%: Html.LabelFor(model => model.StartDate) %>
                                            </div>
                                            <div>
                                                <%:
                            Html.Telerik().DatePickerFor(model => model.StartDate).Value(
                            Model.StartDate != DateTime.MinValue ? Model.StartDate : DateTime.Now)%>
                                                <%: Html.ValidationMessageFor(model => model.StartDate)%>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div>
                                                <%: Html.Label("", "Наименование проекта")%>
                                            </div>
                                            <div class="request-edit-contract-form">
                                                <% if (requestModel.Phases.Count == 1 || requestModel.Phases.Count == 0)
                                                   {
                                                       var phase = requestModel.Phases.Count == 1 ? requestModel.Phases[0] : null;
                                                       if (phase != null)
                                                       {
                                                %>
                                                <%: Html.TextArea("Phases[0].Name", phase.Name, new { onchange = "ValidateInputProjectName()" })%>
                                                <%   
                                                       }
                                                       else
                                                       {
                                                %>
                                                <%: Html.TextArea("Phases[0].Name", "", new { onchange = "ValidateInputProjectName()" })%>
                                                <%-- <textarea id='Phases[0].Name' name='Phases[0].Name' type='text' value=''></textarea>--%>
                                                <%  
                                                       }
                                                   }
                                                %>
                                                <%: Html.ValidationMessage("Phases[0].Name")%>
                                            </div>
                                        </td>
                                        <% if (requestModel.Phases.Count == 1 || requestModel.Phases.Count == 0)
                                           {%>
                                        <td style='width: 150px;'>
                                            <div>
                                                <%: Html.Label("", "Дата окончания работ")%>
                                            </div>
                                            <div>
                                                <%var phase = requestModel.Phases.Count == 1 ? requestModel.Phases[0] : null;%>
                                                <%: Html.Telerik()
                            .DatePicker()
                            .Name(string.Format("Phases[{0}].DeadLine", 0))
                            .ShowButton(true)
                            .Value(phase != null ? phase.DeadLine.Value.Date : DateTime.Now)%>
                                            </div>
                                        </td>
                                        <%
                                           }
                                        %>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label id='ProjectNameValidation' style='color: Red;'>
                                            </label>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div class="footer-description" style="width: 100%; margin-top: -35px;">
                    <fieldset>
                        <legend>
                            <%: Html.Label("", "Детали заявки")%></legend>
                        <div style="margin-top: -15px;">
                            <table style="width: 100%;">
                                <% if (requestModel.Type.ID == 2)
                                   {%>
                                <tr align='left'>
                                    <td style="width: 40%;">
                                        <div id="request_docum_label" style="display: block;">
                                            <%: Html.LabelFor(model => model.Documentation)%></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div id="request_docum_value" style="display: block;">
                                            <%: Html.TextAreaFor(model => model.Documentation, new { onchange = "ValidateInputProjectName()" })%></div>
                                    </td>
                                </tr>
                                <%}
                                   else
                                   {%>
                                <tr align='left'>
                                    <td style="width: 40%;">
                                        <div id="request_docum_label" style="display: none;">
                                            <%: Html.LabelFor(model => model.Documentation)%></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div id="request_docum_value" style="display: none;">
                                            <%: Html.TextAreaFor(model => model.Documentation, new { onchange = "ValidateInputProjectName()" })%></div>
                                    </td>
                                </tr>
                                <%} %>
                                <tr align='left'>
                                    <td style="width: 40%;">
                                        <%: Html.LabelFor(model => model.Detail)%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <%: Html.TextAreaFor(model => model.Detail, new { onchange = "ValidateInputProjectName()" })%>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="detailTable" style="margin-top: -10px;">
                            <% if (requestDetails.RequestDetailTypes.Count() == 6)
                               {
                            %>
                            <table style="width: 100%;" id="tableDetails">
                                <tr>
                                    <td style="width: 5%;">
                                        <%: Html.Label("", "№")%>
                                    </td>
                                    <td>
                                        <%: Html.Label("", "Наименование")%>
                                    </td>
                                    <td align="center" style="width: 10%;">
                                        <%: Html.Label("", "Необходимое отметить")%>
                                    </td>
                                </tr>
                                <%
                                   foreach (var requestDetailType in requestEditor.RequestDetailTypes)
                                   {
                                %>
                                <tr>
                                    <td>
                                        <%: Html.Label("", requestDetailType.ID.ToString())%>
                                    </td>
                                    <td>
                                        <%: Html.Label("", requestDetailType.Name)%>
                                    </td>
                                    <td>
                                        <% var detailIndex = requestDetailType.ID - 1;
                                           var checkedValue = "";
                                           if (requestDetails.RequestDetailTypes.Count() > 0)
                                           {
                                               if (requestDetails.RequestDetailTypes[(int)detailIndex].Checked)
                                               {
                                                   checkedValue = string.Format("checked ='{0}'",
                                                       requestDetails.RequestDetailTypes[(int)detailIndex].CheckedValue);
                                               }
                                           }
                                           var checkboxName = string.Format("RequestDetails[{0}].CheckedValue", detailIndex);
                                           var requestDetailTypeName = string.Format("RequestDetails[{0}].RequestDetailTypeID", detailIndex); %>
                                        <input type="checkbox" name='<%: checkboxName %>' <%:checkedValue%>></input>
                                        <input type="hidden" name='<%:  requestDetailTypeName %>' value='<%:requestDetailType.ID %>'></input>
                                    </td>
                                </tr>
                                <%  
                                   }
                                %>
                            </table>
                            <%} %>
                        </div>
                    </fieldset>
                </div>
            </td>
        </tr>
        <%  if (requestModel.Phases.Count == 1 || requestModel.Phases.Count == 0)
            {  %>
        <tr>
            <td>
                <div style="margin-top: -25px;">
                    <fieldset>
                        <legend>
                            <%: Html.Label("", "Распределение бюджета")%></legend>
                        <% var phase = requestModel.Phases.Count == 1 ? requestModel.Phases[0] : null;

                           Infocom.TimeManager.WebAccess.Models.BudgetAllocationModel budgetAllocation = null;
                           var deadLine = DateTime.Now;
                           if (phase != null)
                           {
                               deadLine = phase.DeadLine != null ? phase.DeadLine.Value.Date : DateTime.Now;
                           }%>
                        <table id="budgetAllocation" style="width: 100%">
                            <thead>
                                <tr>
                                    <td align="center" style='width: 80px;'>
                                        <%: Html.Label("", "Департамент")%>
                                    </td>
                                    <td align="center" style='width: 180px;'>
                                        <%: Html.Label("", "Трудозатраты в чел.ч")%>
                                    </td>
                                    <td align="center" style='width: 80px;'>
                                        <%: Html.Label("", "Бюджет в грн.")%>
                                    </td>
                                    <td align="center" style="width: 180px;">
                                        <%: Html.Label("", "Дата завершения работ департаментом" )%>
                                    </td>
                                    <td align="center">
                                        <%:Html.Label("", "Отметить необходимые департаменты")%>
                                    </td>
                                </tr>
                            </thead>
                            <% int baIndex = -1;
                               if (phase != null)
                               {
                                   foreach (var department in requestEditor.Departments)
                                   {
                                       if (department.ID != 5)
                                       {
                                           baIndex++;
                                           if (phase.Budget.BudgetAllocations.Any(i => i.Department.ID == department.ID))
                                           { %>
                            <tr>
                                <td style='width: 90px;' align="center">
                                    <input style='width: 90px;' name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].Department.ID", 0, baIndex) %>'
                                        type="hidden" value='<%: department.ID %>' />
                                    <span>
                                        <%: department.ShortName %></span>
                                </td>
                                <td style='width: 170px;' align="center">
                                    <input style='width: 170px;' type='text' id='<%: string.Format("Phases_{0}_Budget_BudgetAllocations_{1}_AmountOfHours", 0, baIndex) %>'
                                        name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfHours", 0, baIndex) %>'
                                        value='<%:
    phase.Budget.BudgetAllocations.FirstOrDefault(
        b => b.Department.ID == department.ID).AmountOfHours %>' onchange='ValidateInputAmountOfHours(0)' />
                                </td>
                                <td style='width: 150px;'>
                                    <input type='text' style='width: 170px;' id='<%: string.Format("Phases_{0}_Budget_BudgetAllocations_{1}_AmountOfMoney", 0, baIndex) %>'
                                        name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfMoney", 0, baIndex) %>'
                                        value='<%: string.Format("{0:0.00}",
                                       phase.Budget.BudgetAllocations.
                                           FirstOrDefault(
                                               b =>
                                               b.Department.ID ==
                                               department.ID).AmountOfMoney) %>' onchange='ValidateInputAmountOfMoney(0)' />
                                </td>
                                <td align="center">
                                    <%: Html.Telerik()
                             .DatePicker()
                             .Name(string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].DateEndWorkByDepartment", 0,
                                                 baIndex)).HtmlAttributes(new
                                                     {
                                                         style = "font-size:9pt;width:170px;"
                                                     })
                             .ShowButton(true)
                             .Value(
                                 phase.Budget.BudgetAllocations.FirstOrDefault(b => b.Department.ID == department.ID).
                                     DateEndWorkByDepartment.Value.ToShortDateString())
                                    %>
                                </td>
                                <td align="center" style="width: 100px">
                                    <input type='checkbox' id='checkbox_<%: baIndex %>' checked='checked' onclick='onClick(<%: baIndex %>)' />
                                </td>
                            </tr>
                            <% }
                                           else
                                           {
                            %>
                            <tr>
                                <td align="center" style='width: 90px;'>
                                    <input style='width: 90px;' name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].Department.ID", 0, baIndex) %>'
                                        type="hidden" value='<%: department.ID %>' />
                                    <span>
                                        <%: department.ShortName %></span>
                                </td>
                                <td style='width: 170px;'>
                                    <input type='text' style="width: 170px" id='<%: string.Format("Phases_{0}_Budget_BudgetAllocations_{1}_AmountOfHours", 0, baIndex) %>'
                                        name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfHours", 0, baIndex) %>'
                                        value='0' onchange='ValidateInputAmountOfHours(0)' disabled="disabled" />
                                </td>
                                <td style='width: 170px;'>
                                    <input type='text' style="width: 170px" id='<%: string.Format("Phases_{0}_Budget_BudgetAllocations_{1}_AmountOfMoney", 0, baIndex) %>'
                                        name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfMoney", 0, baIndex) %>'
                                        value='0,00' onchange='ValidateInputAmountOfMoney(0)' disabled="disabled" />
                                </td>
                                <td align="center">
                                    <%: Html.Telerik()
                             .DatePicker()
                             .Name(string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].DateEndWorkByDepartment", 0,
                                                 baIndex)).HtmlAttributes(new
                                                     {
                                                         style = "font-size:9pt;width:170px;"
                                                     })
                             .ShowButton(true)
                             .ClientEvents(events => events
                                                         .OnLoad("onLoad"))
                             .Value(DateTime.Now.ToShortDateString())
                                    %>
                                </td>
                                <td align="center" style="width: 100px">
                                    <input type='checkbox' id='checkbox_<%: baIndex %>' onclick='onClick(<%: baIndex %>)' />
                                </td>
                            </tr>
                            <%

                             }
                                       }
                                   }
                               }
                               else
                               {

                                   foreach (var department in requestEditor.Departments)
                                   {

                                       if (department.ID != 5)
                                       {
                                           baIndex++;
                            %>
                            <tr>
                                <td align="center" style='width: 100px;'>
                                    <input style='width: 90px;' name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].Department.ID", 0, baIndex) %>'
                                        type="hidden" value='<%: department.ID %>' />
                                    <span>
                                        <%: department.ShortName %></span>
                                </td>
                                <td style='width: 170px;'>
                                    <input type='text' style="width: 170px" id='<%:string.Format("Phases_{0}_Budget_BudgetAllocations_{1}_AmountOfHours", 0, baIndex) %>'
                                        name='<%:string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfHours", 0, baIndex) %>'
                                        value='0' onchange='ValidateInputAmountOfHours(0)' disabled="disabled" />
                                </td>
                                <td style='width: 170px;'>
                                    <input type='text' style="width: 170px" id='<%:string.Format("Phases_{0}_Budget_BudgetAllocations_{1}_AmountOfMoney", 0, baIndex) %>'
                                        name='<%:string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfMoney", 0, baIndex) %>'
                                        value='0,00' onchange='ValidateInputAmountOfMoney(0)' disabled="disabled" />
                                </td>
                                <td align="center">
                                    <%: Html.Telerik()
                            .DatePicker()
                            .Name(string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].DateEndWorkByDepartment", 0, baIndex)).HtmlAttributes(new
                                {
                                    style = "font-size:9pt;width:170px;"
                                })
                            .ShowButton(true)
                            .ClientEvents(events => events
                                .OnLoad("onLoad"))
                            .Value(DateTime.Now.ToShortDateString())
                                    %>
                                </td>
                                <td align="center" style="width: 100px">
                                    <input type='checkbox' id='checkbox_<%:baIndex %>' onclick='onClick(<%:baIndex %>)' />
                                </td>
                            </tr>
                            <% 
                                       }
                                   } 
                               
                            %>
                            <%} %>
                        </table>
                        <%-- <div style="text-align: right;">
                            <input type="button" value="Добавить департамент" onclick="AddBudgetAllocationNew()">
                            <!--<input type="button" value="Кнопка" onclick="Buttonfunction()">-->
                            <input type="button" value="Удалить" onclick="DeleteDepartmentNew()">
                        </div>--%>
                    </fieldset>
                </div>
            </td>
        </tr>
        <% }
            else
            { %>
        <tr>
            <td valign="top">
                <div class="footer-description" style="margin-top: -45px; width: 200px;">
                    <fieldset>
                        <legend>
                            <%: Html.Label("", "Этапы проекта")%></legend>
                        <table id="phases" style="margin-top: -25px;">
                            <thead>
                                <td>
                                    <%: Html.Label("", "№")%>
                                </td>
                                <td>
                                    <%: Html.Label("", "Наименование этапа")%>
                                </td>
                                <td>
                                    <%: Html.Label("", "Дата окончания")%>
                                </td>
                                <td align="left">
                                    <table>
                                        <tr>
                                            <td align="right">
                                                <%: Html.Label("", "Департамент")%>
                                            </td>
                                            <td align="center" style='width: 150px;'>
                                                <%: Html.Label("", "Трудозатраты в чел./час")%>
                                            </td>
                                            <td align="center" style='width: 150px;'>
                                                <%: Html.Label("", "Бюджет в грн.")%>
                                            </td>
                                            <td align="center" style="width: 300px;">
                                                <%: Html.Label("", "Дата завершения работ департаментом" )%>
                                            </td>
                                            <td>
                                                <%:Html.Label("", "Отметить необходимые департаменты")%>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                </td>
                            </thead>
                            <tbody id="phase">
                                <%
                int index = -1;
                foreach (var phase in requestModel.Phases)
                {
                    index += 1;
                                %>
                                <tr class="phase" id="phase<%:index%>">
                                    <td>
                                        <%: Html.Label("", string.Format("{0}.", index + 1))%>
                                    </td>
                                    <td>
                                        <%: Html.TextBox(string.Format("Phases[{0}].Name", index), phase.Name)%>
                                    </td>
                                    <td>
                                        <%: Html.Telerik()
                            .DatePicker()
                            .Name(string.Format("Phases[{0}].DeadLine", index))
                            .ShowButton(true)
                            .Value(phase.DeadLine != null ? phase.DeadLine.Value.Date : DateTime.Now)%>
                                    </td>
                                    <td>
                                        <div>
                                            <table id="ba<%: index %>">
                                                <% int baIndex = -1;
                                                   foreach (var budgetAllocation in phase.Budget.BudgetAllocations)
                                                   {
                                                       baIndex++;
                                                %>
                                                <tr>
                                                    <td style='width: 100px;'>
                                                        <%-- <span>     <%: department.ShortName %></span>--%>
                                                        <%--<select name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].Department.ID", index, baIndex)%>'>
                                                        --%>
                                                        <% foreach (var department in requestEditor.Departments)
                                                           {
                                                               if (budgetAllocation.Department.ID != department.ID)
                                                               {	
                                                        %>
                                                        <%-- <option value='<%:department.ID%>'>
                                                                <%:department.ShortName%></option>--%>
                                                        <input style='width: 90px;' name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].Department.ID", 0, baIndex) %>'
                                                            type="hidden" value='<%: department.ID %>' />
                                                        <span>
                                                            <%: department.ShortName %></span>
                                                        <%
                                                                   }
                                                                   else
                                                                   {
                                                        %>
                                                        <%--<option value='<%:department.ID%>' selected='selected'>
                                                                <%:department.ShortName%></option>--%>
                                                        <input style='width: 90px;' name='<%: string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].Department.ID", 0, baIndex) %>'
                                                            type="hidden" value='<%: department.ID %>' />
                                                        <span>
                                                            <%: department.ShortName %></span>
                                                        <%
                                                                   }
                                                               }%>
                                                        <%-- </select>--%>
                                                    </td>
                                                    <td style='width: 150px;'>
                                                        <%: Html.TextBox(string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfHours", index, baIndex), budgetAllocation.AmountOfHours)%>
                                                    </td>
                                                    <td style='width: 150px;'>
                                                        <%: Html.TextBox(string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].AmountOfMoney", index, baIndex), string.Format("{0:0.00}", budgetAllocation.AmountOfMoney))%>
                                                    </td>
                                                    <td>
                                                        <%: Html.Telerik()
                                                        .DatePicker()
                                                        .Name(string.Format("Phases[{0}].Budget.BudgetAllocations[{1}].DateEndWorkByDepartment", index, baIndex))
                                                        .HtmlAttributes(new
                                                        {
                                                            style = "font-size:10pt;width:170px;"
                                                        })
                                                        .ShowButton(true)
                                                        .Value(DateTime.Now.ToShortDateString())
                                                        %>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <%}%>
                                            </table>
                                        </div>
                                    </td>
                                    <td>
                                        <input type="button" value="+" onclick="AddBudgetAllocation('<%:index %>')">
                                        <input type="button" value="-" onclick="DeleteDepartment('<%:index %>')">
                                    </td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                        <div>
                            <input type="button" id="uxAddPhase" value="Добавить этап" onclick="AddPhase()" />
                            <input type="button" id="uxDeletePhase" value="Удалить этап" onclick="DeletePhase()" />
                        </div>
                    </fieldset>
                </div>
            </td>
        </tr>
        <% } %>
    </table>
    <%} %>
</div>
