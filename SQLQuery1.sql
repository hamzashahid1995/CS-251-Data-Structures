SELECT Count(*) AS Number
FROM Stations;

SELECT Name
FROM Stations
ORDER BY Name;

SELECT SUM(CONVERT(bigint,DailyTotal)) 
FROM Stations, Riderships
WHERE Stations.StationID = Riderships.StationID
AND Stations.Name = 'Clark/Lake (Blue; Brown; Green; Orange; Purple & Pink Lines)';

SELECT SUM(CONVERT(bigint,DailyTotal)) 
FROM Stations, Riderships
WHERE Stations.StationID = Riderships.StationID;

SELECT AVG(DailyTotal)
FROM Stations, Riderships
WHERE Stations.StationID = Riderships.StationID
AND Stations.Name = 'Clark/Lake (Blue; Brown; Green; Orange; Purple & Pink Lines)';

SELECT *
FROM ( SELECT SUM(CONVERT(bigint,DailyTotal))
       FROM Stations,Riderships
       Where Stations.StationID = Riderships.StationID
       AND Stations.Name = 'Clark/Lake (Blue; Brown; Green; Orange; Purple & Pink Lines)') AVG(DailyTotal)
CROSS JOIN ( SELECT SUM(CONVERT(bigint,DailyTotal))
             FROM Stations,Riderships
             Where Stations.StationID = Riderships.StationID) SUM(DailyTotal)



SELECT CONVERT(varchar(10),Stops.Name)
FROM Stations,Stops
WHERE Stations.StationID = Stops.StationID
AND Stops.StationID = '40380'
ORDER BY Name;


SELECT SUM(CONVERT(bigint,DailyTotal))
FROM Stops,Riderships
WHERE Stops.StationID = Riderships.StationID
AND Stops.Name = 'Clark/Lake (Inner Loop)'
AND Riderships.TypeOfDay = 'W';


SELECT CAST ( CASE WHEN Stops.ADA = 'True'
              THEN 'YES'
              ELSE 'NO'
            END AS char) 
FROM Stations,Stops
WHERE Stations.StationID = Stops.StationID
AND Stops.Name = 'Clark/Lake (Inner Loop)';
           

SELECT Stops.Direction
FROM Stations,Stops
WHERE Stations.StationID = Stops.StationID
AND Stops.Name = 'Clark/Lake (Inner Loop)';

SELECT Stops.Latitude,Stops.Longitude
FROM Stations,Stops
WHERE Stations.StationID = Stops.StationID
AND Stops.Name = 'Clark/Lake (Inner Loop)';

SELECT Lines.Color
FROM StopDetails,Lines,Stops
WHERE StopDetails.StopID = Stops.StopID 
AND StopDetails.LineID = Lines.LineID
AND Stops.Name = 'Clark/Lake (Inner Loop)'
ORDER BY Name;


