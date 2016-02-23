<%@ Page Title="" UICulture="ru-RU" Culture="ru-RU" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
    Inherits="ViewPage<Infocom.TimeManager.WebAccess.ViewModels.RequestDetailsViewModel>" %>

<%@ Import Namespace="Infocom.TimeManager.WebAccess.Extensions" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.Models" %>
<%@ Import Namespace="Infocom.TimeManager.WebAccess.ViewModels" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали заявки
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% var isHasErrorText = !String.IsNullOrEmpty((string)this.ViewBag.OverviewErrorMessage);
       if (isHasErrorText)
       {
    %><span style="color: Red; margin-left: 30px; font-size: 14px;"> Ошибка:
        <%: this.ViewBag.OverviewErrorMessage%>. Пожалуйста, исправьте ошибки и повторите
        действие снова.</span><%
       } %>
    <div class="project_left_info">
        <table style="width: 100%">
            <tr>
                <td style="width: 70%">
                    <table class="menucontainer">
                        <tr>
                            <td style="width: 30%">
                                Номер:
                            </td>
                            <td style="width: 70%">
                                <%: Model.Request.Number%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Заказчик:
                            </td>
                            <td>
                                <%: Model.Request.CustomerName %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Дата создания:
                            </td>
                            <td>
                                <%: Model.Request.Date.ToString("dd.MM.yyyy")%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Дата начала работ:
                            </td>
                            <td>
                                <%: Model.Request.StartDate.ToString("dd.MM.yyyy")%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Наименование договора:
                            </td>
                            <td>
                                <%: Model.Request.ContractName%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Тип:
                            </td>
                            <td>
                                <%: Model.Request.Type.Name%>
                            </td>
                        </tr>
                        <%if (Model.Request.Type.ID != 3)
                          { %>
                        <tr>
                            <td>
                                Номер договора:
                            </td>
                            <td>
                                <%: Model.Request.ContractNumber%>
                            </td>
                        </tr>
                        <%}%>
                        <tr>
                            <td>
                                Руководитель проекта в КД:
                            </td>
                            <td>
                                <%: Model.Request.ProjectManagerShortName%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Дата договора:
                            </td>
                            <td>
                                <%: string.Format("{0:dd.MM.yyyy}", Model.Request.ContractSigningDate)%>
                            </td>
                        </tr>
                        <% if (Model.Request.Type.ID == 2)
                           {%>
                        <tr>
                            <td style="width: 100%">
                                <%: Html.LabelFor(model => model.Request.Documentation)%>:
                            </td>
                            <td>
                                <%: Html.Label("", Model.Request.Documentation)%>
                            </td>
                        </tr>
                        <%} %>
                    </table>
                </td>
                <td valign="top">
                    <table>
                        <tr>
                            <td>
                                Статус заявки:
                            </td>
                            <td>
                                <%: Model.Request.StatusName%>
                            </td>
                            <td>
                                <%if (Model.Request.StatusId != 131077 && ((bool)this.Session["RequestPermission"]))
                                  {
                                %>
                                <%-- <form action = '/Requests/RequestStatusClose' method="post">--%>
                                <input type='hidden' name='requestId' value="<%: Model.Request.ID %>" />
                                <input type="submit" name="IsValidate" value="Закрыть" onclick='RequestCloseConfirm("<%: Model.Request.ID%>")' />
                                <%--</form>--%>
                                <%
                                 
                          }
                                  else
                                  {
                                      if (Model.IsAdmin && (bool)this.Session["RequestPermission"]) // Только для руководителей КД
                                      {
                                %>
                                <form action='/Requests/RequestStatusReOpen' method="post">
                                <input type='hidden' name='requestId' value="<%: Model.Request.ID %>" />
                                <input type="submit" name="IsValidate" value="Открыть" />
                                </form>
                                <%
                              }
                          } %>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                Подтверждение заявки:
                            </td>
                            <td>
                                <% if (Model.Request.IsValidated)
                                   {%>
                                <label style='font-size: 14px; color: Green;'>
                                    Подтверждена</label>
                                <% }
                                   else
                                   { %>
                                <label style='font-size: 14px; color: Red;'>
                                    Не подтверждена</label>
                                <%}%>
                            </td>
                            <td>
                                <%  if (!Model.Request.IsValidated)
                                        if ((bool)this.Session["ApprovePermission"])
                                        { %>
                                <form action='/Requests/RequestCheck?requestID=<%: Model.Request.ID %>' method="post">
                                <input type="submit" name="IsValidate" value="Подтвердить" />
                                </form>
                                <%} %>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <% if (Model.Request.LastUpdateDate.ToString() != "01.01.0001 0:00:00")
                               {%>
                            <td>
                                <%: Html.LabelFor(model => model.Request.LastUpdateDate) %>:
                            </td>
                            <td>
                                <%: Model.Request.LastUpdateDate %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%: Html.LabelFor(model => model.Request.LastUpdateUserShortName)%>:
                            </td>
                            <td>
                                <%: Model.Request.LastUpdateUserShortName %>
                            </td>
                            <% } %>
                        </tr>
                    </table>
                    <% if ((bool)this.Session["RequestPermission"])
                       {
                           if (Model.Request.Type.ID == 3)
                           {
                           
                    %>
                    <form action='/Requests/TenderResult' method="post">
                    <input type="hidden" name='requestID' value="<%: Model.Request.ID %>" />
                    <table>
                        <tr>
                            <td>
                                Результат тендера:
                            </td>
                            <td>
                                <%: Model.Request.TenderResult%>
                            </td>
                            <td>
                                <select name='TenderResult'>
                                    <option>Выиграли</option>
                                    <% if (Model.Request.TenderResult == "Проиграли")
                                       {%>
                                    <option selected='selected'>Проиграли</option>
                                    <%}
                                       else
                                       { %>
                                    <option>Проиграли</option>
                                    <%} if (Model.Request.TenderResult == "На рассмотрении")
                                       {%>
                                    <option selected='selected'>На рассмотрении</option>
                                    <%}
                           else
                           { %>
                                    <option>На рассмотрении</option>
                                    <%} %>
                                </select>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr style='width: 60%;'>
                            <td>
                                Примечание по тендеру:
                            </td>
                            <td>
                                <textarea type="text" name="tenderRemark"><%: Model.Request.TenderRemark %></textarea>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <%if (string.IsNullOrEmpty(Model.Request.TenderResult))
                                  {
                                %><input type="submit" name="IsValidate" value="Сохранить" /><%
                              }
                                  else
                                  { 
                                %><input type="submit" name="IsValidate" value="Изменить" /><%
                              } %>
                            </td>
                        </tr>
                    </table>
                    </form>
                    <% }
                   }%>
                </td>
            </tr>
        </table>
        <table style="width:100%">
            <tr>
                <td style="width: 100%">
                    <%: Html.LabelFor(model => model.Request.Detail)%>:
                </td>
            </tr>
            <tr>
                <td>
                    <%: Html.Label("",Model.Request.Detail)%>
                </td>
            </tr>
        </table>
        <hr />
        <% if (Model.Request.RequestDetails.Count == 6)
           {
        %>
        <table style="width: 100%;">
            <tr>
                <td style="width: 5%;">
                    <%: Html.Label("", "№")%>
                </td>
                <td style="width: 85%;">
                    <%: Html.Label("", "Наименование")%>
                </td>
                <td align="center" style="width: 10%;">
                    <%: Html.Label("", "Необходимое отметить")%>
                </td>
            </tr>
            <%
                   foreach (var requestDetailType in Model.RequestDetailTypes)
                   {
            %>
            <tr>
                <td>
                    <%: Html.Label("", requestDetailType.ID.ToString())%>
                </td>
                <td>
                    <%: Html.Label("", requestDetailType.Name)%>
                </td>
                <td align="center">
                    <% var detailIndex = requestDetailType.ID - 1;
                       var checkedValue = "";
                       if (Model.Request.RequestDetails.Count > 0)
                       {
                           if (Model.Request.RequestDetails[(int)detailIndex].Checked)
                           {
                               checkedValue = string.Format("checked ='{0}'",
                                   Model.Request.RequestDetails[(int)detailIndex].CheckedValue);
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
        <hr />
        <div>
            Распределение бюджетов:</div>
        <% if (Model.IsAdmin || (bool)this.Session["RequestPermission"])
           {
               if (Model.Request.Phases.Count > 0)
               {
                            
                        
        %><table class="t-widget t-grid" style="width: 800px">
            <thead class="t-grid-header">
                <tr>
                    <td style="width: 10%">
                        Департамент
                    </td>
                    <td style="width: 15%">
                        Бюджет, чел.ч
                    </td>
                    <td style="width: 15%">
                        Бюджет, грн.
                    </td>
                    <td style="width: 20%">
                        Затраты, чел.ч
                    </td>
                    <td style="width: 15%">
                        Затраты, грн.
                    </td>
                    <td style="width: 25%">Дата завершения работ</td>
                </tr>
            </thead>
            <tbody>
                <%
                            bool tAlt = true;
                            foreach (var budgetAllocation in Model.Request.Phases.First().Budget.BudgetAllocations)
                            {
                                tAlt = !tAlt;
                %>
                <tr<%:(tAlt ? " class=t-alt" :"") %>>
                           <td  style="text-align:left;">
                                <%: string.Format("{0}", budgetAllocation.Department.ShortName)%>
                            </td>
                            <td  style="text-align:right;">
                              <%: string.Format("{0}", budgetAllocation.AmountOfHours)%>  
                            </td> 
                            <td  style="text-align:right;">
                                <%: string.Format("{0:N2}", budgetAllocation.AmountOfMoney)%>
                            </td>
                           
                           <% var budget = new Infocom.TimeManager.WebAccess.ViewModels.BudgetViewModel();
                              if (Model.Projects.Any())
                              {
                                  budget = ControllerHelper.GetBudgetModelBy(budgetAllocation.Department.ID, Model.Projects.First().ID);
                              }
                              %>  
                           
                            <td style="text-align:right;"> 
                               <%: string.Format("{0}", budget.SpentTime)%> 
                            </td>
                             <td style="text-align:right;"> 
                                <%: string.Format("{0:N2}", budget.SpentMoney)%>
                            </td>
                            <td style="text-align:right;"> 
                                <%: string.Format("{0:dd.MM.yyyy}", budgetAllocation.DateEndWorkByDepartment)%>
                            </td>
                            </tr><%}
                            %>
            </tbody>
        </table>
        <%
                        }

                    }%>
        <hr />
        <table style="width: 80%;">
            <tr>
                <td style="width: 10%">
                    №:
                </td>
                <td style="width: 30%">
                    Наименование проекта:
                </td>
                <td style="width: 20%">
                    Дата окончания:
                </td>
                <td style="width: 20%">
                    Департаменты:
                </td>
                <td style="width: 20%">
                    Статус этапа:
                </td>
                <td style="width: 20%">
                </td>
                <td>
                </td>
            </tr>
            <% int phaseNumber = 0;
               foreach (var phase in Model.Request.Phases)
               {
                   phaseNumber++;
            %>
            <tr>
                <td>
                    <%: phaseNumber%>
                </td>
                <td>
                    <%  long projectId = 0;
                        if (Model.Projects.FirstOrDefault() != null)
                        {
                    %><a href='../ProjectDetails/Index?ProjectID=<%: Model.Projects.First().ID%>'><%: phase.Name%></a>
                    <%  
                        }
                        else
                        { 
                    %>
                    <%: phase.Name%>
                    <%
                        }
                    %>
                </td>
                <td>
                    <%:string.Format("{0:dd.MM.yyyy}",phase.DeadLine)%>
                </td>
                <td>
                    <% 
                        string departmentList = null;
                        foreach (var ba in phase.Budget.BudgetAllocations)
                        {
                            if (ba.Department != null)
                            {

                                if (departmentList != null) departmentList += ", ";
                                departmentList += ba.Department.ShortName;

                            }

                        } 
                    %>
                    <span>
                        <%:departmentList%>
                    </span>
                    <%
                    %>
                </td>
                <%--  <td>  <% string departmentList;
                         foreach (var ba in phase.Budget.BudgetAllocations)
                        {
                            if (ba.Department!=null)
                            {
                                if (departmentList!=null) departmentList+=", ";
                                departmentList += ba.Department.ShortName;
                            }
                       
                        } 
                             
                      %> <span><%:departmentList%></span>
                       </td>--%>
                <%if (Model.Request.StatusId != 131077)
                  { %>
                <td>
                    <%: phase.ProcessState%>
                </td>
                <%if (Roles.IsUserInRole("TimeManagerAdministrators") || (this.Session["userName"].ToString() == "INFOCOM-LTD\\student" && Model.Request.Type.ID == 6))
                  {%>
                <td>
                    <% if (phase.ProcessState == "Необработанная" && Model.Request.IsValidated)
                       {
                    %>
                    <form method="post" action='/ProjectsOverview/CreateProjectFromRequest'>
                    <input type='hidden' name='requestId' value='<%: Model.Request.ID %>' />
                    <input type='submit' style='width: 120px' value='Создать проект' title='Кнопка создаёт проект и выводит окно редактирования проекта' />
                    </form>
                    <% } %>
                    <%-- class="t-button-custom"--%>
                </td>
                <%} %>
                <% }
                  else
                  {%>
                <td>
                    Закрытая
                </td>
                <% }%>
            </tr>
            <% }%>
        </table>
        <hr />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
    <table style='width: 100%;'>
        <tr>
            <td style='width: 90%;'>
                <span class="navlink">
                    <% if ((bool)this.Session["RequestPermission"] || Roles.IsUserInRole("TimeManagerAdministrators"))
                       {%>
                    <%: Html.ActionLink("Заявки", MVC.Requests.Index())%>
                </span><span class="navlink">></span> <span class="navlink">
                    <%} %>
                    <%if (Model.Request != null)
                      {
                    %><%: Model.Request.Number.ReduceTo(30)%><%
  } %>
                </span>
            </td>
            <td align='right'>
                <span class="navlink">
                    <% if ((bool)this.Session["RequestPermission"] || Roles.IsUserInRole("TimeManagerAdministrators"))
                       {%>
                    <%: Html.ActionLink("X", MVC.Requests.Index(), new { style = "color:Red;", title = "Назад" })%>
                    <%} %>
                </span>
            </td>
        </tr>
    </table>
</asp:Content>
