<%@ Page Language="C#" UICulture="ru-RU" Culture="ru-RU" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Infocom.TimeManager.WebAccess.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Аутентификация
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    
        <h2>Аутентификация</h2>
        <b>Введите имя и пароль доменной учетной записи.</b>
  
    <% using (Html.BeginForm())
       { %>
    <%: Html.ValidationSummary(true, "Вход не удался. Пожалуйста, исправьте ошибки и повторите.")%>
    <div>
        <fieldset>
            <legend>Информация об учетной записи</legend>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.UserName) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(m => m.UserName) %>
                <%: Html.ValidationMessageFor(m => m.UserName) %>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.Password) %>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.Password) %>
                <%: Html.ValidationMessageFor(m => m.Password) %>
            </div>
           <%-- <div class="editor-label">
                <%: Html.CheckBoxFor(m => m.RememberMe) %>
                <%: Html.LabelFor(m => m.RememberMe) %>
            </div>--%>
            <p>
                <input type="submit" value="Вход" />
            </p>
        </fieldset>
    </div>
    <% } %>
    
</asp:Content>
