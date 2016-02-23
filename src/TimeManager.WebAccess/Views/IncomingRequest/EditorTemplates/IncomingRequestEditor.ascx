<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Infocom.TimeManager.WebAccess.Models.ContractModel>" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<div class="order-edit-form" style="width:550px;">
    <%
        var contractEditor = (ContractEditorViewModel)ViewBag.ContractEditorViewModel;
    %>
  <table >
    <tr>
        <td><%: Html.LabelFor(model => model.Number)%></td>
        <td>
            <div>
                <%if (Model.Number!=null)
                    {
                        var number = Model.Number.Replace("АР № ", "").Replace("ТП № ", "");
                        %>
                    <input class="t-input valid" name='Number' type='text' value='<%: number %>' style = " width:60%;">
                <%} else { %>
                    <%: Html.TextBoxFor(model => model.Number, new {style = " width:60%;"})%>
                <%} %>
                <%: Html.ValidationMessageFor(model => model.Number)%>
            </div>
       </td>
       <td><%: Html.LabelFor(model => model.ContractTypeName)%></td>
        <td width='30%'>
            <div>
                <%:Html.Telerik().DropDownList()
                        .Name("ContractTypeId")
                        .Encode(false)
                        .Value(Model.ContractTypeId.ToString()) 
                        .BindTo(new SelectList(contractEditor.ContractTypes, "ID", "Name"))
                        .HtmlAttributes(new { style = string.Format("width:{0}px", 150) })
                    %>
            </div>
       </td>
       <td>
            <div>
                <%: Html.LabelFor(model => model.SigningDate)%>
            </div>
        </td>
        <td>
            <div>
                <%:
                    Html.Telerik().DatePickerFor(model => model.SigningDate).Value(
                                        Model.SigningDate != null ? Model.SigningDate : DateTime.Now)%>
                <%: Html.ValidationMessageFor(model => model.SigningDate)%>
            </div>
       </td>
    </tr>
    </table>
    <table>
    <tr>
        <td>
            <div>
                <%: Html.LabelFor(model => model.Customer)%>
            </div>
        </td>
        <td>
            <div>
            <% var selectedIndex = Model.Customer.ID != 0 ? Model.Customer.ID : -1;
               %>
          
          <%: Html.Telerik().AutoComplete()
                .Name("Customer.Name")
                .AutoFill(true)
                .Encode(false)
                .BindTo(contractEditor.Customers.Select(c=>c.Name))
                .HtmlAttributes(new { style = string.Format("width:{0}px", 430) })
                .Filterable(filtering =>
                {
                    filtering.FilterMode(AutoCompleteFilterMode.Contains);
                })
                .HighlightFirstMatch(true) %>
            </div>
        </td>
    </tr>
   <%-- <tr>
        <td>
            <div>
                <%: Html.LabelFor(model => model.Name)%>
            </div>
        </td>
        <td>
            <div>
                <%: Html.TextAreaFor(model => model.Name, new { style = " width:430px;" })%>
                <%: Html.ValidationMessageFor(model => model.Name)%>
            </div>
        </td>
    </tr>--%>
  </table>  
</div>
