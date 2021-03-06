// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

namespace Microsoft.Practices.EnterpriseLibrary.Validation.Validators
{
    /// <summary>
    /// Used to provide a generic API over the unknown validators.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class GenericValidatorWrapper<T> : Validator<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wrappedValidator"></param>
        public GenericValidatorWrapper(Validator wrappedValidator)
            : base(null, null)
        {
            this.WrappedValidator = wrappedValidator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectToValidate"></param>
        /// <param name="currentTarget"></param>
        /// <param name="key"></param>
        /// <param name="validationResults"></param>
        protected override void DoValidate(T objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            this.WrappedValidator.DoValidate(objectToValidate, currentTarget, key, validationResults);
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        protected override string DefaultMessageTemplate
        {
            get { return null; }
        }

        ///<summary>
        /// Returns the validator wrapped by <see cref="GenericValidatorWrapper{T}"/>
        ///</summary>
        public Validator WrappedValidator { get; private set; }
    }
}
