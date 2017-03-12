using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ParserNetpeak.Infrastructure.Repository;
using ParserNetpeak.Model.Entity;

namespace ParserNetpeak.ViewModel
{
    /// <summary>
    ///     ViewModel для DataView(UserControl)
    /// </summary>
    internal class DataBaseViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Page> _pagesCollection;

        private Page _selectedPage;

        /// <summary>
        ///     Конструктор
        /// </summary>
        public DataBaseViewModel()
        {
            IRepository<Page> pageRepository = new PageRepository();
            PagesCollection = new ObservableCollection<Page>(pageRepository.GetList());
            SelectedPage = null;
        }

        public ObservableCollection<Page> PagesCollection
        {
            get { return _pagesCollection; }
            set
            {
                _pagesCollection = value;
                OnPropertyChanged(nameof(PagesCollection));
            }
        }

        public Page SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                OnPropertyChanged(nameof(SelectedPage));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}