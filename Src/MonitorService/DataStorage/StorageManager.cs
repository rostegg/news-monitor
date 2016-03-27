using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorService.DataStorage
{
    public static class StorageManager
    {
        static Semaphore semaphore = new Semaphore(1, 1);
        static Semaphore reader = new Semaphore(1, 1);
            
        public static bool AddItem(string Name, string Url, string Tag)
        {
            try
            {
                semaphore.WaitOne();
                if (!ItemExist(Url,Tag))
                {
                    using (SQLiteConnection Connection = new SQLiteConnection(String.Format(@"Data Source={0};Version=3;", Configuration.Setup.DataPath)))
                    {
                        Connection.Open();
                        string sql = String.Format("insert into {0} (name ,url) values ('{1}','{2}')", Tag, Name, Url);
                        SQLiteCommand command = new SQLiteCommand(sql, Connection);
                        command.ExecuteNonQuery();                      
                    }
                    semaphore.Release();
                    return true;
                }
                else
                {               
                    Service.WriteLog("Item exists!!!");
                    semaphore.Release();
                    return false;
                }
                

            }
            catch (Exception ex)
            {
                semaphore.Release();
                Service.WriteLog(ex.ToString());
                return false;
            }
        }
        public static void CreateTable(string Tag)
        {
            try
            {
                if (!IsTableExists(Tag))
                {
                    using (SQLiteConnection Connection = new SQLiteConnection(String.Format(@"Data Source={0};Version=3;", Configuration.Setup.DataPath)))
                    {
                        Connection.Open();
                        string sql = String.Format("create table {0} (id INTEGER PRIMARY KEY AUTOINCREMENT, timeadd TIMESTAMP DEFAULT CURRENT_TIMESTAMP, name varchar(50), url varchar(50))", Tag);
                        SQLiteCommand command = new SQLiteCommand(sql, Connection);
                        command.ExecuteNonQuery();
                        Service.WriteLog("Created table " + Tag);
                    }
                }
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
            }

        }

        private static bool IsTableExists(string Tag)
        {
            using (SQLiteConnection Connection = new SQLiteConnection(String.Format(@"Data Source={0};Version=3;", Configuration.Setup.DataPath)))
            {
                Connection.Open();
                try
                {
                    string sql = String.Format("SELECT id FROM {0}", Tag);
                    SQLiteCommand command = new SQLiteCommand(sql,Connection);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Service.WriteLog("Table exists\n"+ex);
                    Connection.Close();
                    return false;
                }
            }

        }

        public static bool ItemExist(string Adress, string Tag)
        {
            try
            {
                reader.WaitOne();
                using (SQLiteConnection Connection = new SQLiteConnection(String.Format(@"Data Source={0};Version=3;",Configuration.Setup.DataPath)))
                {
                    Connection.Open();
                    string sql = String.Format("SELECT name FROM {0} WHERE url='{1}'", Tag, Adress);
                    SQLiteCommand Command = new SQLiteCommand(sql, Connection);
                    SQLiteDataReader Reader = Command.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        reader.Release();
                        // here close
                        return true;
                    }
                    else
                    {
                        reader.Release();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
                return false;
            }
        }
    }

}
