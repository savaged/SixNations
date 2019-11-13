using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    /// <summary>
    /// As of version 2 this is the main way to present a model
    /// object that is selected (usually from an index of model
    /// objects presented using the other main view model the 
    /// IndexViewModel. Both are wired up in XaML. The SelectedItem
    /// will point at an instance of this class. Therefore, the
    /// ModelObject property will typically be set by the XaML
    /// at runtime. However, this class may also be used whilst
    /// the 'object graph' is being presented and in that case
    /// the ModelObject property will be set directly. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectedItemViewModel<T>
        : BaseModelObjectViewModel<T>, 
        ISelectedItemViewModel<T>
        where T : IObservableModel, new()
    {
        private T _modelObject;

        /// <summary>
        /// NOTE: Do not add a constructor parameter for
        /// setting the ModelObject. It will be set on the 
        /// property, either late-bound in XaML or directly
        /// on the property when loading an object graph. 
        /// (See also the class summary).
        /// NOTE: There shouldn't be a need to receive a 
        /// parent object because the ModelObject should
        /// already be a database instance so the API will
        /// be able to find the parent from the db.
        /// </summary>
        /// <param name="commonParams"></param>
        /// <param name="owner"></param>
        /// <param name="dialogViewModel"></param>
        public SelectedItemViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner = null,
            IModelObjectDialogViewModel<T> dialogViewModel = null)
            : base(commonParams, owner, dialogViewModel)
        {
            SubmitCmd = new RelayCommand(OnSubmit, () => CanSubmit);
            ShowCmd = new RelayCommand(OnShow, () => CanShow);
            EditCmd = new RelayCommand(OnEdit, () => CanEdit);
            DeleteCmd = new RelayCommand(OnDelete, () => CanDelete);
            DeselectCmd = new RelayCommand(OnDeselect, () => CanExecute);

            Deselect();

            PropertyChanged += OnPropertyChanged;
        }

        public override void Cleanup()
        {
            PropertyChanged -= OnPropertyChanged;
            base.Cleanup();
        }

        /// <summary>
        /// For navigations
        /// </summary>
        /// <param name="parameter"></param>
        public void Initialise(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            if (parameter is T modelObject)
            {
                ModelObject = modelObject;
            }
            else if (parameter is int modelId)
            {
                SeedModelObject(modelId);
            }
        }

        public void SeedModelObject(int modelId)
        {
            IsDirty = false;
            ModelObject = new T
            {
                Id = modelId,
                IsHeader = true
            };
        }

        public virtual void Seed(IObservableModel parent)
        {
            IsDirty = false;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public virtual void Seed(T modelObject, IObservableModel parent = null)
        {
            IsDirty = false;
            ModelObject = modelObject;
            Parent = parent;
        }

        public void ResetModelObjectToHeader()
        {
            if (!ModelObject.IsNullOrNew())
            {
                ModelObject.IsHeader = true;
            }
        }

        public async override Task<bool> LoadAsync()
        {
            if (!HasFocus) return false;

            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var isNullOrNew = ModelObject.IsNullOrNew();
                var isHeader = ModelObject?.IsHeader == true;
                if (!isNullOrNew && isHeader)
                {
                    var mo = await ModelService.ShowAsync(
                        AuthUser.Current, ModelObject);
                    ModelObject = mo;
                }
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            return true;
        }

        public override void Clear()
        {
            IsDirty = false;
            ModelObject = default;
            Parent = null;
        }

        public ICommand SubmitCmd { get; }

        public ICommand ShowCmd { get; protected set; }

        public ICommand EditCmd { get; protected set; }

        public ICommand DeleteCmd { get; protected set; }

        public ICommand DeselectCmd { get; }

        public override bool CanExecute => base.CanExecute &&
            IsItemSelected;

        public virtual bool CanSubmit => CanExecute
            && ModelObject is ISubmittable;

        public virtual bool CanShow => CanExecute;

        public virtual bool CanEdit => CanExecute;

        public virtual bool CanDelete => CanExecute && 
            !ModelObject.IsNullOrNew();


        public T ModelObject
        {
            get => _modelObject;
            set
            {
                if (!Equals(_modelObject, value))
                {
                    if (value == null)
                    {
                        _modelObject.PropertyChanged -=
                            OnModelObjectPropertyChanged;
                    }
                    Set(ref _modelObject, value);                 

                    if (_modelObject != null)
                    {
                        _modelObject.PropertyChanged +=
                            OnModelObjectPropertyChanged;
                    }
                    RaisePropertyChanged(nameof(IsItemSelected));
                }
            }
        }

        public bool IsItemSelected => ModelObject != null && 
            !ModelObject.Equals(default(T));

        public virtual bool CanCopyModelObjectToClipboard =>
            CanExecute;

        protected virtual async Task<bool> FormDelete()
        {
            var result = false;

            if (ModelObject.IsNullOrNew()) return result;

            if (!ConfirmDelete()) return result;

            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var old = ModelObject.Clone();

                await ModelService.DeleteAsync(
                    AuthUser.Current, ModelObject);

                ModelObject = default;

                MessengerInstance.Send(
                    new ModelObjectPersistedMessage<T>(
                        this, 
                        Owner as IOwnedFocusable, 
                        old,
                        ModelObject,
                        Parent,
                        true));

                result = true;
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            return result;
        }

        protected virtual async Task Submit()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                if (ModelObject is ISubmittable submittable)
                {
                    var old = ModelObject.Clone();
                    var tmp = ModelObject.Clone();
                    ((ISubmittable)tmp).ToggleSubmitted();

                    var updated = await ModelService.UpdateAsync(
                        AuthUser.Current, tmp);

                    if (updated == null)
                    {
                        throw new InvalidOperationException(
                            "The API failed to confirm submit!");
                    }
                    if (old.Equals(updated))
                    {
                        throw new InvalidOperationException(
                            "The submit action failed to change " +
                            nameof(submittable.IsSubmitted));
                    }
                    submittable.ToggleSubmitted();
                    ModelObject.Id = updated.Id;

                    MessengerInstance.Send(
                        new ModelObjectPersistedMessage<T>(
                            this,
                            Owner as IOwnedFocusable,
                            old,
                            ModelObject,
                            Parent));
                }
            }
            catch (ApiValidationException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async void OnSubmit()
        {
            await Submit();
        }

        private void OnEdit()
        {
            if (DialogViewModel == null)
            {
                throw new InvalidOperationException(
                    $"The {nameof(DialogViewModel)} should be set by here!");
            }
            ShowDialog(ModelObject);
        }

        private void Deselect()
        {
            ModelObject = default;
        }

        private void OnShow()
        {
            ShowDialog(ModelObject);
        }

        private void OnDeselect()
        {
            Deselect();
        }

        private async void OnDelete()
        {
            await FormDelete();
        }

        private void OnModelObjectPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) &&
                e.PropertyName != nameof(ModelObject.IsUnlocked))
            {
                IsDirty = true;
            }
        }

        private void OnPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (HasFocus && e.PropertyName == nameof(IsItemSelected))
            {
                if (IsItemSelected)
                {
                    if (!ModelObject.IsHeader)
                    {
                        MessengerInstance.Send(
                            new ModelObjectSelectedMessage<T>(
                                this, ModelObject));
                    }
                }
            }
        }

    }
}
