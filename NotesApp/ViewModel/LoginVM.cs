﻿using NotesApp.Model;
using NotesApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.ViewModel
{
    public class LoginVM
    {
		private User user;

		public User User
		{
			get { return user; }
			set { user = value; }
		}

		public RegisterCommand RegisterCommand { get; set; }

		public LoginCommand LoginCommand { get; set; }

		public LoginVM()
		{
			RegisterCommand = new RegisterCommand(this);
			LoginCommand = new LoginCommand(this);
		}

		public void Login()
		{
			using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(DatabaseHelper.dbFile))
			{
				conn.CreateTable<User>();

				var user = conn.Table<User>().Where(u => u.Username == User.Username).FirstOrDefault();

				if(user.Password == User.Password)
				{
					//  Successful login
					//TODO: Establish Login
				}
			}
		}

		public void Register()
		{
			using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(DatabaseHelper.dbFile))
			{
				conn.CreateTable<User>();

				var result = DatabaseHelper.Insert(User);

				if (result)
				{
					// Establish registration
				}
			}
		}
	}
}
