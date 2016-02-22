USE [TimeManager]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE SCHEMA [reports] AUTHORIZATION [IIS_TimeManager]
GO

CREATE FUNCTION [dbo].[fnGetEmployeeShortNameByID]
(
	@ID as bigint
)
RETURNS NVARCHAR(max)
AS
BEGIN
	
	DECLARE @result as NVARCHAR(max)

	SELECT @result = e.LastName + ' ' + LEFT(e.FirstName, 1) + '.' + LEFT(e.PatronymicName, 1) + '.'
	FROM Employee e
	WHERE e.ID = @ID

	RETURN @result

END

GO
CREATE FUNCTION [dbo].[fnBigIntToDecimalHours] 
(
	@bigint as bigint
)
RETURNS decimal(5,2)
AS
BEGIN
	
	RETURN @bigint/36000000000

END

GO
CREATE FUNCTION [dbo].[fnGetSpentMoneyByDepartmentOnProject]
(
	@departmentId as bigint, 
	@projectId as bigint
)
RETURNS decimal (19,2)
AS
BEGIN
	DECLARE @result as decimal (19,2)

	SELECT @result = Cast(Round(sum(tr.SpentTime* e.Rate)/36000000000,2)as decimal (19,2))               
	FROM   dbo.Project AS p 
		inner join Task t
		on t.ProjectId = p.ID
		inner join dbo.TimeRecord tr
		on tr.TaskId = t.ID
		INNER JOIN
				 dbo.Employee AS e ON tr.EmployeeId = e.ID 
		FULL OUTER JOIN
				 dbo.Department AS d ON e.DepartmentId = d.ID 
		--FULL OUTER JOIN
		--		 dbo.Contract AS c ON c.ID = p.ContractId
		--inner join ProjectBudgetAllocation pba
		--	on pba.ProjectId = p.ID 
		--		and pba.DepartmentId=e.DepartmentId
		--		and pba.ProjectBudgetId=p.BudgetId 
	WHERE 
		p.ID = @projectId
		AND
		d.ID = @departmentId
	RETURN @result

END

GO
CREATE FUNCTION [dbo].[fnGetSpentTimeByDepartmentOnProject]
(
	@departmentId as bigint, 
	@projectId as bigint
)
RETURNS decimal (19,2)
AS
BEGIN
	DECLARE @result as decimal (19,2)

	SELECT @result = Cast(Round(sum(tr.SpentTime)/36000000000,2)as decimal (19,2))               
	FROM   dbo.Project AS p 
		inner join Task t
		on t.ProjectId = p.ID
		inner join dbo.TimeRecord tr
		on tr.TaskId = t.ID
		INNER JOIN
				 dbo.Employee AS e ON tr.EmployeeId = e.ID 
		FULL OUTER JOIN
				 dbo.Department AS d ON e.DepartmentId = d.ID 
	WHERE 
		p.ID = @projectId
		AND
		d.ID = @departmentId
	RETURN @result

END

GO
CREATE FUNCTION [dbo].[fnGetEmployeeRowNumber]
(
	@employeeId as bigint
)
RETURNS int
AS
BEGIN
	DECLARE @Result as int
	
    select @Result = et.RowNumber
	from (SELECT TOP 1000 ROW_NUMBER() OVER (ORDER BY dbo.fnGetEmployeeShortNameByID(e.ID)) AS 'RowNumber'
			,e.ID
			FROM [TimeManager].[dbo].[Employee] e
			where [DepartmentId]=(select DepartmentId from Employee where ID = @employeeId)
			order by dbo.fnGetEmployeeShortNameByID(e.ID)) et
	where et.ID = @employeeId

	RETURN @Result
END

GO
CREATE VIEW [dbo].[vwProjectDetails]
AS
BEGIN
SELECT     p.ID, ph.BudgetId, CAST(c.Number AS varchar) + ' от ' + CONVERT(varchar(10), c.SigningDate, 104) AS ContractNumberAndSigningDate, 
                      c.Customer AS ContractCustomer, p.Name AS ProjectName, ph.DeadLine AS ProjectDeadLine, CAST(r.Number AS varchar) + ' от ' + CONVERT(varchar(10), r.Date, 104) 
                      AS ProjectOrder, dbo.fnGetEmployeeShortNameByID(e.ID) AS ManagerShortName, s.Name AS StatusName, dbo.ProjectType.Name AS PorjectTypeName, 
                      p.Code AS ProjectCode
FROM         dbo.Project AS p INNER JOIN
                      dbo.Phase AS ph ON p.PhaseId = ph.ID INNER JOIN
                      dbo.Request AS r ON r.ID = ph.RequestId INNER JOIN
                      dbo.Employee AS e ON r.ProjectManagerId = e.ID INNER JOIN
                      dbo.Status AS s ON p.CurrentStatusId = s.ID INNER JOIN
                      dbo.ProjectType ON p.ProjectTypeId = dbo.ProjectType.ID INNER JOIN
                      dbo.Contract AS c ON c.ID = r.ContractId
GROUP BY p.ID, c.Number, c.SigningDate, c.Customer, p.Name, ph.DeadLine, CAST(r.Number AS varchar) + ' от ' + CONVERT(varchar(10), r.Date, 104), 
                      dbo.fnGetEmployeeShortNameByID(e.ID), ph.BudgetId, s.Name, dbo.ProjectType.Name, p.Code
END

GO
CREATE VIEW [dbo].[vwProjectDetailsWithSpentTimeByBudgetAllocation]
AS
BEGIN
SELECT     pd.ID AS ProjectID, tr.SpentTime, pd.ManagerShortName, dbo.fnGetEmployeeShortNameByID(e.ID) AS EmployeeShortName, tr.StartDate AS TimeRecordStartDate, 
                      pd.ContractNumberAndSigningDate, pd.ContractCustomer, pd.ProjectDeadLine, pd.ProjectName, pd.ProjectOrder, d.ShortName AS DepartmentName, 
                      d.ID AS DepartmentID, t.Name AS TaskName, pba.AmountOfHours, pba.AmountOfMoney, pd.StatusName, pd.PorjectTypeName, pd.ProjectCode, 
                      e.Rate AS EmployeeRate
FROM         dbo.vwProjectDetails AS pd INNER JOIN
                      dbo.ProjectBudgetAllocation AS pba ON pba.ProjectBudgetId = pd.BudgetId INNER JOIN
                      dbo.Task AS t ON t.ProjectId = pd.ID INNER JOIN
                      dbo.TimeRecord AS tr ON tr.TaskId = t.ID INNER JOIN
                      dbo.Employee AS e ON tr.EmployeeId = e.ID AND pba.DepartmentId = e.DepartmentId INNER JOIN
                      dbo.Department AS d ON e.DepartmentId = d.ID
END

GO
CREATE PROCEDURE [reports].[prProjectsWithEmloyeeTimeRecords] 
@startDate AS datetime,
@finishDate AS datetime,
@departmentID AS bigint
AS
BEGIN
SELECT     dbo.fnGetEmployeeRowNumber([Employee].[ID]) AS 'RowNumber'
			,[Employee].[ID] AS EmployeeID
            ,dbo.fnGetEmployeeShortNameByID([Employee].[ID]) AS FIO
            ,[ProjectType].[Name] AS ProjectTypeName
            ,[Status].[Name] AS StatusName
            ,[Project].[Name] AS ProjectName
            ,[Task].[Name] As TaskName
            ,[TimeRecord].[StartDate]
            ,[TimeRecord].[SpentTime]
FROM         [Employee] 
                   INNER JOIN
             [TimeRecord] ON [Employee].[ID] = [TimeRecord].[EmployeeId] 
             INNER JOIN
             [Task] ON [TimeRecord].[TaskId] = [Task].[ID] 
             INNER JOIN
             [Project] ON [Task].[ProjectId] = [Project].[ID]
             INNER JOIN
             [Status] ON [Status].[ID] = [Project].[CurrentStatusId]   
             INNER JOIN
             [ProjectType] ON [Project].[ProjectTypeId] = [ProjectType].[ID]
WHERE   ([TimeRecord].[SpentTime] > 0)
AND         [TimeRecord].[StartDate]>=@startDate
AND         [TimeRecord].[StartDate]<=@finishDate
AND         [Employee].[DepartmentId] = @departmentID
AND         [ProjectType].[ID] >0
UNION
SELECT      dbo.fnGetEmployeeRowNumber([Employee].[ID])
			, [Employee].[ID]
            ,dbo.fnGetEmployeeShortNameByID([Employee].[ID]) AS FIO,'','','','',null,0
FROM         [Employee] 
WHERE [Employee].[DepartmentId] = @departmentID
END

GO
CREATE PROCEDURE [reports].[prTimeAndBudgetSpendingByDepartments]
@startDate AS datetime,
@finishDate AS datetime
AS
BEGIN
SELECT
		 pd.StatusName,
		 pd.PorjectTypeName
		,pd.ContractNumberAndSigningDate
		,pd.ContractCustomer
	    ,pd.ProjectName
	    ,pd.ProjectCode
	    ,pd.ProjectDeadLine
	    ,pd.ProjectOrder 
	    ,pd.ManagerShortName
		,pd.DepartmentName 
	    ,pd.EmployeeShortName 
	    ,pd.TaskName
	    ,pd.AmountOfHours AS BudgetAmountOfHours
	    ,CAST(Round([dbo].[fnGetSpentTimeByDepartmentOnProject](pd.DepartmentID, pd.ProjectID),0)AS INT) as SpentTimeOnProject
		,CAST(Round((pd.AmountOfHours - [dbo].[fnGetSpentTimeByDepartmentOnProject](pd.DepartmentID, pd.ProjectID)),0)AS INT) as ResidueOfAmountBudgetHours
		
		,CAST(pd.AmountOfMoney as decimal(19,2)) as BudgetAmountOfMoney
		,[dbo].[fnGetSpentMoneyByDepartmentOnProject](pd.DepartmentID, pd.ProjectID) as SpentMoneyOnProject
		,CAST((pd.AmountOfMoney - [dbo].[fnGetSpentMoneyByDepartmentOnProject](pd.DepartmentID, pd.ProjectID))as decimal(19,2)) as ResidueOfAmountBudgetMoney
		
		,SUM(pd.SpentTime) as  SpentHoursByPeriod
		,SUM(dbo.fnBigIntToDecimalHours(pd.SpentTime * pd.EmployeeRate)) as SpentMoneyByPeriod
	FROM
		dbo.vwProjectDetailsWithSpentTimeByBudgetAllocation pd
	WHERE   
		pd.TimeRecordStartDate between @startDate and @finishDate
	GROUP BY 
		 pd.StatusName
		,pd.PorjectTypeName
	    ,pd.ContractNumberAndSigningDate
		,pd.ContractCustomer
	    ,pd.ProjectName
	    ,pd.ProjectCode
	    ,pd.ProjectDeadLine
	    ,pd.ProjectOrder 
	    ,pd.ManagerShortName
		,pd.DepartmentName 
	    ,pd.EmployeeShortName
	    ,pd.TaskName 
	    ,[dbo].[fnGetSpentTimeByDepartmentOnProject](pd.DepartmentID,pd.ProjectID)
	    ,pd.AmountOfHours
	    ,[dbo].[fnGetSpentMoneyByDepartmentOnProject](pd.DepartmentID, pd.ProjectID)
	    ,CAST(pd.AmountOfMoney as decimal(19,2))
	    --,(pd.AmountOfHours - dbo.fnGetSpentTimeByProject(pd.ProjectID))
	    ,CAST((pd.AmountOfMoney - [dbo].[fnGetSpentMoneyByDepartmentOnProject](pd.DepartmentID, pd.ProjectID))as decimal(19,2))
END
