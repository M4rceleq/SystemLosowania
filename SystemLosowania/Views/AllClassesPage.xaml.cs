using SystemLosowania.Models;

namespace SystemLosowania.Views;

public partial class AllClassesPage : ContentPage
{
	public AllClassesPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        ((AllClasses) BindingContext).LoadClasses();
    }

    private void AddClass_Clicked(object sender, EventArgs e)
    {
        string newClassName = AddClassEntry.Text;
        if (string.IsNullOrEmpty(newClassName))
        {
            DisplayAlert("Błąd!", "Wprowadź najpierw nazwę klasy.", "OK");
            return;
        }

        var context = (AllClasses)BindingContext;
        if (context.ClassesCollection.Contains(newClassName))
        {
            DisplayAlert("Błąd!", "Taka klasa już istnieje.", "OK");
            return;
        }

        string fileName = $"{newClassName}.txt";
        FileStream fileStream = File.Create(Path.Combine(FileSystem.AppDataDirectory, fileName));
        fileStream.Close();

        context.ClassesCollection.Add(newClassName);
    }

    private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            string selectedClass = (string)e.CurrentSelection[0];
            await Shell.Current.GoToAsync($"{nameof(ClassPage)}?{nameof(ClassPage.ClassName)}={selectedClass}");
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}