using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Tasks.Infrastructure.Core.Triggers;

namespace Tasks.Infrastructure.Administration.Library
{
    public interface ITransmissionServiceDescriber
    {
        Type TransmissionInfoType { get; }

        string Name { get; }
        string Summary { get; }

        string DescribeDto(TriggerInfo triggerInfo);
    }

    public interface ITransmissionServiceCreatorViewModel<TTransmissionInfo> where TTransmissionInfo : TriggerInfo
    {
        void Initialize(TTransmissionInfo model);
        ValidationResult Validate();
        TTransmissionInfo Build();
    }

    public interface ITransmissionServiceViewProvider
    {
        UIElement GetView(object transmissionServiceCreatorViewModel);
    }
}