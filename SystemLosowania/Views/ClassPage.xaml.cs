using Microsoft.Maui.Storage;
using System;
using SystemLosowania.Models;

namespace SystemLosowania.Views;

[QueryProperty(nameof(ClassName), "ClassName")]
public partial class ClassPage : ContentPage
{
	public ClassPage()
	{
		InitializeComponent();
    }

    private int luckyNumber = -1;
    public int LuckyNumber
    {
        get
        { 
            return luckyNumber; 
        }
        set 
        {
            luckyNumber = value;
        }
    }

    private string className;
    public string ClassName
    {
        get
        {
            return className;
        }
        set
        {
            if (value != null)
            {
                className = value;
                Headbar.Text = $"SystemLosowania | Klasa {value}";

                var context = (Class) BindingContext;
                context.LoadStudents(value);
            }
        }
    }

    private void AddStudent_Clicked(object sender, EventArgs e)
    {
        string newStudentIdString = StudentIdEntry.Text;
        string newStudentName = StudentNameEntry.Text;

        if (string.IsNullOrEmpty(newStudentIdString) || string.IsNullOrEmpty(newStudentName))
        {
            DisplayAlert("Błąd!", "Wypełnij formularz.", "OK");
            return;
        }

        if (!int.TryParse(newStudentIdString, out var newStudentId)) 
        {
            DisplayAlert("Błąd!", "Wprowadzony numerek ucznia nie wygląda na liczbę całkowitą.", "OK");
            return;
        }

        if (newStudentId < 1)
        {
            DisplayAlert("Błąd!", "Wprowadzony numerek ucznia musi być większy lub równy 1.", "OK");
        }

        Student newStudent = new Student() 
        { 
            Id = newStudentId, 
            Name = newStudentName 
        };

        var context = (Class) BindingContext;
        context.StudentsCollection.Add(newStudent);
        SaveFile();
    }

    public void SaveFile()
    {
        var context = (Class) BindingContext;
        File.WriteAllText(Path.Combine(FileSystem.AppDataDirectory, $"{ClassName}.txt"), context.ConvertCollectionToString());
    }

    private async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
        {
            var selectedStudent = (Student)e.CurrentSelection[0];
            ((CollectionView)sender).SelectedItem = null;

            bool chooseAction = await DisplayAlert("Akcje", "Czy chcesz dokonać edycji ucznia lub go usunąć?", "Tak", "Nie");
            if (!chooseAction) return;

            var context = (Class) BindingContext;

            bool choosedEdit = await DisplayAlert("Akcje", "Wybierz co chcesz zrobić:", "Edycja", "Usuwanie");
            if (choosedEdit)
            {
                string newId = await DisplayPromptAsync("Edycja", "Numerek ucznia", initialValue: selectedStudent.Id.ToString(), maxLength: 2, keyboard: Keyboard.Numeric);

                bool isInteger = int.TryParse(newId, out int parsedId);
                if (!string.IsNullOrEmpty(newId) && !isInteger)
                {
                    await DisplayAlert("Błąd!", "Wprowadzony numerek ucznia nie wygląda na cyfrę. Spróbuj jeszcze raz.", "OK");
                    return;
                }

                string newName = await DisplayPromptAsync("Edycja", "Nazwisko ucznia", initialValue: selectedStudent.Name);

                selectedStudent.Id = string.IsNullOrEmpty(newId) ? selectedStudent.Id : parsedId;
                selectedStudent.Name = string.IsNullOrEmpty(newName) ? selectedStudent.Name : newName;
                SaveFile();
            }
            else
            {
                bool isDeleteConfirmed = await DisplayAlert("Usuwanie", $"Czy na pewno chcesz usunąć ucznia {selectedStudent.Name}?", "Tak", "Nie");
                if (!isDeleteConfirmed) return;

                context.StudentsCollection.Remove(selectedStudent);
                SaveFile();
            }
        }
    }

    private void RollStudent(object sender, EventArgs e)
    {
        var context = (Class) BindingContext;
        int count = context.StudentsCollection.Count;

        if (count < 2)
        {
            DisplayAlert("Błąd!", "Potrzebni są przynajmniej 2 uczniowie do wylosowania jednego z nich.", "OK");
            return;
        }

        Random random = new Random();
        Student randomStudent;

        do
        {
            randomStudent = context.StudentsCollection[random.Next(0, count)];
        } while(randomStudent.Id == LuckyNumber && LuckyNumber != -1);

        DisplayAlert("Wylosowany uczeń", $"{randomStudent.Name}", "OK");
    }

    private void RollLuckyNumber(object sender, EventArgs e)
    {

        var context = (Class) BindingContext;
        int count = context.StudentsCollection.Count;

        if (count == 0)
        {
            DisplayAlert("Błąd!", "Potrzebni są uczniowie do wylosowania szczęśliwego numerka.", "OK");
            return;
        }

        Random random = new Random();
        var randomStudent = context.StudentsCollection[random.Next(0, count)];
        LuckyNumber = randomStudent.Id;
        LuckyNumberLabel.Text = LuckyNumber.ToString();
    }

    private async void RemoveClass(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Usuwanie klasy", "Czy chcesz na pewno chcesz usunąć CAŁĄ KLASĘ?", "Tak", "Nie");
        if (!confirm) return;

        File.Delete(Path.Combine(FileSystem.AppDataDirectory, $"{ClassName}.txt"));
        await Shell.Current.GoToAsync("..");
    }
}