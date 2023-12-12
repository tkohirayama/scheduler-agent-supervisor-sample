﻿global using Azure.Storage.Queues;
global using FluentValidation;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using Ordering.Job.API.Application.Validations;
global using Ordering.Job.Domain.AggregatesModel.OrderingJobAggregates;
global using Ordering.Job.Domain.Exceptions;
global using Ordering.Job.Infrastructure.Repositories;
global using Ordering.Job.Infrastructure;
global using Ordering.Job.Scheduler.Start.Application.Commands;
global using Ordering.Job.Scheduler.Start.Behaviors;
global using Ordering.Job.Scheduler.Start;