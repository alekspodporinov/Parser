using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using ParserNetpeak.Infrastructure;
using ParserNetpeak.Infrastructure.Repository;
using ParserNetpeak.Model.Entity;

namespace ParserNetpeak.ViewModel
{
    /// <summary>
    ///     ViewModel для MainWindow
    /// </summary>
    public class ParsViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Page> _pageRepository;
        public ObservableCollection<Page> PagesCollection { get; set; }

        private Page _parsPage;
        public Page ParsPage
        {
            get { return _parsPage; }
            set
            {
                _parsPage = value;
                OnPropertyChanged(nameof(ParsPage));
            }
        }

        /// <summary>
        ///     Комманда парсинга
        /// </summary>
        private RelayCommand _parseCommand;
        public RelayCommand ParseCommand
        {
            get
            {
                return _parseCommand ??
                  (_parseCommand = new RelayCommand(async obj =>
                  {
                      var url = obj as string;
                      if (url != null)
                      {
                          try
                          {
                              ParsPage = await Parse(url);
                          }
                          catch (Exception)
                          {
                              // ignored
                          }
                      }
                  }, (obj) =>
                  {
                      string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                      Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                      return reg.IsMatch((string)obj);
                  }));
            }
        }

        /// <summary>
        ///     Комманда сохранения распаршеной странички в бд SQLite
        /// </summary>
        private RelayCommand _saveDataCommand;
        public RelayCommand SaveDataCommand
        {
            get
            {
                return _saveDataCommand ??
                       (_saveDataCommand = new RelayCommand(async obj =>
                       {
                           _pageRepository.Create(ParsPage);
                           await _pageRepository.SaveAsync();
                           ParsPage = null;
                       },
                           (obj) => ParsPage != null));
            }
        }

        /// <summary>
        ///     Комманда открытия нового окна
        /// </summary>
        private RelayCommand _openDataWindowCommand;
        public RelayCommand OpenDataWindowCommand
        {
            get
            {
                return _openDataWindowCommand ??
                       (_openDataWindowCommand = new RelayCommand(obj =>
                       {

                           var vm = new DataBaseViewModel();
                           ViewShower.Show(0, vm, true, b => { if (b != null && b.Value) ParsPage = vm.SelectedPage; });
                       }));
            }
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public ParsViewModel()
        {
            _pageRepository = new PageRepository();
            PagesCollection = new ObservableCollection<Page>(_pageRepository.GetList());

            ParsPage = null;
        }

        /// <summary>
        ///     Парсинг
        /// </summary>
        /// <param name="url">
        ///     Url для парсинга
        /// </param>
        /// <returns>
        ///     Возвращет Page
        /// </returns>
        private async Task<Page> Parse(string url)
        {
            var documentResult = await GetParsHtmlWithTimingInfo(url);
            var document = documentResult.Item1; //Получаем распарсиную страничку

            //Инициализируем Page
            var page = new Page
            {
                PageUrl = url,
                Description = document.Head.GetElementsByTagName("meta")
                    .Cast<IHtmlMetaElement>()
                    .FirstOrDefault(e => e.Name == "description")?.Content ?? "No description",
                Title = document.Title,
                ServerResponseCode = (int) document.StatusCode,
                ServerResponseTime = documentResult.Item2.ToString("g")
            };

            //Создаем и наполняем коллекцию тегов
            var tags = new List<Tag>();
            foreach (var element in document.Links)
            {
                var a = (IHtmlAnchorElement)element;
                var str = a.Href;

                if (a.Href.Contains("about://"))
                    str = a.Href.Replace("about://", page.PageUrl);

                tags.Add(new Tag { Name = element.TagName, InnerHtml = str });
            }

            foreach (var element in document.Images)
            {
                var str = element.Source;

                if (str.Contains("about://"))
                    str = str.Replace("about://", page.PageUrl);

                tags.Add(new Tag { Name = element.TagName, InnerHtml = str });
            }


            tags.AddRange(document.GetElementsByTagName("h1")
                .Select(e =>
                    new Tag
                    {
                        InnerHtml = e.InnerHtml,
                        Name = e.TagName,
                    })
                );
            //Присваеваем коллекцию тегов объекту Page
            page.Tags = tags;

            return page;
        }

        /// <summary>
        ///     Запрос на сервер
        /// </summary>
        /// <param name="url">
        ///     Url для запроса
        /// </param>
        /// <returns>
        ///     Возвращает Tuple(кортеж) где Item1 - IHtmlDocument(Распаршеная страничка), Item2 - TimeSpan(Время ответа сервера)
        /// </returns>
        private async Task<Tuple<IHtmlDocument, TimeSpan>> GetParsHtmlWithTimingInfo(string url)
        {
            var stopWatch = Stopwatch.StartNew();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2");
                var request = await client.GetAsync(url);
                var response = await request.Content.ReadAsStreamAsync();
                var parser = new HtmlParser(); //AngleSharp(Библиотека для парсинга)
                var document = parser.Parse(response);
                return new Tuple<IHtmlDocument, TimeSpan>(document, stopWatch.Elapsed);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
