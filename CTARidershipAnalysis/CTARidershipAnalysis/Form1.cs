using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CTARidershipAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //=================================PART ONE=================================
        private void button1_Click(object sender, EventArgs e)
        {
            string connectionInfo, version, filename;
            SqlConnection db;

            version = "MSSQLLocalDB";
            filename = "CTA.mdf";

            connectionInfo = String.Format(@"
            Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;
            ", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();
            //===========================STATIONS===========================
            string sql = string.Format(@"
            SELECT Name
            FROM Stations
            ORDER BY Name;", version, filename);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cmd.CommandText = sql;
            adapter.Fill(ds);
            //=========================NUM STATIONS==========================
            string sql2 = string.Format(@"
            SELECT Count(*) AS Number
            FROM Stations;", version, filename);

            SqlCommand cmd1 = new SqlCommand();
            cmd1 = new SqlCommand();
            cmd1.Connection = db;
            cmd1.CommandText = sql2;
            object result1 = cmd1.ExecuteScalar();
            string msg1 = String.Format("{0:#,##0}", result1);
            this.listBox11.Items.Add(msg1);

            db.Close();

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string name = Convert.ToString(row["Name"]);
                name = name.Replace("'", "''");
                this.listBox1.Items.Add(name);
            }
        }
        //=================================PART TWO=================================
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionInfo, version, filename;
            SqlConnection db;

            version = "MSSQLLocalDB";
            filename = "CTA.mdf";

            connectionInfo = String.Format(@"
            Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;
            ", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();

            //====================TOTAL RIDERS FOR ONE STATION====================
            string sql1 = string.Format(@"
            SELECT SUM(CONVERT(bigint,DailyTotal)) 
            FROM Stations, Riderships
            WHERE Stations.StationID = Riderships.StationID
            AND Stations.Name = '{0}';", listBox1.SelectedItem, filename);

            SqlCommand cmd1 = new SqlCommand();
            cmd1 = new SqlCommand();
            cmd1.Connection = db;
            cmd1.CommandText = sql1;
            object result1 = cmd1.ExecuteScalar();
            string msg1 = String.Format("{0:#,##0}", result1);
            this.textBox1.Text = msg1;
            //===================AVERAGE RIDERS FOR ONE STATION===================
            string sql2 = string.Format(@"
            SELECT AVG(CONVERT(bigint,DailyTotal)) 
            FROM Stations, Riderships
            WHERE Stations.StationID = Riderships.StationID
            AND Stations.Name = '{0}';", listBox1.SelectedItem, filename);

            SqlCommand cmd2 = new SqlCommand();
            cmd2 = new SqlCommand();
            cmd2.Connection = db;
            cmd2.CommandText = sql2;
            object result2 = cmd2.ExecuteScalar();
            string msg2 = String.Format("{0:#,##0}/day", result2);
            this.textBox2.Text = msg2;
            //========================PERCENTAGE RIDERSHIP========================
            string sql3 = string.Format(@"
            SELECT CAST(a.cnt AS float) / CAST(b.cnt AS float) * 100
            FROM ( SELECT SUM(CONVERT(bigint,DailyTotal)) as cnt
                   FROM Stations,Riderships
                   Where Stations.StationID = Riderships.StationID
                   AND Stations.Name = '{0}') a
            CROSS JOIN ( SELECT SUM(CONVERT(bigint,DailyTotal)) as cnt
                         FROM Stations,Riderships
                         Where Stations.StationID = Riderships.StationID) b;", listBox1.SelectedItem, filename);

            SqlCommand cmd3 = new SqlCommand();
            cmd3 = new SqlCommand();
            cmd3.Connection = db;
            cmd3.CommandText = sql3;
            object result3 = cmd3.ExecuteScalar();
            string msg3 = String.Format("{0:0.00}%", result3);
            this.textBox3.Text = msg3;
            //===========================STOPS NEARBY===========================
            string sql4 = string.Format(@"
            SELECT Stops.Name
            FROM Stations,Stops
            WHERE Stations.StationID = Stops.StationID
            AND Stations.Name = '{0}'
            ORDER BY Name;", listBox1.SelectedItem, filename);

            SqlCommand cmd4 = new SqlCommand();
            cmd4.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd4);
            DataSet ds = new DataSet();
            cmd4.CommandText = sql4;
            adapter.Fill(ds);
            //==========================WEEKDAY RIDERSHIP==========================
            string sql5 = string.Format(@"
            SELECT SUM(DailyTotal)
            FROM Stations,Riderships
            WHERE Stations.StationID = Riderships.StationID
            AND Stations.Name = '{0}'
            AND Riderships.TypeOfDay = 'W';", listBox1.SelectedItem, filename);

            SqlCommand cmd5 = new SqlCommand();
            cmd5 = new SqlCommand();
            cmd5.Connection = db;
            cmd5.CommandText = sql5;
            object result5 = cmd5.ExecuteScalar();
            string msg5 = String.Format("{0:#,##0}", result5);
            this.textBox4.Text = msg5;
            //==========================SATURDAY RIDERSHIP==========================
            string sql6 = string.Format(@"
            SELECT SUM(CONVERT(bigint,DailyTotal))
            FROM Stations,Riderships
            WHERE Stations.StationID = Riderships.StationID
            AND Stations.Name = '{0}'
            AND Riderships.TypeOfDay = 'A';", listBox1.SelectedItem, filename);

            SqlCommand cmd6 = new SqlCommand();
            cmd6 = new SqlCommand();
            cmd6.Connection = db;
            cmd6.CommandText = sql6;
            object result6 = cmd6.ExecuteScalar();
            string msg6 = String.Format("{0:#,##0}", result6);
            this.textBox5.Text = msg6;
            //=========================SUN/HOLIDAY RIDERSHIP=========================
            string sql7 = string.Format(@"
            SELECT SUM(CONVERT(bigint,DailyTotal))
            FROM Stations,Riderships
            WHERE Stations.StationID = Riderships.StationID
            AND Stations.Name = '{0}'
            AND Riderships.TypeOfDay = 'U';", listBox1.SelectedItem, filename);

            SqlCommand cmd7 = new SqlCommand();
            cmd7 = new SqlCommand();
            cmd7.Connection = db;
            cmd7.CommandText = sql7;
            object result7 = cmd7.ExecuteScalar();
            string msg7 = String.Format("{0:#,##0}", result7);
            this.textBox6.Text = msg7;

            db.Close();

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string message = Convert.ToString(row["Name"]);
                this.listBox5.Items.Add(message);
            }
        }

        private void listBox5_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string connectionInfo, version, filename;
            SqlConnection db;

            version = "MSSQLLocalDB";
            filename = "CTA.mdf";

            connectionInfo = String.Format(@"
            Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;
            ", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();

            string name = listBox5.SelectedItem.ToString();
            name = name.Replace("'", "''");


            this.listBox5.Items.Clear();
            this.listBox5.Refresh();

            this.listBox2.Items.Clear();
            this.listBox2.Items.Clear();
            //========================HANDICAP ACCESSIBLE========================
            string sql4 = string.Format(@"
            SELECT CAST ( CASE WHEN Stops.ADA = 'True'
              THEN 'YES'
              ELSE 'NO'
            END AS char) 
           FROM Stations,Stops
           WHERE Stations.StationID = Stops.StationID
           AND Stops.Name = '{0}'", name, filename);

            SqlCommand cmd4 = new SqlCommand();
            cmd4 = new SqlCommand();
            cmd4.Connection = db;
            cmd4.CommandText = sql4;
            object result4 = cmd4.ExecuteScalar();
            string msg4 = String.Format("{0}", result4);
            this.textBox7.Text = msg4;
            //=======================DIRECTION OF TRAVEL=======================
            string sql5 = string.Format(@"
            SELECT Stops.Direction
            FROM Stations,Stops
            WHERE Stations.StationID = Stops.StationID
            AND Stops.Name = '{0}'", name, filename);

            SqlCommand cmd5 = new SqlCommand();
            cmd5 = new SqlCommand();
            cmd5.Connection = db;
            cmd5.CommandText = sql5;
            object result5 = cmd5.ExecuteScalar();
            string msg5 = String.Format("{0}", result5);
            this.textBox8.Text = msg5;
            //============================LOCATION============================
            string sql6 = string.Format(@"
            SELECT Stops.Latitude,Stops.Longitude
            FROM Stations,Stops
            WHERE Stations.StationID = Stops.StationID
            AND Stops.Name = '{0}'", name, filename);

            SqlCommand cmd10 = new SqlCommand();
            cmd10.Connection = db;
            SqlDataAdapter adapter1 = new SqlDataAdapter(cmd10);
            DataSet ds1 = new DataSet();
            cmd10.CommandText = sql6;
            adapter1.Fill(ds1);

            //==============================LINES==============================
            string sql7 = string.Format(@"
            SELECT Lines.Color
            FROM StopDetails,Lines,Stops
            WHERE StopDetails.StopID = Stops.StopID 
            AND StopDetails.LineID = Lines.LineID
            AND Stops.Name = '{0}'
            ORDER BY Name;", name, filename);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cmd.CommandText = sql7;
            adapter.Fill(ds);

            db.Close();

            //string station = "";
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string station = Convert.ToString(row["Color"] + "\n");
                this.listBox2.Items.Add(station);
            }
            //this.textBox11.Text = station + "\n";

            string Latitude = "";
            string Longitude = "";
            foreach (DataRow row in ds1.Tables["TABLE"].Rows)
            {
                Latitude = Convert.ToString(row["Latitude"]);
                Longitude = Convert.ToString(row["Longitude"]);
            }
            this.textBox12.Text = "(" + Latitude + ", " + Longitude + ")";

        }
        //==============================TOP 10==============================
        private void button2_Click(object sender, EventArgs e)
        {
            string connectionInfo, version, filename;
            SqlConnection db;

            version = "MSSQLLocalDB";
            filename = "CTA.mdf";

            connectionInfo = String.Format(@"
            Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;
            ", version, filename);
            db = new SqlConnection(connectionInfo);
            db.Open();

            string sql = string.Format(@"
            SELECT TOP 10 Stations.Name, SUM(CONVERT(bigint,DailyTotal)) AS Ridership
            FROM Stations,Riderships
            WHERE Stations.StationID = Riderships.StationID
            GROUP BY Stations.Name
            ORDER BY Ridership DESC;", version, filename);

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            cmd.CommandText = sql;
            adapter.Fill(ds);

            db.Close();

            string station = "";
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                station += Convert.ToString(row["Name"]) + ", " + Convert.ToString(row["Ridership"]) + "\n";
            }
            MessageBox.Show(station);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox12_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox11_SelectedIndexChanged_2(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
