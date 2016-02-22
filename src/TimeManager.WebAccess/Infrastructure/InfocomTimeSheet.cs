namespace Infocom.TimeManager.WebAccess.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    using Infocom.TimeManager.WebAccess.Extensions;
    using Infocom.TimeManager.WebAccess.ViewModels;

    public class InfocomTimeSheet
    {
   
        #region Public Method
        public static ICollection<RealSpentTimeModel> GetSpentTimeOfWeekByDay(InfocomServer server, string userLogin, DateTime date)
        {
            ICollection<RealSpentTimeModel> result = new List<RealSpentTimeModel>();

            var connection = GetConnection(server);
            using (connection)
            {
                try
                {
                     connection.Open();
                    var reader = GetSpentTimeByPeriod(connection, userLogin, date, date.AddDays(6));
                    while (reader.Read())
                    {
                        var spentTime = new RealSpentTimeModel();
                        var time = new DateTime();
                        if (!reader.IsDBNull(1))
                        {
                            time = new DateTime().AddMinutes(double.Parse(reader.GetInt32(1).ToString()));
                        }
                        bool weekEnd = false;
                        if (!reader.IsDBNull(2) && reader.GetDateTime(2).Hour == 0)
                            weekEnd = true;
                        spentTime = new RealSpentTimeModel { SpentTime = new TimeSpan(0, time.Hour, time.Minute, 0), StartDate = reader.GetDateTime(0).Date, isWeekEnd = weekEnd };
                        result.Add(spentTime);
                    }
                    reader.Close();
                    connection.Close();
                }

                catch
                {
                    var spentTime = new RealSpentTimeModel();
                    spentTime = new RealSpentTimeModel { SpentTime = new TimeSpan(), StartDate = date };
                    result.Add(spentTime);

                }

            }
            return result;
        }

        public static TimeSpan GetSpentTimeOfMonth(InfocomServer server, string userLogin, DateTime date)
        {
            var fromDate = date.FirstDayOfMonth();
            var toDate = date.LastDayOfMonth();//.AddDays(-1).AddMilliseconds(1);
            var result = new DateTime();
            var connection = GetConnection(server);
            using (connection)
            {
                try
                {
                    connection.Open();
                    var reader = GetSpentTimeByPeriod(connection, userLogin, fromDate, toDate);

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(1))
                            result = result.AddMinutes(double.Parse(reader.GetInt32(1).ToString()));
                    }
                    reader.Close();

                    connection.Close();
                }
                catch
                {
                    result = result.AddMilliseconds(1);
                }
            }
            return new TimeSpan(result.Ticks);
        }
        #endregion

        #region Private Method

        private static SqlConnection GetConnection(InfocomServer server)
        {
            var connection = new SqlConnection();

            connection.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["WorkTime"].ConnectionString;
            
            return connection;
        }

        private static SqlDataReader GetSpentTimeByPeriod(SqlConnection connection, string userLogin, DateTime fromDate, DateTime toDate)
        {
            var cmd = new SqlCommand("pr_get_total_time_daily", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@i_login", SqlDbType.NVarChar, 200);
            cmd.Parameters.Add("@i_start_date", SqlDbType.DateTime);
            cmd.Parameters.Add("@i_finish_date", SqlDbType.DateTime);
            cmd.Parameters["@i_login"].Value = userLogin.ToLower().Replace("infocom-ltd\\", "");
            cmd.Parameters["@i_start_date"].Value = fromDate;
            cmd.Parameters["@i_finish_date"].Value = toDate;
            return cmd.ExecuteReader();

        }
        #endregion

    }
}