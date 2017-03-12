using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ParserNetpeak.View;

namespace ParserNetpeak.Infrastructure
{
    class ViewShower
    {
        /// <summary>
        ///     Класс для создания дочерних окон(костыль чтобы не использовать сторониих библиотек)
        /// </summary>
        /// <param name="viewIndex">
        ///     Индекс View(окна) которое нужно создать(см. switch)
        /// </param>
        /// <param name="dataContext">
        ///     Контекст данных для выбранного View
        /// </param>
        /// <param name="isModal">
        ///     true - дочернее окно будет модальным, false - окно будет не модальным
        /// </param>
        /// <param name="closeAction">
        ///     Метод который надо вызвать по закрытию окна
        /// </param>
        public static void Show(int viewIndex, object dataContext, bool isModal, Action<bool?> closeAction)
        {
            //UserControl для дочернего окна
            UserControl control = null;
            //switch для выбора нужного view
            switch (viewIndex)
            {
                case 0:
                    control = new DataBaseView();//UserControl для отображения данных в бд
                    break;
                default:
                    //Вбрасывание исключения в случае не правильного индекса
                    throw new ArgumentOutOfRangeException(nameof(viewIndex), "Такого индекса View не существует");
            }


            var wnd = new Window {SizeToContent = SizeToContent.WidthAndHeight}; //Созданиие окна, установка размеров окна

            control.DataContext = dataContext;//ViewModel для view
            var sp = new StackPanel();//StackPanel для размещения елементов в UserControl
            sp.Children.Add(control);

            var applyButton = new Button//кнопка выбора
            {
                Content = "Принять",
                FontSize = 20,
                Margin = new Thickness(0, 10, 10, 0),
                Padding = new Thickness(5)
            }; 
            applyButton.Click += (sender, args) =>//Событие которое происходит при нажатии на кнопку выбра
            {
                if (isModal) wnd.DialogResult = true;
                else wnd.Close();
            };

            var buttonPanel = new StackPanel//StackPanel для размещения кнопок выбора и выхода
            {
                //Настройки StackPanel
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            }; 
            buttonPanel.Children.Add(applyButton);//Добавление кнопки выбора в StackPanel
            sp.Children.Add(buttonPanel);
            wnd.Content = sp;//Присваиваем окну контролы
            wnd.Closed += (s, e) => closeAction(wnd.DialogResult);//Событие которое происходит при закрытии окна

            if (isModal)
            {
                var cancelButton = new Button//Создание кнопки выходи из модального окна
                {
                    //Инициализация кнопки
                    Content = "Отмена",
                    FontSize = 20,
                    Margin = new Thickness(10, 10, 0, 0),
                    Padding = new Thickness(5)
                }; 
                cancelButton.Click += (s, e) => { wnd.DialogResult = false; };//Событие которое происходит при нажатии на кнопку выхода
                buttonPanel.Children.Add(cancelButton);//Добавление кнопки к контейнеру
                wnd.ShowDialog();//показ окна
            }
            else
            {
                wnd.Show();
            }
        }
    }
}
