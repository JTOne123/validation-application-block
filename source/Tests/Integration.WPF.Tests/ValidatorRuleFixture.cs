// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SWC = System.Windows.Controls;

namespace Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WPF.Tests
{
    [TestClass]
    public class ValidatorRuleFixture
    {
        [TestInitialize]
        public void TestInitialize()
        {
            using (var configSource = new SystemConfigurationSource(false))
            {
                ValidationFactory.SetDefaultConfigurationValidatorFactory(configSource);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ValidationFactory.Reset();
        }

        [STATestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreatingValidatorRuleWithNullBindingExpressionThrows()
        {
            new ValidatorRule(null);
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyInvalidStringValidatedProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedStringProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "bbbb";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string"));
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyStringValidatedProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedStringProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "aaaaaa";

            Assert.IsFalse(SWC.Validation.GetHasError(textBox));
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyInvalidConvertedValidatedProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedIntProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "1";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid int"));
        }

        [STATestMethod]
        public void NonConvertedValueIsNotValidatedForConvertedValidatedProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding =
                new Binding("ValidatedIntProperty")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnExceptions = true    // necessary for the conversion error to become an error!
                };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "aaaa";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsFalse(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid int"));
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyConvertedValidatedProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedIntProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "15";

            Assert.IsFalse(SWC.Validation.GetHasError(textBox));
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyConvertedNonValidatedProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("NonValidatedProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "15";

            Assert.IsFalse(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(15, instance.NonValidatedProperty);
        }

        [STATestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SettingSourceTypeToNullThrows()
        {
            new ValidatorRule { SourceType = null };
        }

        [STATestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SettingSourceTypeOnARuleInitializedWithABindingExpressionThrows()
        {
            var textBox = new TextBox();
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, new Binding("NonValidatedProperty"));
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);

            new ValidatorRule(bindingExpression) { SourceType = typeof(ValidatedObject) };
        }

        [STATestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SettingSourcePropertyNameToNullThrows()
        {
            new ValidatorRule { SourcePropertyName = null };
        }

        [STATestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SettingSourcePropertyNameOnARuleInitializedWithABindingExpressionThrows()
        {
            var textBox = new TextBox();
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, new Binding("NonValidatedProperty"));
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);

            new ValidatorRule(bindingExpression) { SourcePropertyName = "NonValidatedProperty" };
        }

        [STATestMethod]
        public void CanValidateWithExplicitlySetTypeAndProperty()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedStringProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            binding.ValidationRules.Add(
                new ValidatorRule
                {
                    SourceType = typeof(ValidatedObject),
                    SourcePropertyName = "ValidatedStringProperty"
                });

            textBox.Text = "bbbb";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string"));
        }

        [STATestMethod]
        public void ValidatingWithARuleWithNoTypeSetThrows()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedStringProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            binding.ValidationRules.Add(
                new ValidatorRule
                {
                    SourcePropertyName = "ValidatedStringProperty"
                });

            try
            {
                textBox.Text = "bbbb";
                Assert.Fail("should have thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [STATestMethod]
        public void ValidatingWithARuleWithNoSourcePropertySetThrows()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedStringProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            binding.ValidationRules.Add(
                new ValidatorRule
                {
                    SourceType = typeof(ValidatedObject)
                });

            try
            {
                textBox.Text = "bbbb";
                Assert.Fail("should have thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [STATestMethod]
        public void ValidatingWithARuleWithInvalidSourcePropertySetThrows()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("ValidatedStringProperty") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            binding.ValidationRules.Add(
                new ValidatorRule
                {
                    SourceType = typeof(ValidatedObject),
                    SourcePropertyName = "Invalid"
                });

            try
            {
                textBox.Text = "bbbb";
                Assert.Fail("should have thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }


        [STATestMethod]
        public void ValidatesBindingForPropertyInvalidStringValidatedPropertyWithMultipleSources()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding =
                new Binding("MultipleSourceValidatedStringProperty")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(
                new ValidatorRule(bindingExpression)
                {
                    ValidationSpecificationSource = ValidationSpecificationSource.All
                });

            textBox.Text = "bbbb";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string: vab"));
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string: data annotations"));
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyInvalidStringValidatedPropertyWithFilteredSources()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding =
                new Binding("MultipleSourceValidatedStringProperty")
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(
                new ValidatorRule(bindingExpression)
                {
                    ValidationSpecificationSource = ValidationSpecificationSource.Attributes
                });

            textBox.Text = "bbbb";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string: vab"));
            Assert.IsFalse(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string: data annotations"));
        }


        [STATestMethod]
        public void ValidatesBindingForPropertyInvalidStringValidatedPropertyUsingDefaultRuleset()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("MultipleRulesetValidatedStringProperty")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression));

            textBox.Text = "bbbb";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string default"));
            Assert.IsFalse(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string ruleset"));
        }

        [STATestMethod]
        public void ValidatesBindingForPropertyInvalidStringValidatedPropertyUsingNonDefaultRuleset()
        {
            var instance = new ValidatedObject { };

            var textBox = new TextBox();
            textBox.DataContext = instance;
            var binding = new Binding("MultipleRulesetValidatedStringProperty")
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
            var bindingExpression = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
            binding.ValidationRules.Add(new ValidatorRule(bindingExpression) { RulesetName = "A" });

            textBox.Text = "bbbb";

            Assert.IsTrue(SWC.Validation.GetHasError(textBox));
            Assert.AreEqual(1, SWC.Validation.GetErrors(textBox).Count);
            Assert.IsFalse(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string default"));
            Assert.IsTrue(SWC.Validation.GetErrors(textBox)[0].ErrorContent.ToString().Contains("invalid string ruleset"));
        }
    }
}
