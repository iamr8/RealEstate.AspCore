using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using Environment = System.Environment;

namespace RealEstate.Droid.Utils
{
    public class GetItems
    {
        public void CreateDatabase()
        {
            const string dbName = "rsdb.db3";
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), dbName);
            var db = new SQLiteConnection(dbPath);
            //db.CreateTable()
        }
    }
}