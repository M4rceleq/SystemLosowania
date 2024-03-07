using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemLosowania.Models
{
    class AllClasses
    {
        public ObservableCollection<string> ClassesCollection { get; set; } = new ObservableCollection<string>();

        public AllClasses() => LoadClasses();
        public void LoadClasses()
        {
            ClassesCollection.Clear();

            IEnumerable<string> allClasses = Directory.EnumerateFiles(FileSystem.AppDataDirectory, "*.txt")
                .Select(file => Path.GetFileNameWithoutExtension(file));

            foreach (string className in allClasses)
            {
                ClassesCollection.Add(className);
            }
        }
    }
}
