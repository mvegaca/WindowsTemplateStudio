// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Templates.UI.ViewModels.Common;

namespace Microsoft.Templates.UI.Controls
{
    public class StepData : Selectable
    {
        private bool _completed;
        private int _index;
        private string _stepText;

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        public string Title { get; }

        public Func<object> GetPage { get; }

        public bool Completed
        {
            get => _completed;
            set => SetProperty(ref _completed, value);
        }

        public string StepText
        {
            get => _stepText;
            set => SetProperty(ref _stepText, value);
        }

        public string Id { get; set; }

        public bool IsSubStep { get; private set; }

        private StepData(string stepId, string title, Func<object> getPage, bool completed = false, bool isSelected = false)
            : base(isSelected)
        {
            Id = stepId;
            Title = title;
            GetPage = getPage;
            Completed = completed;
        }

        public static StepData MainStep(string stepId, int index, string title, Func<object> getPage, bool completed = false, bool isSelected = false)
        {
            return new StepData(stepId, title, getPage, completed, isSelected)
            {
                Index = index,
                IsSubStep = false,
            };
        }

        public static StepData SubStep(string stepId, int index, string title, Func<object> getPage, bool completed = false, bool isSelected = false)
        {
            return new StepData(stepId, title, getPage, completed, isSelected)
            {
                Index = index,
                IsSubStep = true,
            };
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case StepData step:
                    return Index == step.Index;
                case int index:
                    return Index.Equals(index);
                case Type type:
                    return type.Equals(GetPage().GetType());
                default:
                    return base.Equals(obj);
            }
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
