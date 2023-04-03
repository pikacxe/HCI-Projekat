using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HCI_Projekat
{
    [Serializable]
    public class Single : INotifyPropertyChanged
    {
        private string _name;
        private int _year;
        private string _spotifyLink;
        private string _coverUrl;
        private string _decription;
        private DateTime _dateAdded;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value,
        [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);


        }
        public int Year
        {
            get => _year;
            set => SetField(ref _year, value);
        }
        public string SpotifyLink
        {
            get => _spotifyLink;
            set => SetField(ref _spotifyLink, value);
        }
        public string CoverUrl
        {
            get => _coverUrl;
            set => SetField(ref _coverUrl, value);
        }
        public string Description
        {
            get => _decription;
            set => SetField(ref _decription, value);
        }
        public DateTime DateAdded
        {
            get => _dateAdded;
            set => SetField(ref _dateAdded, value);
        }


        public Single(string name, int year, string spotifyLink, string coverUrl, string decription, DateTime dateAdded)
        {
            _name = name;
            _year = year;
            _spotifyLink = spotifyLink;
            _coverUrl = coverUrl;
            _decription = decription;
            _dateAdded = dateAdded;
        }

        public Single() : this("", DateTime.Now.Year, "https://open.spotify.com/", "resources/noimage.png", "descriptions/template.rtf", DateTime.Now) { }

        public Single Copy()
        {
            Single copy = new Single();
            copy._name = _name;
            copy._year = _year;
            copy._decription = _decription;
            copy._spotifyLink = _spotifyLink;
            copy._coverUrl = _coverUrl;
            copy._dateAdded = _dateAdded;

            return copy;
        }

        public bool Equals(Single s)
        {
            return (s._name == _name) && (s._year == _year)
                && (s._coverUrl == _coverUrl) && (s._decription == _decription)
                && (s._dateAdded == _dateAdded) && (s._spotifyLink == _spotifyLink);
        }

        public override string ToString()
        {
            return $"{_name}|{_year}|{_spotifyLink}|{_coverUrl}|{_decription}|{_dateAdded}";
        }
    }
}
