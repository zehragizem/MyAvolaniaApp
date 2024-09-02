using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace MyAvaloniaApp.Views
{
    public partial class MainWindow : Window
    {
        public List<string> checkBoxNames = new List<string>
        {
            "BaglantiCheckBox",
            "SD_KartCheckBox",
            "Harici_FlashCheckBox",
            "KonfigürasyonCheckBox",
            "Saat_AyarıCheckBox",
            "Buton_KontrolüCheckBox",
            "RS485_1CheckBox",
            "RS485_2CheckBox",
            "Nextion_EkranCheckBox",
            "LedlerCheckBox"
        };

        public List<string> cellHeaders = new List<string>
        {
            "SERİ NUMARASI",
            "KART ADI",
            "BAĞLANTI",
            "SD KART",
            "HARİCİ FLASH",
            "KONFİGÜRASYON",
            "SAAT AYARI",
            "BUTON KONTROLÜ",
            "RS485(1)",
            "RS485(2)",
            "NEXTION EKRAN",
            "LEDLER",
            "KIRMIZI FEEDBACK",
            "SARI FEEDBACK",
            "YEŞİL FEEDBACK",
            "KIRMIZI LAMBA PATLAĞI",
            "KIRMIZI SON LAMBA PATLAĞI",
            "SARI LAMBA PATLAĞI",
            "SARI SON LAMBA PATLAĞI",
            "YEŞİL LAMBA PATLAĞI",
            "YEŞİL SON LAMBA PATLAĞI",
            "TARİH",
            "SAAT"
        };

        public Dictionary<string, List<string>> feedbackCheckBoxes = new Dictionary<string, List<string>>
        {
            { "KIRMIZI FEEDBACK", new List<string> { "KirmiziFeedbackG1", "KirmiziFeedbackG2", "KirmiziFeedbackG3", "KirmiziFeedbackG4" } },
            { "SARI FEEDBACK", new List<string> { "SariFeedbackG1", "SariFeedbackG2", "SariFeedbackG3", "SariFeedbackG4" } },
            { "YEŞİL FEEDBACK", new List<string> { "YesilFeedbackG1", "YesilFeedbackG2", "YesilFeedbackG3", "YesilFeedbackG4" } },
            { "KIRMIZI LAMBA PATLAĞI", new List<string> { "KirmiziLambaPatlagiG1", "KirmiziLambaPatlagiG2", "KirmiziLambaPatlagiG3", "KirmiziLambaPatlagiG4" } },
            { "KIRMIZI SON LAMBA PATLAĞI", new List<string> { "KirmiziSonLambaPatlagiG1", "KirmiziSonLambaPatlagiG2", "KirmiziSonLambaPatlagiG3", "KirmiziSonLambaPatlagiG4" } },
            { "SARI LAMBA PATLAĞI", new List<string> { "SariLambaPatlagiG1", "SariLambaPatlagiG2", "SariLambaPatlagiG3", "SariLambaPatlagiG4" } },
            { "SARI SON LAMBA PATLAĞI", new List<string> { "SariSonLambaPatlagiG1", "SariSonLambaPatlagiG2", "SariSonLambaPatlagiG3", "SariSonLambaPatlagiG4" } },
            { "YEŞİL LAMBA PATLAĞI", new List<string> { "YesilLambaPatlagiG1", "YesilLambaPatlagiG2", "YesilLambaPatlagiG3", "YesilLambaPatlagiG4" } },
            { "YEŞİL SON LAMBA PATLAĞI", new List<string> { "YesilSonLambaPatlagiG1", "YesilSonLambaPatlagiG2", "YesilSonLambaPatlagiG3", "YesilSonLambaPatlagiG4" } }
        };
        public int kontrol = 0;
        private const string SpreadsheetId = "1CfCPYNaHxFHMF0HCzSxdcZvXfvAYB0VMG3eoL62vEIo";
        public string credentialFilePath = "/Users/gizem/MyAvaloniaApp/credentials.json";
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "Test Takip";

        public MainWindow()
        {
            InitializeComponent();
        }

        public async void OnSubmitButtonClick(object sender, RoutedEventArgs e)
        {
            var seriNo = this.FindControl<TextBox>("SerialNumberTextBox")?.Text;
            var kartAdiList = new List<string>();

            if (string.IsNullOrWhiteSpace(seriNo))
            {
                var cancel = new CancelWindow();
                await cancel.ShowDialog(this);
                return;
            }

            var kartCheckbox = new
            {
                Title = "KART ADI",
                CheckBoxes = new List<string> { "KartAdiCheckBox1", "KartAdiCheckBox2" }
            };


            var checkBoxDetails = kartCheckbox.CheckBoxes
                .Select(name => new
                {
                    Name = name,
                    Content = this.FindControl<CheckBox>(name)?.Content!.ToString(),
                    IsChecked = this.FindControl<CheckBox>(name)?.IsChecked == true ? 1 : 0
                })
                .ToList();

            foreach (var name in kartCheckbox.CheckBoxes)
            {
                var checkBox = this.FindControl<CheckBox>(name);
                if (checkBox?.IsChecked == true)
                {
                    kartAdiList.Add(checkBox.Content!.ToString()!);
                }
            }

            if (kartAdiList.Count == 0)
            {
                var errorWindow = new CardSelectErrorWindow();
                await errorWindow.ShowDialog(this);
                return;
            }
            if (kartAdiList.Count == 2)
            {
                var errorWindow = new CardCountErrorWindow();
                await errorWindow.ShowDialog(this);
                return;
            }

            var checkBoxValues = checkBoxNames.Select(name =>
                (this.FindControl<CheckBox>(name)?.IsChecked == true ? 1 : 0)).ToArray();


            var feedbackNames = new[] { "G1", "G2", "G3", "G4" };
            var feedbackValues = new List<string>();

            foreach (var feedback in feedbackCheckBoxes)
            {
                var feedbackDict = new Dictionary<string, int>();
                foreach (var name in feedbackNames)
                {
                    feedbackDict[name] = 0;
                }
                foreach (var checkboxName in feedback.Value)
                {
                    var checkBox = this.FindControl<CheckBox>(checkboxName);
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        var feedbackName = checkboxName.Substring(checkboxName.Length - 2);
                        if (feedbackDict.ContainsKey(feedbackName))
                        {
                            feedbackDict[feedbackName] = 1;
                        }
                    }
                }

                //G1=0,G2=1 formatında ayarlar.
                var feedbackValue = string.Join(",", feedbackNames.Select(name => $"{name}={feedbackDict[name]}"));
                feedbackValues.Add(feedbackValue);
            }


            // Google Sheets service
            var credential = GoogleCredential.FromFile(credentialFilePath).CreateScoped(Scopes);

            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            if (await IsSerialNumberExists(service, seriNo!)) //eğer serino varsa yapılacaklar.
            {
                var existingSerialNumberWindow = new ExistingSerialNumberWindow(seriNo!, this);
                await existingSerialNumberWindow.ShowDialog(this);

                if (existingSerialNumberWindow.UserResponse == true)//kullanıcı evet dedi o seri numaraya sahip olan satır güncelleniyor.
                {
                    await SaveToGoogleSheet(service, seriNo!, checkBoxValues, feedbackValues, kartAdiList);

                }
                else if (existingSerialNumberWindow.UserResponse == false)
                {
                    Console.WriteLine("Kayıt güncellenmeyecek.");
                }

                kontrol += 1;
                ClearInputs();
            }
            else //serino yoksa direkt eklenir.
            {
                await SaveToGoogleSheet(service, seriNo!, checkBoxValues, feedbackValues, kartAdiList);
                ClearInputs();
            }
        }
        public async Task SaveToGoogleSheet(SheetsService service, string seriNo, int[] checkBoxValues, List<string> feedbackValues, List<string> kartAdiList)
        {
            var range = "Sayfa1!A:W";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
            var response = await request.ExecuteAsync();

            var values = response.Values;
            int rowIndex = -1;

            // Sheet boşsa yapılacaklar.
            if (values == null || values.Count == 0 || values[0].Count < cellHeaders.Count)
            {
                await SetColumnHeaders(service, cellHeaders); //öncelikle sütun adları eklenir.
            }

            //Sheet boş değilse
            if (values != null)
            {
                for (int row = 0; row < values.Count; row++)
                {
                    var cellValue = values[row][0].ToString();//values[row][0].ToString(), mevcut satırın ilk hücresindeki değeri (cellValue) alır. Bu değer, aranan seri numarasını (seriNo) temsil eder.
                    if (cellValue == seriNo)
                    {
                        rowIndex = row + 1; //Eğer cellValue ile seriNo eşleşirse, rowIndex değişkeni ilgili satırın indeksini saklar (row + 1, çünkü row değeri -1 di.) ve döngü sonlandırılır.
                        break;
                    }
                }
            }

            var now = DateTime.Now;

            var newRowValues = new List<object>
            {
                seriNo
            }
            .Concat(kartAdiList) //Concat metodu, iki listeyi birleştirir,newRowValues ekler.
            .Concat(checkBoxValues.Cast<object>())
            .Concat(feedbackValues)
            .Concat(new[] { now.ToString("dd-MM-yyyy"), now.ToString("HH:mm:ss") })
            .ToList();

            if (rowIndex > 0)//yani row+1 yapıldıysa
            {

                var updateRange = $"Sayfa1!A{rowIndex}:W{rowIndex}"; // o seri numarasına sahip veri güncellenir.
                var valueRange = new ValueRange
                {
                    Values = new List<IList<object>> { newRowValues }
                };

                var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, updateRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                try
                {
                    await updateRequest.ExecuteAsync();
                    var successUpdateWindow = new SuccessUpdateWindow();
                    await successUpdateWindow.ShowDialog(this);
                    Console.WriteLine("Başarıyla güncellendi");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Güncelleme Başarısız: {ex.Message}");
                }
            }
            else// row hala -1 ise
            {
                //ekleme yapılacaksa
                var appendRange = "Sayfa1!A:W";
                var valueRange = new ValueRange
                {
                    Values = new List<IList<object>> { newRowValues }
                };

                var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, appendRange);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;

                try
                {
                    await appendRequest.ExecuteAsync();
                    var successWindow = new SuccessWindow();
                    await successWindow.ShowDialog(this);
                    Console.WriteLine("Başarıyla eklendi");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ekleme Başarısız: {ex.Message}");
                }
            }

            ClearInputs();
        }

        private async Task SetColumnHeaders(SheetsService service, List<string> cellHeaders)
        {
            // Aralık tanımlama (ilk satır)
            var range = "Sayfa1!A1";

            // Başlıkları tutmak için bir ValueRange nesnesi oluştur
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>>
        {
            cellHeaders.Cast<object>().ToList() // Başlıkları bir obje listesine dönüştür
        }
            };

            // Güncelleme isteği oluştur
            var request = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            // Güncelleme isteğini yürüt
            await request.ExecuteAsync();

            Console.WriteLine("Başlıklar eklendi.");
        }

        public async Task<bool> IsSerialNumberExists(SheetsService service, string seriNo)
        {
            var range = "Sayfa1!A:A";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
            var response = await request.ExecuteAsync();

            var values = response.Values;
            if (values != null)
            {
                foreach (var row in values)
                {
                    if (row.Count > 0 && row[0].ToString() == seriNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ClearInputs()
        {
            var kartCheckbox = new
            {
                Title = "KART ADI",
                CheckBoxes = new List<string> { "KartAdiCheckBox1", "KartAdiCheckBox2" }
            };
            this.FindControl<TextBox>("SerialNumberTextBox")!.Text = string.Empty;

            foreach (var checkBoxName in checkBoxNames)
            {
                var checkBox = this.FindControl<CheckBox>(checkBoxName);
                if (checkBox != null)
                {
                    checkBox.IsChecked = false;
                }
            }

            foreach (var feedbackList in feedbackCheckBoxes.Values)
            {
                foreach (var feedbackName in feedbackList)
                {
                    var checkBox = this.FindControl<CheckBox>(feedbackName);
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = false;
                    }
                }
            }
            // Kart checkbox'larını temizleyin
            foreach (var name in kartCheckbox.CheckBoxes)
            {
                var checkBox = this.FindControl<CheckBox>(name);
                if (checkBox != null)
                {
                    checkBox.IsChecked = false;
                }
            }
        }
    }
}
