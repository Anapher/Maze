using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Orcus.Server.Connection.Tasks.Transmission;

namespace Orcus.Administration.Library.Tasks
{
    public interface ITransmissionServiceDescriber
    {
        Type TransmissionInfoType { get; }

        string Name { get; }
        string Summary { get; }

        string DescribeDto(TransmissionInfo transmissionInfo);
    }

    public interface ITransmissionServiceCreatorViewModel<TTransmissionInfo> where TTransmissionInfo : TransmissionInfo
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