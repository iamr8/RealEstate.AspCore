DECLARE @__p_3 int = 10;

SELECT TOP(@__p_3) [x].[Id], [x].[Audit], [x].[CategoryId], [x].[Description], [x].[PropertyId], [x.Category].[Id], [x.Category].[Audit], [x.Category].[Name], [x.Category].[Type], [x.Property].[Id], [x.Property].[Alley], [x.Property].[Audit], [x.Property].[BuildingName], [x.Property].[CategoryId], [x.Property].[Description], [x.Property].[DistrictId], [x.Property].[Flat], [x.Property].[Floor], [x.Property].[Geolocation], [x.Property].[Number], [x.Property].[Street], [x.Property.Category].[Id], [x.Property.Category].[Audit], [x.Property.Category].[Name], [x.Property.Category].[Type], [x.Property.District].[Id], [x.Property.District].[Audit], [x.Property.District].[Name]
FROM (
    SELECT TOP(1000) I.* FROM Item AS I JOIN(SELECT Id, JSON_VALUE(Audit, '$[0].t') AS Type, JSON_VALUE(Audit, '$[0].d') AS Date FROM Item WHERE ISJSON(Audit) > 0) AS J ON J.Id = I.Id WHERE J.Type != 1 ORDER BY J.Date DESC
) AS [x]
INNER JOIN [Category] AS [x.Category] ON [x].[CategoryId] = [x.Category].[Id]
INNER JOIN [Property] AS [x.Property] ON [x].[PropertyId] = [x.Property].[Id]
LEFT JOIN [Category] AS [x.Property.Category] ON [x.Property].[CategoryId] = [x.Property.Category].[Id]
INNER JOIN [District] AS [x.Property.District] ON [x.Property].[DistrictId] = [x.Property.District].[Id]
WHERE [x].[CategoryId] IN (N'd03ef728-2932-480f-bfa1-11c77b2e8767', N'199acde7-7244-42c8-a6e7-eab43d0a9eee', N'5816a67b-eb6a-464e-b6c9-7f0520eef95e', N'1943a814-7c25-4569-b528-8e96c4b5d3f8') AND [x.Property].[CategoryId] IN (N'91144dc3-70e3-44a8-904c-cc8bf12aaa4c', N'29dfac27-765c-4839-b77b-0afc7b6450e3', N'826e9eaf-40f7-4adc-9a1d-b84ad151e176', N'8f19a3ee-b39a-47f1-84d3-08ca6f62278b')
ORDER BY [x.Property].[Id], [x].[Id] 

--SELECT TOP(1000) I.* FROM Item AS I JOIN(SELECT Id, JSON_VALUE(Audit, '$[0].t') AS Type, JSON_VALUE(Audit, '$[0].d') AS Date FROM Item WHERE ISJSON(Audit) > 0) AS J ON J.Id = I.Id WHERE J.Type != 1 ORDER BY J.Date DESC

--SELECT ROW_NUMBER() OVER ( ORDER BY IV.Date DESC ), * FROM 
--	(SELECT TOP(1000) I.*, J.Date FROM Item AS I
--		JOIN (SELECT Id, JSON_VALUE(Audit, '$[0].t') AS Type, JSON_VALUE(Audit, '$[0].d') AS Date FROM Item WHERE ISJSON(Audit) > 0) AS J
--			ON J.Id = I.Id
--	WHERE J.Type != 3
--	ORDER BY J.Date DESC) AS IV


--SELECT TOP(20) Id, Audit, Description, PropertyId, CategoryId 
--	FROM 
--		(SELECT ROW_NUMBER() OVER ( ORDER BY J.Date DESC ) AS RowNumber, I.*  FROM Item AS I JOIN(SELECT Id, JSON_VALUE(Audit, '$[0].t') AS Type, JSON_VALUE(Audit, '$[0].d') AS Date FROM Item WHERE ISJSON(Audit) > 0) AS J ON J.Id = I.Id) AS RowResult 
--		WHERE RowNumber >= 1 AND RowNumber <= 20

--SELECT TOP(20) *
--	FROM 
--		(SELECT ROW_NUMBER() OVER ( ORDER BY J.Date DESC ) AS RowNumber, I.*  FROM Item AS I
--			JOIN (SELECT Id, JSON_VALUE(Audit, '$[0].t') AS Type, JSON_VALUE(Audit, '$[0].d') AS Date FROM Item WHERE ISJSON(Audit) > 0) AS J ON J.Id = I.Id) AS RowResult 
--		WHERE
--			--CategoryId = (SELECT TOP(1) Id FROM Category where Name = N'پیش فروش') 
--			--AND PropertyId IN (SELECT Id FROM Property WHERE CategoryId = (SELECT TOP(1) Id FROM Category where Name = N'آپارتمان')) AND
--			RowNumber >= 1 AND RowNumber <= 20

--SELECT *
--	FROM Item AS I
--		JOIN (SELECT Id, JSON_VALUE(Audit, '$[0].d') AS Date FROM Item WHERE ISJSON(Audit) > 0) AS J
--			ON J.Id = I.Id
--	WHERE CategoryId = (SELECT TOP(1) Id FROM Category WHERE Name = N'پیش فروش') 
--		AND PropertyId IN (SELECT Id FROM Property WHERE CategoryId = (SELECT TOP(1) Id FROM Category WHERE Name = N'آپارتمان'))

--SELECT * FROM PropertyCategory WHERE CategoryId = (SELECT TOP(1) Id FROM Category where Name = N'آپارتمان')

--SELECT * FROM Property WHERE CategoryId = (SELECT TOP(1) Id FROM Category where Name = N'آپارتمان')

