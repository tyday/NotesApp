using NotesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotesApp.ViewModel.Commands
{
    public class DeleteNotebookCommand : ICommand
    {
        public NotesVM Vm { get; set; }
        public event EventHandler CanExecuteChanged;

        public DeleteNotebookCommand(NotesVM VM)
        {
            Vm = VM;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Notebook notebook = parameter as Notebook;
            Vm.DeleteNotebook(notebook);
        }
    }
}
