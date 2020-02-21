using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;

namespace FluentValidation
{
    /// <summary>
    /// Add Fluent Validator support to an EditContext.
    /// </summary>
    public class FluentValidator : ComponentBase
    {
        private ValidationMessageStore _messageStore;

        /// <summary>
        /// Inherited object from the FormEdit component.
        /// </summary>
        [CascadingParameter]
        EditContext CurrentEditContext { get; set; }

        /// <summary>
        /// Enable access to the ASP.NET Core Service Provider / DI.
        /// </summary>
        [Inject]
        IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// The AbstractValidator object for the corresponding form Model object type.
        /// </summary>
        [Parameter]
        public IValidator Validator { set; get; }

        /// <summary>
        /// Attach to parent EditForm context enabling validation.
        /// </summary>
        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException($"{nameof(DataAnnotationsValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(DataAnnotationsValidator)} " +
                    $"inside an EditForm.");
            }

            if (Validator == null)
            {
                GetValidator();
            }

            AddValidation();
        }


        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            if (errors == null || errors.Count == 0)
                return;

            foreach (var err in errors)
            {
                _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }


        /// <summary>
        /// Try acquiring the form validator implementation from the DI.
        /// </summary>
        private void GetValidator()
        {
            var validatorType = typeof(IValidator<>);
            var formType = CurrentEditContext.Model.GetType();
            var formValidatorType = validatorType.MakeGenericType(formType);

            Validator = ServiceProvider.GetService(formValidatorType) as IValidator;

            if (Validator == null)
            {
                throw new InvalidOperationException($"FluentValidation.IValidator<{formType.FullName}> is"
                    + " not registered in the application service provider.");
            }
        }

        /// <summary>
        /// Add form validation logic handlers.
        /// </summary>
        private void AddValidation()
        {
            _messageStore = new ValidationMessageStore(CurrentEditContext);

            // Perform object-level validation on request
            CurrentEditContext.OnValidationRequested +=
                (sender, eventArgs) => ValidateModel((EditContext)sender, _messageStore);

            // Perform per-field validation on each field edit
            CurrentEditContext.OnFieldChanged +=
                (sender, eventArgs) => ValidateField(CurrentEditContext, _messageStore, eventArgs.FieldIdentifier);
        }

        /// <summary>
        /// Validate the whole form and trigger client UI update.
        /// </summary>
        /// <param name="editContext"></param>
        /// <param name="messages"></param>
        private void ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            // ATTENTION: DO NOT USE Async Void + ValidateAsync
            // Explanation: Blazor UI will get VERY BUGGY for some reason if you do that. (Field CSS lagged behind validation)
            var validationResults = Validator.Validate(editContext.Model);

            messages.Clear();

            foreach (var error in validationResults.Errors)
            {
                var fieldID = editContext.Field(error.PropertyName);
                messages.Add(fieldID, error.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        /// <summary>
        /// Validate a single field and trigger client UI update.
        /// </summary>
        /// <param name="editContext"></param>
        /// <param name="messages"></param>
        /// <param name="fieldIdentifier"></param>
        private void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
        {
            var vselector = new Internal.MemberNameValidatorSelector(new[] { fieldIdentifier.FieldName });
            var vctx = new ValidationContext(editContext.Model, new Internal.PropertyChain(), vselector);
            var validationResults = Validator.Validate(vctx);

            messages.Clear(fieldIdentifier);

            foreach (var error in validationResults.Errors)
            {
                var fieldID = editContext.Field(error.PropertyName);
                messages.Add(fieldID, error.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }
    }
}
