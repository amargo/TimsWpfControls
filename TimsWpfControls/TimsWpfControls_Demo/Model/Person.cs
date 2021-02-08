﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using TimsWpfControls.Model;

namespace TimsWpfControls_Demo.Model
{
    public class Person : ObservableObject
    {

        private Gender _Gender;
        public Gender Gender
        {
            get { return _Gender; }
            set { SetProperty(ref _Gender, value); }
        }


        private string _FirstName;
        public string FirstName
        {
            get { return _FirstName; }
            set { _FirstName = value; OnPropertyChanged(nameof(FirstName)); }
        }

        private string _LastName;
        public string LastName
        {
            get { return _LastName; }
            set { _LastName = value; OnPropertyChanged(nameof(LastName)); }
        }

        private int _Age;
        public int Age
        {
            get { return _Age; }
            set { _Age = value; OnPropertyChanged(nameof(Age)); }
        }

    }
   
}
