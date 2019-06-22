using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace ReminderApp
{
    class Program
    {

        public static string conStr = "Data Source=DESKTOP-1348B8O\\SQLEXPRESS;Initial Catalog=ReminderApp;Persist Security Info=True;User ID=sa;Password=sa123";
        static void Main(string[] args)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
 
            ReminderApp();
  
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            string queryString = "select * from Reminders where REMINDTIME='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        MessageBox.Show(" Name: " + reader["REMINDERNAME"] + "\n Description: " + reader["DESCRIPTION"] + "\n REMINDTIME: " + Convert.ToDateTime(reader["REMINDTIME"]).ToString("dd-MM-yyyy HH:mm"));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
        }

        public static void ReminderApp()
        {

            try
            {
                Console.WriteLine("Enter your selection from  below \n 1. Create \n 2. Edit \n 3. View");


                string selected = Console.ReadLine();
                int selection = Convert.ToInt32(selected);

                if(selection == 1)
                {
                    CreateReminder();
                }
                else if(selection == 2)
                {
                    EditReminder();
                }
                else if(selection == 3)
                {
                    ViewReminder();
                }
                else
                {
                    ReminderApp();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ReminderApp();
            }
          
        }

        public static void ListReminder()
        {
            string queryString = "select * from Reminders where REMINDTIME>'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    Console.WriteLine("\n REMINDER ID || NAME");
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("     {0}       ||{1}",
                        reader["REMINDERID"], reader["REMINDERNAME"])); 
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            } 
        }


        public static void GetReminder(int reminderID)
        {
            string queryString = "select * from Reminders where REMINDERID=" + reminderID + " and REMINDTIME>'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                { 
                    while (reader.Read())
                    {
                        MessageBox.Show(" Name: " + reader["REMINDERNAME"] + "\n Description: " + reader["DESCRIPTION"] + "\n REMINDTIME: " + Convert.ToDateTime(reader["REMINDTIME"]).ToString("dd-MM-yyyy HH:mm"));
                    }

                    if(!reader.HasRows)
                    {
                        Console.WriteLine("You have entered a wrong value");
                        ReminderApp();
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
        }

        public static Reminders GetReminderConsole(int reminderID)
        {
            Reminders reminder = new Reminders();
            string queryString = "select * from Reminders where REMINDERID=" + reminderID + " and REMINDTIME>'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        reminder.REMINDERID = Convert.ToInt32(reader["REMINDERID"]);
                        reminder.REMINDERNAME = reader["REMINDERNAME"].ToString();
                        reminder.DESCRIPTION = reader["DESCRIPTION"].ToString();
                        reminder.REMINDTIME = Convert.ToDateTime(reader["REMINDTIME"]); 
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
                return reminder;
            }
        }

        private static void ViewReminder()
        {
            ListReminder();
            Console.WriteLine("Please enter a Reminder ID from above list to view details");

            string selected = Console.ReadLine();
            int selection = Convert.ToInt32(selected);

            GetReminder(selection);

            ReminderApp();
        }

        private static void EditReminder()
        {
           
            ListReminder();

            Console.WriteLine("Please enter a Reminder ID from above list to edit details");

            string selected = Console.ReadLine();
            int selection = Convert.ToInt32(selected);
            Reminders reminders = new Reminders();
            reminders = GetReminderConsole(selection);


            if (reminders.REMINDERID > 0)
            {
                Console.WriteLine("Please choose a option to edit \n 1. Description \n 2. Remind Time");

                string selectedEdit = Console.ReadLine();
                int selectionEdit = Convert.ToInt32(selectedEdit);

                if (selectionEdit == 1)
                {
                    Console.WriteLine("Description: " + reminders.DESCRIPTION);
                    Console.WriteLine("Enter new description");
                    reminders.DESCRIPTION = Console.ReadLine();

                    int result = UpdateReminder(reminders);

                    if (result > 0)
                    {
                        Console.WriteLine("Successfully updated");
                        ReminderApp();
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
                else if (selectionEdit == 2)
                {
                    Console.WriteLine("REMIND TIME: " + reminders.REMINDTIME);
                    Console.WriteLine("Enter new date(dd-MM-yyyy)");

                    string date = Console.ReadLine();

                    Console.WriteLine("Enter reminder time(HH:mm)");

                    string time = Console.ReadLine();

                    reminders.REMINDTIME = Convert.ToDateTime(date + " " + time);


                    int result = UpdateReminder(reminders);

                    if (result > 0)
                    {
                        Console.WriteLine("Successfully updated");
                        ReminderApp();
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
                else
                {
                    Console.WriteLine("You have entered a wrong value");
                    ReminderApp();
                }
            }
            else
            {
                Console.WriteLine("You have entered a wrong value");
                ReminderApp();
            }
        }

        private static int UpdateReminder(Reminders reminders)
        {
            int result = 0;
            string queryString = "UPDATE [dbo].[Reminders]" +
           " SET [DESCRIPTION] = '" + reminders.DESCRIPTION + "'" +
           ",[REMINDTIME] = '" + reminders.REMINDTIME.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
           " WHERE REMINDERID =" + reminders.REMINDERID;
             
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();

                result = command.ExecuteNonQuery();

            }
            return result;
        }

        private static void CreateReminder()
        {
            Reminders reminders = new Reminders();

            Console.WriteLine("Enter Reminder Name");

            reminders.REMINDERNAME = Console.ReadLine();

            Console.WriteLine("Enter Reminder Description");

            reminders.DESCRIPTION = Console.ReadLine();


            Console.WriteLine("Enter reminder date(dd-MM-yyyy)");

            string date = Console.ReadLine();

            Console.WriteLine("Enter reminder time(HH:mm)");

            string time = Console.ReadLine();

            reminders.REMINDTIME = Convert.ToDateTime(date + " " + time);

            int result = InsertToDatabase(reminders);

            if(result > 0)
            {
                Console.WriteLine("Successfully saved");
                ReminderApp();
            }
            else
            {
                Console.WriteLine("Error");
            }
        }

        private static int InsertToDatabase(Reminders reminders)
        {
            int result = 0;
            string queryString = "INSERT INTO [dbo].[Reminders]" +
           "([REMINDERNAME]" +
           ",[DESCRIPTION]" +
           ",[REMINDTIME])" +
            "VALUES" +
           "('"+ reminders.REMINDERNAME +"'" +
           ",'"+ reminders.DESCRIPTION + "'" +
           ",'"+ reminders.REMINDTIME.ToString("yyyy-MM-dd HH:mm:ss") +"')"; 


            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(queryString, connection); 
                connection.Open();

                result = command.ExecuteNonQuery();
                 
            }
            return result;
        }


    }
}
