﻿using ICSharpCode.AvalonEdit;
using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TimsWpfControls_Demo.Model;

namespace TimsWpfControls_Demo.Views
{
    /// <summary>
    /// Interaction logic for ExampleView_Base.xaml
    /// </summary>
    /// 
    [TemplatePart(Name = nameof(PART_XamlTextEditor), Type = typeof(TextEditor))]
    public partial class ExampleView_Base : ContentControl
    {
        private TextEditor PART_XamlTextEditor;

        public static readonly DependencyProperty DemoPropertiesProperty = DependencyProperty.Register("DemoProperties", typeof(ObservableCollection<DemoProperty>), typeof(ExampleView_Base), new PropertyMetadata(null));
        public static readonly DependencyProperty ExampleXamlProperty = DependencyProperty.Register("ExampleXaml", typeof(string), typeof(ExampleView_Base), new PropertyMetadata(null, ExampleXamlChanged));


        public ExampleView_Base()
        {
            DemoProperties = new ObservableCollection<DemoProperty>();
            DemoProperties.CollectionChanged += DemoProperties_CollectionChanged;
        }

        private void DemoProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DemoProperty item in e.NewItems)
                {
                    item.PropertyChanged += (s, e) => 
                    { 
                        if (PART_XamlTextEditor != null)
                            PART_XamlTextEditor.Text = FillExampleXaml(); 
                    };
                }
            }

            if (e.OldItems != null)
            {
                foreach (DemoProperty item in e.OldItems)
                {
                    item.PropertyChanged -= (s, e) => { PART_XamlTextEditor.Text = FillExampleXaml(); };
                }
            }
        }

        public ObservableCollection<DemoProperty> DemoProperties
        {
            get { return (ObservableCollection<DemoProperty>)GetValue(DemoPropertiesProperty); }
            set { SetValue(DemoPropertiesProperty, value); }
        }


        public string ExampleXaml
        {
            get { return (string)GetValue(ExampleXamlProperty); }
            set { SetValue(ExampleXamlProperty, value); }
        }

        private static void ExampleXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExampleView_Base exampleView && exampleView.PART_XamlTextEditor != null)
            {
                exampleView.PART_XamlTextEditor.Text = exampleView.FillExampleXaml();
            }
        }

        private string FillExampleXaml ()
        {
            var result = ExampleXaml;

            if (string.IsNullOrEmpty(result)) return null;

            foreach (var property in DemoProperties)
            {
               result = result.Replace($"[{property.PropertyName}]", property.Value?.ToString());
            }

            return result;
        }

        public void AddDemoProperty(DependencyProperty dependencyProperty, DependencyObject bindingTarget, string groupName = null, DataTemplate template = null, object MinValue = null, object MaxValue = null)
        {
            if (dependencyProperty is null) throw new ArgumentNullException(nameof(dependencyProperty));

            var property = new DemoProperty()
            {
                GroupName = groupName ?? GetGroupName(bindingTarget, dependencyProperty),
                PropertyName = dependencyProperty.Name,
                MinValue = MinValue,
                MaxValue = MaxValue,
                ValueTemplate = template ?? GetBuildInTemplate(dependencyProperty.PropertyType)
            };

            var binding = new Binding(dependencyProperty.Name)
            {
                Source = bindingTarget,
                Mode = BindingMode.TwoWay
            };

            BindingOperations.SetBinding(property, DemoProperty.ValueProperty, binding);

            DemoProperties.Add(property);
        }

        DataTemplate GetBuildInTemplate (Type type)
        {
            if (type == typeof(object))
            {
                return App.Current.Resources["StringDataTemplate"] as DataTemplate;
            }
            else if (type == typeof(double) || 
                type == typeof(int) ||
                type == typeof(float) ||
                type == typeof(byte) ||
                type == typeof(double?) ||
                type == typeof(int?) ||
                type == typeof(float?) ||
                type == typeof(byte?))
            {
               return App.Current.Resources["NumericDataTemplate"] as DataTemplate;
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return App.Current.Resources["BooleanDataTemplate"] as DataTemplate;
            }
            else if(type.IsAssignableFrom(typeof(Brush)))
            {
                return App.Current.Resources["BrushDataTemplate"] as DataTemplate;
            }
            else if (type.IsAssignableFrom(typeof(string)))
            {
                return App.Current.Resources["StringDataTemplate"] as DataTemplate;
            }
            else if (type.IsEnum)
            {
                return App.Current.Resources["EnumDataTemplate"] as DataTemplate;
            }

            // Fallback
            return App.Current.Resources["StringDataTemplate"] as DataTemplate; ;
        }

        private string GetGroupName(DependencyObject target, DependencyProperty Property)
        {
            var attr = target.GetType().GetProperty(Property.Name)?.GetCustomAttribute(typeof(CategoryAttribute)) as CategoryAttribute;
            

            return attr?.Category ?? "Misc";

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_XamlTextEditor = this.GetTemplateChild(nameof(PART_XamlTextEditor)) as TextEditor;
            PART_XamlTextEditor.Text = FillExampleXaml();
        }
    }
}