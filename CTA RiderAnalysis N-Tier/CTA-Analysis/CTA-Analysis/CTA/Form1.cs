using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessTier;

using System.Data.SqlClient;

namespace CTA
{

    public partial class Form1 : Form
    {
        private string BuildConnectionString()
        {
            string version = "MSSQLLocalDB";
            string filename = this.txtDatabaseFilename.Text;

            string connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", version, filename);

            return connectionInfo;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //
            // setup GUI:
            //
            this.lstStations.Items.Add("");
            this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
            this.lstStations.Items.Add("");

            this.lstStations.ClearSelected();

            toolStripStatusLabel1.Text = string.Format("Number of stations:  0");

            try
            {
                string filename = this.txtDatabaseFilename.Text;
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(filename);

                bizTier.TestConnection();
            }
            catch
            {
                //
                // ignore any exception that occurs, goal is just to startup
                //
            }
            finally
            {
            }
        }


        //
        // File>>Exit:
        //
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //
        // File>>Load Stations:
        //
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            try
            {
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                var stations = bizTier.GetStations();

                foreach (CTAStation station in stations)
                {
                    this.lstStations.Items.Add(station.Name);
                }
                this.toolStripStatusLabel1.Text = string.Format("Number of station: {0:#,##0}", stations.Count);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
            }
        }


        //
        // User has clicked on a station for more info:
        //
        private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
                return;

            //
            // clear GUI in case this fails:
            //
            ClearStationUI();

            //
            // now display info about selected station:
            //
            string stationName = this.lstStations.Text;
            stationName = stationName.Replace("'", "''");

            SqlConnection db = null;

            try
            {
                db = new SqlConnection(BuildConnectionString());
                db.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = db;

                //
                // We need total overall ridership for %:
                //
                string sql = string.Format(@"
SELECT Sum(Convert(bigint,DailyTotal)) As TotalOverall
FROM Riderships;
");

                //MessageBox.Show(sql);

                cmd.CommandText = sql;
                object result = cmd.ExecuteScalar();
                long totalOverall = Convert.ToInt64(result);

                // 
                // now we need total and avg for this station:
                //
                sql = string.Format(@"
SELECT Sum(DailyTotal) As TotalRiders, 
       Avg(DailyTotal) As AvgRiders
FROM Riderships
INNER JOIN Stations ON Riderships.StationID = Stations.StationID
WHERE Name = '{0}';
", stationName);

                //MessageBox.Show(sql);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                cmd.CommandText = sql;
                adapter.Fill(ds);

                System.Diagnostics.Debug.Assert(ds.Tables["TABLE"].Rows.Count == 1);
                DataRow R = ds.Tables["TABLE"].Rows[0];

                int stationTotal = Convert.ToInt32(R["TotalRiders"]);
                double stationAvg = Convert.ToDouble(R["AvgRiders"]);
                double percentage = ((double)stationTotal) / totalOverall * 100.0;


                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                this.txtTotalRidership.Text = bizTier.getTotalRidership(stationName).ToString("#,##0");
                this.txtAvgDailyRidership.Text = bizTier.getAvgRidership(stationName).ToString("#,##0/day");
                this.txtPercentRidership.Text = bizTier.getPercentage(stationName).ToString("0.00%");

                //
                // now ridership values for Weekday, Saturday, and
                // sunday/holiday:
                //
                sql = string.Format(@"
SELECT Riderships.StationID, TypeOfDay, Sum(DailyTotal) AS Total
FROM Stations
INNER JOIN Riderships
ON Stations.StationID = Riderships.StationID
WHERE Name = '{0}'
GROUP BY Riderships.TypeOfDay, Riderships.StationID
ORDER BY Riderships.TypeOfDay;
", stationName);

                //MessageBox.Show(sql);

                ds.Clear();

                cmd.CommandText = sql;
                adapter.Fill(ds);

                //
                // we should get back 3 rows:
                //   row 0:  "A" for saturday
                //   row 1:  "U" for sunday/holiday
                //   row 2:  "W" for weekday
                //
                System.Diagnostics.Debug.Assert(ds.Tables["TABLE"].Rows.Count == 3);

                DataRow R1 = ds.Tables["TABLE"].Rows[0];
                DataRow R2 = ds.Tables["TABLE"].Rows[1];
                DataRow R3 = ds.Tables["TABLE"].Rows[2];

                int stationID = Convert.ToInt32(R1["StationID"]);  // all rows have same station ID:

                this.txtStationID.Text = bizTier.getStationID(stationName).ToString();
                this.txtWeekdayRidership.Text = bizTier.getWeekdayRidership(stationName).ToString("#,##0");
                this.txtSaturdayRidership.Text = bizTier.getSaturdayRidership(stationName).ToString("#,##0");
                this.txtSundayHolidayRidership.Text = bizTier.getSundayRidership(stationName).ToString("#,##0");

                //
                // finally, what stops do we have at this station?
                //
                ds.Clear();

                var stops = bizTier.GetStops(stationID);

                foreach (CTAStop stop in stops)
                {
                    this.lstStops.Items.Add(stop.Name);
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                if (db != null && db.State == ConnectionState.Open)
                    db.Close();
            }
        }

        private void ClearStationUI(bool clearStatations = false)
        {
            ClearStopUI();

            this.txtTotalRidership.Clear();
            this.txtTotalRidership.Refresh();

            this.txtAvgDailyRidership.Clear();
            this.txtAvgDailyRidership.Refresh();

            this.txtPercentRidership.Clear();
            this.txtPercentRidership.Refresh();

            this.txtStationID.Clear();
            this.txtStationID.Refresh();

            this.txtWeekdayRidership.Clear();
            this.txtWeekdayRidership.Refresh();
            this.txtSaturdayRidership.Clear();
            this.txtSaturdayRidership.Refresh();
            this.txtSundayHolidayRidership.Clear();
            this.txtSundayHolidayRidership.Refresh();

            this.lstStops.Items.Clear();
            this.lstStops.Refresh();

            if (clearStatations)
            {
                this.lstStations.Items.Clear();
                this.lstStations.Refresh();
            }
        }
        private void ClearFindUI(bool clearStatation = false)
        {
            ClearFindUI();

            this.textBox1.Clear();
            this.textBox1.Refresh();


            this.txtAvgDailyRidership.Clear();
            this.txtAvgDailyRidership.Refresh();

            this.listBox1.Items.Clear();
            this.listBox1.Refresh();
        }



        //
        // user has clicked on a stop for more info:
        //
        private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
                return;

            //
            // clear GUI in case this fails:
            //
            ClearStopUI();

            //
            // now display info about this stop:
            //
            string stopName = this.lstStops.Text;
            stopName = stopName.Replace("'", "''");

            SqlConnection db = null;

            try
            {
                db = new SqlConnection(BuildConnectionString());
                db.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = db;

                //
                // Let's get some info about the stop:
                //
                // NOTE: we want to use station id, not stop name,
                // because stop name is not unique.  Example: the
                // stop "Damen (Loop-bound)".s
                //
                string sql = string.Format(@"
SELECT StopID, Direction, ADA, Latitude, Longitude
FROM Stops
WHERE Name = '{0}' AND
      StationID = {1};
", stopName, this.txtStationID.Text);

                //MessageBox.Show(sql);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();

                cmd.CommandText = sql;
                adapter.Fill(ds);

                System.Diagnostics.Debug.Assert(ds.Tables["TABLE"].Rows.Count == 1);
                DataRow R = ds.Tables["TABLE"].Rows[0];


                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                // handicap accessible?
                this.txtAccessible.Text = bizTier.getHandicapInfo(stopName).ToString();

                // direction of travel:
                this.txtDirection.Text = bizTier.getDirection(stopName).ToString();

                // lat/long position:
                this.txtLocation.Text = bizTier.getLocation(stopName).ToString();

                //
                // now we need to know what lines are associated 
                // with this stop:
                //
                int stopID = Convert.ToInt32(R["StopID"]);

                sql = string.Format(@"
SELECT Color
FROM Lines
INNER JOIN StopDetails ON Lines.LineID = StopDetails.LineID
INNER JOIN Stops ON StopDetails.StopID = Stops.StopID
WHERE Stops.StopID = {0}
ORDER BY Color ASC;
", stopID);

                //MessageBox.Show(sql);

                ds.Clear();
                
                cmd.CommandText = sql;
                adapter.Fill(ds);

                // lines for each stop:
                var stops = bizTier.getLines(stopName);

                foreach (var stop in stops)
                {
                    this.lstLines.Items.Add(stop);
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
                if (db != null && db.State == ConnectionState.Open)
                    db.Close();
            }
        }

        private void ClearStopUI()
        {
            this.txtAccessible.Clear();
            this.txtAccessible.Refresh();

            this.txtDirection.Clear();
            this.txtDirection.Refresh();

            this.txtLocation.Clear();
            this.txtLocation.Refresh();

            this.lstLines.Items.Clear();
            this.lstLines.Refresh();
        }


        //
        // Top-10 stations in terms of ridership:
        //
        private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            try
            {
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                var top = bizTier.GetTopStations(10);

                foreach (var top10 in top)
                {
                    this.lstStations.Items.Add(top10.Name);
                }
                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", top.Count);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
    //        ClearFindUI(true /*clear stations*/);
            
            try
            {
                BusinessTier.Business bizTier;
                bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                string name = this.textBox1.Text;
                var lines = bizTier.findStations(name);

                foreach (var line in lines)
                    this.listBox1.Items.Add(line);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
            finally
            {
            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string stop = this.lstStops.Text;

            BusinessTier.Business bizTier;
            bizTier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

            this.txtAccessible.Text = bizTier.updateHandicap(stop).ToString();

        }
    }//class
}//namespace
