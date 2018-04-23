//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;



namespace BusinessTier
{

  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    ///
    /// <summary>
    /// Constructs a new instance of the business tier.  The format
    /// of the filename should be either |DataDirectory|\filename.mdf,
    /// or a complete Windows pathname.
    /// </summary>
    /// <param name="DatabaseFilename">Name of database file</param>
    /// 
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    ///
    /// <summary>
    ///  Opens and closes a connection to the database, e.g. to
    ///  startup the server and make sure all is well.
    /// </summary>
    /// <returns>true if successful, false if not</returns>
    /// 
    public bool TestConnection()
    {
      return dataTier.OpenCloseConnection();
    }


    ///
    /// <summary>
    /// Returns all the CTA Stations, ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<CTAStation> GetStations()
    {
      List<CTAStation> stations = new List<CTAStation>();

      try
      {
                //
                // TODO!
                //
                string sql = string.Format(@"
                             SELECT Name 
                             FROM Stations 
                             ORDER BY Name ASC;");
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                int stationID = 0;
                string stationName;
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    stationName = Convert.ToString(row["Name"]);
                    stations.Add(new CTAStation(stationID,stationName));
                }

                
            }
            catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }
            return stations;
    }
        //===================================================================
        public int getTotalRidership (string stationName)
        {

            string sql = string.Format(@"
                         SELECT SUM(CONVERT(bigint, DailyTotal)) AS TotalRiders
                         FROM Stations, Riderships
                         WHERE Stations.StationID = Riderships.StationID
                         AND Stations.Name = '{0}';", stationName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            int totalRiders = Convert.ToInt32(R["TotalRiders"]);

            return totalRiders;
        }
        //===================================================================
        public int getAvgRidership (string stationName)
        {
            string sql = string.Format(@"
                         SELECT AVG(CONVERT(bigint,DailyTotal)) AS Average
                         FROM Stations, Riderships
                         WHERE Stations.StationID = Riderships.StationID
                         AND Stations.Name = '{0}';", stationName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            int average = Convert.ToInt32(R["Average"]);

            return average;
        }
        //===================================================================
        public double getPercentage(string stationName)
        {
            string sql = string.Format(@"
            SELECT CAST(a.cnt AS float) / CAST(b.cnt AS float)  AS Total
            FROM ( SELECT SUM(CONVERT(bigint,DailyTotal)) as cnt
                   FROM Stations,Riderships
                   Where Stations.StationID = Riderships.StationID
                   AND Stations.Name = '{0}') a
            CROSS JOIN ( SELECT SUM(CONVERT(bigint,DailyTotal)) as cnt
                         FROM Stations,Riderships
                         Where Stations.StationID = Riderships.StationID) b;", stationName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            double percentage = Convert.ToDouble(R["Total"]);
            return percentage;
        }
        //===================================================================
        public int getWeekdayRidership(string stationName)
        {
            string sql = string.Format(@"
                         SELECT SUM(DailyTotal) AS WEEKDAY
                         FROM Stations,Riderships
                         WHERE Stations.StationID = Riderships.StationID
                         AND Stations.Name = '{0}'
                         AND Riderships.TypeOfDay = 'W';", stationName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            int weekdayRiders = Convert.ToInt32(R["WEEKDAY"]);

            return weekdayRiders;
        }
        //===================================================================
        public int getSaturdayRidership(string stationName)
        {
            string sql = string.Format(@"
                         SELECT SUM(DailyTotal) AS SATURDAY
                         FROM Stations,Riderships
                         WHERE Stations.StationID = Riderships.StationID
                         AND Stations.Name = '{0}'
                         AND Riderships.TypeOfDay = 'A';", stationName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            int saturdayRiders = Convert.ToInt32(R["SATURDAY"]);

            return saturdayRiders;
        }
        //===================================================================
        public int getSundayRidership(string stationName)
        {
            string sql = string.Format(@"
                         SELECT SUM(DailyTotal) AS SUNDAY_HOLIDAY
                         FROM Stations,Riderships
                         WHERE Stations.StationID = Riderships.StationID
                         AND Stations.Name = '{0}'
                         AND Riderships.TypeOfDay = 'U';", stationName);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            int sundayRiders = Convert.ToInt32(R["SUNDAY_HOLIDAY"]);

            return sundayRiders;
        }
        public int getStationID(string stationName)
        {
            string sql = string.Format(@"
                         SELECT Stations.StationID AS ID
                         FROM Stations, Riderships
                         WHERE Stations.StationID = Riderships.StationID
                         AND Stations.Name = '{0}';", stationName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            int stationID = Convert.ToInt32(R["ID"]);

            return stationID;
        }
    //===================================================================
    ///
    /// <summary>
    /// Returns the CTA Stops associated with a given station,
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStop objects</returns>
    ///
    public IReadOnlyList<CTAStop> GetStops(int stationID)
    {
      List<CTAStop> stops = new List<CTAStop>();

      try
      {
                //
                // TODO!
                //
                string sql = string.Format(@"
                             SELECT * 
                   FROM Stops
                   INNER JOIN Stations ON Stops.StationID = Stations.StationID
                   WHERE Stations.StationID = '{0}'
                   ORDER BY Stops.Name ASC;", stationID);
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                int stopID = 0;
                double latitude = 0.0;
                double longitude = 0.0;
       
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    string stopName = Convert.ToString(row["Name"].ToString());
                    string direction = Convert.ToString(row["Direction"].ToString());
                    bool ada = Convert.ToBoolean(row["ADA"]);
                    stops.Add(new CTAStop(stopID,stopName,stationID,direction,ada,latitude,longitude));
                }
       }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }
      return stops;
    }
        //===================================================================
        public string getHandicapInfo(string stopName)
        {
            string sql = string.Format(@"
            SELECT CAST ( CASE WHEN Stops.ADA = 'True'
              THEN 'YES'
              ELSE 'NO'
            END AS char) AS HANDICAP 
           FROM Stations,Stops
           WHERE Stations.StationID = Stops.StationID
           AND Stops.Name = '{0}'", stopName);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            string handicap = Convert.ToString(R["HANDICAP"]);

            return handicap;
        }
        //===================================================================
        public string getDirection(string stopName)
        {
            string sql = string.Format(@"
                         SELECT Stops.Direction AS DIRECTION
                         FROM Stations,Stops
                         WHERE Stations.StationID = Stops.StationID
                         AND Stops.Name = '{0}'", stopName);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow R = ds.Tables["TABLE"].Rows[0];
            string direction = Convert.ToString(R["DIRECTION"]);

            return direction;
        }
        //===================================================================
        public string getLocation(string stopName)
        {

            string sql = string.Format(@"
                         SELECT Stops.Latitude,Stops.Longitude
                         FROM Stations,Stops
                         WHERE Stations.StationID = Stops.StationID
                         AND Stops.Name = '{0}'", stopName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            string Latitude = "";
            string Longitude = "";
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                Latitude = Convert.ToString(row["Latitude"]);
                Longitude = Convert.ToString(row["Longitude"]);
            }
            return ("(" + Latitude + ", " + Longitude + ")");
        }
        //===================================================================
        public List<string> getLines(string stopName)
        {
            List<string> lines = new List<string>();

            string sql = string.Format(@"
                         SELECT Lines.Color AS COLORS
                         FROM StopDetails,Lines,Stops
                         WHERE StopDetails.StopID = Stops.StopID 
                         AND StopDetails.LineID = Lines.LineID
                         AND Stops.Name = '{0}'
                         ORDER BY Name;", stopName);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
                lines.Add(Convert.ToString(row["COLORS"]));
            
         return lines;
        }
        //===================================================================
        public List<string> findStations(string name)
        {
            List<string> find = new List<string>();

            string sql = string.Format(@"
                         SELECT Stations.Name AS Name
                         FROM Stations
                         WHERE Stations.Name
                         LIKE '%{0}%'", name);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
                find.Add(Convert.ToString(row["Name"]));

            return find;
        }
        //===================================================================
        public string updateHandicap(string stopName)
        {
            try
            {
                string sql = string.Format(@" 
                             UPDATE Stops
                             SET Name = '{0}'
                             WHERE ADA =  0 ", stopName);

                var ds = dataTier.ExecuteActionQuery(sql);
                string handicap = Convert.ToString("HANDICAP");

                return handicap;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.updateHandicap: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
        }
        //===================================================================
    ///
    /// <summary>
    /// Returns the top N CTA Stations by ridership, 
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<CTAStation> GetTopStations(int N)
    {
      if (N < 1)
        throw new ArgumentException("GetTopStations: N must be positive");

      List<CTAStation> stations = new List<CTAStation>();

      try
      {
                //
                // TODO!
                //
                string sql = string.Format(@"
                             SELECT Top 10 Name, Sum(DailyTotal) As TotalRiders 
                             FROM Riderships
                             INNER JOIN Stations ON Riderships.StationID = Stations.StationID 
                             GROUP BY Stations.StationID, Name
                             ORDER BY TotalRiders DESC;");
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                int stationID = 0;
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    string stationName = Convert.ToString(row["Name"]);
                    stations.Add(new CTAStation(stationID,stationName));
                }
            }
      catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    } 
  }//class
}//namespace
