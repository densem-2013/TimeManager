<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    HttpError404
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>HttpError404</h2>

<span>На сервере страница не найдела. Убедитесь в правильности адреса ссылки.</span>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Path" runat="server">
</asp:Content>
