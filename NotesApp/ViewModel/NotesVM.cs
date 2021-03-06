﻿using NotesApp.Model;
using NotesApp.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotesApp.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
        private bool isEditing;

        public bool IsEditing
        {
            get { return isEditing; }
            set 
            { 
                isEditing = value;
                OnPropertyChanged("IsEditing");
            }
        }

        public ObservableCollection<Notebook> Notebooks { get; set; }

        private Notebook selectedNotebook;

        public event PropertyChangedEventHandler PropertyChanged;

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set 
            { 
                selectedNotebook = value;
                OnPropertyChanged("SelectedNotebook");
                ReadNotes();
            }
        }

        public ObservableCollection<Note> Notes { get; set; }

        public NewNotebookCommand NewNotebookCommand { get; set; }
        public NewNoteCommand NewNoteCommand { get; set; }
        public BeginEditCommand BeginEditCommand { get; set; }
        public ReadNotebooksCommand ReadNotebooksCommand { get; set; }
        public HasEditedCommand HasEditedCommand { get; set; }
        public DeleteNotebookCommand DeleteNotebookCommand { get; set; }

        public NotesVM()
        {
            IsEditing = false;

            NewNotebookCommand = new NewNotebookCommand(this);
            NewNoteCommand = new NewNoteCommand(this);
            BeginEditCommand = new BeginEditCommand(this);
            HasEditedCommand = new HasEditedCommand(this);
            ReadNotebooksCommand = new ReadNotebooksCommand(this);
            DeleteNotebookCommand = new DeleteNotebookCommand(this);

            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();

            ReadNotebooks();
            ReadNotes();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void CreateNote(int notebookId)
        {
            Note newNote = new Note()
            {
                NotebookId = notebookId,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Title = "New note"
            };
            DatabaseHelper.Insert(newNote);
            ReadNotes();
        }

        public void CreateNoteBook()
        {
            Notebook newNotebook = new Notebook()
            {
                Name = "New Notebook",
                UserId = int.Parse(App.UserId)
            };
            DatabaseHelper.Insert(newNotebook);
            ReadNotebooks();
        }

        public void ReadNotebooks()
        {
            using(SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(DatabaseHelper.dbFile))
            {
                conn.CreateTable<Notebook>();
                int appUserId;
                bool appUserIdExists = int.TryParse(App.UserId, out appUserId);
                if (!appUserIdExists)
                    appUserId = 0;
                var notebooks = conn.Table<Notebook>().Where(n => n.UserId == appUserId).ToList();

                Notebooks.Clear();
                foreach(var notebook in notebooks)
                {
                    Notebooks.Add(notebook);
                }
            }
        }
        public void ReadNotes()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(DatabaseHelper.dbFile))
            {
                if (SelectedNotebook != null)
                {
                    conn.CreateTable<Note>();
                    var notes = conn.Table<Note>().Where(n => n.NotebookId == selectedNotebook.Id).ToList();
                    Notes.Clear();
                    foreach (var note in notes)
                    {
                        Notes.Add(note);
                    }
                }
            }
        }
        public void StartEditing()
        {
            IsEditing = true;
        }
        public void HasRenamed(Notebook notebook)
        {
            if(notebook != null)
            {
                DatabaseHelper.Update(notebook);
                IsEditing = false;
                ReadNotebooks();
            }
        }
        public void DeleteNotebook(Notebook notebook)
        {
            if(notebook != null)
            {
                DatabaseHelper.Delete(notebook);
                ReadNotebooks();
            }

        }
    }
}
