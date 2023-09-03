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
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using Telerik.Windows.Diagrams.Core;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.EntityFrameworkCore;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Book;
using System.Xml;
using System.Threading.Tasks;

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
        List<Values> Degrees = new()
        {
            new () { Value="Мать"},
            new () { Value="Отец"},
            new () { Value="Муж"},
            new () { Value="Жена"},
            new () { Value="Сын"},
            new () { Value="Дочь"},
            new () { Value="Сестра"},
            new () { Value="Брат"},
        };
        bool mapUsage = false;

        public MainWindow(HttpClient httpClient, AppContext appContext, Countries countries)
        {
            LocalizationManager.Manager = new CustomLocalizationManager();
            InitializeComponent();
            this.httpClient = httpClient;
            db = appContext;
            Countries = countries;
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
                swPerson.Visibility = Visibility.Collapsed;
                spOrg.DataContext = new Organization();
                swOrg.Visibility = Visibility.Visible;
                serOrg = null;
            }
            else if ((TVItem.SelectedItem as Node).Type.EndsWith("Pers"))
            {
                swOrg.Visibility = Visibility.Collapsed;                
                spPerson.DataContext = new Person();
                swPerson.Visibility = Visibility.Visible;
                serPerson = null;
                dtpPDOB.SelectedValue = null;
            }
            cmbPCountry.Text = "Россия";
        }
            
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //установки карт
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


            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //выбор подгрузки карты – онлайн или из ресурсов
            // mapView.MapProvider = GMap.NET.MapProviders.GMapProviders.YandexMap;
            mapAdress.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //какой провайдер карт используется (в нашем случае гугл) 
            mapAdress.MinZoom = 2; //минимальный зум
            mapAdress.MaxZoom = 17; //максимальный зум
            mapAdress.Zoom = 4; // какой используется зум при открытии
            //mapAdress.Position = new GMap.NET.PointLatLng(66.4169575018027, 94.25025752215694);// точка в центре карты при открытии (центр России)
            mapAdress.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter; // как приближает (просто в центр карты или по положению мыши)
            mapAdress.CanDragMap = true; // перетаскивание карты мышью
            mapAdress.DragButton = MouseButton.Left; // какой кнопкой осуществляется перетаскивание
            mapAdress.ShowCenter = false; //показывать или скрывать красный крестик в центре
            mapAdress.ShowTileGridLines = false; //показывать или скрывать тайлы
            mapAdress.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            nodes = new ObservableCollection<Node>
                {
                //заполнение организаций
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
                //заполнение персоналий
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
            cmbPCountry.ItemsSource = Countries.Country;
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //сохранение данных
            Person person = spPerson.DataContext as Person;
            person.PhoneNumber = string.Join(',', Phones.Select(p => p.Value));
            person.EMail = string.Join(',', EMails.Select(p => p.Value));
            person.Country = cmbPCountry.Text;
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

        
        private void btnUpdFC_Click(object sender, RoutedEventArgs e)
        {
            //получение доходов
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
            //заполнение телефонов и почты
            Person person = spPerson.DataContext as Person;
            
            if(!string.IsNullOrEmpty(person.PhoneNumber))
                foreach (var p in person.PhoneNumber?.Split(',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    Phones.Add(new Values { Value = p });
           

            rgvPhone.ItemsSource = Phones;
            if (!string.IsNullOrEmpty(person.EMail))
                foreach (var em in person.EMail?.Split(',',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    EMails.Add(new Values { Value = em });
            rgvEMail.ItemsSource = EMails;
            
        }

        private void rgvRelative_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
            frmDegree fDegree = new();
            Person person = spPerson.DataContext as Person;
            var degr = person.Relatives.Where(r => !r.Degree.StartsWith('M')
                    || !r.Degree.StartsWith('Ж')
                    || !r.Degree.StartsWith('О')).Select(r => r.Degree).ToArray();
            var res = Degrees
                .Where(d => !degr.Contains(d.Value)) //удаление двойных мужей, жён, отцов  и матерей
                .Where(d => !d.Value.Equals(person.Male ? "Муж" : "Жена")) //муж мужика, жена бабы
                .ToList();
            fDegree.Degree = res;
            fDegree.Owner = this;
            if (fDegree.ShowDialog() == true)
            {
                Relative relative = new()
                {
                    Degree = fDegree.Result

                };
                e.NewObject = relative;
            }
            else
                e.Cancel = true;
        }

        private void rgvRelative_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if(e.EditAction==Telerik.Windows.Controls.GridView.GridViewEditAction.Commit && 
                e.EditOperationType==Telerik.Windows.Controls.GridView.GridViewEditOperationType.Insert)
            {
                Relative rel = e.NewData as Relative;
                Person person = spPerson.DataContext as Person;
                rel.Person = person;
                rel.Person_Id = person.Id;
                person.Relatives.Add(rel);
                db.Persons.Update(person);
                db.SaveChanges();
            }
        }

        private void rgvRelative_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            //ограничения на даты 
            Person person = spPerson.DataContext as Person;
            if (e.Cell.Column.UniqueName == "Famil" || e.Cell.Column.UniqueName == "Name")
            {
                if(string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Значение не может быть пустым";
                }
            }
            else if(e.Cell.Column.UniqueName == "DateOfBirth")
            {
                if(person.DateOfBirth is null)
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Сначала введите дату рождения персоны";
                }
                if (string.IsNullOrEmpty(e.NewValue?.ToString()))
                {
                    e.IsValid = false;
                    e.ErrorMessage = "Значение не может быть пустым";
                }
                else if (DateTime.Parse(e.NewValue.ToString())>=person.DateOfBirth)
                {
                    if ((e.Row.Cells[0].Content as TextBlock).Text == "Мать")
                    {
                        e.IsValid = false;
                        e.ErrorMessage = $"Мать не может быть младше {(person.Male ? "сына" : "дочери")}";
                    }
                    else if ((e.Row.Cells[0].Content as TextBlock).Text == "Отец")
                    {
                        e.IsValid = false;
                        e.ErrorMessage = $"Отец не может быть младше {(person.Male ? "сына" : "дочери")}";
                    }
                }
                else if (DateTime.Parse(e.NewValue.ToString()) <= person.DateOfBirth)
                {                   
                    if ((e.Row.Cells[0].Content as TextBlock).Text == "Сын")
                    {
                        e.IsValid = false;
                        e.ErrorMessage = $"Сын не может быть старше {(person.Male ? "отца" : "матери")}";
                    }
                    else if ((e.Row.Cells[0].Content as TextBlock).Text == "Дочь")
                    {
                        e.IsValid = false;
                        e.ErrorMessage = $"Дочь не может быть старше {(person.Male ? "отца" : "матери")}";
                    }
                }
            }
        }

        private void dtpPDOB_ParseDateTimeValue(object sender, ParseDateTimeEventArgs args)
        {
            //проверка даты
            string input = args.TextToParse.ToLower();
            if (!string.IsNullOrEmpty(input))
            {
                DateTime dat;
                if (!DateTime.TryParse(input, out dat))
                {
                    rgvRelative.IsEnabled = false;
                    args.IsParsingSuccessful = false;
                    (sender as RadDatePicker).Focus();
                }
                else
                {
                    args.Result = dat;
                    rgvRelative.IsEnabled = true;
                }
            }
            else
            {
                rgvRelative.IsEnabled = false;
                args.IsParsingSuccessful = false;
                (sender as RadDatePicker).Focus();
            }
        }

        private void dtpPDOB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if(dtpPDOB.SelectedValue == null)
            {
                dtpPDOB.ErrorTooltipContent = "Значение не может быть пустым";
                rgvRelative.IsEnabled = false;
                dtpPDOB.Focus();
            }
            else
                rgvRelative.IsEnabled = true;
        }

        private void dtpPDOB_LostFocus(object sender, RoutedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            if (dtpPDOB.SelectedValue == null)
            {
                dtpPDOB.ErrorTooltipContent = "Значение не может быть пустым";
                rgvRelative.IsEnabled = false;
                
            }
            else
            //Формирование пустых фин. данных
                if (person?.FinConditions?.Count == 0)
                {
                    for (int i = (int)(person.DateOfBirth?.Year ?? DateTime.Now.Year); i <= DateTime.Now.Year; i++)
                        person.FinConditions.Add(new PersonFinCondition
                        {
                            Person_Id = person.Id,
                            Person = person,
                            Year = i,
                            Condition = 0.0
                        });
                    db.PersonFinConditions.AddRange(person.FinConditions);
                    db.SaveChanges();
                }
            rgvRelative.IsEnabled = true;
        }

        private void expMapText_Expanded(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            expTxtMapHeader.Text = "Карту в текст";
            mapUsage = true;
            cmbPCountry.IsEnabled = false;
            txtPCity.IsEnabled = false;
            txtPZIPCode.IsEnabled = false;
            txtPAdress.IsEnabled = false;
        }

        private void expMapText_Collapsed(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            expTxtMapHeader.Text = "Текст в карту";
            mapUsage = false;
            cmbPCountry.IsEnabled = true;
            txtPCity.IsEnabled = true;
            txtPZIPCode.IsEnabled = true;
            txtPAdress.IsEnabled = true;
        }

       
        private void TV_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (TVItem.SelectedItem is null)
                return;
            if ((TVItem.SelectedItem as Node).Type.Equals("Pers"))
            {
                var person = ((Node)TVItem.SelectedItem).Person;
                spPerson.DataContext = person;
                //заполнение данными
                finPerson.ItemsSource = person.FinConditions.OrderBy(f => f.Year);
                serPerson = JsonConvert.SerializeObject(spPerson.DataContext, Newtonsoft.Json.Formatting.Indented,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                rgvRelative.ItemsSource = person.Relatives;
                              
                finPerson.ItemsSource = db.Persons.Include(p => p.FinConditions)
                    .FirstOrDefault(p => p.Id.Equals(person.Id))
                    .FinConditions.Where(f => f.Condition > 0)
                    .OrderBy(f => f.Year);
                swPerson.Visibility = Visibility.Visible;
            }
            else if ((TVItem.SelectedItem as Node).Type.Equals("Orgs"))
            {
                spOrg.DataContext = ((Node)TVItem.SelectedItem).Organization;
                serOrg = JsonConvert.SerializeObject(spOrg.DataContext, Newtonsoft.Json.Formatting.Indented,
                    new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
                swOrg.Visibility = Visibility.Visible;

            }
            else
            {
                swPerson.Visibility = Visibility.Collapsed;
                serPerson = null;
                spPerson.DataContext = null;
                swOrg.Visibility = Visibility.Collapsed;
                serOrg = null;
                spOrg.DataContext = null;
            }
        }
        private async void  cmbPCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            (double?, double?) res = default;
            res = await SetMarkerFromText(cmbPCountry.Text);
            if(res==(null,null))            
                return;
            person.Latitude = (double)res.Item1;
            person.Longitude = (double)res.Item2;
        }
        private async void txtPCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            (double?, double?) res = default;
            res = await SetMarkerFromText($"{cmbPCountry.Text} {txtPCity.Text}");
            if (res == (null, null))
                return;
            person.Latitude = (double)res.Item1;
            person.Longitude = (double)res.Item2;
        }
        private async void txtPZIPCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            (double?, double?) res = default;
            res = await SetMarkerFromText($"{cmbPCountry.Text} {txtPCity?.Text} {txtPZIPCode?.Text}");
            if (res == (null, null))
                return;
            person.Latitude = (double)res.Item1;
            person.Longitude = (double)res.Item2;
        }
        private async void txtPAdress_TextChanged(object sender, TextChangedEventArgs e)
        {
            Person person = spPerson.DataContext as Person;
            (double?, double?) res = default;
            res = await SetMarkerFromText($"{cmbPCountry.Text} {txtPCity?.Text} {txtPZIPCode?.Text} {txtPAdress?.Text}");
            if (res == (null, null))
                return;
            person.Latitude = (double)res.Item1;
            person.Longitude = (double)res.Item2;
            mapAdress.Zoom = 17;
        }

        

        private async Task<(double?,double?)> SetMarkerFromText(string Adress)
        {
            string url = $"https://geocode-maps.yandex.ru/1.x?apikey=f15363f3-bfdb-49d7-ae72-8592b634e355&geocode=" +
                $"{Uri.EscapeDataString(Adress)}&lang=ru_RU";


            var stRes = await httpClient.GetStringAsync(url);


            System.Xml.XmlDocument xmldoc = new();

            xmldoc.LoadXml(stRes);

            if (xmldoc.GetElementsByTagName("found")[0].InnerText != "0")
            {

                //Получение широты и долготы.
                System.Xml.XmlNodeList nodes =
                xmldoc.GetElementsByTagName("pos");

                double latitude = 0.0; //Переменные широты.
                double longitude = 0.0; //Переменные долготы.

                //Получаем первые широту и долготу.
                foreach (System.Xml.XmlNode node in nodes)
                {
                    latitude =
                    System.Xml.XmlConvert.ToDouble(node.InnerText.Split(' ')[1]);
                    longitude =
                    System.Xml.XmlConvert.ToDouble(node.InnerText.Split(' ')[0]);
                    break;
                }

                GMapMarker mapMarker = new(new PointLatLng(latitude, longitude)); // new PointLatLng(latitude, longitude));
                BitmapImage bi = new();
                bi.BeginInit();
                bi.UriSource = new Uri("Picture/pin_red.png", UriKind.Relative);
                bi.EndInit();
                Image image = new()
                {
                    Source = bi,
                    Width = 20,
                    Height = 29
                };
                mapMarker.Shape = image;
                mapMarker.ZIndex = 1;

                mapAdress.Markers.Clear();
                mapAdress.Markers.Add(mapMarker);

                mapAdress.SetZoomToFitRect(new RectLatLng { Lat = latitude, Lng = longitude });
                mapAdress.Position = new GMap.NET.PointLatLng(latitude, longitude);
                return (latitude, longitude);
                //mapAdress.FromLatLngToLocal(mapAdress.Position);
            }
            else
                return (null,null);
        }
    }



    public class Node
    {
        //класс для построения дерева
        public string Title { get; set; }
        public string Type { get; set; }
        public Person Person { get; set; }
        public Organization Organization { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }

    public class Values
    {
        //класс для работы RadGridView
        public string Value { get; set; }
    }
    
}
