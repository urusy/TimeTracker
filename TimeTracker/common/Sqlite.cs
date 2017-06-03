using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace TimeTracker.common
{
    public class Sqlite
    {
        // アプリデータパス
        private static readonly string APP_DATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TimeTracker\";
        // DBファイル名
        private const string DB_FILE_NAME = "TimeTracker.db";
        // テーブル名:統計
        private const string TBL_STTCS = "statictics";

        public static bool SetupDatabase()
        {
            bool ret = false;

            try
            {
                // フォルダが存在するか確認
                if (Directory.Exists(APP_DATA_PATH))
                {
                    // フォルダが存在する場合、ファイルを確認
                    if (!File.Exists(APP_DATA_PATH + DB_FILE_NAME))
                    {
                        // ファイルが存在しない場合、DB作成
                        CreateDb();
                    }
                }
                else
                {
                    // フォルダが存在しない場合、作成
                    Directory.CreateDirectory(APP_DATA_PATH);

                    // DBを作成する
                    CreateDb();
                }
                ret = true;
            }
            catch (Exception e)
            {
                //TODO エラー時の処理を追加
            }
            return ret;
        }

        private static void CreateDb()
        {
            using (var conn = new SQLiteConnection("Data Source=" + APP_DATA_PATH + DB_FILE_NAME))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("create table " + TBL_STTCS + "(");
                    sb.Append("id TEXT PRIMARY KEY,");
                    sb.Append("title TEXT,");
                    sb.Append("start TEXT,");
                    sb.Append("finish TEXT,");
                    sb.Append("time INTEGER");
                    sb.Append(")");

                    command.CommandText = sb.ToString();
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static List<Statictics> SelectStaticticsAll()
        {
            List<Statictics> list = new List<Statictics>();

            using (var conn = new SQLiteConnection("Data Source=" + APP_DATA_PATH + DB_FILE_NAME))
            {
                conn.Open();
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = @"select * from " + TBL_STTCS + " order by finish desc";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader["id"].ToString();
                            string title = reader["title"].ToString();
                            Console.WriteLine(reader["start"].ToString());
                            DateTime start = DateTime.Parse(reader["start"].ToString());
                            DateTime finish = DateTime.Parse(reader["finish"].ToString());
                            long time = long.Parse(reader["time"].ToString());
                            Statictics item = new Statictics();
                            item.Id = id;
                            item.Title = title;
                            item.Start = start;
                            item.Finish = finish;
                            item.Time = time;
                            list.Add(item);
                        }
                    }
                }
            }
            
            return list;
        }

        public static void InsertStatictics(Statictics statictics)
        {
            using (var conn = new SQLiteConnection("Data Source=" + APP_DATA_PATH + DB_FILE_NAME))
            {
                conn.Open();
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("insert into " + TBL_STTCS + "(");
                        sb.Append("id, title, start, finish, time");
                        sb.Append(") values (");
                        sb.Append("'" + statictics.Id + "',");
                        sb.Append("'" + statictics.Title + "',");
                        sb.Append("'" + statictics.Start + "',");
                        sb.Append("'" + statictics.Finish + "', ");
                        sb.Append(statictics.Time);
                        sb.Append(")");

                        command.CommandText = sb.ToString();
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                conn.Close();
            }
        }

        public static void UpdateStatictics(Statictics statictics)
        {
            using (var conn = new SQLiteConnection("Data Source=" + APP_DATA_PATH + DB_FILE_NAME))
            {
                conn.Open();
                using (SQLiteTransaction tran = conn.BeginTransaction())
                {
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("update " + TBL_STTCS + " set ");
                        sb.Append("title = '" + statictics.Title + "',");
                        sb.Append("finish = '" + statictics.Finish + "', ");
                        sb.Append("time = '" + statictics.Time + "' ");
                        sb.Append("where id = '" + statictics.Id + "'");

                        command.CommandText = sb.ToString();
                        command.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                conn.Clone();
            }
        }

        public static void DeleteStatictics(string id)
        {

        }
    }

}
