USE [TimeManager]
GO
CREATE SCHEMA [reports] AUTHORIZATION [IIS_TimeManager]
GO
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [reports].[prProjectsWithEmloyeeTimeRecords] 
@startDate AS datetime,
@finishDate AS datetime,
@departmentID AS bigint
AS
BEGIN
SELECT       Employee.ID AS EmployeeID
				,Employee.LastName+' '+Left(Employee.FirstName,1)+'.'+Left(Employee.PatronymicName,1)+'.' AS FIO 
				,ProjectType.Name AS ProjectTypeName
				,Project.Name AS ProjectName 
				,Task.Name As TaskName
				,TimeRecord.StartDate
				,TimeRecord.SpentTime
FROM         Employee 
			 INNER JOIN
             TimeRecord ON Employee.ID = TimeRecord.EmployeeId 
             INNER JOIN
             Task ON TimeRecord.TaskId = Task.ID 
             INNER JOIN
             Project ON Task.ProjectId = Project.ID 
             INNER JOIN
             ProjectType ON Project.ProjectTypeId = ProjectType.ID 
WHERE   (TimeRecord.SpentTime > 0)
AND		TimeRecord.StartDate>=@startDate
AND		TimeRecord.StartDate<=@finishDate
AND		Employee.DepartmentId= @departmentID
AND		ProjectType.ID>0
UNION
SELECT        Employee.ID
				,Employee.LastName+' '+Left(Employee.FirstName,1)+'.'
				+Left(Employee.PatronymicName,1)+'.' AS FIO ,'','','',null,0
FROM         Employee 
WHERE	Employee.DepartmentId= @departmentID
END

GO
CREATE PROCEDURE [reports].[AllProjectsWithEmployeeTimeRecords] 
@startDate AS datetime,
@finishDate AS datetime,
@departmentID AS bigint
AS
BEGIN
SELECT       Employee.ID AS EmployeeID,Project.ID AS ProjectID
				,Employee.LastName+' '+Left(Employee.FirstName,1)+'.'+Left(Employee.PatronymicName,1)+'.' AS FIO 
				,ProjectType.Name AS ProjectTypeName
				,Project.Name AS ProjectName 
				,Task.Name As TaskName
				,TimeRecord.StartDate
				,TimeRecord.SpentTime
FROM         Employee 
			 INNER JOIN
             TimeRecord ON Employee.ID = TimeRecord.EmployeeId 
             INNER JOIN
             Task ON TimeRecord.TaskId = Task.ID 
             INNER JOIN
             Project ON Task.ProjectId = Project.ID 
             INNER JOIN
             ProjectType ON Project.ProjectTypeId = ProjectType.ID 
WHERE   (TimeRecord.SpentTime > 0)
AND		TimeRecord.StartDate>=@startDate
AND		TimeRecord.StartDate<=@finishDate
AND		Employee.DepartmentId= @departmentID
AND		ProjectType.ID>0
UNION
SELECT        Employee.ID,null, Employee.LastName+' '+Left(Employee.FirstName,1)+'.'
				+Left(Employee.PatronymicName,1)+'.' AS FIO ,'','','',null,0
FROM         Employee 
WHERE	Employee.DepartmentId= @departmentID
UNION
SELECT        null, Project.ID
				, '' AS FIO 
				,ProjectType.Name AS ProjectTypeName
				,Project.Name AS ProjectName 
				,''
				,null
				,0
FROM         Project 
			 INNER JOIN
             ProjectType ON Project.ProjectTypeId = ProjectType.ID 
WHERE
       		 ProjectType.ID>0
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
	    -- Spent Time
	    ,dbo.fnGetSpentTimeByProject(pd.ProjectID) as SpentTimeOnProject
		,pd.AmountOfHours AS BudgetAmountOfHours
		,dbo.fnGetSpentMoneyByProject(pd.ProjectID) as SpentMoneyOnProject
		,CAST(pd.AmountOfMoney as decimal(19,2)) as BudgetAmountOfMoney
		,SUM(dbo.fnBigIntToDecimalHours(pd.SpentTime * pd.EmployeeRate)) as SpentMoneyByPeriod
		,(pd.AmountOfHours - dbo.fnGetSpentTimeByProject(pd.ProjectID)) as ResidueOfAmountBudgetHours
		,SUM(pd.SpentTime) as  SpentHoursByPeriod
		,CAST((pd.AmountOfMoney - dbo.fnGetSpentMoneyByProject(pd.ProjectID))as decimal(19,2)) as ResidueOfAmountBudgetMoney
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
	    ,dbo.fnGetSpentTimeByProject(pd.ProjectID)
	    ,pd.AmountOfHours
	    ,dbo.fnGetSpentMoneyByProject(pd.ProjectID)
	    ,CAST(pd.AmountOfMoney as decimal(19,2))
	    ,(pd.AmountOfHours - dbo.fnGetSpentTimeByProject(pd.ProjectID))
	    ,CAST((pd.AmountOfMoney - dbo.fnGetSpentMoneyByProject(pd.ProjectID))as decimal(19,2))
END



GO



