using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for Commands in CQRS pattern
    /// Commands represent write operations that modify system state
    /// 
    /// Design Principles:
    /// - Interface Segregation: Separates commands from queries
    /// - Single Responsibility: Commands only modify state
    /// - Command-Query Separation: Clear distinction from queries
    /// 
    /// Usage: Implement this interface for all command operations
    /// Example: CreateBookingCommand, UpdateUserCommand, DeleteVehicleCommand
    /// </summary>
    /// <typeparam name="TResponse">Response type returned after command execution</typeparam>
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }

    /// <summary>
    /// Marker interface for Commands with no response (void commands)
    /// Used when command execution doesn't need to return data
    /// 
    /// Example: SendEmailCommand, LogActivityCommand
    /// </summary>
    public interface ICommand : IRequest
    {
    }

}