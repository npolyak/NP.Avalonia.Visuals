using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;

namespace NP.Avalonia.Visuals.ThemingAndL10N
{
    public class ThemeLoader : IResourceProvider
    {
        public string? Name { get; set; }

        private ResourceDictionary _resourceDictionary = new ResourceDictionary();

        public IResourceDictionary Loaded => _resourceDictionary;
        public IResourceHost? Owner => Loaded.Owner;

        void IResourceProvider.AddOwner(IResourceHost owner) => Loaded.AddOwner(owner);
        void IResourceProvider.RemoveOwner(IResourceHost owner) => Loaded.RemoveOwner(owner);

        private Uri? _baseUri;
        public ThemeLoader ProvideValue(IServiceProvider serviceProvider)
        {
            _baseUri = ((IUriContext)serviceProvider.GetService(typeof(IUriContext))).BaseUri;
            return this;
        }

        /// <summary>
        /// Gets or sets the source URL.
        /// </summary>
        public Uri? Source { get; set; }

        public bool HasResources => Loaded.HasResources;

        public bool TryGetResource(object key, out object? value)
        {
            return Loaded.TryGetResource(key, out value);
        }

        public T GetResource<T>(object key, T defaultValue = default)
        {
            if (TryGetResource(key, out object result))
            {
                return (T) result;
            }

            return defaultValue;
        }

        public event EventHandler OwnerChanged
        {
            add => Loaded.OwnerChanged += value;
            remove => Loaded.OwnerChanged -= value;
        }

        public AvaloniaDictionary<object, IResourceProvider> ThemeDictionaries { get; } =
            new AvaloniaDictionary<object, IResourceProvider>();

        private IResourceProvider? _selectedDictionary;
        public IResourceProvider? SelectedDictionary 
        {
            get => _selectedDictionary; 
            private set
            {
                if (_selectedDictionary == value)
                    return;

                if (_selectedDictionary != null)
                {
                    _resourceDictionary.MergedDictionaries.Remove(_selectedDictionary);
                }

                _selectedDictionary = value;

                if (_selectedDictionary != null)
                {
                    _resourceDictionary.MergedDictionaries.Add(_selectedDictionary);
                }
            }
        }

        public Uri? BaseUri { get; set; }

        public void AddDictionary(object dictionaryId, Uri uri)
        {
            ResourceDictionary themeDictionary = 
                (ResourceDictionary) AvaloniaXamlLoader.Load(uri, BaseUri);

            ThemeDictionaries.Add(dictionaryId, themeDictionary);
        }

        private object? _selectedDictionaryId;
        public object? SelectedDictionaryId
        {
            get => _selectedDictionaryId;

            set
            {
                if (_selectedDictionaryId == value)
                {
                    return;
                }

                _selectedDictionaryId = value;

                SelectDictionaryImpl(_selectedDictionaryId!);
            }
        }

        private void SelectDictionaryImpl(object dictionaryId)
        {
            if (dictionaryId == null)
            {
                SelectedDictionary = null;
            }
            else if (ThemeDictionaries.TryGetValue(dictionaryId, out IResourceProvider dictionary))
            {
                SelectedDictionary = dictionary;
            }
        }

        private IDisposable? _subscription;
        public ThemeLoader()
        {
            _subscription =
                ThemeDictionaries.AddBehavior(OnThemeDictionaryAdded, OnThemeDictionaryRemoved);
        }

        private void OnThemeDictionaryRemoved(KeyValuePair<object, IResourceProvider> dictInfo)
        {
            
        }

        private void OnThemeDictionaryAdded(KeyValuePair<object, IResourceProvider> dictInfo)
        {
            if (dictInfo.Key == null)
            {
                throw new Exception("ERROR: theme dictionary key cannot be null");
            }

            if (SelectedDictionaryId != null && dictInfo.Key == SelectedDictionaryId)
            {
                SelectDictionaryImpl(SelectedDictionaryId);
            }
        }
    }
}
