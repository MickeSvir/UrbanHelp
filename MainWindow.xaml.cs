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
//using System.Data.Entity;
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using Telerik.Windows.Diagrams.Core;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.EntityFrameworkCore;
using Telerik.Windows.Controls;

namespace UrbanHelp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string serPerson,serOrg;
        private readonly HttpClient httpClient;
        private readonly AppContext db;
        private readonly Countries Countries;
        ObservableCollection<Node> nodes;
        List<Values> Phones = new();
        List<Values> EMails = new();
        //ObservableCollection<Node> organizations;
        //ObservableCollection<Node> persones;
        //ObservableCollection<Node> organizations;
        public MainWindow(HttpClient httpClient, AppContext appContext, Countries countries)
        {
            LocalizationManager.Manager = new CustomLocalizationManager();
            InitializeComponent();
            this.httpClient = httpClient;
            db = appContext;
            Countries = countries;
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


            //string url = $"https://geocode-maps.yandex.ru/1.x?apikey=f15363f3-bfdb-49d7-ae72-8592b634e355&geocode={Uri.EscapeDataString("Балашов пер. Титова д. 7")}&lang=ru_RU";


            //var stRes = await httpClient.GetStringAsync(url);


            //System.Xml.XmlDocument xmldoc = new();

            //xmldoc.LoadXml(stRes);

            //if (xmldoc.GetElementsByTagName("found")[0].InnerText != "0")
            //{

            //    //Получение широты и долготы.
            //    System.Xml.XmlNodeList nodes =
            //    xmldoc.GetElementsByTagName("pos");
            //    //var nodeVal = xmldoc.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[0].LastChild.FirstChild.InnerText;
            //    //Переменные широты и долготы.
            //    double latitude = 0.0; //Переменные широты.
            //    double longitude = 0.0; //Переменные долготы.

            //    //Получаем широту и долготу.
            //    foreach (System.Xml.XmlNode node in nodes)
            //    {
            //        latitude =
            //        System.Xml.XmlConvert.ToDouble(node.InnerText.Split(' ')[1]);
            //        longitude =
            //        System.Xml.XmlConvert.ToDouble(node.InnerText.Split(' ')[0]);
            //    }
            //    GMapMarker mapMarker = new(new PointLatLng(latitude, longitude)); // new PointLatLng(latitude, longitude));
            //    BitmapImage bi = new();
            //    bi.BeginInit();
            //    bi.UriSource = new Uri("Picture/pin_red.png", UriKind.Relative);
            //    bi.EndInit();
            //    Image image = new()
            //    {
            //        Source = bi,
            //        Width = 20,
            //        Height = 29
            //    };
            //    mapMarker.Shape = image;
            //    mapMarker.ZIndex = 1;
            //    mapView.Markers.Add(mapMarker);
            //}



        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {

                mapView.Height = this.ActualHeight - titemMap.ActualHeight - SystemParameters.CaptionHeight - 20;
                grdItem.Height = mapView.Height;
               
            }
            catch { }
        }
        private void mnuAdd_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if ((TVItem.SelectedItem as Node).Type.EndsWith("Orgs"))
            {
                spPerson.Visibility = Visibility.Collapsed;
                spOrg.DataContext = new Organization();
                spOrg.Visibility = Visibility.Visible;
                serOrg = null;
            }
            else if ((TVItem.SelectedItem as Node).Type.EndsWith("Pers"))
            {
                spOrg.Visibility = Visibility.Collapsed;                
                spPerson.DataContext = new Person();
                spPerson.Visibility = Visibility.Visible;
                serPerson = null;
                dtpPDOB.SelectedValue = null;
            }
            cmbOCountry.Text = "Россия";
        }
            
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            nodes = new ObservableCollection<Node>
                {
                new Node
                    {
                        Title = "Организации",
                        Person = null,
                        Organization=null,
                        Type="MainOrgs",
                        Nodes = new ObservableCollection<Node>(db.Organizations
                            .Include(o => o.OrganizationChanges)
                            .Include(o => o.CourtCases)
                            .Include(o => o.GovProcurements)
                            .Select(o => new Node {Title = o.Title, Organization = o, Type="Orgs"})
                            .OrderBy(o => o.Title).ToIList())
                    },
                    new Node
                    {
                        Title = "Персоналии",
                        Person = null,
                        Organization=null,
                        Type="MainPers",
                        Nodes = new ObservableCollection<Node>(db.Persons
                            .Include(p => p.FinConditions)
                            .Include(p => p.PersonChanges)
                            .Include(p => p.Relatives)
                            .Select(p => new Node {Title = p.FIO, Person = p, Type="Pers"})
                            .ToIList().OrderBy(n => n.Title))
                    }                                
            };

            TVItem.ItemsSource = nodes;
            TVItem.ExpandAll();
            cmbOCountry.ItemsSource = Countries.Country;
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            person.PhoneNumber = string.Join(',', Phones.Select(p => p.Value));
            person.EMail = string.Join(',', EMails.Select(p => p.Value));
            person.Country = cmbOCountry.Text;
            person.AddDate = DateTime.Now;
            if (!string.IsNullOrEmpty(serPerson))
            {
                Person oldPersone = JsonConvert.DeserializeObject<Person>(serPerson,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                person.PersonChanges.Add(new PersonChange
                {
                    AddDate = oldPersone.AddDate,
                    Person = person,
                    Person_Id = person.Id,
                    Data = serPerson
                });
                
            }
            else
                nodes.Where(n => n.Type == "MainPers").First().Nodes.Add(new Node { Title = person.FIO, Person = person, Type = "Pers" });
            
            db.Persons.Update(person);
            db.SaveChanges();
            TVItem.ExpandAll();
            
        }
        private void btnOSave_Click(object sender, RoutedEventArgs e)
        {
            Organization org = spOrg.DataContext as Organization;
            org.AddDate = DateTime.Now;
            if (!string.IsNullOrEmpty(serOrg))
            {
                Organization oldOrg = JsonConvert.DeserializeObject<Organization>(serOrg,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                org.OrganizationChanges.Add(new OrganizationChange
                {
                    AddDate = oldOrg.AddDate,
                    Organization = org,
                    Organization_Id = org.Id,
                    Data = serOrg
                });
            }
            else
                nodes.Where(n => n.Type == "MainOrgs").First().Nodes.Add(new Node { Title = org.Title, Organization = org, Type = "Orgs" });
            db.Organizations.Update(org);
            db.SaveChanges();
            
        }

        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if ((TVItem.SelectedItem as Node).Type.StartsWith("Main"))
                mnuRem.IsEnabled = false;
            else
                mnuRem.IsEnabled = true;
        }

        private void expPers_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            if (person?.FinConditions?.Count == 0)
            {
                for (int i = person.DateOfBirth.Year; i <= DateTime.Now.Year; i++)
                    person.FinConditions.Add(new PersonFinCondition
                    {
                        Person_Id=person.Id,
                        Person=person,
                        Year=i,
                        Condition=0.0
                    });
                db.PersonFinConditions.AddRange(person.FinConditions);
                db.SaveChanges();
            }
            finPerson.ItemsSource = db.Persons.Include(p => p.FinConditions)
                .FirstOrDefault(p=>p.Id.Equals(person.Id))
                .FinConditions.Where(f => f.Condition > 0)
                .OrderBy(f => f.Year);

        }

        private void btnUpdFC_Click(object sender, RoutedEventArgs e)
        {
            FinConditions finConditions = new ();
            finConditions.Owner=this;
            finConditions.Persone = spPerson.DataContext as Person;
            var pfc = finConditions.Persone.FinConditions.OrderBy(f=>f.Id).Select(f=>new {Id = f.Id,Condition = f.Condition }).ToList();
           
            if (finConditions.ShowDialog()==true)
            {
                db.Persons.Update(finConditions.Persone);
                db.SaveChanges();
                finPerson.ItemsSource = db.Persons.Include(p => p.FinConditions)
                    .FirstOrDefault(p => p.Id.Equals(finConditions.Persone.Id))
                    .FinConditions.Where(f => f.Condition > 0)
                    .OrderBy(f => f.Year);

            }
            else
            {
                foreach (var fc in finConditions.Persone.FinConditions)
                    fc.Condition = pfc.FirstOrDefault(pf => pf.Id.Equals(fc.Id)).Condition;
            }
        }

        private void expConnect_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            
            if(!string.IsNullOrEmpty(person.PhoneNumber))
                foreach (var p in person.PhoneNumber?.Split(',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    Phones.Add(new Values { Value = p });
           
            //ph.Phones = new ObservableCollection<string>(person.PhoneNumbers.OrderBy(ph => ph));
            //person.PhoneNumber = "1234567";
            rgvPhone.ItemsSource = Phones;
            if (!string.IsNullOrEmpty(person.EMail))
                foreach (var em in person.EMail?.Split(',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    EMails.Add(new Values { Value = em });
            rgvEMail.ItemsSource = EMails;
            
        }

        private void TV_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (TVItem.SelectedItem is null)
                return;
            if ((TVItem.SelectedItem as Node).Type.Equals("Pers"))
            {
                var person = ((Node)TVItem.SelectedItem).Person;
                spPerson.DataContext = person;
                finPerson.ItemsSource = person.FinConditions.OrderBy(f => f.Year);
                serPerson = JsonConvert.SerializeObject(spPerson.DataContext, Formatting.Indented,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

                
                spPerson.Visibility = Visibility.Visible;
            }
            else if ((TVItem.SelectedItem as Node).Type.Equals("Orgs"))
            {
                spOrg.DataContext = ((Node)TVItem.SelectedItem).Organization;
                serOrg = JsonConvert.SerializeObject(spOrg.DataContext, Formatting.Indented,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                spOrg.Visibility = Visibility.Visible;

            }
            else
            {
                spPerson.Visibility = Visibility.Collapsed;
                serPerson = null;
                spPerson.DataContext = null;
                spOrg.Visibility = Visibility.Collapsed;
                serOrg = null;
                spOrg.DataContext = null;
            }
        }

        
    }

    public class Node
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public Person Person { get; set; }
        public Organization Organization { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }

    class Values
    {
        public string Value { get; set; }
    }
}
