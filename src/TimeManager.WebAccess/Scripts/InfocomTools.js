function dateToYmd(date) {
        var d = date.getDate();
        var m = date.getMonth() + 1;
        var y = date.getFullYear();
        return '' + (d <= 9 ? '0' + d : d) + '.' + (m <= 9 ? '0' + m : m) + '.' + y;
    }
function onLoad(e) {
    //alert("Hello");
    $(this).data("tDatePicker").disable(true);
}
function onClick(e) {
    //alert("Hello");
    var checkBoxId = document.getElementById("checkbox_" + e);
   if (checkBoxId.checked) {
       document.getElementById("Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfHours").removeAttribute("disabled");
       document.getElementById("Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfMoney").removeAttribute("disabled");
       $("#Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfMoney").val("1,00");
       $("#Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfHours").val("1");
       $("#Phases\\[0\\]_Budget_BudgetAllocations\\[" + e + "\\]_DateEndWorkByDepartment").data("tDatePicker").enable(true);
   } else {
       var dat = new Date();
       document.getElementById("Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfHours").setAttribute("disabled", "disabled");
       document.getElementById("Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfMoney").setAttribute("disabled", "disabled");
       $("#Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfMoney").val("0,00");
       $("#Phases_0_Budget_BudgetAllocations_" + e + "_AmountOfHours").val("0");

       $("#Phases\\[0\\]_Budget_BudgetAllocations\\[" + e + "\\]_DateEndWorkByDepartment").data("tDatePicker").value(dateToYmd(dat));
      $("#Phases\\[0\\]_Budget_BudgetAllocations\\[" + e + "\\]_DateEndWorkByDepartment").data("tDatePicker").disable(true);
//      document.getElementById("Phases[0]_Budget_BudgetAllocations[" + e + "]_DateEndWorkByDepartment").setAttribute("disabled", "disabled");
        
   
    }
   // alert(e);
}
function MoveEmployeeProjectListToAssignedList(projectEmlpoyeeListControlID, assignedEmlpoyeeListControlID) {
    var x = document.getElementById(projectEmlpoyeeListControlID);
    if (x.selectedIndex >= 0) {
        employee = x.options[x.selectedIndex].text;
        employeeId = x.options[x.selectedIndex].id;
        if ($("#"+assignedEmlpoyeeListControlID+" option[id='"+employeeId+"']").length > 0) {
            alert(employee+" уже в списке");
            return;
        } 
        // новый элемент
        var newItem = document.createElement('option');

        newItem.innerHTML = employee;
        newItem.setAttribute("id", employeeId);
        document.getElementById(assignedEmlpoyeeListControlID).appendChild(newItem);
        x.remove(x.selectedIndex);

        var employeesIdHTML = document.getElementById("div_hidden").innerHTML;
        var divId = "employee" + employeeId;
        var newDiv = document.getElementById(divId);
        if (newDiv != null) {
            newDiv.innerHTML = "<input type='hidden' name='AssignedEmployeeIDs' value='" + employeeId + "' />";
        }
        else {
            var newEmployeesId = employeesIdHTML + "<div id='" + divId + "'><input type='hidden' name='AssignedEmployeeIDs' value='" + employeeId + "' /></div>";
            document.getElementById('div_hidden').innerHTML = newEmployeesId;
        }
    }
    else {
        alert('Выберите исполнителя из списка всех исполнителей');
    }
}

function ShowAllEmployeeList() { 
    document.getElementById("AllEmployeeListLabel").style.fontWeight = "bold";
    document.getElementById("DepartmentEmployeeListLabel").style.fontWeight = "normal";
    document.getElementById("notAssignedEmployees").style.display = "";
    document.getElementById("notAssignedEmployeesDepartment").style.display = "none";
    document.getElementById("addEmployeeButton").onclick = function () { MoveEmployeeProjectListToAssignedList('notAssignedEmployees', 'assignedEmployeeList'); };
    document.getElementById("removeEmployeeButton").onclick = function () { MoveEmployeeAssignedListToProjectList('assignedEmployeeList', 'notAssignedEmployees'); };
    document.getElementById("assignedEmployeeList").ondblclick = function () { MoveEmployeeAssignedListToProjectList('assignedEmployeeList', 'notAssignedEmployees'); };
}

function ShowDepartmentEmployeeList() {
    document.getElementById("DepartmentEmployeeListLabel").style.fontWeight = "bold";
    document.getElementById("AllEmployeeListLabel").style.fontWeight = "normal";
    document.getElementById("notAssignedEmployees").style.display = "none";
    document.getElementById("notAssignedEmployeesDepartment").style.display = "";
    document.getElementById("addEmployeeButton").onclick = function () { MoveEmployeeProjectListToAssignedList('notAssignedEmployeesDepartment', 'assignedEmployeeList'); };
    document.getElementById("removeEmployeeButton").onclick = function () { MoveEmployeeAssignedListToProjectList('assignedEmployeeList', 'notAssignedEmployeesDepartment'); };
    document.getElementById("assignedEmployeeList").ondblclick = function () { MoveEmployeeAssignedListToProjectList('assignedEmployeeList', 'notAssignedEmployeesDepartment'); };
}



function MoveEmployeeAssignedListToProjectList(assignedEmlpoyeeListControlID, projectEmlpoyeeListControlID) {
    var x = document.getElementById(assignedEmlpoyeeListControlID);
    if (x.selectedIndex >= 0) {
        employee = x.options[x.selectedIndex].text;
        employeeId = x.options[x.selectedIndex].id;
        // новый элемент
        var newItem = document.createElement('option');
        newItem.innerHTML = employee;
        newItem.setAttribute("id", employeeId);
        document.getElementById(projectEmlpoyeeListControlID).appendChild(newItem);
        x.remove(x.selectedIndex);

        var divId = "employee" + employeeId;
        var delDiv = document.getElementById(divId);
        if (delDiv != null) {
            delDiv.innerHTML = "";
        }
    }
    else
    { alert('Выберите исполнителя из списка назначеных'); }
}

function AddPhase()
{
    var trCount = jQuery('tr.phase', $('table#phases')).length;
    var requestDate = document.getElementById("Date").value;
    var rowNumber = trCount + 1;
    var addMethodName = "AddBudgetAllocation('" + trCount + "')";
    var delMethodName = "DeleteDepartment('" + trCount + "')";
	var text = "<tr class='phase' id='phase" + trCount + "'><td><label>" + rowNumber + ".</label></td><td><input name='Phases[" + trCount + "].Name' type='text'/></td><td><div class='t-widget t-datepicker'><div class='t-picker-wrap'>" +
                "<input value='' name='Phases[" + trCount + "].DeadLine' id='Date"+ trCount +"' class='t-input'><span class='t-select'>" +
                "<span title='Open the calendar' class='t-icon t-icon-calendar'>Open the calendar</span></span></div></div></td>"+

				 "<td><table id='ba" + trCount + "'><tr><td style='width:100px; '>" +
				 "<select  name='Phases[" + trCount + "].Budget.BudgetAllocations[0].Department.ID'>" +
                        "<option value='1'>КД</option>" +
						"<option value='2'>ДИТ</option>" +
						"<option value='3'>ПКД</option>" +
                        "<option value='4'>ДСА</option>" +
                        "<option value='6'>ПД</option>" +
                        "<option value='7'>ДР</option>" +
                        "<option value='8'>ДC</option>" +
                        "</select></td>" +
                "<td style='width:150px;'><input name='Phases[" + trCount + "].Budget.BudgetAllocations[0].AmountOfHours' type='text' value='0'/></td>" +
                "<td style='width:150px;'><input name='Phases[" + trCount + "].Budget.BudgetAllocations[0].AmountOfMoney' type='text' value='0,00'/></td></tr></table>" +
                "<td><input type='button' onclick=" + addMethodName + " value='+' />" +
                "<input type='button' onclick=" + delMethodName + "  value='-' />" +
				 "</td></tr>";
	 $("tbody#phase").append(text);
	 $('#Date' + trCount).tDatePicker({ format: 'dd.MM.yyyy', minValue: new Date(1899, 11, 31), maxValue: new Date(2100, 00, 01), selectedValue: requestDate }); // new Date(2011, 01, 01) });
}

function AddBudgetAllocation(rowNumber) {
    var tablePsevdoName = 'table#ba' + rowNumber;
    var trCount = rowNumber; //$('tr', $('table#phases')).length;
    var baIndex = $('tr', $(tablePsevdoName)).length;
    $(tablePsevdoName).append("<tr id='ba" + trCount + baIndex + "'><td style='width:100px;'>" +
						"<select name='Phases[" + trCount + "].Budget.BudgetAllocations[" + baIndex + "].Department.ID'  >" +
                        "<option value='1'>КД</option>" +
						"<option value='2'>ДИТ</option>" +
						"<option value='3'>ПКД</option>" +
                        "<option value='4'>ДСА</option>" +
                        "<option value='6'>ПД</option>" +
                        "<option value='7'>ДР</option>" +
                        "<option value='8'>ДC</option>" +
                        "<option value='9'>ЭК</option>" +
                        "</select></td><td style='width:150px;'><input id='Phases_" + trCount + "_Budget_BudgetAllocations_" + baIndex + "_AmountOfHours'" +
                        " name='Phases[" + trCount + "].Budget.BudgetAllocations[" + baIndex + "].AmountOfHours' type='text' value='0' onchange='ValidateInputAmountOfHours("+ baIndex +")' />" +
                        "</td><td style='width:150px;'><input id='Phases_" + trCount + "_Budget_BudgetAllocations_" + baIndex + "_AmountOfMoney'" +
                        " name='Phases[" + trCount + "].Budget.BudgetAllocations[" + baIndex + "].AmountOfMoney' type='text' value='0,00'  onchange='ValidateInputAmountOfMoney("+ baIndex +")'/></td></tr>");

}

function AddBudgetAllocationNew() {
    var tablePsevdoName = 'table#budgetAllocation';
    var baIndex = $('tr', $(tablePsevdoName)).length - 1;
    var dat = new Date();
    
 $(tablePsevdoName).append("<tr id='ba" + baIndex + "'><td style='width:100px;'>" +
						"<select name='Phases[0].Budget.BudgetAllocations[" + baIndex + "].Department.ID' id='BudgetAllocations[" + baIndex + "]'>" +
                        "<option value='1'>КД</option>" +
						"<option value='2'>ДИТ</option>" +
						"<option value='3'>ПКД</option>" +
                        "<option value='4'>ДСА</option>" +
                        "<option value='6'>ПД</option>" +
                        "<option value='7'>ДР</option>" +
                        "<option value='8'>ДC</option>" +
                        "<option value='9'>ЭК</option>" +
                        "</select></td><td style='width:150px;'><input id='Phases_0_Budget_BudgetAllocations_" + baIndex + "_AmountOfHours'" +
                        " name='Phases[0].Budget.BudgetAllocations[" + baIndex + "].AmountOfHours' type='text' value='0' onchange='ValidateInputAmountOfHours(" + baIndex + ")'/>" +
                        "</td>" +
                        "<td style='width:150px;'><input id='Phases_0_Budget_BudgetAllocations_" + baIndex + "_AmountOfMoney'" +
                        " name='Phases[0].Budget.BudgetAllocations[" + baIndex + "].AmountOfMoney' type='text' value='0,00' onchange='ValidateInputAmountOfMoney(" + baIndex + ")' /> </td>" +
                "<td><input  class='t-datepicker' type='text' id='Phases[{0}].DeadLine" + baIndex + "' name='Phases[{0}].DeadLine" + baIndex + "' value='"+dateToYmd(dat)+"'/></td><td align='center' style='width: 300px;'></td></tr>");

        var name_input = document.getElementById('BudgetAllocations[' + baIndex + ']');

      }

    

    function Buttonfunction() {

        var tablePsevdoName = 'table#budgetAllocation';
        var baIndex = $('tr', $(tablePsevdoName)).length - 1;

        for (var i = 0; i < baIndex - 1; i++) {

            var Item = document.getElementById('BudgetAllocations[' + i + ']').value;

            for (var k = i + 1; k < baIndex; k++) {

            var Item2 = document.getElementById('BudgetAllocations[' + k + ']').value;

            if (Item == Item2) {

                alert('Дублирование отделов запрещено!');
                return false;

            }
             
        
            }
    }
    return true;
 }


function DeletePhase() 
{
    var trCount = $('tr.phase', $('table#phases')).length - 1;
    if (trCount >= 0) {
        $('tr', $('table#phases')).remove('#phase' + trCount);
    }
}

function DeleteDepartment(rowNumber) {
    var tablePsevdoName = 'table#ba' + rowNumber;
    var baIndex = $('tr', $(tablePsevdoName)).length-1;
    if (baIndex > 0) 
    {
        var removedRow = '#ba' + rowNumber + baIndex;
        $('tr', $(tablePsevdoName)).remove(removedRow);
    }
}

function DeleteDepartmentNew() {
    var tablePsevdoName = 'table#budgetAllocation'; 
    var baIndex = $('tr', $(tablePsevdoName)).length - 2;
    if (baIndex > 0) {
        var removedRow = '#ba' + baIndex;
        $('tr', $(tablePsevdoName)).remove(removedRow);
    }
}

function ChangeRequestVariant(variant) 
{
    var requests = $('#RequestID').data('tDropDownList');
    $.ajax({
            type: 'POST',
            url: '/ProjectsOverview/GetRequests',
            data: "variant=" + variant,
            success: function (data) { requests.dataBind(data); },
            error: function(xhr, status, errorThrown) {
				alert('An error occurred! ' + ( errorThrown ? errorThrown :xhr.status ));
				}
		});
}

var phases = "";

function ChangeRequestForGetPhase(e) {
    var requestID = $('#RequestID').data('tDropDownList').value();
    var phase = $('#PhaseID').data('tDropDownList');
    $.ajax({
        type: 'POST',
        url: '/ProjectsOverview/GetPhaseShortModel',
        data: "requestID=" + requestID,
        success: function (data) { phase.dataBind(data);},
        error: function (xhr, status, errorThrown) {
            alert('An error occurred! ' + (errorThrown ? errorThrown : xhr.status));
        }
    });
    $.ajax({
        type: 'POST',
        url: '/ProjectsOverview/GetPhaseShortModelForNameList',
        data: "requestID=" + requestID,
        success: function (data) { phases = data; },
        error: function (xhr, status, errorThrown) {
            alert('An error occurred! ' + (errorThrown ? errorThrown : xhr.status));
        }
    });
}

function GetPhase(e) {
    var requestID = $('#RequestID').data('tDropDownList').value();
    var element = document.getElementById("Name");
    var elementCode = document.getElementById("Code");
    var text = "";
   
    $.ajax({
        type: 'POST',
        url: '/ProjectsOverview/GetPhaseShortModelForNameList',
        data: "requestID=" + requestID,
        success: function (data) {
            phases = data; 
            if (phases.length == 1) {
            var phaseName = phases[0].Text;
                    text = phaseName;
                }
            element.value = text;
        },
        error: function (xhr, status, errorThrown) {
            alert('An error occurred! ' + (errorThrown ? errorThrown : xhr.status));
        }
    });
    $.ajax({
        type: 'POST',
        url: '/ProjectsOverview/GetProjectCode',
        data: "requestID=" + requestID,
        success: function (data) {
            codes = data;
            if (codes.length == 1) {
                var code = codes[0].Text;
                text = code;
            }
            elementCode.value = text;
        },
        error: function (xhr, status, errorThrown) {
            alert('An error occurred! ' + (errorThrown ? errorThrown : xhr.status));
        }
    });
}

function GetRequestNumber(e) {
    var typeID = $('#Type_Id').data('tDropDownList').value();
    var element = document.getElementById("Number");
    var contracts = $('#ContractId').data('tDropDownList');
   //
    var text = "";
    
    $.ajax({
        type: 'POST',
        url: '/Requests/GetRequestNumber',
        data: "typeID=" + typeID,
        success: function (data) {
            element.value = data;
        },
        error: function (xhr, status, errorThrown) {
            alert('An error occurred! ' + (errorThrown ? errorThrown : xhr.status));
        }
    });  

       
        if (typeID == 3) {
            AddDetailTable();

            var contractItem = { "Selected": true, "Text": "Проработка ТП", "Value": "3" };
            var contractArray = new Array(contractItem);
            contracts.dataBind(contractArray);
        }
       
        else {

            ClearDeteilTable();
            var contractArray = new Array();
            contracts.dataBind(contractArray);
        }
       //
        if (typeID == 2) {
            document.getElementById("request_docum_label").style.display = "block";
            document.getElementById("request_docum_value").style.display = "block";
        }
        else {
            document.getElementById("request_docum_label").style.display = "none";
            document.getElementById("request_docum_value").style.display = "none";
        }
        
    
    
}

function ChangeCustomerForGetName(e) {
    var typeID = $('#Type_Id').data('tDropDownList').value();
    var customerID = $('#CustomerId').data('tComboBox').value();
    var customer = $('#CustomerId').data('tComboBox');
    var customerName = document.getElementById("CustomerName");
    var checkBoxvalue = document.getElementById('isCustomersWithoutContracts').checked;
    var contracts = $('#ContractId').data('tDropDownList');
    $.ajax({
        type: 'POST',
        url: '/Contracts/GetContractsName',
        data: "customerID=" + customerID,
        success: function (data) {
            if (typeID == 3) {
                var contractItem = { "Selected": true, "Text": "Проработка ТП", "Value": "3" };
                var contractArray = new Array(contractItem);
                contracts.dataBind(contractArray);
            }
            if (checkBoxvalue) {
                if (typeID == 1) {
                    var contractItem = { "Selected": true, "Text": "Административный проект", "Value": "4" }; //Value передаваемое значение ID согласно таблицы ConctractType
                    var contractArray = new Array(contractItem);
                    contracts.dataBind(contractArray);
                }
                if (typeID == 2) {
                    var contractItem = { "Selected": true, "Text": "Проект", "Value": "5" };
                    var contractArray = new Array(contractItem);
                    contracts.dataBind(contractArray);
                }
                if (typeID == 4) {
                    var contractItem = { "Selected": true, "Text": "Производство", "Value": "6" };
                    var contractArray = new Array(contractItem);
                    contracts.dataBind(contractArray);
                }
                if (typeID == 5) {
                    var contractItem = { "Selected": true, "Text": "Обучение", "Value": "7" };
                    var contractArray = new Array(contractItem);
                    contracts.dataBind(contractArray);
                }
                if (typeID == 6) {
                    var contractItem = { "Selected": true, "Text": "Сервисное обслуживание", "Value": "8" };
                    var contractArray = new Array(contractItem);
                    contracts.dataBind(contractArray);
                }
                if (typeID == 7) {
                    var contractItem = { "Selected": true, "Text": "Вызов специалиста", "Value": "9" };
                    var contractArray = new Array(contractItem);
                    contracts.dataBind(contractArray);
                }
            }
            else {               
                contracts.dataBind(data);                
            }


        },
        error: function (xhr, status, errorThrown) {
            alert('An error occurred! ' + (errorThrown ? errorThrown : xhr.status));
        }
    });
    if (customer.selectedIndex != -1) 
    {
        customerName.value = customer.data[customer.selectedIndex].Text;
    }
    else
    {customerName.value = customer.element.value; }
}

function EditRequest() 
{
    var requestDiv = $('div#request');
    requestDiv.remove();
    var content = '<table id=tableDetails style="margin-top: -15px;" class="edit-form">' +
                   '<tbody><tr><td align="left" valign="top" class="project-edit-form-left-cell">'+
                   '<div>'+
                        '<input type="radio" onclick="ChangeRequestVariant("new")" value="new" name="requestVariant" checked="checked" style=" width:10%;"> <label for=""> Новые</label>'+
                        '<input type="radio" onclick="ChangeRequestVariant("old")" value="old" name="requestVariant" style=" width:10%;">  <label for="">Старые</label>'+
                        '<input type="radio" onclick="ChangeRequestVariant("all")" value="all" name="requestVariant" style=" width:10%;">  <label for="">Все</label>'+
                    '</div>'+
                    '</td></tr>'+
                       '<tr valign="top" class="project-edit-form-left-cell">'+
                        '<td><div>'+
                        '<table>'+
                        '<tbody><tr>'+
                            '<td>'+
                               '<div>'+
                                '<label>Заявка:</label>'+
                               '</div>'+
                            '</td>'+
                            '<td>'+
                             '<div style="width:250px;" class="t-widget t-dropdown t-header" tabindex="0"><div class="t-dropdown-wrap t-state-default"><span class="t-input">&nbsp;</span><span class="t-select"><span class="t-icon t-arrow-down">select</span></span></div><input type="text" style="display:none" name="RequestID" id="RequestID"></div>'+
                              '<span id="RequestID_validationMessage" class="field-validation-valid"></span>'+
                            '</td>'+
                         '</tr><tr>'+
                            '<td>'+
                               '<div>'+
                               '<label>Фаза:</label>'+
                               '</div>'+
                            '</td>'+
                            '<td>'+
                            '<div style="width:250px;" class="t-widget t-dropdown t-header" tabindex="0"><div class="t-dropdown-wrap t-state-default"><span class="t-input">&nbsp;</span><span class="t-select"><span class="t-icon t-arrow-down">select</span></span></div><input type="text" style="display:none" name="PhaseID" id="PhaseID"></div>' +
                               '<span id="PhaseID_validationMessage" class="field-validation-valid"></span>'+
                            '</td>'+
                        '</tr>'+
                        '</tbody></table>'+
                        '</div>'+
                        '</td>'+   
                        '</tr>'+
                        '</tbody></table>';
    $('div#requestForEdit').append(content);
}

function AddEmployee() 
{
    var requestDiv = $('div#Employees');
    //..('div.t-toolbar t-grid-toolbar t-grid-top');
    var test = "";

}

function AddDetailTable() 
{
    var content = '<table style="width: 100%;" id=tableDetails> ' +
    '<tbody>'+
    '	<tr>'+
    '        <td style="width:5%;"><label for="">№</label></td>'+
    '        <td style="width:300px;"><label for="">Наименование</label></td>'+
    '        <td align="center" style="width:10%;"><label for="">Необходимое отметить</label></td>'+
    '    </tr>'+
    '    <tr>'+
    '        <td><label for="">1</label></td>'+
	'		<td><label for="">Провести совещание ответственных лиц за проработку заявки</label></td>'+
    '        <td>'+
    '            <input type="checkbox" name="RequestDetails[0].CheckedValue">' +
    '            <input type="hidden" name="RequestDetails[0].RequestDetailTypeID" value="1">'+
    '        </td>'+
    '    </tr>'+
    '    <tr>'+
    '        <td><label for="">2</label></td>'+
    '        <td><label for="">Подготовить предварительную спецификацию</label></td>'+
    '        <td>'+
    '            <input type="checkbox" name="RequestDetails[1].CheckedValue">'+
    '            <input type="hidden" name="RequestDetails[1].RequestDetailTypeID" value="2">'+
    '        </td>'+
	'	</tr>'+
    '    <tr>'+
    '        <td><label for="">3</label></td>'+
    '        <td><label for="">Выдать трудозатраты на разработку проекта, сборку ПД, монтаж, наладку</label></td>'+
    '        <td>'+
    '            <input type="checkbox" name="RequestDetails[2].CheckedValue">'+
    '            <input type="hidden" name="RequestDetails[2].RequestDetailTypeID" value="3">'+
    '        </td>'+
    '    </tr>'+
    '    <tr>'+
    '        <td><label for="">4</label></td>'+
    '        <td><label for="">Подготовить структурную схему</label></td>'+
    '        <td>'+
	'			<input type="checkbox" name="RequestDetails[3].CheckedValue">'+
    '            <input type="hidden" name="RequestDetails[3].RequestDetailTypeID" value="4">'+
    '        </td>'+
    '    </tr>'+
    '    <tr>'+
    '        <td><label for="">5</label></td>'+
    '        <td><label for="">Подготовить пояснительную записку</label></td>'+
    '        <td>'+
	'			<input type="checkbox" name="RequestDetails[4].CheckedValue">'+
    '            <input type="hidden" name="RequestDetails[4].RequestDetailTypeID" value="5">'+
	'		</td>'+
    '    </tr>'+
    '    <tr>'+
    '        <td><label for="">6</label></td>'+
    '        <td><label for="">Подготовить презентацию</label></td>'+
    '        <td>'+
	'			<input type="checkbox" name="RequestDetails[5].CheckedValue">'+
    '            <input type="hidden" name="RequestDetails[5].RequestDetailTypeID" value="6">'+
    '        </td>'+
    '    </tr>'+
    '</tbody>' +
'</table>';
    $('div#detailTable').append(content);
}

function ClearDeteilTable() 
{
    var requestDiv = $('table#tableDetails');
        requestDiv.remove();
}

function TaskCloseConfirm(taskId) 
{
    var result = confirm("Вы уверены что хотите закрыть задачу?");
    if (result == true)
    {
        var ref = location.host;
        location.href = 'http://' + ref + '/Task/CloseTask?taskID=' + taskId;
    }
}

function ProjectCloseConfirm(projectId) {
    var result = confirm("Вы уверены что хотите закрыть проект?\nЗакрытие проекта приведёт к закрытию всех открытых задач по данному проекту.");
    if (result == true) {
        var ref = location.host;
        location.href = 'http://' + ref + '/ProjectDetails/CloseProject?projectID=' + projectId;
    }
}

//my
function RequestCloseConfirm(requestId)
 {
    var result = confirm("Вы уверены что хотите закрыть заявку?\nЗакрытие заявки приведёт к закрытию проекта и всех открытых задач по данному проекту.");
    if (result == true) {
        var ref = location.host;
        location.href = 'http://' + ref + '/Requests/RequestStatusClose?RequestId=' + requestId;
        }
   
}
//my



function ProjectCloseConfirm2(projectId) {
    var result = prompt("Вы уверены что хотите закрыть проект?\nЗакрытие проекта приведёт к закрытию всех открытых задач по данному проекту.");
    if (result == true) {
        var ref = location.host;
        location.href = 'http://' + ref + '/ProjectDetails/CloseProject?projectID=' + projectId;
    }
}

function ValidateInputAmountOfMoney(budgetNumber) {
    var elementId = '#Phases_0_Budget_BudgetAllocations_' + budgetNumber + '_AmountOfMoney' ;
    var element = $(elementId)[0];
    if ($(element).length)
        if (element.value == '')
            element.value = '0,00';  
}

function ValidateInputAmountOfHours(budgetNumber) {
    var elementId = '#Phases_0_Budget_BudgetAllocations_' + budgetNumber + '_AmountOfHours' ;
    var element = $(elementId)[0];
    if ($(element).length)
        if (element.value == '')
            element.value = '0';
}

function ValidateInputProjectName() {
    var elementId = '#Phases_0__Name';
     var validationElement = $('#ProjectNameValidation')[0];
     var element = $(elementId)[0];
    if ($(element).length)
        if (element.value == '')
            validationElement.textContent = 'Поле "Наименование проекта" осталось незаполненым.';
        else
            validationElement.textContent = '';
    if ($('#ProjectNameValidation') != null) {
        var validationElement = $('#ProjectNameValidation')[0];
        var element = $(elementId)[0];
        if ($(element).length)
            if (element.value == '')
                validationElement.textContent = 'Поле "Наименование проекта" осталось незаполненым.';
            else
                validationElement.textContent = '';
    }
}