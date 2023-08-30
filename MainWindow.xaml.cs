using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET.WindowsPresentation;
using System.Globalization;
using System.Windows.Markup;
using GMap.NET;
using System.Xml.Linq;
using MapControl;
using System.Net.Http;
using System.Data.Entity;
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using Telerik.Windows.Diagrams.Core;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UrbanHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string serPerson;
        private readonly HttpClient httpClient;
        private readonly AppContext db;
        public MainWindow(HttpClient httpClient,AppContext appContext)
        {
            InitializeComponent();
            this.httpClient = httpClient;
            db = appContext;
        }




        private async void mapView_Loaded(object sender, RoutedEventArgs e)
        {

            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
           // mapView.MapProvider = GMap.NET.MapProviders.GMapProviders.YandexMap;
            mapView.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            mapView.MinZoom = 2; //минимальный зум
            mapView.MaxZoom = 16; //максимальный зум
            mapView.Zoom = 4; // какой используется зум при открытии
            mapView.Position = new GMap.NET.PointLatLng(66.4169575018027, 94.25025752215694);// точка в центре карты при открытии (центр России)
            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            mapView.CanDragMap = true; // перетаскивание карты мышью
            mapView.DragButton = MouseButton.Left; // какой кнопкой осуществляется перетаскивание
            mapView.ShowCenter = false; //показывать или скрывать красный крестик в центре
            mapView.ShowTileGridLines = false; //показывать или скрывать тайлы
            mapView.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);
            
            
            string url = $"https://geocode-maps.yandex.ru/1.x?apikey=f15363f3-bfdb-49d7-ae72-8592b634e355&geocode={Uri.EscapeDataString("Балашов пер. Титова д. 7")}&lang=ru_RU";


            var stRes = await httpClient.GetStringAsync(url);


            System.Xml.XmlDocument xmldoc = new();

            xmldoc.LoadXml(stRes);

            if (xmldoc.GetElementsByTagName("found")[0].InnerText != "0")
            {

                //Получение широты и долготы.
                System.Xml.XmlNodeList nodes =
                xmldoc.GetElementsByTagName("pos");
                //var nodeVal = xmldoc.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[0].LastChild.FirstChild.InnerText;
                //Переменные широты и долготы.
                double latitude = 0.0; //Переменные широты.
                double longitude = 0.0; //Переменные долготы.

                //Получаем широту и долготу.
                foreach (System.Xml.XmlNode node in nodes)
                {
                    latitude =
                    System.Xml.XmlConvert.ToDouble(node.InnerText.Split(' ')[1]);
                    longitude =
                    System.Xml.XmlConvert.ToDouble(node.InnerText.Split(' ')[0]);
                }
                GMapMarker mapMarker = new GMapMarker(new PointLatLng(latitude, longitude)); // new PointLatLng(latitude, longitude));
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri("Picture/pin_red.png", UriKind.Relative);
                bi.EndInit();
                Image image = new Image();
                image.Source = bi;
                image.Width = 20;
                image.Height = 29;
                mapMarker.Shape = image;
                mapMarker.ZIndex = 1;
                mapView.Markers.Add(mapMarker);
            }
            

            
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                
                mapView.Height = this.ActualHeight - titemMap.ActualHeight - SystemParameters.CaptionHeight - 20;
                grdPerson.Height = mapView.Height;
                grdOrganization.Height = mapView.Height;
            }
            catch { }
        }

        private void mnuPersAdd_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            spPerson.Visibility = Visibility;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var persones =new ObservableCollection<Node>
                {
                    new Node
                    {
                        FIO = "Персоналии",
                        Person = null,
                        Persons = new ObservableCollection<Node>(db.Persons.Select(p=>new Node {FIO=p.FIO, Person=p}).ToIList())
                    }
                };
            TVPerson.ItemsSource = persones;
            TVPerson.ExpandAll();
            //var persone = db.Persons.Include(p => p.PersonChanges).Include(p => p.FinConditions).Include(p => p.Relatives).ToIList();
        }

        private void TVPerson_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if ((TVPerson.SelectedItem is null) || ((Node)TVPerson.SelectedItem).Person is null)
            {
                spPerson.Visibility = Visibility.Hidden;
                serPerson = null;
                spPerson.DataContext = null;

            }
            else
            {
                spPerson.DataContext = ((Node)TVPerson.SelectedItem).Person;
                serPerson=JsonConvert.SerializeObject(spPerson.DataContext, Formatting.Indented,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                spPerson.Visibility = Visibility.Visible;
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Person oldPersone = JsonConvert.DeserializeObject<Person>(serPerson,
                new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
            Person person = ((Node)TVPerson.SelectedItem).Person;
            person.AddDate = DateTime.Now;
            person.PersonChanges.Add(new PersonChange
            {
                AddDate = oldPersone.AddDate,
                Person = person,
                Person_Id = person.Id,
                Data = serPerson
            });
            db.Persons.Update(person);
            db.SaveChanges();
        }
    }

    public class Node
    {
        public string FIO { get; set; }
        public Person Person { get; set; }
        public ObservableCollection<Node> Persons { get; set; }
    }
}
