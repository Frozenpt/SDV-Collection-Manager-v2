using System;
using System.Windows.Input;
using Managers;

namespace SDV_Collection_Manager_v2.Commands
{
    public class UpdateItem : ICommand
    {
        public CollectionsXMLManager CollectionsXML { get; set; }

        public UpdateItem(CollectionsXMLManager collectionsXML)
        {
            this.CollectionsXML = collectionsXML;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Item)
                { return true; }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            this.CollectionsXML.updateItemStatus(parameter as Item);
        }
    }

    public class OpenWebpage : ICommand
    {
        public CollectionsXMLManager CollectionsXML { get; set; }

        public OpenWebpage(CollectionsXMLManager collectionsXML)
        {
            this.CollectionsXML = collectionsXML;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter)
        {
            if (parameter != null)
            {
                if (parameter is Item)
                { return true; }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            this.CollectionsXML.openWebpage(parameter as Item);
        }
    }
}
